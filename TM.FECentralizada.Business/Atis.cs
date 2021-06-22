using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.FECentralizada.Entities.Atis;

namespace TM.FECentralizada.Business
{
    public static class Atis
    {
        public static List<InvoiceHeader> GetInvoceHeader(String filename, List<String> data, DateTime timestamp, ref int intentos, int maxIntentos)
        {
            List<InvoiceHeader> ListHeaders = new List<InvoiceHeader>();
            bool debeRepetir = false;
            try
            {
                for (int i = 0; i < maxIntentos; i++)
                {
                    ListHeaders = Data.Atis.ReadInvoiceHeader(filename, data, timestamp, ref debeRepetir);
                    intentos++;
                    if (!debeRepetir) break;

                }

            }

            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
            return ListHeaders;
        }

        public static List<InvoiceDetail> GetInvoceDetail(String filename, List<String> data, DateTime timestamp)
        {
            List<InvoiceDetail> ListDetails = new List<InvoiceDetail>();
            try
            {
                ListDetails = Data.Atis.ReadInvoiceDetail(filename, data, timestamp);
            }

            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }

            return ListDetails;
        }


        public static bool ValidateInvoices(List<InvoiceHeader> invoiceHeaders, ref string messageResult)
        {
            bool checkInvoice = true;

            foreach (var invoice in invoiceHeaders.ToList())
            {
                if (!ValidateInvoice(invoice, ref messageResult))
                {
                    invoiceHeaders.Remove(invoice);
                    checkInvoice &= false;
                }

            }

            return checkInvoice;
        }

        private static bool ValidateInvoice(InvoiceHeader invoiceHeader, ref string messageResult)
        {
            bool isValid = true;
            if (String.IsNullOrEmpty(invoiceHeader.serieNumero) || invoiceHeader.serieNumero.Length < 13 || !invoiceHeader.serieNumero.StartsWith("F"))
            {
                messageResult += "La serie y número de la factura: " + invoiceHeader.serieNumero + " tiene una longitud invalida o no cumple con el formato correcto";
                isValid &= false;
            }

            if (String.IsNullOrEmpty(invoiceHeader.fechaEmision))
            {
                messageResult += "La fecha de emision de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacía.";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(invoiceHeader.tipoDocumentoAdquiriente))
            {
                messageResult += "El tipo de documento adquiriente de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacío.";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(invoiceHeader.numeroDocumentoAdquiriente))
            {
                messageResult += "El número de documento adquiriente de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacío.";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(invoiceHeader.razonSocialAdquiriente))
            {
                messageResult += "La razon social del adquiriente de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacía.";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(invoiceHeader.tipoMoneda))
            {
                messageResult += "El tipo de moneda de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacía.";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(invoiceHeader.tipooperacion))
            {
                messageResult += "El tipo de operación de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacía.";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(invoiceHeader.codigoestablecimientosunat))
            {
                messageResult += "El codigo de establecimiento sunat de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacía.";
                isValid &= false;
            }
            if (String.IsNullOrWhiteSpace(invoiceHeader.totalvalorventa))
            {
                messageResult += "El total del valor de venta de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacío.";
                isValid &= false;
            }
            return isValid;
        }

        private static bool ShouldDeleteInvoice(InvoiceDetail detail, ref string messageResult)
        {
            bool isValid = true;
            /*if (String.IsNullOrEmpty(detail.serieNumero) || detail.serieNumero.Length < 13 || !detail.serieNumero.StartsWith("F"))
            {
                messageResult += "La serie y número de la factura: " + detail.serieNumero + " tiene una longitud invalida o no cumple con el formato correcto";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(detail.descripcion))
            {
                messageResult += "La descripcion del detalle con número de orden: " + detail.numeroOrdenItem + " esta vacia.";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(detail.unidadMedida))
            {
                messageResult += "La unidad de medida del detalle con número de orden: " + detail.numeroOrdenItem + " esta vacía.";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(detail.codigoImpUnitConImpuesto))
            {
                messageResult += "El codigo de imp. unitario del detalle con número de orden: " + detail.numeroOrdenItem + " esta vacía.";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(detail.codigoRazonExoneracion))
            {
                messageResult += "El codigo de razon de exgoneracion del detalle con número de orden: " + detail.numeroOrdenItem + " esta vacía.";
                isValid &= false;
            }*/
            return !isValid;
        }

        public static bool ValidateInvoiceDetail(List<InvoiceDetail> invoiceDetail, ref string messageResult)
        {
            bool isValid = true;

            foreach (var detail in invoiceDetail.ToList())
            {
                if (ShouldDeleteInvoice(detail, ref messageResult))
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
                //Data.Atis.InsertInvoices(invoiceHeaders);
            }
            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
        }

