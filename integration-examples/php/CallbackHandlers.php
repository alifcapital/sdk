<?php

namespace AlifPaymentSDK;


// ------------------------------ Payment callback ------------------------------


class PaymentStatus
{
    public const OK = 'ok';
    public const FAILED = 'failed';
    public const CANCELED = 'canceled';
    public const PENDING = 'pending';
    public const TO_APPROVE = 'to_approve';
}

class PaymentCallbackHandler
{
    /**
     * Example of payment callback handling
     */
    public static function handlePaymentCallback(): void
    {
        if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
            http_response_code(405);
            echo 'Method not allowed';
            return;
        }

        $body = file_get_contents('php://input');
        $data = json_decode($body, true);

        switch ($data['status']) {
            case PaymentStatus::OK:
                error_log(sprintf('Payment successful: order=%s amount=%.2f', $data['orderId'], $data['amount']));
                break;
            case PaymentStatus::FAILED:
            case PaymentStatus::CANCELED:
                error_log(sprintf('Payment %s: order=%s amount=%.2f', $data['status'], $data['orderId'], $data['amount']));
                break;
            case PaymentStatus::PENDING:
            case PaymentStatus::TO_APPROVE:
                error_log(sprintf('Payment in progress: order=%s status=%s', $data['orderId'], $data['status']));
                break;
            default:
                error_log(sprintf('Unknown payment status: %s for order=%s', $data['status'], $data['orderId']));
                break;
        }

        http_response_code(200);
        echo 'OK';
    }
}


// ------------------------------ Tokenization callback ------------------------------


class TokenizationCode
{
    public const SUCCESS = 1;
    public const DUPLICATE = 2;
    public const REJECTED = 9;
}

class TokenizationCallbackHandler
{
    /**
     * Example of tokenization callback handling
     */
    public static function handleTokenizationCallback(): void
    {
        if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
            http_response_code(405);
            echo 'Method not allowed';
            return;
        }

        $body = file_get_contents('php://input');
        $data = json_decode($body, true);

        switch ($data['code']) {
            case TokenizationCode::SUCCESS:
                error_log(sprintf('Tokenization successful: order=%s token=%s', $data['payload']['orderId'], $data['payload']['token']));
                break;
            case TokenizationCode::DUPLICATE:
                error_log(sprintf('Tokenization duplicate: order=%s', $data['payload']['orderId']));
                break;
            case TokenizationCode::REJECTED:
                error_log(sprintf('Tokenization failed: order=%s reason=%s', $data['payload']['orderId'], $data['reason_code']));
                break;
            default:
                error_log(sprintf('Unknown tokenization code: %d for order=%s', $data['code'], $data['payload']['orderId']));
                break;
        }

        http_response_code(200);
        echo 'OK';
    }
}


// ------------------------------ Marketplace callback ------------------------------


class MarketplaceCallbackHandler
{
    /**
     * Example of marketplace callback handling
     */
    public static function handleMarketplaceCallback(): void
    {
        if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
            http_response_code(405);
            echo 'Method not allowed';
            return;
        }

        $body = file_get_contents('php://input');
        $data = json_decode($body, true);

        switch ($data['status']) {
            case PaymentStatus::OK:
                error_log(sprintf('Marketplace payment successful: order=%s amount=%.2f sub_transactions=%d',
                    $data['orderId'], $data['amount'], count($data['sub_transactions'])));
                break;
            case PaymentStatus::FAILED:
            case PaymentStatus::CANCELED:
                error_log(sprintf('Marketplace payment %s: order=%s', $data['status'], $data['orderId']));
                break;
            case PaymentStatus::PENDING:
            case PaymentStatus::TO_APPROVE:
                error_log(sprintf('Marketplace payment in progress: order=%s status=%s', $data['orderId'], $data['status']));
                break;
            default:
                error_log(sprintf('Unknown marketplace payment status: %s for order=%s', $data['status'], $data['orderId']));
                break;
        }

        http_response_code(200);
        echo 'OK';
    }
}

