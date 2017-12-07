using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Distraction {
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

	public class DistractionController : MonoBehaviour {
		private TimerState timerState;
		private CountdownTimer timer;
		private CSVWriter recordDistractors;

		private Distraction[] distractions;

		void Awake () {
			distractions = FindObjectsOfType (typeof(Distraction)) as Distraction[];
		}

		// Use this for initialization
		void Start () {
			timerState = TimerState.Starting;
			timer = new CountdownTimer (-1);
			recordDistractors = CSVWriter.NewOutputFile("distractors");
			recordDistractors.WriteRow ("time,distractor");
			recordDistractors.WriteRow(Time.time + ",Start");

			List<string> ds = new List<string>();
			foreach (Distraction d in distractions) {
				ds.Add(d.distractionName);
			}
			print ("Found distractors:" + string.Join(", ", ds.ToArray()));
		}

		// Update is called once per frame
		void Update () {
//			if (Input.GetButtonDown ("Button4")) {
//				var d = distractions [Random.Range (0, distractions.Length)];
//				print ("Playing distractor: " + d.distractionName);
//				d.TriggerDistraction (null);
//				return;
//			}
//		}

			if (timer.isComplete) {
				if (timerState == TimerState.Waiting) {
					if (distractions.Length > 0) {
						var d = distractions [Random.Range (0, distractions.Length)];
						recordDistractors.WriteRow (Time.time + "," + d.distractionName);
						d.TriggerDistraction (null);
					}
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
