using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BranchOfficeBackend
{
    // to be treated as a Singleton class, Autofac provides that
    public class SynchronizatorService : ISynchronizatorService, IDisposable
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(SynchronizatorService)); 
        private readonly IHQAPIClient hqApiClient;
        private readonly IConfigurationService confService;
        private readonly IDataAccessObjectService daoService;
        private readonly CancellationTokenSource cts;
        private bool synchronizing;
        private readonly Object locker = new Object();

        public SynchronizatorService(IHQAPIClient hqApiClient, IConfigurationService confService, IDataAccessObjectService daoService)
        {
            this.hqApiClient = hqApiClient;
            this.confService = confService;
            this.daoService = daoService;
            cts = new CancellationTokenSource();

            if (confService.GetStartSynchronizationLoopWithHQ())
            {
                Task.Factory.StartNew(() => 
                    SynchronizationLoopTimer());
            }
        }

// TODO: test frequency
        public async Task SynchronizationLoopTimer()
        {
            int frequency_seconds = this.confService.GetSynchronizationFrequency();
            while(true)
            {   
                try {         
                    if (cts.Token.IsCancellationRequested) 
                    {
                        _log.Info("Synchronization service disposed, exit");
                        return;
                    }
                    _log.InfoFormat("Synchronizing with HQ (every {0} seconds)", frequency_seconds.ToString());
                    await Synchronize();
                    await Task.Delay(frequency_seconds*1000);
                } catch (Exception ex) {
                    _log.Error("Ignoring exception in synchronization loop", ex);
                }
            }
        }

// TODO: itest that this can be invoked many times in one moment
// TODO: test how many times a method was invoked
// TODO: test db state
        public async Task Synchronize() 
        {
            if (hqApiClient != null)
            {
                try {
                    lock (this.locker) {
                        if (synchronizing) {
                            _log.Warn("Already synchronizing in this moment. Synchronization skipped");
                            return;
                        }
                        synchronizing = true;
                    }
                    _log.Info("Starting synchronization");

                    List<HQEmployee> employees = await hqApiClient.ListEmployees(
                        this.confService.GetBranchOfficeId());
                    if (employees != null) {
                        // TODO; replace bo db contents with the above employees
                        for (int i=0; i< employees.Count; i++)
                        {
                            List<HQSalary> salaries = await hqApiClient.ListSalariesForEmployee(i);
                            // TODO; replace bo db contents with the above salaries
                        }
                    }
                } catch (System.Net.Http.HttpRequestException ex) {
                    _log.Error("Synchronizing with HQ failed (is the HQ server running?).", ex);
                } finally {
                    synchronizing = false;
                }
            }
        }


        public void Dispose()
        {
            cts.Cancel();
            this.hqApiClient.Dispose();
        }
    }
}