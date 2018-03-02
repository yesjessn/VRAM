using System;
using UnityEngine;

namespace VerbalStroop {
	public class AvatarController : MonoBehaviour {
		public AvatarTalkController controller;

		public AudioClip redClip;
		public AudioClip blueClip;
		public AudioClip greenClip;
		public string redTrigger;
		public string blueTrigger;
		public string greenTrigger;

		void Awake() {
			controller = GameObject.Find ("NewestTeacher").GetComponent<AvatarTalkController> ();
		}


		public void Play(TrialSound sound) {
			switch (sound) {
			case TrialSound.Red:
				controller.PlayClipWithAnimation (redClip, redTrigger, null);
				break;
			case TrialSound.Blue:
				controller.PlayClipWithAnimation (blueClip, blueTrigger, null);
				break;
			case TrialSound.Green:
				controller.PlayClipWithAnimation (greenClip, greenTrigger, null);
				break;
			}
		}
	}
}

