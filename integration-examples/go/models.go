package main

import "fmt"


// ------------------------------ Payment ------------------------------


// PaymentRequest - used for Standard payment and Payment via external gateway
type PaymentRequest struct {
	// Mandatory fields
	Key         string `json:"key"`
	Token       string `json:"token"`
	OrderID     string `json:"order_id"`
	Amount      string `json:"amount"`
	CallbackURL string `json:"callback_url"`
	ReturnURL   string `json:"return_url"`

	// Optional fields
	Email    string `json:"email,omitempty"`
	Phone    string `json:"phone,omitempty"`
	Info     string `json:"info,omitempty"`
	Gate     string `json:"gate,omitempty"`
	Deadline string `json:"deadline,omitempty"` // For cash payments (invoice gate)

	Invoices *Invoices `json:"invoices,omitempty"` // Mandatory if enabled in the terminal settings
}

// Invoice represents a single item in the payment
type Invoice struct {
	Name       string `json:"name"`
	Category   string `json:"category"`
	Quantity   int    `json:"quantity"`
	Price      string `json:"price"`
	VatPercent string `json:"vat_percent"`
}

// Invoices wraps the invoices array
type Invoices struct {
	Invoices []Invoice `json:"invoices"`
}


// ------------------------------ Tokenization ------------------------------


// TokenizationRequest represents the tokenization request structure
type TokenizationRequest struct {
	Data TokenizationData `json:"data"`
}

// TokenizationData contains the actual tokenization parameters
type TokenizationData struct {
	OrderID     string `json:"orderId"`
	Key         string `json:"key"`
	Token       string `json:"token"`
	CallbackURL string `json:"callbackURL"`
	ReturnURL   string `json:"returnURL"`
	Phone       string `json:"phone"`
	Gate        string `json:"gate"`
	ClientID    string `json:"clientID"`
}


// ------------------------------ Marketplace payment ------------------------------


// MarketplacePaymentRequest represents a marketplace payment request with split payments
type MarketplacePaymentRequest struct {
	Key            string           `json:"key"`
	Token          string           `json:"token"`
	OrderID        string           `json:"order_id"`
	Amount         string           `json:"amount"`
	CallbackURL    string           `json:"callback_url"`
	ReturnURL      string           `json:"return_url"`
	Email          string           `json:"email,omitempty"`
	Phone          string           `json:"phone,omitempty"`
	Info           string           `json:"info,omitempty"`
	MpTerminalInfo []MpTerminalInfo `json:"mpTerminalInfo"`
}

// MpTerminalInfo represents payment split information for a partner terminal
type MpTerminalInfo struct {
	TerminalID  string               `json:"terminal_id"`
	Amount      string               `json:"amount"`
	ConditionID int                  `json:"condition_id,omitempty"`
	Invoices    []MarketplaceInvoice `json:"invoices"`
}

// MarketplaceInvoice represents a single item in marketplace payment
type MarketplaceInvoice struct {
	Name     string `json:"name"`
	Category string `json:"category"`
	Quantity int    `json:"quantity"`
	Price    string `json:"price"`
}

// ConfirmDeliveryRequest represents delivery confirmation request
type ConfirmDeliveryRequest struct {
	Key           string `json:"key"`
	TransactionID string `json:"transaction_id"`
	Token         string `json:"token"`
	Amount        string `json:"amount"`
}

// ConfirmVsaMcrDeliveryRequest represents delivery confirmation for Visa/Mastercard
type ConfirmVsaMcrDeliveryRequest struct {
	Key                 string                      `json:"key"`
	ParentTransactionID string                      `json:"parent_transaction_id"`
	Token               string                      `json:"token"`
	Extra               []VsaMcrDeliveryTransaction `json:"extra"`
}

// VsaMcrDeliveryTransaction represents a single transaction to confirm
type VsaMcrDeliveryTransaction struct {
	TransactionID string `json:"transaction_id"`
	Amount        string `json:"amount"`
}


// ------------------------------ Common ------------------------------


// Response - common response for all requests
type Response struct {
	Code    int    `json:"code"`
	Message string `json:"message"`
	URL     string `json:"url"`
}

// APIError represents an error from the API
type APIError struct {
	StatusCode int
	Code       int
	Message    string
}

func (e *APIError) Error() string {
	return fmt.Sprintf("API error (HTTP %d, code %d): %s", e.StatusCode, e.Code, e.Message)
}
