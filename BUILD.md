How to build it:

You can use the debian_build.sh script if you want.

For Debian/Linux .deb:

1. Download the Source Code:
    `git clone https://github.com/RebelCoderJames/noteconsole.git`

2. Install Dependencies:
    `sudo apt-get update && sudo apt-get install -y dotnet-sdk-6.0 dpkg-dev debhelper gpg fakeroot`

3. Build it:
   ```
   cd noteconsole
   ./debian_build.sh
   ```

