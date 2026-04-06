# Scrambler

A Python desktop GUI app that scrambles images by slicing them into a 15×15 grid of tiles, randomly rearranging them, and applying a random hue shift to each tile.

## How It Works

1. The image is divided into a **15×15 grid** (225 tiles).
2. All tiles are **shuffled** into a random order.
3. Each tile receives a **random hue shift**, recoloring it while preserving brightness.
4. The tiles are **reassembled** into a new image, preserving the original aspect ratio.

Each click of **Scramble** produces a different random arrangement and color variation.

## Requirements

- Python 3
- [Pillow](https://pypi.org/project/Pillow/)

```
pip install Pillow
```

## Usage

```
python scrambler.py
```

1. Click **Load Image** to pick an image (PNG, JPG, BMP, GIF, TIFF, or WebP).
2. Click **Scramble** to shuffle the tiles.
3. Click **Save Result** to export the scrambled image.

The window shows the original and scrambled images side by side.
