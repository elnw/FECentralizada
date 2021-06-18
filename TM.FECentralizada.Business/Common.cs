using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.FECentralizada.Entities.Common;
using Newtonsoft;
using System.Net.Mail;

namespace TM.FECentralizada.Business
{
    public static class Common
    {
        public static List<Entities.Common.Parameters> GetParametersByKey(Entities.Common.Parameters oParametersRequest)
        {
            List<Entities.Common.Parameters> oListParameter = new List<Entities.Common.Parameters>();
            try
            {
                oListParameter = TM.FECentralizada.Data.Common.GetParametersByKey(oParametersRequest);
            }
            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
            return oListParameter;
        }

        public static T GetParameterDeserialized<T>(Parameters parameter)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(parameter.ValueJson);
            }catch(Exception ex)
            {
                Tools.Logging.Error(ex.Message + ex.InnerException);
                throw ex;
            }
           
        }

        public static int InsertAudit(string idGrupo,  int legado, int estado, int cantidadRegistros, int intentos, int procesoSunat)
        {
            try
            {
                return TM.FECentralizada.Data.Common.InsertAudit(idGrupo, legado, estado, cantidadRegistros, intentos, procesoSunat);
            }catch(Exception ex)
            {
                Tools.Logging.Error(ex.Message);
                return 0;
            }
        }

        public static void SendFileNotification(Entities.Common.Mail mail, string messageResult)
        {
            bool result = Tools.Mail.SendEmailBySMTP(mail.From, mail.To, null, null, mail.Subject, messageResult, mail.User, mail.Password, mail.Port, mail.Host);

            if (!result)
            {
                Tools.Logging.Error("Ocurrió un error al mandar uno o varios emails, revisar logs anteriores");
            }

        }

        public static void UpdateAudit(int auditoriaId, int estadoid, int intentos)
        {
            try
            {
                TM.FECentralizada.Data.Common.UpdateAudit(auditoriaId, estadoid, intentos);
            }catch(Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
        }
       
        public static void BulkInsertListToTable<T>(List<T> list, string tableName)
        {
            try
            {
                TM.FECentralizada.Data.Common.BulkInsertListToTable(list, tableName);
            }
            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
        }

        public static void UpdateDocumentInvoice(string alignet, string sendDate)
        {
            try
            {
                Data.Common.UpdateDocumentInvoice(alignet, sendDate);
            }catch(Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
        }
        
        

    }
}
