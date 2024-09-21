using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DynamicWall
{
    public partial class Form1 : Form
    {
        private ImageList imageList; // To hold images for the ListView

        public Form1()
        {
            InitializeComponent();
            InitializeImageList();
        }

        private void InitializeImageList()
        {
            imageList = new ImageList();
            imageList.ImageSize = new Size(150, 100); // Set the size for the images in the ListView
            listView1.LargeImageList = imageList; // Assign the ImageList to the ListView
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            updateStatus();
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
            }
            else
            {
                DynamicWallpaper.Initialize();
                DynamicWallpaper.wallpaperTimer.Start();
                DynamicWallpaper.isTimerRunning = true;
            }
            updateStatus();
        }

        public void updateStatus()
        {
            label1.Text = (DynamicWallpaper.isTimerRunning) ? "App is running..." : "App Stopped";
            startButton.Text = (DynamicWallpaper.isTimerRunning) ? "Stop" : "Start";
        }

        private void button1_Click(object sender, EventArgs e)
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
                var filePaths = openFileDialog.FileNames;
                listView1.Items.Clear();
                imageList.Images.Clear(); // Clear previous images

                foreach (string fileName in filePaths)
                {
                    Image image = Image.FromFile(fileName);

                    imageList.Images.Add(image); // Add the image to the ImageList
                    ListViewItem item = new ListViewItem(Path.GetFileName(fileName))
                    {
                        ImageIndex = imageList.Images.Count - 1, // Use the index of the added image
                        Tag = fileName
                    };
                    listView1.Items.Add(item);
                }

                DynamicWallpaper.changePrpos(filePaths);
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

    }
}
