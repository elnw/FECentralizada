﻿using System;
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

        public static List<CreditNoteDetail> GetCreditNoteDetails(DateTime timestamp)
        {
            List<CreditNoteDetail> ListHeaders = new List<CreditNoteDetail>();
            CreditNoteDetail objBillHeader = new CreditNoteDetail();

            Tools.Logging.Info("Iniciando Consulta BD- Cabecera");
            try
            {


                ListHeaders = Data.Pacifyc.ReadCreditNoteDetails(timestamp);



                Tools.Logging.Info("Fin Consulta BD- Cabecera");
                Tools.Logging.Info("Iniciando registro en BD - Cabecera");
            }

            catch (Exception ex)
            {
                Tools.Logging.Error(ex.Message);
            }
            return ListHeaders;
        }

        public static List<DebitNoteHeader> GetDebitNoteHeaders(DateTime timestamp, ref int intentos, int maxIntentos)
        {
            List<DebitNoteHeader> ListHeaders = new List<DebitNoteHeader>();

            bool debeRepetir = false;
            Tools.Logging.Info("Iniciando Consulta BD- Cabecera");
            try
            {
                for (int i = 0; i < maxIntentos; i++)
                {
                    ListHeaders = Data.Pacifyc.ReadDebitNoteHeader(timestamp, ref debeRepetir);
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

        public static List<DebitNoteDetail> GetDebitNoteDetails(DateTime timestamp)
        {
            List<DebitNoteDetail> ListDetails = new List<DebitNoteDetail>();
            DebitNoteDetail objBillHeader = new DebitNoteDetail();

            Tools.Logging.Info("Iniciando Consulta BD- Cabecera");
            try
            {
                ListDetails = Data.Pacifyc.ReadDebitNoteDetails(timestamp);
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

        private static bool ValidateCreditNoteHeader(CreditNoteHeader creditNoteHeader, List<string> messages)
        {
            bool checkCN = true;

            if (String.IsNullOrEmpty(creditNoteHeader.fechaEmision))
            {
                checkCN &= false;
                messages.Add($"La cabecera de la nota de crédito con serie: {creditNoteHeader.serieNumero} tiene la fecha de emisión vacía");
            }

            if (String.IsNullOrEmpty(creditNoteHeader.codigoSerieNumeroAfectado))
            {
                checkCN &= false;
                messages.Add($"La cabecera de la nota de crédito con serie: {creditNoteHeader.serieNumero} tiene el codigo serie numero afectado vacío");
            }

            if (String.IsNullOrEmpty(creditNoteHeader.serieNumeroAfectado))
            {
                checkCN &= false;
                messages.Add($"La cabecera de la nota de crédito con serie: {creditNoteHeader.serieNumero} tiene la serie de numero afectado vacío");
            }
            if (String.IsNullOrEmpty(creditNoteHeader.razonSocialAdquiriente))
            {
                checkCN &= false;
                messages.Add($"La cabecera de la nota de crédito con serie: {creditNoteHeader.serieNumero} tiene la razón social adquiriente vacía");
            }
            if (String.IsNullOrEmpty(creditNoteHeader.correoAdquiriente))
            {
                checkCN &= false;
                messages.Add($"La cabecera de la nota de crédito con serie: {creditNoteHeader.serieNumero} tiene el correo adquiriente vacío");
            }
            if (String.IsNullOrEmpty(creditNoteHeader.motivoDocumento))
            {
                checkCN &= false;
                messages.Add($"La cabecera de la nota de crédito con serie: {creditNoteHeader.serieNumero} tiene el motivo del documento vacío");
            }
            if (String.IsNullOrEmpty(creditNoteHeader.tipoMoneda))
            {
                checkCN &= false;
                messages.Add($"La cabecera de la nota de crédito con serie: {creditNoteHeader.serieNumero} tiene el tipo de moneda vacío");
            }
            if (String.IsNullOrEmpty(creditNoteHeader.tipoDocRefPrincipal))
            {
                checkCN &= false;
                messages.Add($"La cabecera de la nota de crédito con serie: {creditNoteHeader.serieNumero} tiene el tipo de doc ref principal vacío");
            }
            if (String.IsNullOrEmpty(creditNoteHeader.numeroDocRefPrincipal))
            {
                checkCN &= false;
                messages.Add($"La cabecera de la nota de crédito con serie: {creditNoteHeader.serieNumero} tiene el numero de doc ref principal vacío");
            }
            return checkCN;
        }


        public static bool ValidateCreditNoteHeaders(List<CreditNoteHeader> creditNoteHeaders, List<string> messages)
        {
            bool checkCN = true;

            foreach(CreditNoteHeader item in creditNoteHeaders.ToList())
            {
                if(!ValidateCreditNoteHeader(item, messages))
                {
                    checkCN &= false;
                    creditNoteHeaders.Remove(item);
                }
                
                
            }
            return checkCN;
        }

        private static bool CheckDebitNote(DebitNoteHeader debitNoteHeader, List<string> messages)
        {
            bool checkDN = true;

            if (String.IsNullOrEmpty(debitNoteHeader.fechaEmision))
            {
                checkDN &= false;
                messages.Add( $"La nota de débito con número de serie: {debitNoteHeader.serieNumero} tiene la fecha de emisión vacía");
            }
            if (String.IsNullOrEmpty(debitNoteHeader.codigoSerieNumeroAfectado))
            {
                checkDN &= false;
                messages.Add( $"La nota de débito con número de serie: {debitNoteHeader.serieNumero} tiene el codigo de serie número afectado vacío");
            }
            if (String.IsNullOrEmpty(debitNoteHeader.serieNumeroAfectado))
            {
                checkDN &= false;
                messages.Add( $"La nota de débito con número de serie: {debitNoteHeader.serieNumero} tiene el serie número afectado vacío");
            }
            if (String.IsNullOrEmpty(debitNoteHeader.tipoDocumentoAdquiriente))
            {
                checkDN &= false;
                messages.Add( $"La nota de débito con número de serie: {debitNoteHeader.serieNumero} tiene el tipo de documento adquiriente vacío");
            }
            if (String.IsNullOrEmpty(debitNoteHeader.numeroDocumentoAdquiriente))
            {
                checkDN &= false;
                messages.Add( $"La nota de débito con número de serie: {debitNoteHeader.serieNumero} tiene el número de documento adquiriente vacío");
            }
            if (String.IsNullOrEmpty(debitNoteHeader.razonSocialAdquiriente))
            {
                checkDN &= false;
                messages.Add( $"La nota de débito con número de serie: {debitNoteHeader.serieNumero} tiene la razón social del adquiriente vacío");
            }
            if (String.IsNullOrEmpty(debitNoteHeader.correoAdquiriente))
            {
                checkDN &= false;
                messages.Add( $"La nota de débito con número de serie: {debitNoteHeader.serieNumero} tiene el correo del adquiriente vacío");
            }
            if (String.IsNullOrEmpty(debitNoteHeader.motivoDocumento))
            {
                checkDN &= false;
                messages.Add( $"La nota de débito con número de serie: {debitNoteHeader.serieNumero} tiene el motivo del documento vacío");
            }
            if (String.IsNullOrEmpty(debitNoteHeader.tipoMoneda))
            {
                checkDN &= false;
                messages.Add( $"La nota de débito con número de serie: {debitNoteHeader.serieNumero} tiene el tipo de moneda vacío");
            }
            if (String.IsNullOrEmpty(debitNoteHeader.tipoDocRefPrincipal))
            {
                checkDN &= false;
                messages.Add( $"La nota de débito con número de serie: {debitNoteHeader.serieNumero} tiene el tipo de doc ref principal vacío");
            }
            if (String.IsNullOrEmpty(debitNoteHeader.numeroDocRefPrincipal))
            {
                checkDN &= false;
                messages.Add( $"La nota de débito con número de serie: {debitNoteHeader.serieNumero} tiene el número de doc ref principal vacío");
            }
            return checkDN;
        }

        public static bool CheckDebitNotes(List<DebitNoteHeader> debitNoteHeaders, List<string> messages)
        {
            bool checkDN = true;

            foreach (DebitNoteHeader item in debitNoteHeaders.ToList())
            {
                if (!CheckDebitNote(item, messages))
                {
                    checkDN &= false;
                    debitNoteHeaders.Remove(item);
                }

            }
            return checkDN;
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

        public static void UpdateInvoicePickUpDate(List<InvoiceHeader> invoiceHeaders)
        {


            Data.Pacifyc.UpdatePickupDate(invoiceHeaders.Select(x => x.serieNumero).ToList(), "TEMP_SERIES");
            Data.Pacifyc.InvokeUpdate("PKG_PACIFYC_TRANSACCIONES.SP_ACTUALIZAR_FECH_RECOJO_FACT");

        }
        public static void UpdateCreditNotePickUpDate(List<CreditNoteHeader> invoiceHeaders)
        {
            Data.Pacifyc.UpdatePickupDate(invoiceHeaders.Select(x => x.serieNumero).ToList(), "temp_series_nc");
            Data.Pacifyc.InvokeUpdate("PKG_PACIFYC_TRANSACCIONES.SP_ACTUALIZAR_FECH_RECOJO_NCRE");
        }
        public static void UpdateDebitNotePickUpDate(List<DebitNoteHeader> debitNoteHeaders)
        {
            Data.Pacifyc.UpdatePickupDate(debitNoteHeaders.Select(x => x.serieNumero).ToList(), "temp_series_dn");
            Data.Pacifyc.InvokeUpdate("PKG_PACIFYC_TRANSACCIONES.SP_ACTUALIZAR_FECH_RECOJO_NDEB");
        }

        public static string CreateInvoiceFile340(List<InvoiceHeader> invoices, List<InvoiceDetail> invoiceDetails, string path)
        {
            DateTime current = DateTime.Now;
            string fileName = "FACT_02" + current.ToString("_yyyyMMddHHmmss") + ".txt";

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
                        $"{invoice.monredimporttotal}|||||||||");

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
            string fileName = "FACT_02" + current.ToString("_yyyyMMddHHmmss") + ".txt";

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
                         $"{invoice.monredimporttotal}|Contado::-::-::-::-|{codigoMotivo}|{invoice.porcentajeDetraccion}|{invoice.totalRetencion}|{montoBaseRetencion}||||");

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

        public static string CreateDebitNoteFile340(List<DebitNoteHeader> debitNoteHeaders, List<DebitNoteDetail> debitNoteDetails, string path)
        {
            DateTime current = DateTime.Now;
            string fileName = "NDEB_02" + current.ToString("_yyyyMMddHHmmss") + ".txt";

            using (StreamWriter writer = new StreamWriter(Path.Combine(path, fileName)))
            {
                foreach (DebitNoteHeader dnHeader in debitNoteHeaders)
                {
                    writer.WriteLine($"C|{dnHeader.serieNumero}|{dnHeader.fechaEmision}|{dnHeader.horadeEmision}|" +
                        $"{dnHeader.codigoSerieNumeroAfectado}|{dnHeader.tipoMoneda}|{dnHeader.numeroDocumentoEmisor}|{dnHeader.tipoDocumentoAdquiriente}|" +
                        $"{dnHeader.numeroDocumentoAdquiriente}|{dnHeader.razonSocialAdquiriente}||{dnHeader.tipoDocRefPrincipal}|" +
                        $"{dnHeader.numeroDocRefPrincipal}|{dnHeader.tipoReferencia_1}|{dnHeader.numeroDocumentoReferencia_1}|{dnHeader.tipoReferencia_2}|" +
                        $"{dnHeader.numeroDocumentoReferencia_2}|{dnHeader.motivoDocumento}|{dnHeader.totalvalorventanetoopgravadas}|{dnHeader.totalVVNetoOpNoGravada}|" +
                        $"{dnHeader.conceptoVVNetoOpNoGravada}|{dnHeader.totalVVNetoOpExoneradas}|{dnHeader.conceptoVVNetoOpExoneradas}|{dnHeader.totalVVNetoOpGratuitas}|{dnHeader.conceptoVVNetoOpGratuitas}|" +
                        $"{dnHeader.totalVVNetoExportacion}|{dnHeader.conceptoVVExportacion}|{dnHeader.totalIgv}|{dnHeader.totalVenta}|{dnHeader.leyendas}|{dnHeader.datosAdicionales}|{dnHeader.codigoEstablecimientoSunat}|" +
                        $"{dnHeader.montoTotalImpuestos}|{dnHeader.sumImpuestosOpGratuitas}|{dnHeader.monRedImportTotal}||||");

                    var currentDetails = debitNoteDetails.Where(x => x.serieNumero == dnHeader.serieNumero).ToList();


                    foreach (DebitNoteDetail dnDetail in currentDetails)
                    {

                        writer.WriteLine($"D|{dnDetail.numeroOrdenItem}|{dnDetail.unidadMedida}|{dnDetail.cantidad}|" +
                            $"{dnDetail.codigoProducto}|{dnDetail.codigoProductoSunat}|{dnDetail.descripcion}|" +
                            $"{dnDetail.montoBaseIGV}|{dnDetail.importeIGV}|{dnDetail.codigoRazonExoneracion}|{dnDetail.tasaIGV}|" +
                            $"{dnDetail.importeUnitarioSinImpuesto}|{dnDetail.importeTotalSinImpuesto}|{dnDetail.montoTotalImpuestoItem}|" +
                            $"{dnDetail.codigoImpUnitConImpuesto}|{dnDetail.importeUnitarioConImpuesto}");
                    }
                }
            }
            return Path.Combine(path, fileName);
        }


        


        public static string CreateCreditNoteFile340(List<CreditNoteHeader> creditNoteHeaders, List<CreditNoteDetail> creditNoteDetails, string path)
        {
            DateTime current = DateTime.Now;
            string fileName = "NCRE_02"  +current.ToString("_yyyyMMddHHmmss") + ".txt";
            using (StreamWriter writer = new StreamWriter(Path.Combine(path, fileName)))
            {
                foreach (CreditNoteHeader creditNoteHeader in creditNoteHeaders)
                {
                    writer.WriteLine($"C|{creditNoteHeader.serieNumero}|{creditNoteHeader.fechaEmision}|{creditNoteHeader.horadeEmision}|{creditNoteHeader.codigoSerieNumeroAfectado}|" +
                        $"{creditNoteHeader.tipoMoneda}|{creditNoteHeader.numeroDocumentoEmisor}|{creditNoteHeader.tipoDocumentoAdquiriente}|{creditNoteHeader.numeroDocumentoAdquiriente}|" +
                        $"{creditNoteHeader.razonSocialAdquiriente}|{creditNoteHeader.lugarDestino}|{creditNoteHeader.tipoDocRefPrincipal}|{creditNoteHeader.tipoReferencia_1}|{creditNoteHeader.numeroDocumentoReferencia_1}|" +
                        $"{creditNoteHeader.tipoReferencia_2}|{creditNoteHeader.numeroDocumentoReferencia_2}|{creditNoteHeader.motivoDocumento}|{creditNoteHeader.totalvalorventanetoopgravadas}|{creditNoteHeader.totalVVNetoOpNoGravada}|" +
                        $"{creditNoteHeader.conceptoVVNetoOpNoGravada}|{creditNoteHeader.totalVVNetoOpExoneradas}|{creditNoteHeader.conceptoVVNetoOpExoneradas}|{creditNoteHeader.totalVVNetoOpGratuitas}|" +
                        $"{creditNoteHeader.conceptoVVNetoOpGratuitas}|{creditNoteHeader.totalVVNetoExportacion}|{creditNoteHeader.conceptoVVExportacion}|{creditNoteHeader.totalIgv}|{creditNoteHeader.totalVenta}|" +
                        $"{creditNoteHeader.leyendas}||{creditNoteHeader.codigoEstablecimientoSunat}|{creditNoteHeader.montoTotalImpuestos}|{creditNoteHeader.sumImpuestosOpGratuitas}|{creditNoteHeader.monRedImportTotal}|" +
                        $"||||");

                    var currentDetails = creditNoteDetails.Where(x => x.serieNumero == creditNoteHeader.serieNumero).ToList();

                    foreach (CreditNoteDetail cnDetail in currentDetails)
                    {

                        writer.WriteLine($"D|{cnDetail.numeroOrdenItem}|{cnDetail.unidadMedida}|{cnDetail.cantidad}|" +
                            $"{cnDetail.codigoProducto}|{cnDetail.codigoProductoSunat}|{cnDetail.descripcion}|" +
                            $"{cnDetail.montoBaseIGV}|{cnDetail.importeIGV}|{cnDetail.codigoRazonExoneracion}|{cnDetail.tasaIGV}|" +
                            $"{cnDetail.codigoImporteReferencial}|{cnDetail.importeReferencial}|{cnDetail.importeUnitarioSinImpuesto}|" +
                            $"{cnDetail.importeTotalSinImpuesto}|{cnDetail.montoTotalImpuestoItem}|{cnDetail.codigoImpUnitConImpuesto}|" +
                            $"{cnDetail.importeUnitarioConImpuesto}");
                    }
                }
            }
            return Path.Combine(path, fileName);
        }

        public static string CreateCreditNoteFile193(List<CreditNoteHeader> creditNoteHeaders, List<CreditNoteDetail> creditNoteDetails, string path)
        {
            DateTime current = DateTime.Now;
            string fileName = "NCRE_02" + current.ToString("_yyyyMMddHHmmss") + ".txt"; ;

            using (StreamWriter writer = new StreamWriter(Path.Combine(path, fileName)))
            {
                foreach (CreditNoteHeader creditNoteHeader in creditNoteHeaders)
                {
                    writer.WriteLine($"C|{creditNoteHeader.serieNumero}|{creditNoteHeader.fechaEmision}|{creditNoteHeader.horadeEmision}|{creditNoteHeader.codigoSerieNumeroAfectado}|" +
                        $"{creditNoteHeader.tipoMoneda}|{creditNoteHeader.numeroDocumentoEmisor}|{creditNoteHeader.tipoDocumentoAdquiriente}|{creditNoteHeader.numeroDocumentoAdquiriente}|" +
                        $"{creditNoteHeader.razonSocialAdquiriente}|{creditNoteHeader.lugarDestino}|{creditNoteHeader.tipoDocRefPrincipal}|{creditNoteHeader.tipoReferencia_1}|{creditNoteHeader.numeroDocumentoReferencia_1}|" +
                        $"{creditNoteHeader.tipoReferencia_2}|{creditNoteHeader.numeroDocumentoReferencia_2}|{creditNoteHeader.motivoDocumento}|{creditNoteHeader.totalvalorventanetoopgravadas}|{creditNoteHeader.totalVVNetoOpNoGravada}|" +
                        $"{creditNoteHeader.conceptoVVNetoOpNoGravada}|{creditNoteHeader.totalVVNetoOpExoneradas}|{creditNoteHeader.conceptoVVNetoOpExoneradas}|{creditNoteHeader.totalVVNetoOpGratuitas}|" +
                        $"{creditNoteHeader.conceptoVVNetoOpGratuitas}|{creditNoteHeader.totalVVNetoExportacion}|{creditNoteHeader.conceptoVVExportacion}|{creditNoteHeader.totalIgv}|{creditNoteHeader.totalVenta}|" +
                        $"{creditNoteHeader.leyendas}||{creditNoteHeader.codigoEstablecimientoSunat}|{creditNoteHeader.montoTotalImpuestos}|{creditNoteHeader.sumImpuestosOpGratuitas}|{creditNoteHeader.monRedImportTotal}|" +
                        $"|||||");

                    var currentDetails = creditNoteDetails.Where(x => x.serieNumero == creditNoteHeader.serieNumero).ToList();

                    foreach (CreditNoteDetail cnDetail in currentDetails)
                    {

                        writer.WriteLine($"D|{cnDetail.numeroOrdenItem}|{cnDetail.unidadMedida}|{cnDetail.cantidad}|" +
                            $"{cnDetail.codigoProducto}|{cnDetail.codigoProductoSunat}|{cnDetail.descripcion}|" +
                            $"{cnDetail.montoBaseIGV}|{cnDetail.importeIGV}|{cnDetail.codigoRazonExoneracion}|{cnDetail.tasaIGV}|" +
                            $"{cnDetail.codigoImporteReferencial}|{cnDetail.importeReferencial}|{cnDetail.importeUnitarioSinImpuesto}|" +
                            $"{cnDetail.importeTotalSinImpuesto}|{cnDetail.montoTotalImpuestoItem}|{cnDetail.codigoImpUnitConImpuesto}|" +
                            $"{cnDetail.importeUnitarioConImpuesto}");
                    }

                }
            }
            return Path.Combine(path, fileName);
        }


    }
}
