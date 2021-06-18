using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TM.FECentralizada.Entities.Common;

namespace TM.FECentralizada.Pacifyc.Response
{
    public partial class PacifycResponse : ServiceBase
    {
        Timer oTimer = new Timer();
        public PacifycResponse()
        {
            InitializeComponent();
        }

        public void TestProject()
        {
            Procedure();
        }

        protected override void OnStart(string[] args)
        {
            oTimer.Enabled = true;
            oTimer.AutoReset = false;
            oTimer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            oTimer.Start();
            oTimer.Interval = 10000;
        }

        protected override void OnStop()
        {
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Procedure();
        }

        private void Procedure()
        {
            Tools.Logging.Info("Inicio del Proceso: Respuesta Pacyfic.");
            Tools.Logging.Info("Inicio : Obtener Parámetros");

            List<Parameters> ParamsResponse = TM.FECentralizada.Business.Common.GetParametersByKey(new Parameters() { Domain = Tools.Constants.PacyficResponse, KeyDomain = "", KeyParam = "" });
            Tools.Logging.Info("Fin : Obtener Parámetros");

            if (ParamsResponse != null && ParamsResponse.Any())
            {
                List<Parameters> ParametersInvoce = ParamsResponse.FindAll(x => x.KeyDomain.ToUpper().Equals(Tools.Constants.PacyficResponse_Invoice.ToUpper())).ToList();
                List<Parameters> ParametersCreditNote = ParamsResponse.FindAll(x => x.KeyDomain.ToUpper().Equals(Tools.Constants.PacyficResponse_CreditNote.ToUpper())).ToList();
                List<Parameters> ParametersDebitNote = ParamsResponse.FindAll(x => x.KeyDomain.ToUpper().Equals(Tools.Constants.PacyficResponse_DebitNote.ToUpper())).ToList();

                Tools.Logging.Info("Inicio : Procesar documentos de BD Pacyfic");

                Invoice();
                //parallel invoke


            }
            else
            {
                Tools.Logging.Error("Ocurrió un error al obtener la configuración para pacyfic.");
            }
            Tools.Logging.Info("Fin del Proceso: Lectura Pacyfic.");
        }

        private void Invoice(List<Parameters> oListParameters)
        {
            Mail mailConfig;
            FileServer fileServerConfig;

            DateTime timestamp = DateTime.Now;
            List<string> targetFiles;

            Tools.Logging.Info("Inicio: Obtener parámetros para lectura");

            Parameters ftpParameter = oListParameters.FirstOrDefault(x => x.KeyParam == Tools.Constants.FTP_CONFIG);

            if (ftpParameter != null)
            {
                fileServerConfig = Business.Common.GetParameterDeserialized<FileServer>(ftpParameter);

                targetFiles = Tools.FileServer.ListDirectory(fileServerConfig.Host, fileServerConfig.Port, fileServerConfig.User, fileServerConfig.Password, fileServerConfig.Directory);

                if(targetFiles.Count > 0)
                {

                }
                else
                {
                    Tools.Logging.Info("No se encontraron archivos por procesar - Pacyfic Response");
                }


            }


        }

    }
}
