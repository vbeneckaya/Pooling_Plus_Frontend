import i18n from 'i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import { initReactI18next } from 'react-i18next';
import XHR from 'i18next-xhr-backend';

i18n.use(LanguageDetector)
    .use(XHR)
    .use(initReactI18next)
    .init({
        // we init with resources
        fallbackLng: 'en',
        debug: false,

        // have a common namespace used around the full app
        ns: ['translations'],
        defaultNS: 'translations',
        backend: {
            loadPath: './translations/search',
        },
        keySeparator: false, // we use content as keys
        react: {
            wait: true,
        },
    });

export default i18n;
