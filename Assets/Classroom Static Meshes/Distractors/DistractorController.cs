using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Distractors{
	public enum TimerState {Starting, Waiting, Cooldown}

	public static class TimerStateExtensions {
		public static float Duration(this TimerState state) {
			switch (state) {
			case TimerState.Waiting:  return Random.Range(0f, 40f);
			case TimerState.Cooldown: return 20f;
			default:                  return -1f;
			}
		}

		public static TimerState Next (this TimerState state) {
			switch (state) {
			case TimerState.Starting: return TimerState.Waiting;
			case TimerState.Waiting:  return TimerState.Cooldown;
			case TimerState.Cooldown: return TimerState.Waiting;
			default:                  return TimerState.Starting;
			}
		}
	}

	public class DistractorController : MonoBehaviour {
		private TimerState timerState;
		private CountdownTimer timer;
		private CSVWriter recordDistractors;

		// Use this for initialization
		void Start () {
			timerState = TimerState.Starting;
			timer = new CountdownTimer (-1);
			recordDistractors = new CSVWriter ("distractors.csv");
			recordDistractors.WriteRow ("time,distractor");
			recordDistractors.WriteRow(Time.time + ",Start");
		}
		
		// Update is called once per frame
		void Update () {
			if (timer.isComplete) {
				if (timerState == TimerState.Waiting) {
				//set distractor active
					recordDistractors.WriteRow(Time.time + ",distractor"); //to be changed
				}
				timerState = timerState.Next ();
				timer.duration = timerState.Duration ();
				timer.Start ();
			}	
		}

		void OnDisable () {
			recordDistractors.Close ();
		}
	}
}