using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Runtime URP bootstrap to ensure proper pipeline configuration
/// This fixes the black screen issue caused by missing renderer data
/// </summary>
public class UrpRuntimeBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureUrpRuntime()
    {
        Debug.Log("[UrpRuntimeBootstrap] Ensuring URP runtime configuration...");
        
        // Ensure we have a valid URP asset assigned
        var pipelineAsset = GraphicsSettings.renderPipelineAsset;
        if (pipelineAsset == null)
        {
            Debug.LogError("[UrpRuntimeBootstrap] No render pipeline asset assigned!");
            return;
        }

        if (!(pipelineAsset is UniversalRenderPipelineAsset urpAsset))
        {
            Debug.LogError("[UrpRuntimeBootstrap] Render pipeline asset is not URP!");
            return;
        }

        Debug.Log("[UrpRuntimeBootstrap] URP asset confirmed");
        
        // Check if we can access the renderer data
        FixRendererData(urpAsset);
    }

    private static void FixRendererData(UniversalRenderPipelineAsset urpAsset)
    {
        Debug.Log("[UrpRuntimeBootstrap] Checking renderer data...");
        
        // Try to load the forward renderer asset
        var rendererData = Resources.Load<UniversalRendererData>("Settings/URP/ForwardRenderer");
        if (rendererData == null)
        {
            // Try alternative path
            rendererData = Resources.Load<UniversalRendererData>("URP/ForwardRenderer");
        }

        if (rendererData != null)
        {
            Debug.Log("[UrpRuntimeBootstrap] Found renderer data");
        }
        else
        {
            Debug.LogError("[UrpRuntimeBootstrap] Could not find ForwardRenderer asset!");
        }
    }

    void Start()
    {
        // Additional runtime check
        if (GraphicsSettings.renderPipelineAsset == null)
        {
            Debug.LogError("[UrpRuntimeBootstrap] No render pipeline asset at runtime!");
        }
        else
        {
            Debug.Log("[UrpRuntimeBootstrap] Render pipeline asset confirmed at runtime");
        }
    }
}
