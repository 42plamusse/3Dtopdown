using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Hideable : NetworkBehaviour
{
    private void Start()
    {
        if (!isLocalPlayer)
        {
            ToggleRenderers(enabled = false);
        }
    }

    public void ToggleRenderers(bool enabled)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = enabled;
        }
    }
}
