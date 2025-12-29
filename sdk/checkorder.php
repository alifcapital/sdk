<?php

    require_once('alif.php');
    $a = new Alif('keycode','passcode');
    $a->orderid = '321123'; //orderid
    $token = $a->checkOrderToken();

    // The data to send to the API
$postData = array(
    'orderId' => $a->orderid,
    'key' => $a->key,
    'token' => $token
);

// Setup cURL
$ch = curl_init('https://alifpay.tj/web/checktxn');
curl_setopt_array($ch, array(
    CURLOPT_RETURNTRANSFER => TRUE,
    CURLOPT_HTTPHEADER => array('Content-Type: application/json'),
    CURLOPT_POSTFIELDS => json_encode($postData)
));

// Send the request
$response = curl_exec($ch);

// Check for errors
if($response === FALSE){
    die(curl_error($ch));
}
/**
 * 
 success response
 {
    "orderId": "12345678",
    "transactionId": "92938922",
    "status": "ok",
    "token": "3792cujidwjkdj981uj918dj19djdad32d23f2f",
    "amount": 10,
    "phone": "+992931234455"
} 


error response

{
"orderId": "12345678",
"status": "not found",
} 

or

{
"orderId": "12345678",
"status": "failed",
} 
 */
// Decode the response
$responseData = json_decode($response);

// Print the date from the response
echo $responseData['status'];
?>