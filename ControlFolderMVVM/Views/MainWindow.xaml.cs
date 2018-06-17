using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControlFolderMVVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Конструктор класу головної форми
        public MainWindow()
        {
            InitializeComponent();
            Synchronaizer.NotifyIcon.Icon = new Icon(SystemIcons.Shield, 40, 40);
            Synchronaizer.NotifyIcon.Visible = true;
            Synchronaizer.NotifyIcon.Click += NotifyIcon_Click;
            Synchronaizer.NotifyIcon.BalloonTipClicked += NotifyIcon_Click;
        }
        //Розгортання головного вікна з системного трею
        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
        }
        //Згортання головного вікна в системний трей
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {

                Hide();
                Synchronaizer.NotifyIcon.BalloonTipTitle = "Програма була схована";

                Synchronaizer.NotifyIcon.BalloonTipText = "Програма схована в трей і продовжить роботу";

                Synchronaizer.NotifyIcon.ShowBalloonTip(5000);
            }
        }
        //Вилучення позначки проекту з системного трею перед завершення роботи проекту
        private void Window_Closed(object sender, EventArgs e)
        {
            Synchronaizer.NotifyIcon.Visible = false;
            Synchronaizer.NotifyIcon.Icon = null;
            Synchronaizer.NotifyIcon.Dispose();
        }
    }
}
