using DProject.List;

namespace DProject.Game.Component
{
    public class ModelComponent : IComponent
    {
        public string ModelPath { get; set; }

        public ModelComponent(string path)
        {
            ModelPath = path;
        }

        public ModelComponent(ushort id)
        {
            ModelPath = Props.PropList[id].AssetPath;
        }
    }
}
