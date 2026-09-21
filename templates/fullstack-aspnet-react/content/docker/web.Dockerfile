# Development image for the Vite dev server (hot reload, source mounted by
# docker-compose). This is deliberately not a production image: a production
# build would compile with 'npm run build' and serve the static output.
FROM node:{{NodeImageTag}}
WORKDIR /app

COPY src/Frontend/web/package.json src/Frontend/web/package-lock.json* ./
RUN npm install

COPY src/Frontend/web/ ./

EXPOSE {{WebDevPort}}
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]
