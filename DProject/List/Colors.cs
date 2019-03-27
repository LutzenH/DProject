using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DProject.List
{
    public static class Colors
    {
        public static readonly Dictionary<ushort, Color> ColorList = new Dictionary<ushort, Color>();

        static Colors()
        {
            #region Default

            ColorList[0] = Color.White;
            ColorList[1] = Color.Black;
            ColorList[2] = Color.Red;
            ColorList[3] = Color.Green;
            ColorList[4] = Color.Blue;
            ColorList[5] = Color.Gray;

            #endregion
            
            #region Hills
            
            ColorList[6] = new Color(194, 134, 68);
            ColorList[7] = new Color(136, 132, 53);
            ColorList[8] = new Color(127, 140, 57);
            
            #endregion
        }
        
        public static ushort GetDefaultColorId()
        {
            return 0;
        }
    }

}