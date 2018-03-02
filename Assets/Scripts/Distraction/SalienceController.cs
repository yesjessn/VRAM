using UnityEngine;
using System;

namespace Distraction {
    public class SalienceController : MonoBehaviour {
        public static SalienceController instance;
    
        // These are the parameters to the algorithm
        [Tooltip("The size of salience update after each behaviour.")]
        public float salienceStepSize       = 0.1f;
        [Tooltip("If the weighted running average of correct responses rises above this percentage (0 to 1), start increasing the salience.")]
        public double correctMaxVal          = 0.95;
        [Tooltip("If the weighted running average of correct responses falls below this percentage (0 to 1), start decreasing the salience.")]
        public double correctMinVal          = 0.85;
        [Tooltip("How much weight (0 to 1) of the past is considered when updating after each response. Closer to 1 means the running average updates more slowly.")]
        public double runningAverageMomentum = 0.9;

        private double runningAverage;

        public float salience = 0.5f;
        
        void Awake() {
            if(instance != null) {
                Destroy(this.gameObject);
            } else {
                instance = this;
            }
        }

        void Start() {
            ResetRunningAverage();
        }

        public void ResetRunningAverage() {
            runningAverage = (correctMinVal + correctMaxVal)/2;
        }

        public void addResponseResult(bool wasCorrect) {
            runningAverage = runningAverage * runningAverageMomentum;
            if (wasCorrect) {
                runningAverage += (1 - runningAverageMomentum);
                if(runningAverage > correctMaxVal) {
                    salience += salienceStepSize;
                }
            } else {
                if(runningAverage < correctMinVal) {
                    salience -= salienceStepSize;
                    if (salience < 0) {
                        salience = 0f;
                    }
                }
            }
            Debug.Log(String.Format("Updating salience to {0:N2} (runningAverage={1:N2}, wasCorrect={2})", salience, runningAverage, wasCorrect));
        }
    }
}