using UnityEngine;
using System.Collections;
using SMI;
namespace SMI
{
    public class Gazer : GazeAction
    {
        //public GameObject hitObject;
        public string objectNameOverride = "";
        public string distractionType = "";
        public bool timing = false;
        public float timer = 0.0f;
        // Use this for initialization
        public override void Start()
        {
            
        }

        public override void OnGazeEnter(RaycastHit hitInformation)
        {
            //If this game object has a parent GazeAction object, its OnGazeEnter must be called.
            if (gazeParent != null)
            {
                gazeParent.OnGazeEnter(hitInformation);
                isGazed = true;
               // MasterDataController.instance.hitObject = hitInformation.transform.gameObject;
            }
            else
            {
                isGazed = true;
                MasterDataController.instance.hitObject = this.transform.gameObject;
                if(string.IsNullOrEmpty(distractionType))
                {
                    MasterDataController.instance.SaveData("Enter Gaze");
                }
                else
                {
                    MasterDataController.instance.SaveDataEnterDistraction("Distraction Entered", distractionType, objectNameOverride);
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
                //MasterDataController.instance.SaveData("Exit Gaze");
                if (string.IsNullOrEmpty(distractionType))
                {
                    MasterDataController.instance.SaveData("Exit Gaze");
                }
                else
                {
                    MasterDataController.instance.SaveDataExitDistraction("Distraction Exited", distractionType, objectNameOverride, timer);
                    timing = false;
                    timer = 0.0f;
                }

                if (MasterDataController.instance.hitObject == this.gameObject)
                {
                    MasterDataController.instance.hitObject = null;
                }
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
