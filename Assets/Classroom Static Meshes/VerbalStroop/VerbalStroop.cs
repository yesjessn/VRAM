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

	public enum TrialState { Starting, Word, ITI, Ending };

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
			case TrialState.Word: return 2.0f;
			case TrialState.ITI:  return 1.0f;
			default:              return -1f;
			}
		}

		public static TrialState Next(this TrialState state) {
			switch (state) {
			case TrialState.Starting: return TrialState.Word;
			case TrialState.Word:     return TrialState.ITI;
			case TrialState.ITI:      return TrialState.Word;
			default:                  return TrialState.Ending;
			}
		}
	}

	public class TrialOutput {
		public readonly int num;
		public readonly TrialProperties properties;
		public readonly RecordResponses.Response response;

		public TrialOutput(int num, TrialProperties properties, RecordResponses.Response response) {
			this.num = num;
			this.properties = properties;
			this.response = response;
		}

		public override string ToString() {
			return num.ToString () + "," +
				properties.text.ToString () + "," +
				properties.color.ToString() + "," +
				properties.sound.ToString() + "," +
				response.buttonPressed + "," +
				(response == RecordResponses.EMPTY_RESPONSE ? "" : response.responseTime.ToString ());
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
			timer = new CountdownTimer (-1);
			recordResults = new CSVWriter ("verbal_stroop_results.csv");
			recordResults.WriteRow ("trial_number,trial_properties,button_pressed,reaction_time");
			print ("Starting Verbal Stroop");
		}

		void Update () {
			if (trialState != TrialState.Ending && timer.isComplete) {
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

