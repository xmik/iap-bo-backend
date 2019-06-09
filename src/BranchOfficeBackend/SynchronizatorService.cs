using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BranchOfficeBackend
{
    // to be treated as a Singleton class, Autofac provides that
    public class SynchronizatorService : ISynchronizatorService, IDisposable
    {
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
                        Console.WriteLine("Synchronization service disposed, exit");
                        return;
                    }
                    Console.WriteLine("Synchronizing with HQ (every {0} seconds)", frequency_seconds.ToString());
                    await Synchronize();
                    await Task.Delay(frequency_seconds*1000);
                } catch (Exception ex) {
                    Console.WriteLine("Ignoring exception in synchronization loop: {0}", ex);
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
                            Console.WriteLine("Already synchronizing in this moment. Synchronization skipped");
                            return;
                        }
                        synchronizing = true;
                    }

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
                    Console.WriteLine("Synchronizing with HQ failed (is the HQ server running?). Ex: {0}", ex);
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