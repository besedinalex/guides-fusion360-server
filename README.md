# Beginner's guides to Fusion 360

Web server for keeping guides data and making possible creating and editing them.

### Setup

Install Node.js from [official site](https://nodejs.org/en/download).

Make sure that your CLI now handles `node` and `npm` commands.

### Debug

Run `npm start` to start dev server (auto-compile TypeScript files on change).

Run `npm test` to run tests.

### Deploy

Run `npm i pkg -g` to be able to package entire node project into executable file.

Install Python from [official site](https://www.python.org/downloads).

Make sure that your CLI now handles `python3`.

Server project folder should be named `guides-fusion360-server`. Client project folder should be named `guides-fusion360-client`. Both folders should be in the same folder. All of this should be default names when you do `git clone` so I'm mentioning it just in case you renamed any of them.

Run `python3 build.py` to build entire project. Files you need will be placed in `/guides-fusion360-server/build/` folder.

Build options:

`--npm_install=true` also runs `npm install` (it doesn't by default) if you just cloned projects and wanna build it asap.

`--os=win` lets you choose targeted OS (Windows by default). Also supports `macos` and `linux`.

`--arch=x64` lets you choose targeted processor architecture (x64 by default). Also supports `x86`.
