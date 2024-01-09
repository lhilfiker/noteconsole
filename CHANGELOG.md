# Changelog 

## v.0.4.0 UNRELEASED
New:
 - Load Plugins in /AppData/noteconsole/plugins.
 - Use MainFunction of loaded Plugin to manipulate GlobalColorsList
 - Find suitable Plugin for file extension.
 - Only load plugins with supported version.
 - Default Highlighting Plugin
 - Upgraded to Dot NET 8.0 SDK for better performance.
 - Selection in Selection Mode is now color highlighted.
 - Ctrl P to easily disable/enable plugins
Improvments:
 - Rendering is now much faster and less flickering

Bug Fixes:
 - Do not allow to go one over when last char is space.

## v.0.3.0 (6.12.2023)
New:
 - Allow to enter file name / path in command line to open it.
 - Ctrl Delete Backspace or D to delete from start line to current Position on line.

Improvments:
 - Hide Cursor on Welcome Screen
 - Properly handle Ctrl and Alt Keys to prevent breaking.

Bug Fixes:
 - Fix crash after enter and backspace on start of file.
 - Fix that it prooperly sets cursorx
 - Clear Temrminal when leaving the app.
 - Ctrl Right Arrow is one too far
 - Fix that delete will not change max character limit.

## v.0.2.3 (30.11.2023)
Improvments:
 - Preperation: Background service wait for changes
 - Q to leave app
 - Better Message when window is too small

Bug Fixes:
 - Remove testing colors

## v.0.2.2 (30.11.2023)
New:
 - Add support for Background Color.

Improvements:
 - Rendering optimization.

Bug fixes:
 - Fix that app crashes when GlobalColorsList is updated at the same time a line is proceded.
 - Fix crash when window is too small.
 - Fix that background color will be white after going into selection mode in some terminals.

## v.0.2.1
- Remove DotNet 6.0 as a requirement as it is already selfcontained and it isn't in debian repositorys.
- New Build and signing proceedure.

## v.0.2.0
New Features:
 - Support for rendering Colors. in v0.3 a plugin system will get added that can then load diffrent display plugins which can set specific text to specific color.

Bug fixes:
 - Fix that it doesnt try to open nothing after pressing a key not n,r,s on the welcome screen causing slight lag.
 - Fix that after writing a characcter you cant go to the last char in the line anymore.
 - Fix that when pressing up or downarrow and the cursorx is more than the line going to it would set cursorx one too far.
 - Fix multiple possible null refrences
 - Fix that when scrolling horizontally the mouse pointer will be one off.

 Smaller Changes:
  - Minor Code Cleanup
   

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
