using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlFolderMVVM.Models
{
    public interface IFileSystemElement
    {
        string Name { get; set; }

        string Path { get; set; }

        long Size { get; set; }
    }
}
