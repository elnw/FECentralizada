using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TM.FECentralizada.Atis.Response
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {
#if DEBUG
            AtisResponse pacifycResponse = new AtisResponse();
            pacifycResponse.TestProject();



#else
             Tools.Logging.Configure();
             ServiceBase[] ServicesToRun;
             ServicesToRun = new ServiceBase[]
             {
                 new PacifycResponse()
             };
             ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
