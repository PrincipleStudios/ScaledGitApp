import { initReactI18next } from 'react-i18next';
import i18nextBase from 'i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import HttpApi, { type HttpBackendOptions } from 'i18next-http-backend';
import MultiloadAdapter, {
	type MultiloadBackendOptions,
} from 'i18next-multiload-backend-adapter';
import { constructUrl as toLocalizationUrl } from '../../generated/api/operations/getTranslationData';

const reportedKeys = new Set<string>();

export const i18n = i18nextBase
	.use(MultiloadAdapter)
	.use(LanguageDetector)
	.use(initReactI18next);
void i18n.init({
	// lng: 'en', // if you're using a language detector, do not define the lng option
	fallbackLng: 'en',

	saveMissing: true,
	missingKeyHandler(languages, namespace, key) {
		const fullKeys = languages
			.map((l) => `${l}::${namespace}::${key}`)
			.filter((k) => !reportedKeys.has(k));
		if (!fullKeys.length) return;
		for (const k of fullKeys) {
			reportedKeys.add(k);
		}
		console.warn('missing translations:', fullKeys);
	},

	backend: {
		backend: HttpApi,
		backendOption: {
			loadPath: (languages, namespaces) =>
				toLocalizationUrl({
					lng: languages.join(' '),
					ns: namespaces.join(' '),
				}),
		} satisfies HttpBackendOptions,
	} satisfies MultiloadBackendOptions,

	interpolation: {
		escapeValue: false, // react already safes from xss => https://www.i18next.com/translation-function/interpolation#unescape
	},
});
