using Autofac;

namespace BranchOfficeBackend
{
    public class BranchOfficeAutofacModule : Autofac.Module
    {
        protected override void Load(Autofac.ContainerBuilder builder) {
            builder.RegisterType<PostgresDataAccessObjectService>().As<IDataAccessObjectService>();
            builder.RegisterType<EmployeeRepository>().As<IEmployeeRepository>();
            builder.Register(c =>
                new BranchOfficeDbContext()
            ).As<BranchOfficeDbContext>().SingleInstance();
        }
    }
}