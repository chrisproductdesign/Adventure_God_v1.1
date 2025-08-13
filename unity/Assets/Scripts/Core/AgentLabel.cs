using UnityEngine;
using UnityEngine.UI;

public class AgentLabel : MonoBehaviour
{
    public Transform target;
    private Text _text;
    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
        var canvas = GameObject.Find("DiceCanvas");
        if (canvas == null)
        {
            canvas = new GameObject("DiceCanvas");
            var c = canvas.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
        }
        var go = new GameObject("AgentLabel");
        go.transform.SetParent(canvas.transform);
        _text = go.AddComponent<Text>();
        _text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        _text.fontSize = 14;
        _text.color = Color.white;
    }

    void Update()
    {
        if (target == null || _cam == null || _text == null) return;
        var screen = _cam.WorldToScreenPoint(target.position + Vector3.up * 1.2f);
        _text.rectTransform.anchoredPosition = screen;
        var tag = target.GetComponent<AgentTag>();
        _text.text = tag != null ? tag.actorId : target.name;
    }
}


