using System;
using System.Collections.Generic;
using UnityEngine;

public class InputBroker : MonoBehaviour {
	private HashSet<string> forcedKeys = new HashSet<string>();

	// Clear on late update so we don't accidentally clear between different scripts.
	void LateUpdate() {
		forcedKeys.Clear ();
	}

	public void SetButtonDown(string button) {
		forcedKeys.Add (button);
	}

	public bool GetButtonDown(string button) {
		var inputdown = Input.GetButtonDown (button);
		var forceddown = forcedKeys.Contains(button);
		return inputdown || forceddown;
	}
}
