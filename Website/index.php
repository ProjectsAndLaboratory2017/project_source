<?php
session_start();
include 'dbconnect.php';

//if user is already logged in, redirect to home.php
if (isset ( $_SESSION ['userId'] ) != "") {
	header ( "Location: home.php" );
	exit();
}

//storing the current file path in Session Variable
$_SESSION['location'] = $_SERVER['REQUEST_URI']; 

?>

<!DOCTYPE html>
<html>
<head>

<meta charset="ISO-8859-1">
<meta http-equiv="X-UA-Compatible" content="IE=edge">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>Welcome to buy & buy</title>

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
			<div id="myCarousel" class="carousel slide" data-ride="carousel">
				<!-- Indicators -->
				<ol class="carousel-indicators">
					<li data-target="#myCarousel" data-slide-to="0" class="active"></li>
					<li data-target="#myCarousel" data-slide-to="1"></li>
					<li data-target="#myCarousel" data-slide-to="2"></li>
				</ol>

				<!-- Wrapper for slides -->
				<div class="carousel-inner" role="listbox">
					<div class="item active fezImg">
						<img src="images/prod1.png" alt="Image 1">
					</div>

					<div class="item fezImg">
						<img src="images/prod2.png" alt="Image 2">
					</div>

					<div class="item fezImg">
						<img src="images/prod3.png" alt="Image 3">
					</div>
				</div>

				<!-- Left and right controls -->
				<a class="left carousel-control" href="#myCarousel" role="button"
					data-slide="prev"> <span class="glyphicon glyphicon-chevron-left"
					aria-hidden="true"></span> <span class="sr-only">Previous</span>
				</a> <a class="right carousel-control" href="#myCarousel"
					role="button" data-slide="next"> <span
					class="glyphicon glyphicon-chevron-right" aria-hidden="true"></span>
					<span class="sr-only">Next</span>
				</a>
			</div>
		</div>
		<br /> <br />
		<div class="row rm">
			<div class="col-md-6 text-center">
				<h2 class="text-primary">New User?</h2> <br /> 
  				<a class="btn btn-success btn-text" href="register.php">Register Here</a> 
				<br /> <br /> <br />
			</div>
			<div class="col-md-6 text-center">
				<h2 class="text-primary">Already Registered?</h2> <br />
				<a class="btn btn-success btn-text" href="login.php">Login Here</a> 
				<br /> <br /> <br />
			</div>
		</div>

		<div class="row rm">
			<div class=" footer bg-primary" id="footer">
				<br />
				<p>Copyrights @ All rights are reserved by MSG Team, 2017</p>
			</div>
		</div>
	</div>
</body>
</html>