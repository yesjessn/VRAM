﻿using UnityEngine;
using System.Collections;
using SMI;

namespace SMI
{
    public class Gazer : GazeAction
    {
        //public GameObject hitObject;
        private string objectNameOverride = "";
        private string distractionType = "";
        public bool timing = false;
        public float timer = 0.0f;

        private EyeTrackingData dataWriter;

        // Use this for initialization
        public override void Start() {
        }

        private EyeTrackingData getEyeTracker() {
            if (this.dataWriter == null) {
                var eyeTrackingData = GameObject.Find("DistractionController");
                if (eyeTrackingData != null) {
                    this.dataWriter = eyeTrackingData.GetComponent(typeof(EyeTrackingData)) as EyeTrackingData;
                }
            }
            return dataWriter;
        }

        private void addDataEvent(EyeTrackingData.Event e) {
            var writer = getEyeTracker();
            if (writer == null) {
                return;
            }
            writer.AddEvent(e);
        }

        private void setHitObject(GameObject obj) {
            var writer = getEyeTracker();
            if (writer == null) {
                return;
            }
            writer.SetHitObject(obj);
        }

        private void clearHitObject() {
            var writer = getEyeTracker();
            if (writer == null) {
                return;
            }
            writer.ClearHitObject();
        }

		public void SetDistractionParams(string distractionType, string distractionName) {
			this.distractionType = distractionType;
			this.objectNameOverride = distractionName;
		}

        public override void OnGazeEnter(RaycastHit hitInformation)
        {
            //If this game object has a parent GazeAction object, its OnGazeEnter must be called.
            if (gazeParent != null)
            {
                gazeParent.OnGazeEnter(hitInformation);
                isGazed = true;
            }
            else
            {
                isGazed = true;
                setHitObject(this.transform.gameObject);
                if(string.IsNullOrEmpty(distractionType))
                {
                    addDataEvent(EyeTrackingData.Event.EnterGaze);
                }
                else
                {
                    addDataEvent(new EyeTrackingData.Event(EyeTrackingData.EventType.EnterDistraction, distractionType, objectNameOverride));
                    timer = 0.0f;
                    timing = true;
                }
            }
        }

        public override void OnGazeStay(RaycastHit hitInformation)
        {
            //If this game object has a parent GazeAction object, its OnGazeStay must be called.
            if (gazeParent != null)
            {
                gazeParent.OnGazeStay(hitInformation);
            }
            else
            {
                
            }
        }

        public override void OnGazeExit()
        {
            //If this game object has a parent GazeAction object, its OnGazeExit must be called.
            if (gazeParent != null)
            {
                gazeParent.OnGazeExit();
                isGazed = false;
            }
            else
            {
                isGazed = false;
                if (string.IsNullOrEmpty(distractionType))
                {
                    addDataEvent(EyeTrackingData.Event.ExitGaze);
                }
                else
                {
                    addDataEvent(new EyeTrackingData.Event(EyeTrackingData.EventType.ExitDistraction, distractionType, objectNameOverride, timer));
                    timing = false;
                    timer = 0.0f;
                }

                clearHitObject();
            }
        }

        protected override void DoAction()
        {
            /*if (particle != null)
            {
                //When the particle object is given, emit the particles by instantiating the particle object
                RaycastHit rayHit;
                bool hasAHitpoint = SMI.SMIEyeTrackingUnity.Instance.smi_GetRaycastHitFromGaze(out rayHit);
                if (hasAHitpoint)
                {
                    Instantiate(particle, rayHit.point, Quaternion.identity);
                }
            }*/

        }

        // Update is called once per frame
        public override void Update()
        {
            if(timing)
            {
                timer += Time.deltaTime;
            }
        }
    }
}
