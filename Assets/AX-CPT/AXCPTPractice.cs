using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AXCPT {
	public class AXCPTPractice : MonoBehaviour {
		public TrialList trials;

		private TrialState _previousState;
		public TrialState PreviousState { get; }

		void Start () {}

		public Option<TrialState> HandleStopRecording(TrialState state, RecordResponses recorder, TrialType trialType) {
			var response = recorder.StopRecording ();
			_previousState = state;
			if (state == TrialState.ISI || state == TrialState.PreCueITI) {
				if (response.Count == 0) {
					return Option<TrialState>.Create(TrialState.Slow);
				} else if (trialType.CheckResponse (state, response.Last().buttonPressed)) {
					return Option<TrialState>.Create(TrialState.Correct);
				} else {
					return Option<TrialState>.Create(TrialState.Incorrect);
				}
			}
			return Option<TrialState>.CreateEmpty();
		}
	}
}