using System;
using System.Collections.Generic;
using DProject.Game.Component;
using DProject.Game.Component.Lighting;
using DProject.Game.Component.Physics;
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

        private readonly ShaderManager _shaderManager;
        private readonly GraphicsDevice _graphicsDevice;
     
        private readonly ImGuiRenderer _imGuiRenderer;
        private readonly IntPtr[] _imGuiTexture = { IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero };

        private bool _showTestWindow;
        private bool _showRenderBufferWindow;
        private bool _showEntityListWindow;

        public static Dictionary<int, string> EntityIdentifiers = new Dictionary<int, string>();
        
        public DebugUIRenderSystem(GraphicsDevice graphicsDevice, ShaderManager shaderManager) : base(Aspect.Exclude())
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
            if (ImGui.Button("Entity List")) _showEntityListWindow = !_showEntityListWindow;
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
        }

        private void BuildComponentListWindow(int selectedEntity)
        {
            var entity = GetEntity((int) SelectedEntity);

            #region ComponentListBuilder
            
            //TODO: Find another way to loop over every possible Component. 
            if (entity.Has<TransformComponent>())
            {
                var component = entity.Get<TransformComponent>();
                BuildComponentPropertiesList(component);
            }

            if (entity.Has<ActivePhysicsComponent>())
            {
                var component = entity.Get<ActivePhysicsComponent>();
                BuildComponentPropertiesList(component);
            }
            
            if (entity.Has<StaticPhysicsComponent>())
            {
                var component = entity.Get<StaticPhysicsComponent>();
                BuildComponentPropertiesList(component);
            }
            
            if (entity.Has<LoadedModelComponent>())
            {
                var component = entity.Get<LoadedModelComponent>();
                BuildComponentPropertiesList(component);
            }
            
            if (entity.Has<PrimitiveComponent>())
            {
                var component = entity.Get<PrimitiveComponent>();
                BuildComponentPropertiesList(component);
            }
            
            if (entity.Has<LensComponent>())
            {
                var component = entity.Get<LensComponent>();
                BuildComponentPropertiesList(component);
            }
            
            if (entity.Has<DirectionalLightComponent>())
            {
                var component = entity.Get<DirectionalLightComponent>();
                BuildComponentPropertiesList(component);
            }
            
            if (entity.Has<PointLightComponent>())
            {
                var component = entity.Get<PointLightComponent>();
                BuildComponentPropertiesList(component);
            }

            #endregion

            ImGui.End();
        }

        private static void BuildComponentPropertiesList(object component)
        {
            if (component != null && ImGui.CollapsingHeader(component.GetType().Name))
            {
                foreach (var property in component.GetType().GetProperties())
                {
                    var propertyValue = property.GetValue(component, null);

                    if (ImGui.TreeNode(property.Name))
                    {
                        if (propertyValue is Vector3 vector)
                        {
                            var x = vector.X;
                            var y = vector.Y;
                            var z = vector.Z;
                            
                            ImGui.InputFloat("x###" + "PropertyVector3X", ref x);
                            ImGui.InputFloat("y###" + "PropertyVector3Y", ref y);
                            ImGui.InputFloat("z###" + "PropertyVector3Z", ref z);
                            
                            propertyValue = new Vector3(x, y, z);
                            property.SetValue(component, propertyValue);
                        }
                        else if (propertyValue is Quaternion quaternion)
                        {
                            var x = quaternion.X;
                            var y = quaternion.Y;
                            var z = quaternion.Z;
                            var w = quaternion.W;
                            
                            ImGui.InputFloat("x###" + "PropertyQuaternionX", ref x);
                            ImGui.InputFloat("y###" + "PropertyQuaternionY", ref y);
                            ImGui.InputFloat("z###" + "PropertyQuaternionZ", ref z);
                            ImGui.InputFloat("w###" + "PropertyQuaternionW", ref w);
                            
                            propertyValue = new Quaternion(x, y, z, w);
                            property.SetValue(component, propertyValue);
                        }
                        else if (propertyValue is PrimitiveType type)
                        {
                            var intType = (int) type;
                            ImGui.SliderInt("Type###PropertyPrimitiveType", ref intType, 0, 1, type.ToString());
                            
                            propertyValue = (PrimitiveType) intType;
                            property.SetValue(component, propertyValue);
                        }
                        else if (propertyValue is Color color)
                        {
                            var vec4Color = color.ToVector4();
                            var inColor = new global::System.Numerics.Vector4(vec4Color.X, vec4Color.Y, vec4Color.Z, vec4Color.W);

                            ImGui.ColorEdit4("Color###PropertyColor", ref inColor);
                            
                            propertyValue = new Color(inColor.X, inColor.Y, inColor.Z, inColor.W);
                            property.SetValue(component, propertyValue);
                        }
                        else if (propertyValue is float value)
                        {
                            var floatValue = value;
                            
                            ImGui.InputFloat("value###" + "PropertyFloatValue", ref floatValue);

                            propertyValue = floatValue;
                            property.SetValue(component, propertyValue);
                        }
                        else if (propertyValue is Model model)
                        {
                            ImGui.Text("Root: " + model.Root.Name);
                            ImGui.Text("Hash: " + model.GetHashCode());
                        }
                        else
                        {
                            ImGui.Text(propertyValue.ToString());
                        }

                        ImGui.TreePop();
                    }
                }   
            }
        }

        public override void Initialize(IComponentMapperService mapperService) { }
    }
}
