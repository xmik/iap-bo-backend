using System.Collections.Generic;

namespace BranchOfficeBackend
{
    /// <summary>
    /// Data access layer, one if implementations could be interacting with some database
    /// </summary>
    public interface IDataAccessObjectService
    {
        List<Employee> ListEmployees();
    }
}
