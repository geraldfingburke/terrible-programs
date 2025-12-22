using System;
using System.IO;

namespace LossyCompressor
{
    public class LossyCompression
    {
        /// <summary>
        /// Compresses a file by removing 99% of the bits.
        /// This revolutionary algorithm achieves maximum compression!
        /// </summary>
        public void Compress(string inputPath, string outputPath)
        {
            byte[] inputData = File.ReadAllBytes(inputPath);
            
            // Calculate how many bytes to keep (1% of original)
            int bytesToKeep = Math.Max(1, inputData.Length / 100);
            
            // Create output array with header + compressed data
            byte[] outputData = new byte[bytesToKeep + 8];
            
            // Write original size as header (8 bytes for long)
            BitConverter.GetBytes((long)inputData.Length).CopyTo(outputData, 0);
            
            // Keep only 1% of the data (evenly distributed)
            for (int i = 0; i < bytesToKeep; i++)
            {
                int sourceIndex = (int)((long)i * inputData.Length / bytesToKeep);
                outputData[i + 8] = inputData[sourceIndex];
            }
            
            File.WriteAllBytes(outputPath, outputData);
        }

        /// <summary>
        /// Decompresses a lossy-compressed file.
        /// Note: 99% of the data was lost, so this reconstructs by filling with random bits.
        /// </summary>
        public void Decompress(string inputPath, string outputPath)
        {
            byte[] compressedData = File.ReadAllBytes(inputPath);
            
            if (compressedData.Length < 8)
            {
                throw new InvalidDataException("Invalid compressed file format.");
            }
            
            // Read original size from header
            long originalSize = BitConverter.ToInt64(compressedData, 0);
            int compressedBytes = compressedData.Length - 8;
            
            byte[] outputData = new byte[originalSize];
            Random random = new Random();
            
            // Fill the entire output with random bytes
            random.NextBytes(outputData);
            
            // Then place the 1% of preserved data at evenly distributed positions
            for (int i = 0; i < compressedBytes; i++)
            {
                int targetIndex = (int)((long)i * originalSize / compressedBytes);
                if (targetIndex < originalSize)
                {
                    outputData[targetIndex] = compressedData[i + 8];
                }
            }
            
            File.WriteAllBytes(outputPath, outputData);
        }
    }
}
