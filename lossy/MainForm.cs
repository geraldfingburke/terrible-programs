using System;
using System.IO;
using System.Windows.Forms;
using LossyCompressor;

namespace LossyCompressorGUI
{
    public class MainForm : Form
    {
        private TextBox inputFileTextBox;
        private TextBox outputFileTextBox;
        private Button browseInputButton;
        private Button browseOutputButton;
        private Button compressButton;
        private Button decompressButton;
        private Label statusLabel;
        private Label inputLabel;
        private Label outputLabel;
        private Label titleLabel;

        public MainForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Lossy Compressor";
            this.Width = 600;
            this.Height = 350;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Title
            titleLabel = new Label
            {
                Text = "Lossy Compressor - 99% Compression!",
                Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold),
                AutoSize = false,
                Width = 560,
                Height = 40,
                Left = 20,
                Top = 20,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            // Input file
            inputLabel = new Label
            {
                Text = "Input File:",
                Left = 20,
                Top = 80,
                Width = 100
            };

            inputFileTextBox = new TextBox
            {
                Left = 120,
                Top = 77,
                Width = 350,
                ReadOnly = true
            };

            browseInputButton = new Button
            {
                Text = "Browse...",
                Left = 480,
                Top = 75,
                Width = 90
            };
            browseInputButton.Click += BrowseInputButton_Click;

            // Output file
            outputLabel = new Label
            {
                Text = "Output File:",
                Left = 20,
                Top = 120,
                Width = 100
            };

            outputFileTextBox = new TextBox
            {
                Left = 120,
                Top = 117,
                Width = 350
            };

            browseOutputButton = new Button
            {
                Text = "Browse...",
                Left = 480,
                Top = 115,
                Width = 90
            };
            browseOutputButton.Click += BrowseOutputButton_Click;

            // Compress button
            compressButton = new Button
            {
                Text = "Compress",
                Left = 120,
                Top = 170,
                Width = 150,
                Height = 40,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
            };
            compressButton.Click += CompressButton_Click;

            // Decompress button
            decompressButton = new Button
            {
                Text = "Decompress",
                Left = 290,
                Top = 170,
                Width = 150,
                Height = 40,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
            };
            decompressButton.Click += DecompressButton_Click;

            // Status label
            statusLabel = new Label
            {
                Text = "Ready",
                Left = 20,
                Top = 230,
                Width = 560,
                Height = 60,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding = new Padding(10)
            };

            this.Controls.Add(titleLabel);
            this.Controls.Add(inputLabel);
            this.Controls.Add(inputFileTextBox);
            this.Controls.Add(browseInputButton);
            this.Controls.Add(outputLabel);
            this.Controls.Add(outputFileTextBox);
            this.Controls.Add(browseOutputButton);
            this.Controls.Add(compressButton);
            this.Controls.Add(decompressButton);
            this.Controls.Add(statusLabel);
        }

        private void BrowseInputButton_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "All files (*.*)|*.*|Lossy files (*.lossy)|*.lossy";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    inputFileTextBox.Text = openFileDialog.FileName;
                    if (string.IsNullOrEmpty(outputFileTextBox.Text))
                    {
                        outputFileTextBox.Text = openFileDialog.FileName + ".lossy";
                    }
                }
            }
        }

        private void BrowseOutputButton_Click(object? sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Lossy files (*.lossy)|*.lossy|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    outputFileTextBox.Text = saveFileDialog.FileName;
                }
            }
        }

        private void CompressButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(inputFileTextBox.Text))
            {
                MessageBox.Show("Please select an input file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(inputFileTextBox.Text))
            {
                MessageBox.Show("Input file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(outputFileTextBox.Text))
            {
                outputFileTextBox.Text = inputFileTextBox.Text + ".lossy";
            }

            try
            {
                statusLabel.Text = "Compressing...";
                Application.DoEvents();

                var compressor = new LossyCompression();
                compressor.Compress(inputFileTextBox.Text, outputFileTextBox.Text);

                var originalSize = new FileInfo(inputFileTextBox.Text).Length;
                var compressedSize = new FileInfo(outputFileTextBox.Text).Length;
                var ratio = originalSize > 0 ? (1 - (double)compressedSize / originalSize) * 100 : 0;

                statusLabel.Text = $"✓ Compressed! {FormatBytes(originalSize)} → {FormatBytes(compressedSize)} ({ratio:F2}% reduction)";
                MessageBox.Show($"Compression complete!\n\nOriginal: {FormatBytes(originalSize)}\nCompressed: {FormatBytes(compressedSize)}\nRatio: {ratio:F2}%", 
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Error during compression";
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DecompressButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(inputFileTextBox.Text))
            {
                MessageBox.Show("Please select an input file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(inputFileTextBox.Text))
            {
                MessageBox.Show("Input file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(outputFileTextBox.Text))
            {
                outputFileTextBox.Text = inputFileTextBox.Text.Replace(".lossy", "") + ".recovered";
            }

            try
            {
                statusLabel.Text = "Decompressing...";
                Application.DoEvents();

                var compressor = new LossyCompression();
                compressor.Decompress(inputFileTextBox.Text, outputFileTextBox.Text);

                var compressedSize = new FileInfo(inputFileTextBox.Text).Length;
                var decompressedSize = new FileInfo(outputFileTextBox.Text).Length;

                statusLabel.Text = $"✓ Decompressed! {FormatBytes(compressedSize)} → {FormatBytes(decompressedSize)} (99% random bits)";
                MessageBox.Show($"Decompression complete!\n\nCompressed: {FormatBytes(compressedSize)}\nDecompressed: {FormatBytes(decompressedSize)}\n\nNote: 99% of data was filled with random bits.", 
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Error during decompression";
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string FormatBytes(long bytes)
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
