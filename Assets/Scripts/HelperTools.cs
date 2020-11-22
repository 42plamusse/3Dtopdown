using UnityEngine;

public static class HelperTools
{
    public static void ToggleChildrenRenderers(GameObject target, bool enabled)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = enabled;
        }
    }
}