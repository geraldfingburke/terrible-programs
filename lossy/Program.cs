using System;
using System.IO;

namespace LossyCompressor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Lossy Compressor - Removes 99% of bits for maximum compression!");
            Console.WriteLine("================================================================\n");

            if (args.Length < 1)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  Compress:   lossy <input_file> [output_file]");
                Console.WriteLine("  Decompress: lossy -d <compressed_file> [output_file]");
                Console.WriteLine("\nExamples:");
                Console.WriteLine("  lossy document.txt document.lossy");
                Console.WriteLine("  lossy -d document.lossy recovered.txt");
                return;
            }

            bool decompress = args[0] == "-d" || args[0] == "--decompress";
            string inputFile = decompress ? (args.Length > 1 ? args[1] : "") : args[0];
            string outputFile;
            
            if (decompress)
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Error: Please specify input file for decompression.");
                    return;
                }
                outputFile = args.Length > 2 ? args[2] : inputFile.Replace(".lossy", "") + ".recovered";
            }
            else
            {
                outputFile = args.Length > 1 ? args[1] : inputFile + ".lossy";
            }

            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Error: Input file '{inputFile}' not found.");
                return;
            }

            try
            {
                Console.WriteLine($"Input file:  {inputFile}");
                Console.WriteLine($"Output file: {outputFile}");
                
                var compressor = new LossyCompression();

                if (decompress)
                {
                    Console.WriteLine("\nDecompressing...");
                    compressor.Decompress(inputFile, outputFile);
                    
                    var compressedSize = new FileInfo(inputFile).Length;
                    var decompressedSize = new FileInfo(outputFile).Length;
                    
                    Console.WriteLine($"\nDecompression complete!");
                    Console.WriteLine($"Compressed size:    {FormatBytes(compressedSize)}");
                    Console.WriteLine($"Decompressed size:  {FormatBytes(decompressedSize)}");
                    Console.WriteLine("\nNote: 99% of data was filled with random bits.");
                }
                else
                {
                    Console.WriteLine("\nCompressing...");
                    compressor.Compress(inputFile, outputFile);

                    var originalSize = new FileInfo(inputFile).Length;
                    var compressedSize = new FileInfo(outputFile).Length;
                    var ratio = originalSize > 0 ? (1 - (double)compressedSize / originalSize) * 100 : 0;

                    Console.WriteLine($"\nCompression complete!");
                    Console.WriteLine($"Original size:    {FormatBytes(originalSize)}");
                    Console.WriteLine($"Compressed size:  {FormatBytes(compressedSize)}");
                    Console.WriteLine($"Compression ratio: {ratio:F2}%");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }

        static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
