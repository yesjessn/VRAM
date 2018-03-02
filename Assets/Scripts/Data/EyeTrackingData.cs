using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Collections;
using UnityEngine;
using SMI;
using Distraction;
using Subject;

// This sets up two threads:
// 1) Poll eye tracking data and task state every 4ms and enqueue the results
// 2) Drain the queue and write all entries to the output file every 10 seconds
public class EyeTrackingData : MonoBehaviour {
	class EyeData {

		// Eye data
		public Vector2 binocularPor;
		public Vector3 cameraRaycast;
		public float ipd;
		public Vector2 leftPor;
		public Vector2 rightPor;
		public Vector3 leftBasePoint;
		public Vector3 rightBasePoint;
		public Vector3 leftGazeDirection;
		public Vector3 rightGazeDirection;

		
		public void Update() {
			var smi = SMI.SMIEyeTrackingUnity.Instance;
			if (smi != null) {
				binocularPor = smi.smi_GetBinocularPor ();
				cameraRaycast = smi.smi_GetCameraRaycast ();
				ipd = smi.smi_GetIPD ();
				leftPor = smi.smi_GetLeftPor ();
				rightPor = smi.smi_GetRightPor ();
				leftBasePoint = smi.smi_GetLeftGazeBase ();
				rightBasePoint = smi.smi_GetRightGazeBase ();
				leftGazeDirection = smi.smi_GetLeftGazeDirection ();
				rightGazeDirection = smi.smi_GetRightGazeDirection ();
			} else {
				binocularPor = Vector2.zero;
				cameraRaycast = Vector3.zero;
				ipd = 0f;
				leftPor = Vector2.zero;
				rightPor = Vector2.zero;
				leftBasePoint = Vector3.zero;
				rightBasePoint = Vector3.zero;
				leftGazeDirection = Vector3.zero;
				rightGazeDirection = Vector3.zero;
			}
		}
	}

	class RowData {
		public static string COLUMN_DELIM = ",";
		// static string HEADER = "TimeStamp,Time Since Start,Player Head Position,Player Rotation Euler,Binocular POR," +
		// 	"Camera Raycast Direction,Interpupillary Distance,Left Eye POR, Right Eye POR," + 
		// 	"Left Eye Base Position,Right Eye Base Position,Left Eye Gaze Direction,Right Eye Gaze Direction," +
		// 	"Gaze Object Position,Type Of Message,DistractionName,Duration Distracted,Active Gaze Object Name,Distance To Active Gaze Object,# Of Data Entries";

		public static string HEADER = String.Join(COLUMN_DELIM, new string[] {
			"Entry #",
			"Timestamp", "Time Since Start",
			"Type Of Message",
			"Active Task", "Task State",
			"Active Gaze Object Name", "Gaze Object Position", "Distance To Active Gaze Object",
			"Current Distraction Object", "Current Distraction Name", 
			"Is Distracted", "Duration Distracted",
			"Player Head Position", "Player Rotation Euler",
			"Binocular POR", "Camera Raycast Direction", "Interpupillary Distance",
			"Left Eye POR", "Right Eye POR",
			"Left Eye Base Position", "Right Eye Base Position",
			"Left Eye Gaze Direction", "Right Eye Gaze Direction",
		});

		private int messageNumber = -1;
		public readonly DateTime startTime;
		DateTime snapshotTime;
		TimeSpan timeSinceStart;
		public EyeData eye;
		public Transform playersTransform = null;
		public GameObject hitObject;

		// Game state data, updated every Update()
		private Vector3 playerHeadPosition;
		private Vector3 playerRotation;
		private Vector3 gazePoint;
		private string gazeObjectName;
		private float distanceToObject = 0.0f;

		// Distractions
		public DistractionController distractionController;
		public Distraction.Distraction currentDistraction;

		// Task
		public string activeTaskName;
		public string currentTaskState;

		public RowData (DateTime startTime) {
			this.startTime = startTime;
			this.eye = new EyeData();
		}

		public void update() {
			snapshotTime = DateTime.Now;
			timeSinceStart = snapshotTime - startTime;
			eye.Update();
			this.playerHeadPosition = playersTransform != null ? playersTransform.position : Vector3.zero;
			this.playerRotation = playersTransform != null ? playersTransform.eulerAngles : Vector3.zero;
			this.gazePoint = GetGazeObjectsPosition();
			this.gazeObjectName = GetHitObjectName();
			this.distanceToObject = GetHitObjectDistance();
			this.currentDistraction = distractionController.GetCurrentDistraction();

			var activeTask = TaskList.instance.GetActiveTask();
			this.activeTaskName = activeTask != null ? activeTask.name : "";
			this.currentTaskState = activeTask != null ? activeTask.GetCurrentState() : "";
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

		private static string toCsvCell(object o) {
			if (o == null) {
				return "";
			}
			var s = o.ToString();
			if (s.Contains(",")) {
				if (s.Contains("\"")) {
					s.Replace("\"", "\\\"");
				}
				return "\"" + s + "\"";
			}
			return s;
		}

		private static string toRow(List<object> l) {
			return String.Join(COLUMN_DELIM, l.Select(x => toCsvCell(x)).ToArray());
		}

		public string ToRow(Event evt) {
			messageNumber++;
			if (evt == null) {
				evt = Event.Standard;
			}
			EventType eType = evt.type;
			switch (eType) {
				case EventType.Distracted:
					eType = EventType.Standard;
					break;
				case EventType.EnterDistraction:
					eType = EventType.EnterGaze;
					break;
				case EventType.ExitDistraction:
					eType = EventType.ExitGaze;
					break;
			}
			var currentDistractionObj = currentDistraction != null ? currentDistraction.distractionObjectName : "";
			var currentDistractionName = currentDistraction != null ? currentDistraction.distractionName : "";
			var isDistracted = currentDistraction != null && gazeObjectName == currentDistraction.distractionObjectName;
			return toRow(new List<object>{
				messageNumber,
				snapshotTime.ToString("MM/dd/yyyy hh:mm:ss.fffff"), timeSinceStart.TotalMilliseconds,
				eType,
				activeTaskName, currentTaskState,
				gazeObjectName, gazePoint, distanceToObject,
				currentDistractionObj, currentDistractionName,
				isDistracted, evt.duration,
				playerHeadPosition, playerRotation,
				eye.binocularPor, eye.cameraRaycast, eye.ipd,
				eye.leftPor, eye.rightPor,
				eye.leftBasePoint, eye.rightBasePoint,
				eye.leftGazeDirection, eye.rightGazeDirection,
			});
		}
	}

