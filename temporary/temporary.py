import tkinter as tk

root = tk.Tk()
root.title("temporary")
root.geometry("800x600")

text = tk.Text(root, wrap="word", undo=True)
text.pack(fill="both", expand=True)
text.focus_set()

root.mainloop()
