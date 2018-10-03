namespace ControlFolderMVVM.Models
{
    
    public delegate void ChangeFolder(IFileSystemElement file);
    /// <summary>
    /// События и методы для контроля за директорией
    /// </summary>
    public interface IControlFolder
    {
        /// <summary>
        /// инициируется в случае добавления файла
        /// </summary>
        event ChangeFolder FileAdded;

        /// <summary>
        /// инициируется в случае удаления файла
        /// </summary>
        event ChangeFolder FileRemoved;

        /// <summary>
        /// инициируется при изменении файла
        /// </summary>
        event ChangeFolder FileChanged;

        /// <summary>
        /// инициируется в случае добавления директории
        /// </summary>
        event ChangeFolder FolderAdded;

        /// <summary>
        /// инициируется в случае удаления директории 
        /// </summary>
        event ChangeFolder FolderRemoved;

        /// <summary>
        /// Запускает контроль за директорией
        /// </summary>
        /// <param name="path">путь к директории</param>
        void StartControl(object path);
    }
}
