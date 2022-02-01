using System.Linq;

using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using FirebirdDbComparer.Interfaces;

namespace FirebirdDbComparer.Ioc;

public class FactoryInstaller : IWindsorInstaller
{
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
        container.Register(
            Component
                .For<IMetadataFactory>()
                .AsFactory(),
            Component
                .For<IComparerFactory>()
                .AsFactory(),
            Component.For<DatabaseObjectFactorySelector>(),
            Component
                .For<IDatabaseObjectFactory>()
                .AsFactory(c => c.SelectedWith<DatabaseObjectFactorySelector>()));
    }
}
