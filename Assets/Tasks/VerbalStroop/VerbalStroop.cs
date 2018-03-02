using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Distraction;
using System.Linq;

namespace VerbalStroop {
	public enum TrialText {Blue, Green, Red};

	public enum TrialColor {Blue, Green, Red};

	public enum TrialSound {Blue, Green, Red};

	public class TrialProperties {
		public TrialText text { get; }
		public TrialColor color { get; }
		public TrialSound sound { get; }
	
		public TrialProperties(TrialText text, TrialColor color, TrialSound sound) {
			this.text = text;
			this.color = color;
			this.sound = sound;
		}

		public bool CheckResponse (string buttonPressed) {
			if (sound.ToString () == color.ToString () && buttonPressed == "Button1") {
				return true;
			}
			if (sound.ToString() != color.ToString() && buttonPressed == "Button2") {
				return true;
			} 
			return false;
		}
	}


	public enum TrialState { Starting, Instruction1, Instruction2, Instruction3, Instruction4, Instruction5, Ready, Word, ITI, Correct, Incorrect, Slow, Ending };

	public static class TrialColorExtension {
		public static Color GetColor(this TrialColor color) {
			switch (color) {
			case TrialColor.Blue:  return Color.blue;
			case TrialColor.Green: return Color.green;
			case TrialColor.Red:   return Color.red;
			default:               return Color.black;
			}
		}
	}

	public static class TrialStateExtensions {
		public static float Duration(this TrialState state) {
			switch (state) {
			case TrialState.Word: return  2.0f;
			case TrialState.ITI:  return  1.0f;
			default:              return -1.0f;
			}
		}

		public static bool isInstruction(this TrialState state) {
			var beforeReady = state < TrialState.Ready;
			var isPracticeFeedback = state == TrialState.Correct || state == TrialState.Incorrect || state == TrialState.Slow;
			return beforeReady || isPracticeFeedback;
		}

		public static string Instructions(this TrialState state){
			switch (state) {
			case TrialState.Starting:     return "<size=60>Verbal Stroop\nTask\n\nFor each trial,\nyou will see\na word written\nin a color.</size>";
			case TrialState.Instruction1: return "<size=60>The word and\ncolor will change\neach trial.\nAdditionally, you will\nhear the teacher\nsay a color.</size>";
			case TrialState.Instruction2: return "<size=60>Your goal is\nto let the\nteacher know if\nshe read the\nink color correctly.</size>";
			case TrialState.Instruction3: return "<size=60>If she is\ncorrect press <b>up</b>.\nIf she is\nincorrect press <b>down</b></size>";	
			case TrialState.Instruction4: return "<size=60>You must respond\nbefore the next\nword appears.</size>";
			case TrialState.Ready:        return "<size=60><b>Remember:\npress up if\nshe is correct\nand press down if\nshe is incorrect.</b></size>\n\n<size=30><b>Coordinator begin task.</b></size>";
			case TrialState.Correct:      return "<size=60>Correct!</size>\n\n<size=30><i>Press any key to continue.</i></size>";
			case TrialState.Incorrect:    return "<size=60>Incorrect.\nTry again!</size>\n\n<size=30><i>Press any key to continue.</i></size>";
			case TrialState.Slow:         return "<size=60><b>Too slow!</b>\nYou must respond\nbefore the next\nword appears.</size>\n\n<size=30><i>Press any key to continue.</i></size>";
			default:                      return "";
			}
		}

		public static TrialState Next(this TrialState state) {
			switch (state) {
			case TrialState.Starting:     return TrialState.Instruction1;
			case TrialState.Instruction1: return TrialState.Instruction2;
			case TrialState.Instruction2: return TrialState.Instruction3;
			case TrialState.Instruction3: return TrialState.Instruction4;
			case TrialState.Instruction4: return TrialState.Ready;
			case TrialState.Ready:        return TrialState.ITI;
			case TrialState.Word:         return TrialState.ITI;
			case TrialState.ITI:          return TrialState.Word;
			case TrialState.Slow:         return TrialState.ITI;
			case TrialState.Incorrect:    return TrialState.ITI;
			case TrialState.Correct:      return TrialState.ITI;
			default:                      return TrialState.Ending;
			}
		}

	}

	public class TrialOutput {
		public readonly int num;
		public readonly TrialProperties properties;
		public readonly List<RecordResponses.Response> response;

		public TrialOutput(int num, TrialProperties properties, List<RecordResponses.Response> response) {
			this.num = num;
			this.properties = properties;
			this.response = response;
		}

