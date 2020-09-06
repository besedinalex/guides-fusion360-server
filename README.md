# Beginner's guides to Fusion 360

![Build](https://github.com/besedinalex/guides-fusion360-server/workflows/Build/badge.svg)

Web server to keep guides and their files and make possible to create new and edit existing ones.

## Features

All features below can be tested with [Postman](https://www.postman.com/) by using premade [collection and enviroment](https://github.com/besedinalex/guides-fusion360-server/tree/master/GuidesFusion360Server.Postman).

- GET, POST, PUT, DELETE requests via .NET Core.
- SQLite database via Entity Framework.
- JWT authentication.
- File storage in `~/Fusion360GuideStorage`.
- External 3D-model converter via HTTPClient.
- Responses are based on user access.
- All this features are used by [React app](https://github.com/besedinalex/guides-fusion360-client).

## Setup

Install .NET Core from [official site](https://dotnet.microsoft.com/download).

Make sure that your CLI now handles `dotnet` command.

Run `dotnet restore` to install dependencies.

## Debug

Run `dotnet run` or `dotnet watch run`.

## Deploy

Proper way to deploy can be found in [official Microsoft documentation](https://docs.microsoft.com/en-us/dotnet/core/deploying/)  or [another their documentation](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/?view=aspnetcore-3.1).

### Easy way:

1. Install Python 3 from [official site](https://www.python.org/downloads/).
2. Make sure your CLI now handles `python` or `python3` commands.
3. Client project folder should be named `/guides-fusion360-client` and placed in the same folder where guides-fusion360-server is. Read it's [README.md](https://github.com/besedinalex/guides-fusion360-client/blob/master/README.md) to see if you need anything else installed and working on your machine.
4. Run `python build.py`.
5. Folder with build is `/Fusion360Guide`.

Additional info: use `--os=VALUE` flag to specify OS. Supported values are: `win10` (default), `linux`, `osx`.
