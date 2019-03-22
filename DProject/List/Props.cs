using System.Collections.Generic;
using DProject.Type;

namespace DProject.List
{
    public static class Props
    {
        public static readonly Dictionary<ushort, Prop> PropList = new Dictionary<ushort, Prop>();

        static Props()
        {
            #region Debug
                        
            PropList[0] = new Prop("models/barrel");
            PropList[1] = new Prop("models/book");
            PropList[2] = new Prop("models/cube");
            PropList[3] = new Prop("models/factory");
            PropList[4] = new Prop("models/plane");
            PropList[5] = new Prop("models/camera");

            #endregion

            #region Walls

            //Plank
            PropList[6] = new Prop("models/structures/walls/plank_wall_default");
            PropList[7] = new Prop("models/structures/walls/plank_wall_doorframe");
            PropList[8] = new Prop("models/structures/walls/plank_wall_outside_corner");
            PropList[9] = new Prop("models/structures/walls/plank_wall_window");
            PropList[10] = new Prop("models/structures/walls/plank_wall_window_closed");
            
            //Plaster
            PropList[11] = new Prop("models/structures/walls/plaster_wall_default");
            PropList[12] = new Prop("models/structures/walls/plaster_wall_default_plinted");
            PropList[13] = new Prop("models/structures/walls/plaster_wall_doorframe");
            PropList[14] = new Prop("models/structures/walls/plaster_wall_double_l");
            PropList[15] = new Prop("models/structures/walls/plaster_wall_double_r");
            PropList[16] = new Prop("models/structures/walls/plaster_wall_inside_corner");
            
            #endregion          
        }
        
        public static ushort GetDefaultPropId()
        {
            return 0;
        }
    }

}