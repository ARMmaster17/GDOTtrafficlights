xbuild /p:Configuration=Debug GDOTtrafficsystem.sln
xbuild /p:Configuration=Release GDOTtrafficsystem.sln
mkdir latest
cp /Core/bin/Release/Core.exe /latest/Core.exe
