/** @type {import('eslint').Linter.Config} */
module.exports = {
	overrides: [
		{
			files: ['**/*.{ts,tsx}'],
			parserOptions: {
				ecmaVersion: 'latest',
				sourceType: 'module',
				project: './tsconfig.node.json',
				tsconfigRootDir: __dirname,
			},
		},
		{
			files: ['src/**/*.{ts,tsx}'],
			extends: [
				// React-recommended, followed by tuning off needing to `import React from "react"`
				'plugin:react/recommended',
				'plugin:react/jsx-runtime',
				'plugin:react-hooks/recommended',
				'plugin:storybook/recommended',
			],
			parserOptions: {
				project: './tsconfig.json',
				tsconfigRootDir: __dirname,
			},
			rules: {
				'no-restricted-imports': [
					'error',
					{
						paths: [
							{
								name: 'react',
								importNames: ['default'],
								message: 'React import is not required',
							},
							{
								name: 'lodash',
								message:
									"Do not import entire lodash package; import 'lodash/<function>' instead.",
							},
						],
					},
				],
			},
		},
	],
	settings: {
		react: {
			version: 'detect',
		},
	},
};
