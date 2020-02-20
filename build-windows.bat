echo:
echo --- Submodules (Dependencies) ---
echo:
git submodule update --init --recursive
echo:
echo --- Nuget Packages ---
echo:
nuget restore DProject.Dependencies/MonoGame.Extended/Source/MonoGame.Extended.sln
nuget restore DProject.Dependencies/bepuphysics2/Library.sln
nuget restore DProject.Dependencies/FNA/FNA.sln
nuget restore DProject.Windows.sln
echo:
echo --- Building Dependencies ---
echo:
call msbuild DProject.Dependencies/FNA/FNA.sln -property:Configuration=Release -property:Platform="AnyCPU"
call msbuild DProject.Dependencies/MonoGame.Extended/Source/MonoGame.Extended/MonoGame.Extended.csproj -property:Configuration=Release
call msbuild DProject.Dependencies/MonoGame.Extended/Source/MonoGame.Extended.Animations/MonoGame.Extended.Animations.csproj -property:Configuration=Release
call msbuild DProject.Dependencies/MonoGame.Extended/Source/MonoGame.Extended.Entities/MonoGame.Extended.Entities.csproj -property:Configuration=Release
call msbuild DProject.Dependencies/bepuphysics2/Library.sln -property:Configuration=Release
echo:
echo --- Building Shaders ---
echo:
cd DProject/Content/shaders/
call CompileShaders.bat
cd ../../../
echo:
echo --- Building DProject.Windows ---
echo:
call msbuild DProject.Windows.sln -property:Configuration=Release
curl -L -o fnalibs.tar.bz2 https://www.dropbox.com/s/25nsu414fztq0ot/fnalibs.tar.bz2
7z e fnalibs.tar.bz2
7z e fnalibs.tar -obin/Release/Windows/ x64/*
7z e fnalibs.tar -obin/Release/Testing/Windows/net471/ x64/*
del fnalibs.tar.bz2
del fnalibs.tar
echo:
echo --- Running NUnit Test (DProject.Windows) ---
echo:
cd bin/Release/Testing/Windows/net471/
nunit-console /nologo DProject.Testing.Windows.dll
echo:
echo --- End ---
echo:
