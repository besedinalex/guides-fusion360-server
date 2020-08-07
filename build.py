from os import listdir, path, remove
from shutil import rmtree, copyfile
from argparse import ArgumentParser
from subprocess import call
from distutils.dir_util import copy_tree

# Build options to target specific OS
parser = ArgumentParser(description='Build options.')
parser.add_argument('-o', '--os', default='win')
parser.add_argument('-a', '--arch', default='x64')
parser.add_argument('-i', '--npm_install', default='false')
args = vars(parser.parse_args())
os = args['os']
arch = args['arch']
npm_install = args['npm_install']

# Checks arguments
if os != 'win' and os != 'macos' and os != 'linux':
    print('Supported systems are: win, macos, linux.')
    quit()
if arch != 'x86' and arch != 'x64':
    print('Supported architectures are: x86, x64')
    quit()

# Install dependencies
if npm_install == 'true':
    call('npm i', shell=True)
    call('npm i', shell=True, cwd='../guides-fusion360-client')

# Builds client
call('npm run build', shell=True, cwd='../guides-fusion360-client')

# Removes previous built client
if path.isdir('./public'):
    rmtree('./public')

# Copies built client to server
call('mkdir public', shell=True)
for item in listdir('./../guides-fusion360-client/build'):
    old_path = './../guides-fusion360-client/build/{}'.format(item)
    new_path = './public/{}'.format(item)
    if path.isfile(old_path) or path.islink(old_path):
        copyfile(old_path, new_path)
    elif path.isdir(old_path):
        copy_tree(old_path, new_path)

# Compiles proper sqlite3 for chosen OS and copies it to /build
command = './node_modules/.bin/node-pre-gyp install --directory=./node_modules/sqlite3'
sqlite3_path = './node_modules/sqlite3/lib/binding/node-v72'
if os == 'win':
    command += ' --target_platform=win32'
    sqlite3_path += '-win32'
elif os == 'macos':
    command += ' --target_platform=darwin'
    sqlite3_path += '-darwin'
elif os == 'linux':
    command += ' --target_platform=linux'
    sqlite3_path += '-linux'
if arch == 'x86':
    command += ' --target_arch=ia32'
    sqlite3_path += '-ia32'
elif arch == 'x64':
    command += ' --target_arch=x64'
    sqlite3_path += '-x64'
sqlite3_path += '/node_sqlite3.node'
call(command, shell=True)

# Compiles server
call('./node_modules/typescript/bin/tsc', shell=True)
command = 'pkg . --targets=node12-{}-{}'.format(os, arch)
call(command, shell=True)

# Copies sqlite3, exec and config to /build folder
if path.isdir('./build'):
    rmtree('./build')
call('mkdir build', shell=True)
copyfile(sqlite3_path, './build/node_sqlite3.node')
copyfile('./config.json', './build/config.json')
if os == 'win':
    copyfile('./guides-fusion360-server.exe', './build/FUSION360GUIDE.exe')
    remove('./guides-fusion360-server.exe')
else:
    print('\nWARNING: Copy guides-fusion360-server file to /build folder.')

print('\nBuilding is finished.\n')