using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using System;

/// <summary>
/// Master UX/UI Design - Professional Game Director Interface
/// Designed with accessibility, readability, and usability as top priorities
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
    
    [Header("Collapsible Sections")]
    private Button _logToggleButton;
    private Button _controlsToggleButton;
    private bool _logCollapsed = false;
    private bool _controlsCollapsed = false;
    
    [Header("Master Design System")]
    [SerializeField] private Color _primaryBackground = new Color(0.08f, 0.08f, 0.12f, 0.98f);
    [SerializeField] private Color _secondaryBackground = new Color(0.12f, 0.12f, 0.16f, 0.95f);
    [SerializeField] private Color _accentColor = new Color(0.2f, 0.6f, 1f, 1f);
    [SerializeField] private Color _successColor = new Color(0.2f, 0.8f, 0.4f, 1f);
    [SerializeField] private Color _warningColor = new Color(1f, 0.7f, 0.2f, 1f);
    [SerializeField] private Color _errorColor = new Color(1f, 0.4f, 0.4f, 1f);
    [SerializeField] private Color _textPrimary = new Color(0.95f, 0.95f, 0.95f, 1f);
    [SerializeField] private Color _textSecondary = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color _textMuted = new Color(0.6f, 0.6f, 0.6f, 1f);
    
    [Header("Typography & Spacing")]
    [SerializeField] private int _titleFontSize = 18;
    [SerializeField] private int _bodyFontSize = 16;
    [SerializeField] private int _smallFontSize = 14;
    [SerializeField] private float _sectionSpacing = 20f;
    [SerializeField] private float _elementSpacing = 12f;
    [SerializeField] private float _buttonHeight = 48f;
    [SerializeField] private float _inputHeight = 44f;
    
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
        CreateMasterUI();
        SetupEventHandlers();
        UpdateUI();
    }
    
    void Update()
    {
        UpdateConnectionStatus();
        UpdateCandidateDisplay();
    }
    
    private void CreateMasterUI()
    {
        CreateMainCanvas();
        CreateMainPanel();
        CreateHeaderSection();
        CreateControlsSection();
        CreateLogSection();
        ApplyMasterStyling();
    }
    
    private void CreateMainCanvas()
    {
        var canvasGO = new GameObject("MasterGameCanvas");
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
    
    private void CreateMainPanel()
    {
        var mainPanelGO = new GameObject("MainPanel");
        mainPanelGO.transform.SetParent(_mainCanvas.transform);
        _mainPanel = mainPanelGO.AddComponent<RectTransform>();
        
        _mainPanel.anchorMin = new Vector2(0, 1);
        _mainPanel.anchorMax = new Vector2(1, 1);
        _mainPanel.pivot = new Vector2(0.5f, 1);
        _mainPanel.anchoredPosition = Vector2.zero;
        _mainPanel.sizeDelta = new Vector2(0, 320);
        
        var bg = mainPanelGO.AddComponent<Image>();
        bg.color = _primaryBackground;
        
        // Add subtle shadow
        var shadow = mainPanelGO.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.4f);
        shadow.effectDistance = new Vector2(0, -4);
    }
    
    private void CreateHeaderSection()
    {
        var headerGO = new GameObject("HeaderSection");
        headerGO.transform.SetParent(_mainPanel);
        var headerRect = headerGO.AddComponent<RectTransform>();
        
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.pivot = new Vector2(0, 1);
        headerRect.anchoredPosition = Vector2.zero;
        headerRect.sizeDelta = new Vector2(0, 60);
        
        var headerBg = headerGO.AddComponent<Image>();
        headerBg.color = _secondaryBackground;
        
        // Title
        var title = CreateTextMeshPro(headerGO.transform, "Game Director Console", _titleFontSize, TextAlignmentOptions.Left);
        title.color = _textPrimary;
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0);
        titleRect.anchorMax = new Vector2(0.5f, 1);
        titleRect.offsetMin = new Vector2(20, 0);
        titleRect.offsetMax = new Vector2(-10, 0);
        
        // Connection Status
        CreateConnectionStatus(headerGO.transform, new Vector2(0.5f, 0.5f));
    }
    
    private void CreateConnectionStatus(Transform parent, Vector2 anchor)
    {
        var statusGO = new GameObject("ConnectionStatus");
        statusGO.transform.SetParent(parent);
        var rect = statusGO.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(200, 40);
        
        var bg = statusGO.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.3f);
        
        _connectionStatus = CreateTextMeshPro(statusGO.transform, "Connecting...", _bodyFontSize, TextAlignmentOptions.Center);
        _connectionStatus.color = _textSecondary;
    }
    
    private void CreateControlsSection()
    {
        var controlsGO = new GameObject("ControlsSection");
        controlsGO.transform.SetParent(_mainPanel);
        var controlsRect = controlsGO.AddComponent<RectTransform>();
        
        controlsRect.anchorMin = new Vector2(0, 1);
        controlsRect.anchorMax = new Vector2(1, 1);
        controlsRect.pivot = new Vector2(0, 1);
        controlsRect.anchoredPosition = new Vector2(0, -60);
        controlsRect.sizeDelta = new Vector2(0, 180);
        
        _controlPanel = controlsRect;
        
        // Toggle button for controls
        _controlsToggleButton = CreateToggleButton(controlsGO.transform, "▼ Controls", new Vector2(0, 1), new Vector2(150, 30));
        _controlsToggleButton.onClick.AddListener(ToggleControls);
        
        // Controls content
        CreateControlsContent(controlsGO.transform);
    }
    
    private void CreateControlsContent(Transform parent)
    {
        float yOffset = -40f;
        float xOffset = 20f;
        
        // Row 1: DC Slider and Roll Button
        CreateDCSlider(parent, new Vector2(xOffset, yOffset));
        CreateRollButton(parent, new Vector2(xOffset + 300, yOffset));
        CreateAutoButton(parent, new Vector2(xOffset + 420, yOffset));
        
        yOffset -= (_buttonHeight + _elementSpacing);
        
        // Row 2: Situation Input and Action Buttons
        CreateSituationInput(parent, new Vector2(xOffset, yOffset), 400);
        CreateProposeButton(parent, new Vector2(xOffset + 420, yOffset));
        CreateSendButton(parent, new Vector2(xOffset + 540, yOffset));
        
        yOffset -= (_inputHeight + _elementSpacing);
        
        // Row 3: Result Input and Save Button
        CreateResultInput(parent, new Vector2(xOffset, yOffset), 300);
        CreateSaveButton(parent, new Vector2(xOffset + 320, yOffset));
        CreateActorButton(parent, new Vector2(xOffset + 440, yOffset));
        
        yOffset -= (_inputHeight + _elementSpacing);
        
        // Row 4: Navigation and Toggle
        CreatePOIButton(parent, new Vector2(xOffset, yOffset));
        CreateSnapButton(parent, new Vector2(xOffset + 120, yOffset));
        CreateCandidateNavigation(parent, new Vector2(xOffset + 240, yOffset));
        CreateUseSuggestedDcToggle(parent, new Vector2(xOffset + 400, yOffset));
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
        rect.sizeDelta = new Vector2(280, 40);
        
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
        bgImg.color = new Color(0, 0, 0, 0.4f);
        _dcSlider.targetGraphic = bgImg;
        
        // Fill area
        var fillAreaGO = new GameObject("Fill Area");
        fillAreaGO.transform.SetParent(sliderGO.transform);
        var fillAreaRect = fillAreaGO.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(6, 6);
        fillAreaRect.offsetMax = new Vector2(-6, -6);
        
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
        handleAreaRect.offsetMin = new Vector2(10, 10);
        handleAreaRect.offsetMax = new Vector2(-10, -10);
        
        var handleGO = new GameObject("Handle");
        handleGO.transform.SetParent(handleAreaGO.transform);
        var handleRect = handleGO.AddComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(24, 24);
        var handleImg = handleGO.AddComponent<Image>();
        handleImg.color = Color.white;
        _dcSlider.handleRect = handleRect;
        
        // DC value text
        _dcValueText = CreateTextMeshPro(sliderGO.transform, "DC: 10", _bodyFontSize, TextAlignmentOptions.Center);
        var dcTextRect = _dcValueText.GetComponent<RectTransform>();
        dcTextRect.anchorMin = new Vector2(0, 0);
        dcTextRect.anchorMax = new Vector2(1, 0);
        dcTextRect.pivot = new Vector2(0.5f, 0);
        dcTextRect.anchoredPosition = new Vector2(0, -30);
        dcTextRect.sizeDelta = new Vector2(0, 20);
        _dcValueText.color = _textPrimary;
    }
    
    private void CreateRollButton(Transform parent, Vector2 position)
    {
        _rollButton = CreateMasterButton(parent, "Roll", position, new Vector2(100, _buttonHeight), _accentColor);
    }
    
    private void CreateAutoButton(Transform parent, Vector2 position)
    {
        _autoButton = CreateMasterButton(parent, "Auto: OFF", position, new Vector2(100, _buttonHeight), _secondaryBackground);
    }
    
    private void CreateSituationInput(Transform parent, Vector2 position, float width)
    {
        _situationInput = CreateMasterInputField(parent, "Describe situation or intent...", position, new Vector2(width, _inputHeight));
    }
    
    private void CreateProposeButton(Transform parent, Vector2 position)
    {
        _proposeButton = CreateMasterButton(parent, "Propose", position, new Vector2(100, _buttonHeight), _successColor);
    }
    
    private void CreateSendButton(Transform parent, Vector2 position)
    {
        _sendButton = CreateMasterButton(parent, "Send", position, new Vector2(100, _buttonHeight), _warningColor);
    }
    
    private void CreateResultInput(Transform parent, Vector2 position, float width)
    {
        _resultInput = CreateMasterInputField(parent, "After roll: DM result note...", position, new Vector2(width, _inputHeight));
    }
    
    private void CreateSaveButton(Transform parent, Vector2 position)
    {
        _saveButton = CreateMasterButton(parent, "Save", position, new Vector2(100, _buttonHeight), _successColor);
    }
    
    private void CreateActorButton(Transform parent, Vector2 position)
    {
        _actorButton = CreateMasterButton(parent, "Actor: adv-1", position, new Vector2(120, _buttonHeight), _secondaryBackground);
    }
    
    private void CreatePOIButton(Transform parent, Vector2 position)
    {
        _poiButton = CreateMasterButton(parent, "POI: none", position, new Vector2(100, _buttonHeight), _secondaryBackground);
    }
    
    private void CreateSnapButton(Transform parent, Vector2 position)
    {
        _snapButton = CreateMasterButton(parent, "Snap", position, new Vector2(100, _buttonHeight), _accentColor);
    }
    
    private void CreateCandidateNavigation(Transform parent, Vector2 position)
    {
        _candPrevButton = CreateMasterButton(parent, "◀", position, new Vector2(40, _buttonHeight), _secondaryBackground);
        _candNextButton = CreateMasterButton(parent, "▶", new Vector2(position.x + 50, position.y), new Vector2(40, _buttonHeight), _secondaryBackground);
        
        _candText = CreateTextMeshPro(parent, "Cand 1/1", _bodyFontSize, TextAlignmentOptions.Center);
        var candTextRect = _candText.GetComponent<RectTransform>();
        candTextRect.anchorMin = new Vector2(0, 1);
        candTextRect.anchorMax = new Vector2(0, 1);
        candTextRect.pivot = new Vector2(0, 1);
        candTextRect.anchoredPosition = new Vector2(position.x + 100, position.y);
        candTextRect.sizeDelta = new Vector2(80, _buttonHeight);
        _candText.color = _textPrimary;
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
        bgImg.color = new Color(0, 0, 0, 0.3f);
        _useSuggestedDcToggle.targetGraphic = bgImg;
        
        // Checkmark
        var checkmarkGO = new GameObject("Checkmark");
        checkmarkGO.transform.SetParent(toggleGO.transform);
        var checkmarkRect = checkmarkGO.AddComponent<RectTransform>();
        checkmarkRect.anchorMin = new Vector2(0, 0.5f);
        checkmarkRect.anchorMax = new Vector2(0, 0.5f);
        checkmarkRect.pivot = new Vector2(0, 0.5f);
        checkmarkRect.anchoredPosition = new Vector2(15, 0);
        checkmarkRect.sizeDelta = new Vector2(24, 24);
        var checkmarkImg = checkmarkGO.AddComponent<Image>();
        checkmarkImg.color = _accentColor;
        _useSuggestedDcToggle.graphic = checkmarkImg;
        
        // Label
        var label = CreateTextMeshPro(toggleGO.transform, "Use Suggested DC", _bodyFontSize, TextAlignmentOptions.Left);
        var labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(1, 1);
        labelRect.offsetMin = new Vector2(50, 0);
        labelRect.offsetMax = new Vector2(-10, 0);
        label.color = _textPrimary;
    }
    
    private void CreateLogSection()
    {
        var logGO = new GameObject("LogSection");
        logGO.transform.SetParent(_mainPanel);
        var logRect = logGO.AddComponent<RectTransform>();
        
        logRect.anchorMin = new Vector2(0, 1);
        logRect.anchorMax = new Vector2(1, 1);
        logRect.pivot = new Vector2(0, 1);
        logRect.anchoredPosition = new Vector2(0, -240);
        logRect.sizeDelta = new Vector2(0, 80);
        
        _logPanel = logRect;
        
        // Toggle button for log
        _logToggleButton = CreateToggleButton(logGO.transform, "▼ Log", new Vector2(0, 1), new Vector2(100, 30));
        _logToggleButton.onClick.AddListener(ToggleLog);
        
        // Log content
        CreateLogContent(logGO.transform);
    }
    
    private void CreateLogContent(Transform parent)
    {
        var logContentGO = new GameObject("LogContent");
        logContentGO.transform.SetParent(parent);
        var logContentRect = logContentGO.AddComponent<RectTransform>();
        logContentRect.anchorMin = new Vector2(0, 0);
        logContentRect.anchorMax = new Vector2(1, 1);
        logContentRect.offsetMin = new Vector2(0, 30);
        logContentRect.offsetMax = new Vector2(0, 0);
        
        var logBg = logContentGO.AddComponent<Image>();
        logBg.color = new Color(0, 0, 0, 0.2f);
        
        // Scroll view for log
        var scrollViewGO = new GameObject("LogScrollView");
        scrollViewGO.transform.SetParent(logContentGO.transform);
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
        contentLayout.spacing = 4;
        contentLayout.childControlHeight = false;
        contentLayout.childControlWidth = true;
        contentLayout.childForceExpandHeight = false;
        contentLayout.childForceExpandWidth = true;
        
        _logText = CreateTextMeshPro(contentGO.transform, "", _smallFontSize, TextAlignmentOptions.TopLeft);
        _logText.color = _textSecondary;
        var logTextRect = _logText.GetComponent<RectTransform>();
        logTextRect.sizeDelta = new Vector2(0, 200);
        
        scrollView.viewport = viewportRect;
        scrollView.content = contentRect;
        scrollView.horizontal = false;
        scrollView.vertical = true;
    }
    
    private Button CreateToggleButton(Transform parent, string label, Vector2 anchor, Vector2 size)
    {
        var buttonGO = new GameObject(label + "Toggle");
        buttonGO.transform.SetParent(parent);
        var rect = buttonGO.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
        
        var button = buttonGO.AddComponent<Button>();
        var img = buttonGO.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.3f);
        
        var colors = button.colors;
        colors.normalColor = new Color(0, 0, 0, 0.3f);
        colors.highlightedColor = new Color(0, 0, 0, 0.5f);
        colors.pressedColor = new Color(0, 0, 0, 0.7f);
        button.colors = colors;
        
        var labelText = CreateTextMeshPro(buttonGO.transform, label, _smallFontSize, TextAlignmentOptions.Left);
        labelText.color = _textSecondary;
        var labelRect = labelText.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(10, 0);
        labelRect.offsetMax = new Vector2(-10, 0);
        
        return button;
    }
    
    private Button CreateMasterButton(Transform parent, string label, Vector2 position, Vector2 size, Color color)
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
        colors.highlightedColor = new Color(color.r * 1.2f, color.g * 1.2f, color.b * 1.2f, color.a);
        colors.pressedColor = new Color(color.r * 0.8f, color.g * 0.8f, color.b * 0.8f, color.a);
        button.colors = colors;
        
        var labelText = CreateTextMeshPro(buttonGO.transform, label, _bodyFontSize, TextAlignmentOptions.Center);
        labelText.color = Color.white;
        var labelRect = labelText.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        
        return button;
    }
    
    private TMP_InputField CreateMasterInputField(Transform parent, string placeholder, Vector2 position, Vector2 size)
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
        img.color = new Color(0, 0, 0, 0.4f);
        input.targetGraphic = img;
        
        // Text area
        var textAreaGO = new GameObject("Text Area");
        textAreaGO.transform.SetParent(inputGO.transform);
        var textAreaRect = textAreaGO.AddComponent<RectTransform>();
        textAreaRect.anchorMin = Vector2.zero;
        textAreaRect.anchorMax = Vector2.one;
        textAreaRect.offsetMin = new Vector2(12, 8);
        textAreaRect.offsetMax = new Vector2(-12, -8);
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
        tmp.color = _textPrimary;
        
        return tmp;
    }
    
    private void ApplyMasterStyling()
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
        _autoButton.GetComponent<Image>().color = _isAutoMode ? _successColor : _secondaryBackground;
        _actorButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Actor: {_actors[_actorIndex]}";
    }
    
    private void ToggleLog()
    {
        _logCollapsed = !_logCollapsed;
        var logContent = _logPanel.Find("LogContent");
        if (logContent != null)
        {
            logContent.gameObject.SetActive(!_logCollapsed);
        }
        _logToggleButton.GetComponentInChildren<TextMeshProUGUI>().text = _logCollapsed ? "▶ Log" : "▼ Log";
    }
    
    private void ToggleControls()
    {
        _controlsCollapsed = !_controlsCollapsed;
        var controlsContent = _controlPanel.Find("ControlsContent");
        if (controlsContent != null)
        {
            controlsContent.gameObject.SetActive(!_controlsCollapsed);
        }
        _controlsToggleButton.GetComponentInChildren<TextMeshProUGUI>().text = _controlsCollapsed ? "▶ Controls" : "▼ Controls";
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
