using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DProject.Entity
{
    public abstract class AbstractEntity
    {
        protected Vector3 Position;
        protected Vector3 Scale;
        protected Matrix Rotation;

        protected AbstractEntity(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.Position = position;
            this.Rotation = Matrix.CreateFromQuaternion(rotation);
            this.Scale = scale;
        }
        
        protected AbstractEntity(Vector3 position, float pitch, float yaw, float roll, Vector3 scale)
        {
            this.Position = position;
            this.Rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            this.Scale = scale;
        }

        public abstract void LoadContent(ContentManager content);

        public Vector3 GetPosition()
        {
            return Position;
        }

        public Vector3 GetScale()
        {
            return Scale;
        }

        public void setRotation(float pitch, float yaw, float roll)
        {
            Rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
        }

        public void setRotation(Quaternion rotation)
        {
            Rotation = Matrix.CreateFromQuaternion(rotation);
        }

        public Matrix getWorldMatrix()  
        {   
            return Rotation * Matrix.CreateScale(Scale) *  Matrix.CreateTranslation(Position);   
        }  
    }
}