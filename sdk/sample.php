<?php
 require_once('alif.php');
  $a = new Alif('keycode','passcode');
  $a->amount = 2.99;
  $a->orderid = '321123'; //from merchant shop
  $a->callbackUrl = 'http://myshop.tj/thank_you.php';
  $a->returnUrl = 'http://myshop.tj';
  $a->info = 'Xiaomi Mi Mix 2S 6/64 Gb';
  $a->email = 'testUsers@gmail.com';//optional
  $a->phone = '988888888';//optional
  $token = $a->token();

?>
 <form name="AlifPayForm" action="https://alifpay.tj/web" method="post" id="alifPayForm">
    <input type="hidden" name="token" id="token" value="<?php echo  $token;?>">
    <input type="hidden" name="key" id="key" value="<?php echo  $a->key;?>">
    <input type="hidden" name="callbackUrl" id="callbackUrl" value="<?php echo $a->callbackUrl;?>">
    <!-- callback url where alif sends information about status of transactions -->
    <input type="hidden" name="returnUrl" id="returnUrl" value="<?php echo $a->returnUrl;?>">
    <input type="hidden" name="amount" id="amount" value="<?php echo $a->amount;?>" required>
    <input type="hidden" name="orderId" id="orderId" value="<?php echo $a->orderid;?>">

    <input type="hidden" name="info" id="info" value="<?php echo $a->info;?>">
    
    <input type="hidden" name="email" id="email" value="<?php echo $a->email;?>">
    <input type="hidden" name="phone" id="phone" value="<?php echo $a->phone;?>">
    <input type="submit" value="Пардохт бо корти милли">
</form>
