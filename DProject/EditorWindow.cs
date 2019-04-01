using System;
using DProject.Entity;
using DProject.List;
using Gtk;
using Microsoft.Xna.Framework;

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
        
        public EditorWindow()
        {
            Application.Init();
            
            _builder = new Builder();
            _builder.AddFromFile("Content/ui/EditorWindow.glade");
            _builder.Autoconnect(this);
            
            tree_camera_value.Editable = true;
            
            _game = new Game1();
            _gameWidget = _game.Services.GetService<Widget>();
            _gameBox.PackStart(_gameWidget, true, true, 5);
            
            SetupEvents();
            
            FillPropList();
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
                var text_label = new Label(prop.Key + " " + prop.Value.GetAssetPath());

                text_label.Halign = Align.Start;
                prop_list_row.Child = text_label;
            
                box_prop_list.Add(prop_list_row);
            }
            
            box_prop_list.ShowAll();
        }

        private void FillColorList()
        {
            
        }

        private void UpdateGameResolution()
        {
            _game.SetWidgetOffset(left_pane.AllocatedWidth+5, top_bar.AllocatedHeight);
            _game.SetScreenResolution(_gameWidget.AllocatedWidth, _gameWidget.AllocatedHeight);
        }
    }
}
