using System.Collections.Generic;
using DProject.Type;

namespace DProject.List
{
    public static class Props
    {
        public static readonly Dictionary<ushort, Prop> PropList = new Dictionary<ushort, Prop>();

        static Props()
        {
            PropList[0] = new Prop("models/barrel");
            PropList[1] = new Prop("models/book");
            PropList[2] = new Prop("models/cube");
            PropList[3] = new Prop("models/factory");
            PropList[4] = new Prop("models/plane");
            PropList[5] = new Prop("models/camera");
        }
        
        public static ushort GetDefaultPropId()
        {
            return 0;
        }
    }

}