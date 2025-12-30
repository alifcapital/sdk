using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlifPaymentSDK
{
    public class AlifClient
    {
        private readonly string _baseURL;
        private readonly string _terminalID;
        private readonly string _terminalPassword;
        private readonly HttpClient _httpClient;

        public AlifClient(string baseURL, string terminalID, string terminalPassword)
        {
            _baseURL = baseURL;
            _terminalID = terminalID;
            _terminalPassword = terminalPassword;
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        private async Task<Response> DoRequestAsync(string url, string method, object body, Dictionary<string, string> headers)
        {
            var request = new HttpRequestMessage(new HttpMethod(method), url);

            if (body != null)
            {
                var jsonBody = JsonSerializer.Serialize(body);
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            var response = await _httpClient.SendAsync(request);
            var responseBytes = await response.Content.ReadAsByteArrayAsync();
            var responseBody = Encoding.UTF8.GetString(responseBytes);
            var responseData = JsonSerializer.Deserialize<Response>(responseBody);

            // Handle error responses based on HTTP status codes (e.g., 400, 401, 403, 500)
            if (!response.IsSuccessStatusCode)
            {
                throw new APIException(
                    (int)response.StatusCode,
                    responseData?.Code ?? 0,
                    responseData?.Message ?? "Unknown error"
                );
            }

            // Handle error responses based on the response code
            if (responseData.Code != 200)
            {
                throw new APIException(
                    (int)response.StatusCode,
                    responseData.Code,
                    responseData.Message
                );
            }

            return responseData;
        }


        // ------------------------------ Payment ------------------------------


        /// <summary>
        /// Initiates a payment through standard payment systems or external gateways
        /// Supports: korti_milli, wallet, salom, invoice (cash), vsa, mcr, cybersource_checkout
        /// </summary>
        public async Task<Response> InitiatePaymentAsync(PaymentRequest request)
        {
            request.Key = _terminalID;
            var dataToSign = _terminalID + request.OrderID + request.Amount + request.CallbackURL;
            request.Token = TokenGenerator.GenerateToken(dataToSign, _terminalPassword, _terminalID);

            var gate = request.Gate; // depends on the payment gateway, see the documentation

            var headers = new Dictionary<string, string>
            {
                { "gate", gate }
            };

            return await DoRequestAsync(_baseURL + "/v2/", "POST", request, headers);
        }


        // ------------------------------ Tokenization ------------------------------


        /// <summary>
        /// Initiates a tokenization process for payment methods
        /// Supports: tokenization_korti_milli, tokenization_wallet, tokenization_salom,
        /// tokenization_tcell, tokenization_megafon, tokenization_babilon,
        /// tokenization_zetmobile, tokenization_procard (Visa/Mastercard)
        /// </summary>
        public async Task<Response> InitiateTokenizationAsync(TokenizationRequest request)
        {
            request.Data.Key = _terminalID;
            var dataToSign = _terminalID + request.Data.Phone + request.Data.Gate;
            request.Data.Token = TokenGenerator.GenerateToken(dataToSign, _terminalPassword, _terminalID);

            var gate = request.Data.Gate; // depends on the tokenization type, see the documentation

            var headers = new Dictionary<string, string>
            {
                { "gate", gate }
            };

            return await DoRequestAsync(_baseURL + "/v2/", "POST", request, headers);
        }


        // ------------------------------ Marketplace payment ------------------------------


        /// <summary>
        /// Initiates a marketplace payment with split payments
        /// Supports: korti_milli, vsa, mcr, salom
        /// </summary>
        public async Task<Response> InitiateMarketplacePaymentAsync(MarketplacePaymentRequest request, string gate)
        {
            request.Key = _terminalID;
            var dataToSign = _terminalID + request.OrderID + request.Amount + request.CallbackURL;
            request.Token = TokenGenerator.GenerateToken(dataToSign, _terminalPassword, _terminalID);

            var headers = new Dictionary<string, string>
            {
                { "gate", gate },
                { "isMarketPlace", "true" }
            };

            return await DoRequestAsync(_baseURL + "/v2/", "POST", request, headers);
        }

        /// <summary>
        /// Confirms delivery for hold-payments (all payment methods except VSA/MCR)
        /// </summary>
        public async Task<Response> ConfirmDeliveryAsync(ConfirmDeliveryRequest request)
        {
            request.Key = _terminalID;
            var dataToSign = _terminalID + request.TransactionID + request.Amount;
            request.Token = TokenGenerator.GenerateToken(dataToSign, _terminalPassword, _terminalID);

            var headers = new Dictionary<string, string>();

            return await DoRequestAsync(_baseURL + "/confirm-delivery", "POST", request, headers);
        }

        /// <summary>
        /// Confirms delivery for Visa/Mastercard marketplace payments
        /// </summary>
        public async Task<Response> ConfirmVsaMcrDeliveryAsync(ConfirmVsaMcrDeliveryRequest request)
        {
            request.Key = _terminalID;
            var dataToSign = _terminalID + request.ParentTransactionID;
            request.Token = TokenGenerator.GenerateToken(dataToSign, _terminalPassword, _terminalID);

            var headers = new Dictionary<string, string>();

            return await DoRequestAsync(_baseURL + "/confirm-vsa-and-mcr-delivery", "POST", request, headers);
        }
    }
}

