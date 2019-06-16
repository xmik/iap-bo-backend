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
        private bool VerifySalaryInCollection(List<Salary> salariesColl, Salary oneSalary)
        {
            var result = salariesColl.Where(
                x => (x.TimePeriod == oneSalary.TimePeriod && x.EmployeeId == oneSalary.EmployeeId)).FirstOrDefault();
            if (result != null) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get information from HQ about: employees and salaries.
        /// Then, ensure that BO db contains only those employees and salaries that also
        /// exist in HQ.
        /// Then, delete any employeeHours and salaries which are connected to a deleted employee. 
        /// </summary>
        /// <returns></returns>
        public async Task Synchronize() 
        {
            if (hqApiClient != null)
            {
                try {
                    lock (this.locker) {
                        if (synchronizing) {
                            _log.Warn("Synchronization skipped, because it is already running");
                            return;
                        }
                        synchronizing = true;
                    }
                    _log.Debug("Starting synchronization");

                    List<HQEmployee> hqEmployees = await hqApiClient.ListEmployees(
                        this.confService.GetBranchOfficeId());
                    if (hqEmployees != null && hqEmployees.Count != 0) {
                        var boEmployees = daoService.GetAllEmployees();

                        // add all the employes from HQ into BO
                        for (int i=0; i< hqEmployees.Count; i++)
                        {
                            var hqEmp = hqEmployees[i];
                            int hqEmpId = hqEmp.ID;

                            var hqAsBoEmployee = new Employee(hqEmp);
                            bool empPresent = VerifyEmployeeInCollection(boEmployees, hqAsBoEmployee);
                            if (!empPresent) {
                                daoService.AddEmployee(hqAsBoEmployee);
                            }
                            // now, if the employee was just added, it will most surely have different Id
                            // We need to know this Id, so that we can assign later a salary
                            // to an employee in BO.
                            int boEmployeeId = daoService.GetEmployeeIdByMail(hqAsBoEmployee.Email);

                            var boSalaries = daoService.GetSalariesForAnEmployee(boEmployeeId);
                            // add all the salaries from HQ into BO
                            List<HQSalary> hqSalaries = await hqApiClient.ListSalariesForEmployee(hqEmpId);
                            if (hqSalaries != null && hqSalaries.Count != 0) {
                                for (int s=0; s< hqSalaries.Count; s++)
                                {
                                    var hqAsBoSalary = new Salary(hqSalaries[s]);
                                    hqAsBoSalary.EmployeeId = boEmployeeId;
                                    bool salaryPresent = VerifySalaryInCollection(boSalaries, hqAsBoSalary);
                                    if (!salaryPresent) {
                                        daoService.AddSalary(hqAsBoSalary);
                                    }
                                }
                            }
                        }

                        // remove all the employees in BO that are not present in HQ
                        boEmployees = daoService.GetAllEmployees();
                        for (int i=0; i< boEmployees.Count; i++)
                        {
                            var boEmp = boEmployees[i];
                            bool empPresent = VerifyEmployeeInCollection(hqEmployees, boEmp);
                            if (!empPresent) {
                                _log.InfoFormat("Deleting Employee {0} together with its related EmployeeHours and Salaries", boEmp.EmployeeId);
                                daoService.DeleteEmployee(boEmp.EmployeeId);
                                // remove all the employeesHours for the deleted employee
                                var employeeHoursColl = daoService.GetEmployeeHoursForAnEmployee(boEmp.EmployeeId);
                                for (int j=0; j< employeeHoursColl.Count; j++)
                                {
                                    daoService.DeleteEmployeeHours(employeeHoursColl[j].EmployeeHoursId);
                                }
                                // remove all the salaries for the deleted employee
                                var salariesColl = daoService.GetSalariesForAnEmployee(boEmp.EmployeeId);
                                for (int j=0; j< salariesColl.Count; j++)
                                {
                                    daoService.DeleteSalary(salariesColl[j].SalaryId);
                                }
                            }
                        }
                    }
                    else {
                        _log.Info("No employees found in headquarters");
                    }
                    _log.Info("Synchronization was successful");
                    this.daoService.InformOnDBContents();
                } catch (System.Net.Http.HttpRequestException ex) {
                    _log.WarnFormat("Synchronizing with HQ failed (is the HQ server running?). Ex: {0}", ex.Message);
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