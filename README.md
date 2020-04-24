# Beginner's guides to Fusion 360

Web server for keeping guides data and making possible creating and editing them with simple http requests.

### Setup

Install Node.js from their [site](https://nodejs.org/en/download).

Make sure that your CLI now handles `node` and `npm` commands.

Create folder `data/` in project root folder.

### Debug

Run `npm run dev` to start dev server (auto-compile TypeScript files on change).

Run `npm run test` to run tests.

### Deploy

Run `npm run build` to get JavaScript files.

Remove or rename `src/` folder and rename built `dist/` to `src/`.

Run `npm run start` to start server from JavaScript files.
