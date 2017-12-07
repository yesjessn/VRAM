using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Collections;
using UnityEngine;
using SMI;

public class EyeTrackingData : MonoBehaviour {
	static string COLUMN_DELIM = ",";
	static string HEADER = "TimeStamp,Time Since Start,Player Head Position,Player Rotation Euler,Binocular POR," +
		"Camera Raycast Direction,Interpupillary Distance,Left Eye POR, Right Eye POR," + 
		"Left Eye Base Position,Right Eye Base Position,Left Eye Gaze Direction,Right Eye Gaze Direction," +
		"Gaze Object Position,Type Of Message,DistractionType,Duration Distracted,Active Gaze Object Name,Distance To Active Gaze Object,# Of Data Entries";

	class EyeData {
		public readonly DateTime startTime;

		// Eye data
		DateTime snapshotTime;
		TimeSpan timeSpan;
		Vector2 binocularPor;
		Vector3 cameraRaycast;
		float ipd;
		Vector2 leftPor;
		Vector2 rightPor;
		Vector3 leftBasePoint;
		Vector3 rightBasePoint;
		Vector3 leftGazeDirection;
		Vector3 rightGazeDirection;

		public EyeData (DateTime startTime) {
			this.startTime = startTime;
		}
		
		public void Update() {
			snapshotTime = DateTime.Now;
			timeSpan = snapshotTime - startTime;
			binocularPor = SMI.SMIEyeTrackingUnity.Instance.smi_GetBinocularPor ();
			cameraRaycast = SMI.SMIEyeTrackingUnity.Instance.smi_GetCameraRaycast ();
			ipd = SMI.SMIEyeTrackingUnity.Instance.smi_GetIPD ();
			leftPor = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftPor ();
			rightPor = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightPor ();
			leftBasePoint = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftGazeBase ();
			rightBasePoint = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightGazeBase ();
			leftGazeDirection = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftGazeDirection ();
			rightGazeDirection = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightGazeDirection ();
		}

		public List<object> GetColumns(Vector3 playerHeadPosition, Vector3 playerRotation, Vector3 gazePoint) {
			return new List<object>{
				snapshotTime.ToString("MM/dd/yyyy hh:mm:ss.fffff"),
				timeSpan.Hours + "-" + timeSpan.Minutes + "-" + timeSpan.Seconds + "-" + timeSpan.Milliseconds,
				playerHeadPosition,
				playerRotation,
				binocularPor,
				cameraRaycast,
				ipd,
				leftPor,
				rightPor,
				leftBasePoint,
				rightBasePoint,
				leftGazeDirection,
				rightGazeDirection,
				gazePoint,
			};
		}
	}

	public enum EventType {
		Standard,
		EnterDistraction,
		ExitDistraction,
		EnterGaze,
		ExitGaze,
	}

	public class Event {
		public EventType type;
		public string distractionName;
		public string nameOverride;
		public float Duration;

		public Event(EventType type)
			: this(type, "", "", 0f) {}

		public Event(EventType type, string distractionName, string nameOverride)
			: this(type, distractionName, nameOverride, 0f) {}

		public Event(EventType type, string distractionName, string nameOverride, float Duration) {
			this.type = type;
			this.distractionName = distractionName;
			this.nameOverride = nameOverride;
			this.Duration = Duration;
		}
		
		public static readonly Event Standard = new Event(EventType.Standard);
		public static readonly Event EnterGaze = new Event(EventType.EnterGaze);
		public static readonly Event ExitGaze = new Event(EventType.ExitGaze);
	}

	public Transform playersTransform = null;
	public GameObject hitObject;
	public string filename;
	public bool recordingEnabled;

	// SMI data
	private readonly System.Object eyeDataLock = new System.Object ();
	private EyeData latestData;

	// Messages
	private readonly ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
	private int messageNumber = -1;

	// Output
	private CSVWriter outputFile; // Computed on Start() from `filename`

	// Game state data, updated every Update()
    private Vector3 playerHeadPosition;
    private Vector3 playerRotation;
    private Vector3 gazePoint;
    private string gazeObjectName;
    private float distanceToObject = 0.0f;

