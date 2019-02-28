using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

using DProject.Type;
using Microsoft.Xna.Framework;

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
        }
    }

}