        public static void InsertInvoicesDetails(List<InvoiceDetail> invoiceDetails)
        {
            try
            {
                //Data.Atis.InsertInvoiceDetails(invoiceDetails);
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
            string fileName = "FACT_03_" + current.ToString("yyyyMMddHHmmss") + ".txt";

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

                    if (!String.IsNullOrWhiteSpace(invoice.porcentajeRetencion) && !String.IsNullOrWhiteSpace(invoice.totalRetencion))
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

            //Data.Atis.UpdatePickupDate(invoiceHeaders.Select(x => x.serieNumero).ToList());
            // Data.Atis.InvokeInvoiceUpdate();

        }

        public static List<BillHeader> GetBillHeader(String filename, List<String> data, DateTime timestamp, ref int intentos, int maxIntentos)
        {
            List<BillHeader> ListHeaders = new List<BillHeader>();
            bool debeRepetir = false;
            try
            {
                for (int i = 0; i < maxIntentos; i++)
                {
                    ListHeaders = Data.Atis.ReadBillHeader(filename, data, timestamp, ref debeRepetir);
                    intentos++;
                    if (!debeRepetir) break;

                }

            }

            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
            return ListHeaders;
        }

        public static List<BillDetail> GetBillDetail(String filename, List<String> data, DateTime timestamp)
        {
            List<BillDetail> ListDetails = new List<BillDetail>();
            try
            {
                ListDetails = Data.Atis.ReadBillDetail(filename, data, timestamp);
            }

            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }

            return ListDetails;
        }

        public static bool ValidateBills(List<BillHeader> invoiceHeaders, ref string messageResult)
        {
            bool checkInvoice = true;

            foreach (var invoice in invoiceHeaders.ToList())
            {
                if (!ValidateBills(invoice, ref messageResult))
                {
                    invoiceHeaders.Remove(invoice);
                    checkInvoice &= false;
                }

            }

            return checkInvoice;
        }

        private static bool ValidateBills(BillHeader invoiceHeader, ref string messageResult)
        {
            bool isValid = true;
            if (String.IsNullOrEmpty(invoiceHeader.serieNumero) || invoiceHeader.serieNumero.Length < 13 || !invoiceHeader.serieNumero.StartsWith("B"))
            {
                messageResult += "La serie y número de la factura: " + invoiceHeader.serieNumero + " tiene una longitud invalida o no cumple con el formato correcto";
                isValid &= false;
            }

            if (String.IsNullOrEmpty(invoiceHeader.fechaEmision))
            {
                messageResult += "La fecha de emision de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacía.";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(invoiceHeader.tipoDocumentoAdquiriente))
            {
                messageResult += "El tipo de documento adquiriente de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacío.";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(invoiceHeader.numeroDocumentoAdquiriente))
            {
                messageResult += "El número de documento adquiriente de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacío.";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(invoiceHeader.razonSocialAdquiriente))
            {
                messageResult += "La razon social del adquiriente de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacía.";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(invoiceHeader.tipoMoneda))
            {
                messageResult += "El tipo de moneda de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacía.";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(invoiceHeader.tipooperacion))
            {
                messageResult += "El tipo de operación de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacía.";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(invoiceHeader.codigoestablecimientosunat))
            {
                messageResult += "El codigo de establecimiento sunat de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacía.";
                isValid &= false;
            }
            if (String.IsNullOrWhiteSpace(invoiceHeader.totalvalorventa))
            {
                messageResult += "El total del valor de venta de la factura con número de serie: " + invoiceHeader.serieNumero + " está vacío.";
                isValid &= false;
            }
            return isValid;
        }

        public static bool ValidateBillDetails(List<BillDetail> invoiceDetail, ref string messageResult)
        {
            bool isValid = true;

            foreach (var detail in invoiceDetail.ToList())
            {
                if (ShouldDeleteBill(detail, ref messageResult))
                {
                    invoiceDetail.Remove(detail);
                    isValid &= false;
                }

            }
            return isValid;
        }
        private static bool ShouldDeleteBill(BillDetail detail, ref string messageResult)
        {
            bool isValid = true;
            /*if (String.IsNullOrEmpty(detail.serieNumero) || detail.serieNumero.Length < 13 || !detail.serieNumero.StartsWith("F"))
            {
                messageResult += "La serie y número de la factura: " + detail.serieNumero + " tiene una longitud invalida o no cumple con el formato correcto";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(detail.descripcion))
            {
                messageResult += "La descripcion del detalle con número de orden: " + detail.numeroOrdenItem + " esta vacia.";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(detail.unidadMedida))
            {
                messageResult += "La unidad de medida del detalle con número de orden: " + detail.numeroOrdenItem + " esta vacía.";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(detail.codigoImpUnitConImpuesto))
            {
                messageResult += "El codigo de imp. unitario del detalle con número de orden: " + detail.numeroOrdenItem + " esta vacía.";
                isValid &= false;
            }
            if (String.IsNullOrEmpty(detail.codigoRazonExoneracion))
            {
                messageResult += "El codigo de razon de exgoneracion del detalle con número de orden: " + detail.numeroOrdenItem + " esta vacía.";
                isValid &= false;
            }*/
            return !isValid;
        }

