<?php

namespace AlifPaymentSDK;

class APIException extends \Exception
{
    private int $statusCode;
    private int $apiCode;

    public function __construct(int $statusCode, int $code, string $message)
    {
        parent::__construct(sprintf('API error (HTTP %d, code %d): %s', $statusCode, $code, $message), $code);
        $this->statusCode = $statusCode;
        $this->apiCode = $code;
    }

    public function getStatusCode(): int
    {
        return $this->statusCode;
    }

    public function getApiCode(): int
    {
        return $this->apiCode;
    }
}

