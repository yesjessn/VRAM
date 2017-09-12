using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMI
{
    public class DistractionController : MonoBehaviour
    {

        [SerializeField]
        public List<Distraction> distractors = new List<Distraction>();

        // Use this for initialization
        void Start()
        {

        }

        void DistractorCallback()
        {
            Debug.Log("------------------------Finished Distraction----------------------");
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(UnityEngine.KeyCode.D))
            {
                distractors[3].TriggerDistraction(DistractorCallback);
            }
        }
    }
}
