# Changelog 

## v.0.2.0 UNRELEASED
New Features:
 - Rendering colors, this will be then used by a background service which will run stuff like Spellchecker and will tell the filformater to make those stuff this color. Documentation will follow.

Bug fixes:
 - Fix that it doesnt try to open nothing after pressing a key not n,r,s on the welcome screen causing slight lag.
 - Fix that after writing a characcter you cant go to the last char in the line anymore.
   
Planned:
 - Background Service
 - Documentation for Background Service
 - Initial Spellchecker implementation.

## v.0.1.3
Bug Fixes:
 - Fix that the whole Terminal is off by one line(caused only on unix systems)
 - Build Linux Application into their own folder.
 - Fix that unneccecary files are included in the build.
 - Make .net 6.0 runtime as a dependenicy to reduce package size and remove unnececary disk space when already installed.

## v.0.1.2
Bug fixes:
 - Clear the whole console in UNIX Terminals.
 - Fix wrong path in debian_build.sh

## v.0.1.1
Fixed multiple bugs and made few improvments. You can now encrypt a non encrypted file with [Ctrl E]

## v.0.1.0
Initial Release
