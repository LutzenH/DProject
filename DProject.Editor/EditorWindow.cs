using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Timers;
using Cairo;
using DProject.Entity;
using DProject.List;
using DProject.Manager;
using DProject.Type.Serializable;
using Gdk;
using Gtk;
using Microsoft.Xna.Framework;
using Color = Gdk.Color;
using Rectangle = Gdk.Rectangle;

#pragma warning disable 649

namespace DProject
{
    class EditorWindow
    {
        #region GTKWidgets

        //Center Box for the Game
        [Builder.Object] private Box _gameBox;
        private Widget _gameWidget;
        [Builder.Object] private Label label_bottom_info;

        //PropList
        [Builder.Object] private ListBox box_prop_list;

        //Selected Tool RadioButton's
        [Builder.Object] private RadioButton button_tool_select;
        [Builder.Object] private RadioButton button_tool_flatten;
        [Builder.Object] private RadioButton button_tool_raise;
        [Builder.Object] private RadioButton button_tool_paint;
        [Builder.Object] private RadioButton button_tool_objectplacer;

        //Menu > File > ...
        [Builder.Object] private MenuItem menu_file_save;
        [Builder.Object] private MenuItem menu_file_revert;
        [Builder.Object] private MenuItem menu_file_quit;

        //Menu > Edit > ...
        [Builder.Object] private MenuItem menu_edit_resetcameraposition;
        [Builder.Object] private MenuItem menu_edit_import_heightmap;

        //Camera Info Table
        [Builder.Object] private TreeView debug_info_tree_view;
        private Dictionary<string, object> list_debug_info_items;
        
        //Other
        [Builder.Object] private Box left_pane;
        [Builder.Object] private Box top_bar;

        //Terrain Brush Settings
        [Builder.Object] private Adjustment adjustment_brush_size;
        [Builder.Object] private Adjustment adjustment_height;

        //Colors
        [Builder.Object] private FlowBox box_flow_colors;
        [Builder.Object] private ColorButton colorbutton_color;
        [Builder.Object] private Entry entry_color_name;
        [Builder.Object] private Button color_add;
        [Builder.Object] private Button color_apply;
        [Builder.Object] private SearchEntry search_entry_color;
        private string _colorSelectedChild;

        //Textures
        [Builder.Object] private FlowBox box_flow_textures;
        [Builder.Object] private SearchEntry search_entry_texture;
        private Pixbuf _textureAtlasPixBuf;
        private string _textureSelectedChild;

        //Additional Windows/Dialogs
        //Import Heightmap
        [Builder.Object] private Dialog dialog_import_heightmap;
        [Builder.Object] private Button button_import_heightmap_open;
        [Builder.Object] private Button button_import_heightmap_cancel;
        #endregion

        private Game1 _game;
        private EditorEntityManager _editorEntityManager;

        private Builder _builder;
        private Timer _timer;

        public EditorWindow(string[] args)
        {
            Application.Init();

            SetTimer();

            _builder = new Builder();
            _builder.AddFromFile(Game1.RootDirectory + "ui/EditorWindow.glade");
            _builder.Autoconnect(this);
            
            _editorEntityManager = new EditorEntityManager();

            _game = new Game1(_editorEntityManager);
            _gameWidget = _game.Services.GetService<Widget>();
            _gameBox.PackStart(_gameWidget, true, true, 5);

            SetupEvents();

            FillDebugTreeView();
            FillPropList();
            FillColorList();
            FillTextureList();
            _gameBox.ShowAll();

            Application.Run();
        }

