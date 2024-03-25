import { fileURLToPath } from 'node:url';
import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';
import { VitePWA } from 'vite-plugin-pwa';

// https://vitejs.dev/config/
export default defineConfig({
	plugins: [
		react(),

		VitePWA({
			registerType: 'autoUpdate',
			strategies: 'injectManifest',
			srcDir: 'src',
			filename: 'service-worker/implementation.ts',
			manifest: {
				name: 'scaledgitapp',
				short_name: 'scaledgitapp',
			},
			devOptions: {
				enabled: true,
				type: 'module',
			},
		}),
	],
	build: {
		outDir: '../Server/wwwroot',
		emptyOutDir: true,
		assetsInlineLimit: 0,
	},
	resolve: {
		alias: {
			'@': fileURLToPath(await import.meta.resolve('./src')),
		},
	},
	server: {
		hmr: {
			path: '/.vite/hmr',
		},
	},
	test: {
		globals: true,
		environment: 'jsdom',
		setupFiles: ['./vitest.setup.mts'],
	},
});
