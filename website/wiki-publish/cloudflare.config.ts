import { defineConfig } from "cf/config";

export default defineConfig({
  worker: {
    name: "die-wulpischen-lande-wiki",
    compatibilityDate: "2026-10-06",
    observability: { enabled: true },
    domains: ["wiki.wulpischelande.de"],
  },
});
