using System;
using System.Collections.Generic;
using System.Linq;
using DProject.Game;
using DProject.Game.Component;
using DProject.Type.Rendering;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Num = System.Numerics;
using PrimitiveType = DProject.Type.Rendering.Primitives.PrimitiveType;

namespace DProject.Manager.System
{
    public class DebugUIRenderSystem : EntityDrawSystem
    {
        public static int? SelectedEntity;

        private readonly Game1 _game;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly PhysicsSystem _physicsSystem;
        private readonly ViewportRenderSystem _viewportRenderSystem;
     
        private readonly ImGuiRenderer _imGuiRenderer;
        private readonly IntPtr[] _imGuiTexture = { IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero };

        private readonly IntPtr[] _viewPortTextures = {IntPtr.Zero };
        
        private bool _showTestWindow;
        private bool _showRenderBufferWindow;
        private bool _showEntityListWindow;
        private bool _showViewportWindow;

        public static readonly Dictionary<int, string> EntityIdentifiers = new Dictionary<int, string>();
        private readonly IEnumerable<global::System.Type> _componentTypes;

        private object _clipboard;

        private const int MaxFrameRateValues = 100;
        private readonly float[] _frameRateValues = new float[MaxFrameRateValues];
        private int _frameRateValuesIndex;
        
        public DebugUIRenderSystem(Game1 game, GraphicsDevice graphicsDevice, PhysicsSystem physicsSystem, ViewportRenderSystem viewportRenderSystem) : base(Aspect.Exclude())
        {
            _graphicsDevice = graphicsDevice;
            _physicsSystem = physicsSystem;
            _viewportRenderSystem = viewportRenderSystem;
            _game = game;
            
            _imGuiRenderer = new ImGuiRenderer(_graphicsDevice);
            _imGuiRenderer.RebuildFontAtlas();
            
            _componentTypes = typeof(IComponent).Assembly.GetTypes().Where(
                t => typeof(IComponent).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
        }

        public override void Draw(GameTime gameTime)
        {
            _graphicsDevice.SetRenderTarget(null);
            _graphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1, 0);
            
            _imGuiTexture[0] = _imGuiRenderer.BindTexture(ShaderManager.Instance.Color);
            _imGuiTexture[1] = _imGuiRenderer.BindTexture(ShaderManager.Instance.Depth);
            _imGuiTexture[2] = _imGuiRenderer.BindTexture(ShaderManager.Instance.LightInfo);
            _imGuiTexture[3] = _imGuiRenderer.BindTexture(ShaderManager.Instance.Normal);
            _imGuiTexture[4] = _imGuiRenderer.BindTexture(ShaderManager.Instance.Lights);
            _imGuiTexture[5] = _imGuiRenderer.BindTexture(ShaderManager.Instance.ShadowMap);
            _imGuiTexture[6] = _imGuiRenderer.BindTexture(ShaderManager.Instance.SSAO);

            _viewPortTextures[0] = _imGuiRenderer.BindTexture(_viewportRenderSystem.GetViewports()[0].GetViewport());
            
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
            _imGuiRenderer.UnbindTexture(_imGuiTexture[5]);
            _imGuiRenderer.UnbindTexture(_imGuiTexture[6]);
            
            _imGuiRenderer.UnbindTexture(_viewPortTextures[0]);
        }

