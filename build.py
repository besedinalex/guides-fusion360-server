from os import path
from shutil import rmtree
from argparse import ArgumentParser
from subprocess import call
from distutils.dir_util import copy_tree

# Build options to target specific OS
parser = ArgumentParser(description='Build options.')
parser.add_argument('-o', '--os', default='win10')
args = vars(parser.parse_args())
os = args['os']

# Checks arguments
if os != 'win10' and os != 'linux' and os != 'osx':
    print('Supported systems are: win10, linux, osx.')
    quit()

# Checks for guides-fusion360-client folder
if not path.isdir('./../guides-fusion360-client'):
    print('Folder where you keep /guides-fusion360-server/ should contain /guides-fusion360-client/.')
    exit()

# Installs dependencies
if not path.isdir('./../guides-fusion360-client/node_modules'):
    call('npm i', shell=True, cwd='../guides-fusion360-client')

# Builds client
call('npm run build', shell=True, cwd='../guides-fusion360-client')

# Removes previous built client
if path.isdir('./GuidesFusion360Server/wwwroot'):
    rmtree('./GuidesFusion360Server/wwwroot')

# Copies built client to server
copy_tree('./../guides-fusion360-client/build', './GuidesFusion360Server/wwwroot')

# Compiles server
command = 'dotnet publish -r {}-x64 -c Release'.format(os)
call(command, shell=True, cwd='./GuidesFusion360Server')

# Copies sqlite3, exec and config to /build folder
if path.isdir('./Fusion360Guide'):
    rmtree('./Fusion360Guide')
build = './GuidesFusion360Server/bin/Release/netcoreapp3.1/{}-x64/publish'.format(os)
copy_tree(build, './Fusion360Guide')

print('\nBuilding is finished.\n')