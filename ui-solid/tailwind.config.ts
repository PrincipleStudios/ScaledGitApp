import type { Config } from 'tailwindcss';

const config: Config = {
	content: ['./index.html', './src/**/*.{js,ts,jsx,tsx,css}'],
	darkMode: 'class',
	theme: {
		extend: {},
		zIndex: {
			normal: '0',
			dropdown: '5',
			modalBackground: '10',
			modalForeground: '20',
		},
	},
	plugins: [],
};

export default config;
