using System;
using System.Collections.Generic;
using System.Linq;
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

        private bool VerifyEmployeeInCollection(List<Employee> employeesColl, Employee oneEmployee)
        {
            var result = employeesColl.Where(x => x.Email == oneEmployee.Email).FirstOrDefault();
            if (result != null) {
                return true;
            }
            // for (int j=0; j< boEmployees.Count; j++)
            // {
            //     if (boEmployees[j].Email == hqAsBoEmployee.Email) {
            //         // employee from HQ already exists in our DB
            //         return true;
            //     }
            // }
            return false;
        }
        private bool VerifyEmployeeInCollection(List<HQEmployee> employeesColl, Employee oneEmployee)
        {
            var result = employeesColl.Where(x => x.Email == oneEmployee.Email).FirstOrDefault();
            if (result != null) {
                return true;
            }
            return false;
        }

// TODO: itest that this can be invoked many times in one moment
// TODO: test how many times a method was invoked
// TODO: test db state

        /// <summary>
        /// Get information from HQ about: employees and salaries.
        /// Then, ensure that BO db contains only those employees and salaries that also
        /// exist in HQ.
        /// Then, delete any employeeHours which are connected to a deleted employee. 
        /// </summary>
        /// <returns></returns>
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

                    List<HQEmployee> hqEmployees = await hqApiClient.ListEmployees(
                        this.confService.GetBranchOfficeId());
                    if (hqEmployees != null) {
                        var boEmployees = daoService.GetAllEmployees();

                        // add all the employes from HQ into BO
                        for (int i=0; i< hqEmployees.Count; i++)
                        {
                            var hqAsBoEmployee = new Employee(hqEmployees[i]);
                            bool empPresent = VerifyEmployeeInCollection(boEmployees, hqAsBoEmployee);
                            if (!empPresent) {
                                daoService.AddEmployee(hqAsBoEmployee);
                            }

                            List<HQSalary> salaries = await hqApiClient.ListSalariesForEmployee(i);
                            // TODO; replace bo db contents with the above salaries
                        }

                        // remove all the employees in BO that are not present in HQ
                        boEmployees = daoService.GetAllEmployees();
                        for (int i=0; i< boEmployees.Count; i++)
                        {
                            var boEmp = boEmployees[i];
                            bool empPresent = VerifyEmployeeInCollection(hqEmployees, boEmp);
                            if (!empPresent) {
                                daoService.DeleteEmployee(boEmp.EmployeeId);
                                var employeeHoursColl = daoService.GetEmployeeHoursForAnEmployee(boEmp.EmployeeId);
                                for (int j=0; j< employeeHoursColl.Count; j++)
                                {
                                    daoService.DeleteEmployeeHours(employeeHoursColl[j].EmployeeHoursId);
                                }
                            }
                        }
                    }
                    _log.Info("Synchronization was successful");
                    this.daoService.InformOnDBContents();
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