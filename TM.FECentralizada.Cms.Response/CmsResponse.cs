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

namespace TM.FECentralizada.Cms.Response
{
    public partial class CmsResponse : ServiceBase
    {
        Timer oTimer = new Timer();
        public CmsResponse()
        {
            InitializeComponent();
        }

        public void TestProject()
        {
            Procedure();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                oTimer.Enabled = true;
                oTimer.AutoReset = false;
                oTimer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
                oTimer.Start();
                oTimer.Interval = 10000;
            }
            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Procedure();
        }

        private void Procedure()
        {
            Tools.Logging.Info("Inicio del Proceso: Respuesta Cms.");

            Tools.Logging.Info("Inicio : Obtener Parámetros");
            List<Parameters> ParamsResponse = TM.FECentralizada.Business.Common.GetParametersByKey(new Parameters() { Domain = Tools.Constants.CmsResponse, KeyDomain = "", KeyParam = "" });
            Tools.Logging.Info("Fin : Obtener Parámetros");

            if (ParamsResponse != null && ParamsResponse.Any())
            {
                List<Parameters> ParametersInvoce = ParamsResponse.FindAll(x => x.KeyDomain.ToUpper().Equals(Tools.Constants.CmsResponse_Invoice.ToUpper())).ToList();
                List<Parameters> ParametersCreditNote = ParamsResponse.FindAll(x => x.KeyDomain.ToUpper().Equals(Tools.Constants.CmsResponse_CreditNote.ToUpper())).ToList();
                List<Parameters> ParametersDebitNote = ParamsResponse.FindAll(x => x.KeyDomain.ToUpper().Equals(Tools.Constants.CmsResponse_DebitNote.ToUpper())).ToList();

                Tools.Logging.Info("Inicio : Procesar documentos de BD Cms");

                Invoice(ParametersInvoce);
                //parallel invoke


            }
            else
            {
                Tools.Logging.Error("Ocurrió un error al obtener la configuración para Cms.");
            }
            Tools.Logging.Info("Fin del Proceso: Lectura Cms.");
        }

