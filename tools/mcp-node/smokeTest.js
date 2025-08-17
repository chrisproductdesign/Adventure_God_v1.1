const fetch = require('node-fetch');
const util = require('util');

const BASE = process.env.MCP_BASE || 'http://localhost:3000';
(async () => {
  console.log('Smoke against', BASE);
  const h = await (await fetch(`${BASE}/health`)).json().catch(()=>null);
  console.log('health:', h);
  const t = await (await fetch(`${BASE}/mcp-tools`)).json().catch(()=>null);
  console.log('tools:', t && t.tools ? t.tools.length : 'none');

  try {
    const resp = await fetch(`${BASE}/invoke`, {
      method: 'POST',
      headers: {'content-type':'application/json'},
      body: JSON.stringify({ tool: 'createGameObject', input: { name: 'Cursor_SmokeTest_Object' } })
    });
    const j = await resp.json().catch(()=>null);
    console.log('invoke(createGameObject):', util.inspect(j, {depth:2}));
  } catch(e) {
    console.log('invoke error:', e.message);
  }
})();
