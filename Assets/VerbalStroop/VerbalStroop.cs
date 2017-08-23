using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	}

	public enum TrialState { Starting, Instruction1, Instruction2, Instruction3, Instruction4, Instruction5, Ready, Word, ITI, Ending };

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

		public static TrialState Next(this TrialState state) {
			switch (state) {
			case TrialState.Starting:     return TrialState.Instruction1;
			case TrialState.Instruction1: return TrialState.Instruction2;
			case TrialState.Instruction2: return TrialState.Instruction3;
			case TrialState.Instruction3: return TrialState.Instruction4;
			case TrialState.Instruction4: return TrialState.Instruction5;
			case TrialState.Instruction5: return TrialState.Ready;
			case TrialState.Ready:        return TrialState.Word;
			case TrialState.Word:     return TrialState.ITI;
			case TrialState.ITI:      return TrialState.Word;
			default:                  return TrialState.Ending;
			}
		}

		public static string Instructions(this TrialState state){
			switch (state) {
			case TrialState.Starting:     return "<size=120>Verbal Stroop\nTask</size>\n\n<size=90><i>Please press any key to continue.</i></size>";
			case TrialState.Instruction1: return "<size=100>For each trial,\nyou will see\na word written\nin a color.\nThe word and\ncolor will change\neach trial.\n\n</size><size=90><i>Press any key to continue.</i></size>";
			case TrialState.Instruction2: return "<size=100>Additionally, you will\nhear the teacher\nsay a color.\n\n</size><size=90><i>Press any key to continue.</i></size>";
			case TrialState.Instruction3: return "<size=100>Your goal is\nto let the\nteacher know if\nshe read the\nfont color correctly.</size>\n\n<size=90><i>Press any key to continue.</i></size>";	
			case TrialState.Instruction4: return "<size=100>If she is\ncorrect press <b>1</b>.\nIf she is\nincorrect press <b>2</b>.</size>\n\n<size=90><i>Press any key to continue.</i></size>";
			case TrialState.Instruction5: return "<size=100>You must respond\nbefore the next\nword appears.</size>\n\n<size=90><i>Press any key to continue.</i></size>";
			case TrialState.Ready:        return "<size=100><b>Remember:\npress 1 if\nshe is right\nand press 2 if\nshe is wrong.</b></size>\n\n<size=90><b>Press 1 to begin task.</b></size>";
			default:                      return "";
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

	public class VerbalStroop : MonoBehaviour {
		public ShowText whiteboardText;
		public ShowImage whiteboardImage;
		public TrialList trials;
		public Textures textures;
		public RecordResponses recorder;

		private TrialState trialState;
		private int currentTrial;

		private CountdownTimer timer;
		private CSVWriter recordResults;

		void Start () {
			currentTrial = 0;
			trialState = TrialState.Starting;
			this.timer = new CountdownTimer (-1);
			recordResults = new CSVWriter ("verbal_stroop_results.csv");
			recordResults.WriteRow ("trial_number,trial_properties,button_pressed,reaction_time");
			print ("Starting Verbal Stroop");
            whiteboardText = GameObject.Find("WhiteBoardWithDisplay").GetComponent<ShowText>();
            whiteboardImage = GameObject.Find("WhiteBoardWithDisplay").GetComponent<ShowImage>();
            whiteboardText.SetText (trialState.Instructions());
			whiteboardText.Show ();
		}

		void Update () {
			var finishInstructions = trialState == TrialState.Ready && Input.GetButtonDown ("Button1");
			var finishState = (int)trialState >= (int)TrialState.Word && (int)trialState <= (int)TrialState.ITI && timer.isComplete;

			if (((int)trialState < (int)TrialState.Ready) && Input.anyKeyDown) {
				trialState = trialState.Next ();
				whiteboardText.SetText (trialState.Instructions());
				return;
			} 
			if (finishInstructions || finishState) {
				if (trialState == TrialState.ITI) {
					currentTrial++;

					var response = recorder.StopRecording ();
					var output = new TrialOutput (currentTrial, trials.trialProperties [currentTrial], response);
					recordResults.WriteRow (output.ToString());

					if (currentTrial == trials.trialProperties.Length) {
						trialState = TrialState.Ending;
						recordResults.Close ();
						whiteboardText.Hide ();
						whiteboardImage.Hide ();
						return;
					}
				}

				trialState = trialState.Next ();
				print ("Starting state " + trialState);
				switch (trialState) {
				case TrialState.Word:
					whiteboardText.SetText (trials.trialProperties [currentTrial].text.ToString ());
					whiteboardText.SetColor (trials.trialProperties [currentTrial].color.GetColor ());
					whiteboardImage.Hide ();
					whiteboardText.Show ();
					break;
				case TrialState.ITI:
					whiteboardImage.SetTexture (textures.iti);
					whiteboardText.Hide ();
					whiteboardImage.Show ();
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
}

