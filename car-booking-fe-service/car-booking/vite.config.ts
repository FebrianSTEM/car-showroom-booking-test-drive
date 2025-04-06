import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react-swc'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
    server: {
      proxy: {
        '/auth': {
          target: 'https://localhost:7125',
          changeOrigin: true,
          rewrite: (path) => path.replace(/^\/auth/, '/api'),
          secure: false,
        },
        '/carBooking': {
          target: 'https://localhost:7126',
          changeOrigin: true,
          rewrite: (path) => path.replace(/^\/carBooking/, '/api'),
          secure: false,
        },
      },
    }
})