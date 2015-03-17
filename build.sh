# BUILD
xbuild /p:Configuration=Debug GDOTtrafficsystem.sln
xbuild /p:Configuration=Release GDOTtrafficsystem.sln
# PREPARE RELEASE FOLDER
mkdir latest
# DEBUG
ls
# COPY FILES
echo 'Copying LICENSE'
cp ./LICENSE ./latest/LICENSE
echo 'Copying README.md'
cp ./README.md ./latest/README.md
echo 'Copying settings.ini'
cp ./settings.ini ./latest/settings.ini
echo 'Copying modules.ini'
cp ./modules.ini ./latest/modules.ini
echo 'Copying Core.exe'
cp ./Core/bin/Release/Core.exe ./latest/Core.exe
echo 'Copying TLgui'
cd ./latest
mkdir TLgui
cd ..
cp ./TLgui/bin/Release/TLgui.exe ./latest/TLgui/TLgui.exe
echo 'Copying TLpserver'
cd ./latest
mkdir TLpserver
cd ..
cp ./TLpserver/bin/Release/TLpserver.exe ./latest/TLserver/TLserver.exe

tar -czf latest.tar.gz ./latest
