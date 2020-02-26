using System;
using System.Collections.Generic;
using System.IO;
using DProject.Type.Serializable;

namespace DProject.List
{
    public static class Models
    {
        private const string FileExtension = ".dpm";
        private const string ModelFolderPath = Game1.RootDirectory + "models/";
        
        public static readonly Dictionary<int, Model> ModelList = new Dictionary<int, Model>();

        static Models()
        {
            var files = Directory.GetFiles(ModelFolderPath, "*" + FileExtension, SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var modelName = Path.GetFileNameWithoutExtension(file);
                var assetPath = file?.Substring(Game1.RootDirectory.Length, file.Length - FileExtension.Length - Game1.RootDirectory.Length);

                var model = new Model()
                {
                    Name = modelName,
                    AssetPath = assetPath
                };

                ModelList.Add(assetPath.GetHashCode(), model);
            }

            Console.WriteLine("Retrieved " + ModelList.Count + " Models from " + ModelFolderPath);
        }
    }
}
