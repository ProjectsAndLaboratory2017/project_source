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

// set validation error flag as false
$error = false;

// check if form submitted
if (isset ( $_POST ['addproduct'] )) {
	$barcode = mysqli_real_escape_string ( $conn, $_POST ['barcode'] );
	$productName = mysqli_real_escape_string ( $conn, $_POST ['productName'] );
	$price = mysqli_real_escape_string ( $conn, $_POST ['price'] );
	$storeQty = mysqli_real_escape_string ( $conn, $_POST ['storeQty'] );
	$warehouseQty = mysqli_real_escape_string ( $conn, $_POST ['warehouseQty'] );
	$points = mysqli_real_escape_string ( $conn, $_POST ['points'] );
	
	if (! preg_match ( "/^[a-zA-Z0-9_ ]+$/", $productName )) {
		$error = true;
		$productNameError = "Product Name must contain alphabets and/or digits only";
	}
	if (! preg_match ( "/^[0-9]{9}+$/", $barcode )) {
		$error = true;
		$barcodeError = "Barcode must be a 9 digit number";
	}
	
	if (! $error) {
		$query = "SELECT * FROM product WHERE Barcode = '" . $barcode . "'";
		$query_name = "SELECT * FROM product WHERE Name = '" . $productName . "'";
		
		$result = mysqli_query ( $conn, $query );
		$result_name = mysqli_query ( $conn, $query_name );
		
		if (! $result || !$result_name) {
			echo ("Unable to execute query: " . mysqli_error ( $conn ));
			exit();
		} else {
			if (! $row = mysqli_fetch_array ( $result )) {
				if(! $row_name = mysqli_fetch_array ( $result_name )){
					$insert_query = "INSERT INTO product(Barcode, Name, Price, StoreQty, WarehouseQty, Points) VALUES('" . $barcode . "', '" . $productName . "', '" . $price . "', '" . $storeQty . "', '" . $warehouseQty . "', '" . $points . "')";
					$result = mysqli_query ( $conn, $insert_query );
					if (! $result) {
						$failure = "Error in Registering --- Please try again later!";
					} else {
						$success = "A new product is entered successfully!";
					}
				} else 
					$failure = "Product with same name already exists!";
			} else
				$failure = "Product with same Barcode already exists!";
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
<title>Home</title>

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
			<div class="col-md-8">
				<h1><a class="header" href="index.php">electronics.com</a></h1>
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
				<!--a class="btn btn-success btn-text" href="all_reservations.php">Show All Reservations</a-->
			</div>
			<div class="col-md-4">	
				<h1 class="text-primary">Add Product</h1>
					<br /> <br />
					<form name="addproductform" action="<?php echo $_SERVER['PHP_SELF']; ?>" method="post">
						<div class="form-group">
							
							<input type="text" class="form-control" name="barcode" id="barcode" placeholder="Barcode" required="required" value="<?php if($error) echo stripslashes($barcode); ?>" />
							<span class="text-danger"><?php if(isset($barcodeError)) echo $barcodeError; ?></span>
							<br /> <br />
							
							<input type="text" class="form-control" name="productName" id="productName" placeholder="Product Name" required="required" value="<?php if($error) echo stripslashes($productName); ?>" />
							<span class="text-danger"><?php if(isset($productNameError)) echo $productNameError;  ?></span>
							<br /> <br />
							
							<input type="number" step="0.01" class="form-control" name="price" id="price" placeholder="Price" required="required" value="<?php if($error) echo stripslashes($price); ?>" />
							<br /> <br />
							
							<input type="number" class="form-control" name="storeQty" id="storeQty" placeholder="Quantity in Store" required="required" value="<?php if($error) echo stripslashes($storeQty); ?>" />
							<br /> <br />
							
							<input type="number" class="form-control" name="warehouseQty" id="warehouseQty" placeholder="Quantity in Warehouse" required="required" value="<?php if($error) echo stripslashes($warehouseQty); ?>" />
							<br /> <br />
							
							<input type="number" class="form-control" name="points" id="points" placeholder="Points" required="required" value="<?php if($error) echo stripslashes($points); ?>" />
							<br /> <br />
							
							
							<input type="submit" class="btn btn-success btn-text" name="addproduct" value="Add">
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