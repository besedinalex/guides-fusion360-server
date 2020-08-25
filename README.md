# Beginner's guides to Fusion 360

Web server for keeping guides data and making possible creating and editing them.

## Setup

Install .NET Core from [official site](https://dotnet.microsoft.com/download).

Make sure that your CLI now handles `dotnet` command.

## Debug

Run `cd GuidesFusion360Server`.

Run `dotnet run` or `dotnet watch run`.

## Deploy

Proper way to deploy can be found in [official Microsoft documentation](https://docs.microsoft.com/en-us/dotnet/core/deploying/)  or [another their documentation](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/?view=aspnetcore-3.1).

### Considering my app:

1. Install Python 3 from [official site](https://www.python.org/downloads/).
2. Make sure your CLI now handles `python` or `python3` commands.
3. Client project folder should be named `/guides-fusion360-client` and placed in the same folder where guides-fusion360-server is. Read it's [README.md](https://github.com/besedinalex/guides-fusion360-client/blob/master/README.md) to see if you need anything else installed and working on your machine.
4. Run `python build.py`.
5. Folder you need is `/Fusion360Guide`.

Additional info: use `--os=VALUE` flag to specify OS. Supported values are: `win10` (default), `linux`, `osx`.
