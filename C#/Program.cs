using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace chromiumdl
{
    class Program
    {
        static (string, string, string) GetChromiumDownloadUrls()
        {
            WebClient client = new WebClient();
            string os = "UNKNOWN";
            string chromedriverUrl = String.Empty;
            string chromiumUrl = String.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                os = "Linux";
                string chromium_last_v_url =
                    "https://www.googleapis.com/download/storage/v1/b/chromium-browser-snapshots/o/Linux_x64%2FLAST_CHANGE?alt=media";
                string chromiumLastV = client.DownloadString(chromium_last_v_url);
                chromedriverUrl =
                    "https://www.googleapis.com/download/storage/v1/b/chromium-browser-snapshots/o/Linux_x64%2F" +
                    chromiumLastV + "%2Fchromedriver_linux64.zip?&alt=media";
                chromiumUrl =
                    "https://www.googleapis.com/download/storage/v1/b/chromium-browser-snapshots/o/Linux_x64%2F" +
                    chromiumLastV + "%2Fchrome-linux.zip?alt=media";
            }

            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                os = "Windows";
                string chromium_last_v_url =
                    "https://www.googleapis.com/download/storage/v1/b/chromium-browser-snapshots/o/Win%2FLAST_CHANGE?&alt=media";
                string chromiumLastV = client.DownloadString(chromium_last_v_url);
                chromedriverUrl =
                    "https://www.googleapis.com/download/storage/v1/b/chromium-browser-snapshots/o/Win%2F" +
                    chromiumLastV + "%2Fchromedriver_win32.zip?&alt=media";
                chromiumUrl =
                    "https://www.googleapis.com/download/storage/v1/b/chromium-browser-snapshots/o/Win%2F" +
                    chromiumLastV + "%2Fchrome-win.zip?alt=media";
            }

            return (os, chromedriverUrl, chromiumUrl);
        }

        static MemoryStream DownloadToMs(string url)
        {
            WebClient client = new WebClient();
            MemoryStream stream = new MemoryStream(client.DownloadData(url));
            return stream;
        }

        static ZipArchive DownloadArchiveToMemory(string url)
        {
            MemoryStream data = DownloadToMs(url);
            ZipArchive zip = new ZipArchive(data);
            return zip;
        }

        static void Main()
        {
            var downloadUrls = GetChromiumDownloadUrls();
            string os = downloadUrls.Item1;
            string chromedriverUrl = downloadUrls.Item2;
            string chromiumUrl = downloadUrls.Item3;
            Console.WriteLine("Detected OS: " + os);
            Console.WriteLine("Downloading Chromium");
            ZipArchive chromiumzip = DownloadArchiveToMemory(chromiumUrl);
            Console.WriteLine("Chromium finished downloading");
            Console.WriteLine("Downloading Chromedriver");
            ZipArchive chromedriverzip = DownloadArchiveToMemory(chromedriverUrl);
            Console.WriteLine("Chromedriver finished downloading");
            if (Directory.Exists("chromium"))
            {
                Console.WriteLine("Replacing old chromium with new one");
                Directory.Delete("chromium", true);
            }
            Directory.CreateDirectory("chromium");
            chromiumzip.ExtractToDirectory("chromium");
            chromedriverzip.ExtractToDirectory("chromium");
            Console.WriteLine("Done");
        }
    }
}
