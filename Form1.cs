using DDXViewer.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace DDXViewer
{
    public partial class Form1 : Form
    {
        private static readonly string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        private static readonly string exeDirectory = Path.GetDirectoryName(exePath);
        private static readonly string resourcesPath = $@"{exeDirectory}\Resources";
        private static readonly string toolsPath = $@"{exeDirectory}\Tools";
        private static readonly string texConvPath = $@"{toolsPath}\texconv.exe";

        private static string resourceFilePath;
        private static string resourceFileConvertedPath;

        private int numberOfFiles;
        private Image displayImage;

        private static readonly ProcessStartInfo TexConvInfo = new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = texConvPath,
            WindowStyle = ProcessWindowStyle.Hidden
        };
        private static readonly Process TexConv = new Process { StartInfo = TexConvInfo, EnableRaisingEvents = true };

        private readonly string[] arguments = Environment.GetCommandLineArgs();
        public Form1()
        {
            InitializeComponent();

            // Hide the Form until the picture is loaded
            Hide();
            InitializeThemes();


            // Might need to change that later
            if (!InputIsDDX())
            {
                Application.Exit();
            }

            // Check if TexConv and Ressources folder exists, if not, create them.
            if (!CheckIntegrity())
            {
                CreateIntegrity();
            }

            // Get The numbers of files in the ressoucers to generate names
            GetInputedFiles();

            // Copy the picture into Ressources folder
            CopyOriginalDDX();

            // Convert the DDX into a PNG 
            ConvertRessourceDDX();

            // Display the Converted DDX into the form
            DisplayConvertedDDX();

        }

        private void InitializeThemes()
        {
            Type colorType = typeof(System.Drawing.Color);
            PropertyInfo[] propInfos = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
            foreach (PropertyInfo propInfo in propInfos)
            {
                if (propInfo.Name != "Transparent")
                {
                    ThemesList.Items.Add(propInfo.Name);
                }
            }

            if (Settings.Default.Color != "")
            {
                BackColor = Color.FromName(Settings.Default.Color);
            }
        }

        private void GetInputedFiles()
        {
            DirectoryInfo info = new DirectoryInfo(resourcesPath);
            if (info.GetFiles().Length == 0)
            {
                numberOfFiles = 1;
            }
            else
            {
                numberOfFiles = info.GetFiles().Length + 1;
            }
        }

        private bool InputIsDDX()
        {
            if (Path.GetExtension(arguments[1]) == ".ddx")
            {
                return true;
            }
            return false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CleanUp();
        }

        private bool CheckIntegrity()
        {
            if (Directory.Exists(resourcesPath) && Directory.Exists(toolsPath))
            {
                if (File.Exists(texConvPath))
                {
                    return true;
                }
            }
            return false;
        }

        private void CreateIntegrity()
        {
            if (!Directory.Exists(resourcesPath)) { Directory.CreateDirectory(resourcesPath); }
            if (!Directory.Exists(toolsPath)) { Directory.CreateDirectory(toolsPath); }
            if (!File.Exists(texConvPath))
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("https://github.com/microsoft/DirectXTex/releases/download/feb2022/texconv.exe", texConvPath);
                }
            }

        }

        private void CopyOriginalDDX()
        {
            resourceFilePath = $@"{resourcesPath}\{numberOfFiles}.dds";
            File.Copy(arguments[1], resourceFilePath, true);
        }

        private void ConvertRessourceDDX()
        {
            TexConv.StartInfo.Arguments = $"\"{resourceFilePath}\" -ft png -y -o \"{resourcesPath}\"";
            TexConv.Start();

            TexConv.WaitForExit();

            if (File.Exists(resourceFilePath)) { File.Delete(resourceFilePath); }
            resourceFileConvertedPath = $@"{resourcesPath}\{numberOfFiles}.png";
        }

        private void DisplayConvertedDDX()
        {
            displayImage = Image.FromFile(resourceFileConvertedPath);
            DisplayBox.Image = displayImage;

            Width = displayImage.Width;
            Height = displayImage.Height;
        }

        private void CleanUp()
        {
            Settings.Default.Save();
            displayImage?.Dispose();
            DisplayBox.Image?.Dispose();
            if (File.Exists(resourceFileConvertedPath)) { File.Delete(resourceFileConvertedPath); }
        }

        private void ThemesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            BackColor = Color.FromName(ThemesList.SelectedItem.ToString());
            Settings.Default.Color = ThemesList.SelectedItem.ToString();

            Settings.Default.Save();
        }

        private void openINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process paint = new Process();

            FileDialogPaint.ShowHelp = false;
            FileDialogPaint.Title = "Select paintdotnet.exe";
            FileDialogPaint.FileName = "paintdotnet.exe";

            if (Settings.Default.PaintPath != "")
            {
                paint.StartInfo.FileName = Settings.Default.PaintPath;
                paint.StartInfo.Arguments = $"{resourceFileConvertedPath}";
                paint.Start();
            }
            else
            {
                if (FileDialogPaint.ShowDialog() == DialogResult.OK)
                {
                    paint.StartInfo.FileName = FileDialogPaint.FileName;
                    paint.StartInfo.Arguments = $"{resourceFileConvertedPath}";
                    paint.Start();

                    Settings.Default.PaintPath = FileDialogPaint.FileName;
                    Settings.Default.Save();
                }
            }
        }
    }
}

