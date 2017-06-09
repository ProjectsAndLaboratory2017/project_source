<?php

session_start();

if(isset($_SESSION['userId'])){
	session_destroy();
	unset($_SESSION['userId']);
	unset($_SESSION['username']);
	unset($_SESSION['lastActivity']);
	header("Location: index.php");
	exit();
}
else{
	header("Location: index.php");
	exit();
}
?>