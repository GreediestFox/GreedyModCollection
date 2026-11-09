import { defineWranglerConfig } from "wrangler/experimental-config";

export default defineWranglerConfig({
  assetsDirectory: "public",   // the placeholder page (later: the real recipe site)
});
