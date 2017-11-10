using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Math {
	public class MathPractice : MonoBehaviour {
		public Textures textures;
		public MathProblemChecker checker;

		void Start () {
			print ("Enabled Math Practice");	
		}

		public Option<TrialState> HandleStopRecording(TrialState state, RecordResponses recorder, Texture currentProblem) {
			var response = recorder.StopRecording ();
			if (state == TrialState.Problem) {
				if (response.Count == 0) {
					return Option<TrialState>.Create(TrialState.Slow);
				} else if (checker.Check (currentProblem.name, response.Last().buttonPressed)) {
					return Option<TrialState>.Create(TrialState.Correct);
				} else {
					return Option<TrialState>.Create(TrialState.Incorrect);
				}
			}
			return Option<TrialState>.CreateEmpty();
		}
	}
}
