using UnityEngine;
using UnityEngine.UI;

public class SaveLoadUI : MonoBehaviour
{
	private SaveLoadManager _sl;
	void Awake()
	{
		_sl = GetComponent<SaveLoadManager>();
		if (_sl == null) _sl = gameObject.AddComponent<SaveLoadManager>();
	}

	void Start()
	{
		var canvas = GameObject.Find("DiceCanvas");
		if (canvas == null)
		{
			canvas = new GameObject("DiceCanvas");
			var c = canvas.AddComponent<Canvas>();
			c.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.AddComponent<CanvasScaler>();
			canvas.AddComponent<GraphicRaycaster>();
		}

		CreateButton(canvas.transform, "Save", new Vector2(10, -220)).onClick.AddListener(_sl.Save);
		CreateButton(canvas.transform, "Load", new Vector2(140, -220)).onClick.AddListener(_sl.Load);
	}

	private Button CreateButton(Transform parent, string label, Vector2 anchoredPos)
	{
		var go = new GameObject(label + " Button");
		go.transform.SetParent(parent);
		var rect = go.AddComponent<RectTransform>();
		rect.anchorMin = new Vector2(0, 1);
		rect.anchorMax = new Vector2(0, 1);
		rect.pivot = new Vector2(0, 1);
		rect.anchoredPosition = anchoredPos;
		rect.sizeDelta = new Vector2(120, 30);
		go.AddComponent<CanvasRenderer>();
		var img = go.AddComponent<Image>();
		img.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
		var btn = go.AddComponent<Button>();

		var labelGO = new GameObject("Label");
		labelGO.transform.SetParent(go.transform);
		var txt = labelGO.AddComponent<Text>();
		txt.text = label;
		txt.alignment = TextAnchor.MiddleCenter;
		txt.color = Color.white;
		txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		var lrect = labelGO.GetComponent<RectTransform>();
		lrect.anchorMin = Vector2.zero;
		lrect.anchorMax = Vector2.one;
		lrect.offsetMin = Vector2.zero;
		lrect.offsetMax = Vector2.zero;
		return btn;
	}
}


