<?php
    require_once("alif.php");
    $a = new Alif('keycode','passcode');

    $jsn = json_decode(file_get_contents("php://input"), false);

/*
{
  "orderId": "2133213",
  "transactionId": "2231223",
  "status": "ok",
  "token": "lasjdlkasjdlajdoi1321i3921391203",
  "amount": 12.00,
  "phone": "+992921223100"
}
*/
   
    if(isset($jsn)){

     // $jsn->phone
     // $jsn->amount

        $a->orderId = $jsn->orderId;
        $a->status = $jsn->status;
        $a->transactionId = $jsn->transactionId;
        $token = $a->callback();
        
        if ($token === $jsn->token){
            if ($jsn->status === "ok"){
                //update datebase for success payment
                echo "Success";
            }else{
                // update database for failed payment
                echo "Failed";
            }            
        }
    }
    echo "No json";
?>