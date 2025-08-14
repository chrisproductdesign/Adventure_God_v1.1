using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public class DMHud : MonoBehaviour
{
    private RectTransform _panel;
    private Text _title;
    private Text _list;
    private Button _toggle;
    private bool _collapsed = false;

    void Start()
    {
        EnsureCanvasAndEventSystem();
        BuildHud();
        RefreshList();
        DMNarration.NoteChanged += OnNoteChanged;
    }

    void OnDestroy()
    {
        DMNarration.NoteChanged -= OnNoteChanged;
    }

    private void EnsureCanvasAndEventSystem()
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
        if (EventSystem.current == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }

    private void BuildHud()
    {
        var root = new GameObject("DMHud");
        root.transform.SetParent(GameObject.Find("DiceCanvas").transform);
        _panel = root.AddComponent<RectTransform>();
        _panel.anchorMin = new Vector2(1, 1);
        _panel.anchorMax = new Vector2(1, 1);
        _panel.pivot = new Vector2(1, 1);
        _panel.anchoredPosition = new Vector2(-20, -20);
        _panel.sizeDelta = new Vector2(320, 220);
        var bg = root.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.35f);

        _toggle = CreateButton(root.transform, "⮟", new Vector2(-10, -10), new Vector2(24, 24));
        _toggle.onClick.AddListener(ToggleCollapse);

        _title = CreateText(root.transform, "DM Notes", 16, TextAnchor.UpperRight,
            new Vector2(-44, -10), new Vector2(260, 24));

        _list = CreateText(root.transform, "", 13, TextAnchor.UpperRight,
            new Vector2(-10, -40), new Vector2(300, 170));
    }

    private Button CreateButton(Transform parent, string label, Vector2 topRightOffset, Vector2 size)
    {
        var go = new GameObject(label + " Btn");
        go.transform.SetParent(parent);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = topRightOffset;
        rect.sizeDelta = size;
        go.AddComponent<CanvasRenderer>();
        var img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f, 0.7f);
        var btn = go.AddComponent<Button>();
        var labelGO = new GameObject("Label");
        labelGO.transform.SetParent(go.transform);
        var txt = labelGO.AddComponent<Text>();
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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

    private Text CreateText(Transform parent, string content, int size, TextAnchor anchor, Vector2 topRightOffset, Vector2 boxSize)
    {
        var go = new GameObject("Text");
        go.transform.SetParent(parent);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = topRightOffset;
        rect.sizeDelta = boxSize;
        go.AddComponent<CanvasRenderer>();
        var txt = go.AddComponent<Text>();
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = size;
        txt.color = Color.white;
        txt.alignment = anchor;
        txt.text = content;
        return txt;
    }

    private void ToggleCollapse()
    {
        _collapsed = !_collapsed;
        if (_list != null) _list.gameObject.SetActive(!_collapsed);
        if (_title != null) _title.text = _collapsed ? "DM Notes (collapsed)" : "DM Notes";
        var label = _toggle.GetComponentInChildren<Text>();
        if (label != null) label.text = _collapsed ? "⮝" : "⮟";
    }

    private void OnNoteChanged(string actorId, string note)
    {
        RefreshList();
    }

    private void RefreshList()
    {
        if (_list == null) return;
        var all = DMNarration.GetAllNotes();
        var sb = new StringBuilder();
        foreach (var kv in all)
        {
            if (string.IsNullOrEmpty(kv.Value)) continue;
            sb.AppendLine(kv.Key + ": " + kv.Value);
        }
        _list.text = sb.ToString();
    }
}


