import { initReactI18next } from 'react-i18next';
import i18nextBase from 'i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import HttpApi from 'i18next-http-backend';

const reportedKeys = new Set<string>();

export const i18n = i18nextBase
	.use(HttpApi)
	.use(LanguageDetector)
	.use(initReactI18next);
void i18n.init({
	// lng: 'en', // if you're using a language detector, do not define the lng option
	fallbackLng: 'en',

	backend: {
		loadPath: '/src/i18n/{{ns}}/{{lng}}.json',
	},

	interpolation: {
		escapeValue: false, // react already safes from xss => https://www.i18next.com/translation-function/interpolation#unescape
	},
});