	public enum EventType {
		Standard,
		EnterDistraction,
		Distracted,
		ExitDistraction,
		EnterGaze,
		ExitGaze,
	}

	public class Event {
		public EventType type;
		public string distractionName;
		public string nameOverride;
		public float duration;

		public Event(EventType type)
			: this(type, null, null, 0f) {}

		public Event(EventType type, string distractionName, string nameOverride)
			: this(type, distractionName, nameOverride, 0f) {}

		public Event(EventType type, string distractionName, string nameOverride, float duration) {
			this.type = type;
			this.distractionName = distractionName;
			this.nameOverride = nameOverride;
			this.duration = duration;
		}
		
		public static readonly Event Standard = new Event(EventType.Standard);
		public static readonly Event EnterGaze = new Event(EventType.EnterGaze);
		public static readonly Event ExitGaze = new Event(EventType.ExitGaze);
	}

	static TimeSpan NO_DELAY = new TimeSpan(0, 0, 0);

	public static EyeTrackingData instance;

	public string filename;
	public bool recordingEnabled;

	// SMI data
	private readonly System.Object rowDataLock = new System.Object ();
	private RowData latestData;

	// Messages
	private readonly ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

	// Output
	private CSVWriter outputFile; // Computed on OnEnable() from `filename`


	// Timers
	private Timer standardGazeTimer;
	private Timer fileWriteTimer;

	public void AddEvent(Event t) {
		if (!recordingEnabled) {
			return;
		}
		try {
			lock (rowDataLock) {
				if (latestData != null) {
					messageQueue.Enqueue(latestData.ToRow(t));
				} else {
					UnityEngine.Debug.LogError(String.Format("RowData not yet initialized,Event: {0}", t.type));
				}
			}
		} catch (Exception e) {
			var frames = new StackTrace(e, true).GetFrames();
			var frame = frames.Length > 1 ? frames[frames.Length - 2] : frames.Last();
			var locInfo = String.Format("\"{0}:{1}({2},{3})\"", frame.GetFileName(), frame.GetMethod().Name, frame.GetFileLineNumber(), frame.GetFileColumnNumber());
			UnityEngine.Debug.LogError(e.GetType().Name + ": " + e.Message + " @ " + e.TargetSite + " - " + locInfo);
		}
	}

	private void WriteDataToFile() {
		StringBuilder sb = new StringBuilder();
		string row;
		int count = 0;
		while (messageQueue.TryDequeue(out row)) {
			sb.AppendLine(row);
			count++;
		}
		if (sb.Length > 0) {
			outputFile.Write(sb.ToString());
			print("Wrote " + count + " lines");
		}
	}

	public void SetHitObject(GameObject obj) {
		lock (rowDataLock) {
			latestData.hitObject = obj;
		}
	}

	public void ClearHitObject() {
		lock (rowDataLock) {
			latestData.hitObject = null;
		}
	}

	void OnEnable() {
		var startTime = DateTime.Now;
		
		lock (rowDataLock) {
			latestData = new RowData(startTime);
			if(latestData.playersTransform == null) {
				latestData.playersTransform = GameObject.Find("SMI_CameraWithEyeTracking").transform;
			}
			var distractionObj = GameObject.Find("DistractionController");
			if (distractionObj != null) {
				latestData.distractionController = distractionObj.GetComponent<DistractionController>();
			}

			latestData.update();
		}


		var standardGazeCallback = new TimerCallback(delegate(object state) { AddEvent(Event.Standard); });
		var standardGazeInterval = new TimeSpan(0, 0, 0, 0, 4); // Every 4 milliseconds
		this.standardGazeTimer = new Timer(standardGazeCallback, null, NO_DELAY, standardGazeInterval);
		print("Started interval eye tracker. Schedule: every " + standardGazeInterval.ToString());
	}

	void OnDisable() {
		this.standardGazeTimer.Dispose();
	}

	void Awake() {
        if(instance != null) {
            Destroy(this.gameObject);
        } else {
            instance = this;
			outputFile = CSVWriter.NewOutputFile(FindObjectOfType<SubjectDataHolder>(), filename);
			messageQueue.Enqueue(RowData.HEADER);

			var fileWriteCallback = new TimerCallback(delegate(object state) { WriteDataToFile(); });
			var fileWriteInterval = new TimeSpan(0, 0, 10); // Every 10 seconds
			this.fileWriteTimer = new Timer(fileWriteCallback, null, NO_DELAY, fileWriteInterval);
			print("Started file writer. Schedule: every " + fileWriteInterval.ToString());
		}
	}

	void OnDestroy() {
		outputFile.Close();
		fileWriteTimer.Dispose();
	}

	void Update() {
		lock (rowDataLock) {
			latestData.update();
		}
	}
}

