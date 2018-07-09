using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using FirebirdDbComparer.Compare;
using FirebirdDbComparer.DatabaseObjects;
using FirebirdDbComparer.Interfaces;
using FirebirdDbComparer.SqlGeneration;

namespace FirebirdDbComparer.Ioc
{
    public class ServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes
                    .FromAssemblyInThisApplication(Assembly.GetExecutingAssembly())
                    .BasedOn<IDatabaseObject>()
                    .WithService.FirstInterface()
                    .LifestyleTransient(),
                Classes
                    .FromAssemblyInThisApplication(Assembly.GetExecutingAssembly())
                    .BasedOn<ISqlHelper>()
                    .WithService.DefaultInterfaces()
                    .LifestyleSingleton(),
                Component
                    .For<ScriptBuilder>()
                    .LifestyleTransient(),
                Component
                    .For<IComparerContext>()
                    .ImplementedBy<ComparerContext>()
                    .LifestyleTransient(),
                Component
                    .For<IMetadata>()
                    .ImplementedBy<Metadata>()
                    .LifestyleTransient(),
                Component
                    .For<IComparer>()
                    .ImplementedBy<Comparer>()
                    .LifestyleTransient());
        }
    }
}