        public static string CreateBillFile340(List<BillHeader> bills, List<BillDetail> currentBills, string path)
        {
            DateTime current = DateTime.Now;
            //string fileName = $"FACT_{current.Year}{current.Month}{current.Day}_{current.Hour}_{current.Year}{current.Month}{current.Day}{current.Hour}{current.Minute}{current.Second}.txt";
            string fileName = "BOLE_03_" + current.ToString("yyyyMMddHHmmss") + ".txt";

            using (StreamWriter writer = new StreamWriter(Path.Combine(path, fileName)))
            {
                foreach (BillHeader invoice in bills)
                {
                    writer.WriteLine($"C|{invoice.serieNumero}|{invoice.fechaEmision}|{invoice.Horadeemision}|" +
                        $"{invoice.tipoMoneda}|{invoice.numeroDocumentoEmisor}|{invoice.tipoDocumentoAdquiriente}|{invoice.numeroDocumentoAdquiriente}|" +
                        $"{invoice.razonSocialAdquiriente}|{invoice.direccionAdquiriente}|{invoice.tipoReferencia_1}|{invoice.numeroDocumentoReferencia_1}|" +
                        $"{invoice.tipoReferencia_2}|{invoice.numeroDocumentoReferencia_2}|{invoice.totalVVNetoOpGravadas}|{invoice.conceptovvnetoopnogravada}|" +
                        $"{invoice.totalVVNetoOpExoneradas}|{invoice.conceptovvnetoopexoneradas}|{invoice.totalVVNetoOpGratuitas}|" +
                        $"{invoice.conceptovvnetoopgratuitas}|{invoice.totalVVNetoExportacion}|{invoice.conceptovvexportacion}|{invoice.totalDescuentos}|{invoice.totalIgv}|" +
                        $"{invoice.totalVenta}|{invoice.tipooperacion}|{invoice.leyendas}|{invoice.datosAdicionales}|{invoice.codigoestablecimientosunat}|{invoice.montototalimpuestos}|{invoice.cdgcodigomotivo}|{invoice.cdgporcentaje}|" +
                        $"{invoice.descuentosGlobales}|{invoice.cdgmontobasecargo}|{invoice.sumimpuestosopgratuitas}|{invoice.totalvalorventa}|{invoice.totalprecioventa}|" +
                        $"{invoice.monredimporttotal}|{invoice.estado}||{invoice.origen}|{invoice.fechaRegistro}");

                    var currentDetails = currentBills.Where(x => x.serieNumero == invoice.serieNumero).ToList();

                    foreach (BillDetail billDetail in currentBills)
                    {

                        writer.WriteLine($"D|{billDetail.numeroOrdenItem}|{billDetail.unidadMedida}|{billDetail.cantidad}|" +
                            $"{billDetail.codigoProducto}|{billDetail.codigoproductosunat}|{billDetail.descripcion}|" +
                            $"{billDetail.montobaseigv}|{billDetail.importeIgv}|{billDetail.codigoRazonExoneracion}|{billDetail.tasaigv}|" +
                            $"{billDetail.importeDescuento}|{billDetail.codigodescuento}|{billDetail.factordescuento}|" +
                            $"{billDetail.montobasedescuento}|" +
                            $"{billDetail.codigoImporteReferencial}|{billDetail.importeReferencial}|" +
                            $"{billDetail.importeUnitarioSinImpuesto}|{billDetail.importeTotalSinImpuesto}|{billDetail.montototalimpuestoitem}|" +
                            $"||{billDetail.codigoImpUnitConImpuesto}|{billDetail.importeUnitarioConImpuesto}");
                    }


                }



            }
            return Path.Combine(path, fileName);
        }


        public static List<DebitNoteHeader> GetDebitNoteHeader(String filename, List<String> data, DateTime timestamp, ref int intentos, int maxIntentos)
        {
            List<DebitNoteHeader> ListHeaders = new List<DebitNoteHeader>();
            bool debeRepetir = false;
            try
            {
                for (int i = 0; i < maxIntentos; i++)
                {
                    ListHeaders = Data.Atis.ReadDebitNoteHeader(filename, data, timestamp, ref debeRepetir);
                    intentos++;
                    if (!debeRepetir) break;

                }

            }

            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
            return ListHeaders;
        }

        public static List<DebitNoteDetail> GetDebitNoteDetail(String filename, List<String> data, DateTime timestamp)
        {
            List<DebitNoteDetail> ListDetails = new List<DebitNoteDetail>();
            try
            {
                ListDetails = Data.Atis.ReadDebitNoteDetail(filename, data, timestamp);
            }

            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }

            return ListDetails;
        }

    }
}
