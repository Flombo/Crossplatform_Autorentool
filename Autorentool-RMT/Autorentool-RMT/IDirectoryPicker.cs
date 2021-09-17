using Autorentool_RMT.ViewModels;
using System.Threading.Tasks;

namespace Autorentool_RMT
{
    /// <summary>
    /// Interface for the directory-picker implementations.
    /// Is needed for the cross-platform dependency injection.
    /// </summary>
    public interface IDirectoryPicker
    {
        Task ShowFolderPicker(ContentManagementViewModel contentManagementViewModel);
    }
}
