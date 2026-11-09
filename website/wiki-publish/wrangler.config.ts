import { defineWranglerConfig } from "wrangler/experimental-config";

export default defineWranglerConfig({
  assetsDirectory: "public",   // the built wiki (copied here from wiki\dist by Wiki-veroeffentlichen.bat)
});
