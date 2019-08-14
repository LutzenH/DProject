using DProject.List;
using Microsoft.Xna.Framework.Graphics;

namespace DProject.Game.Component
{
    public class ModelComponent
    {
        private string _modelPath;
        private Model _model;
        
        public ModelComponent(string path)
        {
            _modelPath = path;
        }

        public ModelComponent(ushort id)
        {
            _modelPath = Props.PropList[id].AssetPath;
        }
        
        public Model Model
        {
            get => _model;
            set => _model = value;
        }

        public string ModelPath
        {
            get => _modelPath;
            set => _modelPath = value;
        }
    }
}
