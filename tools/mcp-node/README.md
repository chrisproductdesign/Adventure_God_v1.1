MCP Node shim for local Cursor ↔ Unity prototyping.

Run:
  cd tools/mcp-node
  npm ci
  npm start

Smoke test:
  npm run test:smoke

Notes:
- Localhost only (http://localhost:3000).
- Forwards commands to Unity listener at http://localhost:8081/command.
- Logs in tools/mcp-node/logs/.
- Do NOT expose this to public networks.
