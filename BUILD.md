# How to build it:

## For Debian/Linux .deb:

1. Download the Source Code:
    `git clone https://github.com/RebelCoderJames/noteconsole.git`

2. Install Dependencies:
    `sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0 dpkg-dev debhelper gpg fakeroot`

3. Build it:
   ```
   cd noteconsole
   ./debian_build.sh
   ```
4. Install
    ```
    cd noteconsole
    sudo dpkg -i noteconsole_Version_amd64.deb
    ```
