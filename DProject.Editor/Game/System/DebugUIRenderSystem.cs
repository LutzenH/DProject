using System;
using DProject.Type.Rendering;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities.Systems;
using Num = System.Numerics;

namespace DProject.Manager.System
{
    public class DebugUIRenderSystem : DrawSystem
    {
        private readonly ShaderManager _shaderManager;
        private readonly GraphicsDevice _graphicsDevice;
     
        private readonly ImGuiRenderer _imGuiRenderer;
        private readonly IntPtr[] _imGuiTexture = { IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero };

        private bool _showTestWindow;
        private bool _showRenderBufferWindow;
        
        public DebugUIRenderSystem(GraphicsDevice graphicsDevice, ShaderManager shaderManager)
        {
            _shaderManager = shaderManager;
            _graphicsDevice = graphicsDevice;
            
            _imGuiRenderer = new ImGuiRenderer(_graphicsDevice);
            _imGuiRenderer.RebuildFontAtlas();
        }

        public override void Draw(GameTime gameTime)
        {
            _imGuiTexture[0] = _imGuiRenderer.BindTexture(_shaderManager.Color);
            _imGuiTexture[1] = _imGuiRenderer.BindTexture(_shaderManager.Depth);
            _imGuiTexture[2] = _imGuiRenderer.BindTexture(_shaderManager.Normal);
            _imGuiTexture[3] = _imGuiRenderer.BindTexture(_shaderManager.Lights);
            _imGuiTexture[4] = _imGuiRenderer.BindTexture(_shaderManager.SSAO);

            // Call BeforeLayout first to set things up
            _imGuiRenderer.BeforeLayout(gameTime);
            
            // Draw our UI
            ImGuiLayout();
            
            // Call AfterLayout now to finish up and draw all the things
            _imGuiRenderer.AfterLayout();
            
            _imGuiRenderer.UnbindTexture(_imGuiTexture[0]);
            _imGuiRenderer.UnbindTexture(_imGuiTexture[1]);
            _imGuiRenderer.UnbindTexture(_imGuiTexture[2]);
            _imGuiRenderer.UnbindTexture(_imGuiTexture[3]);
            _imGuiRenderer.UnbindTexture(_imGuiTexture[4]);
        }

        protected virtual void ImGuiLayout()
        {
            if (ImGui.Button("Test Window")) _showTestWindow = !_showTestWindow;
            if (ImGui.Button("Render Buffers")) _showRenderBufferWindow = !_showRenderBufferWindow;
            ImGui.Text($"Application average {1000f / ImGui.GetIO().Framerate:F3} ms/frame ({ImGui.GetIO().Framerate:F1} FPS)");

            if (_showRenderBufferWindow)
            {
                ImGui.SetNextWindowSize(new Num.Vector2(200, 100), ImGuiCond.FirstUseEver);
                ImGui.Begin("Render Buffers", ref _showRenderBufferWindow);
                
                ImGui.Text("Color");
                ImGui.Image(_imGuiTexture[0], new Num.Vector2(300, 150), Num.Vector2.Zero, Num.Vector2.One, Num.Vector4.One, Num.Vector4.One);
                ImGui.Text("Depth");
                ImGui.Image(_imGuiTexture[1], new Num.Vector2(300, 150), Num.Vector2.Zero, Num.Vector2.One, Num.Vector4.One, Num.Vector4.One);
                ImGui.Text("Normal");
                ImGui.Image(_imGuiTexture[2], new Num.Vector2(300, 150), Num.Vector2.Zero, Num.Vector2.One, Num.Vector4.One, Num.Vector4.One);
                ImGui.Text("Lights");
                ImGui.Image(_imGuiTexture[3], new Num.Vector2(300, 150), Num.Vector2.Zero, Num.Vector2.One, Num.Vector4.One, Num.Vector4.One);
                ImGui.Text("SSAO");
                ImGui.Image(_imGuiTexture[4], new Num.Vector2(300, 150), Num.Vector2.Zero, Num.Vector2.One, Num.Vector4.One, Num.Vector4.One);
                
                ImGui.End();
            }

            if (_showTestWindow)
            {
                ImGui.SetNextWindowPos(new Num.Vector2(650, 20), ImGuiCond.FirstUseEver);
                ImGui.ShowDemoWindow(ref _showTestWindow);
            }
        }
    }
}
