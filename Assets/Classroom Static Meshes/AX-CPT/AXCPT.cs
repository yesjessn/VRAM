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

	public class AXCPT : MonoBehaviour {
		public ShowImage whiteboard;
		public TrialList trials;
		public Textures textures;

		private int currentTrial;
		private TrialState trialState;
		private CountdownTimer timer;

		// Use this for initialization
		void Start () {
			currentTrial = 0;
			trialState = TrialState.Starting;
			timer = new CountdownTimer (-1);
			whiteboard.Show ();
			print ("Starting AX-CPT");
		}
		
		// Update is called once per frame
		void Update () {
			if (trialState != TrialState.Ending && timer.isComplete) {
				currentTrial++;
				if (trialState == TrialState.ITI && currentTrial == trials.trialTypes.Length) {
					trialState = TrialState.Ending;
					return;
				}
				trialState = trialState.Next ();
				print ("Starting state " + trialState);

				whiteboard.img.texture = trialState.GetTexture(trials.trialTypes[currentTrial], textures);

				timer.duration = trialState.Duration ();
				timer.Start ();
			}
		}
	}
}
