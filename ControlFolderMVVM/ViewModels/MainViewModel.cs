using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Media;
using System.Diagnostics;
using ControlFolderMVVM.Models;

namespace ControlFolderMVVM.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        private readonly IControlFolder controlFolder;
        Thread thread;
        ParameterizedThreadStart parameterizedThreadStart;

        public List<File> FileList { get; set; }
        public ObservableCollection<string> LogList { get; set; }

        #region Ui Property
        bool iSdisableFolderChange;
        /// <summary>
        /// признак запрета изменения папки
        /// </summary>
        public bool IsDisableFolderChange
        {
            get
            {
                return iSdisableFolderChange;
            }
            set
            {
                iSdisableFolderChange = value;
                Synchronaizer.IsDisableChangeFolder = value;
            }
        }


        string status = "Не контролюється";
        /// <summary>
        /// состояние контроля
        /// </summary>
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                OnePropertyChanged("Status");
            }
        }

        Brush brush = new SolidColorBrush(Colors.Azure);
        /// <summary>
        /// цвет панели управления
        /// </summary>
        public Brush Brush
        {
            get
            {
                return brush;
            }
            set
            {
                brush = value; OnePropertyChanged("Brush");
            }
        }

        int selectedindex;
        /// <summary>
        /// индекс выбранного элемента ListView
        /// </summary>
        public int SelectedIndex
        {
            get { return selectedindex; }
            set
            {
                selectedindex = value;
                OpenFileCommand.OneExecuteChaneged();
                RemoveFileCommand.OneExecuteChaneged();
                OpenInFileExplorerCommand.OneExecuteChaneged();
            }
        }
        #endregion

        #region Commands 
        /// <summary>
        /// команда начала контроля
        /// </summary>
        public Command StartControlCommand { get; set; }

        /// <summary>
        /// команда открытия файла
        /// </summary>
        public Command OpenFileCommand { get; set; }

        /// <summary>
        /// команда удаления файла
        /// </summary>
        public Command RemoveFileCommand { get; set; }

        /// <summary>
        /// команда открытия файла в проводнике
        /// </summary>
        public Command OpenInFileExplorerCommand { get; set; }
        #endregion


        public MainViewModel()
        {
            FileList = new List<File>();
            LogList = new ObservableCollection<string>();
            controlFolder = new ControlFolderLogic();
            StartControlCommand = new Command(StartControl);
            OpenFileCommand = new Command(OpenFile, FileExist);
            RemoveFileCommand = new Command(RemoveFile, FileExist);
            OpenInFileExplorerCommand = new Command(OpenFileWithFileExplorer, FileExist);
            controlFolder.FileAdded += ControlFolder_FileAdded;
            controlFolder.FileRemoved += ControlFolder_FileRemoved;
            controlFolder.FileChanged += ControlFolder_FileChanged;
            controlFolder.FolderAdded += ControlFolder_FolderAdded;
            controlFolder.FolderRemoved += ControlFolder_FolderRemoved;
            parameterizedThreadStart = new ParameterizedThreadStart(controlFolder.StartControl);
            thread = new Thread(parameterizedThreadStart)
            {
                IsBackground = true
            };
            BindingOperations.EnableCollectionSynchronization(LogList, this);
        }

        /// <summary>
        /// начинает контроль за директорией
        /// </summary>
        /// <param name="parametr"></param>
        void StartControl(object parametr)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult dialogResult = folderBrowserDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                if (thread.IsAlive)
                {
                    thread.Abort();
                    thread = new Thread(parameterizedThreadStart);
                    thread.Start(folderBrowserDialog.SelectedPath);
                }
                else thread.Start(folderBrowserDialog.SelectedPath);
                Status = "Контролюється папка " + folderBrowserDialog.SelectedPath;
                Brush = new SolidColorBrush(Colors.Green);
            }
        }
        /// <summary>
        /// осуществляет открытие выделенного файла программой по умолчанию
        /// </summary>
        /// <param name="parametr"></param>
        void OpenFile(object parametr) 
        {
            try
            {
                Process.Start(FileList[SelectedIndex].Path);
            }
            catch (Exception)
            {
                MessageBox.Show("Не вдається відкрити файл");
            }
        }

        /// <summary>
        /// удаляет выделенный файл
        /// </summary>
        /// <param name="parametr"></param>
        void RemoveFile(object parametr) 
        {
            System.IO.File.Delete(FileList[SelectedIndex].Path);
        }

        /// <summary>
        /// открывает выделенный файл в проводнике
        /// </summary>
        /// <param name="parametr"></param>
        void OpenFileWithFileExplorer(object parametr)
        {
            Process.Start(new ProcessStartInfo("explorer.exe", " /select, " + FileList[SelectedIndex].Path));
        }

        bool FileExist(object parametr) 
        {
            var el = FileList[SelectedIndex];
            return System.IO.File.Exists(el.Path);
        }

        /// <summary>
        /// осуществляет вывод сообщения в системный трей
        /// </summary>
        /// <param name="message">текст сообщения</param>
        void OneNotifiMessage(string message) // метод що виводить передане повідомлення
        {
            Synchronaizer.NotifyIcon.BalloonTipTitle = "Розмір папки змінився";
            Synchronaizer.NotifyIcon.BalloonTipText = message;
            Synchronaizer.NotifyIcon.ShowBalloonTip(5000);
        }

        /// <summary>
        /// обработчик события удаления директории
        /// </summary>
        /// <param name="file"></param>
        private void ControlFolder_FolderRemoved(File file) 
        {
            FileList.Add(file);
            LogList.Add(string.Format("[{0}:{1}:{2} ] папку {3} було видалено, шлях \"{4}\"", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, file.Name, file.Path));
            OneNotifiMessage(string.Format("Папку {0} , було видалено, шлях {1}", file.Name, file.Path));
        }

        /// <summary>
        /// обработчик события добавления директории
        /// </summary>
        /// <param name="file"></param>
        private void ControlFolder_FolderAdded(File file) 
        {
            FileList.Add(file);
            LogList.Add(string.Format("[{0}:{1}:{2} ] папку {3} було додано, шлях \"{4}\"", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, file.Name, file.Path));
            OneNotifiMessage(string.Format("Папку {0} , було додано, шлях {1}", file.Name, file.Path));
        }

        /// <summary>
        /// обработчик события изменения файла
        /// </summary>
        /// <param name="file">файл который был изменен</param>
        private void ControlFolder_FileChanged(File file) // обробник події зміни файлу
        {
            FileList.Add(file);
            LogList.Add(string.Format("[{0}:{1}:{2} ] файл {3} було змінено, шлях \"{4}\"", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, file.Name, file.Path));
            OneNotifiMessage(string.Format("Файл {0} , було змінено, шлях {1}", file.Name, file.Path));
        }

        /// <summary>
        /// обработчик события удаления файла
        /// </summary>
        /// <param name="file">файл который был удален</param>
        private void ControlFolder_FileRemoved(File file)
        {
            FileList.Add(file);
            LogList.Add(string.Format("[{0}:{1}:{2} ] файл {3} було видалено, шлях \"{4}\"", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, file.Name, file.Path));
            OneNotifiMessage(string.Format("Файл {0} , було видалено, шлях {1}", file.Name, file.Path));
        }

        /// <summary>
        /// обработчик события добавления файла
        /// </summary>
        /// <param name="file">файл который был добавлен</param>
        private void ControlFolder_FileAdded(File file)
        {
            FileList.Add(file);
            LogList.Add(string.Format("[{0}:{1}:{2} ] файл {3} було додано, шлях \"{4}\"", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, file.Name, file.Path));
            OneNotifiMessage(string.Format("Файл {0} , було додано, шлях {1}", file.Name, file.Path));
        }
    }
}
