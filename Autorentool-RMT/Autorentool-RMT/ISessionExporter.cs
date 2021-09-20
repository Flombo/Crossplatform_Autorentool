using Autorentool_RMT.ViewModels;
using Autorentool_RMT.Models;
using System.Threading.Tasks;

namespace Autorentool_RMT
{
    public interface ISessionExporter
    {
        Task<bool> ExportSession(SessionViewModel sessionViewModel, Session selectedSession);

    }
}
