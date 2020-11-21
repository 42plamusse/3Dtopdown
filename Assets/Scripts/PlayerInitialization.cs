using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInitialization : NetworkBehaviour
{
    [Header("References")]
    //public MeshRenderer fieldOfViewVisualisationMesh;
    public MeshRenderer characterMesh;
    public GameObject character;

    private void Start()
    {
        transform.name = "Player" + netId;
        if (!isLocalPlayer)
        {
            // disable all renderers
            toggleSelfChildrenRenderers(false);
        }
    }
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CameraFollow cameraFollowScript =
            Camera.main.GetComponent<CameraFollow>();
        cameraFollowScript.target = transform; //Fix camera on "me"
        cameraFollowScript.enabled = true;

        // Localplayer is on Default ; Others are on Hideable
        character.layer = 0;
        // enable all renderers
        toggleSelfChildrenRenderers(true);
    }

    private void toggleSelfChildrenRenderers(bool enabled)
    {
        Renderer[] playerRenderers = gameObject.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < playerRenderers.Length; i++)
        {
            playerRenderers[i].enabled = enabled;
        }
    }
}
