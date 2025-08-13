import WebSocket from "ws";

const url = "ws://127.0.0.1:8787";

async function run() {
  const ws = new WebSocket(url);

  ws.on("open", () => {
    console.log("Connected to", url);
    let tick = 0;
    const actorId = "adv-1";
    const interval = setInterval(() => {
      tick++;
      const event = {
        type: "PerceptionEvent",
        actorId,
        tick,
        observations: tick % 3 === 0 ? [{ kind: "enemy", id: "gob-1", distance: 1.5 }] : [],
        world: { time: "noon" }
      } as const;
      ws.send(JSON.stringify(event));
      if (tick >= 5) {
        clearInterval(interval);
        setTimeout(() => ws.close(), 500);
      }
    }, 2000);
  });

  ws.on("message", (data) => {
    try {
      const text = data.toString();
      console.log("[reply]", text);
    } catch {}
  });

  ws.on("error", (err) => {
    console.error("WS error:", err.message);
  });
}

run().catch((e) => {
  console.error(e);
  process.exit(1);
});


