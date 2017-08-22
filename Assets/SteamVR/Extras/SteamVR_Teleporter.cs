//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using System.Collections;

public class SteamVR_Teleporter : MonoBehaviour
{
    public enum TeleportType
    {
        TeleportTypeUseTerrain,
        TeleportTypeUseCollider,
        TeleportTypeUseZeroY
    }

    public bool teleportOnClick = false;
    public TeleportType teleportType = TeleportType.TeleportTypeUseZeroY;

	Transform reference
	{
		get
		{
			var top = SteamVR_Render.Top();
			return (top != null) ? top.origin : null;
		}
	}

	void Start ()
    {
		var trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
			trackedController = gameObject.AddComponent<SteamVR_TrackedController>();
        }

		//trackedController.PadClicked += new ClickedEventHandler(DoClick);

        if (teleportType == TeleportType.TeleportTypeUseTerrain)
        {
			// Start the player at the level of the terrain
			var t = reference;
			if (t != null)
	            t.position = new Vector3(t.position.x, Terrain.activeTerrain.SampleHeight(t.position), t.position.z);
        }
	}

    //Added By Zack ------------------------------------------------------------------------
    public void DoTeleport(Vector3 teleportTargetPoint)
    {
        /*  Vector3 rayDirection = new Vector3((teleportTargetPoint.x - this.transform.position.x), 0f, (teleportTargetPoint.z - this.transform.position.z));
          float dist = Vector3.Distance(this.transform.position, teleportTargetPoint);
          Vector3 headPosOnGround = new Vector3(SteamVR_Render.Top().head.localPosition.x, 0.0f, SteamVR_Render.Top().head.localPosition.z);
          reference.position = this.transform.position + rayDirection * dist - new Vector3(reference.GetChild(0).localPosition.x, 0f, reference.GetChild(0).localPosition.z) - headPosOnGround;*/
        //Debug.LogError("teleport pos = " + teleportTargetPoint);
        if(teleportTargetPoint != Vector3.zero)
        {
            reference.position = teleportTargetPoint;
        }
        
    }
    //--------------------------------------------------------------------------------------
	
    /*void DoClick(object sender, ClickedEventArgs e)
    {
        if (teleportOnClick)
        {
			var t = reference;
			if (t == null)
				return;

            float refY = t.position.y;

            Plane plane = new Plane(Vector3.up, -refY);
            Ray ray = new Ray(this.transform.position, transform.forward);

            bool hasGroundTarget = false;
            float dist = 0f;
            if (teleportType == TeleportType.TeleportTypeUseTerrain)
            {
                RaycastHit hitInfo;
                TerrainCollider tc = Terrain.activeTerrain.GetComponent<TerrainCollider>();
                hasGroundTarget = tc.Raycast(ray, out hitInfo, 1000f);
                dist = hitInfo.distance;
            }
            else if (teleportType == TeleportType.TeleportTypeUseCollider)
            {
                RaycastHit hitInfo;
                Physics.Raycast(ray, out hitInfo);
                dist = hitInfo.distance;
            }
            else
            {
                hasGroundTarget = plane.Raycast(ray, out dist);
            }

            if (hasGroundTarget)
            {
				Vector3 headPosOnGround = new Vector3(SteamVR_Render.Top().head.localPosition.x, 0.0f, SteamVR_Render.Top().head.localPosition.z);
				t.position = ray.origin + ray.direction * dist - new Vector3(t.GetChild(0).localPosition.x, 0f, t.GetChild(0).localPosition.z) - headPosOnGround;
            }
        }
    }*/
}
