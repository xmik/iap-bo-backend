using Carter;

namespace BranchOfficeBackend
{
    public class SynchronizationAPIModule : CarterModule
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(SynchronizationAPIModule)); 
        public SynchronizationAPIModule(ISynchronizatorService service)
        {
            Post("/api/synchronize", async(req, res, routeData) => {
                _log.Debug("Received HTTP request: POST /api/synchronize");
                await service.Synchronize();
            });

        }
    }
}