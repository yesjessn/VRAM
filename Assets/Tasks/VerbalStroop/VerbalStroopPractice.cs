using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VerbalStroop {
public class VerbalStroopPractice : MonoBehaviour {
		public TrialList trials;

		void Start () {}

		public Option<TrialState> HandleResponse(TrialState state, List<RecordResponses.Response> responses, TrialProperties trialProperties) {
			if (state == TrialState.ITI) {
				if (responses.Count == 0) {
					return Option<TrialState>.Create(TrialState.Slow);
				} else if (trialProperties.CheckResponse (responses.Last().buttonPressed)) {
					return Option<TrialState>.Create(TrialState.Correct);
				} else {
					return Option<TrialState>.Create(TrialState.Incorrect);
				}
			}
			return Option<TrialState>.CreateEmpty();
		}
	}
}
