using System;
using UnityEngine;

public class TaskActivator : MonoBehaviour {
	void Awake()
	{
		TaskSelector.instance.ActivateActiveTask();
	}
}