        private void SetTimer()
        {
            // Create a timer with a two second interval.
            _timer = new Timer(1000);
            // Hook up the Elapsed event for the timer. 
            _timer.Elapsed += (sender, args) => FetchGameInfo();
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void SetupEvents()
        {
            _gameWidget.Shown += (o, e) => _game.Run();
            _gameWidget.SizeAllocated += (o, args) => UpdateGameResolution();

            _gameBox.Destroyed += MainWindow_Destroyed;
            _gameBox.ButtonPressEvent += (o, args) => _gameBox.GrabFocus();

            //Selected Tool RadioButton's
            button_tool_select.Pressed += (o, args) => SetSelectedTool(WorldEditorEntity.Tools.Select);
            button_tool_flatten.Pressed += (o, args) => SetSelectedTool(WorldEditorEntity.Tools.Flatten);
            button_tool_raise.Pressed += (o, args) => SetSelectedTool(WorldEditorEntity.Tools.Raise);
            button_tool_paint.Pressed += (o, args) => SetSelectedTool(WorldEditorEntity.Tools.Paint);
            button_tool_objectplacer.Pressed += (o, args) => SetSelectedTool(WorldEditorEntity.Tools.ObjectPlacer);

            //Menu > File > ...
            menu_file_save.Activated += EditorSaveGame;
            menu_file_revert.Activated +=
                (o, args) => _game.GetEntityManager().GetChunkLoaderEntity().ReloadChangedChunks();
            menu_file_quit.Activated += MainWindow_Destroyed;

            //Menu > Edit > ...
            menu_edit_resetcameraposition.Activated += (o, args) =>
                _game.GetEntityManager().GetActiveCamera().SetPosition(new Vector3(0, 0, 0));
            menu_edit_import_heightmap.Activated += (o, args) => dialog_import_heightmap.Show();

            //Terrain Brush Settings
            adjustment_brush_size.ValueChanged += (o, args) =>
                _editorEntityManager.GetWorldEditorEntity().SetBrushSize((int) adjustment_brush_size.Value - 1);

            //Colors
            box_flow_colors.SelectedChildrenChanged +=
                (o, args) => UpdateSelectedItem(o, args, typeof(SerializableColor));
            search_entry_color.SearchChanged += (o, args) => FilterItemList(o, args, box_flow_colors, Colors.ColorList);
            color_apply.Clicked += ChangeSelectedColor;
            color_add.Clicked += AddNewColor;

            //Texture List
            box_flow_textures.SelectedChildrenChanged += (o, args) => UpdateSelectedItem(o, args, typeof(Texture));
            search_entry_texture.SearchChanged +=
                (o, args) => FilterItemList(o, args, box_flow_textures, Textures.TextureList);

            //Box Prop List
            box_prop_list.SelectedRowsChanged += UpdateSelectedProp;
        }

        private void SetSelectedTool(WorldEditorEntity.Tools tool)
        {
            _editorEntityManager.GetWorldEditorEntity().SetCurrentTool(tool);
        }

        private void EditorSaveGame(object obj, EventArgs args)
        {
            Colors.ExportColorListToJson();
            _game.GetEntityManager().GetChunkLoaderEntity().SerializeChangedChunks();
        }

        private void UpdateSelectedItem(object obj, EventArgs args, System.Type type)
        {
            var box = (FlowBox) obj;
            var selectedChild = (FlowBoxChild) box.SelectedChildren[0];

            if (type == typeof(Texture))
            {
                if (selectedChild != null)
                    _textureSelectedChild = selectedChild.TooltipText;

                _editorEntityManager.GetWorldEditorEntity()
                    .SetActiveTexture(Textures.GetTextureIdFromName(_textureSelectedChild));
            }
            else if (type == typeof(SerializableColor))
            {
                if (selectedChild != null)
                    _colorSelectedChild = selectedChild.TooltipText;

                colorbutton_color.Rgba = GetRgbaFromName(_colorSelectedChild);

                _editorEntityManager.GetWorldEditorEntity()
                    .SetSelectedColor(Colors.GetColorIdFromName(_colorSelectedChild));
            }
        }

        private void ChangeSelectedColor(object obj, EventArgs args)
        {
            var selectedColor = _editorEntityManager.GetWorldEditorEntity().GetSelectedColor();
            SetColorFromRgba(colorbutton_color.Rgba, selectedColor);

            foreach (var widget in box_flow_colors)
            {
                var child = (FlowBoxChild) widget;
                if (child.TooltipText.Equals(_colorSelectedChild))
                {
                    var image = (Image) child.Children[0];
                    image.ModifyBg(StateType.Normal,
                        new Color((byte) Colors.ColorList[selectedColor].Red,
                            (byte) Colors.ColorList[selectedColor].Green, (byte) Colors.ColorList[selectedColor].Blue));
                }
            }
        }

        private void AddNewColor(object obj, EventArgs args)
        {
            var name = MakeNameConsistent(entry_color_name.Text);

            if (name.Equals(""))
                return;

            var alreadyExists = false;

            foreach (var child in box_flow_colors.Children)
            {
                if (child.TooltipText.Equals(name))
                {
                    box_flow_colors.SelectChild((FlowBoxChild) child);
                    alreadyExists = true;
                }
            }

            if (!alreadyExists)
            {
                if (Colors.ColorList.Count < ushort.MaxValue)
                {
                    var color = new SerializableColor()
                    {
                        Name = name,
                        Red = (byte) (colorbutton_color.Rgba.Red * 255),
                        Green = (byte) (colorbutton_color.Rgba.Green * 255),
                        Blue = (byte) (colorbutton_color.Rgba.Blue * 255)
                    };

                    Colors.ColorList.Add((ushort) Colors.ColorList.Count, color);

                    box_flow_colors.Add(
                        CreateFlowBoxColor(
                            color.Name,
                            (byte) color.Red,
                            (byte) color.Green,
                            (byte) color.Blue)
                    );

                    box_flow_colors.ShowAll();
                }
                else
                {
                    throw new OverflowException("ColorList already contains the maximum amount of colors.");
                }
            }

            entry_color_name.Text = name;
        }

        private void FilterItemList<T>(object obj, EventArgs args, FlowBox flowBox, Dictionary<ushort, T> list)
        {
            var searchEntry = (SearchEntry) obj;
            var text = MakeNameConsistent(searchEntry.Text);

            foreach (var flowBoxChild in flowBox.Children)
            {
                flowBoxChild.Destroy();
            }

            foreach (var item in list)
            {
                if (item.Value.GetType() == typeof(SerializableColor))
                {
                    var color = item.Value as SerializableColor;

                    if (color.Name.Contains(text))
                    {
                        flowBox.Add(
                            CreateFlowBoxColor(
                                color.Name,
                                (byte) color.Red,
                                (byte) color.Green,
                                (byte) color.Blue)
                        );
                    }
                }
                else if (item.Value.GetType() == typeof(Texture))
                {
                    var texture = item.Value as Texture;

                    if (texture.TextureName.Contains(text))
                    {
                        flowBox.Add(
                            CreateFlowBoxTexture(
                                _textureAtlasPixBuf,
                                texture.TextureName,
                                texture.XSize,
                                texture.YSize,
                                texture.XOffset,
                                texture.YOffset,
                                2
                            )
                        );
                    }
                }
            }

            flowBox.ShowAll();
        }

        private static string MakeNameConsistent(string name)
        {
            return name.Trim().ToLower().Replace(" ", "_");
        }

        private void UpdateSelectedProp(object obj, EventArgs args)
        {
            var box = (ListBox) obj;
            var selectedChild = (ListBoxRow) box.SelectedRows[0];
            _editorEntityManager.GetWorldEditorEntity()
                .SetSelectedObject(Props.GetPropIdFromName(selectedChild.TooltipText));
        }

        private void MainWindow_Destroyed(object sender, EventArgs e)
        {
            _game.Exit();
            Application.Quit();
        }

        private void FillPropList()
        {
            foreach (var prop in Props.PropList)
            {
                var prop_list_row = new ListBoxRow();
                var text_label = new Label(prop.Value.GetAssetPath());

                text_label.Halign = Align.Start;
                prop_list_row.Child = text_label;
                prop_list_row.TooltipText = prop.Value.Name;

                box_prop_list.Add(prop_list_row);
            }

            box_prop_list.ShowAll();
        }

        private void FillColorList()
        {
            foreach (var color in Colors.ColorList)
            {
                box_flow_colors.Add(
                    CreateFlowBoxColor(
                        color.Value.Name,
                        (byte) color.Value.Red,
                        (byte) color.Value.Green,
                        (byte) color.Value.Blue)
                );
            }

            box_flow_colors.ShowAll();
        }

        private void FillTextureList()
        {
            SetupTextureAtlasPixBuf();

            foreach (var texture in Textures.TextureList)
            {
                box_flow_textures.Add(
                    CreateFlowBoxTexture(
                        _textureAtlasPixBuf,
                        texture.Value.TextureName,
                        texture.Value.XSize,
                        texture.Value.YSize,
                        texture.Value.XOffset,
                        texture.Value.YOffset,
                        2
                    ));
            }

            box_flow_textures.ShowAll();
        }

        private void SetupTextureAtlasPixBuf()
        {
            MemoryStream memoryStream = new MemoryStream();
            Textures.TextureAtlas.Save(memoryStream, ImageFormat.Png);
            memoryStream.Position = 0;

            _textureAtlasPixBuf = new Pixbuf(memoryStream);
        }

        private static FlowBoxChild CreateFlowBoxColor(string name, byte r, byte g, byte b)
        {
            FlowBoxChild child = new FlowBoxChild();
            child.HasTooltip = true;
            child.TooltipText = name;

            Image image = new Image();

            image.ModifyBg(StateType.Normal, new Color(r, g, b));
            image.WidthRequest = 25;
            image.HeightRequest = 25;
            child.Add(image);

            return child;
        }

        private static FlowBoxChild CreateFlowBoxTexture(Pixbuf buf, string name, int xsize, int ysize, int xoffset,
            int yoffset, int scale)
        {
            FlowBoxChild child = new FlowBoxChild();
            child.HasTooltip = true;
            child.TooltipText = name;

            var buffer = new Pixbuf(buf, xoffset, yoffset, xsize, ysize);
            buffer = buffer.ScaleSimple(xsize * scale, ysize * scale, InterpType.Nearest);

            Image image = new Image(buffer);
            image.WidthRequest = xsize * scale;
            image.HeightRequest = ysize * scale;

            child.Add(image);

            return child;
        }

        private void UpdateGameResolution()
        {
            _game.SetWidgetOffset(left_pane.AllocatedWidth + 5, top_bar.AllocatedHeight);
            _game.SetScreenResolution(_gameWidget.AllocatedWidth, _gameWidget.AllocatedHeight);
        }

        private void FillDebugTreeView()
        {
            list_debug_info_items = new Dictionary<string, object>
            {
                {"camera_position_x", 0.00f},
                {"camera_position_y", 0.00f},
                {"camera_position_z", 0.00f}
            };

            var keyColumn = new TreeViewColumn { Title = "Key" };
            var keyCell = new CellRendererText ();
            keyColumn.PackStart (keyCell, true);

            var valueColumn = new TreeViewColumn { Title = "Value" };
            var valueCell = new VariableCellRenderer();
            valueColumn.PackStart (valueCell, true);

            var listDebugInfo = new ListStore(typeof(string), typeof(object));
            
            foreach (var debugItem in list_debug_info_items)
                listDebugInfo.AppendValues(debugItem.Key);

            keyColumn.SetCellDataFunc (keyCell, RenderDebugTreeKeyText);
            valueColumn.SetCellDataFunc (valueCell, RenderDebugTreeValue);
            
            debug_info_tree_view.Model = listDebugInfo;
            debug_info_tree_view.AppendColumn(keyColumn);
            debug_info_tree_view.AppendColumn(valueColumn);
        }

        private class VariableCellRenderer : CellRenderer
        {
            internal enum CellRendererType { Text, Toggle, Spin, Combo };

            public static CellRendererType RendererType = CellRendererType.Text;
            
            private readonly CellRendererText _textRenderer;
            private readonly CellRendererToggle _toggleRenderer;
            private readonly CellRendererSpin _spinRenderer;
            private readonly CellRendererCombo _comboRenderer;

            private readonly Adjustment _spinnerAdjustment;

            public VariableCellRenderer()
            {
                _textRenderer = new CellRendererText { Editable = true };
                _spinRenderer = new CellRendererSpin { Editable = true };
                _toggleRenderer = new CellRendererToggle { Activatable = true };
                _comboRenderer = new CellRendererCombo { HasEntry = false, TextColumn = 0, Editable = true };
                
                _spinnerAdjustment = new Adjustment(0d, float.MinValue, float.MaxValue, 1d, 10d, 0d);
                _spinRenderer.Adjustment = _spinnerAdjustment;
            }

            protected override void OnRender(Context cr, Widget widget, Rectangle background_area, Rectangle cell_area, CellRendererState flags)
            {
                switch (RendererType)
                {
                    case CellRendererType.Text:
                        _textRenderer.Render(cr, widget, background_area, cell_area, flags);
                        break;
                    case CellRendererType.Toggle:
                        _toggleRenderer.Render(cr, widget, background_area, cell_area, flags);
                        break;
                    case CellRendererType.Spin:
                        _spinRenderer.Render(cr, widget, background_area, cell_area, flags);
                        break;
                    case CellRendererType.Combo:
                        _comboRenderer.Render(cr, widget, background_area, cell_area, flags);
                        break;
                    default:
                        base.OnRender(cr, widget, background_area, cell_area, flags);
                        break;
                }
            }

            public CellRendererToggle GetCellRendererToggle()
            {
                RendererType = CellRendererType.Toggle;
                return _toggleRenderer;
            }

            public CellRendererText GetCellRendererText()
            {
                RendererType = CellRendererType.Text;
                return _textRenderer;
            }

            public CellRendererSpin GetCellRendererSpin()
            {
                RendererType = CellRendererType.Spin;
                return _spinRenderer;
            }

            public CellRendererCombo GetCellRendererCombo()
            {
                RendererType = CellRendererType.Combo;
                return _comboRenderer;
            }
        }

        private void RenderDebugTreeKeyText (TreeViewColumn column, CellRenderer cell, ITreeModel model, TreeIter iter)
        {
            var item = (string) model.GetValue (iter, 0);
            ((CellRendererText) cell).Text = item;
        }
 
        private void RenderDebugTreeValue (TreeViewColumn column, CellRenderer cell, ITreeModel model, TreeIter iter)
        {
            var item = (string) model.GetValue (iter, 0);

            var value = list_debug_info_items[item];
            
            switch (System.Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    ((VariableCellRenderer) cell).GetCellRendererToggle().Active = (bool) value;
                    break;
                case TypeCode.Single:
                    ((VariableCellRenderer) cell).GetCellRendererSpin().Adjustment.Value = (float) value;
                    break;
                case TypeCode.String:
                    ((VariableCellRenderer) cell).GetCellRendererText().Text = value.ToString();
                    break;
            }
        }

        private void FetchGameInfo()
        {
            label_bottom_info.Text = "FPS: " + _game.GetFPS();

            list_debug_info_items["camera_position_x"] = _game.GetEntityManager().GetActiveCamera().GetPosition().X;
            list_debug_info_items["camera_position_y"] = _game.GetEntityManager().GetActiveCamera().GetPosition().Y;
            list_debug_info_items["camera_position_z"] = _game.GetEntityManager().GetActiveCamera().GetPosition().Z;
            
            debug_info_tree_view.QueueDraw();
        }

        public static RGBA GetRgbaFromName(string name)
        {
            foreach (var color in Colors.ColorList)
            {
                if (color.Value.GetColorName() == name)
                    return new RGBA()
                    {
                        Red = color.Value.Red/256d,
                        Green = color.Value.Green/256d,
                        Blue = color.Value.Blue/256d,
                        Alpha = 1d
                    };
            }
            
            throw new ArgumentException();
        }
        
        private static void SetColorFromRgba(RGBA rgba, ushort colorId)
        {
            Colors.ColorList[colorId].Color = new Microsoft.Xna.Framework.Color((byte) (rgba.Red * 255), (byte) (rgba.Green * 255), (byte) (rgba.Blue * 255));
        }
        
        public static void SetColorFromRgba(RGBA rgba, string name)
        {
            SetColorFromRgba(rgba, Colors.GetColorIdFromName(name));
        }
    }
}
