using DDXViewer.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace DDXViewer
{
    public partial class MainForm : Form
    {
        private static readonly string exePath = Assembly.GetExecutingAssembly().Location;
        private static readonly string exeDirectory = Path.GetDirectoryName(exePath);
        private static readonly string resourcesPath = $@"{exeDirectory}\Resources";
        private static readonly string backupPath = $@"{exeDirectory}\Backups";
        private static readonly string dependenciesPath = $@"{exeDirectory}\Dependencies";
        private static readonly string texConvPath = $@"{dependenciesPath}\texconv.exe";
        private string originalFilePath;
        private static string resourceFilePath;
        private string resourceFileName;
        private static string resourceFileConvertedPath;

        private Image displayImage;

        private static readonly ProcessStartInfo TexConvInfo = new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = texConvPath,
            WindowStyle = ProcessWindowStyle.Hidden
        };
        private static readonly Process TexConv = new Process { StartInfo = TexConvInfo, EnableRaisingEvents = true };
        private readonly Process PaintDotNet = new Process();
        private static readonly string[] SizeSuffixes =
                  { "bytes", "KB", "MB", "GB" };

        private readonly string[] arguments = Environment.GetCommandLineArgs();

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            pe.Graphics.SmoothingMode = SmoothingMode.None;
            base.OnPaint(pe);
        }

        public MainForm()
        {
            InitializeComponent();

            // Load Prefered BackColor
            BackColor = Settings.Default.BackColor;

            // Check if the backup limit hasn't been reached, if yes, delete.
            CheckBackUp();

            // Close the application if input is not ddx.
            CheckIfInputIsDDX();

            // Check if Dependencies and Resources folder exists, if not, create them.
            if (!HasDependencies())
            {
                CreateDependencies();
            }

            // Copy the picture into Ressources folder
            CopyOriginalDDX();

            // Convert the DDX into a PNG 
            ConvertResourceDDX();

            // Display the Converted DDX into the form
            DisplayConvertedDDX();

        }

        private void CheckBackUp()
        {
            DirectoryInfo di = new DirectoryInfo(backupPath);

            if (di.GetFiles().Length > Settings.Default.MaxBackUp)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
        }

        private void CheckIfInputIsDDX()
        {
            if (Path.GetExtension(arguments[1]) != ".ddx")
            {
                if (MessageBox.Show("Opened file is not in a valid DDX format!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    Application.Exit();
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) => CleanUp();

        private bool HasDependencies()
        {
            if (Directory.Exists(resourcesPath) && Directory.Exists(dependenciesPath) && Directory.Exists(backupPath))
            {
                if (File.Exists(texConvPath))
                {
                    return true;
                }
            }
            return false;
        }

        private void CreateDependencies()
        {
            if (!Directory.Exists(resourcesPath)) { Directory.CreateDirectory(resourcesPath); }
            if (!Directory.Exists(backupPath)) { Directory.CreateDirectory(backupPath); }
            if (!Directory.Exists(dependenciesPath)) { Directory.CreateDirectory(dependenciesPath); }
            if (!File.Exists(texConvPath))
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(Settings.Default.TexConvURL, texConvPath);
                }
            }
        }

        private void CopyOriginalDDX()
        {
            originalFilePath = arguments[1];
            FileInfo file = new FileInfo(originalFilePath);
            resourceFileName = Path.GetFileNameWithoutExtension(file.Name);
            resourceFilePath = $@"{resourcesPath}\{resourceFileName}.dds";
            File.Copy(originalFilePath, resourceFilePath, true);
        }

        private void ConvertResourceDDX()
        {
            TexConv.StartInfo.Arguments = $"\"{resourceFilePath}\" -ft png -y -o \"{resourcesPath}\"";
            TexConv.Start();

            TexConv.WaitForExit();

            if (File.Exists(resourceFilePath)) { File.Delete(resourceFilePath); }
            resourceFileConvertedPath = $@"{resourcesPath}\{resourceFileName}.png";
        }

        private void DisplayConvertedDDX()
        {
            displayImage = Image.FromFile(resourceFileConvertedPath);
            DisplayBox.Image = displayImage;

            if (displayImage.Width >= Settings.Default.MediumRate)
            {
                Width = displayImage.Width;
                Height = displayImage.Height;
            }
        }

        private void CleanUp()
        {
            Settings.Default.Save();
            displayImage?.Dispose();
            DisplayBox.Image?.Dispose();

            DirectoryInfo di = new DirectoryInfo(resourcesPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        private void OpenInPaintDotNet(object sender, EventArgs e)
        {
            FileDialog.ShowHelp = false;
            FileDialog.Title = "Select paintdotnet.exe";
            FileDialog.FileName = "paintdotnet.exe";

            if (Settings.Default.PaintPath != "")
            {
                PaintDotNet.StartInfo.FileName = Settings.Default.PaintPath;
                PaintDotNet.StartInfo.Arguments = $"{resourceFileConvertedPath}";
                PaintDotNet.Start();
            }
            else
            {
                if (FileDialog.ShowDialog() == DialogResult.OK)
                {
                    PaintDotNet.StartInfo.FileName = FileDialog.FileName;
                    PaintDotNet.StartInfo.Arguments = $"{resourceFileConvertedPath}";
                    PaintDotNet.Start();

                    Settings.Default.PaintPath = FileDialog.FileName;
                    Settings.Default.Save();
                }
            }
        }

        private void ThemeColorSelection(object sender, EventArgs e)
        {
            if (ColorPicker.ShowDialog() == DialogResult.OK)
            {
                BackColor = ColorPicker.Color;
                Settings.Default.BackColor = ColorPicker.Color;

                Settings.Default.Save();
            }
        }
        private void CompressionMaxRate(object sender, EventArgs e) => CompressAndSaveImage(displayImage, Settings.Default.MaxRate);

        private void CompressionGoodRate(object sender, EventArgs e) => CompressAndSaveImage(displayImage, Settings.Default.GoodRate);

        private void CompressionMediumRate(object sender, EventArgs e) => CompressAndSaveImage(displayImage, Settings.Default.MediumRate);

        private void CompressionPoorRate(object sender, EventArgs e) => CompressAndSaveImage(displayImage, Settings.Default.PoorRate);

        private void CompressAndSaveImage(Image img, int compressionRate)
        {
            if (img.Size.Width >= compressionRate && img.Size.Height >= compressionRate)
            {
                Bitmap bitmap = new Bitmap(img, new Size(compressionRate, compressionRate));
                string compressedPath = $@"{resourcesPath}\{resourceFileName}_compressed.png";
                bitmap.Save(compressedPath, System.Drawing.Imaging.ImageFormat.Png);

                long old = new FileInfo(resourceFileConvertedPath).Length;
                long _new = new FileInfo($@"{resourcesPath}\{resourceFileName}_compressed.png").Length;

                if (MessageBox.Show($"Current Size = {SizeSuffix(old)}.\nEstimated Compressed Size = {SizeSuffix(_new)}.", "Estimater", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    BackUpImage();

                    TexConv.StartInfo.Arguments = $"\"{compressedPath}\" -r:keep -f BC7_UNORM -bc x -if POINT -ft dds -fl 11.0 -tu -m 1 -y -o \"{Path.GetDirectoryName(compressedPath)}\"";
                    TexConv.Start();

                    TexConv.WaitForExit();

                    File.Copy(compressedPath.Replace(".png", ".dds"), originalFilePath, true);

                    Application.Restart();
                }
                else
                {
                    bitmap.Dispose();
                    File.Delete($@"{resourcesPath}\{resourceFileName}_compressed.png");
                }
            }
            else if (img.Size.Width <= compressionRate && img.Size.Height <= compressionRate && File.Exists($@"{backupPath}\{resourceFileName}.ddx"))
            {
                File.Copy($@"{backupPath}\{resourceFileName}.ddx", originalFilePath, true);
                Application.Restart();
            }
            else
            {
                MessageBox.Show("This file is already at this compression level!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void BackUpImage() => File.Copy(originalFilePath, $@"{backupPath}\{resourceFileName}.ddx", true);

        private static string SizeSuffix(long value, int decimalPlaces = 1)
        {
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }

            int i = 0;
            decimal dValue = value;
            while (Math.Round(dValue, decimalPlaces) >= 1000)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[i]);
        }

    }
}

