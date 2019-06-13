using System;
using System.Collections.Generic;
using System.Linq;

namespace BranchOfficeBackend
{
    /// <summary>
    /// Implementation of interacting with PostgreSQL database
    /// </summary>
    public class PostgresDataAccessObjectService : IDataAccessObjectService
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(PostgresDataAccessObjectService)); 
        private readonly BranchOfficeDbContext dbContext;

        public PostgresDataAccessObjectService(BranchOfficeDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<Employee> GetAllEmployees()
        {
            return dbContext.Employees.ToList();
        }

        public Employee GetOneEmployee(int id)
        {
            var employees = dbContext.Employees.ToList();
            var oneEmp = employees.Where(obj => obj.EmployeeId == id);
            return (Employee)(oneEmp.FirstOrDefault());
        }

        private void VerifyEmployee(Employee emp)
        {
            if (emp.Email == null || emp.Email == "")
            {
                throw new ArgumentException("Email was empty");
            }
            if (emp.Name == null || emp.Name == "")
            {
                throw new ArgumentException("Name was empty");
            }
        }
        public void AddEmployee(Employee employee, bool keepId=false)
        {
            VerifyEmployee(employee);

            var existingObjects = this.GetAllEmployees();
            var employeeWithEmail = existingObjects.Where(e => e.Email == employee.Email).FirstOrDefault();
            if (employeeWithEmail != null){
                throw new ArgumentException(String.Format("Employee with email: {0} already exists", employee.Email));
            }

            Employee employeeWithId;
            if (keepId) {
                employeeWithId = new Employee(employee, employee.EmployeeId);
            } else {
                int maxId = 0;
                for (int i=0; i<existingObjects.Count(); i++)
                {
                    if (existingObjects[i].EmployeeId > maxId)
                        maxId = existingObjects[i].EmployeeId;
                }
                employeeWithId = new Employee(employee, maxId+1);
            }

            var employeeWithIdExistng = existingObjects.Where(e => e.EmployeeId == employeeWithId.EmployeeId).FirstOrDefault();
            if (employeeWithIdExistng != null){
                throw new ArgumentException(String.Format("Employee with EmployeeId: {0} already exists", employeeWithId.EmployeeId));
            }

            dbContext.Employees.Add(employeeWithId);
            dbContext.SaveChanges();
            _log.Info(String.Format("Employee added to db: {0}", employeeWithId));
        }

        public void DeleteEmployee(int employeeId)
        {
            Employee myobj = GetOneEmployee(employeeId);
            if (myobj == null)
            {
                return;
            }
            dbContext.Employees.Remove(myobj);
            dbContext.SaveChanges();
            _log.Info(String.Format("Employee deleted from db: {0}", myobj));
        }

        public void EditEmployee(Employee emp)
        {
            Employee myobj = GetOneEmployee(emp.EmployeeId);
            if (myobj == null)
            {
                throw new InvalidOperationException("Employee object not found");
            }
            VerifyEmployee(emp);

            this.DeleteEmployee(emp.EmployeeId);
            dbContext.SaveChanges();
            this.AddEmployee(emp,true);
            dbContext.SaveChanges();
            _log.Info(String.Format("Employee edited in db: {0}", emp));
        }

        public List<EmployeeHours> GetEmployeeHoursForAnEmployee(int employeeId)
        {
            var all = dbContext.EmployeeHoursCollection.ToList();
            var selectedByEmployeeId = all.Where(obj => obj.EmployeeId == employeeId);
            return selectedByEmployeeId.ToList();
        }
        public List<EmployeeHours> GetAllEmployeeHours()
        {
            return dbContext.EmployeeHoursCollection.ToList();
        }
        

        public EmployeeHours GetOneEmployeeHours(int employeeHoursId)
        {
            var all = dbContext.EmployeeHoursCollection.ToList();
            var selectedByEmployeeHoursId = all.Where(obj => obj.EmployeeHoursId == employeeHoursId);
            return (EmployeeHours)(selectedByEmployeeHoursId.FirstOrDefault());
        }
        private void VerifyEmployeeHours(EmployeeHours employeeHours)
        {
            int employeeId = employeeHours.EmployeeId;
            if (employeeId == -1)
                throw new ArgumentException("EmployeeId was not set");
            if (employeeHours.Value < 0)
                throw new ArgumentException("Value < 0");

            var existingEmployees = this.GetAllEmployees();
            var employeeWithId = existingEmployees.Where(e => e.EmployeeId == employeeId).FirstOrDefault();
            if (employeeWithId == null)
            {
                throw new ArgumentException(String.Format("Employee with Id: {0} not found", employeeId));
            }
        }

        public void AddEmployeeHours(EmployeeHours employeeHours, bool keepId=false)
        {
            VerifyEmployeeHours(employeeHours);
            EmployeeHours employeeHoursWithId;

            // the IDs for employeeHours are common for all employes, thus use this.GetAllEmployeeHours()
            // instead of this.GetAllEmployeeHours(employeeHours.employeeId)
            var existingObjects = this.GetAllEmployeeHours();

            if (keepId) {
                employeeHoursWithId = new EmployeeHours(employeeHours, employeeHours.EmployeeHoursId);
            } else {
                int maxId = 0;
                for (int i=0; i<existingObjects.Count(); i++)
                {
                    if (existingObjects[i].EmployeeHoursId > maxId)
                        maxId = existingObjects[i].EmployeeHoursId;
                }
                employeeHoursWithId = new EmployeeHours(employeeHours, maxId+1);
            }

            var objWithIdExistng = existingObjects.Where(
                e => e.EmployeeHoursId == employeeHoursWithId.EmployeeHoursId).FirstOrDefault();
            if (objWithIdExistng != null){
                throw new ArgumentException(String.Format("EmployeeHours with EmployeeHoursId: {0} already exists", employeeHoursWithId.EmployeeHoursId));
            }

            var objWithTimePeriodAndEmployeeIdExistng = existingObjects.Where(
                e => (e.TimePeriod == employeeHoursWithId.TimePeriod && e.EmployeeId == employeeHoursWithId.EmployeeId)).FirstOrDefault();
            if (objWithTimePeriodAndEmployeeIdExistng != null){
                throw new ArgumentException(String.Format("EmployeeHours with EmployeeId: {0} and TimePeriod {1} already exists",
                 objWithTimePeriodAndEmployeeIdExistng.EmployeeId, objWithTimePeriodAndEmployeeIdExistng.TimePeriod));
            }

            _log.Debug(String.Format("Adding employeeHours to db: {0}", employeeHoursWithId));
            dbContext.EmployeeHoursCollection.Add(employeeHoursWithId);
            dbContext.SaveChanges();
            _log.Info(String.Format("EmployeeHours added to db: {0}", employeeHoursWithId));
        }

        public void DeleteEmployeeHours(int employeeHoursId)
        {
            EmployeeHours eh = GetOneEmployeeHours(employeeHoursId);
            if (eh == null)
            {
                return;
            }
            dbContext.EmployeeHoursCollection.Remove(eh);
            dbContext.SaveChanges();
            _log.Info(String.Format("EmployeeHours deleted from db: {0}", eh));
        }

        public void EditEmployeeHours(EmployeeHours employeeHours)
        {
            EmployeeHours eh = GetOneEmployeeHours(employeeHours.EmployeeHoursId);
            if (eh == null)
            {
                throw new InvalidOperationException("EmployeeHours object not found");
            }
            VerifyEmployeeHours(employeeHours);

            this.DeleteEmployeeHours(employeeHours.EmployeeHoursId);
            dbContext.SaveChanges();
            this.AddEmployeeHours(employeeHours,true);
            dbContext.SaveChanges();
            _log.Info(String.Format("EmployeeHours edited in db: {0}", employeeHours));
        }

        public void InformOnDBContents()
        {
            var employees = this.GetAllEmployees();
            var eh = this.GetAllEmployeeHours();
            var ss = this.GetAllSalaries();
            _log.InfoFormat("There are {0} Employees, {1} EmployeeHours and {2} Salaries in db",
                 employees.Count, eh.Count, ss.Count);
        }

        public List<Salary> GetAllSalaries()
        {
            return dbContext.Salaries.ToList();
        }

        public List<Salary> GetSalariesForAnEmployee(int employeeId)
        {
            var objs = this.GetAllSalaries();
            var selected = objs.Where(obj => obj.EmployeeId == employeeId);
            return selected.ToList();
        }

        public Salary GetOneSalary(int id)
        {
            var objs = this.GetAllSalaries();
            var selected = objs.Where(obj => obj.SalaryId == id);
            return (Salary)(selected.FirstOrDefault());
        }

        private void VerifySalary(Salary salary)
        {
            int employeeId = salary.EmployeeId;
            if (employeeId == -1)
                throw new ArgumentException("EmployeeId was not set");
            if (salary.Value < 0)
                throw new ArgumentException("Value < 0");

            var existingEmployees = this.GetAllEmployees();
            var employeeWithId = existingEmployees.Where(e => e.EmployeeId == employeeId).FirstOrDefault();
            if (employeeWithId == null)
            {
                throw new ArgumentException(String.Format("Employee with Id: {0} not found", employeeId));
            }
        }

        // no 2 salaries for 1 employee can have the same time period
        public void AddSalary(Salary salary, bool keepId=false)
        {
            VerifySalary(salary);
            Salary salaryWithId;
            var existingObjects = this.GetAllSalaries();

            var objWithExistingTimePeriod = existingObjects.Where(
                e => (e.EmployeeId == salary.EmployeeId && e.TimePeriod == salary.TimePeriod)).FirstOrDefault();
            if (objWithExistingTimePeriod != null){
                throw new ArgumentException(String.Format(
                    "Salary with TimePeriod: {0} for Employee: {1} already exists", salary.TimePeriod, salary.EmployeeId));
            }

            if (keepId) {
                salaryWithId = new Salary(salary, salary.SalaryId);
            } else {
                int maxId = 0;
                for (int i=0; i<existingObjects.Count(); i++)
                {
                    if (existingObjects[i].SalaryId > maxId)
                        maxId = existingObjects[i].SalaryId;
                }
                salaryWithId = new Salary(salary, maxId+1);
            }

            var objWithIdExistng = existingObjects.Where(e => e.SalaryId == salaryWithId.SalaryId).FirstOrDefault();
            if (objWithIdExistng != null){
                throw new ArgumentException(String.Format("Salary with SalaryId: {0} already exists", salaryWithId.SalaryId));
            }

            dbContext.Salaries.Add(salaryWithId);
            dbContext.SaveChanges();
            _log.Info(String.Format("Salary added to db: {0}", salaryWithId));
        }
        public void DeleteSalary(int salaryId) {
            var existingObj = GetOneSalary(salaryId);
            if (existingObj == null)
            {
                return;
            }
            dbContext.Salaries.Remove(existingObj);
            dbContext.SaveChanges();
            _log.Info(String.Format("Salary deleted from db: {0}", existingObj));
        }

        public int GetEmployeeIdByMail(string email)
        {
            var employees = this.GetAllEmployees();
            var oneEmp = employees.Where(obj => obj.Email == email);
            return ((Employee)(oneEmp.FirstOrDefault())).EmployeeId;
        }
    }
}
