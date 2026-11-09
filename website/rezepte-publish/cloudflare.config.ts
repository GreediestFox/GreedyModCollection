import { defineConfig } from "cf/config";

export default defineConfig({
  worker: {
    name: "die-wulpischen-lande-rezepte",
    compatibilityDate: "2026-10-06",
    observability: { enabled: true },
    domains: ["rezepte.wulpischelande.de"],
  },
});
