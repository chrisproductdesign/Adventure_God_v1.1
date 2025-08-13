using UnityEngine;
using UnityEngine.UI;

public class DiceGateUI : MonoBehaviour
{
	private Slider _slider;
	private Button _rollButton;
	private Text _logText;

	void Start()
	{
		// Build a minimal UI at runtime so no scene asset is required
		var canvasGO = new GameObject("DiceCanvas");
		var canvas = canvasGO.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvasGO.AddComponent<CanvasScaler>();
		canvasGO.AddComponent<GraphicRaycaster>();

		_slider = CreateSlider(new Vector2(160, -40));
		_rollButton = CreateButton("Roll", new Vector2(160, -80));
		_logText = CreateText(new Vector2(10, -140), 14, 600, 200);

		_rollButton.onClick.AddListener(() => GetComponent<DiceGate>()?.RerollLast());
		_slider.onValueChanged.AddListener(v => GetComponent<DiceGate>()?.SetDC(v));
		_slider.minValue = 5;
		_slider.maxValue = 20;
		_slider.wholeNumbers = true;
		_slider.value = 10;
	}

	public void AppendLog(string line)
	{
		if (_logText == null) return;
		_logText.text += (string.IsNullOrEmpty(_logText.text) ? "" : "\n") + line;
	}

	private Slider CreateSlider(Vector2 anchoredPos)
	{
		var go = new GameObject("DC Slider");
		go.transform.SetParent(GameObject.Find("DiceCanvas").transform);
		var rect = go.AddComponent<RectTransform>();
		rect.anchorMin = new Vector2(0, 1);
		rect.anchorMax = new Vector2(0, 1);
		rect.pivot = new Vector2(0, 1);
		rect.anchoredPosition = anchoredPos;
		rect.sizeDelta = new Vector2(300, 20);
		go.AddComponent<CanvasRenderer>();
		var bg = go.AddComponent<Image>();
		bg.color = new Color(0, 0, 0, 0.2f);
		var slider = go.AddComponent<Slider>();
		return slider;
	}

	private Button CreateButton(string label, Vector2 anchoredPos)
	{
		var go = new GameObject("Roll Button");
		go.transform.SetParent(GameObject.Find("DiceCanvas").transform);
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
		var lrect = labelGO.GetComponent<RectTransform>();
		lrect.anchorMin = Vector2.zero;
		lrect.anchorMax = Vector2.one;
		lrect.offsetMin = Vector2.zero;
		lrect.offsetMax = Vector2.zero;
		return btn;
	}

	private Text CreateText(Vector2 anchoredPos, int fontSize, float width, float height)
	{
		var go = new GameObject("Log Text");
		go.transform.SetParent(GameObject.Find("DiceCanvas").transform);
		var rect = go.AddComponent<RectTransform>();
		rect.anchorMin = new Vector2(0, 1);
		rect.anchorMax = new Vector2(0, 1);
		rect.pivot = new Vector2(0, 1);
		rect.anchoredPosition = anchoredPos;
		rect.sizeDelta = new Vector2(width, height);
		go.AddComponent<CanvasRenderer>();
		var txt = go.AddComponent<Text>();
		txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		txt.fontSize = fontSize;
		txt.color = Color.white;
		txt.alignment = TextAnchor.UpperLeft;
		return txt;
	}
}


