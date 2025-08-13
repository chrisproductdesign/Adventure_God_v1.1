import { WebSocketServer, WebSocket, RawData } from "ws";
import { PerceptionEvent, IntentProposalStrict, PerceptionEventT } from "./schema/events";
import { selectIntent } from "./ai/selectIntent";

const wss = new WebSocketServer({ port: 8787 });
console.log("AI Gateway listening on ws://127.0.0.1:8787");

wss.on("connection", (socket: WebSocket) => {
  socket.on("message", (msg: RawData) => {
    try {
      const parsed: unknown = JSON.parse(msg.toString());

      if (typeof parsed === "object" && parsed !== null && (parsed as any).type === "PerceptionEvent") {
        const evt = PerceptionEvent.parse(parsed) as PerceptionEventT;
        const rawProposal = selectIntent(evt);
        const reply = IntentProposalStrict.parse(rawProposal);
        socket.send(JSON.stringify(reply));
        return;
      }
      socket.send(
        JSON.stringify({ type: "Error", message: "Only PerceptionEvent is accepted at this endpoint" })
      );
    } catch (err) {
      const message = err instanceof Error ? err.message : "bad input";
      socket.send(JSON.stringify({ type: "Error", message }));
    }
  });
});
