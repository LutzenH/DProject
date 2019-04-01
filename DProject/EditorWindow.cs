using System;
using System.Threading;
using DProject.List;
using Gtk;

namespace DProject
{
    class EditorWindow
    {
        private Game1 _game;
          
        private Builder _builder;
    
        [Builder.Object]
        private Box _gameBox;

        [Builder.Object]
        private ListBox box_prop_list;

        [Builder.Object]
        private Label label_bottom_info;
        
        [Builder.Object]
        private Box left_pane;

        [Builder.Object]
        private Box top_bar;
        
        private Widget _gameWidget;
        
        public EditorWindow()
        {
            Application.Init();
            
            _builder = new Builder();
            _builder.AddFromFile("Content/ui/EditorWindow.glade");
            _builder.Autoconnect(this);
            
            _game = new Game1();

            
            
            _gameWidget = _game.Services.GetService<Widget>();
            
            _gameBox.PackStart(_gameWidget, true, true, 5);
            
            _gameWidget.Shown += (o, e) => _game.Run();
            _gameBox.Destroyed += MainWindow_Destroyed;
                         
            FillPropList();
            
            _gameBox.ShowAll();

            _gameWidget.SizeAllocated += (o, args) => UpdateGameResolution();
            _gameBox.ButtonPressEvent += (o, args) => _gameBox.GrabFocus();
            
            _game.SetScreenResolution(_gameWidget.AllocatedWidth, _gameWidget.AllocatedHeight);
            
            Application.Run();
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
