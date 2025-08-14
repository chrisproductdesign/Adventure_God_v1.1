// Flat ESLint config (ESLint v9)
// Minimal parser-only setup to keep linting lightweight for TypeScript sources.
// We can layer rules/plugins later as needed.
const tsParser = require('@typescript-eslint/parser');

module.exports = [
  {
    files: ['**/*.ts', '**/*.tsx'],
    ignores: ['node_modules/**', 'dist/**'],
    languageOptions: {
      parser: tsParser,
      parserOptions: {
        ecmaVersion: 'latest',
        sourceType: 'module',
      },
    },
    rules: {},
  },
];


