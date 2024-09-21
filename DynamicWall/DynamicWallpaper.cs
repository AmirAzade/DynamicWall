using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DynamicWall
{
    class DynamicWallpaper
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDCHANGE = 0x02;
        const int totalHour = 24;
        const int totalMinuts = totalHour * 60;
        const int totalSeconds = totalMinuts * 60;

        public static Timer wallpaperTimer;
        static string[] imageFiles;
        static int currentImageIndex = 0;
        static int changeDelaySecond = 10;
        static int changeDelayHour = 1;
        public static bool isTimerRunning = false;

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //string folderPath = GetFolderPathFromUser();
            //if (string.IsNullOrEmpty(folderPath))
            //{
            //    MessageBox.Show("No folder selected. Exiting...");
            //    return;
            //}

            //imageFiles = GetImageFilesFromFolder(folderPath);
            //if (imageFiles.Length == 0)
            //{
            //    MessageBox.Show("No images found in the selected folder. Exiting...");
            //    return;
            //}

            //Initialize();

            var frm = new Form1();

            wallpaperTimer = new Timer();
            wallpaperTimer.Interval = changeDelaySecond * 1000;
            wallpaperTimer.Tick += new EventHandler(OnTimerTick);
            //isTimerRunning = true;
            //frm.updateStatus();
            //wallpaperTimer.Start();

            Application.Run(frm);
        }

        public static void changePrpos(string[] imagePaths)
        {
            if (imagePaths.Length == 0)
            {
                MessageBox.Show("No images found in the selected folder. Exiting...");
                return;
            }

            imageFiles = imagePaths;

        }

        static string GetFolderPathFromUser()
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select folder containing your wallpaper images";
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    return folderBrowserDialog.SelectedPath;
                }
                return null;
            }
        }

        static string[] GetImageFilesFromFolder(string folderPath)
        {
            string[] supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp" };
            return Directory.GetFiles(folderPath)
                            .Where(file => supportedExtensions.Contains(Path.GetExtension(file).ToLower()))
                            .ToArray();
        }

        static void OnTimerTick(object sender, EventArgs e)
        {
            if (imageFiles.Length == 0)
                return;

            string wallpaperPath = imageFiles[currentImageIndex];

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaperPath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

            currentImageIndex = (currentImageIndex + 1) % imageFiles.Length;
        }

        public static void Initialize()
        {
            changeDelaySecond = totalSeconds / imageFiles.Length;
            changeDelayHour = totalHour / imageFiles.Length;

            var startIndex = (DateTime.Now.Hour / changeDelayHour);

            if (startIndex < 0)
                startIndex = 0;
            else if (startIndex > imageFiles.Length - 1)
                startIndex = imageFiles.Length - 1;

            currentImageIndex = startIndex;

            changeDelaySecond /= (360 * 3 * 5);

            string wallpaperPath = imageFiles[currentImageIndex];
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaperPath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
            currentImageIndex = (currentImageIndex + 1) % imageFiles.Length;
        }
    }
}
