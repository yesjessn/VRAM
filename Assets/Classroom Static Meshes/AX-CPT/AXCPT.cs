using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AXCPT {
	public enum TrialItem {A, B, X, Y};

	// Class to hold the cue and target types
	public class TrialType {
		public TrialItem cue { get; }
		public TrialItem target { get; }

		public TrialType(TrialItem cue, TrialItem target) {
			this.cue = cue;
			this.target = target;
		}

		public override string ToString() {
			return cue.ToString () + target.ToString ();
		}
	}

	public enum TrialState { Starting, Cue, ISI, Target, ITI, Ending };

	public static class TrialStateExtensions {
		public static float Duration(this TrialState state) {
			switch (state) {
			case TrialState.Cue:    return 1.0f;
			case TrialState.ISI:    return 2.0f;
			case TrialState.Target: return 0.5f;
			case TrialState.ITI:    return 1.2f;
			default:                return -1f;
			}
		}

		public static TrialState Next(this TrialState state) {
			switch (state) {
			case TrialState.Starting: return TrialState.Cue;
			case TrialState.Cue:      return TrialState.ISI;
			case TrialState.ISI:      return TrialState.Target;
			case TrialState.Target:   return TrialState.ITI;
			case TrialState.ITI:      return TrialState.Cue;
			default:                  return TrialState.Ending;
			}
		}

		public static Texture GetTexture(this TrialState state, TrialType type, Textures textures) {
			switch (state) {
			case TrialState.Cue:    return textures.Get(type.cue);
			case TrialState.ISI:    return textures.isi;
			case TrialState.Target: return textures.Get(type.target);
			case TrialState.ITI:    return textures.iti;
			default:                return null;
			}
		}
	}

	public class TrialOutput {
		public readonly int num;
		public readonly TrialType type;
		public readonly RecordResponses.Response response;

		public TrialOutput(int num, TrialType type, RecordResponses.Response response) {
			this.num = num;
			this.type = type;
			this.response = response;
		}

		public override string ToString() {
			return num.ToString () + "," +
			type.ToString () + "," +
			response.buttonPressed + "," +
			(response == RecordResponses.EMPTY_RESPONSE ? "" : response.responseTime.ToString ());
		}
	}

	public class AXCPT : MonoBehaviour {
		public ShowImage whiteboard;
		public TrialList trials;
		public Textures textures;
		public RecordResponses recorder;

		private int currentTrial;
		private TrialState trialState;
		private CountdownTimer timer;
		private CountdownTimer recordingTimer;
		private CSVWriter recordResults;

		void Start () {
			currentTrial = 0;
			trialState = TrialState.Starting;
			timer = new CountdownTimer (-1);
			recordingTimer = new CountdownTimer (1.2f);
			recordResults = new CSVWriter ("results.csv");
			recordResults.WriteRow ("trial_number,trial_type,button_pressed,reaction_time");
			print ("Starting AX-CPT");
		}

		void Update () {
			if (trialState != TrialState.Ending && timer.isComplete) {
				if (trialState == TrialState.ITI) {
					currentTrial++;
					if (currentTrial == trials.trialTypes.Length) {
						trialState = TrialState.Ending;
						recordResults.Close ();
						whiteboard.Hide ();
						return;
					}
				}

				trialState = trialState.Next ();
				print ("Starting state " + trialState);

				whiteboard.SetTexture(trialState.GetTexture(trials.trialTypes[currentTrial], textures));
				whiteboard.Show ();

				timer.duration = trialState.Duration ();
				if (trialState == TrialState.Target) {
					recorder.StartRecording ();
					recordingTimer.Start ();
				}
				timer.Start ();
			}
			if (recorder.isRecording && recordingTimer.isComplete) {
				var response = recorder.StopRecording ();
				var output = new TrialOutput (currentTrial, trials.trialTypes [currentTrial], response);
				recordResults.WriteRow (output.ToString());
			}
		}
	}
}
