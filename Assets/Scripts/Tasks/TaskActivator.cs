using System;
using UnityEngine;

public class TaskActivator : MonoBehaviour {
	void Awake() {
		var sel = TaskSelector.instance;
		if (sel != null) {
			sel.ActivateActiveTask();
		}
	}
}


