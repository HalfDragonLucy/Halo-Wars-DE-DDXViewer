using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DDXViewer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MainPanel = new System.Windows.Forms.Panel();
            this.DisplayBox = new System.Windows.Forms.PictureBox();
            this.TopMenu = new System.Windows.Forms.MenuStrip();
            this.OpenInMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.PaintNetItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ThemesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ColorPickerItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CompressionMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.MaxCompression = new System.Windows.Forms.ToolStripMenuItem();
            this.GoodCompression = new System.Windows.Forms.ToolStripMenuItem();
            this.MediumCompression = new System.Windows.Forms.ToolStripMenuItem();
            this.PoorCompression = new System.Windows.Forms.ToolStripMenuItem();
            this.FileDialog = new System.Windows.Forms.OpenFileDialog();
            this.ColorPicker = new System.Windows.Forms.ColorDialog();
            this.MainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DisplayBox)).BeginInit();
            this.TopMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainPanel
            // 
            this.MainPanel.Controls.Add(this.DisplayBox);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(0, 24);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(1008, 961);
            this.MainPanel.TabIndex = 0;
            // 
            // DisplayBox
            // 
            this.DisplayBox.BackColor = System.Drawing.Color.Transparent;
            this.DisplayBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DisplayBox.Location = new System.Drawing.Point(0, 0);
            this.DisplayBox.Name = "DisplayBox";
            this.DisplayBox.Size = new System.Drawing.Size(1008, 961);
            this.DisplayBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.DisplayBox.TabIndex = 0;
            this.DisplayBox.TabStop = false;
            this.DisplayBox.WaitOnLoad = true;
            // 
            // TopMenu
            // 
            this.TopMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenInMenu,
            this.ThemesMenu,
            this.CompressionMenu});
            this.TopMenu.Location = new System.Drawing.Point(0, 0);
            this.TopMenu.Name = "TopMenu";
            this.TopMenu.Size = new System.Drawing.Size(1008, 24);
            this.TopMenu.TabIndex = 1;
            this.TopMenu.Text = "TopMenu";
            // 
            // OpenInMenu
            // 
            this.OpenInMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PaintNetItem});
            this.OpenInMenu.Image = global::DDXViewer.Properties.Resources.OpenFile;
            this.OpenInMenu.Name = "OpenInMenu";
            this.OpenInMenu.Size = new System.Drawing.Size(77, 20);
            this.OpenInMenu.Text = "Open in";
            // 
            // PaintNetItem
            // 
            this.PaintNetItem.Image = global::DDXViewer.Properties.Resources.Pencil;
            this.PaintNetItem.Name = "PaintNetItem";
            this.PaintNetItem.Size = new System.Drawing.Size(123, 22);
            this.PaintNetItem.Text = "Paint.Net";
            this.PaintNetItem.Click += new System.EventHandler(this.OpenInPaintDotNet);
            // 
            // ThemesMenu
            // 
            this.ThemesMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ColorPickerItem});
            this.ThemesMenu.Image = global::DDXViewer.Properties.Resources.ColorDialog;
            this.ThemesMenu.Name = "ThemesMenu";
            this.ThemesMenu.Size = new System.Drawing.Size(76, 20);
            this.ThemesMenu.Text = "Themes";
            // 
            // ColorPickerItem
            // 
            this.ColorPickerItem.Image = global::DDXViewer.Properties.Resources.ColorPicker;
            this.ColorPickerItem.Name = "ColorPickerItem";
            this.ColorPickerItem.Size = new System.Drawing.Size(138, 22);
            this.ColorPickerItem.Text = "Color Picker";
            this.ColorPickerItem.Click += new System.EventHandler(this.ThemeColorSelection);
            // 
            // CompressionMenu
            // 
            this.CompressionMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MaxCompression,
            this.GoodCompression,
            this.MediumCompression,
            this.PoorCompression});
            this.CompressionMenu.Image = global::DDXViewer.Properties.Resources.Version;
            this.CompressionMenu.Name = "CompressionMenu";
            this.CompressionMenu.Size = new System.Drawing.Size(105, 20);
            this.CompressionMenu.Text = "Compression";
            // 
            // MaxCompression
            // 
            this.MaxCompression.Image = global::DDXViewer.Properties.Resources.FileSystemEditor;
            this.MaxCompression.Name = "MaxCompression";
            this.MaxCompression.Size = new System.Drawing.Size(119, 22);
            this.MaxCompression.Text = "Max";
            this.MaxCompression.Click += new System.EventHandler(this.CompressionMaxRate);
            // 
            // GoodCompression
            // 
            this.GoodCompression.Image = global::DDXViewer.Properties.Resources.FileSystemEditor;
            this.GoodCompression.Name = "GoodCompression";
            this.GoodCompression.Size = new System.Drawing.Size(119, 22);
            this.GoodCompression.Text = "Good";
            this.GoodCompression.Click += new System.EventHandler(this.CompressionGoodRate);
            // 
            // MediumCompression
            // 
            this.MediumCompression.Image = global::DDXViewer.Properties.Resources.FileSystemEditor;
            this.MediumCompression.Name = "MediumCompression";
            this.MediumCompression.Size = new System.Drawing.Size(119, 22);
            this.MediumCompression.Text = "Medium";
            this.MediumCompression.Click += new System.EventHandler(this.CompressionMediumRate);
            // 
            // PoorCompression
            // 
            this.PoorCompression.Image = global::DDXViewer.Properties.Resources.FileSystemEditor;
            this.PoorCompression.Name = "PoorCompression";
            this.PoorCompression.Size = new System.Drawing.Size(119, 22);
            this.PoorCompression.Text = "Poor";
            this.PoorCompression.Click += new System.EventHandler(this.CompressionPoorRate);
            // 
            // FileDialog
            // 
            this.FileDialog.FileName = "FileDialog";
            // 
            // ColorPicker
            // 
            this.ColorPicker.SolidColorOnly = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1008, 985);
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.TopMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.TopMenu;
            this.Name = "MainForm";
            this.Text = "Halo Wars DE: DDXViewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.MainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DisplayBox)).EndInit();
            this.TopMenu.ResumeLayout(false);
            this.TopMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.PictureBox DisplayBox;
        private System.Windows.Forms.MenuStrip TopMenu;
        private System.Windows.Forms.OpenFileDialog FileDialog;
        private System.Windows.Forms.ToolStripMenuItem ThemesMenu;
        private System.Windows.Forms.ToolStripMenuItem OpenInMenu;
        private System.Windows.Forms.ToolStripMenuItem PaintNetItem;
        private System.Windows.Forms.ColorDialog ColorPicker;
        private System.Windows.Forms.ToolStripMenuItem ColorPickerItem;
        private System.Windows.Forms.ToolStripMenuItem CompressionMenu;
        private System.Windows.Forms.ToolStripMenuItem MaxCompression;
        private System.Windows.Forms.ToolStripMenuItem GoodCompression;
        private System.Windows.Forms.ToolStripMenuItem MediumCompression;
        private System.Windows.Forms.ToolStripMenuItem PoorCompression;
    }
}

