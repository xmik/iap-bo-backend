using System.Threading.Tasks;

namespace BranchOfficeBackend
{
    public interface ISynchronizatorService
    {
        Task Synchronize();
    }
}