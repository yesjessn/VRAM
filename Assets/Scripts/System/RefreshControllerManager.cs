using UnityEngine;
using System;

public class RefreshControllerManager : MonoBehaviour {
	void Start() {
        var mgr = GetComponent<SteamVR_ControllerManager>();
        if (mgr != null) {
            mgr.right.SetActive(true);
        }
    }
}