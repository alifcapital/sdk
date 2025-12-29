<?php

namespace AlifPaymentSDK;

class APIException extends \Exception
{
    private int $statusCode;
    private int $code;

    public function __construct(int $statusCode, int $code, string $message)
    {
        parent::__construct(sprintf('API error (HTTP %d, code %d): %s', $statusCode, $code, $message));
        $this->statusCode = $statusCode;
        $this->code = $code;
    }

    public function getStatusCode(): int
    {
        return $this->statusCode;
    }

    public function getCode(): int
    {
        return $this->code;
    }
}

