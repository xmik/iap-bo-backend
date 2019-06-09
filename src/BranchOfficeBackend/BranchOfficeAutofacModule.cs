using Autofac;

namespace BranchOfficeBackend
{
    public class BranchOfficeAutofacModule : Autofac.Module
    {
        protected override void Load(Autofac.ContainerBuilder builder) {
            builder.RegisterType<PostgresDataAccessObjectService>().As<IDataAccessObjectService>();
            builder.RegisterType<WebObjectService>().As<IWebObjectService>();
            builder.Register(c =>
                new BranchOfficeDbContext()
            ).As<BranchOfficeDbContext>().SingleInstance();
            builder.RegisterType<ConfigurationService>().As<IConfigurationService>();
            builder.RegisterType<HQAPIClient>().As<IHQAPIClient>();
            builder.RegisterType<SynchronizatorService>().
                As<ISynchronizatorService>().SingleInstance();
            builder.RegisterType<UserRepository>().As<IUserRepository>();
        }
    }
}
