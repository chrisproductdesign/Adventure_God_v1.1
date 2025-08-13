// Placeholder dice helpers for future use by LLM selector
export function rollD20(): number {
  return Math.floor(Math.random() * 20) + 1;
}

export function suggestedDC(base: number): number {
  // Keep stable for now; hook for future scaling logic
  return base;
}


