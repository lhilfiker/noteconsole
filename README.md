# 📝 noteconsole

💻 | Welcome to the Console-Based Notes App, a minimalistic text editor that runs in your console. 
      This app allows you to create, edit, and manage your text-based notes with ease. 
       It's a perfect tool for quick note-taking and text file management.

# Table of contents

- [Introduction](#introduction)
- [Features](#features)
- [Installation](#Installation)
- [Keyboard Shortcuts](#keyboard-shortcuts)
- [Contributing](#contributing)
- [License](#license)

## Introduction

The Console-Based Notes App is a command-line tool designed for note management, catering to developers, 
writers, and anyone who appreciates the simplicity of console-based applications.

## Features

- 📁 | Select a file to open from a file picker (PC files)
- 📁 | Create a new file and select a folder with folder picker
- 📝 | Open and show recently opened files
- 🔒 | Password-based encryption
- 🧭 | Easily navigate through your notes
- 💡 | Minimalistic and distraction-free interface
- ✏️ | Edit the text
- 🖱️ | Select text
- ✂️ | Copy Text
- 📋 | Paste Text
- 🖨️ | Print Text

## Installation
#### Download the Executable:
Download the latest release from here: https://github.com/RebelCoderJames/noteconsole/releases


#### Package Managers for Debian / Ubuntu / Linux

1. **Add the GPG Key:**
   ```bash
   wget -qO - http://rep.rebelcoderjames.tech/repo-key.gpg | sudo gpg --dearmor -o /usr/share/keyrings/rebelcoderjames.gpg
   ```

2. **Add the Repository:**
   ```bash
   echo "deb [signed-by=/usr/share/keyrings/rebelcoderjames.gpg] http://rep.rebelcoderjames.tech/ stable main" | sudo tee /etc/apt/sources.list.d/rebelcoderjames.list
   ```

3. **Install the Package:**
   ```bash
   sudo apt update && sudo apt install noteconsole
   ```

### Building it on your own

**Currently tested on Debian Linux only.**
Follow the instructions BUILD.md

### Additional Installation Information

For Windows:
- none

For Linux:
- For Copy/Paste to work, be sure to have `xsel` installed.

## Keyboard Shortcuts

- N | Create new note
- R | Open recent files
- S | Pick a file from PC
- Ctrl N | Open the sidepanel
- Esc | to exit
- Ctrl S | saving the file
- Ctrl P | Disable/Enable Plugins

## Contributing

Contributions are welcome. 
Whether you want to fix a bug, add a new feature, or improve the documentation, feel free to submit a pull request. 

## License
This project is licensed under the Apache 2.0 License - see the [LICENSE](LICENSE) file for details.
