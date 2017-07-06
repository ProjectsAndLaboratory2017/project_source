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

// check if form submitted
if (isset ( $_POST ['signup'] )) {
	$firstName = mysqli_real_escape_string ( $conn, $_POST ['firstName'] );
	$lastName = mysqli_real_escape_string ( $conn, $_POST ['lastName'] );
	$email = mysqli_real_escape_string ( $conn, $_POST ['email'] );
	$password = mysqli_real_escape_string ( $conn, $_POST ['password'] );
	$confirmPassword = mysqli_real_escape_string ( $conn, $_POST ['confirmPassword'] );
	
	$query = "SELECT * FROM customer WHERE Email = '" . $email . "'";
	$result = mysqli_query ( $conn, $query );
	if (! $result) {
		echo ("Unable to execute query: " . mysqli_error ( $conn ));
		exit();
	} else {
		if (! $row = mysqli_fetch_array ( $result )) {
			$insert_query = "INSERT INTO customer(FirstName, LastName, Email, Password) VALUES('" . $firstName . "', '" . $lastName . "', '" . $email . "', '" . md5($password) . "')";
			$result = mysqli_query ( $conn, $insert_query );
			if (! $result) {
				$failure = "Error in Registering --- Please try again later!";
			} else {
				$success = "Congratulations! You have registered successfully. You can login now!";
			}
		} else
			$failure = "This email address is already registered! Please choose another email address";
	}
}
?>

<script type='text/javascript'> 
	function checkName(input) {  
		if(input.validity.patternMismatch)
			input.setCustomValidity("Name must contain alphabets, '-' or '_' symbols");    
		else 
			input.setCustomValidity("");  
	}
	
	function checkPassword(input) {  
		if(input.validity.patternMismatch)
			input.setCustomValidity("Password must be atleast 6 characters long and must include lowercase letter, uppercase letter, digit and atleast one special symbol from '@#$%' ");    
		else 
			input.setCustomValidity("");  
	}	
	
	var password = document.getElementById("password");
	var confirmPassword = document.getElementById("confirmPassword");

	function checkConfirmPassword(){
		if(password.value != confirmPassword.value) {
			confirmPassword.setCustomValidity("Passwords Don't Match");
		} else {
			confirmPassword.setCustomValidity('');
		}
	}	
	
 </script>


<!DOCTYPE html>
<html>
<head>

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
			<h1><a class="header" href="index.php">Buy&Buy.com</a></h1>
		</div>
		<div class="row rm">
			<div class="col-md-2">
				<br /> <br /> <br />
				<h4 class="text-primary">Already Registered?</h4>
				<a class="btn btn-success btn-text" href="login.php">Login Here</a> 
				<br /> <br /> <br />
			</div>
			<div class="col-md-10">
				<div class="col-md-3"></div>
					<div class="col-md-4">
				<br />
					<h1 class="text-primary">Sign Up!</h1>
					<br /> <br />
					<form name="signupform" action="<?php echo $_SERVER['PHP_SELF']; ?>" method="post">
						<div class="form-group">
							
							<input type="text" class="form-control" name="firstName" id="firstName" placeholder="First Name" required="required" pattern="[-_a-zA-Z ]+" oninput="checkName(this)" />
							<br /> <br />
							
							<input type="text" class="form-control" name="lastName" id="lastName" placeholder="Last Name" required="required" pattern="[-_a-zA-Z ]+" oninput="checkName(this)" />
							<br /> <br />
							
							<input type="email" class="form-control" name="email" id="email" placeholder="Email" required="required" />
							<br /> <br />
							
							<input type="password" class="form-control" name="password" id="password" placeholder="Password" required="required" pattern="((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%]).{6,20})" oninput="checkPassword(this)" />
							<br /> <br />
							
							<input type="password" class="form-control" name="confirmPassword" id="confirmPassword" placeholder="Confirm Password" required="required" onkeyup="checkConfirmPassword()" />
							<br /> <br /> <br />
							
							<input type="submit" class="btn btn-success btn-text" name="signup" value="Register" />
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