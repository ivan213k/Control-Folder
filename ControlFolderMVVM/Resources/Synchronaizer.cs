using System.Drawing;
using System.Windows.Forms;

namespace ControlFolderMVVM
{
    static class Synchronaizer
    {
        public static bool IsDisableChangeFolder { get; set; }
        public static NotifyIcon NotifyIcon { get; set; }

        static Synchronaizer()
        {
            NotifyIcon = new NotifyIcon
            {
                Icon = new Icon(SystemIcons.Shield, 40, 40),
                Visible = true
            };
        }
    }
}
