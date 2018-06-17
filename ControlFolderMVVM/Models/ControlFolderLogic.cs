using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ControlFolderMVVM.Models
{
    /// <summary>
    /// События и методы для контроля за директорией
    /// </summary>
    public class ControlFolderLogic : IControlFolder
    {
        /// <summary>
        /// интервал проверки
        /// </summary>
        const int CheckInterval = 500;

        /// <summary>
        /// инициируется в случае добавления файла
        /// </summary>
        public event ChangeFolder FileAdded;

        /// <summary>
        /// инициируется в случае удаления файла
        /// </summary>
        public event ChangeFolder FileRemoved;

        /// <summary>
        /// инициируется при изменении файла
        /// </summary>
        public event ChangeFolder FileChanged;

        /// <summary>
        /// инициируется в случае добавления директории
        /// </summary>
        public event ChangeFolder FolderAdded;

        /// <summary>
        /// инициируется в случае удаления директории 
        /// </summary>
        public event ChangeFolder FolderRemoved;

        #region Private
        private long controlDirectorySize;

        List<File> fileList;

        List<Directory> directoryList;
        #endregion

        /// <summary>
        /// определяет размер директории
        /// </summary>
        /// <param name="directory">Директория размер которой нужно определить</param>
        /// <returns>размер директории</returns>
        public long DirSize(DirectoryInfo directory)               
        {
            long size = 0;
            FileInfo[] files = directory.GetFiles();
            
            foreach (FileInfo file in files)
            {
                size += file.Length;
            }
            DirectoryInfo[] directorys = directory.GetDirectories();
            foreach (DirectoryInfo dir in directorys)
            {
                size += DirSize(dir);
            }
            return (size);
        }

        /// <summary>
        /// инициализирует список файлов с заданного пути
        /// </summary>
        /// <param name="path">путь</param>
        /// <returns>список файлов</returns>
        List<File> InitFileList(string path) 
        {
            var files = System.IO.Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            List<File> filelist = new List<File>();
            foreach (var item in files)
            {
                FileInfo fileInfo = new FileInfo(item);
                File myFile = new File
                {
                    Path = item,
                    Name = fileInfo.Name,
                    Size = fileInfo.Length
                };
                filelist.Add(myFile);
            }
            return filelist;
        }
        /// <summary>
        /// инициализирует список директорий с заданного пути
        /// </summary>
        /// <param name="path">путь</param>
        /// <returns>список директорий</returns>
        List<Directory> InitDirectoryList(string path) 
        {
            var directorys = System.IO.Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
            List<Directory> directorylist = new List<Directory>();
            foreach (var dir in directorys)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                Directory myFile = new Directory
                {
                    Path = dir,
                    Name = directoryInfo.Name,
                    Size = DirSize(directoryInfo)
                };
                directorylist.Add(myFile);
            }
            return directorylist;
        }
        /// <summary>
        /// Проверяет наличие файла в списке
        /// </summary>
        /// <param name="files">список файлов</param>
        /// <param name="file"></param>
        /// <returns>В случае присутствия файла возвращает true</returns>
        bool ContainsItem(IEnumerable<File> files, File file) 
        {
            foreach (var item in files)
            {
                if (item.Name == file.Name)
                {
                    return true;
                }
            }
            return false;
        }

        bool ContainsItem(IEnumerable<Directory> directorys, Directory directory)
        {
            foreach (var item in directorys)
            {
                if (item.Name == directory.Name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// проверяет папку на предмет изменений
        /// </summary>
        /// <param name="path">путь к папке</param>
        /// <returns>если содержимое папки изменилось возвращает true</returns>
        bool CheckDirectoryChange(string path) 
        {
            var currentdirectorylist = InitDirectoryList(path);
            if (directoryList.Count < currentdirectorylist.Count) // если папку было добавлено
            {
                foreach (var item in currentdirectorylist)
                {
                    if (!ContainsItem(directoryList, item))
                    {
                        FolderAdded.Invoke(item);
                        if (Synchronaizer.IsDisableChangeFolder)
                        {
                            System.IO.Directory.Delete(item.Path, true);
                            FileRemoved.Invoke(item);
                            currentdirectorylist.Remove(item);
                        }
                    }
                    break;
                }
                directoryList = currentdirectorylist;
                fileList = InitFileList(path);
                return true;
            }
            else if (directoryList.Count > currentdirectorylist.Count) // если папку было удалено

            {
                foreach (var item in directoryList)
                {
                    if (!ContainsItem(currentdirectorylist, item))
                    {
                        FolderRemoved.Invoke(item);
                        break;
                    }
                }
                directoryList = currentdirectorylist;
                fileList = InitFileList(path);
                return true;
            }
            return false;
        }

        /// <summary>
        /// начинает контроль за заданной папкой
        /// </summary>
        /// <param name="path">путь к папке</param>
        public void StartControl(object path) 
        {

            string _path = path as string;
            controlDirectorySize = DirSize(new DirectoryInfo(_path));
            fileList = InitFileList(_path);
            directoryList = InitDirectoryList(_path);
            while (true)
            {
                if (controlDirectorySize != DirSize(new DirectoryInfo(_path))) // если размер папки изменился
                {
                    if (!CheckDirectoryChange(_path)) // если в контролируемую директорию не было добавлено или удалено ПАПКУ
                    {
                        List<File> currentlist = InitFileList(_path);
                        if (fileList.Count < currentlist.Count) //если файл был добавлен
                        {
                            foreach (var item in currentlist)
                            {
                                if (!ContainsItem(fileList, item))
                                {
                                    FileAdded.Invoke(item);
                                    if (Synchronaizer.IsDisableChangeFolder)
                                    {
                                        System.IO.File.Delete(item.Path);
                                        FileRemoved.Invoke(item);

                                    }
                                }
                            }
                            
                        }
                        else if (fileList.Count > currentlist.Count) // если файл был удален
                        {
                            foreach (var item in fileList)
                            {
                                if (!ContainsItem(currentlist, item))
                                {
                                    FileRemoved.Invoke(item);
                                }
                            }
                        }

                        else // размер файла был изменен
                        {
                            for (int i = 0; i < currentlist.Count; i++)
                            {
                                if (fileList[i].Size != currentlist[i].Size) // дизнаемся размер которого файла было изменено
                                {
                                    FileChanged.Invoke(fileList[i]);
                                }
                            }
                        }
                        controlDirectorySize = DirSize(new DirectoryInfo(_path));
                        fileList = InitFileList(_path);
                    }
                }
                Thread.Sleep(CheckInterval); // интервал проверки
            }
        }
    }
}
