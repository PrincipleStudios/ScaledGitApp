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
			parserOptions: {
				project: './tsconfig.json',
				tsconfigRootDir: __dirname,
			},
		},
	],
};
