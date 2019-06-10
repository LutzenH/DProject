using System;
using System.Drawing;
using System.Collections.Generic;
using BitMiracle.LibTiff.Classic;
using DProject.Entity.Chunk;
using DProject.Type.Enum;
using DProject.Type.Rendering;
using DProject.Type.Serializable.Chunk;
using Object = DProject.Type.Serializable.Chunk.Object;

namespace DProject.Type
{
    public static class ImageProcessing
    {
        public static List<ChunkData> GenerateChunkDataUsingPixelRaster(int xPos, int yPos, ushort[,] heightMap)
        {
            return GenerateChunkDataUsingPixelRaster(xPos, yPos, heightMap, null, null, null, null, null);
        }

        public static List<ChunkData> GenerateChunkDataUsingPixelRaster(int xPos, int yPos, ushort[,] heightMap,
            Bitmap splat, ushort? splatColor1Id, ushort? splatColor2Id, ushort? splatColor3Id, ushort? splatColor4Id)
        {
            var list = new List<ChunkData>();

            var hasSplatInfo = (splat != null && splatColor1Id != null && splatColor2Id != null && splatColor3Id != null && splatColor4Id != null);

            var width = heightMap.GetLength(0);
            var height = heightMap.GetLength(1);
            
            if (width % ChunkLoaderEntity.ChunkSize == 1 
                && height % ChunkLoaderEntity.ChunkSize == 1)
            {
                for (var chunkX = 0; chunkX < (width - 1) / ChunkLoaderEntity.ChunkSize; chunkX++)
                {
                    for (var chunkY = 0; chunkY < (height - 1) / ChunkLoaderEntity.ChunkSize; chunkY++)
                    {
                        var subHeightMap = new ushort[ChunkLoaderEntity.ChunkSize + 1, ChunkLoaderEntity.ChunkSize + 1];
                        
                        for (var xPix = 0; xPix < ChunkLoaderEntity.ChunkSize + 1; xPix++)
                        {
                            for (var yPix = 0; yPix < ChunkLoaderEntity.ChunkSize + 1; yPix++)
                            {
                                var xPixel = xPix + ChunkLoaderEntity.ChunkSize * chunkX;
                                var yPixel = yPix + ChunkLoaderEntity.ChunkSize * chunkY;
                                
                                subHeightMap[xPix, yPix] = heightMap[xPixel, yPixel];
                            }
                        }

                        var tiles = new Tile[1][,];

                        if (hasSplatInfo)
                        {
                            var splatMap = new ushort[ChunkLoaderEntity.ChunkSize + 1, ChunkLoaderEntity.ChunkSize + 1];

                            var color1 = (ushort) splatColor1Id;
                            var color2 = (ushort) splatColor2Id;
                            var color3 = (ushort) splatColor3Id;
                            var color4 = (ushort) splatColor4Id;
                            
                            for (var xPix = 0; xPix < ChunkLoaderEntity.ChunkSize + 1; xPix++)
                            {
                                for (var yPix = 0; yPix < ChunkLoaderEntity.ChunkSize + 1; yPix++)
                                {
                                    var xPixel = xPix + ChunkLoaderEntity.ChunkSize * chunkX;
                                    var yPixel = yPix + ChunkLoaderEntity.ChunkSize * chunkY;

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
                            
                            tiles[0] = HeightMap.GenerateTileMap(subHeightMap, splatMap);
                        }
                        else
                            tiles[0] = HeightMap.GenerateTileMap(subHeightMap);

                        var chunkData = new ChunkData()
                        {
                            ChunkPositionX = xPos + chunkX,
                            ChunkPositionY = yPos + chunkY,
                            Tiles = tiles,
                    
                            Objects = Object.GenerateObjects(0, 0, 1, 0),
                    
                            LightingInfo = LightingProperties.DefaultInfo,
                            
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
    }
} 
