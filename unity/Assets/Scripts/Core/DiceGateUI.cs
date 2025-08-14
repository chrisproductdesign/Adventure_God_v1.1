using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DiceGateUI : MonoBehaviour
{
	private Slider _slider;
	private Button _rollButton;
	private Button _autoButton;
	private Text _logText;
	private InputField _dmInput;
	private Button _proposeButton;
	private InputField _resultInput;
	private Button _saveResultButton;
	private Button _sendToGatewayButton;
	private Toggle _useSuggestedDc;
	private Button _nudgeLeft;
	private Button _nudgeRight;
	private Button _nudgeUp;
	private Button _nudgeDown;
	private Button _actorButton;
	private Text _connText;
	private readonly string[] _actors = new[] { "adv-1", "adv-2", "adv-3" };
	private int _actorIndex = 0;
	private string _selectedActor = "adv-1";
	private Button _snapButton;
	private Button _poiButton;
	private List<string> _poiNames = new List<string>();
	private int _poiIndex = -1;
	private string _selectedPoi = "";
	private Button _candPrev;
	private Button _candNext;
	private Text _candText;
	private int _candIndex = 0;
	private int _candCount = 0;

	void Start()
	{
		// Build a minimal UI at runtime so no scene asset is required
		var canvasGO = new GameObject("DiceCanvas");
		var canvas = canvasGO.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvasGO.AddComponent<CanvasScaler>();
		canvasGO.AddComponent<GraphicRaycaster>();

		// Ensure an EventSystem exists for UI interaction
		if (EventSystem.current == null)
		{
			var es = new GameObject("EventSystem");
			es.AddComponent<EventSystem>();
			es.AddComponent<StandaloneInputModule>();
		}

		_slider = CreateSlider(new Vector2(160, -40));
		_rollButton = CreateButton("Roll", new Vector2(160, -80));
		_autoButton = CreateButton("Auto: ON", new Vector2(50, -80));
		_dmInput = CreateInput(new Vector2(320, -80), 280, 30, "Describe situation or intent (e.g., adv-1 move 0 5)");
		_proposeButton = CreateButton("Propose", new Vector2(620, -80));
		_sendToGatewayButton = CreateButton("Send→GW", new Vector2(720, -80));
		_resultInput = CreateInput(new Vector2(320, -120), 280, 30, "After roll: DM result note (e.g., 'door opens')");
		_saveResultButton = CreateButton("Save Result", new Vector2(620, -120));
		_logText = CreateText(new Vector2(10, -170), 14, 800, 290);
		_useSuggestedDc = CreateToggle(new Vector2(760, -80), "Use suggestedDC");
		_nudgeLeft = CreateButton("◀", new Vector2(900, -80));
		_nudgeRight = CreateButton("▶", new Vector2(936, -80));
		_nudgeUp = CreateButton("▲", new Vector2(918, -108));
		_nudgeDown = CreateButton("▼", new Vector2(918, -52));
		_actorButton = CreateButton("Actor: adv-1", new Vector2(50, -120));
		_connText = CreateText(new Vector2(10, -10), 12, 140, 18);
		_snapButton = CreateButton("Snap to POI", new Vector2(360, -150));
		_poiButton = CreateButton("POI: none", new Vector2(200, -150));
		_candPrev = CreateButton("◄", new Vector2(200, -180));
		_candNext = CreateButton("►", new Vector2(360, -180));
		_candText = CreateText(new Vector2(260, -180), 14, 90, 24);

		_rollButton.onClick.AddListener(() => GetComponent<DiceGate>()?.RerollLast());
		_proposeButton.onClick.AddListener(OnProposeClicked);
		_saveResultButton.onClick.AddListener(OnSaveResultClicked);
		_sendToGatewayButton.onClick.AddListener(SendContextToGateway);
		_autoButton.onClick.AddListener(ToggleAuto);
		_actorButton.onClick.AddListener(CycleActor);
		_snapButton.onClick.AddListener(SnapToPoiSelected);
		_poiButton.onClick.AddListener(CyclePoi);
		_candPrev.onClick.AddListener(() => CycleCandidate(-1));
		_candNext.onClick.AddListener(() => CycleCandidate(1));
		_slider.onValueChanged.AddListener(v => GetComponent<DiceGate>()?.SetDC(v));
		_slider.minValue = 5;
		_slider.maxValue = 20;
		_slider.wholeNumbers = true;
		_slider.value = 10;
		_useSuggestedDc.onValueChanged.AddListener(OnUseSuggestedDcChanged);
		_nudgeLeft.onClick.AddListener(() => NudgeSelected(-1, 0));
		_nudgeRight.onClick.AddListener(() => NudgeSelected(1, 0));
		_nudgeUp.onClick.AddListener(() => NudgeSelected(0, 1));
		_nudgeDown.onClick.AddListener(() => NudgeSelected(0, -1));
	}

	void Update()
	{
		var bc = GetComponent<BrainClient>();
		if (_connText != null)
		{
			bool ok = bc != null && bc.IsConnected;
			_connText.text = ok ? "GW: Connected" : "GW: Offline";
			_connText.color = ok ? Color.green : Color.red;
		}
		// Update candidate display from last proposal if available
		var gate = GetComponent<DiceGate>();
		if (gate != null && _candText != null)
		{
			// We don't have direct access to last proposal; just show index/count if set via cycling
			_candText.text = "Cand " + (Mathf.Clamp(_candIndex, 0, Mathf.Max(0, _candCount - 1)) + 1) + "/" + Mathf.Max(1, _candCount);
		}
	}

	public void AppendLog(string line)
	{
		if (_logText == null) return;
		_logText.text += (string.IsNullOrEmpty(_logText.text) ? "" : "\n") + line;
	}

	private Slider CreateSlider(Vector2 anchoredPos)
	{
		var root = new GameObject("DC Slider");
		root.transform.SetParent(GameObject.Find("DiceCanvas").transform);
		var rect = root.AddComponent<RectTransform>();
		rect.anchorMin = new Vector2(0, 1);
		rect.anchorMax = new Vector2(0, 1);
		rect.pivot = new Vector2(0, 1);
		rect.anchoredPosition = anchoredPos;
		rect.sizeDelta = new Vector2(300, 20);
		root.AddComponent<CanvasRenderer>();

		// Background
		var background = new GameObject("Background");
		background.transform.SetParent(root.transform);
		var bgRect = background.AddComponent<RectTransform>();
		bgRect.anchorMin = new Vector2(0, 0);
		bgRect.anchorMax = new Vector2(1, 1);
		bgRect.offsetMin = Vector2.zero;
		bgRect.offsetMax = Vector2.zero;
		var bgImg = background.AddComponent<Image>();
		bgImg.color = new Color(0, 0, 0, 0.25f);

		// Fill Area / Fill
		var fillArea = new GameObject("Fill Area");
		fillArea.transform.SetParent(root.transform);
		var faRect = fillArea.AddComponent<RectTransform>();
		faRect.anchorMin = new Vector2(0, 0);
		faRect.anchorMax = new Vector2(1, 1);
		faRect.offsetMin = new Vector2(5, 5);
		faRect.offsetMax = new Vector2(-25, -5);

		var fill = new GameObject("Fill");
		fill.transform.SetParent(fillArea.transform);
		var fillRect = fill.AddComponent<RectTransform>();
		fillRect.anchorMin = new Vector2(0, 0);
		fillRect.anchorMax = new Vector2(1, 1);
		fillRect.offsetMin = Vector2.zero;
		fillRect.offsetMax = Vector2.zero;
		var fillImg = fill.AddComponent<Image>();
		fillImg.color = new Color(0.2f, 0.8f, 0.2f, 0.9f);

		// Handle Slide Area / Handle
		var handleArea = new GameObject("Handle Slide Area");
		handleArea.transform.SetParent(root.transform);
		var haRect = handleArea.AddComponent<RectTransform>();
		haRect.anchorMin = new Vector2(0, 0);
		haRect.anchorMax = new Vector2(1, 1);
		haRect.offsetMin = new Vector2(10, 10);
		haRect.offsetMax = new Vector2(-10, -10);

		var handle = new GameObject("Handle");
		handle.transform.SetParent(handleArea.transform);
		var handleRect = handle.AddComponent<RectTransform>();
		handleRect.sizeDelta = new Vector2(16, 16);
		var handleImg = handle.AddComponent<Image>();
		handleImg.color = new Color(0.9f, 0.9f, 0.9f, 1f);

		var slider = root.AddComponent<Slider>();
		slider.fillRect = fillRect;
		slider.handleRect = handleRect;
		slider.targetGraphic = handleImg;
		slider.direction = Slider.Direction.LeftToRight;
		return slider;
	}

	private Button CreateButton(string label, Vector2 anchoredPos)
	{
		var go = new GameObject(label + " Button");
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
		txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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
		txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		txt.fontSize = fontSize;
		txt.color = Color.white;
		txt.alignment = TextAnchor.UpperLeft;
		return txt;
	}

	private InputField CreateInput(Vector2 anchoredPos, float width, float height, string placeholder)
	{
		var go = new GameObject("DM Input");
		go.transform.SetParent(GameObject.Find("DiceCanvas").transform);
		var rect = go.AddComponent<RectTransform>();
		rect.anchorMin = new Vector2(0, 1);
		rect.anchorMax = new Vector2(0, 1);
		rect.pivot = new Vector2(0, 1);
		rect.anchoredPosition = anchoredPos;
		rect.sizeDelta = new Vector2(width, height);
		go.AddComponent<CanvasRenderer>();
		var img = go.AddComponent<Image>();
		img.color = new Color(0, 0, 0, 0.4f);
		var input = go.AddComponent<InputField>();
		input.textComponent = CreateText(new Vector2(anchoredPos.x + 5, anchoredPos.y - 3), 14, width - 10, height - 6);
		var phGO = new GameObject("Placeholder");
		phGO.transform.SetParent(go.transform);
		var phRect = phGO.AddComponent<RectTransform>();
		phRect.anchorMin = new Vector2(0, 1);
		phRect.anchorMax = new Vector2(0, 1);
		phRect.pivot = new Vector2(0, 1);
		phRect.anchoredPosition = new Vector2(5, -3);
		phRect.sizeDelta = new Vector2(width - 10, height - 6);
		var phText = phGO.AddComponent<Text>();
		phText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		phText.fontSize = 14;
		phText.color = new Color(1, 1, 1, 0.5f);
		phText.alignment = TextAnchor.UpperLeft;
		phText.text = placeholder;
		input.placeholder = phText;
		return input;
	}

	private Toggle CreateToggle(Vector2 anchoredPos, string label)
	{
		var go = new GameObject(label + " Toggle");
		go.transform.SetParent(GameObject.Find("DiceCanvas").transform);
		var rect = go.AddComponent<RectTransform>();
		rect.anchorMin = new Vector2(0, 1);
		rect.anchorMax = new Vector2(0, 1);
		rect.pivot = new Vector2(0, 1);
		rect.anchoredPosition = anchoredPos;
		rect.sizeDelta = new Vector2(150, 24);
		go.AddComponent<CanvasRenderer>();
		var img = go.AddComponent<Image>();
		img.color = new Color(0, 0, 0, 0.25f);
		var toggle = go.AddComponent<Toggle>();
		var labelGO = new GameObject("Label");
		labelGO.transform.SetParent(go.transform);
		var txt = labelGO.AddComponent<Text>();
		txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		txt.text = label;
		txt.alignment = TextAnchor.MiddleLeft;
		txt.color = Color.white;
		var lrect = labelGO.GetComponent<RectTransform>();
		lrect.anchorMin = new Vector2(0, 0);
		lrect.anchorMax = new Vector2(1, 1);
		lrect.offsetMin = new Vector2(24, 0);
		lrect.offsetMax = new Vector2(0, 0);
		return toggle;
	}

	private bool _respectSuggestedDc = false;
	private void OnUseSuggestedDcChanged(bool on)
	{
		_respectSuggestedDc = on;
		AppendLog(on ? "[DM] Using suggestedDC when present" : "[DM] Ignoring suggestedDC");
	}

	private string _lastActorForNudge = "adv-1";
	private void NudgeSelected(float dx, float dz)
	{
		// Nudge the last actor you staged/rolled for, falling back to adv-1
		var planner = GetComponent<Planner>();
		var actor = string.IsNullOrEmpty(_selectedActor) ? _lastActorForNudge : _selectedActor;
		var agent = planner != null ? planner.ResolveAgentFor(actor) : null;
		if (agent == null) return;
		agent.position += new Vector3(dx, 0, dz);
		var party = GetComponent<PartyState>();
		if (party != null) party.SyncFromTransform(actor, agent);
		AppendLog($"[DM] Nudged {actor} by ({dx},{dz})");
	}

	private void CycleActor()
	{
		_actorIndex = (_actorIndex + 1) % _actors.Length;
		_selectedActor = _actors[_actorIndex];
		var label = _actorButton != null ? _actorButton.GetComponentInChildren<Text>() : null;
		if (label != null) label.text = "Actor: " + _selectedActor;
		AppendLog("[DM] Selected actor " + _selectedActor);
	}

	private void SnapToPoiSelected()
	{
		if (string.IsNullOrEmpty(_selectedPoi))
		{
			AppendLog("[DM] No POI selected");
			return;
		}
		var target = POIRegistry.Get(_selectedPoi);
		if (target == null)
		{
			AppendLog("[DM] POI not found");
			return;
		}
		var planner = GetComponent<Planner>();
		var actor = string.IsNullOrEmpty(_selectedActor) ? _lastActorForNudge : _selectedActor;
		var agent = planner != null ? planner.ResolveAgentFor(actor) : null;
		if (agent == null) { AppendLog("[DM] No agent for actor"); return; }
		agent.position = target.position;
		var party = GetComponent<PartyState>();
		if (party != null) party.SyncFromTransform(actor, agent);
		AppendLog($"[DM] Snapped {actor} to POI '{_selectedPoi}'");
	}

	private void CyclePoi()
	{
		_poiNames = POIRegistry.GetAllNames() ?? new List<string>();
		if (_poiNames.Count == 0)
		{
			_selectedPoi = "";
			var lblNone = _poiButton?.GetComponentInChildren<Text>();
			if (lblNone != null) lblNone.text = "POI: none";
			AppendLog("[DM] No POIs available");
			return;
		}
		_poiIndex = (_poiIndex + 1) % _poiNames.Count;
		_selectedPoi = _poiNames[_poiIndex];
		var label = _poiButton != null ? _poiButton.GetComponentInChildren<Text>() : null;
		if (label != null) label.text = "POI: " + _selectedPoi;
		AppendLog("[DM] Selected POI '" + _selectedPoi + "'");
	}

	private void CycleCandidate(int delta)
	{
		_candCount = Mathf.Max(1, _candCount);
		_candIndex = Mathf.Clamp(_candIndex + delta, 0, _candCount - 1);
		var gate = GetComponent<DiceGate>();
		gate?.SetCandidateIndex(_candIndex);
		if (_candText != null) _candText.text = "Cand " + (_candIndex + 1) + "/" + _candCount;
	}

	public void SetCandidateMeta(int count)
	{
		_candCount = Mathf.Max(1, count);
		_candIndex = Mathf.Clamp(_candIndex, 0, _candCount - 1);
		if (_candText != null) _candText.text = "Cand " + (_candIndex + 1) + "/" + _candCount;
	}

	public void ResetCandidateIndex()
	{
		_candIndex = 0;
		GetComponent<DiceGate>()?.SetCandidateIndex(_candIndex);
		if (_candText != null) _candText.text = "Cand " + (_candIndex + 1) + "/" + Mathf.Max(1, _candCount);
	}

	private void OnProposeClicked()
	{
		var text = _dmInput != null ? _dmInput.text : string.Empty;
		// Simple command grammar: "adv-1 move dx dz" or "adv-2 talk" or "adv-3 inspect"
		string actor = "adv-1";
		string action = "wait";
		float dx = 0f, dz = 0f;
		if (!string.IsNullOrEmpty(text))
		{
			var parts = text.Split(' ');
			if (parts.Length >= 2)
			{
				actor = parts[0];
				action = parts[1];
				if (action == "move")
				{
					if (parts.Length >= 4)
					{
						float.TryParse(parts[2], out dx);
						float.TryParse(parts[3], out dz);
					}
				}
			}
		}
		_lastActorForNudge = actor;

		var candidate = new CandidateAction
		{
			action = action,
			@params = action == "move"
				? new System.Collections.Generic.Dictionary<string, object> { { "destDX", dx }, { "destDZ", dz } }
				: new System.Collections.Generic.Dictionary<string, object>()
		};
		var proposal = new IntentProposal
		{
			type = "IntentProposal",
			actorId = actor,
			goal = action == "move" ? "advance" : (action == "talk" ? "communicate" : action == "inspect" ? "observe" : "wait"),
			intent = action,
			rationale = "dm-input",
			suggestedDC = null,
			candidateActions = new System.Collections.Generic.List<CandidateAction> { candidate }
		};
		var gate = GetComponent<DiceGate>();
		var planner = GetComponent<Planner>();
		if (gate != null && planner != null)
		{
		var agent = planner != null ? planner.ResolveAgentFor(actor) : null;
			gate.StageProposal(proposal, agent);
			AppendLog($"[DM] Staged: {actor} {action} {(action=="move"?$"dx={dx} dz={dz}":"")}");
		}
	}

	private void ToggleAuto()
	{
		var demo = GetComponent<LocalIntentDemo>();
		if (demo == null) demo = gameObject.AddComponent<LocalIntentDemo>();
		bool next = !demo.enabledDemo;
		demo.SetEnabled(next);
		if (_autoButton != null)
		{
			var label = _autoButton.GetComponentInChildren<Text>();
			if (label != null) label.text = next ? "Auto: ON" : "Auto: OFF";
		}
		AppendLog(next ? "[DM] Auto demo enabled" : "[DM] Auto demo disabled");
	}

	private void OnSaveResultClicked()
	{
		if (_resultInput == null || string.IsNullOrEmpty(_resultInput.text))
		{
			AppendLog("[DM] No result note entered");
			return;
		}
		// Try to infer actor from last staged/rolled proposal
		var gate = GetComponent<DiceGate>();
		var planner = GetComponent<Planner>();
		string actor = "adv-1";
		// Heuristic: parse prefix like "adv-x ..." if present in the note
		var parts = _resultInput.text.Split(' ');
		if (parts.Length > 0 && parts[0].StartsWith("adv-")) actor = parts[0];
		DMNarration.SetLastNote(actor, _resultInput.text);
		AppendLog($"[DM] Note saved for {actor}: {_resultInput.text}");
	}

	private void SendContextToGateway()
	{
		var bc = GetComponent<BrainClient>();
		if (bc == null || !bc.IsConnected)
		{
			AppendLog("[DM] Gateway not connected");
			return;
		}
		var text = _dmInput != null ? _dmInput.text : string.Empty;
		string actor = "adv-1";
		var parts = text.Split(' ');
		if (parts.Length > 0 && parts[0].StartsWith("adv-")) actor = parts[0];
		bc.SendDMContext(actor, text);
		AppendLog("[DM] Sent context to gateway for " + actor);
	}
}


