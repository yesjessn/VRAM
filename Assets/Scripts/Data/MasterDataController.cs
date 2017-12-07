using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using System.Threading;
using SMI;

public class MasterDataController : MonoBehaviour {

    public static MasterDataController instance;
    public string exportFileName = "TestEyeData";
    public int dataPoints = 0;
    public float timeInApp = 0.0f;
    public bool canRecordData = true;
    private bool _toggleValidationWindow = false;
    private bool _doOnce = true;
    public GameObject hitObject;
    public Transform playersTransform;
    public DateTime startTime;
    private Vector3 playerHeadPosition;
    private Vector3 playerRotation;
    private Vector3 gazePoint;
    private string gazeObjectName;
    private float distanceToObject = 0.0f;
    //public ShowText whiteboardText;
    //public ShowImage whiteboardImage;
    public Queue<string> messageQueue = new Queue<string>();

    void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        //TaskSelector.instance.ActivateActiveTask();
    }

   // Use this for initialization
    void Start ()
    {
        /*if (canRecordData)
        {
            InvokeRepeating("RecordData", 0.0f, 0.004f);
        }*/
        timeInApp = 0.0f;
    }

    public void SaveData(string type)
    {
        if (canRecordData)
        {
            string DELIM = ",";
            DateTime tempDateTime = DateTime.Now;
            TimeSpan timeSpan = tempDateTime - startTime;
            string timeSinceStart = timeSpan.Hours + "-" + timeSpan.Minutes + "-" + timeSpan.Seconds + "-" + timeSpan.Milliseconds;
            Vector2 binocularPor = SMI.SMIEyeTrackingUnity.Instance.smi_GetBinocularPor();
            Vector3 cameraRaycast = SMI.SMIEyeTrackingUnity.Instance.smi_GetCameraRaycast();
            float ipd = SMI.SMIEyeTrackingUnity.Instance.smi_GetIPD();
            Vector2 leftPor = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftPor();
            Vector2 rightPor = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightPor();
            Vector3 leftBasePoint = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftGazeBase();
            Vector3 rightBasePoint = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightGazeBase();
            Vector3 leftGazeDirection = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftGazeDirection();
            Vector3 rightGazeDirection = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightGazeDirection();
            Vector3 playerHeadPosition = playersTransform.position;
            Vector3 playerRotation = playersTransform.eulerAngles;
            Vector3 gazePoint = GetGazeObjectsPosition();
            /*RaycastHit gazeHit;
            if (SMI.SMIEyeTrackingUnity.Instance.smi_GetRaycastHitFromGaze(out gazeHit))
            {
                gazePoint = gazeHit.point;
            }*/
            string gazeObjectName = GetHitObjectName();

            string temp = tempDateTime.ToString("MM/dd/yyyy hh:mm:ss.fffff") + DELIM + timeSinceStart +
                DELIM + "\"" + playerHeadPosition + "\"" +
                DELIM + "\"" + playerRotation + "\"" + DELIM + "\"" + binocularPor + "\"" + 
                DELIM + "\"" + cameraRaycast + "\"" + DELIM + ipd + DELIM + "\"" + leftPor + "\"" + 
                DELIM + "\"" + rightPor + "\"" + DELIM + "\"" + leftBasePoint + "\"" + 
                DELIM + "\"" + rightBasePoint + "\"" + DELIM + "\"" + leftGazeDirection + "\"" + 
                DELIM + "\"" + rightGazeDirection + "\"" + DELIM + "\"" + gazePoint + "\"" + 
                DELIM + type + DELIM + "" + DELIM + "" + DELIM + gazeObjectName + DELIM + distanceToObject + DELIM + dataPoints;

            dataPoints++;

            messageQueue.Enqueue(temp);
        }
    }

    public void SaveDataEnterDistraction(string type, string distractionName, string nameOverride)
    {
        if (canRecordData)
        {
            string DELIM = ",";
            DateTime tempDateTime = DateTime.Now;
            TimeSpan timeSpan = tempDateTime - startTime;
            string timeSinceStart = timeSpan.Hours + "-" + timeSpan.Minutes + "-" + timeSpan.Seconds + "-" + timeSpan.Milliseconds;
            Vector2 binocularPor = SMI.SMIEyeTrackingUnity.Instance.smi_GetBinocularPor();
            Vector3 cameraRaycast = SMI.SMIEyeTrackingUnity.Instance.smi_GetCameraRaycast();
            float ipd = SMI.SMIEyeTrackingUnity.Instance.smi_GetIPD();
            Vector2 leftPor = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftPor();
            Vector2 rightPor = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightPor();
            Vector3 leftBasePoint = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftGazeBase();
            Vector3 rightBasePoint = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightGazeBase();
            Vector3 leftGazeDirection = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftGazeDirection();
            Vector3 rightGazeDirection = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightGazeDirection();
            Vector3 playerHeadPosition = playersTransform.position;
            Vector3 playerRotation = playersTransform.eulerAngles;
            Vector3 gazePoint = GetGazeObjectsPosition();
            /*RaycastHit gazeHit;
            if (SMI.SMIEyeTrackingUnity.Instance.smi_GetRaycastHitFromGaze(out gazeHit))
            {
                gazePoint = gazeHit.point;
            }*/
            string gazeObjectName = nameOverride;

            string temp = tempDateTime.ToString("MM/dd/yyyy hh:mm:ss.fffff") + DELIM + timeSinceStart +
                DELIM + "\"" + playerHeadPosition + "\"" +
                DELIM + "\"" + playerRotation + "\"" + DELIM + "\"" + binocularPor + "\"" +
                DELIM + "\"" + cameraRaycast + "\"" + DELIM + ipd + DELIM + "\"" + leftPor + "\"" +
                DELIM + "\"" + rightPor + "\"" + DELIM + "\"" + leftBasePoint + "\"" +
                DELIM + "\"" + rightBasePoint + "\"" + DELIM + "\"" + leftGazeDirection + "\"" +
                DELIM + "\"" + rightGazeDirection + "\"" + DELIM + "\"" + gazePoint + "\"" +
                DELIM + type + DELIM + distractionName + DELIM + "" + DELIM + gazeObjectName + DELIM + distanceToObject + DELIM + dataPoints;

            dataPoints++;

            messageQueue.Enqueue(temp);
        }
    }

    public void SaveDataExitDistraction(string type, string distractionName, string nameOverride, float Duration)
    {
        if (canRecordData)
        {
            string DELIM = ",";
            DateTime tempDateTime = DateTime.Now;
            TimeSpan timeSpan = tempDateTime - startTime;
            string timeSinceStart = timeSpan.Hours + "-" + timeSpan.Minutes + "-" + timeSpan.Seconds + "-" + timeSpan.Milliseconds;
            Vector2 binocularPor = SMI.SMIEyeTrackingUnity.Instance.smi_GetBinocularPor();
            Vector3 cameraRaycast = SMI.SMIEyeTrackingUnity.Instance.smi_GetCameraRaycast();
            float ipd = SMI.SMIEyeTrackingUnity.Instance.smi_GetIPD();
            Vector2 leftPor = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftPor();
            Vector2 rightPor = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightPor();
            Vector3 leftBasePoint = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftGazeBase();
            Vector3 rightBasePoint = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightGazeBase();
            Vector3 leftGazeDirection = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftGazeDirection();
            Vector3 rightGazeDirection = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightGazeDirection();
            Vector3 playerHeadPosition = playersTransform.position;
            Vector3 playerRotation = playersTransform.eulerAngles;
            Vector3 gazePoint = GetGazeObjectsPosition();
            /*RaycastHit gazeHit;
            if (SMI.SMIEyeTrackingUnity.Instance.smi_GetRaycastHitFromGaze(out gazeHit))
            {
                gazePoint = gazeHit.point;
            }*/
            string gazeObjectName = nameOverride;

            string temp = tempDateTime.ToString("MM/dd/yyyy hh:mm:ss.fffff") + DELIM + timeSinceStart +
                DELIM + "\"" + playerHeadPosition + "\"" +
                DELIM + "\"" + playerRotation + "\"" + DELIM + "\"" + binocularPor + "\"" +
                DELIM + "\"" + cameraRaycast + "\"" + DELIM + ipd + DELIM + "\"" + leftPor + "\"" +
                DELIM + "\"" + rightPor + "\"" + DELIM + "\"" + leftBasePoint + "\"" +
                DELIM + "\"" + rightBasePoint + "\"" + DELIM + "\"" + leftGazeDirection + "\"" +
                DELIM + "\"" + rightGazeDirection + "\"" + DELIM + "\"" + gazePoint + "\"" +
                DELIM + type + DELIM + distractionName + DELIM + Duration.ToString() + DELIM + gazeObjectName + DELIM + distanceToObject + DELIM + dataPoints;

            dataPoints++;

            messageQueue.Enqueue(temp);
        }
    }

    public void RecordData(System.Object work)
    {
        if (canRecordData)
        {
            string entryToWrite = "";
            string DELIM = ",";
            if (messageQueue.Count <= 0)
            {
                DateTime tempDateTime = DateTime.Now;
                TimeSpan timeSpan = tempDateTime - startTime;
                string timeSinceStart = timeSpan.Hours + "-" + timeSpan.Minutes + "-" + timeSpan.Seconds + "-" + timeSpan.Milliseconds;
                Vector2 binocularPor = SMI.SMIEyeTrackingUnity.Instance.smi_GetBinocularPor();
                Vector3 cameraRaycast = SMI.SMIEyeTrackingUnity.Instance.smi_GetCameraRaycast();
                float ipd = SMI.SMIEyeTrackingUnity.Instance.smi_GetIPD();
                Vector2 leftPor = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftPor();
                Vector2 rightPor = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightPor();
                Vector3 leftBasePoint = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftGazeBase();
                Vector3 rightBasePoint = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightGazeBase();
                Vector3 leftGazeDirection = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftGazeDirection();
                Vector3 rightGazeDirection = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightGazeDirection();
                //Vector3 playerHeadPosition = playersTransform.position;
                //Vector3 playerRotation = playersTransform.eulerAngles;
                //Vector3 gazePoint = GetGazeObjectsPosition();
                //RaycastHit gazeHit;
                /*if (SMI.SMIEyeTrackingUnity.Instance.smi_GetRaycastHitFromGaze(out gazeHit))
                {
                    gazePoint = gazeHit.point;
                }*/
                //

                entryToWrite = tempDateTime.ToString("MM/dd/yyyy hh:mm:ss.fffff") + DELIM + timeSinceStart +
                    DELIM + "\"" + playerHeadPosition + "\"" +
                    DELIM + "\"" + playerRotation + "\"" + 
                    DELIM + "\"" + binocularPor + "\"" +
                    DELIM + "\"" + cameraRaycast + "\"" + 
                    DELIM + ipd + 
                    DELIM + "\"" + leftPor + "\"" +
                    DELIM + "\"" + rightPor + "\"" + 
                    DELIM + "\"" + leftBasePoint + "\"" +
                    DELIM + "\"" + rightBasePoint + "\"" + 
                    DELIM + "\"" + leftGazeDirection + "\"" +
                    DELIM + "\"" + rightGazeDirection + "\"" + 
                    DELIM + "\"" + gazePoint + "\"" +
                    DELIM + "Standard" + 
                    DELIM + "" + 
                    DELIM + "" + 
                    DELIM + gazeObjectName + 
                    DELIM + distanceToObject + 
                    DELIM + dataPoints;
                //Debug.Log(entryToWrite);
                dataPoints++;
            }
            else
            {
                entryToWrite = messageQueue.Dequeue();
            }

            if (!exportFileName.ToLower().EndsWith(".csv"))
            {
                startTime = DateTime.Now;
                exportFileName += startTime.ToString("MM_dd_yyyy_hh_mm_ss") + ".csv";
                string temp = entryToWrite;
                entryToWrite = "TimeStamp,Time Since Start,Player Head Position,Player Rotation Euler,Binocular POR," +
                    "Camera Raycast Direction,Interpupillary Distance,Left Eye POR, Right Eye POR," + 
                    "Left Eye Base Position,Right Eye Base Position,Left Eye Gaze Direction,Right Eye Gaze Direction," +
                    "Gaze Object Position,Type Of Message,DistractionType,Duration Distracted,Active Gaze Object Name,Distance To Active Gaze Object,# Of Data Entries\n" + temp;
            }

            //string path = UnityEngine.Application.dataPath;
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            /*if (UnityEngine.Application.platform == RuntimePlatform.OSXPlayer)
            {
                path += "/../../";
            }
            else if (UnityEngine.Application.platform == RuntimePlatform.WindowsPlayer)
            {
                path += "/../";
            }*/

            //path += "/../";
            path += "/OutputLogs/";
            //UnityEngine.Windows
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            var filepath = Path.Combine(path, exportFileName);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(entryToWrite);
            File.AppendAllText(filepath, sb.ToString());
            //File.AppendAllText(filepath, entryToWrite);
            //Debug.LogError("Wrote File To : " + filepath);
        }
    }
	
    private string GetHitObjectName()
    {
        string gazeObjectName = "";
        if (hitObject != null)
        {
            gazeObjectName = hitObject.name;

        }
        else
        {
            gazeObjectName = "Null";
        }

        return gazeObjectName;
    }

    private float GetHitObjectDistance()
    {
        float gazeObjectdistance = 0.0f;
        if (hitObject != null)
        {
            gazeObjectdistance = Vector3.Distance(hitObject.transform.position, playerHeadPosition);

        }
        else
        {
            gazeObjectdistance = 0.0f;
        }

        return gazeObjectdistance;
    }

    private Vector3 GetGazeObjectsPosition()
    {
        Vector3 objectPos = Vector3.zero;

        if (hitObject != null)
        {
            objectPos = hitObject.transform.position;
        }

        return objectPos;
    }
    void OnCalibrationResult(int returnCode)
    {
        Debug.Log("SMI HMD calibration result: " + SMI.SMIEyeTrackingUnity.ErrorIDContainer.getErrorMessage(returnCode));
    }

	// Update is called once per frame
	void Update ()
    {
        
        if(Input.GetKeyDown(UnityEngine.KeyCode.Space))
        {
            if(_doOnce)
            {
                _doOnce = false;
                canRecordData = true;
                TimerCallback timerDelegate = new TimerCallback(RecordData);
                TimeSpan delayTime = new TimeSpan(0, 0, 0);
                TimeSpan intervalTime = new TimeSpan(0, 0, 0, 0, 4);
                Timer dataTimer = new Timer(RecordData, null, delayTime, intervalTime);
                Debug.LogError("I did a thing");
                //250Hz
                //InvokeRepeating("RecordData", 0.0f, 0.004f);
                //200Hz
                //InvokeRepeating("RecordData", 0.0f, 0.005f);
                //150Hz
                //InvokeRepeating("RecordData", 0.0f, 0.0067f);
                //125Hz
                //InvokeRepeating("RecordData", 0.0f, 0.008f);
            }
        }

        playerHeadPosition = playersTransform.position;
        playerRotation = playersTransform.eulerAngles;
        gazePoint = GetGazeObjectsPosition();
        gazeObjectName = GetHitObjectName();
        distanceToObject = GetHitObjectDistance();

        if (Input.GetKeyDown(UnityEngine.KeyCode.C))
        {
            SMI.SMIEyeTrackingUnity.Instance.smi_StartFivePointCalibration();
            SMI.SMIEyeTrackingUnity.Instance.SetCalibrationReturnCallback(OnCalibrationResult);
            
            //SMI.SMIEyeTrackingUnity.Instance.smi_StartNumericalValidation();
        }

        if (Input.GetKeyDown(UnityEngine.KeyCode.V))
        {
            if(!_toggleValidationWindow)
            {
                _toggleValidationWindow = !_toggleValidationWindow;
                //SMI.SMIEyeTrackingUnity.Instance.smi_ShowValidation();
                SMI.SMIEyeTrackingUnity.Instance.smi_StartNumericalValidation();
            }
            else
            {
                _toggleValidationWindow = !_toggleValidationWindow;
                SMI.SMIEyeTrackingUnity.Instance.smi_CloseVisualization();
            }
        }

        if (!_doOnce)
        {
            timeInApp += Time.deltaTime;
        }

        /*if(playersTransform.gameObject == null)
        {
            playersTransform = GameObject.Find("SMI_CameraWithEyeTracking").transform;
        }*/
    }
}
