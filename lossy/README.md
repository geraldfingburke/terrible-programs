# Lossy Compressor

A revolutionary Windows native application that achieves incredible compression ratios by removing 99% of the bits from your files!

## How It Works

The Lossy Compression algorithm works by:

1. Reading the input file
2. Keeping only 1% of the bytes (evenly distributed across the file)
3. Storing the original file size in a header
4. Writing the compressed output

**Warning:** This is a lossy compression algorithm. 99% of your data will be permanently lost. The decompressed file will not match the original!

## Building

```powershell
dotnet build
```

## Usage

### Compress a file:

```powershell
dotnet run <input_file> [output_file]
```

Examples:

```powershell
# Compress a file (output will be input_file.lossy)
dotnet run document.txt

# Compress with custom output name
dotnet run document.txt compressed.lossy
```

### Build as executable:

```powershell
dotnet publish -c Release -r win-x64 --self-contained
```

The executable will be in `bin\Release\net8.0\win-x64\publish\lossy.exe`

## Example

```powershell
> dotnet run test.txt test.lossy
Lossy Compressor - Removes 99% of bits for maximum compression!
================================================================

Input file:  test.txt
Output file: test.lossy

Compressing...

Compression complete!
Original size:    10.00 KB
Compressed size:  108 B
Compression ratio: 98.95%
```

## Features

- ✅ Extremely high compression ratios (typically 98-99%)
- ✅ Very fast compression
- ✅ Windows native application
- ✅ Simple command-line interface
- ⚠️ Lossy compression (data cannot be fully recovered)

## System Requirements

- .NET 8.0 or higher
- Windows (or any OS supporting .NET)

## License

Use at your own risk! Not recommended for important data. 😄
