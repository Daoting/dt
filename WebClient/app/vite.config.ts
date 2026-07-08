import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { resolve } from 'path'

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      /* 让 Vite 识别 workspace 包并支持热更新 */
      'dt.client': resolve(__dirname, '../pkg/src/index.ts')
    }
  },
  build: {
    sourcemap: true
  }
})