		public override string ToString() {
			var rows = new List<string> ();
			foreach (RecordResponses.Response r in response) {
				rows.Add(num.ToString () + "," +
				properties.text.ToString () + "," +
				properties.color.ToString() + "," +
				properties.sound.ToString() + "," +
				r.buttonPressed + "," +
				r.responseTime.ToString ());
		}

			if (response.Count == 0) {
				return num.ToString () + "," +
					properties.text.ToString () + "," +
					properties.color.ToString() + "," +
					properties.sound.ToString() + ",,";
			}
			return String.Join("\n", rows.ToArray ());
		}
	}

	public class VerbalStroop : VRAMTask {
		public TrialList trials;
		public Textures textures;
		public RecordResponses recorder;

		private int currentTrial;
		private AvatarController avatar;
		private TrialState trialState;
		private CountdownTimer timer;
		private CSVWriter recordResults;
		private VerbalStroopPractice practice;

		private TrialList trialList {
			get {
				if (practice.enabled) {
					return practice.trials;
				} else {
					return trials;
				}
			}
		}

		protected void Awake () {
			base.Awake();
			avatar = GetComponent<AvatarController> ();
			practice = GetComponent<VerbalStroopPractice> ();
		}

		void OnEnable() {
			base.OnEnable();
			currentTrial = -1; // Start at -1 because we start the trial into ITI which will increment currentTrial
			trialState = TrialState.Starting;
			this.timer = new CountdownTimer (-1);
			recordResults = CSVWriter.NewOutputFile(subject, "verbal_stroop_results");
			recordResults.WriteRow ("trial_number,trial_properties,button_pressed,reaction_time");
		}

		void OnDisable() {
			recordResults.Close();
		}

		void Update () {
			if (trialState == TrialState.Ending) {
				EndTask();
				return;
			} else if (trialState == TrialState.Starting) {
				print ("Starting Verbal Stroop");
				ShowText (trialState.Instructions());
				trialState = trialState.Next();
				return;
			}
			var finishReady = trialState == TrialState.Ready && input.GetButtonDown ("Button3");
			var finishInstructions = trialState.isInstruction() && (input.GetButtonDown ("Button1") || input.GetButtonDown ("Button2") || input.GetButtonDown ("Button4"));
			var finishState = (int)trialState > (int)TrialState.Ready && (int)trialState <= (int)TrialState.ITI && timer.isComplete;

			if (finishInstructions || finishReady || finishState) {
				if (finishReady) {
					distractionController.gameObject.SetActive (true);
				}

				Option<TrialState> nextState = Option<TrialState>.CreateEmpty(); 
				if (recorder.isRecording && trialState == TrialState.ITI) {
					var responses = recorder.StopRecording ();
					salienceController.addResponseResult(trialList.trialProperties[currentTrial].CheckResponse(responses.Count > 0 ? responses.Last().buttonPressed : null));
					if (practice.enabled) {
						nextState = practice.HandleResponse (trialState, responses, trialList.trialProperties[currentTrial]);
					} else {
						var output = new TrialOutput (currentTrial, trialList.trialProperties[currentTrial], responses);
						recordResults.WriteRow (output.ToString ());
					}
				}
				if (trialState == TrialState.ITI && nextState.Count() == 0) {
					currentTrial++;
					if (currentTrial == trialList.trialProperties.Length) {
						trialState = TrialState.Ending;
						recordResults.Close ();
						return;
					}
				} 

				if (nextState.Count () > 0) {
					trialState = nextState.First ();
					if (trialState != TrialState.Correct) {
						currentTrial--;
					}
				} else {
					trialState = trialState.Next();
				}
				//print ("Starting state " + trialState + " in trial number " + currentTrial);
					
				var instruction = trialState.Instructions();
				if (instruction != "") {
					ShowText(instruction);
				} else {
					switch (trialState) {
					case TrialState.Word:
						avatar.Play (trialList.trialProperties [currentTrial].sound);
						var trialProps = trialList.trialProperties [currentTrial];
						ShowColorText (trialProps.text.ToString (), trialProps.color.GetColor ());
						break;
					case TrialState.ITI:
						ShowImage (textures.iti);
						break;
					}
					timer.duration = trialState.Duration ();
					if (trialState == TrialState.Word) {
						recorder.StartRecording ();
					}
					timer.Start ();
				}
			}
		}

		public override string GetCurrentState() {
			return trialState.ToString();
		}
	}
}
