import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import { MAIN_SITE, DISCORD, SERVER_NAME } from './src/config.ts';

export default defineConfig({
  site: 'https://wiki.wulpischelande.de',
  integrations: [
    starlight({
      title: SERVER_NAME + ' Wiki',
      description: 'Installationsanleitung, Serverregeln und Lore der wulpischen Lande (Life is Feudal: Your Own).',
      customCss: ['./src/styles/fonts.css', './src/styles/theme.css'],
      social: [
        { icon: 'discord', label: 'Discord', href: DISCORD },
        { icon: 'external', label: 'Hauptseite', href: MAIN_SITE },
      ],
      components: {
        ThemeProvider: './src/components/ThemeProvider.astro',
        ThemeSelect: './src/components/Empty.astro',
        Footer: './src/components/Footer.astro',
      },
      // German is the main language (pages in src/content/docs/), English pages live in src/content/docs/en/ with the same file names
      defaultLocale: 'root',
      locales: {
        root: { label: 'Deutsch', lang: 'de' },
        en: { label: 'English', lang: 'en' },
      },
      lastUpdated: false,
      pagination: true,
      sidebar: [
        { slug: 'install', label: 'Installationsanleitung', translations: { en: 'Install guide' } },
        { slug: 'concept', label: 'Serverkonzept', translations: { en: 'Server concept' } },
        { slug: 'rules', label: 'Serverregeln', translations: { en: 'Server rules' } },
        { slug: 'lore', label: 'Server-Lore', translations: { en: 'Server lore' } },
      ],
    }),
  ],
});
