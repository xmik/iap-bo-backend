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

        public Employee GetEmployee(int id)
        {
            var employees = dbContext.Employees.ToList();
            var oneEmp = employees.Where(obj => obj.EmployeeId == id);
            return (Employee)(oneEmp.FirstOrDefault());
        }

        private void VerifyEmployee(Employee emp)
        {
            if (emp.Email == "")
            {
                throw new ArgumentException("Email was empty");
            }
        }
        public void AddEmployee(Employee employee, bool keepId=false)
        {
            VerifyEmployee(employee);

            var existingEmployees = this.GetAllEmployees();
            var employeeWithEmail = existingEmployees.Where(e => e.Email == employee.Email).FirstOrDefault();
            if (employeeWithEmail != null){
                throw new ArgumentException(String.Format("Employee with email: {0} already exists", employee.Email));
            }

            Employee employeeWithId;
            if (keepId) {
                employeeWithId = new Employee(employee, employee.EmployeeId);
            } else {
                int maxId = -1;
                for (int i=0; i<existingEmployees.Count(); i++)
                {
                    if (existingEmployees[i].EmployeeId > maxId)
                        maxId = existingEmployees[i].EmployeeId;
                }
                employeeWithId = new Employee(employee, maxId+1);
            }

            var employeeWithIdExistng = existingEmployees.Where(e => e.EmployeeId == employeeWithId.EmployeeId).FirstOrDefault();
            if (employeeWithIdExistng != null){
                throw new ArgumentException(String.Format("Employee with EmployeeId: {0} already exists", employeeWithId.EmployeeId));
            }

            dbContext.Employees.Add(employeeWithId);
            dbContext.SaveChanges();
            _log.Info(String.Format("Employee added to db: {0}", employeeWithId));
        }

        public void DeleteEmployee(int employeeId)
        {
            Employee myobj = GetEmployee(employeeId);
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
            Employee myobj = GetEmployee(emp.EmployeeId);
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

        public List<EmployeeHours> GetAllEmployeeHours(int employeeId)
        {
            var all = dbContext.EmployeeHoursCollection.ToList();
            var selectedByEmployeeId = all.Where(obj => obj.EmployeeId == employeeId);
            return selectedByEmployeeId.ToList();
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
            if (employeeHours.HoursCount < 0)
                throw new ArgumentException("HoursCount < 0");

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

            if (keepId) {
                employeeHoursWithId = new EmployeeHours(employeeHours, employeeHours.EmployeeHoursId);
            } else {
                var existingEH = this.GetAllEmployeeHours(employeeHours.EmployeeId);
                int maxId = -1;
                for (int i=0; i<existingEH.Count(); i++)
                {
                    if (existingEH[i].EmployeeHoursId > maxId)
                        maxId = existingEH[i].EmployeeHoursId;
                }
                employeeHoursWithId = new EmployeeHours(employeeHours, maxId+1);
            }

            var existingObjects = dbContext.EmployeeHoursCollection.ToList();
            var objWithIdExistng = existingObjects.Where(e => e.EmployeeHoursId == employeeHoursWithId.EmployeeHoursId).FirstOrDefault();
            if (objWithIdExistng != null){
                throw new ArgumentException(String.Format("EmployeeHours with EmployeeHoursId: {0} already exists", employeeHoursWithId.EmployeeHoursId));
            }

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
    }
}
