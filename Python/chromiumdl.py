from requests import get
import os
import sys
from io import BytesIO
from zipfile import ZipFile
import shutil


def getChromiumDownloadUrls(os):
    if (os == "posix"):
        chromium_last_v_url = "https://www.googleapis.com/download/storage/v1/b/" \
                              "chromium-browser-snapshots/o/Linux_x64%2FLAST_CHANGE?alt=media"
        chromium_last_v = get(chromium_last_v_url).text
        chrome_driver_url = "https://www.googleapis.com/download/storage/v1/b/chromium-browser-snapshots/o/Linux_x64%2F" +\
                            chromium_last_v + "%2Fchromedriver_linux64.zip?&alt=media"
        chromium_url = "https://www.googleapis.com/download/storage/v1/b/chromium-browser-snapshots/o/Linux_x64%2F" +\
                       chromium_last_v + "%2Fchrome-linux.zip?alt=media"

    elif (os == "win32"):
        chromium_last_v_url = "https://www.googleapis.com/download/storage/v1/b/" \
                              "chromium-browser-snapshots/o/Win%2FLAST_CHANGE?&alt=media"
        chromium_last_v = get(chromium_last_v_url).text
        chrome_driver_url = "https://www.googleapis.com/download/storage/v1/b/chromium-browser-snapshots/o/Win%2F" +\
                            chromium_last_v + "%2Fchromedriver_win32.zip?&alt=media"
        chromium_url = "https://www.googleapis.com/download/storage/v1/b/chromium-browser-snapshots/o/Win%2F" +\
                       chromium_last_v + "%2Fchrome-win.zip?alt=media"

    return chrome_driver_url, chromium_url


def downloadArchiveToMemory(url):
    data = get(url).content
    zip = ZipFile(BytesIO(data))
    return zip;


def chromiumdl(path, removeold=True):
    chromium_path = os.path.join(path, 'chromium')
    if os.path.isdir(chromium_path):
        if removeold:
            shutil.rmtree(chromium_path)
        else:
            return
    os.mkdir(chromium_path)
    operating_system = sys.platform
    os_parse = {"win32": "Windows", "posix": "linux"}
    chrome_driver_url, chromium_url = getChromiumDownloadUrls(operating_system)
    print("Detected OS: " + os_parse[operating_system])
    print("Downloading Chromium")
    chromiumzip = downloadArchiveToMemory(chromium_url)
    print("Chromium finished downloading")
    print("Downloading Chromedriver")
    chromedriverzip = downloadArchiveToMemory(chrome_driver_url)
    print("Chromedriver finished downloading")
    print("Extracting Chromium")
    chromiumzip.extractall(chromium_path)
    print("Chromium extracted")
    print("Extracting Chromedriver")
    chromedriverzip.extractall(chromium_path)
    print("Chromedriver extracted")
    print("Done")


if __name__ == "__main__":
    current_dirrectory = os.getcwd()
    chromiumdl(current_dirrectory)
