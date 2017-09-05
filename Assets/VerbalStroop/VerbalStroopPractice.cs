using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VerbalStroop {
public class VerbalStroopPractice : MonoBehaviour {
		public TrialList trials;

		void Start () {}

		public Option<TrialState> HandleStopRecording(TrialState state, RecordResponses recorder, TrialProperties trialProperties) {
			var response = recorder.StopRecording ();
			if (state == TrialState.ITI) {
				if (response.Count == 0) {
					return Option<TrialState>.Create(TrialState.Slow);
				} else if (trialProperties.CheckResponse (response.Last().buttonPressed)) {
					return Option<TrialState>.Create(TrialState.Correct);
				} else {
					return Option<TrialState>.Create(TrialState.Incorrect);
				}
			}
			return Option<TrialState>.CreateEmpty();
		}
	}
}
