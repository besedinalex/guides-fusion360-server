# Beginner's guides to Fusion 360

Web server for keeping guides data and making possible creating and editing them with simple http requests.

## Basic stuff

The server is made with [Node.js](https://nodejs.org) using [TypeScript](https://www.typescriptlang.org).

### Setup

Install Node.js from their [site](https://nodejs.org/en/download).

Run `npm i typescript -g` to get TypeScript.

Make sure that your CLI now handles `node`, `npm`, `tsc` commands.

### Debug

Run `npm run dev` to start dev server (auto-compile TypeScript files on change).

### Deploy

Run `npm run build` to get JavaScript files.

Remove or rename `src/` folder and rename built `dist/` to `src/`.

Run `npm run start` to start server from JavaScript files.
