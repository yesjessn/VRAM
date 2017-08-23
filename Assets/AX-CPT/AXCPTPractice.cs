using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AXCPT {
	public static class PracticeTrialStateExtensions {

		public static string PracticeInstructions(this TrialState state) {
			switch (state) {
			case TrialState.Starting:     return "<size=3>AX-CPT\nPractice</size>\n\n<size=1><i>Press any key to continue.</i></size>";
			case TrialState.Instruction1: return "<size=2>For each trial,\nyou will see\na pair of\npictures with\ntext below.\n\n</size><size=1><i>Press any key to continue.</i></size>";
			case TrialState.Instruction2: return "<size=2>The 1st picture will\nhave <color=blue>blue</color> text.\nThe 2nd picture will\nhave <color=orange>orange</color> text.\nYour goal is to\nfind the special pair.</size>\n\n<size=1><i>Press any key to continue.</i></size>";
			case TrialState.Instruction3: return "<size=2>The special pair\nis <color=blue>circle</color>\nbefore <color=orange>square</color>.\nIf you see\nthis pair, press <b>1</b>.</size>\n\n<size=1><i>Press any key to continue.</i></size>";
			case TrialState.Instruction4: return "<size=2>For all other\n pairs, press <b>2</b>.</size>\n\n<size=1><i>Press any key to continue.</i></size>";
			case TrialState.Instruction5: return "<size=2>You must respond\nbefore the next\npair appears.</size>\n\n<size=1><i>Press any key to continue.</i></size>";
			case TrialState.Ready:        return "<size=2><b>Remember: <color=blue>circle</color>\nbefore <color=orange>square</color>.</b></size>\n\n<size=1><i>Press 1 to begin task.</i></size>";
			case TrialState.Correct:      return "<size=2>Correct!</size>\n\n<size=1><i>Press any key to continue.</i></size>";
			case TrialState.Incorrect:    return "<size=2>Incorrect.\nTry again!</size>\n\n<size=1><i>Press any key to continue.</i></size>";
			case TrialState.Slow:         return "<size=2><b>Too slow!</b>\nYou must respond\nbefore the next\npair appears.</size>\n\n<size=1><i>Press any key to continue.</i></size>";
			case TrialState.Ending:       return "<size=2>Note: in the\nreal task,\nyou will see\ndifferent objects\ninstead of shapes.</size>\n\n<size=1><i>Press any key to finish.</i></size>";
			default:                      return "";
			}
		}
	}

	public class AXCPTPractice : MonoBehaviour {
		public ShowText whiteboardText;
		public ShowImage whiteboardImage;
		public TrialList trials;
		public Textures textures;
		public RecordResponses recorder;

		private int currentTrial;
		private TrialState trialState;
		private CountdownTimer timer;
		private CountdownTimer recordingTimer;
		private List<RecordResponses.Response> response;

		void Start () {
			currentTrial = 0;
			trialState = TrialState.Starting;
			timer = new CountdownTimer (-1);
			recordingTimer = new CountdownTimer (1.5f);
			print ("Starting AX-CPT");
			whiteboardText.SetText (trialState.PracticeInstructions());
			whiteboardText.Show ();
		}

		void Update () {
			var finishInstructions = trialState == TrialState.Ready && Input.GetButtonDown ("Button1");
			var finishState = (int)trialState >= (int)TrialState.Cue && (int)trialState <= (int)TrialState.ITI && timer.isComplete;
			var finishTrial = (int)trialState >= (int)TrialState.Correct && (int)trialState <= (int)TrialState.Slow && Input.anyKeyDown;

			if (((int)trialState < (int)TrialState.Ready) && Input.anyKeyDown) {
				trialState = trialState.Next ();
				whiteboardText.SetText (trialState.Instructions(textures));
				return;
			} 
			if (finishInstructions || finishState || finishTrial) {
				whiteboardText.Hide ();
				if (trialState == TrialState.ITI) {
					if (response.Count == 0) {
						trialState = TrialState.Slow;
						whiteboardImage.Hide ();
						whiteboardText.SetText (trialState.PracticeInstructions ());
						whiteboardText.Show ();
					} else if (trials.trialTypes [currentTrial].CheckResponse (response[0].buttonPressed)) {
						trialState = TrialState.Correct;
						whiteboardImage.Hide ();
						whiteboardText.SetText (trialState.PracticeInstructions ());
						whiteboardText.Show ();
						currentTrial++;
					} else {
						trialState = TrialState.Incorrect;
						whiteboardImage.Hide ();
						whiteboardText.SetText (trialState.PracticeInstructions ());
						whiteboardText.Show ();
					}
				} else if (trialState == TrialState.Correct && currentTrial == trials.trialTypes.Length) {
					trialState = TrialState.Ending;
					whiteboardImage.Hide ();
					return;
				} else {
					trialState = trialState.Next ();
					whiteboardImage.SetTexture(trialState.GetTexture(trials.trialTypes[currentTrial], textures));
					whiteboardImage.Show ();
				}
				print ("Starting state " + trialState);

				timer.duration = trialState.Duration ();
				if (trialState == TrialState.Probe) {
					recorder.StartRecording ();
					recordingTimer.Start ();
				}
				timer.Start ();
			}
			if (recorder.isRecording && recordingTimer.isComplete) {
				response = recorder.StopRecording ();
				}

			}
		}
	}

