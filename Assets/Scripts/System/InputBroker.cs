using System;
using System.Collections.Generic;
using System.Threading.Collections;
using System.Linq;
using UnityEngine;

public class InputBroker : MonoBehaviour {
	private readonly ConcurrentQueue<string> keyQueue = new ConcurrentQueue<string>();
	private HashSet<string> forcedKeys = new HashSet<string>();

	// Clear on late update so we don't accidentally clear between different scripts.
	void Update() {
		forcedKeys.Clear ();
		string key;
		while (keyQueue.TryDequeue(out key)) {
			forcedKeys.Add(key);
		}
	}

	public void SetButtonDown(string button) {
		keyQueue.Enqueue (button);
	}

	public bool GetButtonDown(string button) {
		var inputdown = Input.GetButtonDown (button);
		var forceddown = forcedKeys.Contains(button);
		return inputdown || forceddown;
	}

	public bool IsAnyKeyDown() {
		return GetButtonDown("Button1") || GetButtonDown("Button2") || GetButtonDown("Button3") || GetButtonDown("Button4");
	}

	public override string ToString() {
		return "Forced=" + forcedKeys.ToArray().ToString() + " [" +
			GetButtonDown("Button1") +
			GetButtonDown("Button2") +
			GetButtonDown("Button3") +
			GetButtonDown("Button4") + "]";
	}
}
