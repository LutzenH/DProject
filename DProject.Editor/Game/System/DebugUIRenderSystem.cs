using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly ShaderManager _shaderManager;
        private readonly GraphicsDevice _graphicsDevice;
     
        private readonly ImGuiRenderer _imGuiRenderer;
        private readonly IntPtr[] _imGuiTexture = { IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero };

        private bool _showTestWindow;
        private bool _showRenderBufferWindow;
        private bool _showEntityListWindow;

        public static readonly Dictionary<int, string> EntityIdentifiers = new Dictionary<int, string>();
        private readonly IEnumerable<global::System.Type> _componentTypes;
        
        public DebugUIRenderSystem(GraphicsDevice graphicsDevice, ShaderManager shaderManager) : base(Aspect.Exclude())
        {
            _shaderManager = shaderManager;
            _graphicsDevice = graphicsDevice;
            
            _imGuiRenderer = new ImGuiRenderer(_graphicsDevice);
            _imGuiRenderer.RebuildFontAtlas();
            
            _componentTypes = typeof(IComponent).Assembly.GetTypes().Where(
                t => typeof(IComponent).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
        }

        public override void Draw(GameTime gameTime)
        {
            _imGuiTexture[0] = _imGuiRenderer.BindTexture(_shaderManager.Color);
            _imGuiTexture[1] = _imGuiRenderer.BindTexture(_shaderManager.Depth);
            _imGuiTexture[2] = _imGuiRenderer.BindTexture(_shaderManager.LightInfo);
            _imGuiTexture[3] = _imGuiRenderer.BindTexture(_shaderManager.Normal);
            _imGuiTexture[4] = _imGuiRenderer.BindTexture(_shaderManager.Lights);
            _imGuiTexture[5] = _imGuiRenderer.BindTexture(_shaderManager.SSAO);

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
                
                DisplayImageWithTooltip(_imGuiTexture[0], "Color", new Num.Vector2(300, 150));
                DisplayImageWithTooltip(_imGuiTexture[1], "Depth", new Num.Vector2(300, 150));
                DisplayImageWithTooltip(_imGuiTexture[2], "Light Info", new Num.Vector2(300, 150));
                DisplayImageWithTooltip(_imGuiTexture[3], "Normal", new Num.Vector2(300, 150));
                DisplayImageWithTooltip(_imGuiTexture[4], "Lights", new Num.Vector2(300, 150));
                DisplayImageWithTooltip(_imGuiTexture[5], "SSAO", new Num.Vector2(300, 150));
                
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
                    
                    BuildComponentPropertiesList(entityGetComponent, entity);
                }
            }
            
            #endregion

            ImGui.End();
        }

        private static void BuildComponentPropertiesList(IComponent component, Entity entity)
        {
            var isAlive = true;
            if (component != null && ImGui.CollapsingHeader(component.GetType().Name, ref isAlive))
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
                            ImGui.SliderInt("Type###PropertyPrimitiveType", ref intType, 0, 2, type.ToString());
                            
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

                            property.SetValue(component, floatValue);
                        }
                        else if (propertyValue is Model model)
                        {
                            ImGui.Text("Root: " + model.Root.Name);
                            ImGui.Text("Hash: " + model.GetHashCode());
                        }
                        else if (propertyValue is bool boolean)
                        {
                            var boolValue = boolean;
                            ImGui.Checkbox("###PropertyBooleanValue", ref boolValue);
                            property.SetValue(component, boolValue);
                        }
                        else if (propertyValue is DPModel dpModel)
                        {
                            ImGui.Text("Name: " + dpModel.Name);
                            ImGui.Text("Triangle Count: " + dpModel.PrimitiveCount);
                            ImGui.Text("BoundingSphere: " + dpModel.BoundingSphere);
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
                
                var regionY = (io.MousePos.Y - pos.Y + 156) - regionSize * 0.5f;
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

        public override void Initialize(IComponentMapperService mapperService) { }
    }
}