        private void Invoice(List<Parameters> oListParameters)
        {

            ServiceConfig serviceConfig;
            Mail mailConfig;
            FileServer fileServerConfig;

            DateTime timestamp = DateTime.Now;
            List<string> messagesResponse;
            List<ResponseFile> responseFiles;
            int auditId;

            Tools.Logging.Info("Inicio: Obtener parámetros para lectura");

            Parameters configParameter = oListParameters.FirstOrDefault(x => x.KeyParam == Tools.Constants.KEY_CONFIG);
            serviceConfig = Business.Common.GetParameterDeserialized<ServiceConfig>(configParameter);
            if (configParameter != null)
            {
                Parameters ftpParameter = oListParameters.FirstOrDefault(x => x.KeyParam == Tools.Constants.FTP_CONFIG_INPUT);

                Tools.Logging.Info("Inicio: Descargar archivos de respuesta de gfiscal - Cms Response");
                if (ftpParameter != null)
                {
                    fileServerConfig = Business.Common.GetParameterDeserialized<FileServer>(ftpParameter);

                    messagesResponse = new List<string>();
                    responseFiles = Business.Common.DownloadFileOutput(fileServerConfig, messagesResponse, "RPTA_FACT_03");



                    if (responseFiles != null && responseFiles.Count > 0)
                    {
                        Tools.Logging.Info("Inicio: Insertar auditoria - Cms Response");
                        auditId = TM.FECentralizada.Business.Common.InsertAudit(DateTime.Now.ToString(Tools.Constants.DATETIME_FORMAT_AUDIT), 2, Tools.Constants.NO_LEIDO, responseFiles.Count, 1, serviceConfig.Norm);

                        Tools.Logging.Info("Inicio:  Obtener configuración de email - Cms Response");
                        Parameters mailParameter = oListParameters.FirstOrDefault(x => x.KeyParam == Tools.Constants.MAIL_CONFIG);
                        mailConfig = Business.Common.GetParameterDeserialized<Mail>(mailParameter);

                        if (mailConfig != null)
                        {
                            if (messagesResponse.Count > 0)
                            {
                                Business.Common.SendFileNotification(mailConfig, messagesResponse);
                            }
                            Business.Common.UpdateAudit(auditId, Tools.Constants.RETORNO_GFISCAL, 1);

                            Tools.Logging.Info("Inicio: Actualizar documentos en FECentralizada - Cms Response");

                            Business.Common.UpdateInvoiceState(responseFiles);

                            Tools.Logging.Info("Inicio: Envio archivo respuesta a Legado - Cms Response");

                            Tools.Logging.Info("Fin: Envio archivo respuesta a Legado - Cms Response");

                            Tools.Logging.Info("Inicio: Actualizar auditoria - Cms Response");
                            Business.Common.UpdateAudit(auditId, Tools.Constants.ENVIADO_LEGADO, 1);


                        }
                        else
                        {
                            Tools.Logging.Error("No se encontró el parámetro de configuracion MAILCONFIG - Cms Response");
                        }




                    }
                    else
                    {
                        Tools.Logging.Info("No se encontraron archivos por procesar - Cms Response");
                        auditId = TM.FECentralizada.Business.Common.InsertAudit(DateTime.Now.ToString(Tools.Constants.DATETIME_FORMAT_AUDIT), 2, Tools.Constants.NO_LEIDO, 0, 1, 193);
                    }


                }

            }
            else
            {
                Tools.Logging.Error("No se encontró el parámetro de configuracion KEYCONFIG - Cms Response");
            }



        }
        private void Bill(List<Parameters> oListParameters)
        {
            Tools.Logging.Info("Inicio : Obtener documentos de BD Isis - Boletas");


            Tools.Logging.Info("Inicio : Registrar Auditoria");


            Tools.Logging.Info("Inicio : Validar Documentos ");

            Tools.Logging.Info("Inicio : Notificación de Validación");

            Tools.Logging.Info("Inicio : Actualizo Auditoria");

            Tools.Logging.Info("Inicio : Insertar Documentos Validados ");

            Tools.Logging.Info("Inicio : Valido Documentos insertados ");

            Tools.Logging.Info("Inicio : Lees  Documentos insertados ");

            Tools.Logging.Info("Inicio : enviar GFiscal ");

            Tools.Logging.Info("Inicio :  Notificación de envio  GFiscal ");

            Tools.Logging.Info("Inicio : Actualizo Auditoria");

        }
        private void CreditNote(List<Parameters> oListParameters)
        {

            Tools.Logging.Info("Inicio : Obtener documentos de BD Isis - Boletas");


            Tools.Logging.Info("Inicio : Registrar Auditoria");


            Tools.Logging.Info("Inicio : Validar Documentos ");

            Tools.Logging.Info("Inicio : Notificación de Validación");

            Tools.Logging.Info("Inicio : Actualizo Auditoria");

            Tools.Logging.Info("Inicio : Insertar Documentos Validados ");

            Tools.Logging.Info("Inicio : Valido Documentos insertados ");

            Tools.Logging.Info("Inicio : Lees  Documentos insertados ");

            Tools.Logging.Info("Inicio : enviar GFiscal ");

            Tools.Logging.Info("Inicio :  Notificación de envio  GFiscal ");

            Tools.Logging.Info("Inicio : Actualizo Auditoria");
        }
        private void DebitNote(List<Parameters> oListParameters)
        {

            Tools.Logging.Info("Inicio : Obtener documentos de BD Isis - Boletas");


            Tools.Logging.Info("Inicio : Registrar Auditoria");


            Tools.Logging.Info("Inicio : Validar Documentos ");

            Tools.Logging.Info("Inicio : Notificación de Validación");

            Tools.Logging.Info("Inicio : Actualizo Auditoria");

            Tools.Logging.Info("Inicio : Insertar Documentos Validados ");

            Tools.Logging.Info("Inicio : Valido Documentos insertados ");

            Tools.Logging.Info("Inicio : Lees  Documentos insertados ");

            Tools.Logging.Info("Inicio : enviar GFiscal ");

            Tools.Logging.Info("Inicio :  Notificación de envio  GFiscal ");

            Tools.Logging.Info("Inicio : Actualizo Auditoria");
        }

        protected override void OnStop()
        {
            oTimer.Stop();
        }
    }
}
