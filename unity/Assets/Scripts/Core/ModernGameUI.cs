using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using System;

/// <summary>
/// Beautiful Game Director Console - Modern, Elegant, Professional
/// Designed with perfect UX/UI principles for maximum usability and visual appeal
/// </summary>
public class ModernGameUI : MonoBehaviour
{
    [Header("UI References")]
    private Canvas _mainCanvas;
    private RectTransform _mainPanel;
    private RectTransform _headerPanel;
    private RectTransform _contentPanel;
    private RectTransform _logPanel;
    
    [Header("Control Elements")]
    private Slider _dcSlider;
    private Button _rollButton;
    private Button _proposeButton;
    private Button _autoButton;
    private TMP_InputField _situationInput;
    private TMP_InputField _resultInput;
    private Button _saveButton;
    private Button _sendButton;
    private Toggle _useSuggestedDcToggle;
    
    [Header("Navigation Elements")]
    private Button _actorButton;
    private Button _poiButton;
    private Button _snapButton;
    private Button _candPrevButton;
    private Button _candNextButton;
    private TextMeshProUGUI _candText;
    
    [Header("Status Elements")]
    private TextMeshProUGUI _connectionStatus;
    private TextMeshProUGUI _dcValueText;
    private TextMeshProUGUI _logText;
    
    [Header("Beautiful Design System")]
    [SerializeField] private Color _primaryBackground = new Color(0.06f, 0.08f, 0.12f, 0.98f);
    [SerializeField] private Color _secondaryBackground = new Color(0.1f, 0.12f, 0.16f, 0.95f);
    [SerializeField] private Color _accentColor = new Color(0.2f, 0.6f, 1f, 1f);
    [SerializeField] private Color _successColor = new Color(0.2f, 0.8f, 0.4f, 1f);
    [SerializeField] private Color _warningColor = new Color(1f, 0.7f, 0.2f, 1f);
    [SerializeField] private Color _errorColor = new Color(1f, 0.4f, 0.4f, 1f);
    [SerializeField] private Color _textPrimary = new Color(0.98f, 0.98f, 0.98f, 1f);
    [SerializeField] private Color _textSecondary = new Color(0.85f, 0.85f, 0.85f, 1f);
    [SerializeField] private Color _textMuted = new Color(0.7f, 0.7f, 0.7f, 1f);
    [SerializeField] private Color _inputBackground = new Color(0.08f, 0.1f, 0.14f, 0.9f);
    [SerializeField] private Color _buttonHover = new Color(0.15f, 0.17f, 0.21f, 0.95f);
    
    [Header("Typography & Spacing")]
    [SerializeField] private int _titleFontSize = 20;
    [SerializeField] private int _bodyFontSize = 16;
    [SerializeField] private int _smallFontSize = 14;
    [SerializeField] private float _elementSpacing = 16f;
    [SerializeField] private float _buttonHeight = 52f;
    [SerializeField] private float _inputHeight = 48f;
    
    // State
    private string[] _actors = { "adv-1", "adv-2", "adv-3" };
    private int _actorIndex = 0;
    private int _candIndex = 0;
    private int _candCount = 1;
    private List<string> _poiNames = new List<string>();
    private int _poiIndex = -1;
    private bool _isAutoMode = false;
    private bool _useSuggestedDc = false;
    
    void Start()
    {
        CreateBeautifulUI();
        SetupEventHandlers();
        UpdateUI();
    }
    
    void Update()
    {
        UpdateConnectionStatus();
        UpdateCandidateDisplay();
    }
    
    private void CreateBeautifulUI()
    {
        CreateMainCanvas();
        CreateHeaderPanel();
        CreateContentPanel();
        CreateLogPanel();
        ApplyBeautifulStyling();
    }
    