        protected virtual void ImGuiLayout()
        {
            if (ImGui.Button("Test Window")) _showTestWindow = !_showTestWindow;
            if (ImGui.Button("Render Buffers")) _showRenderBufferWindow = !_showRenderBufferWindow;
            if (ImGui.Button("Entity List")) _showEntityListWindow = !_showEntityListWindow;
            if (ImGui.Button("Viewport")) _showViewportWindow = !_showViewportWindow;
            
            var frameRate = 1000f / ImGui.GetIO().Framerate;
            _frameRateValues[_frameRateValuesIndex++ % MaxFrameRateValues] = frameRate;
            ImGui.PlotHistogram("", 
                ref _frameRateValues[0],
                MaxFrameRateValues,
                1,
                $"framerate (ms/frame) - {ImGui.GetIO().Framerate:F1} FPS",
                0f,
                _frameRateValues.Average()*2,
                new Num.Vector2(256, 64));

            ImGui.Separator();

            ImGui.Text("Graphics Settings:");
            
            var graphicsSettingsEnableFxaa = GraphicsManager.Instance.EnableFXAA;
            ImGui.Checkbox("FXAA", ref graphicsSettingsEnableFxaa);
            GraphicsManager.Instance.EnableFXAA = graphicsSettingsEnableFxaa;

            var graphicsSettingsEnableSsao = GraphicsManager.Instance.EnableSSAO;
            ImGui.Checkbox("SSAO", ref graphicsSettingsEnableSsao);
            GraphicsManager.Instance.EnableSSAO = graphicsSettingsEnableSsao;

            var graphicsSettingsEnableLights = GraphicsManager.Instance.EnableLights;
            ImGui.Checkbox("Lights", ref graphicsSettingsEnableLights);
            GraphicsManager.Instance.EnableLights = graphicsSettingsEnableLights;
            
            var graphicsSettingsEnableShadows = GraphicsManager.Instance.EnableShadows;
            ImGui.Checkbox("Shadows", ref graphicsSettingsEnableShadows);
            GraphicsManager.Instance.EnableShadows = graphicsSettingsEnableShadows;

            if (graphicsSettingsEnableShadows)
            {
                var graphicsSettingsShadowMapResolution = GraphicsManager.Instance.ShadowMapResolution;
                ImGui.InputInt("Resolution", ref graphicsSettingsShadowMapResolution, 1);
                GraphicsManager.Instance.ShadowMapResolution = graphicsSettingsShadowMapResolution;
            }

            var graphicsSettingsEnableSky = GraphicsManager.Instance.EnableSky;
            ImGui.Checkbox("Sky", ref graphicsSettingsEnableSky);
            GraphicsManager.Instance.EnableSky = graphicsSettingsEnableSky;
            
            var graphicsSettingsEnableVSync = GraphicsManager.Instance.EnableVSync;
            ImGui.Checkbox("VSync", ref graphicsSettingsEnableVSync);
            GraphicsManager.Instance.EnableVSync = graphicsSettingsEnableVSync;
            
            var graphicsSettingsEnableFullscreen = GraphicsManager.Instance.EnableFullscreen;
            ImGui.Checkbox("Fullscreen", ref graphicsSettingsEnableFullscreen);
            GraphicsManager.Instance.EnableFullscreen = graphicsSettingsEnableFullscreen;
            
            var graphicsSettingsEnableMaxFps = GraphicsManager.Instance.EnableMaxFps;
            ImGui.Checkbox("Limit FPS", ref graphicsSettingsEnableMaxFps);
            GraphicsManager.Instance.EnableMaxFps = graphicsSettingsEnableMaxFps;

            if (graphicsSettingsEnableMaxFps)
            {
                var graphicsSettingsMaxFps = GraphicsManager.Instance.MaxFps;
                ImGui.InputInt("Max FPS", ref graphicsSettingsMaxFps, 1);
                GraphicsManager.Instance.MaxFps = graphicsSettingsMaxFps;
            }

            if(ImGui.Button("Apply"))
                _game.UpdateGraphicsSettings();
            
            ImGui.Separator();
            
            ImGui.Text("Physics Settings:");

            var physicsSystemEnableSimulation = _physicsSystem.EnableSimulation;
            ImGui.Checkbox("Enabled", ref physicsSystemEnableSimulation);
            _physicsSystem.EnableSimulation = physicsSystemEnableSimulation;

            if (_clipboard != null)
            {
                ImGui.Separator();
                ImGui.Text("Clipboard: " + _clipboard.GetType().Name + ":" + _clipboard.GetHashCode());
            }

            if (_showRenderBufferWindow)
            {
                ImGui.SetNextWindowSize(new Num.Vector2(200, 100), ImGuiCond.FirstUseEver);
                ImGui.Begin("Render Buffers", ref _showRenderBufferWindow);
                
                DisplayImageWithTooltip(_imGuiTexture[0], "Color", new Num.Vector2(300, 150));
                DisplayImageWithTooltip(_imGuiTexture[1], "Depth", new Num.Vector2(300, 150));
                if(GraphicsManager.Instance.EnableLights)
                    DisplayImageWithTooltip(_imGuiTexture[2], "Light Info", new Num.Vector2(300, 150));
                DisplayImageWithTooltip(_imGuiTexture[3], "Normal", new Num.Vector2(300, 150));
                if(GraphicsManager.Instance.EnableLights)
                    DisplayImageWithTooltip(_imGuiTexture[4], "Lights", new Num.Vector2(300, 150));
                if(GraphicsManager.Instance.EnableShadows)
                    DisplayImageWithTooltip(_imGuiTexture[5], "Shadows", new Num.Vector2(300, 300));
                if(GraphicsManager.Instance.EnableSSAO)
                    DisplayImageWithTooltip(_imGuiTexture[6], "SSAO", new Num.Vector2(300, 150));
                
                ImGui.End();
            }

            if (_showTestWindow)
            {
                ImGui.SetNextWindowPos(new Num.Vector2(650, 20), ImGuiCond.FirstUseEver);
                ImGui.ShowDemoWindow(ref _showTestWindow);
            }

            if (_showEntityListWindow)
            {
                var windowTitle = "Entity List";

                if (SelectedEntity != null)
                    windowTitle += " - Selected Entity: " + SelectedEntity;

                windowTitle += "###SelectedEntity";
                
                ImGui.SetNextWindowSize(new Num.Vector2(200, 100), ImGuiCond.FirstUseEver);
                ImGui.Begin(windowTitle, ref _showEntityListWindow);

                ImGui.Columns(2);

                ImGui.BeginChild("Entities###SelectedEntityList");
                
                foreach (var entity in ActiveEntities)
                {
                    var itemName = entity.ToString();

                    if (EntityIdentifiers.ContainsKey(entity))
                        itemName += " - " + EntityIdentifiers[entity];
                    
                    if (ImGui.Selectable(itemName, entity == SelectedEntity))
                        SelectedEntity = entity;
                }

                ImGui.EndChild();
                
                ImGui.NextColumn();

                ImGui.BeginChild("Properties###SelectedEntityProperties");

                if (SelectedEntity != null)
                    BuildComponentListWindow((int) SelectedEntity);
                
                ImGui.EndChild();

                ImGui.End();
            }

            if (_showViewportWindow)
            {
                BuildViewportWindow(_viewPortTextures);
            }
        }

