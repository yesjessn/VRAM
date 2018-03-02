using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AXCPT {
	public class AXCPTPractice : MonoBehaviour {
		public TrialList trials;

		private TrialState _previousState;
		public TrialState PreviousState { get; }

		void Start () {}

		public Option<TrialState> HandleResponse(TrialState state, List<RecordResponses.Response> responses, TrialType trialType) {
			_previousState = state;
			if (state == TrialState.ISI || state == TrialState.PreCueITI) {
				if (responses.Count == 0) {
					return Option<TrialState>.Create(TrialState.Slow);
				} else if (trialType.CheckResponse (state, responses.Last().buttonPressed)) {
					return Option<TrialState>.Create(TrialState.Correct);
				} else {
					return Option<TrialState>.Create(TrialState.Incorrect);
				}
			}
			return Option<TrialState>.CreateEmpty();
		}
	}
}