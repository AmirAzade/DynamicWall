using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DynamicWall
{
    public partial class Form1 : Form
    {
        string filePath = "last_files.txt";
        string[] filePaths;

        private ImageList imageList;

        public Form1()
        {
            InitializeComponent();
            InitializeImageList();
        }

        private void InitializeImageList()
        {
            imageList = new ImageList();
            imageList.ImageSize = new Size(150, 100);
            listView1.LargeImageList = imageList;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            updateStatus();

            filePaths = LoadList();

            listView1.Items.Clear();
            imageList.Images.Clear();

            foreach (string fileName in filePaths)
            {
                Image image = Image.FromFile(fileName);

                imageList.Images.Add(image);
                ListViewItem item = new ListViewItem(Path.GetFileName(fileName))
                {
                    ImageIndex = imageList.Images.Count - 1,
                    Tag = fileName
                };
                listView1.Items.Add(item);
            }

            DynamicWallpaper.changePrpos(filePaths);

            if(Properties.Settings.Default.runing)
            {
                startButton_Click(this, EventArgs.Empty);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000, "App is Running", "Your dynamic wallpaper app is running in the background.", ToolTipIcon.Info);
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (DynamicWallpaper.isTimerRunning)
            {
                DynamicWallpaper.wallpaperTimer.Stop();
                DynamicWallpaper.isTimerRunning = false;
                Properties.Settings.Default.runing = false;
                Properties.Settings.Default.Save();
            }
            else
            {
                DynamicWallpaper.Initialize();
                DynamicWallpaper.wallpaperTimer.Start();
                DynamicWallpaper.isTimerRunning = true;
                Properties.Settings.Default.runing = true;
                Properties.Settings.Default.Save();
            }
            updateStatus();
        }

        public void updateStatus()
        {
            label1.Text = (DynamicWallpaper.isTimerRunning) ? "App is running..." : "App Stopped";
            startButton.Text = (DynamicWallpaper.isTimerRunning) ? "Stop" : "Start";
        }

        private void BrowsButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                CheckFileExists = false,
                ValidateNames = false,
                CheckPathExists = false,
                Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Select an Image File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePaths = openFileDialog.FileNames;
                listView1.Items.Clear();
                imageList.Images.Clear();

                foreach (string fileName in filePaths)
                {
                    Image image = Image.FromFile(fileName);

                    imageList.Images.Add(image);
                    ListViewItem item = new ListViewItem(Path.GetFileName(fileName))
                    {
                        ImageIndex = imageList.Images.Count - 1,
                        Tag = fileName
                    };
                    listView1.Items.Add(item);
                }

                DynamicWallpaper.changePrpos(filePaths);

                SaveList(filePaths);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string filePath = listView1.SelectedItems[0].Tag.ToString();
                pictureBox1.BackgroundImage = Image.FromFile(filePath);
            }
        }

        private void SaveList(string[] stringList)
        {
            try
            {
                File.WriteAllLines(filePath, stringList);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving the list: " + ex.Message);
            }
        }

        private string[] LoadList()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return File.ReadAllLines(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading the list: " + ex.Message);
            }

            return null;
        }

    }
}
