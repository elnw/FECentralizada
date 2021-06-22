using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.FECentralizada.Entities.Pacifyc;
using TM.FECentralizada.Data;
using System.IO;

namespace TM.FECentralizada.Business
{
    public static class Pacifyc
    {
        public static List<InvoiceHeader> GetInvoceHeader(DateTime timestamp, ref int intentos, int maxIntentos)
        {
            List<InvoiceHeader> ListHeaders = new List<InvoiceHeader>();
            bool debeRepetir = false;
            Tools.Logging.Info("Iniciando Consulta BD- Factura Cabecera");
            try
            {
                for(int i = 0; i < maxIntentos; i++)
                {
                    ListHeaders = Data.Pacifyc.ReadInvoiceHeader(timestamp, ref debeRepetir);
                    intentos++;
                    Tools.Logging.Info("Fin Consulta BD-Factura Cabecera");
                    if (!debeRepetir) break;
                    
                }
                
            }
             
            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
            return ListHeaders;
        }

        public static List<InvoiceDetail> GetInvoceDetail(DateTime timestamp)
        {
            List<InvoiceDetail> ListDetails = new List<InvoiceDetail>();
            InvoiceDetail objBillHeader = new InvoiceDetail();

            Tools.Logging.Info("Iniciando Consulta BD- Cabecera");
            try
            {
                ListDetails = Data.Pacifyc.ReadInvoiceDetail(timestamp);
                Tools.Logging.Info("Fin Consulta BD- Cabecera");
            }

            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
            return ListDetails;
        }

        public static List<CreditNoteHeader> GetCreditNoteHeaders(DateTime timestamp, ref int intentos, int maxIntentos)
        {
            List<CreditNoteHeader> ListHeaders = new List<CreditNoteHeader>();
            bool debeRepetir = false;

            Tools.Logging.Info("Iniciando Consulta BD- Cabecera");
            try
            {
                for (int i = 0; i < maxIntentos; i++)
                {
                    ListHeaders = Data.Pacifyc.ReadCreditNoteHeaders(timestamp, ref debeRepetir);
                    intentos++;
                    Tools.Logging.Info("Fin Consulta BD-Factura Cabecera");
                    if (!debeRepetir) break;
                }

                    
                Tools.Logging.Info("Fin Consulta BD- Cabecera");
              
            }

            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
            return ListHeaders;
        }

        public static List<CreditNoteDetail> GetCreditNoteDetails()
        {
            List<CreditNoteDetail> ListHeaders = new List<CreditNoteDetail>();
            CreditNoteDetail objBillHeader = new CreditNoteDetail();

            Tools.Logging.Info("Iniciando Consulta BD- Cabecera");
            try
            {
               // ListHeaders = Data.Pacifyc.ReadCreditNoteDetails();
                Tools.Logging.Info("Fin Consulta BD- Cabecera");
                Tools.Logging.Info("Iniciando registro en BD - Cabecera");
            }

            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
            return ListHeaders;
        }

        public static List<DebitNoteHeader> GetDebitNoteHeaders()
        {
            List<DebitNoteHeader> ListHeaders = new List<DebitNoteHeader>();
            DebitNoteHeader objBillHeader = new DebitNoteHeader();

            Tools.Logging.Info("Iniciando Consulta BD- Cabecera");
            try
            {
                ListHeaders = Data.Pacifyc.ReadDebitNoteHeader();
                Tools.Logging.Info("Fin Consulta BD- Cabecera");
                Tools.Logging.Info("Iniciando registro en BD - Cabecera");
            }

            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
            return ListHeaders;
        }

        public static List<DebitNoteDetail> GetDebitNoteDetails()
        {
            List<DebitNoteDetail> ListDetails = new List<DebitNoteDetail>();
            DebitNoteDetail objBillHeader = new DebitNoteDetail();

            Tools.Logging.Info("Iniciando Consulta BD- Cabecera");
            try
            {
                ListDetails = Data.Pacifyc.ReadDebitNoteDetails();
                Tools.Logging.Info("Fin Consulta BD- Cabecera");
                Tools.Logging.Info("Iniciando registro en BD - Cabecera");
            }

            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
            return ListDetails;
        }

        public static bool ValidateInvoices(List<InvoiceHeader> invoiceHeaders, List<string> messageResult)
        {
            bool checkInvoice = true;

            foreach (var invoice in invoiceHeaders.ToList())
            {
                if(!ValidateInvoice(invoice, messageResult))
                {
                    invoiceHeaders.Remove(invoice);
                    checkInvoice &= false;
                }
                
            }

            return checkInvoice;
        }

