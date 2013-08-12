using System;
using System.Web.Mvc;
using Castle.Windsor;
using IndyCode.Infrastructure.Common;
using IndyCode.Infrastructure.Common.Container;
using IndyCode.Infrastructure.Common.Container.Windsor;
using MvcExtensions;
using MvcExtensions.Windsor;
using log4net;

namespace ru_football
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : WindsorMvcApplication
    {
        public MvcApplication()
        {
            Bootstrapper.BootstrapperTasks
                .Include<RegisterIoC>(x => x.Initialize(new WindsorContainerWrapper(WindsorContainer)))
                .Include<RegisterRoutes>()
                .Include<RegisterModelMetadata>()
                .Include<RegisterControllers>()
                .Include<RegisterActionInvokers>();
        }

        private IWindsorContainer WindsorContainer
        {
            get { return ((WindsorAdapter)Adapter).Container; }
        }

        public void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            LogManager.GetLogger("logger").Error("Ошибка ", exception);
        }
    }

    public class RegisterIoC : BootstrapperTask
    {
        private IContainer container;

        public override TaskContinuation Execute()
        {
            IoC.Initialize(container);

            return TaskContinuation.Continue;
        }

        public void Initialize(IContainer aContainer)
        {
            container = aContainer;
        }
    }
}