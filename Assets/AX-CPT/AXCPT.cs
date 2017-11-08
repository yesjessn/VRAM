using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Distraction;
using System.IO;


namespace AXCPT {
	public enum TrialItem {A, B, X, Y};

	// Class to hold the cue and probe types
	public class TrialType {
		public TrialItem cue { get; set; }
		public TrialItem probe { get; set; }

		public TrialType(TrialItem cue, TrialItem probe) {
			this.cue = cue;
			this.probe = probe;
		}

		public bool CheckResponse (TrialState state, string buttonPressed) {
			if (state == TrialState.ISI) {
				if (buttonPressed == "Button1") {
					return true;
				} else {
					return false;
				}
			}
			if (cue == TrialItem.A && probe == TrialItem.X) {
				if (buttonPressed == "Button2") {
					return true;
				} else {
					return false;
				}
			}
			if (buttonPressed == "Button1") {
				return true;
			} else {
				return false;
			}
		}

		public override string ToString() {
			return cue.ToString () + probe.ToString ();
		}
	}

	public enum TrialState { Starting, Instruction1, Instruction2, Instruction3, Instruction4, Instruction5, Ready, PreCueITI, Cue, ISI, PreProbeITI, Probe, Correct, Incorrect, Slow, Ending };

	public static class TrialStateExtensions {
		public static float Duration(this TrialState state) {
			switch (state) {
			case TrialState.Cue:         return  1.0f;
			case TrialState.ISI:         return  2.0f;
			case TrialState.Probe:       return  0.5f;
			case TrialState.PreCueITI:   return  1.2f;
			case TrialState.PreProbeITI: return  1.2f;
			default:                     return -1.0f;
			}
		}

		public static bool isInstruction(this TrialState state) {
			var beforeReady = state < TrialState.Ready;
			var isPracticeFeedback = state == TrialState.Correct || state == TrialState.Incorrect || state == TrialState.Slow;
			return beforeReady || isPracticeFeedback;
		}

		public static string Instructions(this TrialState state, Textures textures) {
			switch (state) {
			case TrialState.Starting:     return "<size=60>AX-CPT Task</size>\n\n<size=30><i>Press any key to continue.</i></size>";
			case TrialState.Instruction1: return "<size=60>For each trial,\nyou will see\na pair of\npictures with\ntext below.\n\n</size><size=30><i>Press any key to continue.</i></size>";
			case TrialState.Instruction2: return "<size=60>The 1st picture will\nhave <color=blue>blue</color> text.\nThe 2nd picture will\nhave <color=orange>orange</color> text.\nYour goal is to\nfind the special pair.</size>\n\n<size=30><i>Press any key to continue.</i></size>";
			case TrialState.Instruction3: return "<size=60>The special pair\nis <color=blue>" + textures.a_group [0].name + "</color>\nbefore <color=orange>" + textures.x_group [0].name + "</color>.\nIf you see\nthis pair, press <b>1</b>.</size>\n\n<size=30><i>Press any key to continue.</i></size>";
			case TrialState.Instruction4: return "<size=60>For all other\n pairs, press <b>2</b>.</size>\n\n<size=30><i>Press any key to continue.</i></size>";
			case TrialState.Instruction5: return "<size=60>You must respond\nbefore the next\npair appears.</size>\n\n<size=30><i>Press any key to continue.</i></size>";
			case TrialState.Ready:        return "<size=60><b>Remember: <color=blue>" + textures.a_group [0].name + "</color>\nbefore <color=orange>" + textures.x_group [0].name + "</color>.</b></size>\n\n<size=30><i>Press 1 to begin task.</i></size>";
			case TrialState.Correct:      return "<size=60>Correct!</size>\n\n<size=30><i>Press any key to continue.</i></size>";
			case TrialState.Incorrect:    return "<size=60>Incorrect.\nTry again!</size>\n\n<size=30><i>Press any key to continue.</i></size>";
			case TrialState.Slow:         return "<size=60><b>Too slow!</b>\nYou must respond\nbefore the next\npair appears.</size>\n\n<size=30><i>Press any key to continue.</i></size>";
			case TrialState.Ending:       return "<size=60>Note: in the\nreal task,\nyou will se\ndifferent objects\ninstead of shapes.</size>\n\n<size=30><i>Press any key to continue.</i></size>";
			default:                      return "";
			}
		}

		public static TrialState Next(this TrialState state) {
			switch (state) {
			case TrialState.Starting:     return TrialState.Instruction1;
			case TrialState.Instruction1: return TrialState.Instruction2;
			case TrialState.Instruction2: return TrialState.Instruction3;
			case TrialState.Instruction3: return TrialState.Instruction4;
			case TrialState.Instruction4: return TrialState.Instruction5;
			case TrialState.Instruction5: return TrialState.Ready;
			case TrialState.Ready:        return TrialState.PreCueITI;
			case TrialState.PreCueITI:    return TrialState.Cue;
			case TrialState.Cue:          return TrialState.ISI;
			case TrialState.ISI:          return TrialState.Probe;
			case TrialState.PreProbeITI:  return TrialState.Probe;
			case TrialState.Probe:        return TrialState.PreCueITI;
			case TrialState.Slow:         return TrialState.PreCueITI;
			case TrialState.Incorrect:    return TrialState.PreCueITI;
			default:                      return TrialState.Ending;
			}
		}

		public static Texture GetTexture(this TrialState state, TrialType type, Textures textures) {
			switch (state) {
			case TrialState.Cue:         return textures.Get(type.cue);
			case TrialState.ISI:         return textures.isi;
			case TrialState.Probe:       return textures.Get(type.probe);
			case TrialState.PreCueITI:   return textures.iti;
			case TrialState.PreProbeITI: return textures.iti;
			default:                     return null;
			}
		}
	}

