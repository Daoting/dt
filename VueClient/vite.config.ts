import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import { quasar, transformAssetUrls } from "@quasar/vite-plugin";
import path from "path";
import AutoImport from "unplugin-auto-import/vite";
import { VitePWA } from "vite-plugin-pwa";

export default defineConfig({
  plugins: [
    vue({ template: { transformAssetUrls } }),
    AutoImport({
      imports: [
        // 自动导入以下库的 API：ref, computed...
        "vue",
        { zod: [["default", "z"]] },
        { quasar: ["useQuasar"] },
        {
          "@/core": [
            ["kit", "kit"],
            ["Table", "Table"],
            ["Row", "Row"],
            ["Dict", "Dict"],
            ["SaveItem", "SaveItem"],
            ["at", "at"],
          ],
        },
      ],
      dts: path.resolve(__dirname, "./src/types/auto-imports.d.ts"),
    }),
    quasar(),
    VitePWA({
      registerType: "autoUpdate",
      devOptions: {
        enabled: true,
      },
      includeAssets: ["favicon.svg", "apple-touch-icon.png"],
      manifest: {
        name: "搬运工",
        short_name: "搬运工",
        description: "搬运工",
        theme_color: "#1BA1E2",
        background_color: "#ffffff",
        display: "standalone",
        start_url: "./",
        scope: "./",
        icons: [
          {
            src: "pwa-192.png",
            sizes: "192x192",
            type: "image/png",
            purpose: "any maskable",
          },
          {
            src: "pwa-512.png",
            sizes: "512x512",
            type: "image/png",
            purpose: "any maskable",
          },
        ],
      },
      workbox: {
        globPatterns: ["**/*.{js,css,html,svg,woff,woff2,json}"] ,
        navigateFallback: "index.html",
        cleanupOutdatedCaches: true,
      },
    }),
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
  base: './', // 相对路径
});
