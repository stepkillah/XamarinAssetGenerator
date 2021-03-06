﻿using System.Threading.Tasks;

namespace AssetGenerator.IconsGeneration
{
    public interface IAssetGenerator
    {
        Task CreateAsset(string filepath, string filename, string destinationDirectory, int quality,
            string postfix = default);

        Task CreateIcon(string filePath, string fileName, string destinationDirectory, int quality);
    }
}