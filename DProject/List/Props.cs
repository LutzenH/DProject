using System.Collections.Generic;
using DProject.Type;

namespace DProject.List
{
    public static class Props
    {
        public static readonly Dictionary<string, Prop> PropList = new Dictionary<string, Prop>();

        static Props()
        {
            PropList["barrel"] = new Prop("models/barrel");
            PropList["book"] = new Prop("models/book");
            PropList["cube"] = new Prop("models/cube");
            PropList["factory"] = new Prop("models/factory");
            PropList["plane"] = new Prop("models/plane");
            PropList["camera"] = new Prop("models/camera");
        }
    }

}