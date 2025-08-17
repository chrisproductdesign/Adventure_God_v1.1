using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
using System.Reflection;

#if UNITY_EDITOR
public static class UrpBootstrap
{
	[InitializeOnLoadMethod]
	private static void EnsureUrp()
	{
		// Run once for heavy setup, but always ensure a valid URP asset is assigned
		const string key = "UrpBootstrap:Configured";
		bool firstTime = !EditorPrefs.GetBool(key, false);
		if (firstTime)
		{
			EditorPrefs.SetBool(key, true);
		}

		// Create or reuse a URP pipeline asset
		var pipelineAsset = GraphicsSettings.renderPipelineAsset;
		if (pipelineAsset == null || !(pipelineAsset is UniversalRenderPipelineAsset))
		{
			var path = "Assets/Settings";
			if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder("Assets", "Settings");
			var asset = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
			AssetDatabase.CreateAsset(asset, path + "/UniversalRenderPipelineAsset.asset");
			AssetDatabase.SaveAssets();
			GraphicsSettings.renderPipelineAsset = asset;
			QualitySettings.renderPipeline = asset;
			EnsureRendererAssigned(asset);
		}

		// Prefer 4x MSAA at project level (URP also controls per-quality)
		if (QualitySettings.antiAliasing < 4) QualitySettings.antiAliasing = 4;

		// URP quality tweaks: shadows and cascades
		var urp = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
		if (urp != null)
		{
			urp.shadowDistance = 50f;
			urp.msaaSampleCount = 4;
		}

		// Create per-quality URP assets and assign with reflection if API exists
		try
		{
			var settingsPath = "Assets/Settings/URP";
			if (!AssetDatabase.IsValidFolder(settingsPath)) AssetDatabase.CreateFolder("Assets/Settings", "URP");

			UniversalRenderPipelineAsset low = LoadOrCreate(settingsPath + "/URP_Low.asset");
			ConfigureUrpAsset(low, 2, new Vector3(0.25f, 0.5f, 0.75f));
			EnsureRendererAssigned(low);
			UniversalRenderPipelineAsset medium = LoadOrCreate(settingsPath + "/URP_Medium.asset");
			ConfigureUrpAsset(medium, 2, new Vector3(0.2f, 0.4f, 0.6f));
			EnsureRendererAssigned(medium);
			UniversalRenderPipelineAsset high = LoadOrCreate(settingsPath + "/URP_High.asset");
			ConfigureUrpAsset(high, 4, new Vector3(0.067f, 0.2f, 0.467f));
			EnsureRendererAssigned(high);

			AssignToQualityLevels(low, medium, high);
			// Default to high in Editor (re-assign each load if needed)
			GraphicsSettings.renderPipelineAsset = high;
			QualitySettings.renderPipeline = high;
			AssetDatabase.SaveAssets();
		}
		catch (Exception ex)
		{
			Debug.LogWarning($"[UrpBootstrap] Per-quality URP setup skipped: {ex.Message}");
		}

		// Cap target frame rate to avoid runaway CPU/GPU in Editor
		Application.targetFrameRate = 60;
		Debug.Log("[UrpBootstrap] URP configured");
	}

	private static UniversalRenderPipelineAsset LoadOrCreate(string path)
	{
		var existing = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path);
		if (existing != null) return existing;
		var asset = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
		AssetDatabase.CreateAsset(asset, path);
		return asset;
	}

	private static void ConfigureUrpAsset(UniversalRenderPipelineAsset asset, int cascades, Vector3 fourSplit)
	{
		if (asset == null) return;
		asset.shadowDistance = 50f;
		asset.msaaSampleCount = 4;
		// Set cascade count only; per-split values vary across URP versions
		try { asset.shadowCascadeCount = Mathf.Clamp(cascades, 1, 4); } catch { }
	}

	private static void AssignToQualityLevels(RenderPipelineAsset low, RenderPipelineAsset medium, RenderPipelineAsset high)
	{
		var names = QualitySettings.names;
		var setAt = typeof(QualitySettings).GetMethod("SetRenderPipelineAssetAt", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(int), typeof(RenderPipelineAsset) }, null);
		if (setAt == null)
		{
			Debug.Log("[UrpBootstrap] API SetRenderPipelineAssetAt not found; assign per-quality RP assets manually if needed.");
			return;
		}
		for (int i = 0; i < names.Length; i++)
		{
			var n = names[i].ToLowerInvariant();
			RenderPipelineAsset chosen = high;
			if (n.Contains("very low") || n.Contains("low")) chosen = low;
			else if (n.Contains("medium")) chosen = medium;
			else if (n.Contains("very high") || n.Contains("ultra") || n.Contains("high")) chosen = high;
			setAt.Invoke(null, new object[] { i, chosen });
		}
	}

	private static void EnsureRendererAssigned(UniversalRenderPipelineAsset asset)
	{
		if (asset == null) return;
		var so = new SerializedObject(asset);
		var list = so.FindProperty("m_RendererDataList");
		var defaultIndex = so.FindProperty("m_DefaultRendererIndex");
		bool needsRenderer = (list == null) || (list.arraySize == 0) || list.GetArrayElementAtIndex(0).objectReferenceValue == null;
		if (!needsRenderer) return;
		var renderData = ScriptableObject.CreateInstance<UniversalRendererData>();
		var dir = "Assets/Settings/URP";
		if (!AssetDatabase.IsValidFolder("Assets/Settings")) AssetDatabase.CreateFolder("Assets", "Settings");
		if (!AssetDatabase.IsValidFolder(dir)) AssetDatabase.CreateFolder("Assets/Settings", "URP");
		var rdPath = dir + "/ForwardRenderer.asset";
		AssetDatabase.CreateAsset(renderData, rdPath);
		AssetDatabase.SaveAssets();
		if (list != null)
		{
			list.arraySize = 1;
			list.GetArrayElementAtIndex(0).objectReferenceValue = renderData;
		}
		if (defaultIndex != null) defaultIndex.intValue = 0;
		so.ApplyModifiedProperties();
	}
}
#endif


