using DProject.Manager.Entity;
using DProject.UI;
using DProject.UI.Handler;

namespace DProject.Manager.UI
{
    public class GameUIManager : UIManager
    {
        private readonly PortsUI _portsUI;

        public PortsEventHandler PortsEventHandler { get; }

        public GameUIManager(EntityManager entityManager) : base(entityManager)
        {
            var gameEntityManager = (GameEntityManager) entityManager;
            
            _portsUI = new PortsUI(gameEntityManager, this);
            AddInterface(_portsUI);
            
            PortsEventHandler = new PortsEventHandler(entityManager, this);
        }

        public PortsUI GetPortsUI()
        {
            return _portsUI;
        }
    }
}
