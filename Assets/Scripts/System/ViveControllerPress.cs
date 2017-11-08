using UnityEngine;
using System;

public class ViveControllerPress : MonoBehaviour {
	private Valve.VR.EVRButtonId touchpadButton = Valve.VR.EVRButtonId.k_EButton_Axis0;
	private SteamVR_TrackedObject trackedObject;
	private SteamVR_Controller.Device device;

	private InputBroker input;

	void Start() {
		trackedObject = GetComponent<SteamVR_TrackedObject> ();
		input = (InputBroker)FindObjectOfType(typeof(InputBroker));
	}

	void Update() {
		device = SteamVR_Controller.Input ((int)trackedObject.index);
		if (!device.GetPressDown (touchpadButton)) {
			return;
		}
		var axis = device.GetAxis (touchpadButton);
		if (System.Math.Abs (axis.x) > System.Math.Abs (axis.y)) {
			if (axis.x > 0) {
				// right
				input.SetButtonDown ("Button2");
				device.TriggerHapticPulse (1000);
			} else {
				// left
				input.SetButtonDown ("Button4");
				device.TriggerHapticPulse (1000);
			}
		} else {
			if (axis.y > 0) {
				// up
				input.SetButtonDown ("Button1");
			} else {
				// down
				input.SetButtonDown ("Button3");
			}
		}
	}
}


