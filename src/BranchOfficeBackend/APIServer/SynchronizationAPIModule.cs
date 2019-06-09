using Carter;

namespace BranchOfficeBackend
{
    public class SynchronizationAPIModule : CarterModule
    {
        public SynchronizationAPIModule(ISynchronizatorService service)
        {
            Post("/api/synchronize", async(req, res, routeData) => {
                await service.Synchronize();
            });

        }
    }
}