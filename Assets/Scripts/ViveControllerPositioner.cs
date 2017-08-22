using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveControllerPositioner : MonoBehaviour {

    public Transform smiHeadset;
    public Vector3 newOriginPosition = new Vector3(0.0f,0.0f,0.0f);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        newOriginPosition = new Vector3(smiHeadset.position.x, newOriginPosition.y, smiHeadset.position.z);
	}
}
