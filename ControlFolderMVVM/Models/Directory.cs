namespace ControlFolderMVVM.Models
{
    /// <summary>
    /// Описывает директорию с необходимыми атрибутами
    /// </summary>
    public class Directory
    {
        /// <summary>
        /// имя директории
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// размер директории
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// путь к директории
        /// </summary>
        public string Path { get; set; }
    }
}
