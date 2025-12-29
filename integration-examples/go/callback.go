package main

import (
	"encoding/json"
	"io"
	"log"
	"net/http"
)


// ------------------------------ Payment callback ------------------------------


const (
	StatusOK        = "ok"
	StatusFailed    = "failed"
	StatusCanceled  = "canceled"
	StatusPending   = "pending"
	StatusToApprove = "to_approve"
)

// CallbackData представляет уведомление о платеже
type CallbackData struct {
	OrderID         string  `json:"orderId"`
	TransactionID   string  `json:"transactionId"`
	Status          string  `json:"status"`
	Token           string  `json:"token"`
	Amount          float64 `json:"amount"`
	Account         string  `json:"account"`
	Phone           string  `json:"phone"`
	TransactionType string  `json:"transaction_type"`
}

// HandlePaymentCallback - example of payment callback handling
func HandlePaymentCallback(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Method not allowed", http.StatusMethodNotAllowed)
		return
	}

	body, err := io.ReadAll(r.Body)
	if err != nil {
		http.Error(w, "Bad request", http.StatusBadRequest)
		return
	}
	defer r.Body.Close()

	var data CallbackData
	if err := json.Unmarshal(body, &data); err != nil {
		http.Error(w, "Bad request", http.StatusBadRequest)
		return
	}

	switch data.Status {
	case StatusOK:
		log.Printf("Payment successful: order=%s amount=%.2f", data.OrderID, data.Amount)
	case StatusFailed, StatusCanceled:
		log.Printf("Payment %s: order=%s amount=%.2f", data.Status, data.OrderID, data.Amount)
	case StatusPending, StatusToApprove:
		log.Printf("Payment in progress: order=%s status=%s", data.OrderID, data.Status)
	default:
		log.Printf("Unknown payment status: %s for order=%s", data.Status, data.OrderID)
	}

	w.WriteHeader(http.StatusOK)
	w.Write([]byte("OK"))
}


// ------------------------------ Tokenization callback ------------------------------


const (
	TokenizationCodeSuccess   = 1
	TokenizationCodeDuplicate = 2
	TokenizationCodeRejected  = 9
)

type TokenizationCallbackData struct {
	Code       int                         `json:"code"`
	Message    string                      `json:"message"`
	ReasonCode string                      `json:"reason_code"`
	Payload    TokenizationCallbackPayload `json:"payload"`
}

type TokenizationCallbackPayload struct {
	TransactionID   int    `json:"transactionId"`
	OrderID         string `json:"orderId"`
	Token           string `json:"token"`
	Account         string `json:"account"`
	Status          string `json:"status"`
	TransactionType string `json:"transaction_type"`
}

// HandleTokenizationCallback - example of tokenization callback handling
func HandleTokenizationCallback(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Method not allowed", http.StatusMethodNotAllowed)
		return
	}

	body, err := io.ReadAll(r.Body)
	if err != nil {
		http.Error(w, "Bad request", http.StatusBadRequest)
		return
	}
	defer r.Body.Close()

	var data TokenizationCallbackData
	if err := json.Unmarshal(body, &data); err != nil {
		http.Error(w, "Bad request", http.StatusBadRequest)
		return
	}

	switch data.Code {
	case TokenizationCodeSuccess:
		log.Printf("Tokenization successful: order=%s token=%s", data.Payload.OrderID, data.Payload.Token)
	case TokenizationCodeDuplicate:
		log.Printf("Tokenization duplicate: order=%s", data.Payload.OrderID)
	case TokenizationCodeRejected:
		log.Printf("Tokenization failed: order=%s reason=%s", data.Payload.OrderID, data.ReasonCode)
	default:
		log.Printf("Unknown tokenization code: %d for order=%s", data.Code, data.Payload.OrderID)
	}

	w.WriteHeader(http.StatusOK)
	w.Write([]byte("OK"))
}


// ------------------------------ Marketplace callback ------------------------------


// MarketplaceCallbackData represents the marketplace payment callback notification
type MarketplaceCallbackData struct {
	OrderID         string           `json:"orderId"`
	TransactionID   string           `json:"transactionId"`
	Status          string           `json:"status"`
	Token           string           `json:"token"`
	Amount          float64          `json:"amount"`
	Account         string           `json:"account"`
	Phone           string           `json:"phone"`
	TransactionType string           `json:"transaction_type"`
	SubTransactions []SubTransaction `json:"sub_transactions"`
}

// SubTransaction represents a sub-transaction for a partner terminal
type SubTransaction struct {
	TerminalID    string `json:"terminal_id"`
	TransactionID string `json:"transaction_id"`
	Status        string `json:"status"`
}

// HandleMarketplaceCallback - example of marketplace callback handling
func HandleMarketplaceCallback(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Method not allowed", http.StatusMethodNotAllowed)
		return
	}

	body, err := io.ReadAll(r.Body)
	if err != nil {
		http.Error(w, "Bad request", http.StatusBadRequest)
		return
	}
	defer r.Body.Close()

	var data MarketplaceCallbackData
	if err := json.Unmarshal(body, &data); err != nil {
		http.Error(w, "Bad request", http.StatusBadRequest)
		return
	}

	switch data.Status {
	case StatusOK:
		log.Printf("Marketplace payment successful: order=%s amount=%.2f sub_transactions=%d",
			data.OrderID, data.Amount, len(data.SubTransactions))
	case StatusFailed, StatusCanceled:
		log.Printf("Marketplace payment %s: order=%s", data.Status, data.OrderID)
	case StatusPending, StatusToApprove:
		log.Printf("Marketplace payment in progress: order=%s status=%s", data.OrderID, data.Status)
	default:
		log.Printf("Unknown marketplace payment status: %s for order=%s", data.Status, data.OrderID)
	}

	w.WriteHeader(http.StatusOK)
	w.Write([]byte("OK"))
}
