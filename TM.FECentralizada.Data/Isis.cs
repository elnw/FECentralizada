using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.FECentralizada.Entities.Isis;

namespace TM.FECentralizada.Data
{
    public static class Isis
    {
        public static List<InvoiceHeader> GetInvoceHeader()
        {
            List<InvoiceHeader> oListInvoiceHeader = new List<InvoiceHeader>();
            try
            {
               // oListInvoiceHeader = TM.FECentralizada.Data.Isis.GetInvoce();




            }
            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
            return oListInvoiceHeader;
        }

        public static List<InvoiceHeader> GetInvoceDetail()
        {
            List<InvoiceHeader> oListInvoiceHeader = new List<InvoiceHeader>();
            try
            {
                // oListInvoiceHeader = TM.FECentralizada.Data.Isis.GetInvoce();




            }
            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
            return oListInvoiceHeader;
        }

    }
}
