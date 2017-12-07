using UnityEngine;
using SMI;

namespace SMI {
    public class Calibrater : MonoBehaviour {
        private bool _toggleValidationWindow = false;

        void OnCalibrationResult(int returnCode) {
            Debug.Log("SMI HMD calibration result: " + SMI.SMIEyeTrackingUnity.ErrorIDContainer.getErrorMessage(returnCode));
        }

        void Update() {
            if (Input.GetKeyDown(UnityEngine.KeyCode.C)) {
                SMI.SMIEyeTrackingUnity.Instance.smi_StartFivePointCalibration();
                SMI.SMIEyeTrackingUnity.Instance.SetCalibrationReturnCallback(OnCalibrationResult);
                
                //SMI.SMIEyeTrackingUnity.Instance.smi_StartNumericalValidation();
            }

            if (Input.GetKeyDown(UnityEngine.KeyCode.V)) {
                if(!_toggleValidationWindow)
                {
                    _toggleValidationWindow = !_toggleValidationWindow;
                    //SMI.SMIEyeTrackingUnity.Instance.smi_ShowValidation();
                    SMI.SMIEyeTrackingUnity.Instance.smi_StartNumericalValidation();
                }
                else
                {
                    _toggleValidationWindow = !_toggleValidationWindow;
                    SMI.SMIEyeTrackingUnity.Instance.smi_CloseVisualization();
                }
            }
        }
    }
}