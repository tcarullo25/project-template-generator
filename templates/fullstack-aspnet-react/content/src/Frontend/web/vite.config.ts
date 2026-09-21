import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';

const apiPort = process.env.API_PORT ?? '{{ApiHttpPort}}';
const apiHost = process.env.API_HOST ?? 'localhost';

export default defineConfig({
  plugins: [react()],
  server: {
    port: Number(process.env.WEB_PORT ?? {{WebDevPort}}),
    // Bind on all interfaces so the container-hosted dev server is reachable.
    host: true,
    // With VITE_API_BASE_URL empty, the app calls /api on its own origin and
    // this proxy forwards to the backend. No CORS involved during development.
    proxy: {
      '/api': {
        target: `http://${apiHost}:${apiPort}`,
        changeOrigin: true,
      },
    },
  },
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./src/test/setup.ts'],
  },
});