        private static bool ValidateInvoice(InvoiceHeader invoiceHeader, List<string> messageResult)
        {
            bool isValid = true;
            if(String.IsNullOrEmpty(invoiceHeader.serieNumero) || invoiceHeader.serieNumero.Length < 13 || !invoiceHeader.serieNumero.StartsWith("F"))
            {
                messageResult.Add("La serie y número de la factura: " + invoiceHeader.serieNumero + " tiene una longitud invalida o no cumple con el formato correcto");
                isValid &= false;
            }

            if (String.IsNullOrEmpty(invoiceHeader.fechaEmision))
            {
                messageResult.Add("La fecha de emision de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacía.");
                isValid &= false;
            }
            if (String.IsNullOrEmpty(invoiceHeader.tipoDocumentoAdquiriente))
            {
                messageResult.Add("El tipo de documento adquiriente de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacío.");
                isValid &= false;
            }
            if (String.IsNullOrEmpty(invoiceHeader.numeroDocumentoAdquiriente))
            {
                messageResult.Add("El número de documento adquiriente de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacío.");
                isValid &= false;
            }
            if (String.IsNullOrEmpty(invoiceHeader.razonSocialAdquiriente))
            {
                messageResult.Add("La razon social del adquiriente de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacía.");
                isValid &= false;
            }
            if (String.IsNullOrEmpty(invoiceHeader.tipoMoneda))
            {
                messageResult.Add("El tipo de moneda de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacía.");
                isValid &= false;
            }
            if (String.IsNullOrEmpty(invoiceHeader.tipooperacion))
            {
                messageResult.Add("El tipo de operación de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacía.");
                isValid &= false;
            }
            if (String.IsNullOrEmpty(invoiceHeader.codigoestablecimientosunat))
            {
                messageResult.Add("El codigo de establecimiento sunat de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacía.");
                isValid &= false;
            }
            if (String.IsNullOrWhiteSpace(invoiceHeader.totalvalorventa))
            {
                messageResult.Add("El total del valor de venta de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacío.");
                isValid &= false;
            }
            return isValid;
        }

        private static bool ShouldDeleteInvoice(InvoiceDetail detail, List<string> messageResult)
        {
            bool isValid = true;
            if (String.IsNullOrEmpty(detail.serieNumero) || detail.serieNumero.Length < 13 || !detail.serieNumero.StartsWith("F"))
            {
                messageResult.Add("La serie y número de la factura: " + detail.serieNumero + " tiene una longitud invalida o no cumple con el formato correcto");
                isValid &= false;
            }
            if (String.IsNullOrEmpty(detail.descripcion))
            {
                messageResult.Add("La descripcion del detalle con número de orden: " + detail.numeroOrdenItem + " esta vacia.");
                isValid &= false;
            }
            if (String.IsNullOrEmpty(detail.unidadMedida))
            {
                messageResult.Add("La unidad de medida del detalle con número de orden: " + detail.numeroOrdenItem + " esta vacía.");
                isValid &= false;
            }
            if (String.IsNullOrEmpty(detail.codigoImpUnitConImpuesto))
            {
                messageResult.Add("El codigo de imp. unitario del detalle con número de orden: " + detail.numeroOrdenItem + " esta vacía.");
                isValid &= false;
            }
            if (String.IsNullOrEmpty(detail.codigoRazonExoneracion))
            {
                messageResult.Add("El codigo de razon de exgoneracion del detalle con número de orden: " + detail.numeroOrdenItem + " esta vacía.");
                isValid &= false;
            }
            return !isValid;
        }

        public static bool ValidateInvoiceDetail(List<InvoiceDetail> invoiceDetail, List<string> messageResult)
        {
            bool isValid = true;

            foreach (var detail in invoiceDetail.ToList())
            {
                if(ShouldDeleteInvoice(detail, messageResult))
                {
                    invoiceDetail.Remove(detail);
                    isValid &= false;
                }

            }
            return isValid;

        }


        public static void InsertInvoices(List<InvoiceHeader> invoiceHeaders)
        {
            try
            {
                Data.Pacifyc.InsertInvoices(invoiceHeaders);
            }catch(Exception ex){
                Tools.Logging.Error(ex.Message);
            }
        }

        public static void InsertInvoicesDetails(List<InvoiceDetail> invoiceDetails)
        {
            try
            {
                Data.Pacifyc.InsertInvoiceDetails(invoiceDetails);
            }
            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
        }

