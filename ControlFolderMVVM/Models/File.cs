namespace ControlFolderMVVM.Models
{
    /// <summary>
    /// Описывает файл с необходимыми атрибутами
    /// </summary>
    public class File
    {
        /// <summary>
        /// имя файла
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// размер файла
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// путь к файлу
        /// </summary>
        public string Path { get; set; } 
    }
}
