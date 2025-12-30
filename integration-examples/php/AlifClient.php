<?php

namespace AlifPaymentSDK;

class AlifClient
{
    private string $baseURL;
    private string $terminalID;
    private string $terminalPassword;

    public function __construct(string $baseURL, string $terminalID, string $terminalPassword)
    {
        $this->baseURL = $baseURL;
        $this->terminalID = $terminalID;
        $this->terminalPassword = $terminalPassword;
    }

    private function doRequest(string $url, string $method, ?array $body, array $headers): array
    {
        // Prepare headers
        if ($body !== null) {
            $headers['Content-Type'] = 'application/json';
        }
        
        $httpHeaders = [];
        foreach ($headers as $key => $value) {
            $httpHeaders[] = "$key: $value";
        }

        // Prepare context options
        $options = [
            'http' => [
                'method' => $method,
                'header' => implode("\r\n", $httpHeaders),
                'timeout' => 30,
                'ignore_errors' => true
            ]
        ];

        if ($body !== null) {
            $options['http']['content'] = json_encode($body);
        }

        $context = stream_context_create($options);
        $response = file_get_contents($url, false, $context);

        if ($response === false) {
            throw new \Exception('Failed to execute request');
        }

        // Get HTTP response code
        $httpCode = 200;
        if (isset($http_response_header)) {
            foreach ($http_response_header as $header) {
                if (preg_match('/^HTTP\/\d\.\d\s+(\d+)/', $header, $matches)) {
                    $httpCode = (int)$matches[1];
                    break;
                }
            }
        }

        $responseData = json_decode($response, true);

        // Handle error responses based on HTTP status codes (e.g., 400, 401, 403, 500)
        if ($httpCode !== 200) {
            throw new APIException(
                $httpCode,
                $responseData['code'] ?? 0,
                $responseData['message'] ?? 'Unknown error'
            );
        }

        // Handle error responses based on the response code
        if (($responseData['code'] ?? 0) !== 200) {
            throw new APIException(
                $httpCode,
                $responseData['code'],
                $responseData['message']
            );
        }

        return $responseData;
    }


    // ------------------------------ Payment ------------------------------


    /**
     * Initiates a payment through standard payment systems or external gateways
     * Supports: korti_milli, wallet, salom, invoice (cash), vsa, mcr, cybersource_checkout
     */
    public function initiatePayment(array $request): array
    {
        $request['key'] = $this->terminalID;
        $dataToSign = $this->terminalID . $request['order_id'] . $request['amount'] . $request['callback_url'];
        $request['token'] = TokenGenerator::generateToken($dataToSign, $this->terminalPassword, $this->terminalID);

        $gate = $request['gate'] ?? ''; // depends on the payment gateway, see the documentation

        $headers = ['gate' => $gate];

        return $this->doRequest($this->baseURL . '/v2/', 'POST', $request, $headers);
    }


    // ------------------------------ Tokenization ------------------------------


    /**
     * Initiates a tokenization process for payment methods
     * Supports: tokenization_korti_milli, tokenization_wallet, tokenization_salom,
     * tokenization_tcell, tokenization_megafon, tokenization_babilon,
     * tokenization_zetmobile, tokenization_procard (Visa/Mastercard)
     */
    public function initiateTokenization(array $request): array
    {
        $request['data']['key'] = $this->terminalID;
        $dataToSign = $this->terminalID . $request['data']['phone'] . $request['data']['gate'];
        $request['data']['token'] = TokenGenerator::generateToken($dataToSign, $this->terminalPassword, $this->terminalID);

        $gate = $request['data']['gate']; // depends on the tokenization type, see the documentation

        $headers = ['gate' => $gate];

        return $this->doRequest($this->baseURL . '/v2/', 'POST', $request, $headers);
    }


    // ------------------------------ Marketplace payment ------------------------------


    /**
     * Initiates a marketplace payment with split payments
     * Supports: korti_milli, vsa, mcr, salom
     */
    public function initiateMarketplacePayment(array $request, string $gate): array
    {
        $request['key'] = $this->terminalID;
        $dataToSign = $this->terminalID . $request['order_id'] . $request['amount'] . $request['callback_url'];
        $request['token'] = TokenGenerator::generateToken($dataToSign, $this->terminalPassword, $this->terminalID);

        $headers = [
            'gate' => $gate,
            'isMarketPlace' => 'true'
        ];

        return $this->doRequest($this->baseURL . '/v2/', 'POST', $request, $headers);
    }

    /**
     * Confirms delivery for hold-payments (all payment methods except VSA/MCR)
     */
    public function confirmDelivery(array $request): array
    {
        $request['key'] = $this->terminalID;
        $dataToSign = $this->terminalID . $request['transaction_id'] . $request['amount'];
        $request['token'] = TokenGenerator::generateToken($dataToSign, $this->terminalPassword, $this->terminalID);

        $headers = [];

        return $this->doRequest($this->baseURL . '/confirm-delivery', 'POST', $request, $headers);
    }

    /**
     * Confirms delivery for Visa/Mastercard marketplace payments
     */
    public function confirmVsaMcrDelivery(array $request): array
    {
        $request['key'] = $this->terminalID;
        $dataToSign = $this->terminalID . $request['parent_transaction_id'];
        $request['token'] = TokenGenerator::generateToken($dataToSign, $this->terminalPassword, $this->terminalID);

        $headers = [];

        return $this->doRequest($this->baseURL . '/confirm-vsa-and-mcr-delivery', 'POST', $request, $headers);
    }
}

