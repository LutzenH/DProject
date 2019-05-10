using System;
using System.Drawing.Imaging;
using System.IO;
using DProject.Entity;
using DProject.List;
using DProject.Type.Serializable;
using Gdk;
using Gtk;
using Microsoft.Xna.Framework;
using Color = Gdk.Color;

#pragma warning disable 649

namespace DProject
{
    class EditorWindow
    {
        private Game1 _game;
        private Builder _builder;
    
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
        
        //Camera Info Table
        [Builder.Object] private ListStore list_camera_info;
        [Builder.Object] private CellRendererText tree_camera_value;
        
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
        
        public EditorWindow(string[] args)
        {
            Application.Init();
            
            _builder = new Builder();
            _builder.AddFromFile(Game1.RootDirectory + "ui/EditorWindow.glade");
            _builder.Autoconnect(this);
            
            tree_camera_value.Editable = true;
            
            _game = new Game1();
            _gameWidget = _game.Services.GetService<Widget>();
            _gameBox.PackStart(_gameWidget, true, true, 5);
            
            SetupEvents();
            
            FillPropList();
            FillColorList();
            FillTextureList();
            _gameBox.ShowAll();
                        
            Application.Run();
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
            menu_file_save.Activated += (o, args) => _game.GetEntityManager().GetChunkLoaderEntity().SerializeChangedChunks();
            menu_file_revert.Activated += (o, args) => _game.GetEntityManager().GetChunkLoaderEntity().ReloadChangedChunks();
            menu_file_quit.Activated += MainWindow_Destroyed;
            
            //Menu > Edit > ...
            menu_edit_resetcameraposition.Activated += (o, args) => _game.GetEntityManager().GetActiveCamera().SetPosition(new Vector3(0, 0, 0));
            
            //Camera Info Tree
            tree_camera_value.Edited += UpdateCamera;
            
            //Terrain Brush Settings
            adjustment_brush_size.ValueChanged += (o, args) => _game.GetEntityManager().GetWorldEditorEntity().SetBrushSize((int)adjustment_brush_size.Value - 1);
            
            //Colors
            box_flow_colors.SelectedChildrenChanged += UpdateSelectedColor;
            color_apply.Clicked += ChangeSelectedColor;
            color_add.Clicked += AddNewColor;
            search_entry_color.SearchChanged += FilterColorList;
            
            //Texture List
            box_flow_textures.SelectedChildrenChanged += UpdateSelectedTexture;
            
            //Box Prop List
            box_prop_list.SelectedRowsChanged += UpdateSelectedProp;
        }

        private void SetSelectedTool(WorldEditorEntity.Tools tool)
        {
            _game.GetEntityManager().GetWorldEditorEntity().SetCurrentTool(tool);
        }

        private void UpdateCamera(object obj, EditedArgs args)
        {
            TreeIter iter;
            list_camera_info.GetIter (out iter, new TreePath (args.Path));
                        
            try
            {
                list_camera_info.SetValue(iter, 1, Convert.ToSingle(args.NewText));
            }
            catch (FormatException e)
            {
                return;
            }

            Single newValue = (float) list_camera_info.GetValue(iter, 1);

            var currentPosition = _game.GetEntityManager().GetActiveCamera().GetPosition();
            
            switch (args.Path)
            {
                case "0":
                    _game.GetEntityManager().GetActiveCamera().SetPosition(new Vector3(newValue, currentPosition.Y, currentPosition.Z));
                    break;
                case "1":
                    _game.GetEntityManager().GetActiveCamera().SetPosition(new Vector3(currentPosition.X, newValue, currentPosition.Z));
                    break;
                case "2":
                    _game.GetEntityManager().GetActiveCamera().SetPosition(new Vector3(currentPosition.X, currentPosition.Y, newValue));
                    break;
            }
        }

        private void UpdateSelectedColor(object obj, EventArgs args)
        {
            var box = (FlowBox) obj;
            var selectedChild = (FlowBoxChild) box.SelectedChildren[0];

            if (selectedChild != null)
                _colorSelectedChild = selectedChild.TooltipText;

            colorbutton_color.Rgba = Colors.GetRgbaFromName(_colorSelectedChild);
            
            _game.GetEntityManager().GetWorldEditorEntity().SetSelectedColor(Colors.GetColorIdFromName(_colorSelectedChild));
        }

