using DDXViewer.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DDXViewer
{
    public partial class MainForm : Form
    {
        private static readonly string exePath = Assembly.GetExecutingAssembly().Location, exeDirectory = Path.GetDirectoryName(exePath), resourcesPath = $@"{exeDirectory}\Resources", backupPath = $@"{exeDirectory}\Backups", dependenciesPath = $@"{exeDirectory}\Dependencies", texConvPath = $@"{dependenciesPath}\texconv.exe";
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

            // Close the application if input is not ddx.
            CheckIfInputIsDDX();

            // Check if Dependencies exists, if not, display error message and close the application.
            if (!HasDependencies())
            {
                MessageBox.Show("Dependencies not found. Please download the dependencies from the GitHub repository.", "Dependencies not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }

            // Check if the backup limit hasn't been reached, if yes, delete.
            CheckBackUp();

            // Copy the picture into Ressources folder
            CopyOriginalDDX();

            // Convert the DDX into a PNG 
            ConvertResourceDDX();

            // Display the Converted DDX into the form
            DisplayConvertedDDX();

        }

        private void CheckBackUp()
        {
            // Check if the backup limit hasn't been reached, if yes, delete.
            if (Directory.GetFiles(backupPath).Length >= Settings.Default.MaxBackUp)
            {
                // for each file in the backup folder
                foreach (string file in Directory.GetFiles(backupPath))
                {
                    // delete the file
                    File.Delete(file);
                }
            }
        }

        private void CheckIfInputIsDDX()
        {
            // Check if the input has the extension ddx
            if (arguments.Length > 1 && !arguments[1].EndsWith(".ddx"))
            {
                // show a message box with the error message and close the application 
                MessageBox.Show("The input file must be a .ddx file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) => CleanUp();

        private bool HasDependencies() =>
            // check if the dependencies folder exists and resource folder exists and backup folder exists and texconv.exe exists
            Directory.Exists(dependenciesPath) && Directory.Exists(resourcesPath) && Directory.Exists(backupPath) && File.Exists(texConvPath);

        private void CopyOriginalDDX()
        {
            // set the original file path to the input file path
            originalFilePath = arguments[1];

            // set the resource file name to the input file name
            resourceFileName = Path.GetFileNameWithoutExtension(originalFilePath);

            // set the resource file path to the resource folder path + resource file name and change the extension to .dds
            resourceFilePath = $@"{resourcesPath}\{resourceFileName}.dds";

            // if the resource file doesn't exist, copy the original file to the resource folder
            if (!File.Exists(resourceFilePath))
            {
                File.Copy(originalFilePath, resourceFilePath);
            }
        }

        private void ConvertResourceDDX()
        {
            // Set TexConv arguments to convert the resource file to a .png file and save it to the resources folder with the same name
            TexConvInfo.Arguments = $"\"{ resourceFilePath}\" -ft png -y -o \"{resourcesPath}\"";

            // Start the process
            TexConv.Start();

            // Wait for the process to finish
            TexConv.WaitForExit();

            // if the process has exited with an error code
            if (TexConv.ExitCode != 0)
            {
                // show a message box with the error message and close the application 
                MessageBox.Show("An error occured while converting the file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            // if the resource file exist in the resources folder delete it
            if (File.Exists(resourceFilePath))
            {
                File.Delete(resourceFilePath);
            }

            // set the resource file converted path to the resources folder path + resource file name and change the extension to .png
            resourceFileConvertedPath = $@"{resourcesPath}\{resourceFileName}.png";
        }

        private void DisplayConvertedDDX()
        {
            // if the resource file converted path exist set the display image to the resource file converted path
            if (File.Exists(resourceFileConvertedPath))
            {
                displayImage = Image.FromFile(resourceFileConvertedPath);
                // set the DisplayBox to the display image
                DisplayBox.Image = displayImage;
            }
            else
            {
                // if the resource file converted path doesn't exist show a message box with the error message and close the application
                MessageBox.Show("An error occured while converting the file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            // set the display image size to the display image size if the display image size is smaller than the form size and if the display image size is bigger than the form size
            if (displayImage.Size.Width < Size.Width && displayImage.Size.Height < Size.Height)
            {
                DisplayBox.Size = displayImage.Size;
            }
            else if (displayImage.Size.Width > Size.Width && displayImage.Size.Height > Size.Height)
            {
                DisplayBox.Size = Size;
            }
            else
            {
                // if the display image size is smaller than the form size and bigger than the form size set the display image size to the form size
                DisplayBox.Size = Size;
            }
        }

        private void CleanUp()
        {
            // save the default settings
            Settings.Default.Save();

            // disposes the display image and the display box if they exist
            displayImage?.Dispose();
            DisplayBox?.Dispose();

            // dispose of the files in the resources folder if they exist
            foreach (string file in Directory.GetFiles(resourcesPath))
            {
                File.Delete(file);
            }

            // dispose of texconv process if it exists
            TexConv?.Dispose();
        }

        private void OpenInPaintDotNet(object sender, EventArgs e)
        {
            // if Settings.Default.PaintPath is not empty run PaintDotNet with the resource file converted path
            if (!string.IsNullOrEmpty(Settings.Default.PaintPath))
            {
                Process.Start(Settings.Default.PaintPath, resourceFileConvertedPath);
            }
            else
            {
                // set the FileDialog to open a .exe file and set the filter to .exe files only set the title to Open Paint.NET
                FileDialog.Filter = "Paint.NET Executable|*.exe";
                FileDialog.Title = "Open Paint.NET";

                // if the user has selected a file set PaintDotNet path to the file path and save the settings else show a message box with the error message and close the application
                if (FileDialog.ShowDialog() == DialogResult.OK)
                {
                    Settings.Default.PaintPath = FileDialog.FileName;
                    Settings.Default.Save();
                    Process.Start(Settings.Default.PaintPath, resourceFileConvertedPath);
                }
                else
                {
                    MessageBox.Show("An error occured while opening Paint.NET.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
            }
        }

        private void ThemeColorSelection(object sender, EventArgs e)
        {
            // Show ColorPicker dialog and set the BackColor to the selected color if the user has selected a color and save the settings BackColor to the selected color else return 
            if (ColorPicker.ShowDialog() == DialogResult.OK)
            {
                BackColor = ColorPicker.Color;
                Settings.Default.BackColor = ColorPicker.Color;
                Settings.Default.Save();
            }
            else
            {
                return;
            }
        }
        private void CompressionMaxRate(object sender, EventArgs e) => CompressAndSaveImage(displayImage, Settings.Default.MaxRate);

        private void CompressionGoodRate(object sender, EventArgs e) => CompressAndSaveImage(displayImage, Settings.Default.GoodRate);

        private void CompressionMediumRate(object sender, EventArgs e) => CompressAndSaveImage(displayImage, Settings.Default.MediumRate);

        private void CompressionPoorRate(object sender, EventArgs e) => CompressAndSaveImage(displayImage, Settings.Default.PoorRate);

        private void CompressAndSaveImage(Image img, int compressionRate)
        {
            // if img is not null and img.Size is bigger than compressionRate then resize the image to compressionRate and save it with the same name with "compressed" added to the end of the file name and the extension to .png 
            if (img != null && img.Size.Width > compressionRate && img.Size.Height > compressionRate)
            {
                img = img.GetThumbnailImage(compressionRate, compressionRate, null, IntPtr.Zero);
                string compressedPath = $@"{resourcesPath}\{resourceFileName}_compressed.png";
                img.Save(compressedPath, ImageFormat.Png);

                // get img file size in bytes and show a message box with the file size in bytes
                if (MessageBox.Show($"Current Size = {SizeSuffix(File.ReadAllBytes(resourceFileConvertedPath).Length)}\nEstimated Compressed Size = {SizeSuffix(File.ReadAllBytes(compressedPath).Length)} bytes", "Estimater", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    // BackUp the image
                    BackUpImage();

                    // set TexConv arguments to the img file and compress it 
                    TexConv.StartInfo.Arguments = $"\"{compressedPath}\" -r:keep -f BC7_UNORM -bc x -if POINT -ft dds -fl 11.0 -tu -m 1 -y -o \"{Path.GetDirectoryName(compressedPath)}\"";

                    // run TexConv process and wait for it to finish
                    TexConv.Start();
                    TexConv.WaitForExit();

                    // remplace the compressed image extension to .dds
                    string compressedDDS = $@"{Path.GetDirectoryName(compressedPath)}\{Path.GetFileNameWithoutExtension(compressedPath)}.dds";
                    // copy the compressed image to original image path and delete the compressed image
                    File.Copy(compressedDDS, originalFilePath, true);
                    File.Delete(compressedDDS);

                    // restart the application
                    Application.Restart();
                }
                else
                {
                    // dispose of the img
                    img.Dispose();

                    // dipose of the compressed image if it exists
                    if (File.Exists(compressedPath))
                    {
                        File.Delete(compressedPath);
                    }
                }
            }
            // else if the img is not null and img.Size is smaller than compressionRate then show a message box with the error message and dispose of the img
            else if (img != null && img.Size.Width < compressionRate && img.Size.Height < compressionRate)
            {
                MessageBox.Show("The image is too small to compress.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                img.Dispose();
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

