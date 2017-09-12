using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Math {
	public enum BlockType {Easy, Medium};

	public enum TrialState { Starting, Problem, ITI, Ending };

	public static class TrialStateExtensions {
		public static float Duration(this TrialState state) {
			switch (state) {
			case TrialState.Problem: return 10.0f;
			case TrialState.ITI:     return  2.0f;
			default:                 return -1.0f;
			}
		}

		public static TrialState Next(this TrialState state) {
			switch (state) {
			case TrialState.Starting: return TrialState.Problem;
			case TrialState.Problem:  return TrialState.ITI;
			case TrialState.ITI:      return TrialState.Problem;
			default:                  return TrialState.Ending;
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

	public class Math : MonoBehaviour {
		const int NumberOfBlocks = 6;
		const float BlockTime = 3 * 60.0f;

		public ShowImage whiteboard;
        public ShowText whiteboardText;

        public Textures textures;
		public RecordResponses recorder;

		private TrialState trialState;
		private int currentTrial;
		private BlockType type;
		private int currentBlock;
		private CountdownTimer trialTimer;
		private CountdownTimer blockTimer;
		private CSVWriter recordResults;

		void Start () {
			currentTrial = 0;
			currentBlock = 0;
			type = BlockType.Easy;
			trialState = TrialState.Starting;
			trialTimer = new CountdownTimer (-1);
			blockTimer = new CountdownTimer (BlockTime);
            whiteboardText = GameObject.Find("WhiteBoardWithDisplay").GetComponent<ShowText>();
            whiteboard = GameObject.Find("WhiteBoardWithDisplay").GetComponent<ShowImage>();
            recordResults = new CSVWriter ("math_results.csv");
			recordResults.WriteRow ("trial_number,block_number,trial_item,button_pressed,reaction_time");
			print ("Starting Math");
		}

		void Update () {
			if (trialState != TrialState.Ending) {
				var blockComplete = blockTimer.isComplete;
				if (blockComplete) {
					currentBlock++;
					if (currentBlock == NumberOfBlocks) {
						trialState = TrialState.Ending;
						recordResults.Close ();
						whiteboard.Hide ();
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
				if (trialTimer.isComplete || blockComplete || recorder.hasResponse) {
					if (trialState == TrialState.Problem) {	
						var response = recorder.StopRecording ();
						var output = new TrialOutput (currentTrial, currentBlock, type, response);
						recordResults.WriteRow (output.ToString());
					}
					trialState = trialState.Next ();
					print ("Starting state " + trialState);

					whiteboard.SetTexture(trialState.GetTexture(type, textures));
					whiteboard.Show ();

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