        public static string CreateInvoiceFile340(List<InvoiceHeader> invoices, List<InvoiceDetail> invoiceDetails, string path)
        {
            DateTime current = DateTime.Now;
            //string fileName = $"FACT_{current.Year}{current.Month}{current.Day}_{current.Hour}_{current.Year}{current.Month}{current.Day}{current.Hour}{current.Minute}{current.Second}.txt";
            string fileName = "FACT_" + current.ToString("yyyyMMdd_HH_yyyyMMddHHmmss") + ".txt";

            using (StreamWriter writer = new StreamWriter(Path.Combine(path, fileName)))
            {
                foreach (InvoiceHeader invoice in invoices)
                {
                    writer.WriteLine($"C|{invoice.serieNumero}|{invoice.fechaEmision}|{invoice.Horadeemision}|" +
                        $"{invoice.tipoMoneda}|{invoice.numeroDocumentoEmisor}|{invoice.tipoDocumentoAdquiriente}|{invoice.numeroDocumentoAdquiriente}|" +
                        $"{invoice.razonSocialAdquiriente}|{invoice.direccionAdquiriente}|{invoice.tipoReferencia_1}|{invoice.numeroDocumentoReferencia_1}|" +
                        $"{invoice.tipoReferencia_2}|{invoice.numeroDocumentoReferencia_2}|{invoice.totalVVNetoOpGravadas}|{invoice.conceptovvnetoopnogravada}|" +
                        $"{invoice.totalVVNetoOpExoneradas}|{invoice.conceptovvnetoopexoneradas}|{invoice.totalVVNetoOpGratuitas}|" +
                        $"{invoice.conceptovvnetoopgratuitas}|{invoice.totalVVNetoExportacion}|{invoice.conceptovvexportacion}|{invoice.totalDescuentos}|{invoice.totalIgv}|" +
                        $"{invoice.totalVenta}|{invoice.tipooperacion}|{invoice.leyendas}||||{invoice.porcentajeDetraccion}|{invoice.totalDetraccion}|{invoice.descripcionDetraccion}|" +
                        $"{invoice.ordenCompra}|{invoice.datosAdicionales}|{invoice.codigoestablecimientosunat}|{invoice.montototalimpuestos}|{invoice.cdgcodigomotivo}|{invoice.cdgporcentaje}|" +
                        $"{invoice.descuentosGlobales}|{invoice.cdgmontobasecargo}|{invoice.sumimpuestosopgratuitas}|{invoice.totalvalorventa}|{invoice.totalprecioventa}|" +
                        $"{invoice.monredimporttotal}||||||{invoice.estado}||{invoice.origen}|");

                    var currentDetails = invoiceDetails.Where(x => x.serieNumero == invoice.serieNumero).ToList();

                    foreach (InvoiceDetail invoiceDetail in currentDetails)
                    {

                        writer.WriteLine($"D|{invoiceDetail.numeroOrdenItem}|{invoiceDetail.unidadMedida}|{invoiceDetail.cantidad}|" +
                            $"{invoiceDetail.codigoProducto}|{invoiceDetail.codigoproductosunat}|{invoiceDetail.descripcion}|" +
                            $"{invoiceDetail.montobaseigv}|{invoiceDetail.importeIgv}|{invoiceDetail.codigoRazonExoneracion}|{invoiceDetail.tasaigv}|" +
                            $"{invoiceDetail.importeDescuento}|{invoiceDetail.codigodescuento}|{invoiceDetail.factordescuento}|" +
                            $"{invoiceDetail.montobasedescuento}|{invoiceDetail.codigoImporteReferencial}|{invoiceDetail.importeReferencial}|" +
                            $"{invoiceDetail.importeUnitarioSinImpuesto}|{invoiceDetail.importeTotalSinImpuesto}|{invoiceDetail.montototalimpuestoitem}|" +
                            $"||{invoiceDetail.codigoImpUnitConImpuesto}|{invoiceDetail.importeUnitarioConImpuesto}|{invoiceDetail.numeroExpediente}|{invoiceDetail.codigoUnidadEjecutora}|" +
                            $"{invoiceDetail.numeroContrato}|{invoiceDetail.numeroProcesoSeleccion}");
                    }


                }

                

            }
            return Path.Combine(path, fileName);
        }

