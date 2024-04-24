using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Business.Abstract;
using Business.Concrete;
using Castle.DynamicProxy;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Microsoft.Extensions.FileProviders;

namespace Business.DependencyResolvers.Autofac
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DocumentManager>().As<IDocumentService>().SingleInstance();
            builder.RegisterType<EfDocumentDal>().As<IDocumentDal>().SingleInstance();

            //Roboflow
            builder.RegisterType<RoboflowManager>().As<IRoboflowService>().SingleInstance();
            builder.RegisterType<EfRoboflowDal>().As<IRoboflowDal>().SingleInstance();

            builder.RegisterType<PlantNetManager>().As<IPlantNetService>().SingleInstance();

            builder.RegisterType<TrefleIOManager>().As<ITrefleIOService>().SingleInstance();

            builder.RegisterType<OpenStreetMapManager>().As<IOpenStreetMapService>().SingleInstance();

            builder.RegisterType<ChatGPTManager>().As<IChatGPTService>().InstancePerLifetimeScope();


            //builder.Register(context => new MapperConfiguration(cfg =>
            //{
            //    // Configure your AutoMapper mappings here
            //    cfg.AddProfile<DocumentProfile>(); // Replace with your actual AutoMapper profile class
            //})).AsSelf().SingleInstance();

            //builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve)).As<IMapper>().InstancePerLifetimeScope();


            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            //builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces()
            //    .EnableInterfaceInterceptors(new ProxyGenerationOptions()
            //    {
            //        Selector = new AspectInterceptorSelector()
            //    }).SingleInstance();

        }
    }
}
