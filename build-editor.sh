echo -e '\n--- Submodules (Dependencies) ---\n'
git submodule update --init --recursive
echo -e '\n--- Nuget Packages ---\n'
nuget restore DProject.Dependencies/MonoGame.Extended/Source/MonoGame.Extended.sln
nuget restore DProject.Dependencies/bepuphysics2/Library.sln
nuget restore DProject.Dependencies/FNA/FNA.sln
nuget restore DProject.Editor.sln
echo -e '\n--- Building Dependencies ---\n'
msbuild DProject.Dependencies/FNA/FNA.sln -property:Configuration=Release
msbuild DProject.Dependencies/MonoGame.Extended/Source/MonoGame.Extended/MonoGame.Extended.csproj -property:Configuration=Release
msbuild DProject.Dependencies/MonoGame.Extended/Source/MonoGame.Extended.Animations/MonoGame.Extended.Animations.csproj -property:Configuration=Release
msbuild DProject.Dependencies/MonoGame.Extended/Source/MonoGame.Extended.Entities/MonoGame.Extended.Entities.csproj -property:Configuration=Release
msbuild DProject.Dependencies/bepuphysics2/Library.sln -property:Configuration=Release
echo -e '\n--- Building Shaders ---\n'
cd DProject/Content/shaders/
./CompileShaders.sh
cd ../../../
echo -e '\n--- Building DProject.Editor ---\n'
msbuild DProject.Editor.sln -property:Configuration=Release
wget https://www.dropbox.com/s/25nsu414fztq0ot/fnalibs.tar.bz2
tar -jxvf fnalibs.tar.bz2 -C bin/Release/Editor lib64/ --strip-components 1
tar -jxvf fnalibs.tar.bz2 -C bin/Release/Testing/Editor/net471 lib64/ --strip-components 1
rm fnalibs.tar.bz2
echo -e '\n--- Running NUnit Test (DProject.Editor) ---\n'
cd bin/Release/Testing/Editor/net471/
nunit-console --nologo DProject.Testing.Editor.dll
echo -e '\n--- End ---\n'
