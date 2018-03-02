using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SMI;

namespace Distraction {
	public enum DistractionTypes{ Audio, Visual, AudioVisual};

    public class Distraction : MonoBehaviour {
        [SerializeField]
        private AudioSource soundSource;
        [SerializeField]
        private AudioClip soundClip;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Collider distractionCollider;
        [SerializeField]
        private Gazer gazer;
        [SerializeField]
        private Gazer gazerToModify;

        public string animTriggerName;
		public string distractionName;
		public string distractionObjectName;

        private Queue<Action> _actionQueue = new Queue<Action>();

		private DistractionTypes distractionType {
			get {
				if (soundClip != null && animTriggerName != "") {
					return DistractionTypes.AudioVisual;
				} else if (soundClip != null) {
					return DistractionTypes.Audio;
				} else {
					return DistractionTypes.Visual;
				}
			}
		}

        void Start() {
			if (distractionName == null || distractionName == "") {
				UnityEngine.Debug.LogError(String.Format("{0} distraction on {1} is missing name!", distractionType, gameObject.name), gameObject);
			}
			distractionObjectName = gameObject.gameObject.name;
		}

        public void TriggerDistraction(float salience, Action callback) {
			activateCollider ();
			updateGazer ();
			var animDuration = triggerAnimation ();
			var soundDuration = playSound (salience);
			var maxDuration = System.Math.Max (animDuration, soundDuration);
			Debug.Log(String.Format("Starting distraction: {0} with salience {1:N2}", distractionName, salience), this.gameObject);
			setupOnComplete (callback, maxDuration);
        }

		private void activateCollider() {
			distractionCollider.gameObject.SetActive(true);
		}

		private void updateGazer() {
			gazer.SetDistractionParams (distractionType.ToString (), distractionName);
			if (gazerToModify != null) {
				gazerToModify.gazeParent = gazer;
			}
		}

		private float playSound(float salience) {
			if (distractionType == DistractionTypes.Audio || distractionType == DistractionTypes.AudioVisual) {
				var volume = salience;
				soundSource.PlayOneShot (soundClip, volume);
				return soundClip.length;
			}
			return 0;
		}

		private float triggerAnimation() {
			if (distractionType == DistractionTypes.Visual || distractionType == DistractionTypes.AudioVisual || animator != null) {
				animator.SetTrigger(animTriggerName);
				AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
				return state.length;
			}
			return 0;
		}

		private void setupOnComplete(Action callback, float duration) {
			if (callback != null) {
				StartCoroutine (WaitForToFinish (callback, duration));
			}
		}

		IEnumerator WaitForToFinish(Action callback, float waitDuration) {
            yield return new WaitForSeconds(waitDuration);
			_actionQueue.Enqueue(callback);
        }

        // Update is called once per frame
        void Update() {
            if(_actionQueue.Count > 0) {
                Action a = _actionQueue.Dequeue();
                a();
            }
        }
    }
}
