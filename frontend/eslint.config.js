import js from '@eslint/js'
import pluginVue from 'eslint-plugin-vue'
import skipFormatting from '@vue/eslint-config-prettier/skip-formatting'

export default [
  {
    name: 'app/files-to-lint',
    files: ['**/*.{js,mjs,jsx,vue}'],
  },

  {
    name: 'app/files-to-ignore',
    ignores: ['**/dist/**', '**/dist-ssr/**', '**/coverage/**', '**/cypress/**'],
  },

  js.configs.recommended,
  ...pluginVue.configs['flat/essential'],
  skipFormatting,
  {
    name: 'Cypress',
    files: ['**/cypress/**/*.js'],
    env: {
      'cypress/globals': true, // Enables Cypress global variables like cy
    },
    extends: [
      'plugin:cypress/recommended'
    ],
    rules: {
      // You can specify rules specific to Cypress test files here
    }
  },
]
