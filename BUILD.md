How to build it:

You can use the debian_build.sh script if you want.

For Debian/Linux .deb:

1. Download the Source Code:
    `git clone https://github.com/RebelCoderJames/noteconsole.git`

2. Install Dependencies:
    `sudo apt-get update && sudo apt-get install -y dotnet-sdk-6.0 dpkg-dev debhelper gpg fakeroot`

3. Build the .Net Application:
    - Navigate to the source codes folder:
        `cd noteconsole/noteconsole`
    - Build it with dotnet
        `dotnet publish -c Release -r linux-x64 --self-contained`
        You can ignore the errors safely.

4. Create the Debian File Structur
    - Create the Directory:
        `mkdir -p noteconsole/DEBIAN noteconsole/usr/share/noteconsole`
    - Copy the application inside:
        `cp -r noteconsole/bin/Release/net6.0/linux-x64/publish/* noteconsole/usr/share/noteconsole/`

5. Create a Debian Control File
    - Create and open it:
        `nano noteconsole/DEBIAN/control`
    - Add the following content:

        ```
        Package: noteconsole
        Version: 0.1.0
        Section: editors
        Priority: optional
        Architecture: amd64
        Maintainer: RebelCoderJames <contact@rebelcoderjames.tech>
        Recommends: xsel
        Description: A simple note editor for the console.
        ```

6. Build the .deb package:
    - Build it:
        `dpkg-deb --build noteconsole`
    - (optional) Rename the output file:
        `mv noteconsole.deb noteconsole_0.1.0_amd64.deb`
