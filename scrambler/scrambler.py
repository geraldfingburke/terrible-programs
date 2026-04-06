import tkinter as tk
from tkinter import filedialog, messagebox
from PIL import Image, ImageTk
import colorsys
import random
import os
import threading


class ScramblerApp:
    def __init__(self, root):
        self.root = root
        self.root.title("Scrambler")
        self.root.geometry("900x600")
        self.root.minsize(600, 400)

        self.source_image = None
        self.result_image = None

        self._build_ui()

    def _build_ui(self):
        # Top button bar
        btn_frame = tk.Frame(self.root)
        btn_frame.pack(side=tk.TOP, fill=tk.X, padx=8, pady=8)

        self.load_btn = tk.Button(btn_frame, text="Load Image", command=self._load_image)
        self.load_btn.pack(side=tk.LEFT, padx=4)

        self.scramble_btn = tk.Button(btn_frame, text="Scramble", command=self._scramble, state=tk.DISABLED)
        self.scramble_btn.pack(side=tk.LEFT, padx=4)

        self.save_btn = tk.Button(btn_frame, text="Save Result", command=self._save, state=tk.DISABLED)
        self.save_btn.pack(side=tk.LEFT, padx=4)

        self.status = tk.Label(btn_frame, text="Load an image to begin.", anchor=tk.W)
        self.status.pack(side=tk.LEFT, padx=12, fill=tk.X, expand=True)

        # Image panels
        panel_frame = tk.Frame(self.root)
        panel_frame.pack(fill=tk.BOTH, expand=True, padx=8, pady=(0, 8))
        panel_frame.columnconfigure(0, weight=1)
        panel_frame.columnconfigure(1, weight=1)
        panel_frame.rowconfigure(1, weight=1)

        tk.Label(panel_frame, text="Original").grid(row=0, column=0)
        tk.Label(panel_frame, text="Scrambled").grid(row=0, column=1)

        self.original_label = tk.Label(panel_frame, bg="#222")
        self.original_label.grid(row=1, column=0, sticky="nsew", padx=(0, 4))

        self.result_label = tk.Label(panel_frame, bg="#222")
        self.result_label.grid(row=1, column=1, sticky="nsew", padx=(4, 0))

        # Keep references so the GC doesn't discard PhotoImages
        self._tk_original = None
        self._tk_result = None

    def _load_image(self):
        path = filedialog.askopenfilename(
            filetypes=[("Image files", "*.png *.jpg *.jpeg *.bmp *.gif *.tiff *.webp")]
        )
        if not path:
            return
        try:
            self.source_image = Image.open(path).convert("RGB")
        except Exception as e:
            messagebox.showerror("Error", f"Could not open image:\n{e}")
            return

        self._show_thumbnail(self.source_image, self.original_label, "_tk_original")
        self.result_image = None
        self._clear_label(self.result_label, "_tk_result")
        self.scramble_btn.config(state=tk.NORMAL)
        self.save_btn.config(state=tk.DISABLED)
        self.status.config(text=f"Loaded {os.path.basename(path)}  ({self.source_image.width}x{self.source_image.height})")

    def _scramble(self):
        if self.source_image is None:
            return

        self.scramble_btn.config(state=tk.DISABLED)
        self.save_btn.config(state=tk.DISABLED)
        self.status.config(text="Scrambling...")
        self.root.update_idletasks()

        # Run the heavy pixel work on a background thread
        threading.Thread(target=self._do_scramble, daemon=True).start()

    def _do_scramble(self):
        img = self._colorize(self.source_image.copy())
        w, h = img.size
        cols, rows = 15, 15
        tile_w = w // cols
        tile_h = h // rows

        # Crop out each tile
        tiles = []
        for r in range(rows):
            for c in range(cols):
                box = (c * tile_w, r * tile_h, (c + 1) * tile_w, (r + 1) * tile_h)
                tiles.append(img.crop(box))

        random.shuffle(tiles)

        # Apply a random hue shift to each tile
        for i, tile in enumerate(tiles):
            tiles[i] = self._shift_hue(tile, random.random())

        # Reassemble into an output image (same size as the tiled area)
        out = Image.new("RGB", (tile_w * cols, tile_h * rows))
        for idx, tile in enumerate(tiles):
            r = idx // cols
            c = idx % cols
            out.paste(tile, (c * tile_w, r * tile_h))

        self.result_image = out
        # Schedule UI update on the main thread
        self.root.after(0, self._on_scramble_done)

    def _on_scramble_done(self):
        self._show_thumbnail(self.result_image, self.result_label, "_tk_result")
        self.scramble_btn.config(state=tk.NORMAL)
        self.save_btn.config(state=tk.NORMAL)
        self.status.config(text="Done! You can save the result or scramble again.")

    def _save(self):
        if self.result_image is None:
            return
        path = filedialog.asksaveasfilename(
            defaultextension=".png",
            filetypes=[("PNG", "*.png"), ("JPEG", "*.jpg"), ("BMP", "*.bmp")],
        )
        if not path:
            return
        try:
            self.result_image.save(path)
            self.status.config(text=f"Saved to {path}")
        except Exception as e:
            messagebox.showerror("Error", f"Could not save image:\n{e}")

    # -- helpers ----------------------------------------------------------

    @staticmethod
    def _colorize(img, min_saturation=0.4):
        """Boost saturation on the whole image so even B&W pixels take on color."""
        pixels = img.load()
        w, h = img.size
        for y in range(h):
            for x in range(w):
                r, g, b = pixels[x, y]
                ho, s, v = colorsys.rgb_to_hsv(r / 255.0, g / 255.0, b / 255.0)
                if s < min_saturation:
                    s = min_saturation
                    nr, ng, nb = colorsys.hsv_to_rgb(ho, s, v)
                    pixels[x, y] = (int(nr * 255), int(ng * 255), int(nb * 255))
        return img

    @staticmethod
    def _shift_hue(tile, hue_offset):
        """Shift every pixel's hue by hue_offset (0-1), preserving S and V."""
        pixels = tile.load()
        w, h = tile.size
        for y in range(h):
            for x in range(w):
                r, g, b = pixels[x, y]
                ho, s, v = colorsys.rgb_to_hsv(r / 255.0, g / 255.0, b / 255.0)
                ho = (ho + hue_offset) % 1.0
                nr, ng, nb = colorsys.hsv_to_rgb(ho, s, v)
                pixels[x, y] = (int(nr * 255), int(ng * 255), int(nb * 255))
        return tile

    def _show_thumbnail(self, img, label, attr):
        """Resize image to fit the label and display it."""
        label.update_idletasks()
        lw = max(label.winfo_width(), 200)
        lh = max(label.winfo_height(), 200)
        thumb = img.copy()
        thumb.thumbnail((lw, lh), Image.LANCZOS)
        tk_img = ImageTk.PhotoImage(thumb)
        label.config(image=tk_img)
        setattr(self, attr, tk_img)

    def _clear_label(self, label, attr):
        label.config(image="")
        setattr(self, attr, None)


if __name__ == "__main__":
    root = tk.Tk()
    app = ScramblerApp(root)
    root.mainloop()