        private void ChangeSelectedColor(object obj, EventArgs args)
        {
            var selectedColor = _game.GetEntityManager().GetWorldEditorEntity().GetSelectedColor();
            Colors.SetColorFromRgba(colorbutton_color.Rgba, selectedColor);

            foreach (var widget in box_flow_colors)
            {
                var child = (FlowBoxChild) widget;
                if (child.TooltipText.Equals(_colorSelectedChild))
                {
                    var image = (Image) child.Children[0];
                    image.ModifyBg(StateType.Normal, new Color((byte) Colors.ColorList[selectedColor].Red, (byte) Colors.ColorList[selectedColor].Green, (byte) Colors.ColorList[selectedColor].Blue));
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

        private void FilterColorList(object obj, EventArgs args)
        {
            var searchEntry = (SearchEntry) obj;
            var text = MakeNameConsistent(searchEntry.Text);

            foreach (var flowBoxColor in box_flow_colors.Children)
            {
                flowBoxColor.Destroy();
            }

            foreach (var color in Colors.ColorList)
            {
                if (color.Value.Name.Contains(text))
                {
                    box_flow_colors.Add(
                        CreateFlowBoxColor(
                            color.Value.Name,
                            (byte) color.Value.Red,
                            (byte) color.Value.Green,
                            (byte) color.Value.Blue)
                    );
                }
            }
            
            box_flow_colors.ShowAll();
        }

        private string MakeNameConsistent(string name)
        {
            return name.Trim().ToLower().Replace(" ", "_");
        }

        private void UpdateSelectedProp(object obj, EventArgs args)
        {
            var box = (ListBox) obj;
            var selectedChild = (ListBoxRow) box.SelectedRows[0];                   
            _game.GetEntityManager().GetWorldEditorEntity().SetSelectedObject(Props.GetPropIdFromName(selectedChild.TooltipText));
        }
        
        private void UpdateSelectedTexture(object obj, EventArgs args)
        {
            var box = (FlowBox) obj;
            var selectedChild = (FlowBoxChild) box.SelectedChildren[0];                   
            _game.GetEntityManager().GetWorldEditorEntity().SetActiveTexture(Textures.GetTextureIdFromName(selectedChild.TooltipText));
        }

        private void MainWindow_Destroyed(object sender, EventArgs e)
        {
            _game.Exit();
            Application.Quit();
        }

        private void FillPropList()
        {
            label_bottom_info.Text = "Current Camera Position: ";
                        
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
            MemoryStream memoryStream = new MemoryStream();
            Textures.TextureAtlas.Save(memoryStream, ImageFormat.Png);
            memoryStream.Position = 0;
            
            var buf = new Pixbuf(memoryStream);
            
            foreach (var texture in Textures.TextureList)
            {
                box_flow_textures.Add(
                    CreateFlowBoxTexture(
                        buf,
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

        private FlowBoxChild CreateFlowBoxColor(string name, byte r, byte g, byte b)
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

        private FlowBoxChild CreateFlowBoxTexture(Pixbuf buf, string name, int xsize, int ysize, int xoffset, int yoffset, int scale)
        {
            FlowBoxChild child = new FlowBoxChild();
            child.HasTooltip = true;
            child.TooltipText = name;
            
            var buffer = new Pixbuf(buf, xoffset, yoffset, xsize, ysize);
            buffer = buffer.ScaleSimple(xsize * scale, ysize * scale, InterpType.Nearest);
                        
            Image image = new Image(buffer);
            image.WidthRequest = xsize*scale;
            image.HeightRequest = ysize*scale;
            
            child.Add(image);

            return child;
        }

        private void UpdateGameResolution()
        {
            _game.SetWidgetOffset(left_pane.AllocatedWidth+5, top_bar.AllocatedHeight);
            _game.SetScreenResolution(_gameWidget.AllocatedWidth, _gameWidget.AllocatedHeight);
        }
    }
}