        private void BuildComponentListWindow(int selectedEntity)
        {
            var entity = GetEntity((int) SelectedEntity);

            var entityIdentifier = (string) EntityIdentifiers[selectedEntity].Clone();
            ImGui.InputText("Identifier", ref entityIdentifier, 128);
            EntityIdentifiers[selectedEntity] = entityIdentifier;
            
            ImGui.Text("Components: ");
            
            #region ComponentListBuilder
            
            foreach (var componentType in _componentTypes)
            {
                var hasMethod = typeof (Entity).GetMethod("Has");
                var hasGenericMethod = hasMethod.MakeGenericMethod(componentType);
                var entityHasComponent = (bool) hasGenericMethod.Invoke(entity, null);
                
                if (entityHasComponent)
                {
                    var getMethod = typeof (Entity).GetMethod("Get");
                    var getGenericMethod = getMethod.MakeGenericMethod(componentType);
                    var entityGetComponent = (IComponent) getGenericMethod.Invoke(entity, null);
                    
                    BuildComponentPropertiesList(entityGetComponent, entity, ref _clipboard);
                }
            }
            
            #endregion

            ImGui.End();
        }

        private static void BuildComponentPropertiesList(IComponent component, Entity entity, ref object clipboard)
        {
            var isAlive = true;
            if (component != null && ImGui.CollapsingHeader(component.GetType().Name, ref isAlive))
            {
                if (ImGui.IsItemHovered())
                {
                    var io = ImGui.GetIO();
                    
                    if (io.KeyCtrl && ImGui.IsKeyDown(ImGui.GetKeyIndex(ImGuiKey.C)))
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("Copied: " + component.GetHashCode());
                        clipboard = component;
                        ImGui.EndTooltip();
                    }
                }
                foreach (var property in component.GetType().GetProperties())
                {
                    var propertyValue = property.GetValue(component, null);

                    if (ImGui.TreeNode(property.Name))
                    {
                        if (property.CanWrite)
                        {
                            switch (propertyValue)
                            {
                                case Vector3 vector:
                                {
                                    var x = vector.X;
                                    var y = vector.Y;
                                    var z = vector.Z;
                            
                                    ImGui.InputFloat("x###" + "PropertyVector3X:" + property.Name + ":" + component.GetType().Name, ref x);
                                    ImGui.InputFloat("y###" + "PropertyVector3Y:" + property.Name + ":" + component.GetType().Name, ref y);
                                    ImGui.InputFloat("z###" + "PropertyVector3Z:" + property.Name + ":" + component.GetType().Name, ref z);
                            
                                    propertyValue = new Vector3(x, y, z);
                                    property.SetValue(component, propertyValue);
                                    break;
                                }
                                case Quaternion quaternion:
                                {
                                    var x = quaternion.X;
                                    var y = quaternion.Y;
                                    var z = quaternion.Z;
                                    var w = quaternion.W;
                            
                                    ImGui.InputFloat("x###" + "PropertyQuaternionX:" + property.Name + ":" + component.GetType().Name, ref x);
                                    ImGui.InputFloat("y###" + "PropertyQuaternionY:" + property.Name + ":" + component.GetType().Name, ref y);
                                    ImGui.InputFloat("z###" + "PropertyQuaternionZ:" + property.Name + ":" + component.GetType().Name, ref z);
                                    ImGui.InputFloat("w###" + "PropertyQuaternionW:" + property.Name + ":" + component.GetType().Name, ref w);
                            
                                    propertyValue = new Quaternion(x, y, z, w);
                                    property.SetValue(component, propertyValue);
                                    break;
                                }
                                case PrimitiveType type:
                                {
                                    var intType = (int) type;
                                    ImGui.SliderInt("Type###PropertyPrimitiveType:" + property.Name + ":" + component.GetType().Name, ref intType, 0, 2, type.ToString());
                            
                                    propertyValue = (PrimitiveType) intType;
                                    property.SetValue(component, propertyValue);
                                    break;
                                }
                                case Color color:
                                {
                                    var vec4Color = color.ToVector4();
                                    var inColor = new global::System.Numerics.Vector4(vec4Color.X, vec4Color.Y, vec4Color.Z, vec4Color.W);

                                    ImGui.ColorEdit4("Color###PropertyColor:" + property.Name + ":" + component.GetType().Name, ref inColor);
                            
                                    propertyValue = new Color(inColor.X, inColor.Y, inColor.Z, inColor.W);
                                    property.SetValue(component, propertyValue);
                                    break;
                                }
                                case float value:
                                {
                                    var floatValue = value;
                            
                                    ImGui.InputFloat("value###" + "PropertyFloatValue:" + property.Name + ":" + component.GetType().Name, ref floatValue);

                                    property.SetValue(component, floatValue);
                                    break;
                                }
                                case Model model:
                                    ImGui.Text("Root: " + model.Root.Name);
                                    ImGui.Text("Hash: " + model.GetHashCode());
                                    break;
                                case bool boolean:
                                {
                                    var boolValue = boolean;
                                    ImGui.Checkbox("###PropertyBooleanValue:" + property.Name + ":" + component.GetType().Name, ref boolValue);
                                    property.SetValue(component, boolValue);
                                    break;
                                }
                                case DPModel dpModel:
                                    ImGui.Text("Name: " + dpModel.Name);
                                    ImGui.Text("Triangle Count: " + dpModel.PrimitiveCount);
                                    ImGui.Text("BoundingSphere: " + dpModel.BoundingSphere);
                                    break;
                                case TransformComponent transformComponent:
                                {
                                    if (property.PropertyType == typeof(TransformComponent) && clipboard != null && clipboard.GetType() == typeof(TransformComponent) && component != clipboard)
                                    {
                                        if (ImGui.Button("Replace with clipboard"))
                                            property.SetValue(component, clipboard);
                                    }
                                    ImGui.Text("Position: " + transformComponent.Position);
                                    ImGui.Text("Scale: " + transformComponent.Scale);
                                    ImGui.Text("Rotation: " + transformComponent.Rotation);
                                    break;
                                }
                                case null when property.PropertyType == typeof(TransformComponent) && clipboard != null && clipboard.GetType() == typeof(TransformComponent) && component != clipboard:
                                {
                                    if (ImGui.Button("Paste TransformComponent"))
                                        property.SetValue(component, clipboard);
                                    break;
                                }
                                case null:
                                    ImGui.Text("None");
                                    break;
                            }
                        }
                        else
                            ImGui.Text(propertyValue.ToString());

                        ImGui.TreePop();
                    }
                }   
            }

