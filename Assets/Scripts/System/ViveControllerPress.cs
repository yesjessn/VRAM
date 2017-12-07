using UnityEngine;
using System;

public class ViveControllerPress : MonoBehaviour {
	private InputBroker input;

	void Start() {
		var controller = GetComponent<SteamVR_TrackedController> ();
		if (controller != null) {
			controller.PadClicked += HandlePadPress;
		} else {
			print("Controller not found on "+this.gameObject.name);
		}
		input = (InputBroker)FindObjectOfType(typeof(InputBroker));
	}

	void HandlePadPress(object sender, ClickedEventArgs e) {
		var tracked = sender as SteamVR_TrackedController;
		var device = SteamVR_Controller.Input((int) tracked.controllerIndex);
		var x = e.padX;
		var y = e.padY;
		if (System.Math.Abs (x) > System.Math.Abs (y)) {
			if (x > 0) {
				// right
				input.SetButtonDown ("Button2");
				device.TriggerHapticPulse (1000);
			} else {
				// left
				input.SetButtonDown ("Button4");
				device.TriggerHapticPulse (1000);
			}
		} else {
			if (y > 0) {
				// up
				input.SetButtonDown ("Button1");
			} else {
				// down
				input.SetButtonDown ("Button3");
			}
		}
	}
}


