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
	exit();
}

//storing the current file path in Session Variable
$_SESSION['location'] = $_SERVER['REQUEST_URI'];

//check whether cookies are enabled or not
//if(!isset($_COOKIE['testcookie'])){
//	header('Location: test_cookies.php');
//	exit();
//}

// check if form submitted
if (isset ( $_POST ['login'] )) {
	$email = mysqli_real_escape_string ( $conn, $_POST ['email'] );
	$password = mysqli_real_escape_string ( $conn, $_POST ['password'] );
	
	$query = "SELECT * FROM customer WHERE Email = '" . $email . "' AND Password = '" . md5 ( $password ) . "' ";
	$result = mysqli_query ( $conn, $query );
	
	if (! $result) {
		echo ("Unable to execute query: " . mysqli_error ( $conn ));
		exit();
	} else {
		if ($row = mysqli_fetch_array ( $result, MYSQLI_ASSOC )) {
			$_SESSION ['userId'] = $row ['CustomerID'];
			$_SESSION ['username'] = $row ['Email'];
			
			if($row['Email'] == "msg@gmail.com"){
				header ( "Location: home_manager.php" );
			}
			else{
				header ( "Location: home.php" );
			}
			exit();
		} else {
			$error = "Incorrect Email or Password!";
		}
	}
}

?>

<!DOCTYPE html>
<html>
<head>

<!-- Redirect to test_javaScript.php if javaScript is not enabled -->
<!--<noscript> <meta http-equiv="refresh" content="0;url=test_javascript.php"> </noscript>-->

<meta charset="ISO-8859-1">
<title>Login</title>

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
				<h4 class="text-primary">New User?</h4>
				<a class="btn btn-success btn-text" href="register.php">Register Here</a> 
				<br /> <br /> <br />
				<!--<h4 class="text-primary">Reservations</h4> 
				<a class="btn btn-success btn-text" href="all_reservations.php">Show All Reservations</a>
				-->
			</div>
			<div class="col-md-10">
				<div class="col-md-3"></div>
					<div class="col-md-4">
					<br />
					<h1 class="text-primary">Login!</h1>
					<br /> <br />		
					<form name="loginform" action="<?php echo $_SERVER['PHP_SELF']; ?>" method="post">
						<div class="form-group">
							<input type="email" class="form-control" name="email" id="email" placeholder="Email" required="required" />
							<br /> <br />
							<input type="password" class="form-control" name="password" id="password" placeholder="Password" required="required" />
							<br /> <br /> <br />
							<input type="submit" class="btn btn-success btn-text" name="login" value="Login">
						</div>
					</form>
					<br /> <br />
					<span class="h3 text-danger">
						<?php  
							if(isset($error))
								echo $error;
						?>
					</span>
					<br /> <br /> <br /> <br />
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