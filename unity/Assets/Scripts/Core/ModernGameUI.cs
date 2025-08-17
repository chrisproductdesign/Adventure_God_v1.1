using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using System;

/// <summary>
/// Modern Game UI System - Apple-inspired design with elegant interactions
/// Replaces the basic DiceGateUI with a sophisticated, professional interface
/// </summary>
public class ModernGameUI : MonoBehaviour
{
    [Header("UI References")]
    private Canvas _mainCanvas;
    private RectTransform _mainPanel;
    private RectTransform _controlPanel;
    private RectTransform _logPanel;
    private RectTransform _statusPanel;
    
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
    private Button[] _nudgeButtons = new Button[4];
    
    [Header("Status Elements")]
    private TextMeshProUGUI _connectionStatus;
    private TextMeshProUGUI _dcValueText;
    private TextMeshProUGUI _logText;
    
    [Header("Visual Elements")]
    private Image _mainPanelBackground;
    private Image _controlPanelBackground;
    private Image _logPanelBackground;
    
    [Header("Configuration")]
    [SerializeField] private Color _primaryColor = new Color(0.1f, 0.1f, 0.15f, 0.95f);
    [SerializeField] private Color _secondaryColor = new Color(0.15f, 0.15f, 0.2f, 0.9f);
    [SerializeField] private Color _accentColor = new Color(0.2f, 0.6f, 1f, 1f);
    [SerializeField] private Color _successColor = new Color(0.2f, 0.8f, 0.4f, 1f);
    [SerializeField] private Color _warningColor = new Color(1f, 0.6f, 0.2f, 1f);
    [SerializeField] private Color _errorColor = new Color(1f, 0.3f, 0.3f, 1f);
    [SerializeField] private Color _textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
    [SerializeField] private Color _textSecondaryColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    
    [Header("Layout")]
    [SerializeField] private float _panelSpacing = 10f;
    [SerializeField] private float _elementSpacing = 8f;
    [SerializeField] private float _borderRadius = 8f;
    [SerializeField] private float _animationDuration = 0.2f;
    
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
        CreateModernUI();
        SetupEventHandlers();
        UpdateUI();
    }
    
    void Update()
    {
        UpdateConnectionStatus();
        UpdateCandidateDisplay();
    }
    
    private void CreateModernUI()
    {
        CreateMainCanvas();
        CreateMainPanel();
        CreateControlPanel();
        CreateLogPanel();
        CreateStatusPanel();
        ApplyModernStyling();
    }
    
    private void CreateMainCanvas()
    {
        var canvasGO = new GameObject("ModernGameCanvas");
        _mainCanvas = canvasGO.AddComponent<Canvas>();
        _mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _mainCanvas.sortingOrder = 100;
        
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Ensure EventSystem exists
        if (EventSystem.current == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }
    
    private void CreateMainPanel()
    {
        var mainPanelGO = new GameObject("MainPanel");
        mainPanelGO.transform.SetParent(_mainCanvas.transform);
        _mainPanel = mainPanelGO.AddComponent<RectTransform>();
        
        // Position at top of screen with proper spacing
        _mainPanel.anchorMin = new Vector2(0, 1);
        _mainPanel.anchorMax = new Vector2(1, 1);
        _mainPanel.pivot = new Vector2(0.5f, 1);
        _mainPanel.anchoredPosition = Vector2.zero;
        _mainPanel.sizeDelta = new Vector2(0, 280);
        
        _mainPanelBackground = mainPanelGO.AddComponent<Image>();
        _mainPanelBackground.color = _primaryColor;
        
        // Add subtle shadow effect
        var shadow = mainPanelGO.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.3f);
        shadow.effectDistance = new Vector2(0, -2);
    }
    
    private void CreateControlPanel()
    {
        var controlPanelGO = new GameObject("ControlPanel");
        controlPanelGO.transform.SetParent(_mainPanel);
        _controlPanel = controlPanelGO.AddComponent<RectTransform>();
        
        _controlPanel.anchorMin = new Vector2(0, 0);
        _controlPanel.anchorMax = new Vector2(1, 1);
        _controlPanel.offsetMin = new Vector2(_panelSpacing, _panelSpacing);
        _controlPanel.offsetMax = new Vector2(-_panelSpacing, -_panelSpacing);
        
        _controlPanelBackground = controlPanelGO.AddComponent<Image>();
        _controlPanelBackground.color = _secondaryColor;
        
        CreateControlElements();
    }
    
    private void CreateControlElements()
    {
        float yOffset = -20f;
        float elementHeight = 40f;
        
        // Top row: Connection status, DC slider, Roll button
        CreateConnectionStatus(new Vector2(20, yOffset));
        CreateDCSlider(new Vector2(200, yOffset));
        CreateRollButton(new Vector2(500, yOffset));
        CreateAutoButton(new Vector2(620, yOffset));
        
        yOffset -= (elementHeight + _elementSpacing);
        
        // Second row: Situation input, Propose button
        CreateSituationInput(new Vector2(20, yOffset), 400);
        CreateProposeButton(new Vector2(440, yOffset));
        CreateSendButton(new Vector2(560, yOffset));
        CreateUseSuggestedDcToggle(new Vector2(680, yOffset));
        
        yOffset -= (elementHeight + _elementSpacing);
        
        // Third row: Result input, Save button, Actor selection
        CreateResultInput(new Vector2(20, yOffset), 300);
        CreateSaveButton(new Vector2(340, yOffset));
        CreateActorButton(new Vector2(460, yOffset));
        
        yOffset -= (elementHeight + _elementSpacing);
        
        // Fourth row: POI, Snap, Candidate navigation
        CreatePOIButton(new Vector2(20, yOffset));
        CreateSnapButton(new Vector2(140, yOffset));
        CreateCandidateNavigation(new Vector2(260, yOffset));
        CreateNudgeButtons(new Vector2(400, yOffset));
    }
    
    private void CreateConnectionStatus(Vector2 position)
    {
        var statusGO = new GameObject("ConnectionStatus");
        statusGO.transform.SetParent(_controlPanel);
        var rect = statusGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(160, 30);
        
        var bg = statusGO.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.3f);
        
        _connectionStatus = CreateTextMeshPro(statusGO.transform, "GW: Connecting...", 14, TextAlignmentOptions.Left);
        _connectionStatus.color = _textSecondaryColor;
    }
    
    private void CreateDCSlider(Vector2 position)
    {
        var sliderGO = new GameObject("DCSlider");
        sliderGO.transform.SetParent(_controlPanel);
        var rect = sliderGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(280, 30);
        
        _dcSlider = sliderGO.AddComponent<Slider>();
        _dcSlider.minValue = 5;
        _dcSlider.maxValue = 20;
        _dcSlider.wholeNumbers = true;
        _dcSlider.value = 10;
        
        // Create background
        var bgGO = new GameObject("Background");
        bgGO.transform.SetParent(sliderGO.transform);
        var bgRect = bgGO.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0, 0, 0, 0.4f);
        _dcSlider.targetGraphic = bgImg;
        
        // Create fill area
        var fillAreaGO = new GameObject("Fill Area");
        fillAreaGO.transform.SetParent(sliderGO.transform);
        var fillAreaRect = fillAreaGO.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(4, 4);
        fillAreaRect.offsetMax = new Vector2(-4, -4);
        
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
        
        // Create handle
        var handleAreaGO = new GameObject("Handle Slide Area");
        handleAreaGO.transform.SetParent(sliderGO.transform);
        var handleAreaRect = handleAreaGO.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(10, 10);
        handleAreaRect.offsetMax = new Vector2(-10, -10);
        
        var handleGO = new GameObject("Handle");
        handleGO.transform.SetParent(handleAreaGO.transform);
        var handleRect = handleGO.AddComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 20);
        var handleImg = handleGO.AddComponent<Image>();
        handleImg.color = Color.white;
        _dcSlider.handleRect = handleRect;
        
        // Create DC value text
        _dcValueText = CreateTextMeshPro(sliderGO.transform, "DC: 10", 12, TextAlignmentOptions.Center);
        var dcTextRect = _dcValueText.GetComponent<RectTransform>();
        dcTextRect.anchorMin = new Vector2(0, 0);
        dcTextRect.anchorMax = new Vector2(1, 0);
        dcTextRect.pivot = new Vector2(0.5f, 0);
        dcTextRect.anchoredPosition = new Vector2(0, -25);
        dcTextRect.sizeDelta = new Vector2(0, 20);
        _dcValueText.color = _textColor;
    }
    
    private void CreateRollButton(Vector2 position)
    {
        _rollButton = CreateModernButton("Roll", position, new Vector2(100, 40), _accentColor);
    }
    
    private void CreateAutoButton(Vector2 position)
    {
        _autoButton = CreateModernButton("Auto: OFF", position, new Vector2(100, 40), _secondaryColor);
    }
    
    private void CreateSituationInput(Vector2 position, float width)
    {
        _situationInput = CreateModernInputField("Describe situation or intent...", position, new Vector2(width, 40));
    }
    
    private void CreateProposeButton(Vector2 position)
    {
        _proposeButton = CreateModernButton("Propose", position, new Vector2(100, 40), _successColor);
    }
    
    private void CreateSendButton(Vector2 position)
    {
        _sendButton = CreateModernButton("Send", position, new Vector2(100, 40), _warningColor);
    }
    
    private void CreateUseSuggestedDcToggle(Vector2 position)
    {
        var toggleGO = new GameObject("UseSuggestedDcToggle");
        toggleGO.transform.SetParent(_controlPanel);
        var rect = toggleGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(200, 40);
        
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
        bgImg.color = new Color(0, 0, 0, 0.3f);
        _useSuggestedDcToggle.targetGraphic = bgImg;
        
        // Checkmark
        var checkmarkGO = new GameObject("Checkmark");
        checkmarkGO.transform.SetParent(toggleGO.transform);
        var checkmarkRect = checkmarkGO.AddComponent<RectTransform>();
        checkmarkRect.anchorMin = new Vector2(0, 0.5f);
        checkmarkRect.anchorMax = new Vector2(0, 0.5f);
        checkmarkRect.pivot = new Vector2(0, 0.5f);
        checkmarkRect.anchoredPosition = new Vector2(10, 0);
        checkmarkRect.sizeDelta = new Vector2(20, 20);
        var checkmarkImg = checkmarkGO.AddComponent<Image>();
        checkmarkImg.color = _accentColor;
        _useSuggestedDcToggle.graphic = checkmarkImg;
        
        // Label
        var label = CreateTextMeshPro(toggleGO.transform, "Use Suggested DC", 14, TextAlignmentOptions.Left);
        var labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(1, 1);
        labelRect.offsetMin = new Vector2(40, 0);
        labelRect.offsetMax = new Vector2(-10, 0);
        label.color = _textColor;
    }
    
    private void CreateResultInput(Vector2 position, float width)
    {
        _resultInput = CreateModernInputField("After roll: DM result note...", position, new Vector2(width, 40));
    }
    
    private void CreateSaveButton(Vector2 position)
    {
        _saveButton = CreateModernButton("Save", position, new Vector2(100, 40), _successColor);
    }
    
    private void CreateActorButton(Vector2 position)
    {
        _actorButton = CreateModernButton("Actor: adv-1", position, new Vector2(120, 40), _secondaryColor);
    }
    
    private void CreatePOIButton(Vector2 position)
    {
        _poiButton = CreateModernButton("POI: none", position, new Vector2(100, 40), _secondaryColor);
    }
    
    private void CreateSnapButton(Vector2 position)
    {
        _snapButton = CreateModernButton("Snap", position, new Vector2(100, 40), _accentColor);
    }
    
    private void CreateCandidateNavigation(Vector2 position)
    {
        _candPrevButton = CreateModernButton("◀", position, new Vector2(40, 40), _secondaryColor);
        _candNextButton = CreateModernButton("▶", new Vector2(position.x + 50, position.y), new Vector2(40, 40), _secondaryColor);
        
        _candText = CreateTextMeshPro(_controlPanel, "Cand 1/1", 14, TextAlignmentOptions.Center);
        var candTextRect = _candText.GetComponent<RectTransform>();
        candTextRect.anchorMin = new Vector2(0, 1);
        candTextRect.anchorMax = new Vector2(0, 1);
        candTextRect.pivot = new Vector2(0, 1);
        candTextRect.anchoredPosition = new Vector2(position.x + 100, position.y);
        candTextRect.sizeDelta = new Vector2(80, 40);
        _candText.color = _textColor;
    }
    
    private void CreateNudgeButtons(Vector2 position)
    {
        string[] directions = { "▲", "▼", "◀", "▶" };
        Vector2[] offsets = { new Vector2(0, 20), new Vector2(0, -20), new Vector2(-20, 0), new Vector2(20, 0) };
        
        for (int i = 0; i < 4; i++)
        {
            _nudgeButtons[i] = CreateModernButton(directions[i], 
                new Vector2(position.x + offsets[i].x, position.y + offsets[i].y), 
                new Vector2(30, 30), _secondaryColor);
        }
    }
    
    private void CreateLogPanel()
    {
        var logPanelGO = new GameObject("LogPanel");
        logPanelGO.transform.SetParent(_mainPanel);
        _logPanel = logPanelGO.AddComponent<RectTransform>();
        
        _logPanel.anchorMin = new Vector2(0, 0);
        _logPanel.anchorMax = new Vector2(0.4f, 1);
        _logPanel.offsetMin = new Vector2(_panelSpacing, _panelSpacing);
        _logPanel.offsetMax = new Vector2(-_panelSpacing/2, -_panelSpacing);
        
        _logPanelBackground = logPanelGO.AddComponent<Image>();
        _logPanelBackground.color = new Color(0, 0, 0, 0.2f);
        
        // Create scroll view for log
        var scrollViewGO = new GameObject("LogScrollView");
        scrollViewGO.transform.SetParent(logPanelGO.transform);
        var scrollViewRect = scrollViewGO.AddComponent<RectTransform>();
        scrollViewRect.anchorMin = Vector2.zero;
        scrollViewRect.anchorMax = Vector2.one;
        scrollViewRect.offsetMin = new Vector2(10, 10);
        scrollViewRect.offsetMax = new Vector2(-10, -10);
        
        var scrollView = scrollViewGO.AddComponent<ScrollRect>();
        var viewportGO = new GameObject("Viewport");
        viewportGO.transform.SetParent(scrollViewGO.transform);
        var viewportRect = viewportGO.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        var viewportImg = viewportGO.AddComponent<Image>();
        viewportImg.color = new Color(0, 0, 0, 0.1f);
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
        contentLayout.padding = new RectOffset(10, 10, 10, 10);
        contentLayout.spacing = 2;
        contentLayout.childControlHeight = false;
        contentLayout.childControlWidth = true;
        contentLayout.childForceExpandHeight = false;
        contentLayout.childForceExpandWidth = true;
        
        _logText = CreateTextMeshPro(contentGO.transform, "", 12, TextAlignmentOptions.TopLeft);
        _logText.color = _textColor;
        var logTextRect = _logText.GetComponent<RectTransform>();
        logTextRect.sizeDelta = new Vector2(0, 200);
        
        scrollView.viewport = viewportRect;
        scrollView.content = contentRect;
        scrollView.horizontal = false;
        scrollView.vertical = true;
    }
    
    private void CreateStatusPanel()
    {
        var statusPanelGO = new GameObject("StatusPanel");
        statusPanelGO.transform.SetParent(_mainPanel);
        _statusPanel = statusPanelGO.AddComponent<RectTransform>();
        
        _statusPanel.anchorMin = new Vector2(0.4f, 0);
        _statusPanel.anchorMax = new Vector2(1, 1);
        _statusPanel.offsetMin = new Vector2(_panelSpacing/2, _panelSpacing);
        _statusPanel.offsetMax = new Vector2(-_panelSpacing, -_panelSpacing);
        
        var statusBg = statusPanelGO.AddComponent<Image>();
        statusBg.color = new Color(0, 0, 0, 0.1f);
    }
    
    private Button CreateModernButton(string label, Vector2 position, Vector2 size, Color color)
    {
        var buttonGO = new GameObject(label + "Button");
        buttonGO.transform.SetParent(_controlPanel);
        var rect = buttonGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        
        var button = buttonGO.AddComponent<Button>();
        var img = buttonGO.AddComponent<Image>();
        img.color = color;
        
        // Add hover effect
        var colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = new Color(color.r * 1.2f, color.g * 1.2f, color.b * 1.2f, color.a);
        colors.pressedColor = new Color(color.r * 0.8f, color.g * 0.8f, color.b * 0.8f, color.a);
        button.colors = colors;
        
        var labelText = CreateTextMeshPro(buttonGO.transform, label, 14, TextAlignmentOptions.Center);
        labelText.color = Color.white;
        var labelRect = labelText.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        
        return button;
    }
    
    private TMP_InputField CreateModernInputField(string placeholder, Vector2 position, Vector2 size)
    {
        var inputGO = new GameObject("InputField");
        inputGO.transform.SetParent(_controlPanel);
        var rect = inputGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        
        var input = inputGO.AddComponent<TMP_InputField>();
        var img = inputGO.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.4f);
        input.targetGraphic = img;
        
        // Text area
        var textAreaGO = new GameObject("Text Area");
        textAreaGO.transform.SetParent(inputGO.transform);
        var textAreaRect = textAreaGO.AddComponent<RectTransform>();
        textAreaRect.anchorMin = Vector2.zero;
        textAreaRect.anchorMax = Vector2.one;
        textAreaRect.offsetMin = new Vector2(10, 5);
        textAreaRect.offsetMax = new Vector2(-10, -5);
        textAreaGO.AddComponent<Mask>();
        
        var textGO = new GameObject("Text");
        textGO.transform.SetParent(textAreaGO.transform);
        var textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        var text = textGO.AddComponent<TextMeshProUGUI>();
        text.color = _textColor;
        text.fontSize = 14;
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
        placeholderText.color = _textSecondaryColor;
        placeholderText.fontSize = 14;
        placeholderText.alignment = TextAlignmentOptions.Left;
        input.placeholder = placeholderText;
        
        return input;
    }
    
    private TextMeshProUGUI CreateTextMeshPro(Transform parent, string text, int fontSize, TextAlignmentOptions alignment)
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
        tmp.color = _textColor;
        
        return tmp;
    }
    
    private void ApplyModernStyling()
    {
        // Apply rounded corners and shadows to all panels
        ApplyRoundedCorners(_mainPanelBackground, _borderRadius);
        ApplyRoundedCorners(_controlPanelBackground, _borderRadius);
        ApplyRoundedCorners(_logPanelBackground, _borderRadius);
    }
    
    private void ApplyRoundedCorners(Image image, float radius)
    {
        // Note: Unity doesn't have built-in rounded corners, but we can simulate with proper colors and shadows
        // In a real implementation, you might want to use a custom shader or UI asset
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
        
        for (int i = 0; i < 4; i++)
        {
            int index = i; // Capture for closure
            _nudgeButtons[i].onClick.AddListener(() => OnNudgeClicked(index));
        }
    }
    
    private void UpdateConnectionStatus()
    {
        var brainClient = GetComponent<BrainClient>();
        bool isConnected = brainClient != null && brainClient.IsConnected;
        
        _connectionStatus.text = isConnected ? "GW: Connected" : "GW: Offline";
        _connectionStatus.color = isConnected ? _successColor : _errorColor;
        
        var bg = _connectionStatus.transform.parent.GetComponent<Image>();
        bg.color = isConnected ? new Color(0, 0.3f, 0, 0.3f) : new Color(0.3f, 0, 0, 0.3f);
    }
    
    private void UpdateCandidateDisplay()
    {
        _candText.text = $"Cand {_candIndex + 1}/{_candCount}";
    }
    
    private void UpdateUI()
    {
        _dcValueText.text = $"DC: {_dcSlider.value}";
        _autoButton.GetComponentInChildren<TextMeshProUGUI>().text = _isAutoMode ? "Auto: ON" : "Auto: OFF";
        _autoButton.GetComponent<Image>().color = _isAutoMode ? _successColor : _secondaryColor;
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
            // Create a simple proposal based on the situation input
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
                // Add the result note to DM narration
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
            // Send the situation to the gateway
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
    
    private void OnNudgeClicked(int direction)
    {
        var planner = GetComponent<Planner>();
        if (planner != null)
        {
            var agent = planner.ResolveAgentFor(_actors[_actorIndex]);
            if (agent != null)
            {
                Vector3 offset = Vector3.zero;
                switch (direction)
                {
                    case 0: offset = Vector3.forward; break;  // Up
                    case 1: offset = Vector3.back; break;     // Down
                    case 2: offset = Vector3.left; break;     // Left
                    case 3: offset = Vector3.right; break;    // Right
                }
                
                agent.position += offset;
                AppendLog($"[DM] Nudged {_actors[_actorIndex]} {offset}");
            }
        }
    }
    
    public void AppendLog(string line)
    {
        if (_logText != null)
        {
            _logText.text += (string.IsNullOrEmpty(_logText.text) ? "" : "\n") + line;
            
            // Auto-scroll to bottom
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
