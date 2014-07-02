using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Funq;
using reexmonkey.infrastructure.operations.contracts;
using reexmonkey.infrastructure.operations.concretes;
using ServiceStack.ServiceClient.Web;
using xcal.application.client.console.local.presentation.logic;

namespace xcal.application.client.console.local
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new Container();
            var version = typeof(Program).GetAssembly().GetVersionInfo();
            var fversion = typeof(Program).GetAssembly().GetFileVersionInfo();
            container.Register<ServiceClientBase>(x => new JsonServiceClient(Properties.Settings.Default.server));
            container.Register<IGuidKeyGenerator>(x => new GuidKeyGenerator());
            container.Register<IFPIKeyGenerator>(x =>new FPIKeyGenerator<string>()
            {
                Owner = fversion.CompanyName,
                LanguageId = Properties.Settings.Default.fpiLanguageId,
                Description = Properties.Settings.Default.fpiDescription,
                Authority = Authority.None
            });

            Console.WriteLine("Welcome to xCal Client Console - Version: {0}.{1}.{2} r({3})", version.Major, version.Minor, version.Build, version.Revision );
            Console.WriteLine("Please press any key to start");
            Console.ReadKey();

            var manager = new CalendarManager 
            { 
                FpiKeyGenerator = container.Resolve<IFPIKeyGenerator>(),
                GuidKeyGenerator = container.Resolve<IGuidKeyGenerator>(),
                ServiceClient = container.Resolve<ServiceClientBase>()
            };

            manager.MaintainMultipleCalendarsWithEvents();
            
            //emanager.PublishMinimalEvent();

            Console.WriteLine("Please press any key to close");
            Console.ReadKey();
        }
    }
}
