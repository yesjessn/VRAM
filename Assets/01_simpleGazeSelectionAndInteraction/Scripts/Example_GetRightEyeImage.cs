using UnityEngine;
using System.Collections;

public class Example_GetRightEyeImage : MonoBehaviour {
    
    SMI.SMIEyeTrackingUnity smiInstance;
    Material eyeMaterial;
    // Use this for initialization
    void Start ()
    {
        smiInstance = SMI.SMIEyeTrackingUnity.Instance;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.E))
        {
            smiInstance.smi_StartEyeImageStreaming();
        }

        eyeMaterial = smiInstance.smi_GetRightEyeImage();

        Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
        renderer.material = eyeMaterial;
    }



}
