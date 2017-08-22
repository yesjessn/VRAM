using UnityEngine;
using System.Collections;

public class Test3DValue : MonoBehaviour {

    [SerializeField]
    SMI.SMIEyeTrackingUnity smiCamera;

	// Update is called once per frame
	void Update () {

        // gaze direction is normalized to unit norm
        Vector3 cameraRaycast = SMI.SMIEyeTrackingUnity.Instance.smi_GetCameraRaycast();
        Vector3 leftBasePoint = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftGazeBase(); 
        Vector3 rightBasePoint = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightGazeBase();
        Vector3 leftGazeDirection = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftGazeDirection();
        Vector3 rightGazeDirection = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightGazeDirection();

        //the origin is the camera
        cameraRaycast = smiCamera.transform.rotation * cameraRaycast;
        leftBasePoint = smiCamera.transform.rotation * leftBasePoint;
        rightBasePoint = smiCamera.transform.rotation * rightBasePoint;
        leftGazeDirection = smiCamera.transform.rotation * leftGazeDirection;
        rightGazeDirection = smiCamera.transform.rotation * rightGazeDirection;

        // Draw 3d gaze ray for binocular, left, and right eye in the editor. 
        // Note that ou don't see this in the game scene. They are shown in the scene tab
        Debug.DrawRay(smiCamera.transform.position, cameraRaycast * 1000f, Color.blue);
        Debug.DrawRay(smiCamera.transform.position + leftBasePoint, leftGazeDirection * 1000f, Color.red);
        Debug.DrawRay(smiCamera.transform.position + rightBasePoint, rightGazeDirection * 1000f, Color.red);
    }
}
