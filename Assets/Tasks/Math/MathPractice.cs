using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Math {
	public class MathPractice : MonoBehaviour {
		public Textures textures;

		private MathProblemChecker checker;

		void Start () {
			print ("Enabled Math Practice");	
		}

		public void SetProblemChecker(MathProblemChecker checker) {
			this.checker = checker;
		}

		public Option<TrialState> HandleResponse(TrialState state, List<RecordResponses.Response> responses, string currentProblem) {
			if (state == TrialState.Problem) {
				if (responses.Count == 0) {
					return Option<TrialState>.Create(TrialState.Slow);
				} else if (checker.Check (currentProblem, responses.Last().buttonPressed)) {
					return Option<TrialState>.Create(TrialState.Correct);
				} else {
					return Option<TrialState>.Create(TrialState.Incorrect);
				}
			}
			return Option<TrialState>.CreateEmpty();
		}
	}
}
