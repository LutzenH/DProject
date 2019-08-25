using System;
using System.Drawing;
using System.Collections.Generic;
using BitMiracle.LibTiff.Classic;
using DProject.List;
using DProject.Manager.System;
using DProject.Type.Enum;
using DProject.Type.Serializable.Chunk;
using Microsoft.Xna.Framework;
using Object = DProject.Type.Serializable.Chunk.Object;

namespace DProject.Type
{
    public static class ImageProcessing
    {
        public static List<ChunkData> GenerateChunkDataUsingPixelRaster(int xPos, int yPos, ushort[,] heightMap)
        {
            return GenerateChunkDataWithPixelRaster(xPos, yPos, heightMap, null, null, null, null, null);
        }

        public static List<ChunkData> GenerateChunkDataUsingPixelRaster(int xPos, int yPos, ushort[,] heightMap,
            Bitmap splat, ushort splatColor1Id, ushort splatColor2Id, ushort splatColor3Id, ushort splatColor4Id)
        {
            return GenerateChunkDataWithPixelRaster(xPos, yPos, heightMap, splat, splatColor1Id, splatColor2Id, splatColor3Id, splatColor4Id);
        }

        private static List<ChunkData> GenerateChunkDataWithPixelRaster(int xPos, int yPos, ushort[,] heightMap,
            Bitmap splat, ushort? splatColor1Id, ushort? splatColor2Id, ushort? splatColor3Id, ushort? splatColor4Id)
        {
            var list = new List<ChunkData>();

            var hasSplatInfo = (splat != null && splatColor1Id != null && splatColor2Id != null && splatColor3Id != null && splatColor4Id != null);

            var width = heightMap.GetLength(0);
            var height = heightMap.GetLength(1);
            
            if (width % ChunkLoaderSystem.ChunkSize == 1 
                && height % ChunkLoaderSystem.ChunkSize == 1)
            {
                for (var chunkX = 0; chunkX < (width - 1) / ChunkLoaderSystem.ChunkSize; chunkX++)
                {
                    for (var chunkY = 0; chunkY < (height - 1) / ChunkLoaderSystem.ChunkSize; chunkY++)
                    {
                        var subHeightMap = new ushort[ChunkLoaderSystem.ChunkSize + 1, ChunkLoaderSystem.ChunkSize + 1];
                        
                        for (var xPix = 0; xPix < ChunkLoaderSystem.ChunkSize + 1; xPix++)
                        {
                            for (var yPix = 0; yPix < ChunkLoaderSystem.ChunkSize + 1; yPix++)
                            {
                                var xPixel = xPix + ChunkLoaderSystem.ChunkSize * chunkX;
                                var yPixel = yPix + ChunkLoaderSystem.ChunkSize * chunkY;
                                
                                subHeightMap[xPix, yPix] = heightMap[xPixel, yPixel];
                            }
                        }

                        Vertex[,] vertexMap;

                        if (hasSplatInfo)
                        {
                            var splatMap = new ushort[ChunkLoaderSystem.ChunkSize + 1, ChunkLoaderSystem.ChunkSize + 1];

                            var color1 = (ushort) splatColor1Id;
                            var color2 = (ushort) splatColor2Id;
                            var color3 = (ushort) splatColor3Id;
                            var color4 = (ushort) splatColor4Id;
                            
                            for (var xPix = 0; xPix < ChunkLoaderSystem.ChunkSize + 1; xPix++)
                            {
                                for (var yPix = 0; yPix < ChunkLoaderSystem.ChunkSize + 1; yPix++)
                                {
                                    var xPixel = xPix + ChunkLoaderSystem.ChunkSize * chunkX;
                                    var yPixel = yPix + ChunkLoaderSystem.ChunkSize * chunkY;

                                    var pixel = splat.GetPixel(xPixel, yPixel);
                                    
                                    splatMap[xPix, yPix] = color1;

                                    var value = 0;

                                    if (pixel.R > value)
                                    {
                                        value = pixel.R;
                                        splatMap[xPix, yPix] = color2;
                                    }
                                    if (pixel.G > value)
                                    {
                                        value = pixel.G;
                                        splatMap[xPix, yPix] = color3;
                                    }
                                    if (pixel.B > value)
                                    {
                                        splatMap[xPix, yPix] = color4;
                                    }
                                }
                            }
                            
                            vertexMap = HeightmapLoaderSystem.GenerateVertexMap(subHeightMap, splatMap);
                        }
                        else
                            vertexMap = HeightmapLoaderSystem.GenerateVertexMap(subHeightMap);

                        var chunkData = new ChunkData()
                        {
                            ChunkPositionX = xPos + chunkX,
                            ChunkPositionY = yPos + chunkY,
                            VertexMap = vertexMap,
                    
                            Objects = Object.GenerateObjects(0, 0, 0),
                    
                            SkyId = Skies.GetDefaultSkyId(),
                            
                            ChunkStatus = ChunkStatus.Unserialized
                        };
                        
                        list.Add(chunkData);
                    }
                }

                return list;
            }

            return null;
        }

        public static ushort[,] GetPixelRasterFrom16BitTiff(Tiff image)
        {
            if (image == null)
                return null;
            
            var value = image.GetField(TiffTag.IMAGEWIDTH);
            var width = value[0].ToInt();

            value = image.GetField(TiffTag.IMAGELENGTH);
            var height = value[0].ToInt();

            var scanLines = new ushort[height][];
            var byteScanLine = new byte[image.ScanlineSize()];

            var pixelRaster = new ushort[width, height];

            for (var i = 0; i < height; i++)
            {
                scanLines[i] = new ushort[image.ScanlineSize() / 2];
                
                image.ReadScanline(byteScanLine, i);
                ScanLineAs16BitSamples(byteScanLine, scanLines[i]);

                for (var j = 0; j < image.ScanlineSize()/2; j++)
                {
                    pixelRaster[j, i] = scanLines[i][j];
                }
            }

            return pixelRaster;
        }
        
        private static void ScanLineAs16BitSamples(byte[] scanLine, ushort[] temp)
        {
            if (scanLine.Length % 2 != 0)
                throw new ArgumentException();

            Buffer.BlockCopy(scanLine, 0, temp, 0, scanLine.Length);
        }

        public static ushort[,] GetPixelRasterFrom16BitTiff(string imagePath)
        {
            var image = Tiff.Open(imagePath, "r");
            
            return GetPixelRasterFrom16BitTiff(image);
        }
        
        private static void ExportNormalMapToImage(Vector3[,] normals, string filepath)
        {
            var bitmap = new Bitmap(normals.GetLength(0), normals.GetLength(1));

            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    var color = System.Drawing.Color.FromArgb(
                        (int)((normals[x,y].X+1f)*127),
                        (int)((normals[x,y].Y+1f)*127),
                        (int)((normals[x,y].Z+1f)*127));
                    bitmap.SetPixel(x,y, color);
                } 
            }
            
            bitmap.Save(filepath);
        }
    }
} 