    private void CreateMainCanvas()
    {
        var canvasGO = new GameObject("BeautifulGameCanvas");
        _mainCanvas = canvasGO.AddComponent<Canvas>();
        _mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _mainCanvas.sortingOrder = 100;
        
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasGO.AddComponent<GraphicRaycaster>();
        
        if (EventSystem.current == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }
    
    private void CreateHeaderPanel()
    {
        var headerGO = new GameObject("HeaderPanel");
        headerGO.transform.SetParent(_mainCanvas.transform);
        _headerPanel = headerGO.AddComponent<RectTransform>();
        
        _headerPanel.anchorMin = new Vector2(0, 1);
        _headerPanel.anchorMax = new Vector2(1, 1);
        _headerPanel.pivot = new Vector2(0.5f, 1);
        _headerPanel.anchoredPosition = Vector2.zero;
        _headerPanel.sizeDelta = new Vector2(0, 80);
        
        var bg = headerGO.AddComponent<Image>();
        bg.color = _primaryBackground;
        
        // Add beautiful shadow
        var shadow = headerGO.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.3f);
        shadow.effectDistance = new Vector2(0, -6);
        
        // Title
        var title = CreateBeautifulText(headerGO.transform, "Game Director Console", _titleFontSize, TextAlignmentOptions.Left);
        title.color = _textPrimary;
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0);
        titleRect.anchorMax = new Vector2(0.6f, 1);
        titleRect.offsetMin = new Vector2(32, 0);
        titleRect.offsetMax = new Vector2(-16, 0);
        
