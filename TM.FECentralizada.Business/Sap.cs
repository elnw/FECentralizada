using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.FECentralizada.Entities.Sap;

namespace TM.FECentralizada.Business
{
    public static class Sap
    {
        public static List<InvoiceHeader> GetInvoceHeader(string filename, List<string> data, DateTime timestamp, ref int intentos, int maxAttemps)
        {
            return null;
        }

        public static bool ValidateInvoices(List<InvoiceHeader> listInvoceHeader, ref string validationMessage)
        {
            throw new NotImplementedException();
        }

        public static bool ValidateInvoiceDetail(List<InvoiceDetail> listInvoceDetail, ref string validationMessage)
        {
            throw new NotImplementedException();
        }

        public static List<InvoiceDetail> GetInvoceDetail(string filename, List<string> data, DateTime timestamp)
        {
            throw new NotImplementedException();
        }
    }
}
