using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMI
{
    public class Distraction : MonoBehaviour
    {
        public enum DistractionTypes{ Audio, Visual, AudioVisual};
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
        public DistractionTypes distractionType = DistractionTypes.Audio;
        public float volume = 0.5f;
        public string animTriggerName = "Loop1";

        private Queue<Action> _actionQueue = new Queue<Action>();
        private Action _savedAction = null;

        // Use this for initialization
        void Start()
        {

        }

        public void TriggerDistraction(Action callback)
        {
            if(callback != null)
            {
                _savedAction = callback;
            }

            switch(distractionType)
            {
                case DistractionTypes.Audio:
                    TriggerAudioDistraction();
                    break;
                case DistractionTypes.Visual:
                    TriggerVisualDistraction();
                    break;
                case DistractionTypes.AudioVisual:
                    TriggerAudioVisualDistraction();
                    break;
            }
        }

        private void TriggerAudioDistraction()
        {
            gazer.distractionType = "Audio";
            distractionCollider.gameObject.SetActive(true);
            if(animator != null)
            {
                animator.SetTrigger(animTriggerName);
            }
            //----Volume scales from 0 - 1, but can be > 1. If so it might distort the sound----//
            soundSource.PlayOneShot(soundClip,volume);
            StartCoroutine(WaitForToFinish(soundClip.length));
        }

        private void TriggerVisualDistraction()
        {
            distractionCollider.gameObject.SetActive(true);
            if(gazerToModify != null)
                gazerToModify.gazeParent = gazer;
            gazer.distractionType = "Visual";
            animator.SetTrigger(animTriggerName);
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            float clipLength = state.length;
            StartCoroutine(WaitForToFinish(clipLength));
        }

        private void TriggerAudioVisualDistraction()
        {
            distractionCollider.gameObject.SetActive(true);
            if (gazerToModify != null)
                gazerToModify.gazeParent = gazer;
            gazer.distractionType = "AudioVisual";
            animator.SetTrigger(animTriggerName);
            //----Volume scales from 0 - 1, but can be > 1. If so it might distort the sound----//
            soundSource.PlayOneShot(soundClip, volume);
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            float clipLength = state.length;
            StartCoroutine(WaitForToFinish(clipLength));
        }

        IEnumerator WaitForToFinish(float waitDuration)
        {
            yield return new WaitForSeconds(waitDuration);
            if(_savedAction != null)
                _actionQueue.Enqueue(_savedAction);
        }

        // Update is called once per frame
        void Update()
        {
            if(_actionQueue.Count > 0)
            {
                Action a = _actionQueue.Dequeue();
                a();
            }
        }
    }
}
