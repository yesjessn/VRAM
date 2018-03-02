using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Distraction;
using System.Linq;

namespace Math {
	public enum BlockType {Easy, Medium};

	public enum TrialState { Starting, Instruction1, Ready, Problem, ITI, Correct, Incorrect, Slow, Ending };

	public static class TrialStateExtensions {
		public static float Duration(this TrialState state) {
			switch (state) {
			case TrialState.Problem: return 30.0f;
			case TrialState.ITI:     return  2.0f;
			default:                 return -1.0f;
			}
		}

		public static bool isInstruction(this TrialState state) {
			var beforeReady = state < TrialState.Ready;
			var isPracticeFeedback = state == TrialState.Correct || state == TrialState.Incorrect || state == TrialState.Slow;
			return beforeReady || isPracticeFeedback;
		}

		public static string Instructions(this TrialState state){
			switch (state) {
			case TrialState.Starting:     return "<size=60>Math\nTask\n\nFor each trial,\nyou will see\na math problem.</size>";
			case TrialState.Instruction1: return "<size=60>The problems are\nin multiple choice\nformat.</size>";
			case TrialState.Ready:        return "<size=60><b>You only have\n30 seconds to\ncomplete the\nproblem before\nthe next\nquestion appears.</b></size>\n\n<size=30><i>Coordinator begin task.</i></size>";
			case TrialState.Correct:      return "<size=60>Correct!</size>\n\n<size=30><i>Press any key to continue.</i></size>";
			case TrialState.Incorrect:    return "<size=60>Incorrect.\nTry again!</size>\n\n<size=30><i>Press any key to continue.</i></size>";
			case TrialState.Slow:         return "<size=60><b>Too slow!</b>\nYou must respond\nbefore the next\nword appears.</size>\n\n<size=30><i>Press any key to continue.</i></size>";
			default:                      return "";
			}
		}

		public static TrialState Next(this TrialState state) {
			switch (state) {
			case TrialState.Starting:     return TrialState.Instruction1;
			case TrialState.Instruction1: return TrialState.Ready;
			case TrialState.Ready:        return TrialState.ITI;
			case TrialState.Problem:      return TrialState.ITI;
			case TrialState.ITI:          return TrialState.Problem;
			case TrialState.Correct:      return TrialState.ITI;
			case TrialState.Incorrect:    return TrialState.ITI;
			case TrialState.Slow:         return TrialState.ITI;
			default:                      return TrialState.Ending;
			}
		}

		public static Texture GetTexture(this TrialState state, BlockType type, Textures textures) {
			switch (state) {
			case TrialState.Problem: return textures.Get(type);
			case TrialState.ITI:     return textures.iti;
			default:                 return null;
			}
		}
	}

	public class TrialOutput {
		public readonly int trialNum;
		public readonly int blockNum;
		public readonly BlockType type;
		public readonly List<RecordResponses.Response> response;

		public TrialOutput(int trialNum, int blockNum, BlockType type, List<RecordResponses.Response> response) {
			this.trialNum = trialNum;
			this.blockNum = blockNum;
			this.type = type;
			this.response = response;
		}

		public override string ToString() {
			var rows = new List<string> ();
			foreach (RecordResponses.Response r in response) {
				rows.Add(trialNum.ToString () + "," +
				blockNum.ToString() + "," +
				type.ToString () + "," +
				r.buttonPressed + "," +
				r.responseTime.ToString ());
			}
			if (response.Count == 0) {
				return trialNum.ToString () + "," +
					blockNum.ToString() + "," +
					type.ToString () + ",,";
			}
			return String.Join("\n", rows.ToArray ());
		}
	}

	public class Math : VRAMTask {
		const int NumberOfBlocks = 6;
		const float BlockTime = 3 * 60.0f;

        public Textures textures;
		public RecordResponses recorder;
		public MathProblemChecker checker;

