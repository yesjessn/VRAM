using UnityEngine;
using System.Collections;

public class Example_GetLeftEyeImage : MonoBehaviour {

    SMI.SMIEyeTrackingUnity smiInstance;
    Material eyeMaterial;
    // Use this for initialization
    void Start ()
    {
        smiInstance = SMI.SMIEyeTrackingUnity.Instance;
    }
	
	// Update is called once per frame
	void Update () {
        eyeMaterial = smiInstance.smi_GetLeftEyeImage();
        Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
        renderer.material = eyeMaterial;
    }
}
