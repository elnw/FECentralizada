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
using TM.FECentralizada.Entities.Pacifyc;
using Common = TM.FECentralizada.Entities.Common;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace TM.FECentralizada.Pacifyc.Read
{
    public partial class PacifycRead : ServiceBase
    {
        Timer oTimer = new Timer();
        public PacifycRead()
        {
            InitializeComponent();

            Tools.Logging.Configure();
           
        }

        public void probar()
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
            try
            {
                Tools.Logging.Info("Inicio del Proceso: Lectura Pacyfic.");

                Tools.Logging.Info("Inicio : Obtener Parámetros");
                //Método que Obtendrá los Parámetros.
                List<Parameters> ParamsResponse = TM.FECentralizada.Business.Common.GetParametersByKey(new Parameters() { Domain = Tools.Constants.PacyficRead, KeyDomain = "", KeyParam = "" });
                Tools.Logging.Info("Fin : Obtener Parámetros");

                if (ParamsResponse != null && ParamsResponse.Any())
                {
                    List<Parameters> ParametersInvoce = ParamsResponse.FindAll(x => x.KeyDomain.ToUpper().Equals(Tools.Constants.PacyficRead_Invoice.ToUpper())).ToList();
                    List<Parameters> ParametersCreditNote = ParamsResponse.FindAll(x => x.KeyDomain.ToUpper().Equals(Tools.Constants.PacyficRead_CreditNote.ToUpper())).ToList();
                    List<Parameters> ParametersDebitNote = ParamsResponse.FindAll(x => x.KeyDomain.ToUpper().Equals(Tools.Constants.PacyficRead_DebitNote.ToUpper())).ToList();

                    Tools.Logging.Info("Inicio : Procesar documentos de BD Pacyfic");
                    Invoice(ParametersInvoce);
                    CreditNote(ParametersCreditNote);

                    /*Parallel.Invoke(
                               () => Invoice(ParametersInvoce),
                               () => CreditNote(ParametersCreditNote),
                               () => DebitNote(ParametersDebitNote)
                        );*/
                    Tools.Logging.Info("Fin : Procesar documentos de BD Pacyfic");

                    //Obtengo la Configuración Intervalo de Tiempo
                    var oConfiguration = Business.Common.GetParameterDeserialized<ServiceConfig>(ParamsResponse.Find(x => x.KeyParam == Tools.Constants.KEY_CONFIG));
                    
                    var Minutes = oConfiguration.ExecutionRate;//oConfiguration.Key3.Equals("D") ? oConfiguration.Value3 : oConfiguration.Key3.Equals("T") ? oConfiguration.Value2 : oConfiguration.Value1;
                    oTimer.Interval = Tools.Common.ConvertMinutesToMilliseconds(Minutes);
                    oTimer.Start();
                    oTimer.AutoReset = true;
                }
                else
                {
                    Tools.Logging.Error("Ocurrió un error al obtener la configuración para pacyfic.");
                }
                Tools.Logging.Info("Fin del Proceso: Lectura Pacyfic.");
            }
            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
        }

        private void Invoice(List<Parameters> oListParameters)
        {
            ServiceConfig serviceConfig;
            Mail mailConfig;
            FileServer fileServerConfig;
            bool isValid;
            List<string> validationMessage = new List<string>();
            int auditId;
            int intentos = 0;
            DateTime timestamp = DateTime.Now;

           

            Tools.Logging.Info("Inicio: Obtener norma para las facturas de Pacyfic");

            Parameters configParameter = oListParameters.FirstOrDefault(x => x.KeyParam == Tools.Constants.KEY_CONFIG);

            if(configParameter != null)
            {
                serviceConfig = Business.Common.GetParameterDeserialized<ServiceConfig>(configParameter);

                Tools.Logging.Info("Inicio : Obtener documentos de BD Pacyfic - Facturas");
                List<InvoiceHeader> ListInvoceHeader = Business.Pacifyc.GetInvoceHeader(timestamp, ref intentos, serviceConfig.maxAttemps);
                List<InvoiceDetail> ListInvoceDetail = Business.Pacifyc.GetInvoceDetail(timestamp);


                Tools.Logging.Info("Inicio: Obtener configuración de correos electronicos - Facturas Pacyfic");

                Parameters mailParameter = oListParameters.FirstOrDefault(x => x.KeyParam == Tools.Constants.MAIL_CONFIG);

                if (mailParameter != null)
                {
                    mailConfig = Business.Common.GetParameterDeserialized<Mail>(mailParameter);

                    Tools.Logging.Info("Inicio : Registrar Auditoria");

                    auditId = TM.FECentralizada.Business.Common.InsertAudit(DateTime.Now.ToString(Tools.Constants.DATETIME_FORMAT_AUDIT), 2, Tools.Constants.NO_LEIDO, ListInvoceHeader.Count * ListInvoceDetail.Count, 1, serviceConfig.Norm);

                    if(auditId > 0)
                    {

                        Tools.Logging.Info("Inicio : Validar Documentos ");

                        isValid = Business.Pacifyc.ValidateInvoices(ListInvoceHeader, validationMessage);
                        isValid &= Business.Pacifyc.ValidateInvoiceDetail(ListInvoceDetail, validationMessage);


                        /*for(int i = 0; i < ListInvoceDetail.Count; i++)
                        {
                            if(!ListInvoceHeader.Exists(x => x.serieNumero == ListInvoceDetail[i].serieNumero))
                            {
                                ListInvoceDetail.RemoveAt(i);
                            }
                        }*/

                        Tools.Logging.Info("Inicio : Notificación de Validación");

                        if (!isValid)
                        {
                            Business.Common.SendFileNotification(mailConfig, validationMessage);
                            //Business.Common.UpdateAudit(auditId, Tools.Constants.FALLA_VALIDACION, intentos);
                        }
                       
                        Tools.Logging.Info("Inicio : Actualizo Auditoria");
                        Business.Common.UpdateAudit(auditId, Tools.Constants.LEIDO, intentos);

                        Tools.Logging.Info("Inicio : Insertar Documentos Validados ");
                        Business.Common.BulkInsertListToTable(ListInvoceDetail, "Factura_Detalle");
                        Business.Common.BulkInsertListToTable(ListInvoceHeader, "Factura_Cabecera");

                        Tools.Logging.Info("Inicio : enviar GFiscal ");

                        Parameters fileParameter = oListParameters.FirstOrDefault(x => x.KeyParam == Tools.Constants.FTP_CONFIG);
                        fileServerConfig = Business.Common.GetParameterDeserialized<FileServer>(fileParameter);

                        if(fileServerConfig != null)
                        {
                            string resultPath = "";
                            if (serviceConfig.Norm == 340)
                            {
                                resultPath = Business.Pacifyc.CreateInvoiceFile340(ListInvoceHeader, ListInvoceDetail, System.IO.Path.GetTempPath());
                               
                            }
                            else
                            {
                                resultPath = Business.Pacifyc.CreateInvoiceFile193(ListInvoceHeader, ListInvoceDetail, System.IO.Path.GetTempPath());
                            }
                            Tools.FileServer.UploadFile(fileServerConfig.Host, fileServerConfig.Port, fileServerConfig.User, fileServerConfig.Password, fileServerConfig.Directory, System.IO.Path.GetFileName(resultPath), System.IO.File.ReadAllBytes(resultPath));

                            Tools.Logging.Info("Inicio :  Notificación de envio  GFiscal ");
                            Business.Common.SendFileNotification(mailConfig, $"Se envió correctamente el documento: {System.IO.Path.GetFileName(resultPath)} a gfiscal");
                            Tools.Logging.Info("Inicio : Actualizo Auditoria");

                            Business.Common.UpdateAudit(auditId, Tools.Constants.ENVIADO_GFISCAL, intentos);
                            Tools.Logging.Info("Inicio : Actualizar fecha de envio");
                            //actualizar documento factura -> agregar el nombre archivo alignet,fechaenvio,
                            Business.Common.UpdateDocumentInvoice(System.IO.Path.GetFileName(resultPath), DateTime.Now.ToString(Tools.Constants.DATETIME_FORMAT_AUDIT));
                            Business.Pacifyc.UpdatePickUpDate(ListInvoceHeader);

                        }




                        
                    }
                    else
                    {
                        Tools.Logging.Error($"No se pudo recuperar el id de auditoria - Facturas pacyfic");
                        Business.Common.UpdateAudit(auditId, Tools.Constants.ERROR_FECENTRALIZADA, intentos);
                    }


                }
                else
                {
                    Tools.Logging.Error($"No se insertó en base de datos el parámetro con llave: {Tools.Constants.MAIL_CONFIG}");
                    //Business.Common.UpdateAudit(auditId, Tools.Constants.ERROR_FECENTRALIZADA, intentos);
                    return;
                }

               
            }
            else
            {
                Tools.Logging.Error($"No se insertó en base de datos el parámetro con llave: {Tools.Constants.KEY_CONFIG}");
                //Business.Common.UpdateAudit(auditId, Tools.Constants.ERROR_FECENTRALIZADA, intentos);
                return;
            }

            

        }
        private void CreditNote(List<Parameters> oListParameters)
        {
            ServiceConfig serviceConfig;
            Mail mailConfig;
            FileServer fileServerConfig;
            bool isValid;
            List<string> validationMessage = new List<string>();
            int auditId;
            int intentos = 0;
            DateTime timestamp = DateTime.Now;

            Tools.Logging.Info("Inicio: Obtener norma para las facturas de Pacyfic");
            Parameters configParameter = oListParameters.FirstOrDefault(x => x.KeyParam == Tools.Constants.KEY_CONFIG);

            if (configParameter != null)
            {
                serviceConfig = Business.Common.GetParameterDeserialized<ServiceConfig>(configParameter);
                Tools.Logging.Info("Inicio : Obtener documentos de BD Pacyfic - Notas de crédito");

                List<CreditNoteHeader> ListInvoceHeader = Business.Pacifyc.GetCreditNoteHeaders(timestamp, ref intentos, serviceConfig.maxAttemps);
                List<CreditNoteDetail> ListInvoceDetail = Business.Pacifyc.GetCreditNoteDetails(timestamp);

                Tools.Logging.Info("Inicio: Obtener configuración de correos electronicos - Facturas Pacyfic");
                Parameters mailParameter = oListParameters.FirstOrDefault(x => x.KeyParam == Tools.Constants.MAIL_CONFIG);

                if (mailParameter != null)
                {
                    mailConfig = Business.Common.GetParameterDeserialized<Mail>(mailParameter);

                    Tools.Logging.Info("Inicio : Registrar Auditoria");
                    auditId = TM.FECentralizada.Business.Common.InsertAudit(DateTime.Now.ToString(Tools.Constants.DATETIME_FORMAT_AUDIT), 2, Tools.Constants.NO_LEIDO, ListInvoceHeader.Count + ListInvoceDetail.Count, 1, serviceConfig.Norm);


                }



            }


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
        }
    }
}