	public class TrialOutput {
		public readonly int num;
		public readonly TrialType type;
		public readonly TrialState state;
		public readonly string stimulusName;
		public readonly List<RecordResponses.Response> response;

		public TrialOutput(int num, TrialType type, TrialState state, string stimulusName, List<RecordResponses.Response> response) {
			this.num = num;
			this.type = type;
			this.state = state;
			this.stimulusName = stimulusName;
			this.response = response;
		}

		public override string ToString() {
			var rows = new List<string> ();
			foreach (RecordResponses.Response r in response) {
				rows.Add(num.ToString () + "," +
				type.ToString () + "," +
				state.ToString() + "," +
				stimulusName + "," +
				r.buttonPressed + "," +
				r.responseTime.ToString ());
			}
			if (response.Count == 0) {
				return num.ToString () + "," +
					type.ToString () + "," +
					state.ToString() + "," +
					stimulusName + ",,";
			}
			return String.Join("\n", rows.ToArray ());
		}
	}

	public class AXCPT : MonoBehaviour {
		public TrialList trials;
		public Textures textures;
		public RecordResponses recorder;
		public DistractionController distractionController;

		private int currentTrial;
		private TrialState trialState;
		private CountdownTimer timer;
		private CSVWriter recordResults;
		private string stimulusName;
		private ShowText whiteboardText;
		private ShowImage whiteboardImage;
		private AXCPTPractice practice;
		private InputBroker input;

		private TrialList trialList {
			get {
				if (practice.enabled) {
					return practice.trials;
				} else {
					return trials;
				}
			}
		}

		void Start () {
			input = (InputBroker)FindObjectOfType(typeof(InputBroker));
			currentTrial = -1; // Start at -1 because we start the trial into ITI which will increment currentTrial
			trialState = TrialState.Starting;
			timer = new CountdownTimer (-1);
			string desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
			desktop = Path.Combine(desktop, "OutputLogs");
			//UnityEngine.Windows
			if(!Directory.Exists(desktop))
			{
				Directory.CreateDirectory(desktop);
			}
			var filepath = Path.Combine(desktop, "axcpt_results.csv");
			recordResults = new CSVWriter (filepath);
			recordResults.WriteRow ("trial_number,trial_type,stimulus_type,stimulus_name,button_pressed,reaction_time");
			print ("Starting AX-CPT");
            whiteboardText = GameObject.Find("WhiteBoardWithDisplay").GetComponent<ShowText>();
            whiteboardImage = GameObject.Find("WhiteBoardWithDisplay").GetComponent<ShowImage>();
            whiteboardText.SetText (trialState.Instructions(textures));
			whiteboardText.Show ();
			practice = GetComponent<AXCPTPractice> ();
		}

		void Update () {
			var finishReady = trialState == TrialState.Ready && input.GetButtonDown ("Button3");
			var finishInstructions = trialState.isInstruction() && input.GetButtonDown ("Button1");
			var finishState = (int)trialState > (int)TrialState.Ready && (int)trialState <= (int)TrialState.Probe && timer.isComplete;

			if (finishInstructions || finishReady || finishState) {
				if (finishReady) {
					distractionController.gameObject.SetActive (true);
				}

				Option<TrialState> nextState = Option<TrialState>.CreateEmpty();
				if (recorder.isRecording && (trialState == TrialState.ISI || trialState == TrialState.PreCueITI)) {
					if (practice.enabled) {
						nextState = practice.HandleStopRecording (trialState, recorder, trialList.trialTypes [currentTrial]);
					} else {
						var response = recorder.StopRecording ();
						var output = new TrialOutput (currentTrial, trialList.trialTypes [currentTrial], trialState, stimulusName, response);
						recordResults.WriteRow (output.ToString ());
					}
				}

				if (trialState == TrialState.PreCueITI && nextState.Count() == 0) {
					currentTrial++;
					if (currentTrial == trialList.trialTypes.Length) {
						trialState = TrialState.Ending;
						recordResults.Close ();
						whiteboardImage.Hide ();
						distractionController.gameObject.SetActive (false);
						return;
					}
				}

				if (nextState.Count() > 0) {
					trialState = nextState.First();
					if (trialState != TrialState.Correct) {
						// Repeat the current trial
						currentTrial--;
					}
				} else {
					if (trialState == TrialState.Correct) {
						if (practice.PreviousState == TrialState.ISI) {
							trialState = TrialState.PreProbeITI;
						} else {
							trialState = TrialState.PreCueITI;
						}
					} else {
						trialState = trialState.Next ();
					}
				}
				print ("Starting state " + trialState + " in trial number " + currentTrial);

				var instruction = trialState.Instructions(textures);
				if (instruction != "") {
					whiteboardImage.Hide ();
					whiteboardText.SetText (instruction);
					whiteboardText.Show ();
				} else {
					whiteboardText.Hide ();
					// currentTrial will be -1 on the first precueiti state when we need to show the iti texture
					// in that case, using null is okay for trial type because the iti texture doesn't depend on the trial type.
					var trialType = currentTrial == -1 ? null : trialList.trialTypes [currentTrial];
					var selectedTexture = trialState.GetTexture (trialType, textures);
					whiteboardImage.SetTexture(selectedTexture);
					whiteboardImage.Show ();

					timer.duration = trialState.Duration ();
					if (trialState == TrialState.Cue || trialState == TrialState.Probe) {
						stimulusName = selectedTexture.name;
						recorder.StartRecording ();
					}
					timer.Start ();
				}
			}
		}
	}
}
