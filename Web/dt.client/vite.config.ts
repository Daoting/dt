import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
  build: {
    lib: {
      entry: 'src/index.ts',
      name: 'Dt.Client',
      fileName: (format) => `dt.client.${format}.js`
    },
    rollupOptions: {
      external: ['vue'],
      output: {
        globals: { vue: 'Vue' }
      }
    },
    sourcemap: true
  },
  server: {
    // 开启 CORS，默认允许所有源（*）
    cors: true
  }
})