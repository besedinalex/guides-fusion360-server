from os import path
from shutil import rmtree
from argparse import ArgumentParser
from subprocess import call
from distutils.dir_util import copy_tree

parser = ArgumentParser(description='Publish options.')
parser.add_argument('-o', '--os')
parser.add_argument('-ic', '--rebuild-client', default='true')
args = vars(parser.parse_args())
os = args['os']
rebuild_client = args['rebuild_client']

if os != None and os != 'linux' and os != 'win10':
    print('\nSupported --os values are: `linux` and `win10`.\n')
    quit()

if rebuild_client != 'true' and rebuild_client != 'false':
    print('\nSupported --rebuild-client values are `true` or `false`.\n')
    quit()

if rebuild_client == 'true':
    if not path.isdir('./../guides-fusion360-client'):
        print('\nFolder where you keep /guides-fusion360-server should contain /guides-fusion360-client.\n')
        exit()

    if not path.isdir('./../guides-fusion360-client/node_modules'):
        print('\nGetting React app dependencies.')
        call('npm ci', shell=True, cwd='../guides-fusion360-client')
        print('\nFinished getting React app dependencies.')

    print('\nBuilding React app.')
    call('npm run build', shell=True, cwd='../guides-fusion360-client')

    print('Moving React app build to /wwwroot.')
    if path.isdir('./GuidesFusion360Server/wwwroot'):
        rmtree('./GuidesFusion360Server/wwwroot')
    copy_tree('./../guides-fusion360-client/build', './GuidesFusion360Server/wwwroot')

print('\nBuilding server.\n')
command = 'dotnet publish -c Release{}'.format('' if os == None else ' -r {}-x64'.format(os))
call(command, shell=True, cwd='./GuidesFusion360Server')

if path.isdir('./Fusion360Guide'):
    rmtree('./Fusion360Guide')
build = './GuidesFusion360Server/bin/Release/net5.0/{}publish'.format('' if os == None else '{}-x64/'.format(os))
copy_tree(build, './Fusion360Guide')

print('\nBuilding is finished and is available in /Fusion360Guide.\n')