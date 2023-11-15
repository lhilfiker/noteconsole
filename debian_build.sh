#!/bin/bash

# Ask for the version number
echo "Enter the version number for the package (e.g., 0.1.0):"
read version

# Clone the repository
git clone https://github.com/RebelCoderJames/noteconsole.git

# Install dependencies
sudo apt-get update && sudo apt-get install -y dotnet-sdk-6.0 dpkg-dev debhelper gpg fakeroot

# Build the .Net Application
cd noteconsole/noteconsole
dotnet publish -c Release -r linux-x64 --self-contained # You can ignore the errors safely.

# Create the Debian File Structure
mkdir -p ../DEBIAN ../usr/local/bin
cp -r noteconsole/bin/Release/net6.0/linux-x64/publish/* ../usr/local/bin/

# Create a Debian Control File
cat <<EOF >../DEBIAN/control
Package: noteconsole
Version: $version
Section: editors
Priority: optional
Architecture: amd64
Maintainer: RebelCoderJames <contact@rebelcoderjames.tech>
Recommends: xsel
Description: A simple note editor for the console.
EOF

# Build the .deb package
cd ..
dpkg-deb --build noteconsole

# Optional: Rename the output file
mv noteconsole.deb noteconsole_"$version"_amd64.deb
