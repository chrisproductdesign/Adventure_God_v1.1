#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;

[InitializeOnLoad]
public static class UnityMcpListener {
    static HttpListener listener;
    static Thread listenThread;
    static ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
    static string LOG_PATH = "Assets/Editor/McpUnityListener/log.txt";

    static UnityMcpListener() {
        Start(8081);
        EditorApplication.update += ProcessQueue;
    }

    static void Start(int port) {
        try {
            listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{port}/");
            listenThread = new Thread(() => {
                listener.Start();
                while (listener.IsListening) {
                    var ctx = listener.GetContext();
                    var req = ctx.Request;
                    var resp = ctx.Response;
                    using (var sr = new StreamReader(req.InputStream, req.ContentEncoding)) {
                        var body = sr.ReadToEnd();
                        queue.Enqueue(body);
                        File.AppendAllText(LOG_PATH, System.DateTime.Now.ToString("s") + " RECEIVED: " + body + "\n");
                    }
                    var respBytes = Encoding.UTF8.GetBytes("ok");
                    resp.OutputStream.Write(respBytes, 0, respBytes.Length);
                    resp.Close();
                }
            });
            listenThread.IsBackground = true;
            listenThread.Start();
            File.AppendAllText(LOG_PATH, System.DateTime.Now.ToString("s") + " Listener started\n");
        } catch (System.Exception e) {
            Debug.LogError("MCP listener start error: " + e);
            File.AppendAllText(LOG_PATH, System.DateTime.Now.ToString("s") + " Listener error: " + e + "\n");
        }
    }

    static void ProcessQueue() {
        while (queue.TryDequeue(out var body)) {
            try {
                var cmd = JsonUtility.FromJson<SimpleCmd>(body);
                if (cmd == null) continue;
                if (cmd.tool == "createGameObject") {
                    var name = cmd.input != null && cmd.input.name != null ? cmd.input.name : "NewObject";
                    var go = new GameObject(name);
                    Selection.activeGameObject = go;
                    Debug.Log($"MCP: created {name}");
                    File.AppendAllText(LOG_PATH, System.DateTime.Now.ToString("s") + $" Created {name}\n");
                } else if (cmd.tool == "runPlayMode") {
                    if (cmd.input != null && cmd.input.action == "start") EditorApplication.isPlaying = true;
                    if (cmd.input != null && cmd.input.action == "stop") EditorApplication.isPlaying = false;
                    Debug.Log("MCP: playmode toggled");
                } else if (cmd.tool == "takeScreenshot") {
                    var path = (cmd.input != null && !string.IsNullOrEmpty(cmd.input.path)) ? cmd.input.path : "Assets/Editor/mcp_screenshot.png";
                    ScreenCapture.CaptureScreenshot(path);
                    Debug.Log($"MCP: screenshot saved {path}");
                } else if (cmd.tool == "buildProject") {
                    Debug.Log("MCP: buildProject requested (prefer Unity CLI -executeMethod for headless builds).");
                } else {
                    Debug.Log($"MCP: unknown tool {cmd.tool}");
                }
            } catch (System.Exception e) {
                Debug.LogError("MCP command error: " + e);
                File.AppendAllText(LOG_PATH, System.DateTime.Now.ToString("s") + " Cmd error: " + e + "\n");
            }
        }
    }

    [System.Serializable]
    class SimpleCmd {
        public string tool;
        public Input input;
        [System.Serializable]
        public class Input {
            public string name;
            public string action;
            public string path;
        }
    }
}
#endif
