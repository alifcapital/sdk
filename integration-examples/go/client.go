package main

import (
	"bytes"
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"time"
)

// Client - the main client for the Alif Payment API
type Client struct {
	baseURL          string
	terminalID       string
	terminalPassword string
	httpClient       *http.Client
}

// NewClient creates a new Alif Payment API client
func NewClient(baseURL, terminalID, terminalPassword string) *Client {
	return &Client{
		baseURL:          baseURL,
		terminalID:       terminalID,
		terminalPassword: terminalPassword,
		httpClient: &http.Client{
			Timeout: 30 * time.Second,
		},
	}
}

type httpReqCfg struct {
	url     string
	method  string
	body    interface{}
	headers map[string]string
}

func (c *Client) doRequest(cfg httpReqCfg) (*Response, error) {
	var bodyBytes []byte
	var err error

	if cfg.body != nil {
		bodyBytes, err = json.Marshal(cfg.body)
		if err != nil {
			return nil, fmt.Errorf("failed to marshal request body: %w", err)
		}
	}

	req, err := http.NewRequest(cfg.method, cfg.url, bytes.NewReader(bodyBytes))
	if err != nil {
		return nil, fmt.Errorf("failed to create request: %w", err)
	}

	for key, value := range cfg.headers {
		req.Header.Set(key, value)
	}

	resp, err := c.httpClient.Do(req)
	if err != nil {
		return nil, fmt.Errorf("failed to execute request: %w", err)
	}
	defer resp.Body.Close()

	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, fmt.Errorf("failed to read response body: %w", err)
	}

	var respData Response
	if err := json.Unmarshal(body, &respData); err != nil {
		return nil, fmt.Errorf("failed to unmarshal response: %w (body: %s)", err, string(body))
	}

	// Handle error responses based on HTTP status codes (e.g., 400, 401, 403, 500)
	if resp.StatusCode != http.StatusOK {
		return &respData, &APIError{
			StatusCode: resp.StatusCode,
			Code:       respData.Code,
			Message:    respData.Message,
		}
	}

	// Handle error responses based on the response code
	if respData.Code != 200 {
		return &respData, &APIError{
			StatusCode: resp.StatusCode,
			Code:       respData.Code,
			Message:    respData.Message,
		}
	}

	return &respData, nil
}


// ------------------------------ Payment ------------------------------


// InitiatePayment initiates a payment through standard payment systems or external gateways
// Supports: korti_milli, wallet, salom, invoice (cash), vsa, mcr, cybersource_checkout
func (c *Client) InitiatePayment(req PaymentRequest) (*Response, error) {
	req.Key = c.terminalID

	dataToSign := c.terminalID + req.OrderID + req.Amount + req.CallbackURL
	req.Token = c.generateToken(dataToSign)
	gate := req.Gate // depends on the payment gateway, see the documentation

	cfg := httpReqCfg{
		url:    c.baseURL + "/v2/",
		method: http.MethodPost,
		body:   req,
		headers: map[string]string{
			"Content-Type": "application/json",
			"gate":         gate,
		},
	}

	resp, err := c.doRequest(cfg)
	if err != nil {
		return resp, err
	}

	return resp, nil
}


// ------------------------------ Tokenization ------------------------------


// InitiateTokenization initiates a tokenization process for payment methods
// Supports: tokenization_korti_milli, tokenization_wallet, tokenization_salom,
// tokenization_tcell, tokenization_megafon, tokenization_babilon,
// tokenization_zetmobile, tokenization_procard (Visa/Mastercard)
func (c *Client) InitiateTokenization(req TokenizationRequest) (*Response, error) {
	req.Data.Key = c.terminalID
	dataToSign := c.terminalID + req.Data.Phone + req.Data.Gate
	req.Data.Token = c.generateToken(dataToSign)

	gate := req.Data.Gate // depends on the tokenization type, see the documentation

	cfg := httpReqCfg{
		url:    c.baseURL + "/v2/",
		method: http.MethodPost,
		body:   req,
		headers: map[string]string{
			"Content-Type": "application/json",
			"gate":         gate,
		},
	}

	resp, err := c.doRequest(cfg)
	if err != nil {
		return resp, err
	}

	return resp, nil
}


// ------------------------------ Marketplace payment ------------------------------


// InitiateMarketplacePayment initiates a marketplace payment with split payments
// Supports: korti_milli, vsa, mcr, salom
func (c *Client) InitiateMarketplacePayment(req MarketplacePaymentRequest, gate string) (*Response, error) {
	req.Key = c.terminalID
	dataToSign := c.terminalID + req.OrderID + req.Amount + req.CallbackURL
	req.Token = c.generateToken(dataToSign)

	cfg := httpReqCfg{
		url:    c.baseURL + "/v2/",
		method: http.MethodPost,
		body:   req,
		headers: map[string]string{
			"Content-Type":  "application/json",
			"gate":          gate,
			"isMarketPlace": "true",
		},
	}

	resp, err := c.doRequest(cfg)
	if err != nil {
		return resp, err
	}

	return resp, nil
}

// ConfirmDelivery confirms delivery for hold-payments (all payment methods except VSA/MCR)
func (c *Client) ConfirmDelivery(req ConfirmDeliveryRequest) (*Response, error) {
	req.Key = c.terminalID
	dataToSign := c.terminalID + req.TransactionID + req.Amount
	req.Token = c.generateToken(dataToSign)

	cfg := httpReqCfg{
		url:    c.baseURL + "/confirm-delivery",
		method: http.MethodPost,
		body:   req,
		headers: map[string]string{
			"Content-Type": "application/json",
		},
	}

	resp, err := c.doRequest(cfg)
	if err != nil {
		return resp, err
	}

	return resp, nil
}

// ConfirmVsaMcrDelivery confirms delivery for Visa/Mastercard marketplace payments
func (c *Client) ConfirmVsaMcrDelivery(req ConfirmVsaMcrDeliveryRequest) (*Response, error) {
	req.Key = c.terminalID
	dataToSign := c.terminalID + req.ParentTransactionID
	req.Token = c.generateToken(dataToSign)

	cfg := httpReqCfg{
		url:    c.baseURL + "/confirm-vsa-and-mcr-delivery",
		method: http.MethodPost,
		body:   req,
		headers: map[string]string{
			"Content-Type": "application/json",
		},
	}

	resp, err := c.doRequest(cfg)
	if err != nil {
		return resp, err
	}

	return resp, nil
}
