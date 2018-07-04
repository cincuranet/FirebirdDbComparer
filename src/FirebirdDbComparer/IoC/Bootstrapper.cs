using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;

using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.Ioc;

namespace FirebirdDbComparer.IoC
{
    public static class Bootstrapper
    {
        public static IWindsorContainer BootstrapContainerDefault(IComparerSettings settings)
        {
            var container = new WindsorContainer();
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));
            container.Kernel.AddHandlerSelector(new SqlHelperSelector(settings));
            container.Kernel.AddHandlerSelector(new DatabaseObjectSelector(settings));
            container.AddFacility<TypedFactoryFacility>();
            container.Register(Component.For<IComparerSettings>().Instance(settings));
            container.Install(FromAssembly.InThisApplication());
            return container;
        }
    }
}
