using DProject.Manager.Entity;
using Microsoft.Xna.Framework;

namespace DProject.Entity
{
    public abstract class AbstractAwareEntity : AbstractEntity
    {
        protected readonly EntityManager EntityManager;

        protected AbstractAwareEntity(EntityManager entityManager, Vector3 position, Quaternion rotation, Vector3 scale) : base(position, rotation, scale)
        {
            EntityManager = entityManager;
        }

        protected AbstractAwareEntity(EntityManager entityManager, Vector3 position, float pitch, float yaw, float roll, Vector3 scale) : base(position, pitch, yaw, roll, scale)
        {
            EntityManager = entityManager;
        }
    }
}