<?php
session_start();
include 'dbconnect.php';

//if user is already logged in, redirect to home.php
if (isset ( $_SESSION ['userId'] ) != "") {
	header ( "Location: home.php" );
	exit();
}

// if HTTPS is off, redirect to same page with HTTPS enabled
if (! isset ( $_SERVER ['HTTPS'] ) || $_SERVER ['HTTPS'] != "on") {
	header ( "Location: https://" . $_SERVER ['HTTP_HOST'] . $_SERVER ['REQUEST_URI'] );
	exit ();
}

//storing the current file path in Session Variable
$_SESSION['location'] = $_SERVER['REQUEST_URI'];

//check whether cookies are enabled or not
//if(!isset($_COOKIE['testcookie'])){
//	header('Location: test_cookies.php');
//	exit();
//}


// set validation error flag as false
$error = false;

// check if form submitted
if (isset ( $_POST ['signup'] )) {
	$firstName = mysqli_real_escape_string ( $conn, $_POST ['firstName'] );
	$lastName = mysqli_real_escape_string ( $conn, $_POST ['lastName'] );
	$barcode = mysqli_real_escape_string ( $conn, $_POST ['barcode'] );
	$email = mysqli_real_escape_string ( $conn, $_POST ['email'] );
	$password = mysqli_real_escape_string ( $conn, $_POST ['password'] );
	$confirmPassword = mysqli_real_escape_string ( $conn, $_POST ['confirmPassword'] );
	
	if (! preg_match ( "/^[a-zA-Z0-9]+$/", $firstName )) {
		$error = true;
		$firstNameError = "First Name must contain alphabets and/or digits only";
	}
	if (! preg_match ( "/^[a-zA-Z0-9]+$/", $lastName )) {
		$error = true;
		$lastNameError = "Last Name must contain alphabets and/or digits only";
	}
	if (! preg_match ( "/^[0-9]{9}+$/", $barcode )) {
		$error = true;
		$barcodeError = "Barcode must be a 9 digit number";
	}
	if (! preg_match ( "/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/", $email )) {
		$error = true;
		$emailError = "Email address must follow the standard rules";
	}
	if (! preg_match ( "/^[A-Za-z0-9]+$/", $password )) {
		$error = true;
		$passwordError = "Password must contain alphabets and digits only";
	}
	if ($password != $confirmPassword) {
		$error = true;
		$confirmPasswordError = "Password and Confirm Password does not match";
	}
	
	if (! $error) {
		$query = "SELECT * FROM customer WHERE Email = '" . $email . "'";
		$result = mysqli_query ( $conn, $query );
		if (! $result) {
			echo ("Unable to execute query: " . mysqli_error ( $conn ));
			exit();
		} else {
			if (! $row = mysqli_fetch_array ( $result )) {
				$insert_query = "INSERT INTO customer(Barcode, FirstName, LastName, Email, Password) VALUES('" . $barcode . "', '" . $firstName . "', '" . $lastName . "', '" . $email . "', '" . md5($password) . "')";
				$result = mysqli_query ( $conn, $insert_query );
				if (! $result) {
					$failure = "Error in Registering --- Please try again later!";
				} else {
					$success = "Congratulations! You have registered successfully. You can login now!";
				}
			} else
				$failure = "This email address is already registered!";
		}
	}
}
?>

<!DOCTYPE html>
<html>
<head>

<!-- Redirect to test_javaScript.php if javaScript is not enabled -->
<!--<noscript> <meta http-equiv="refresh" content="0;url=test_javascript.php"> </noscript>
-->

<meta charset="ISO-8859-1">
<title>Registration</title>

<!-------------------------- BootStrap Files ---------------------->
<link rel="stylesheet" type="text/css" href="css/bootstrap.min.css">
<link rel="stylesheet" type="text/css" href="css/style.css">
<script src="js/jquery.js"></script>
<script src="js/footer.js"></script>
<script src="js/bootstrap.min.js"></script>
<!-------------------------- BootStrap Files ---------------------->

</head>
<body class="times-new-roman">
	<div class="container-fluid">
		<div class="row rm bg-primary">
			<h1><a class="header" href="index.php">electronics.com</a></h1>
		</div>
		<div class="row rm">
			<div class="col-md-2">
				<br /> <br /> <br />
				<h4 class="text-primary">Already Registered?</h4>
				<a class="btn btn-success btn-text" href="login.php">Login Here</a> 
				<br /> <br /> <br />
				<!--<h4 class="text-primary">Reservations</h4> 
				<a class="btn btn-success btn-text" href="all_reservations.php">Show All Reservations</a>
				-->
			</div>
			<div class="col-md-10">
				<div class="col-md-3"></div>
					<div class="col-md-4">
				<br />
					<h1 class="text-primary">Sign Up!</h1>
					<br /> <br />
					<form name="signupform" action="<?php echo $_SERVER['PHP_SELF']; ?>" method="post">
						<div class="form-group">
							<input type="text" class="form-control" name="firstName" id="firstName" placeholder="First Name" required="required" value="<?php if($error) echo stripslashes($firstName); ?>" />
							<span class="text-danger"><?php if(isset($firstNameError)) echo $firstNameError;  ?></span>
							<br /> <br />
							<input type="text" class="form-control" name="lastName" id="lastName" placeholder="Last Name" required="required" value="<?php if($error) echo stripslashes($lastName); ?>" />
							<span class="text-danger"><?php if(isset($lastNameError)) echo $lastNameError; ?></span>
							<br /> <br />
							<input type="text" class="form-control" name="barcode" id="barcode" placeholder="Barcode" required="required" value="<?php if($error) echo stripslashes($barcode); ?>" />
							<span class="text-danger"><?php if(isset($barcodeError)) echo $barcodeError; ?></span>
							<br /> <br />
							<input type="email" class="form-control" name="email" id="email" placeholder="Email" required="required" value="<?php if($error) echo stripslashes($email); ?>" />
							<span class="text-danger"><?php if(isset($emailError)) echo $emailError; ?></span>
							<br /> <br />
							<input type="password" class="form-control" name="password" id="password" placeholder="Password" required="required" value="<?php if($error) echo stripslashes($password); ?>"/>
							<span class="text-danger"><?php if(isset($passwordError)) echo $passwordError; ?></span>
							<br /> <br />
							<input type="password" class="form-control" name="confirmPassword" id="confirmPassword" placeholder="Confirm Password" required="required" value="<?php if($error) echo stripslashes($confirmPassword); ?>" />
							<span class="text-danger"><?php if(isset($confirmPasswordError)) echo $confirmPasswordError; ?></span>
							<br /> <br /> <br />
							<input type="submit" class="btn btn-success btn-text" name="signup" value="Register">
							<br /> <br />
						</div>
					</form>
					<?php  
						if(isset($success))
							echo "<script type='text/javascript'> alert('$success') </script>";
						elseif(isset($failure))
							echo "<script type='text/javascript'> alert('$failure') </script>";
					?>
					</div>
					<div class="col-md-3"></div>
			</div>
		</div>
		<div class="row rm">
			<div class="footer bg-primary" id="footer">
				<br />
				<p>Copyrights @ All rights are reserved by MSG Team, 2017</p>
			</div>
		</div>	
	</div>
</body>
</html>