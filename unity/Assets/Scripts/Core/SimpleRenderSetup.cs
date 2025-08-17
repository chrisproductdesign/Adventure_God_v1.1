using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Simple render setup that ensures the game renders properly
/// Uses built-in render pipeline for maximum compatibility
/// </summary>
public class SimpleRenderSetup : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void SetupRenderPipeline()
    {
        Debug.Log("[SimpleRenderSetup] Setting up render pipeline...");
        
        // Force use of built-in render pipeline for now
        // This ensures the game always renders properly
        GraphicsSettings.renderPipelineAsset = null;
        QualitySettings.renderPipeline = null;
        
        Debug.Log("[SimpleRenderSetup] Using built-in render pipeline");
    }

    void Start()
    {
        // Ensure camera is set up properly
        var camera = Camera.main;
        if (camera != null)
        {
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.backgroundColor = Color.gray;
            Debug.Log("[SimpleRenderSetup] Camera configured");
        }
        else
        {
            Debug.LogWarning("[SimpleRenderSetup] No main camera found");
        }
    }
}
