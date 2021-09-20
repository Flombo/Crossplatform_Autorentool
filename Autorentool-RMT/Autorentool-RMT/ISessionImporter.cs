using Autorentool_RMT.ViewModels;
using System.Threading.Tasks;

namespace Autorentool_RMT
{
    public interface ISessionImporter
    {
        Task<bool> ImportSession(SessionViewModel sessionViewModel);

    }
}