        public static string CreateInvoiceFile193(List<InvoiceHeader> invoices, List<InvoiceDetail> invoiceDetails, string path)
        {
            DateTime current = DateTime.Now;
            string fileName = "FACT_" + current.ToString("yyyyMMdd_HH_yyyyMMddHHmmss") + ".txt";

            using (StreamWriter writer = new StreamWriter(Path.Combine(path, fileName)))
            {
                foreach (InvoiceHeader invoice in invoices)
                {
                    string codigoMotivo = String.IsNullOrWhiteSpace(invoice.totalRetencion) || invoice.totalRetencion == "0" ? "" : "62";
                    double montoBaseRetencion = 0;

                    if(!String.IsNullOrWhiteSpace(invoice.porcentajeRetencion) && !String.IsNullOrWhiteSpace(invoice.totalRetencion))
                    {
                        montoBaseRetencion = Convert.ToDouble(invoice.totalRetencion) / (1 + Convert.ToDouble(invoice.porcentajeRetencion));
                    }

                    writer.WriteLine($"C|{invoice.serieNumero}|{invoice.fechaEmision}|{invoice.Horadeemision}|" +
                         $"{invoice.tipoMoneda}|{invoice.numeroDocumentoEmisor}|{invoice.tipoDocumentoAdquiriente}|{invoice.numeroDocumentoAdquiriente}|" +
                         $"{invoice.razonSocialAdquiriente}|{invoice.direccionAdquiriente}|{invoice.tipoReferencia_1}|{invoice.numeroDocumentoReferencia_1}|" +
                         $"{invoice.tipoReferencia_2}|{invoice.numeroDocumentoReferencia_2}|{invoice.totalVVNetoOpGravadas}|{invoice.conceptovvnetoopnogravada}|" +
                         $"{invoice.totalVVNetoOpExoneradas}|{invoice.conceptovvnetoopexoneradas}|{invoice.totalVVNetoOpGratuitas}|" +
                         $"{invoice.conceptovvnetoopgratuitas}|{invoice.totalVVNetoExportacion}|{invoice.conceptovvexportacion}|{invoice.totalDescuentos}|{invoice.totalIgv}|" +
                         $"{invoice.totalVenta}|{invoice.tipooperacion}|{invoice.leyendas}||||{invoice.porcentajeDetraccion}|{invoice.totalDetraccion}|{invoice.descripcionDetraccion}|" +
                         $"{invoice.ordenCompra}|{invoice.datosAdicionales}|{invoice.codigoestablecimientosunat}|{invoice.montototalimpuestos}|{invoice.cdgcodigomotivo}|{invoice.cdgporcentaje}|" +
                         $"{invoice.descuentosGlobales}|{invoice.cdgmontobasecargo}|{invoice.sumimpuestosopgratuitas}|{invoice.totalvalorventa}|{invoice.totalprecioventa}|" +
                         $"{invoice.monredimporttotal}|Contado::-::-::-::-|{codigoMotivo}|{invoice.porcentajeDetraccion}|{invoice.totalRetencion}|{montoBaseRetencion}|{invoice.estado}||{invoice.origen}|");

                    var currentDetails = invoiceDetails.Where(x => x.serieNumero == invoice.serieNumero).ToList();

                    foreach (InvoiceDetail invoiceDetail in currentDetails)
                    {

                        writer.WriteLine($"D|{invoiceDetail.numeroOrdenItem}|{invoiceDetail.unidadMedida}|{invoiceDetail.cantidad}|" +
                            $"{invoiceDetail.codigoProducto}|{invoiceDetail.codigoproductosunat}|{invoiceDetail.descripcion}|" +
                            $"{invoiceDetail.montobaseigv}|{invoiceDetail.importeIgv}|{invoiceDetail.codigoRazonExoneracion}|{invoiceDetail.tasaigv}|" +
                            $"{invoiceDetail.importeDescuento}|{invoiceDetail.codigodescuento}|{invoiceDetail.factordescuento}|" +
                            $"{invoiceDetail.montobasedescuento}|{invoiceDetail.codigoImporteReferencial}|{invoiceDetail.importeReferencial}|" +
                            $"{invoiceDetail.importeUnitarioSinImpuesto}|{invoiceDetail.importeTotalSinImpuesto}|{invoiceDetail.montototalimpuestoitem}|" +
                            $"||{invoiceDetail.codigoImpUnitConImpuesto}|{invoiceDetail.importeUnitarioConImpuesto}|{invoiceDetail.numeroExpediente}|{invoiceDetail.codigoUnidadEjecutora}|" +
                            $"{invoiceDetail.numeroContrato}|{invoiceDetail.numeroProcesoSeleccion}");
                    }


                }
            }

            return Path.Combine(path, fileName);
        }

        public static void UpdatePickUpDate(List<InvoiceHeader> invoiceHeaders)
        {
            Data.Pacifyc.UpdatePickupDate(invoiceHeaders.Select(x => x.serieNumero).ToList());
            Data.Pacifyc.InvokeInvoiceUpdate();
        }


    }
}
