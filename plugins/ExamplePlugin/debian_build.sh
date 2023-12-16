#!/bin/bash

# Ask for version
read -p "Enter version: " version

# Define paths
BUILD_DIR="./bin/Release/net8.0"
DEB_DIR="./deb_package"
DEB_CONTROL_FILE="$DEB_DIR/DEBIAN/control"
pkgname="packagename"

rm -rf $BUILD_DIR
rm -rf $DEB_DIR

# Build the .NET project
dotnet build --configuration Release

# Create directories for the Debian package
mkdir -p $DEB_DIR
mkdir -p $DEB_DIR/DEBIAN
mkdir -p $DEB_DIR/usr/share/$pkgname

# Create the control file
cat <<EOT > $DEB_CONTROL_FILE
Package: $pkgname
Version: $version
Section: custom
Priority: optional
Architecture: amd64
Depends: noteconsole
Maintainer: RebelCoderJames <contact@rebelcoderjames.tech>
Description: Plugin for noteconsole.
EOT

# Copy the DLL to a temporary directory in the package
cp $BUILD_DIR/*.dll $DEB_DIR/usr/share/$pkgname

# Delete PluginShared.dll if it exists in the temporary directory
rm -f $DEB_DIR/usr/share/$pkgname/PluginShared.dll

# Create postinst script
cat <<EOT > $DEB_DIR/DEBIAN/postinst
#!/bin/bash
# Post-installation script to move the DLL to the user's home directory

# Create the target directory if it does not exist
mkdir -p $HOME/.config/noteconsole/plugins

# Move the DLL
mv /usr/share/$pkgname/*.dll $HOME/.config/noteconsole/plugins

# Set permissions
chown -R \$USER:\$USER $HOME/.config/noteconsole/plugins
chmod -R 755 $HOME/.config/noteconsole/plugins

exit 0
EOT

# Make the postinst script executable
chmod 755 $DEB_DIR/DEBIAN/postinst

# Build the Debian package
dpkg-deb --build $DEB_DIR $pkgname-$version.deb

echo "Package built: $pkgname-$version.deb"
