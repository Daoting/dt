import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import { quasar, transformAssetUrls } from "@quasar/vite-plugin";
import path from "path";

export default defineConfig({
  plugins: 
  [
    vue({ template: { transformAssetUrls } }), 
    quasar()
  ],
  resolve: {
    alias: {
      /* import from时的路径简写 */
      "@": path.resolve(__dirname, "./src"),
    },
  },
  build: {
    sourcemap: true,
  },
});
