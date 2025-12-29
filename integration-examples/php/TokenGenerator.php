<?php

namespace AlifPaymentSDK;

class TokenGenerator
{
    /**
     * Generates HMAC SHA256 token for API authentication.
     * According to documentation, the password must be pre-hashed with the terminal key.
     * Token generation formula: HMAC_SHA256(dataToSign, HMAC_SHA256(password, key))
     */
    public static function generateToken(string $dataToSign, string $terminalPassword, string $terminalID): string
    {
        // Step 1: Pre-hash the password with terminal ID (key)
        $hashedPassword = self::generateHMAC($terminalPassword, $terminalID);

        // Step 2: Generate final token using the hashed password
        $token = self::generateHMAC($dataToSign, $hashedPassword);

        return $token;
    }

    /**
     * Generates HMAC-SHA256 hash and returns it as a hex string
     */
    private static function generateHMAC(string $data, string $secret): string
    {
        return hash_hmac('sha256', $data, $secret);
    }
}

