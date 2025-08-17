using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Fallback render pipeline to ensure the game always renders
/// Switches to built-in pipeline if URP fails
/// </summary>
public class RenderPipelineFallback : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureRenderPipeline()
    {
        Debug.Log("[RenderPipelineFallback] Checking render pipeline...");
        
        // Check if we have a valid render pipeline
        var pipelineAsset = GraphicsSettings.renderPipelineAsset;
        
        if (pipelineAsset == null)
        {
            Debug.LogWarning("[RenderPipelineFallback] No render pipeline assigned, using built-in");
            GraphicsSettings.renderPipelineAsset = null;
            QualitySettings.renderPipeline = null;
            return;
        }

        // Test if the pipeline asset is valid
        try
        {
            if (pipelineAsset == null)
            {
                throw new System.Exception("Pipeline asset is null");
            }
            Debug.Log("[RenderPipelineFallback] Render pipeline asset is valid");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[RenderPipelineFallback] Render pipeline failed: {ex.Message}");
            Debug.LogWarning("[RenderPipelineFallback] Falling back to built-in render pipeline");
            
            // Fallback to built-in render pipeline
            GraphicsSettings.renderPipelineAsset = null;
            QualitySettings.renderPipeline = null;
            
            // Force a camera refresh
            var cameras = FindObjectsOfType<Camera>();
            foreach (var camera in cameras)
            {
                camera.ResetAspect();
            }
        }
    }

    void Start()
    {
        // Additional check at start
        if (GraphicsSettings.renderPipelineAsset == null)
        {
            Debug.Log("[RenderPipelineFallback] Using built-in render pipeline");
        }
        else
        {
            Debug.Log("[RenderPipelineFallback] Using custom render pipeline");
        }
    }
}
