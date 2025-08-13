using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class PartyHUD : MonoBehaviour
{
	private PartyState _state;
	private Text _text;

	void Awake()
	{
		_state = GetComponent<PartyState>();
		if (_state == null) _state = gameObject.AddComponent<PartyState>();
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

		var go = new GameObject("PartyHUD");
		go.transform.SetParent(canvas.transform);
		var rect = go.AddComponent<RectTransform>();
		rect.anchorMin = new Vector2(1, 1);
		rect.anchorMax = new Vector2(1, 1);
		rect.pivot = new Vector2(1, 1);
		rect.anchoredPosition = new Vector2(-10, -10);
		rect.sizeDelta = new Vector2(260, 120);
		go.AddComponent<CanvasRenderer>();
		var bg = go.AddComponent<Image>();
		bg.color = new Color(0, 0, 0, 0.3f);
		_text = go.AddComponent<Text>();
		_text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		_text.fontSize = 13;
		_text.alignment = TextAnchor.UpperRight;
	}

	void Update()
	{
		if (_text == null || _state == null) return;
		var sb = new StringBuilder();
		foreach (var a in _state.Enumerate())
		{
			sb.AppendLine($"{a.id}: HP {a.hp} @ ({a.position.x:F1},{a.position.z:F1})");
		}
		_text.text = sb.ToString();
	}
}


