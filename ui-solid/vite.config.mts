import { defineConfig } from 'vite';
import solidPlugin from 'vite-plugin-solid';
import { fileURLToPath } from 'node:url';

export default defineConfig({
	plugins: [solidPlugin()],
	build: {
		target: 'esnext',
	},
	resolve: {
		alias: {
			'@': fileURLToPath(await import.meta.resolve('./src')),
		},
	},
});
