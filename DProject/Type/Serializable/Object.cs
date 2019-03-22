using System.Collections.Generic;
using DProject.Type.Enum;
using MessagePack;
using Microsoft.Xna.Framework;

namespace DProject.Type.Serializable
{
    [MessagePackObject]
    public class Object
    {
        [Key(0)]
        public virtual ushort Id { get; set; }

        [Key(1)]
        public virtual int PositionX { get; set; }

        [Key(2)]
        public virtual int PositionY { get; set; }

        [Key(3)]
        public virtual Rotation ObjectRotation { get; set; }
        
        [Key(4)]
        public virtual float ScaleX { get; set; }
        [Key(5)]
        public virtual float ScaleY { get; set; }
        [Key(6)]
        public virtual float ScaleZ { get; set; }
        
        [IgnoreMember]
        public Vector3 Scale => new Vector3(ScaleX, ScaleY, ScaleZ);

        public static List<Object>[] GenerateObjects(int startPositionX, int startPositionY, int floorCount, int objectCountPerFloor)
        {
            var objects = new List<Object>[floorCount];

            for (var floor = 0; floor < floorCount; floor++)
            {
                objects[floor] = new List<Object>();
                
                for (var i = 0; i < objectCountPerFloor; i++)
                {
                    objects[floor].Add(new Object()
                    {
                        Id = 0,
                        PositionX = startPositionX + i%32,
                        PositionY = startPositionY + i/32,
                        ObjectRotation = Rotation.North,
                        ScaleX = 1f,
                        ScaleY = 1f,
                        ScaleZ = 1f
                    });
                }
            }

            return objects;
        }

    }
}