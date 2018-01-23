using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Collections;
using UnityEngine;
using SMI;
using Distraction;

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
			"ActiveTask", "TaskState",
			"Active Gaze Object Name", "Gaze Object Position", "Distance To Active Gaze Object",
			"Current Distraction Object", "Current Distraction Name", "Duration Distracted",
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
		public TaskSelector taskSelector;
		public string activeTaskName;
		public string currentTaskState;

		public RowData (DateTime startTime) {
			this.startTime = startTime;
			this.eye = new EyeData();
		}

		public void update() {
			messageNumber++;
			snapshotTime = DateTime.Now;
			timeSinceStart = snapshotTime - startTime;
			eye.Update();
			this.playerHeadPosition = playersTransform != null ? playersTransform.position : Vector3.zero;
			this.playerRotation = playersTransform != null ? playersTransform.eulerAngles : Vector3.zero;
			this.gazePoint = GetGazeObjectsPosition();
			this.gazeObjectName = GetHitObjectName();
			this.distanceToObject = GetHitObjectDistance();
			this.currentDistraction = distractionController.GetCurrentDistraction();

			var activeTask = taskSelector.activeTask;
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

		private string toCsvCell(object o) {
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

		private string toRow(List<object> l) {
			return String.Join(COLUMN_DELIM, l.Select(x => toCsvCell(x)).ToArray());
		}

		public string ToRow(Event evt) {
			EventType eType = EventType.Standard;
			if (currentDistraction != null && gazeObjectName == currentDistraction.gameObject.name) {
				eType = EventType.Distracted;
			}
			return toRow(new List<object>{
				messageNumber,
				snapshotTime.ToString("MM/dd/yyyy hh:mm:ss.fffff"), timeSinceStart.TotalMilliseconds,
				eType,
				activeTaskName, currentTaskState,
				gazeObjectName, gazePoint, distanceToObject,
				currentDistraction.gameObject.name, currentDistraction.distractionName, evt.duration,
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

	public string filename;
	public bool recordingEnabled;

	// SMI data
	private readonly System.Object rowDataLock = new System.Object ();
	private RowData latestData;

	// Messages
	private readonly ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

	// Output
	private CSVWriter outputFile; // Computed on Start() from `filename`


	// Timers
	private Timer standardGazeTimer;
	private Timer fileWriteTimer;

	public void AddEvent(Event t) {
		if (!recordingEnabled) {
			return;
		}
		try {
			string row;
			lock (rowDataLock) {
				row = latestData.ToRow(Event.Standard);
			}
			messageQueue.Enqueue(row);
		} catch (Exception e) {
			messageQueue.Enqueue(e.Message + "," + e.TargetSite);
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

	void Start() {
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

			var taskControllerObj = GameObject.Find("TaskController");
			if (taskControllerObj != null) {
				latestData.taskSelector = taskControllerObj.GetComponent<TaskSelector>();
			}
		}
		
		this.outputFile = CSVWriter.NewOutputFile(filename);

		messageQueue.Enqueue(RowData.HEADER);

		var noDelay = new TimeSpan(0, 0, 0);

		var standardGazeCallback = new TimerCallback(delegate(object state) { AddEvent(Event.Standard); });
		var standardGazeInterval = new TimeSpan(0, 0, 0, 0, 4); // Every 4 milliseconds
		this.standardGazeTimer = new Timer(standardGazeCallback, null, noDelay, standardGazeInterval);
		print("Started interval eye tracker. Schedule: every " + standardGazeInterval.ToString());

		var fileWriteCallback = new TimerCallback(delegate(object state) { WriteDataToFile(); });
		var fileWriteInterval = new TimeSpan(0, 0, 10); // Every 10 seconds
		this.fileWriteTimer = new Timer(fileWriteCallback, null, noDelay, fileWriteInterval);
		print("Started file writer. Schedule: every " + fileWriteInterval.ToString());
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
}