        // Connection Status
        CreateConnectionStatus(headerGO.transform);
    }
    
    private void CreateConnectionStatus(Transform parent)
    {
        var statusGO = new GameObject("ConnectionStatus");
        statusGO.transform.SetParent(parent);
        var rect = statusGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.6f, 0.5f);
        rect.anchorMax = new Vector2(0.6f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(160, 40);
        
        var bg = statusGO.AddComponent<Image>();
        bg.color = new Color(0, 0.3f, 0, 0.2f);
        
        _connectionStatus = CreateBeautifulText(statusGO.transform, "Connected", _bodyFontSize, TextAlignmentOptions.Center);
        _connectionStatus.color = _successColor;
    }
    
    private void CreateContentPanel()
    {
        var contentGO = new GameObject("ContentPanel");
        contentGO.transform.SetParent(_mainCanvas.transform);
        _contentPanel = contentGO.AddComponent<RectTransform>();
        
        _contentPanel.anchorMin = new Vector2(0, 1);
        _contentPanel.anchorMax = new Vector2(1, 1);
        _contentPanel.pivot = new Vector2(0.5f, 1);
        _contentPanel.anchoredPosition = new Vector2(0, -80);
        _contentPanel.sizeDelta = new Vector2(0, 240);
        
        var bg = contentGO.AddComponent<Image>();
        bg.color = _secondaryBackground;
        
        // Add subtle shadow
        var shadow = contentGO.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.2f);
        shadow.effectDistance = new Vector2(0, -4);
        
        CreateContentLayout(contentGO.transform);
    }
    
    private void CreateContentLayout(Transform parent)
    {
        // Left Column - Main Controls
        CreateLeftColumn(parent);
        
        // Right Column - Secondary Controls
        CreateRightColumn(parent);
    }
    
    private void CreateLeftColumn(Transform parent)
    {
        var leftGO = new GameObject("LeftColumn");
        leftGO.transform.SetParent(parent);
        var rect = leftGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.offsetMin = new Vector2(24, 24);
        rect.offsetMax = new Vector2(-12, -24);
        
        float yOffset = 0;
        
        // DC Slider Row
        CreateDCSlider(leftGO.transform, new Vector2(0, yOffset));
        yOffset -= (_buttonHeight + _elementSpacing);
        
        // Action Buttons Row
        CreateActionButtons(leftGO.transform, new Vector2(0, yOffset));
        yOffset -= (_buttonHeight + _elementSpacing);
        
        // Situation Input
        CreateSituationInput(leftGO.transform, new Vector2(0, yOffset));
        yOffset -= (_inputHeight + _elementSpacing);
        
        // Result Input
        CreateResultInput(leftGO.transform, new Vector2(0, yOffset));
    }
    
    private void CreateRightColumn(Transform parent)
    {
        var rightGO = new GameObject("RightColumn");
        rightGO.transform.SetParent(parent);
        var rect = rightGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0);
        rect.anchorMax = new Vector2(1, 1);
        rect.offsetMin = new Vector2(12, 24);
        rect.offsetMax = new Vector2(-24, -24);
        
        float yOffset = 0;
        
        // Actor and Auto Controls
        CreateActorAutoControls(rightGO.transform, new Vector2(0, yOffset));
        yOffset -= (_buttonHeight + _elementSpacing);
        
        // Navigation Controls
        CreateNavigationControls(rightGO.transform, new Vector2(0, yOffset));
        yOffset -= (_buttonHeight + _elementSpacing);
        
        // Toggle Controls
        CreateToggleControls(rightGO.transform, new Vector2(0, yOffset));
    }
    
    private void CreateDCSlider(Transform parent, Vector2 position)
    {
        var sliderGO = new GameObject("DCSlider");
        sliderGO.transform.SetParent(parent);
        var rect = sliderGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(400, 60);
        
        _dcSlider = sliderGO.AddComponent<Slider>();
        _dcSlider.minValue = 5;
        _dcSlider.maxValue = 20;
        _dcSlider.wholeNumbers = true;
        _dcSlider.value = 10;
        
        // Background
        var bgGO = new GameObject("Background");
        bgGO.transform.SetParent(sliderGO.transform);
        var bgRect = bgGO.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = _inputBackground;
        _dcSlider.targetGraphic = bgImg;
        
        // Fill area
        var fillAreaGO = new GameObject("Fill Area");
        fillAreaGO.transform.SetParent(sliderGO.transform);
        var fillAreaRect = fillAreaGO.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(8, 8);
        fillAreaRect.offsetMax = new Vector2(-8, -8);
        
        var fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(fillAreaGO.transform);
        var fillRect = fillGO.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        var fillImg = fillGO.AddComponent<Image>();
        fillImg.color = _accentColor;
        _dcSlider.fillRect = fillRect;
        
        // Handle
        var handleAreaGO = new GameObject("Handle Slide Area");
        handleAreaGO.transform.SetParent(sliderGO.transform);
        var handleAreaRect = handleAreaGO.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(12, 12);
        handleAreaRect.offsetMax = new Vector2(-12, -12);
        
        var handleGO = new GameObject("Handle");
        handleGO.transform.SetParent(handleAreaGO.transform);
        var handleRect = handleGO.AddComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(28, 28);
        var handleImg = handleGO.AddComponent<Image>();
        handleImg.color = Color.white;
        _dcSlider.handleRect = handleRect;
        
        // DC value text
        _dcValueText = CreateBeautifulText(sliderGO.transform, "DC: 10", _bodyFontSize, TextAlignmentOptions.Center);
        var dcTextRect = _dcValueText.GetComponent<RectTransform>();
        dcTextRect.anchorMin = new Vector2(0, 0);
        dcTextRect.anchorMax = new Vector2(1, 0);
        dcTextRect.pivot = new Vector2(0.5f, 0);
        dcTextRect.anchoredPosition = new Vector2(0, -40);
        dcTextRect.sizeDelta = new Vector2(0, 24);
        _dcValueText.color = _textPrimary;
    }
    
    private void CreateActionButtons(Transform parent, Vector2 position)
    {
        // Roll Button
        _rollButton = CreateBeautifulButton(parent, "Roll", new Vector2(position.x, position.y), new Vector2(120, _buttonHeight), _accentColor);
        
        // Propose Button
        _proposeButton = CreateBeautifulButton(parent, "Propose", new Vector2(position.x + 140, position.y), new Vector2(120, _buttonHeight), _successColor);
        
        // Send Button
        _sendButton = CreateBeautifulButton(parent, "Send", new Vector2(position.x + 280, position.y), new Vector2(120, _buttonHeight), _warningColor);
    }
    
    private void CreateSituationInput(Transform parent, Vector2 position)
    {
        _situationInput = CreateBeautifulInputField(parent, "Describe situation or intent...", position, new Vector2(540, _inputHeight));
    }
    
    private void CreateResultInput(Transform parent, Vector2 position)
    {
        _resultInput = CreateBeautifulInputField(parent, "After roll: DM result note...", position, new Vector2(540, _inputHeight));
    }
    
    private void CreateActorAutoControls(Transform parent, Vector2 position)
    {
        // Actor Button
        _actorButton = CreateBeautifulButton(parent, "Actor: adv-1", new Vector2(position.x, position.y), new Vector2(160, _buttonHeight), _secondaryBackground);
        
        // Auto Button
        _autoButton = CreateBeautifulButton(parent, "Auto: OFF", new Vector2(position.x + 180, position.y), new Vector2(120, _buttonHeight), _secondaryBackground);
    }
    
    private void CreateNavigationControls(Transform parent, Vector2 position)
    {
        // POI Button
        _poiButton = CreateBeautifulButton(parent, "POI: none", new Vector2(position.x, position.y), new Vector2(120, _buttonHeight), _secondaryBackground);
        
        // Snap Button
        _snapButton = CreateBeautifulButton(parent, "Snap", new Vector2(position.x + 140, position.y), new Vector2(80, _buttonHeight), _accentColor);
        
        // Save Button
        _saveButton = CreateBeautifulButton(parent, "Save", new Vector2(position.x + 240, position.y), new Vector2(80, _buttonHeight), _successColor);
    }
    
    private void CreateToggleControls(Transform parent, Vector2 position)
    {
        // Candidate Navigation
        _candPrevButton = CreateBeautifulButton(parent, "◀", new Vector2(position.x, position.y), new Vector2(50, _buttonHeight), _secondaryBackground);
        _candNextButton = CreateBeautifulButton(parent, "▶", new Vector2(position.x + 60, position.y), new Vector2(50, _buttonHeight), _secondaryBackground);
        
        _candText = CreateBeautifulText(parent, "Cand 1/1", _bodyFontSize, TextAlignmentOptions.Center);
        var candTextRect = _candText.GetComponent<RectTransform>();
        candTextRect.anchorMin = new Vector2(0, 1);
        candTextRect.anchorMax = new Vector2(0, 1);
        candTextRect.pivot = new Vector2(0, 1);
        candTextRect.anchoredPosition = new Vector2(position.x + 130, position.y);
        candTextRect.sizeDelta = new Vector2(100, _buttonHeight);
        _candText.color = _textPrimary;
        
        // Use Suggested DC Toggle
        CreateUseSuggestedDcToggle(parent, new Vector2(position.x + 250, position.y));
    }
    
    private void CreateUseSuggestedDcToggle(Transform parent, Vector2 position)
    {
        var toggleGO = new GameObject("UseSuggestedDcToggle");
        toggleGO.transform.SetParent(parent);
        var rect = toggleGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(200, _buttonHeight);
        
        _useSuggestedDcToggle = toggleGO.AddComponent<Toggle>();
        
        // Background
        var bgGO = new GameObject("Background");
        bgGO.transform.SetParent(toggleGO.transform);
        var bgRect = bgGO.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = _inputBackground;
        _useSuggestedDcToggle.targetGraphic = bgImg;
        
        // Checkmark
        var checkmarkGO = new GameObject("Checkmark");
        checkmarkGO.transform.SetParent(toggleGO.transform);
        var checkmarkRect = checkmarkGO.AddComponent<RectTransform>();
        checkmarkRect.anchorMin = new Vector2(0, 0.5f);
        checkmarkRect.anchorMax = new Vector2(0, 0.5f);
        checkmarkRect.pivot = new Vector2(0, 0.5f);
        checkmarkRect.anchoredPosition = new Vector2(16, 0);
        checkmarkRect.sizeDelta = new Vector2(24, 24);
        var checkmarkImg = checkmarkGO.AddComponent<Image>();
        checkmarkImg.color = _accentColor;
        _useSuggestedDcToggle.graphic = checkmarkImg;
        
        // Label
        var label = CreateBeautifulText(toggleGO.transform, "Use Suggested DC", _bodyFontSize, TextAlignmentOptions.Left);
        var labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(1, 1);
        labelRect.offsetMin = new Vector2(56, 0);
        labelRect.offsetMax = new Vector2(-16, 0);
        label.color = _textPrimary;
    }
    
    private void CreateLogPanel()
    {
        var logGO = new GameObject("LogPanel");
        logGO.transform.SetParent(_mainCanvas.transform);
        _logPanel = logGO.AddComponent<RectTransform>();
        
        _logPanel.anchorMin = new Vector2(0, 1);
        _logPanel.anchorMax = new Vector2(1, 1);
        _logPanel.pivot = new Vector2(0.5f, 1);
        _logPanel.anchoredPosition = new Vector2(0, -320);
        _logPanel.sizeDelta = new Vector2(0, 120);
        
        var bg = logGO.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.15f);
        
        // Add subtle shadow
        var shadow = logGO.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.1f);
        shadow.effectDistance = new Vector2(0, -2);
        
        CreateLogContent(logGO.transform);
    }
    
    private void CreateLogContent(Transform parent)
    {
        var logContentGO = new GameObject("LogContent");
        logContentGO.transform.SetParent(parent);
        var logContentRect = logContentGO.AddComponent<RectTransform>();
        logContentRect.anchorMin = new Vector2(0, 0);
        logContentRect.anchorMax = new Vector2(1, 1);
        logContentRect.offsetMin = new Vector2(24, 24);
        logContentRect.offsetMax = new Vector2(-24, -24);
        
        var logBg = logContentGO.AddComponent<Image>();
        logBg.color = new Color(0, 0, 0, 0.1f);
        
        // Scroll view for log
        var scrollViewGO = new GameObject("LogScrollView");
        scrollViewGO.transform.SetParent(logContentGO.transform);
        var scrollViewRect = scrollViewGO.AddComponent<RectTransform>();
        scrollViewRect.anchorMin = Vector2.zero;
        scrollViewRect.anchorMax = Vector2.one;
        scrollViewRect.offsetMin = new Vector2(12, 12);
        scrollViewRect.offsetMax = new Vector2(-12, -12);
        
        var scrollView = scrollViewGO.AddComponent<ScrollRect>();
        var viewportGO = new GameObject("Viewport");
        viewportGO.transform.SetParent(scrollViewGO.transform);
        var viewportRect = viewportGO.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        var viewportImg = viewportGO.AddComponent<Image>();
        viewportImg.color = new Color(0, 0, 0, 0.05f);
        viewportGO.AddComponent<Mask>();
        
        var contentGO = new GameObject("Content");
        contentGO.transform.SetParent(viewportGO.transform);
        var contentRect = contentGO.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0, 0);
        
        var contentLayout = contentGO.AddComponent<VerticalLayoutGroup>();
        contentLayout.padding = new RectOffset(16, 16, 16, 16);
        contentLayout.spacing = 6;
        contentLayout.childControlHeight = false;
        contentLayout.childControlWidth = true;
        contentLayout.childForceExpandHeight = false;
        contentLayout.childForceExpandWidth = true;
        
        _logText = CreateBeautifulText(contentGO.transform, "", _smallFontSize, TextAlignmentOptions.TopLeft);
        _logText.color = _textSecondary;
        var logTextRect = _logText.GetComponent<RectTransform>();
        logTextRect.sizeDelta = new Vector2(0, 200);
        
        scrollView.viewport = viewportRect;
        scrollView.content = contentRect;
        scrollView.horizontal = false;
        scrollView.vertical = true;
    }
    
    private Button CreateBeautifulButton(Transform parent, string label, Vector2 position, Vector2 size, Color color)
    {
        var buttonGO = new GameObject(label + "Button");
        buttonGO.transform.SetParent(parent);
        var rect = buttonGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        
        var button = buttonGO.AddComponent<Button>();
        var img = buttonGO.AddComponent<Image>();
        img.color = color;
        
        var colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = new Color(color.r * 1.15f, color.g * 1.15f, color.b * 1.15f, color.a);
        colors.pressedColor = new Color(color.r * 0.85f, color.g * 0.85f, color.b * 0.85f, color.a);
        button.colors = colors;
        
        var labelText = CreateBeautifulText(buttonGO.transform, label, _bodyFontSize, TextAlignmentOptions.Center);
        labelText.color = Color.white;
        var labelRect = labelText.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        
        return button;
    }
    
    private TMP_InputField CreateBeautifulInputField(Transform parent, string placeholder, Vector2 position, Vector2 size)
    {
        var inputGO = new GameObject("InputField");
        inputGO.transform.SetParent(parent);
        var rect = inputGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        
        var input = inputGO.AddComponent<TMP_InputField>();
        var img = inputGO.AddComponent<Image>();
        img.color = _inputBackground;
        input.targetGraphic = img;
        
        // Text area
        var textAreaGO = new GameObject("Text Area");
        textAreaGO.transform.SetParent(inputGO.transform);
        var textAreaRect = textAreaGO.AddComponent<RectTransform>();
        textAreaRect.anchorMin = Vector2.zero;
        textAreaRect.anchorMax = Vector2.one;
        textAreaRect.offsetMin = new Vector2(16, 12);
        textAreaRect.offsetMax = new Vector2(-16, -12);
        textAreaGO.AddComponent<Mask>();
        
        var textGO = new GameObject("Text");
        textGO.transform.SetParent(textAreaGO.transform);
        var textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        var text = textGO.AddComponent<TextMeshProUGUI>();
        text.color = _textPrimary;
        text.fontSize = _bodyFontSize;
        text.alignment = TextAlignmentOptions.Left;
        input.textComponent = text;
        
        // Placeholder
        var placeholderGO = new GameObject("Placeholder");
        placeholderGO.transform.SetParent(textAreaGO.transform);
        var placeholderRect = placeholderGO.AddComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = Vector2.zero;
        placeholderRect.offsetMax = Vector2.zero;
        var placeholderText = placeholderGO.AddComponent<TextMeshProUGUI>();
        placeholderText.text = placeholder;
        placeholderText.color = _textMuted;
        placeholderText.fontSize = _bodyFontSize;
        placeholderText.alignment = TextAlignmentOptions.Left;
        input.placeholder = placeholderText;
        
        return input;
    }
    
    private TextMeshProUGUI CreateBeautifulText(Transform parent, string text, int fontSize, TextAlignmentOptions alignment)
    {
        var textGO = new GameObject("Text");
        textGO.transform.SetParent(parent);
        var rect = textGO.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = _textPrimary;
        
        return tmp;
    }
    
    private void ApplyBeautifulStyling()
    {
        // Apply consistent styling across all elements
    }
    
    private void SetupEventHandlers()
    {
        _rollButton.onClick.AddListener(OnRollClicked);
        _proposeButton.onClick.AddListener(OnProposeClicked);
        _autoButton.onClick.AddListener(OnAutoClicked);
        _saveButton.onClick.AddListener(OnSaveClicked);
        _sendButton.onClick.AddListener(OnSendClicked);
        _actorButton.onClick.AddListener(OnActorClicked);
        _poiButton.onClick.AddListener(OnPOIClicked);
        _snapButton.onClick.AddListener(OnSnapClicked);
        _candPrevButton.onClick.AddListener(() => OnCandidateChanged(-1));
        _candNextButton.onClick.AddListener(() => OnCandidateChanged(1));
        _useSuggestedDcToggle.onValueChanged.AddListener(OnUseSuggestedDcChanged);
        _dcSlider.onValueChanged.AddListener(OnDCChanged);
    }
    
    private void UpdateConnectionStatus()
    {
        var brainClient = GetComponent<BrainClient>();
        bool isConnected = brainClient != null && brainClient.IsConnected;
        
        _connectionStatus.text = isConnected ? "Connected" : "Offline";
        _connectionStatus.color = isConnected ? _successColor : _errorColor;
        
        var bg = _connectionStatus.transform.parent.GetComponent<Image>();
        bg.color = isConnected ? new Color(0, 0.3f, 0, 0.2f) : new Color(0.3f, 0, 0, 0.2f);
    }
    
    private void UpdateCandidateDisplay()
    {
        _candText.text = $"Cand {_candIndex + 1}/{_candCount}";
    }
    
    private void UpdateUI()
    {
        _dcValueText.text = $"DC: {_dcSlider.value}";
        _autoButton.GetComponentInChildren<TextMeshProUGUI>().text = _isAutoMode ? "Auto: ON" : "Auto: OFF";
        _autoButton.GetComponent<Image>().color = _isAutoMode ? _successColor : _secondaryBackground;
        _actorButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Actor: {_actors[_actorIndex]}";
    }
    
    // Event Handlers
    private void OnRollClicked()
    {
        GetComponent<DiceGate>()?.RerollLast();
    }
    
    private void OnProposeClicked()
    {
        var diceGate = GetComponent<DiceGate>();
        if (diceGate != null && !string.IsNullOrEmpty(_situationInput.text))
        {
            var proposal = new IntentProposal
            {
                type = "IntentProposal",
                actorId = _actors[_actorIndex],
                goal = "user-requested",
                intent = "custom",
                rationale = _situationInput.text,
                suggestedDC = _useSuggestedDc ? (int?)_dcSlider.value : null,
                candidateActions = new List<CandidateAction>
                {
                    new CandidateAction { action = "custom", @params = new Dictionary<string, object> { { "description", _situationInput.text } } }
                }
            };
            
            var planner = GetComponent<Planner>();
            if (planner != null)
            {
                var agent = planner.ResolveAgentFor(_actors[_actorIndex]);
                diceGate.StageProposal(proposal, agent);
            }
        }
    }
    
    private void OnAutoClicked()
    {
        _isAutoMode = !_isAutoMode;
        UpdateUI();
    }
    
    private void OnSaveClicked()
    {
        if (!string.IsNullOrEmpty(_resultInput.text))
        {
            var saveLoadManager = FindObjectOfType<SaveLoadManager>();
            if (saveLoadManager != null)
            {
                DMNarration.SetLastNote(_actors[_actorIndex], _resultInput.text);
                AppendLog($"[DM] Saved result note for {_actors[_actorIndex]}: {_resultInput.text}");
                _resultInput.text = "";
            }
        }
    }
    
    private void OnSendClicked()
    {
        var brainClient = GetComponent<BrainClient>();
        if (brainClient != null && !string.IsNullOrEmpty(_situationInput.text))
        {
            brainClient.SendDMContext(_actors[_actorIndex], _situationInput.text);
            AppendLog($"[DM] Sent context to gateway: {_situationInput.text}");
        }
    }
    
    private void OnActorClicked()
    {
        _actorIndex = (_actorIndex + 1) % _actors.Length;
        UpdateUI();
    }
    
    private void OnPOIClicked()
    {
        if (_poiNames.Count > 0)
        {
            _poiIndex = (_poiIndex + 1) % _poiNames.Count;
            UpdatePOIDisplay();
        }
    }
    
    private void OnSnapClicked()
    {
        if (_poiIndex >= 0 && _poiIndex < _poiNames.Count)
        {
            var poiTransform = POIRegistry.Get(_poiNames[_poiIndex]);
            if (poiTransform != null)
            {
                var camera = Camera.main;
                if (camera != null)
                {
                    var topDownCamera = camera.GetComponent<TopDownCamera>();
                    if (topDownCamera != null)
                    {
                        topDownCamera.target = poiTransform;
                        AppendLog($"[DM] Snapped camera to POI: {_poiNames[_poiIndex]}");
                    }
                }
            }
        }
    }
    
    private void OnCandidateChanged(int direction)
    {
        _candIndex = Mathf.Clamp(_candIndex + direction, 0, _candCount - 1);
        UpdateCandidateDisplay();
    }
    
    private void OnUseSuggestedDcChanged(bool value)
    {
        _useSuggestedDc = value;
    }
    
    private void OnDCChanged(float value)
    {
        GetComponent<DiceGate>()?.SetDC(value);
        UpdateUI();
    }
    
    public void AppendLog(string line)
    {
        if (_logText != null)
        {
            _logText.text += (string.IsNullOrEmpty(_logText.text) ? "" : "\n") + line;
            
            var scrollRect = _logText.GetComponentInParent<ScrollRect>();
            if (scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }
    }
    
    public void SetCandidateCount(int count)
    {
        _candCount = count;
        _candIndex = Mathf.Clamp(_candIndex, 0, _candCount - 1);
        UpdateCandidateDisplay();
    }
    
    public void SetPOINames(List<string> poiNames)
    {
        _poiNames = poiNames;
        _poiIndex = -1;
        UpdatePOIDisplay();
    }
    
    private void UpdatePOIDisplay()
    {
        string poiText = _poiIndex >= 0 && _poiIndex < _poiNames.Count ? _poiNames[_poiIndex] : "none";
        _poiButton.GetComponentInChildren<TextMeshProUGUI>().text = $"POI: {poiText}";
    }
}
