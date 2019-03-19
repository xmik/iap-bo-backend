using Autofac;

namespace BranchOfficeBackend
{
    public class BranchOfficeAutofacModule : Autofac.Module
    {
        protected override void Load(Autofac.ContainerBuilder builder) {
            builder.RegisterType<PostgresDataAccessObjectService>().As<IDataAccessObjectService>();
        }
    }
}