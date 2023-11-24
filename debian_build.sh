#!/bin/bash

# Ask for the version number
echo "Enter the version number for the package (e.g., 0.1.0):"
read version

cd noteconsole
rm -rf noteconsole/DEBIAN
rm -rf package

# Build the .Net Application
dotnet publish -c Release -f net6.0 --self-contained
mkdir -p package/DEBIAN package/usr/lib/noteconsole
cp -r noteconsole/bin/Release/net6.0/linux-x64/publish/* package/usr/lib/noteconsole/
# Create Symbolic Link for Executable
mkdir -p package/usr/bin
ln -s /usr/lib/noteconsole/noteconsole package/usr/bin/noteconsole

# Create a Debian Control File
cat <<EOF > package/DEBIAN/control
Package: noteconsole
Version: $version
Section: editors
Priority: optional
Architecture: amd64
Maintainer: RebelCoderJames <contact@rebelcoderjames.tech>
Depends: dotnet-runtime-6.0
Recommends: xsel
Description: A simple note editor for the console.
EOF

# Build the .deb package
dpkg-deb --build package

# Optional: Rename the output file
mv package.deb noteconsole_"$version"_amd64.deb

rm -rf package
rm -rf noteconsole/DEBIAN