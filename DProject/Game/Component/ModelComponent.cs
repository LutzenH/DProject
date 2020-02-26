using DProject.List;

namespace DProject.Game.Component
{
    public class ModelComponent : IComponent
    {
        public int Hash { get; set; }

        public ModelComponent(int hash)
        {
            Hash = hash;
        }
    }
}
