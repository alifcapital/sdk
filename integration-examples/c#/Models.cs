using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlifPaymentSDK
{

    // ------------------------------ Payment ------------------------------


    public class PaymentRequest
    {
        // Mandatory fields
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("order_id")]
        public string OrderID { get; set; }

        [JsonPropertyName("amount")]
        public string Amount { get; set; }

        [JsonPropertyName("callback_url")]
        public string CallbackURL { get; set; }

        [JsonPropertyName("return_url")]
        public string ReturnURL { get; set; }

        // Optional fields
        [JsonPropertyName("email")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Email { get; set; }

        [JsonPropertyName("phone")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Phone { get; set; }

        [JsonPropertyName("info")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Info { get; set; }

        [JsonPropertyName("gate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Gate { get; set; }

        [JsonPropertyName("deadline")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Deadline { get; set; } // For cash payments (invoice gate)

        [JsonPropertyName("invoices")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Invoices Invoices { get; set; } // Mandatory if enabled in the terminal settings
    }

    public class Invoice
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("price")]
        public string Price { get; set; }

        [JsonPropertyName("vat_percent")]
        public string VatPercent { get; set; }
    }

    public class Invoices
    {
        [JsonPropertyName("invoices")]
        public List<Invoice> InvoiceList { get; set; }
    }


    // ------------------------------ Tokenization ------------------------------


    public class TokenizationRequest
    {
        [JsonPropertyName("data")]
        public TokenizationData Data { get; set; }
    }

    public class TokenizationData
    {
        [JsonPropertyName("orderId")]
        public string OrderID { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("callbackURL")]
        public string CallbackURL { get; set; }

        [JsonPropertyName("returnURL")]
        public string ReturnURL { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("gate")]
        public string Gate { get; set; }

        [JsonPropertyName("clientID")]
        public string ClientID { get; set; }
    }


    // ------------------------------ Marketplace payment ------------------------------


    public class MarketplacePaymentRequest
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("order_id")]
        public string OrderID { get; set; }

        [JsonPropertyName("amount")]
        public string Amount { get; set; }

        [JsonPropertyName("callback_url")]
        public string CallbackURL { get; set; }

        [JsonPropertyName("return_url")]
        public string ReturnURL { get; set; }

        [JsonPropertyName("email")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Email { get; set; }

        [JsonPropertyName("phone")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Phone { get; set; }

        [JsonPropertyName("info")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Info { get; set; }

        [JsonPropertyName("mpTerminalInfo")]
        public List<MpTerminalInfo> MpTerminalInfo { get; set; }
    }

    public class MpTerminalInfo
    {
        [JsonPropertyName("terminal_id")]
        public string TerminalID { get; set; }

        [JsonPropertyName("amount")]
        public string Amount { get; set; }

        [JsonPropertyName("condition_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int ConditionID { get; set; }

        [JsonPropertyName("invoices")]
        public List<MarketplaceInvoice> Invoices { get; set; }
    }

    public class MarketplaceInvoice
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("price")]
        public string Price { get; set; }
    }

    public class ConfirmDeliveryRequest
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("transaction_id")]
        public string TransactionID { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("amount")]
        public string Amount { get; set; }
    }

    public class ConfirmVsaMcrDeliveryRequest
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("parent_transaction_id")]
        public string ParentTransactionID { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("extra")]
        public List<VsaMcrDeliveryTransaction> Extra { get; set; }
    }

    public class VsaMcrDeliveryTransaction
    {
        [JsonPropertyName("transaction_id")]
        public string TransactionID { get; set; }

        [JsonPropertyName("amount")]
        public string Amount { get; set; }
    }


    // ------------------------------ Common ------------------------------


    public class Response
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("url")]
        public string URL { get; set; }
    }

    public class APIException : Exception
    {
        public int StatusCode { get; set; }
        public int Code { get; set; }
        public new string Message { get; set; }

        public APIException(int statusCode, int code, string message)
            : base($"API error (HTTP {statusCode}, code {code}): {message}")
        {
            StatusCode = statusCode;
            Code = code;
            Message = message;
        }
    }
}

