package main

import (
	"crypto/hmac"
	"crypto/sha256"
	"encoding/hex"
)

// generateToken generates HMAC SHA256 token for API authentication.
// According to documentation, the password must be pre-hashed with the terminal key.
// Token generation formula: HMAC_SHA256(key + order_id + amount + callback_url, HMAC_SHA256(password, key))
func (c *Client) generateToken(dataToSign string) string {
	// Step 1: Pre-hash the password with terminal ID (key)
	hashedPassword := generateHMAC(c.terminalPassword, c.terminalID)

	// Step 2: Generate final token using the hashed password
	token := generateHMAC(dataToSign, hashedPassword)

	return token
}

// generateHMAC generates HMAC-SHA256 hash and returns it as a hex string
func generateHMAC(data string, secret string) string {
	h := hmac.New(sha256.New, []byte(secret))
	h.Write([]byte(data))

	return hex.EncodeToString(h.Sum(nil))
}
