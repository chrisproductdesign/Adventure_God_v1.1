/* Node MCP shim — localhost-only */
const express = require('express');
const bodyParser = require('body-parser');
const fetch = require('node-fetch');
const path = require('path');
const fs = require('fs');

const app = express();
app.use(bodyParser.json());

const UNITY_LISTENER = process.env.UNITY_LISTENER || 'http://localhost:8081/';
const PORT = process.env.PORT || 3000;
const LOG_DIR = path.join(__dirname, 'logs');
if (!fs.existsSync(LOG_DIR)) fs.mkdirSync(LOG_DIR, { recursive: true });

function log(msg) {
  const line = `[${new Date().toISOString()}] ${msg}\n`;
  fs.appendFileSync(path.join(LOG_DIR, 'server.log'), line);
  console.log(msg);
}

let tools = [];
try { tools = require('./mcp-tools.json'); } catch(e){ log('Error loading mcp-tools.json: ' + e); tools = []; }

app.get('/health', (_req, res) => res.json({ status: 'ok', timestamp: new Date().toISOString() }));

app.get('/mcp-tools', (_req, res) => res.json({ tools }));

app.post('/invoke', async (req, res) => {
  const { tool, input } = req.body;
  log(`Invoking ${tool} with ${JSON.stringify(input)}`);
  
  try {
    const resp = await fetch(UNITY_LISTENER + 'command', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ tool, input })
    });
    const result = await resp.text();
    log(`Unity response: ${result}`);
    res.json({ success: true, result });
  } catch (e) {
    log(`Error: ${e.message}`);
    res.status(500).json({ success: false, error: e.message });
  }
});

app.listen(PORT, 'localhost', () => {
  log(`MCP shim listening on http://localhost:${PORT}`);
});
