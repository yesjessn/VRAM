using System;
using UnityEngine;

// This class used to delay the enabling of the task until the correct scene is loaded.
public class TaskActivator : MonoBehaviour {
	void Awake() {
		var taskList = TaskList.instance;
		var task = taskList.GetActiveTask();
		task.gameObject.SetActive(true);
	}
}


