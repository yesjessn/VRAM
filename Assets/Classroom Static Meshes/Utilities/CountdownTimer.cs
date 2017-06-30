using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownTimer {
	public float duration;

	private float endTime;

	public bool isComplete { get { return Time.time >= endTime; } }

	public CountdownTimer(float duration) {
		this.duration = duration;
	}

	public void Start() {
		endTime = Time.time + duration;
	}
}