		private TrialState trialState;
		private int currentTrial;
		private BlockType type;
		private int currentBlock;
		private CountdownTimer trialTimer;
		private CountdownTimer blockTimer;
		private CSVWriter recordResults;
		private MathPractice practice;
		private string currentProblemName;

		private Textures mathTextures {
			get {
				if (practice.enabled) {
					return practice.textures;
				} else {
					return textures;
				}
			}
		}

		protected void Awake () {
			base.Awake();
			practice = GetComponent<MathPractice> ();
		}

		void OnEnable() {
			base.OnEnable();
			currentTrial = -1; // Start at -1 because we start the trial into ITI which will increment currentTrial
			currentBlock = 0;
			type = BlockType.Easy;
			trialState = TrialState.Starting;
			trialTimer = new CountdownTimer (-1);
			blockTimer = new CountdownTimer (BlockTime);
			recordResults = CSVWriter.NewOutputFile(subject, "math_results");
			recordResults.WriteRow ("trial_number,block_number,trial_item,button_pressed,reaction_time");
			if (practice != null) {
				practice.SetProblemChecker(checker);
			}
		}

		void OnDisable() {
			recordResults.Close();
		}

		void Update () {
			if (trialState == TrialState.Ending) {
				EndTask();
				return;
			} else if (trialState == TrialState.Starting) {
				print ("Starting Math");
				ShowText (trialState.Instructions());
				trialState = trialState.Next();
				return;
			}
			var finishReady = trialState == TrialState.Ready && input.GetButtonDown ("Button3");
			var finishInstructions = trialState.isInstruction() && (input.GetButtonDown ("Button1") || input.GetButtonDown ("Button2") || input.GetButtonDown ("Button4"));
			var finishState = (int)trialState > (int)TrialState.Ready && (int)trialState <= (int)TrialState.ITI && (trialTimer.isComplete || recorder.hasResponse);

			if (finishInstructions || finishReady || finishState) {
				if (finishReady) {
					distractionController.gameObject.SetActive (true);
				}

				if (trialState != TrialState.Ending) {
					var blockComplete = blockTimer.isComplete;
					if (blockComplete) {
						currentBlock++;
						if (currentBlock == NumberOfBlocks) {
							trialState = TrialState.Ending;
							recordResults.Close ();
							return;
						}
						switch (type) {
						case BlockType.Easy:
							type = BlockType.Medium;
							break;
						case BlockType.Medium:
							type = BlockType.Easy;
							break;
						}
						blockTimer.Start ();
					}
					
					Option<TrialState> nextState = Option<TrialState>.CreateEmpty ();
					if (blockComplete || finishState || finishInstructions || finishReady) {
						if (trialState == TrialState.Problem) {
							var responses = recorder.StopRecording ();
							salienceController.addResponseResult(checker.Check(currentProblemName, responses.Count > 0 ? responses.Last().buttonPressed : null));
							if (practice.enabled) {
								nextState = practice.HandleResponse (trialState, responses, currentProblemName);
							} else {
								var output = new TrialOutput (currentTrial, currentBlock, type, responses);
								recordResults.WriteRow (output.ToString ());
							}
						}
						if (nextState.Count () > 0) {
							trialState = nextState.First ();
							if (trialState != TrialState.Correct) {
								currentTrial--;
							}
						} else {
							trialState = trialState.Next ();
						}
						//print ("Starting state " + trialState);

						var instruction = trialState.Instructions ();
						if (instruction != "") {
							ShowText (instruction);
						} else {
							var currentProblem = trialState.GetTexture (type, mathTextures);
							currentProblemName = currentProblem.name;
							ShowImage (currentProblem);

							trialTimer.duration = trialState.Duration ();
							if (trialState == TrialState.Problem) {
								currentTrial++;
								recorder.StartRecording ();
							}
							trialTimer.Start ();
						}
					}
				}
			}
		}

		public override string GetCurrentState() {
			return trialState.ToString();
		}
	}
}
