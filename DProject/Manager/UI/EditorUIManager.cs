#if EDITOR

using DProject.Manager.Entity;
using DProject.UI;

namespace DProject.Manager.UI
{
    public class EditorUIManager : UIManager
    {
        public EditorUIManager(EntityManager entityManager) : base(entityManager)
        {
            var editorEntityManager = (EditorEntityManager) entityManager;
        }
    }
}

#endif
