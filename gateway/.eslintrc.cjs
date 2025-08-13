module.exports = {
  root: true,
  env: { node: true, es2022: true },
  parser: "@typescript-eslint/parser",
  parserOptions: { ecmaVersion: "latest", sourceType: "module", project: false },
  plugins: ["@typescript-eslint", "import", "prettier"],
  extends: [
    "eslint:recommended",
    "plugin:@typescript-eslint/recommended",
    "plugin:import/recommended",
    "plugin:prettier/recommended"
  ],
  settings: {
    "import/resolver": { typescript: true }
  },
  rules: {
    "prettier/prettier": "warn",
    "import/no-unresolved": "off"
  },
  ignorePatterns: ["dist/**", "node_modules/**"]
};


