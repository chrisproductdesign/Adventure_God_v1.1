using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

/// <summary>
/// Provides Cursor with Unity Editor integration capabilities
/// This script enables better workflow between Cursor and Unity
/// </summary>
public static class CursorIntegration
{
    [MenuItem("Cursor/Export Scene Info")]
    public static void ExportSceneInfo()
    {
        var sceneInfo = new StringBuilder();
        sceneInfo.AppendLine("# Unity Scene Information");
        sceneInfo.AppendLine($"Scene: {UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name}");
        sceneInfo.AppendLine($"Path: {UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path}");
        sceneInfo.AppendLine();
        
        // Find all GameObjects with components
        var allObjects = Object.FindObjectsOfType<GameObject>();
        sceneInfo.AppendLine("## GameObjects in Scene");
        foreach (var obj in allObjects)
        {
            sceneInfo.AppendLine($"- {obj.name} (at {obj.transform.position})");
            var components = obj.GetComponents<Component>();
            foreach (var comp in components)
            {
                if (comp != null)
                {
                    sceneInfo.AppendLine($"  - {comp.GetType().Name}");
                }
            }
        }
        
        // Save to file for Cursor to read
        var path = Path.Combine(Application.dataPath, "..", "cursor_scene_info.md");
        File.WriteAllText(path, sceneInfo.ToString());
        Debug.Log($"Scene info exported to: {path}");
    }
    
    [MenuItem("Cursor/Validate Project Setup")]
    public static void ValidateProjectSetup()
    {
        var issues = new StringBuilder();
        issues.AppendLine("# Project Validation Report");
        
        // Check for required components
        var aiHost = Object.FindObjectOfType<Planner>();
        if (aiHost == null)
        {
            issues.AppendLine("❌ Missing AIHost with Planner component");
        }
        else
        {
            issues.AppendLine("✅ AIHost with Planner found");
        }
        
        // Check for agents
        var agents = GameObject.FindObjectsOfType<AgentTag>();
        if (agents.Length < 3)
        {
            issues.AppendLine($"❌ Expected 3 agents, found {agents.Length}");
        }
        else
        {
            issues.AppendLine("✅ All 3 agents found");
        }
        
        // Check for gateway connection
        var brainClient = Object.FindObjectOfType<BrainClient>();
        if (brainClient == null)
        {
            issues.AppendLine("❌ Missing BrainClient component");
        }
        else
        {
            issues.AppendLine("✅ BrainClient found");
        }
        
        // Check for UI components
        var diceGate = Object.FindObjectOfType<DiceGate>();
        if (diceGate == null)
        {
            issues.AppendLine("❌ Missing DiceGate component");
        }
        else
        {
            issues.AppendLine("✅ DiceGate found");
        }
        
        // Save validation report
        var path = Path.Combine(Application.dataPath, "..", "cursor_validation_report.md");
        File.WriteAllText(path, issues.ToString());
        Debug.Log($"Validation report saved to: {path}");
        
        // Show in console
        Debug.Log(issues.ToString());
    }
    
    [MenuItem("Cursor/Setup Test Scene")]
    public static void SetupTestScene()
    {
        // Create a clean test scene
        UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
        
        // Add AIHost with all required components
        var host = new GameObject("AIHost");
        host.AddComponent<Planner>();
        host.AddComponent<BrainClient>();
        host.AddComponent<DiceGate>();
        host.AddComponent<DMHud>();
        host.AddComponent<PartyState>();
        host.AddComponent<PartyHUD>();
        host.AddComponent<SaveLoadManager>();
        host.AddComponent<SaveLoadUI>();
        host.AddComponent<SceneLoader>();
        
        // Create agents
        var agent1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        agent1.name = "Agent-1";
        agent1.transform.position = Vector3.zero;
        agent1.AddComponent<AgentHighlighter>();
        agent1.AddComponent<AgentTag>().actorId = "adv-1";
        
        var agent2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        agent2.name = "Agent-2";
        agent2.transform.position = new Vector3(2, 0, 0);
        agent2.AddComponent<AgentHighlighter>();
        agent2.AddComponent<AgentTag>().actorId = "adv-2";
        
        var agent3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        agent3.name = "Agent-3";
        agent3.transform.position = new Vector3(-2, 0, 0);
        agent3.AddComponent<AgentHighlighter>();
        agent3.AddComponent<AgentTag>().actorId = "adv-3";
        
        // Setup camera
        var cam = Camera.main;
        if (cam != null)
        {
            var tdc = cam.gameObject.AddComponent<TopDownCamera>();
            tdc.target = agent1.transform;
        }
        
        // Add labels
        var lab1 = host.AddComponent<AgentLabel>();
        lab1.target = agent1.transform;
        var lab2 = host.AddComponent<AgentLabel>();
        lab2.target = agent2.transform;
        var lab3 = host.AddComponent<AgentLabel>();
        lab3.target = agent3.transform;
        
        Debug.Log("Test scene setup complete");
    }
    
    [MenuItem("Cursor/Export Component Documentation")]
    public static void ExportComponentDocumentation()
    {
        var doc = new StringBuilder();
        doc.AppendLine("# Unity Component Documentation");
        doc.AppendLine();
        
        // Document all custom components
        var components = new System.Type[]
        {
            typeof(Planner),
            typeof(BrainClient),
            typeof(DiceGate),
            typeof(ActionExecutor),
            typeof(PartyState),
            typeof(DMNarration),
            typeof(AgentHighlighter),
            typeof(AgentTag),
            typeof(TopDownCamera),
            typeof(AgentLabel),
            typeof(SceneLoader),
            typeof(POIRegistry),
            typeof(SaveLoadManager),
            typeof(SaveLoadUI),
            typeof(PartyHUD),
            typeof(DMHud),
            typeof(OutcomeReporter),
            typeof(LocalIntentDemo)
        };
        
        foreach (var componentType in components)
        {
            doc.AppendLine($"## {componentType.Name}");
            doc.AppendLine();
            
            // Get public fields and properties
            var fields = componentType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var properties = componentType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            
            if (fields.Length > 0)
            {
                doc.AppendLine("### Public Fields");
                foreach (var field in fields)
                {
                    doc.AppendLine($"- `{field.Name}`: {field.FieldType.Name}");
                }
                doc.AppendLine();
            }
            
            if (properties.Length > 0)
            {
                doc.AppendLine("### Public Properties");
                foreach (var prop in properties)
                {
                    doc.AppendLine($"- `{prop.Name}`: {prop.PropertyType.Name}");
                }
                doc.AppendLine();
            }
        }
        
        var path = Path.Combine(Application.dataPath, "..", "cursor_component_docs.md");
        File.WriteAllText(path, doc.ToString());
        Debug.Log($"Component documentation exported to: {path}");
    }
}
