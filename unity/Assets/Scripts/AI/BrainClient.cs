using UnityEngine;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

public class BrainClient : MonoBehaviour
{
    [SerializeField] private string serverUrl = "ws://127.0.0.1:8787";

    private ClientWebSocket _ws;
    private CancellationTokenSource _cts;
    private readonly string[] _actors = new[] { "adv-1", "adv-2", "adv-3" };

    async void Start()
    {
        _cts = new CancellationTokenSource();
        _ws  = new ClientWebSocket();
        try
        {
            await _ws.ConnectAsync(new Uri(serverUrl), _cts.Token);
            Debug.Log("BrainClient connected");
            _ = Task.Run(ReceiveLoop);
            _ = Task.Run(SendHeartbeat);
        }
        catch (Exception e) { Debug.LogError(e.Message); }
    }

    public bool IsConnected => _ws != null && _ws.State == WebSocketState.Open;

    public async void SendDMContext(string actorId, string dmText)
    {
        if (!IsConnected) return;
        try
        {
            string safeText = string.IsNullOrEmpty(dmText) ? "" : dmText.Replace("\"", "'");
            string json = "{\"type\":\"PerceptionEvent\",\"actorId\":\"" + actorId + "\"" +
                           (string.IsNullOrEmpty(safeText) ? "" : ",\"observations\":[{\"kind\":\"info\",\"id\":\"dm:" + safeText + "\"}]") +
                           "}";
            var buf  = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
            await _ws.SendAsync(buf, WebSocketMessageType.Text, true, _cts.Token);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private async Task SendHeartbeat()
    {
        try
        {
            while (_ws.State == WebSocketState.Open && !_cts.Token.IsCancellationRequested)
            {
                foreach (var actor in _actors)
                {
                    var outcome = OutcomeReporter.GetLastOutcome(actor);
                    string obs = string.IsNullOrEmpty(outcome)
                        ? ""
                        : ",\"observations\":[{\"kind\":\"info\",\"id\":\"outcome:" + outcome + "\"}]";
                    var json = "{\"type\":\"PerceptionEvent\",\"actorId\":\"" + actor + "\"" + obs + "}";
                    var buf  = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
                    await _ws.SendAsync(buf, WebSocketMessageType.Text, true, _cts.Token);
                }
                await Task.Delay(2000, _cts.Token);
            }
        }
        catch (OperationCanceledException) {}
        catch (Exception e) { Debug.LogError(e.Message); }
    }

    private async Task ReceiveLoop()
    {
        var buffer = new byte[8192];
        try
        {
            while (_ws.State == WebSocketState.Open && !_cts.Token.IsCancellationRequested)
            {
                var seg = new ArraySegment<byte>(buffer);
                var sb  = new StringBuilder();
                WebSocketReceiveResult res;
                do
                {
                    res = await _ws.ReceiveAsync(seg, _cts.Token);
                    if (res.MessageType == WebSocketMessageType.Close)
                    {
                        await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", _cts.Token);
                        return;
                    }
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, res.Count));
                } while (!res.EndOfMessage);

                Debug.Log($"[Gateway] {sb}");
                Planner.TryConsume(sb.ToString()); // hand off to planner
            }
        }
        catch (OperationCanceledException) {}
        catch (Exception e) { Debug.LogError(e.Message); }
    }

    async void OnApplicationQuit() { await SafeClose(); }
    async void OnDestroy()         { await SafeClose(); }

    private async Task SafeClose()
    {
        try
        {
            _cts?.Cancel();
            if (_ws != null && (_ws.State == WebSocketState.Open || _ws.State == WebSocketState.CloseReceived))
                await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "destroy", CancellationToken.None);
        }
        catch {}
        finally
        {
            _ws?.Dispose();
            _cts?.Dispose();
        }
    }
}