            if (!isAlive)
            {
                var detachMethod = typeof (Entity).GetMethod("Detach");
                var detachGenericMethod = detachMethod.MakeGenericMethod(component.GetType());
                detachGenericMethod.Invoke(entity, null);
            }
        }

        private static void DisplayImageWithTooltip(IntPtr texturePointer, string name, Num.Vector2 size)
        {
            var io = ImGui.GetIO();
            
            ImGui.Text(name);
            ImGui.Image(texturePointer, size, Num.Vector2.Zero, Num.Vector2.One, Num.Vector4.One, Num.Vector4.One);
            var pos = ImGui.GetCursorScreenPos();
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();

                const float regionSize = 48.0f;
                
                var regionX = io.MousePos.X - pos.X - regionSize * 0.5f;
                if (regionX < 0.0f)
                    regionX = 0.0f;
                else if (regionX > size.X - regionSize)
                    regionX = size.X - regionSize;
                
                var regionY = (io.MousePos.Y - pos.Y + (size.Y + 6)) - regionSize * 0.5f;
                if (regionY < 0.0f)
                    regionY = 0.0f;
                else if (regionY > size.Y - regionSize)
                    regionY = size.Y - regionSize;
                
                var zoom = Game1.ScreenResolutionX / size.X;
                
                var uv0 = new Num.Vector2((regionX) / size.X, (regionY) / size.Y);
                var uv1 = new Num.Vector2((regionX + regionSize) / size.X, (regionY + regionSize) / size.Y);
                
                ImGui.Image(texturePointer, 
                    new Num.Vector2(regionSize * zoom, regionSize * zoom),
                    uv0, uv1,
                    new Num.Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                    new Num.Vector4(1.0f, 1.0f, 1.0f, 0.5f));
                
                ImGui.EndTooltip();
            }
        }

        private void BuildViewportWindow(IntPtr[] viewportTexturePointer)
        {
            var io = ImGui.GetIO();

            ImGui.SetNextWindowSize(new Num.Vector2(200, 100), ImGuiCond.FirstUseEver);
            ImGui.Begin("Viewport", ref _showViewportWindow);

            for (var i = 0; i < viewportTexturePointer.Length; i++)
            {
                var dimension = new Num.Vector2(_viewportRenderSystem.GetViewports()[i].Width,
                                                _viewportRenderSystem.GetViewports()[i].Height);
                
                ImGui.Image(viewportTexturePointer[i], dimension, Num.Vector2.Zero, Num.Vector2.One, Num.Vector4.One, Num.Vector4.Zero);

                var mousePosition = io.MousePos;
                var cursorPosition = ImGui.GetCursorScreenPos();

                var relativeMousePosition = new Vector2(
                    (mousePosition.X - cursorPosition.X) / dimension.X,
                    (mousePosition.Y - cursorPosition.Y + dimension.Y) / dimension.Y);
                
                if (ImGui.IsItemHovered())
                {
                    if (InputManager.Instance.IsInputDown(Input.ViewportZoomIn))
                        _viewportRenderSystem.GetViewports()[i].Zoom *= 1.1f;
                    else if(InputManager.Instance.IsInputDown(Input.ViewportZoomOut))
                        _viewportRenderSystem.GetViewports()[i].Zoom /= 1.1f;

                    if (InputManager.Instance.IsInputPressed(Input.ViewportIncreaseGridSize))
                        _viewportRenderSystem.GetViewports()[i].GridSize *= 2;
                    else if (InputManager.Instance.IsInputPressed(Input.ViewportDecreaseGridSize))
                        _viewportRenderSystem.GetViewports()[i].GridSize /= 2;
                }

                ImGui.Separator();
                
                ImGui.Text("Zoom: " + _viewportRenderSystem.GetViewports()[i].Zoom);
                ImGui.Text("Grid Size: " + _viewportRenderSystem.GetViewports()[i].GridSize);
                ImGui.Text("World Mouse Position: " + _viewportRenderSystem.GetViewports()[i].GetMousePosition(relativeMousePosition));
            }

            ImGui.End();
        }
        
        public override void Initialize(IComponentMapperService mapperService) { }
    }
}
