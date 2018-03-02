using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Subject;

namespace Distraction {
	public enum TimerState {Starting, Waiting, Cooldown}

	public static class TimerStateExtensions {
		public static float Duration(this TimerState state) {
			switch (state) {
			case TimerState.Waiting:  return UnityEngine.Random.Range(0f, 10f);
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
		public static DistractionController instance;

		private TimerState timerState;
		private CountdownTimer timer;
		private CSVWriter recordDistractors;

		private Distraction[] distractions;
		private Distraction currentDistraction;

    	private SubjectDataHolder subject;
		private SalienceController salienceController;

		private readonly Action endDistraction;

		public DistractionController() {
			endDistraction = () => currentDistraction = null;
		}

		void Awake () {
			if (instance != null) {
				Destroy(this.gameObject);
			} else {
				instance = this;
				distractions = FindObjectsOfType (typeof(Distraction)) as Distraction[];
				subject = FindObjectOfType<SubjectDataHolder>();
				salienceController = FindObjectOfType<SalienceController>();
			}
		}

		// Use this for initialization
		void OnEnable () {
			timerState = TimerState.Starting;
			timer = new CountdownTimer (-1);
			recordDistractors = CSVWriter.NewOutputFile(FindObjectOfType<SubjectDataHolder>(), "distractors");
			recordDistractors.WriteRow ("time,distractor");
			recordDistractors.WriteRow(Time.time + ",Start");

			List<string> ds = new List<string>();
			foreach (Distraction d in distractions) {
				ds.Add(d.distractionName);
			}
			Debug.Log("Found distractors:" + string.Join(", ", ds.ToArray()));

			if (subject.data != null && subject.data.sessions != null && subject.data.sessions.Count > 0) {
				var last = subject.data.sessions.Last();
				salienceController.salience = last.salience;
				Debug.Log("Setting initial salience to " + last.salience);
			}
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
						currentDistraction = distractions [UnityEngine.Random.Range (0, distractions.Length)];
						recordDistractors.WriteRow (Time.time + "," + currentDistraction.distractionName);
						currentDistraction.TriggerDistraction (salienceController.salience, endDistraction);
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

		public Distraction GetCurrentDistraction() {
			return currentDistraction;
		}
	}
}
