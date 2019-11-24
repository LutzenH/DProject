@echo off
ECHO.
ECHO The shaders for the linux platform require to be DirectX fxc compiled files with profile: fx_2_0, This can be done on a Windows machine with DirectX SDK installed or using wine with DirectX SDK installed. Build using shader model 3 (vs_3_0, ps_3_0). the compile command should look something like this: fxc.exe /Tfx_2_0 "SHADERNAME.fx" /Fo
ECHO.
"C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Utilities\bin\x64\fxc.exe" /Tfx_2_0 "FXAAShader.fx" /Fo
"C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Utilities\bin\x64\fxc.exe" /Tfx_2_0 "WaterShader.fx" /Fo
"C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Utilities\bin\x64\fxc.exe" /Tfx_2_0 "gbuffer/RenderGBuffer.fx" /Fo
"C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Utilities\bin\x64\fxc.exe" /Tfx_2_0 "gbuffer/ClearGBuffer.fx" /Fo
"C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Utilities\bin\x64\fxc.exe" /Tfx_2_0 "gbuffer/DirectionalLight.fx" /Fo
"C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Utilities\bin\x64\fxc.exe" /Tfx_2_0 "gbuffer/PointLight.fx" /Fo
"C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Utilities\bin\x64\fxc.exe" /Tfx_2_0 "gbuffer/CombineFinal.fx" /Fo
"C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Utilities\bin\x64\fxc.exe" /Tfx_2_0 "SkyShader.fx" /Fo
