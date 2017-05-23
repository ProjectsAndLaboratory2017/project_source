<!-- dbconnect.php creates connection with the database -->

<?php

$host = "localhost";
$username = "root";
$password = "";
$database = "plcs";

// Create connection
$conn = mysqli_connect($host, $username, $password, $database);

//Check connection
if(!$conn){
	die("Connection failed: " . mysqli_connect_error() . " " . mysqli_connect_errno());
}

?>