import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

const apiProxy = {
  target: 'http://localhost:5023',
  changeOrigin: true,
}

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    port: 5173,
    strictPort: false,
    proxy: {
      '/api': apiProxy,
      '/register': apiProxy,
      '/login': apiProxy,
      '/logout': apiProxy,
      '/manage': apiProxy,
      '/refresh': apiProxy,
    },
  },
})
