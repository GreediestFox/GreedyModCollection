import { defineConfig } from "cf/config";

export default defineConfig({
  worker: {
    name: "die-wulpischen-lande",
    compatibilityDate: "2026-10-06",
    observability: { enabled: true },
    domains: ["wulpischelande.de", "www.wulpischelande.de"],
  },
});
