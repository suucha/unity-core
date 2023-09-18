using System;
using Zenject;

namespace SuuchaStudio.Unity.Core.Ioc.Zenject
{
    public class ZenjectContainer : IContainer
    {
        private DiContainer container;
        public void Build()
        {
            container = new DiContainer();
        }
        private void BinderLife(FromBinder binder, LifeStyle life)
        {
            if (life == LifeStyle.Singleton)
            {
                binder.AsSingle();
            }
            else
            {
                binder.AsTransient();
            }
        }
        public void RegisterType(Type implementationType, string serviceName = null, LifeStyle life = LifeStyle.Singleton)
        {
            var binder = container.Bind(implementationType);
            BinderLife(binder, life);
            if (!string.IsNullOrEmpty(serviceName))
            {
                binder.WithId(serviceName);
            }
            binder.ToSelf();
        }

        public void RegisterType(Type serviceType, Type implementationType, string serviceName = null, LifeStyle life = LifeStyle.Singleton)
        {
            var binder = container.Bind(serviceType);
            BinderLife(binder, life);
            if (!string.IsNullOrEmpty(serviceName))
            {
                binder.WithId(serviceName);
            }
            binder.To(implementationType);
        }

        public TService Resolve<TService>() where TService : class
        {
            return container.Resolve<TService>();
        }

        public object Resolve(Type serviceType)
        {
            return container.Resolve(serviceType);
        }

        public TService ResolveNamed<TService>(string serviceName) where TService : class
        {
            return container.ResolveId<TService>(serviceName);
        }

        public object ResolveNamed(string serviceName, Type serviceType)
        {
            return container.RebindId(serviceType, serviceName);
        }

        public bool TryResolve<TService>(out TService instance) where TService : class
        {
            instance = container.TryResolve<TService>();
            return instance != null;
        }

        public bool TryResolve(Type serviceType, out object instance)
        {
            instance = container.TryResolve(serviceType);
            return instance != null;
        }

        public bool TryResolveNamed(string serviceName, Type serviceType, out object instance)
        {
            instance = container.TryResolveId(serviceType, serviceName);
            return instance != null;
        }

        void IContainer.Register<TService, TImplementer>(string serviceName, LifeStyle life)
        {
            var binder = container.Bind<TService>().To<TImplementer>();
            BinderLife(binder, life);
        }

        void IContainer.RegisterInstance<TService, TImplementer>(TImplementer instance, string serviceName)
        {
            container.Bind<TService>().To<TImplementer>().FromInstance(instance).AsSingle();
        }
    }
}
