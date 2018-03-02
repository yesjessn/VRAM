using System;
using UnityEngine;

public class TaskActivator : MonoBehaviour {
	void Awake() {
		var taskList = TaskList.instance;
		var task = taskList.GetActiveTask();
		task.gameObject.SetActive(true);
	}
}


