# BUILD
xbuild /p:Configuration=Debug GDOTtrafficsystem.sln
xbuild /p:Configuration=Release GDOTtrafficsystem.sln
# PREPARE RELEASE FOLDER
mkdir latest
# DEBUG
ls
# COPY FILES
cp ./LICENSE ./latest/LICENSE
cp ./README.md ./latest/README.md
cp ./settings.ini ./latest/settings.ini
cp ./modules.ini ./latest/modules.ini
cp ./Core/bin/Release/Core.exe ./latest/Core.exe
cd ./latest
mkdir TLgui
cd ..
cp ./TLgui/bin/Release/TLgui.exe ./latest/TLgui/TLgui.exe