	// Timers
	private Timer standardGazeTimer;
	private Timer fileWriteTimer;

	private int nextMessageNum() {
		return Interlocked.Increment (ref messageNumber);
	}
	
    private string GetHitObjectName() {
        if (hitObject != null) {
            return hitObject.name;
        }
    	return "Null";
    }

    private float GetHitObjectDistance() {
        if (hitObject != null && playerHeadPosition != null) {
            return Vector3.Distance(hitObject.transform.position, playerHeadPosition);
        }
		return 0f;
    }

	private Vector3 GetGazeObjectsPosition() {
		if (hitObject != null) {
			return hitObject.transform.position;
		}

		return Vector3.zero;
	}

	private string toCsvCell(object o) {
		var s = o.ToString();
		if (s.Contains(",")) {
			if (s.Contains("\"")) {
				s.Replace("\"", "\\\"");
			}
			return "\"" + s + "\"";
		}
		return s;
	}

	private string toRow(List<object> l) {
		return String.Join(",", l.Select(x => toCsvCell(x)).ToArray());
	}

	private string MakeStandardRow() {
		lock (eyeDataLock) {
			latestData.Update();
			// Standard row uses most recent values per Update()
			var cols = latestData.GetColumns(playerHeadPosition, playerRotation, gazePoint);
			cols.AddRange(new object[] {
				EventType.Standard,
				"", // DistractionType
				"", // Duration Distracted
				"", // Active Gaze Object Name
				"", // Distance to Active Gaze Object
				nextMessageNum(),
			});
			return toRow(cols);
		}
	}

	private string MakeEventRow(Event t) {
		lock (eyeDataLock) {
			latestData.Update();
			var cols = latestData.GetColumns(playersTransform.position, playersTransform.eulerAngles, GetGazeObjectsPosition());
			cols.AddRange(new object[] {
				t.type,
				t.distractionName,
				t.Duration > 0 ? t.Duration.ToString() : "",
				GetHitObjectName(),
				GetHitObjectDistance(),
				nextMessageNum(),
			});
			return toRow(cols);
		}
	}

	public void AddEvent(Event t) {
		if (!recordingEnabled) {
			return;
		}
		switch (t.type) {
			case EventType.Standard:
				messageQueue.Enqueue(MakeStandardRow());
				break;
			default:
				messageQueue.Enqueue(MakeEventRow(t));
				break;
		}
	}

	private void WriteDataToFile() {
		StringBuilder sb = new StringBuilder();
		string row;
		while (messageQueue.TryDequeue(out row)) {
			sb.AppendLine(row);
		}
		if (sb.Length > 0) {
			outputFile.Write(sb.ToString());
		}
	}

	void Start() {
		var startTime = DateTime.Now;
		
		lock (eyeDataLock) {
			latestData = new EyeData(startTime);
		}
		
		this.outputFile = CSVWriter.NewOutputFile(filename);

		messageQueue.Enqueue(HEADER);

		
        if(playersTransform == null) {
            playersTransform = GameObject.Find("SMI_CameraWithEyeTracking").transform;
        }

		var noDelay = new TimeSpan(0, 0, 0);

		var standardGazeCallback = new TimerCallback(delegate(object state) { AddEvent(Event.Standard); });
		var standardGazeInterval = new TimeSpan(0, 0, 0, 0, 4); // Every 4 milliseconds
		this.standardGazeTimer = new Timer(standardGazeCallback, null, noDelay, standardGazeInterval);

		var fileWriteCallback = new TimerCallback(delegate(object state) { WriteDataToFile(); });
		var fileWriteInterval = new TimeSpan(0, 0, 10); // Every 10 seconds
		this.fileWriteTimer = new Timer(fileWriteCallback, null, noDelay, fileWriteInterval);
	}

	void Update() {
        this.playerHeadPosition = playersTransform.position;
        this.playerRotation = playersTransform.eulerAngles;
        this.gazePoint = GetGazeObjectsPosition();
        this.gazeObjectName = GetHitObjectName();
        this.distanceToObject = GetHitObjectDistance();
	}
}

