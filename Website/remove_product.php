<?php
session_start();
include 'dbconnect.php';
include 'time_elapse.php';

// if HTTPS is off, redirect to same page with HTTPS enabled
if (! isset ( $_SERVER ['HTTPS'] ) || $_SERVER ['HTTPS'] != "on") {
	header ( "Location: https://" . $_SERVER ['HTTP_HOST'] . $_SERVER ['REQUEST_URI'] );
	exit ();
}

//if user is not logged in, redirect to index.php
if (! isset ( $_SESSION ['userId'] )) {
	session_destroy ();
	header ( "Location: index.php" );
	exit();
}

// check if form submitted
if (isset ( $_POST ['removeproduct'] )) {
	$barcode = mysqli_real_escape_string ( $conn, $_POST ['barcode'] );

	$query = "SELECT * FROM product WHERE Barcode = '" . $barcode . "'";
	$result = mysqli_query ( $conn, $query );
		
	if (! $result) {
		echo ("Unable to execute query: " . mysqli_error ( $conn ));
		exit();
	} else {
		if ($row = mysqli_fetch_array ( $result )) {
			$delete_query = "DELETE FROM product WHERE Barcode = '" . $barcode . "'";
			$result = mysqli_query ( $conn, $delete_query );
			if (! $result) {
				$failure = "Error in Removing --- Please try again later!";
			} else {
				$success = "Product is removed successfully!";
			}
		} else
			$failure = "Product with the entered barcode does not exist!";
	}
}

?>

<!DOCTYPE html>
<html>
<head>

<meta charset="ISO-8859-1">
<title>Home</title>

<!-------------------------- BootStrap Files ---------------------->
<link rel="stylesheet" type="text/css" href="css/bootstrap.min.css" />
<link rel="stylesheet" type="text/css" href="css/style.css" />
<script src="js/jquery.js"></script>
<script src="js/footer.js"></script>
<script src="js/bootstrap.min.js"></script>
<!-------------------------- BootStrap Files ---------------------->

</head>
<body class="times-new-roman">
	<div class="container-fluid">
		<div class="row rm bg-primary">
			<div class="col-md-8">
				<h1><a class="header" href="index.php">Buy&Buy.com</a></h1>
			</div>
		<div class="col-md-4">
			<span class="login-text pull-right"> 
			<?php if (isset ( $_SESSION ['userId'] ) != "") { ?>
					Logged in as <?php echo $_SESSION ['username'] . " | "; ?>
					<a class="login-text" href='logout.php'>Logout</a>
			<?php } ?>
			</span>
		</div>
		</div>
		<div class="row rm">
			<div class="col-md-4">
				<br /> <br /> <br />
				<a class="btn btn-success btn-text" href="home_manager.php">Back to Home</a> 
				<br /> <br />
			</div>
			<div class="col-md-4">	
				<h1 class="text-primary">Remove Product</h1>
					<br /> <br />
					<form name="removeproductform" action="<?php echo $_SERVER['PHP_SELF']; ?>" method="post">
						<div class="form-group">
							<label>Enter the barcode of the product to be removed</label><br />
							<input type="text" class="form-control" name="barcode" id="barcode" placeholder="Barcode" required="required" />
							<br /> <br />
							
							<input type="submit" class="btn btn-success btn-text" name="removeproduct" value="Remove">
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
			<div class="col-md-4"></div>
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