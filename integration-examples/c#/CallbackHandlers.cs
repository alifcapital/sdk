using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlifPaymentSDK
{

    // ------------------------------ Payment callback ------------------------------


    public static class PaymentStatus
    {
        public const string OK = "ok";
        public const string Failed = "failed";
        public const string Canceled = "canceled";
        public const string Pending = "pending";
        public const string ToApprove = "to_approve";
    }

    public class CallbackData
    {
        [JsonPropertyName("orderId")]
        public string OrderID { get; set; }

        [JsonPropertyName("transactionId")]
        public string TransactionID { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("account")]
        public string Account { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("transaction_type")]
        public string TransactionType { get; set; }
    }

    public static class PaymentCallbackHandler
    {
        /// <summary>
        /// Example of payment callback handling
        /// Usage: Parse JSON from request body and pass to this method
        /// </summary>
        public static void HandlePaymentCallback(CallbackData data)
        {
            switch (data.Status)
            {
                case PaymentStatus.OK:
                    Console.WriteLine($"Payment successful: order={data.OrderID} amount={data.Amount:F2}");
                    break;
                case PaymentStatus.Failed:
                case PaymentStatus.Canceled:
                    Console.WriteLine($"Payment {data.Status}: order={data.OrderID} amount={data.Amount:F2}");
                    break;
                case PaymentStatus.Pending:
                case PaymentStatus.ToApprove:
                    Console.WriteLine($"Payment in progress: order={data.OrderID} status={data.Status}");
                    break;
                default:
                    Console.WriteLine($"Unknown payment status: {data.Status} for order={data.OrderID}");
                    break;
            }
        }
    }


    // ------------------------------ Tokenization callback ------------------------------


    public static class TokenizationCode
    {
        public const int Success = 1;
        public const int Duplicate = 2;
        public const int Rejected = 9;
    }

    public class TokenizationCallbackData
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("reason_code")]
        public string ReasonCode { get; set; }

        [JsonPropertyName("payload")]
        public TokenizationCallbackPayload Payload { get; set; }
    }

    public class TokenizationCallbackPayload
    {
        [JsonPropertyName("transactionId")]
        public int TransactionID { get; set; }

        [JsonPropertyName("orderId")]
        public string OrderID { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("account")]
        public string Account { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("transaction_type")]
        public string TransactionType { get; set; }
    }

    public static class TokenizationCallbackHandler
    {
        /// <summary>
        /// Example of tokenization callback handling
        /// Usage: Parse JSON from request body and pass to this method
        /// </summary>
        public static void HandleTokenizationCallback(TokenizationCallbackData data)
        {
            switch (data.Code)
            {
                case TokenizationCode.Success:
                    Console.WriteLine($"Tokenization successful: order={data.Payload.OrderID} token={data.Payload.Token}");
                    break;
                case TokenizationCode.Duplicate:
                    Console.WriteLine($"Tokenization duplicate: order={data.Payload.OrderID}");
                    break;
                case TokenizationCode.Rejected:
                    Console.WriteLine($"Tokenization failed: order={data.Payload.OrderID} reason={data.ReasonCode}");
                    break;
                default:
                    Console.WriteLine($"Unknown tokenization code: {data.Code} for order={data.Payload.OrderID}");
                    break;
            }
        }
    }


    // ------------------------------ Marketplace callback ------------------------------


    public class MarketplaceCallbackData
    {
        [JsonPropertyName("orderId")]
        public string OrderID { get; set; }

        [JsonPropertyName("transactionId")]
        public string TransactionID { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("account")]
        public string Account { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("transaction_type")]
        public string TransactionType { get; set; }

        [JsonPropertyName("sub_transactions")]
        public List<SubTransaction> SubTransactions { get; set; }
    }

    public class SubTransaction
    {
        [JsonPropertyName("terminal_id")]
        public string TerminalID { get; set; }

        [JsonPropertyName("transaction_id")]
        public string TransactionID { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public static class MarketplaceCallbackHandler
    {
        /// <summary>
        /// Example of marketplace callback handling
        /// Usage: Parse JSON from request body and pass to this method
        /// </summary>
        public static void HandleMarketplaceCallback(MarketplaceCallbackData data)
        {
            switch (data.Status)
            {
                case PaymentStatus.OK:
                    Console.WriteLine($"Marketplace payment successful: order={data.OrderID} amount={data.Amount:F2} sub_transactions={data.SubTransactions.Count}");
                    break;
                case PaymentStatus.Failed:
                case PaymentStatus.Canceled:
                    Console.WriteLine($"Marketplace payment {data.Status}: order={data.OrderID}");
                    break;
                case PaymentStatus.Pending:
                case PaymentStatus.ToApprove:
                    Console.WriteLine($"Marketplace payment in progress: order={data.OrderID} status={data.Status}");
                    break;
                default:
                    Console.WriteLine($"Unknown marketplace payment status: {data.Status} for order={data.OrderID}");
                    break;
            }
        }
    }

    // ------------------------------ Unified callback handlers ------------------------------

    /// <summary>
    /// Unified class for all callback handlers
    /// </summary>
    public static class CallbackHandlers
    {
        public static void HandlePaymentCallback(CallbackData data)
        {
            PaymentCallbackHandler.HandlePaymentCallback(data);
        }

        public static void HandleTokenizationCallback(TokenizationCallbackData data)
        {
            TokenizationCallbackHandler.HandleTokenizationCallback(data);
        }

        public static void HandleMarketplaceCallback(MarketplaceCallbackData data)
        {
            MarketplaceCallbackHandler.HandleMarketplaceCallback(data);
        }
    }
}

