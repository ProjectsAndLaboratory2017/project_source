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
	
	if (! preg_match ( "/^[a-zA-Z0-9 ]+$/", $productName )) {
		$error = true;
		$productNameError = "Product Name must contain alphabets and/or digits only";
	}
	if (! preg_match ( "/^[0-9]{9}+$/", $barcode )) {
		$error = true;
		$barcodeError = "Barcode must be a 9 digit number";
	}
	
	if (! $error) {
		$query = "SELECT * FROM product WHERE Barcode = '" . $barcode . "'";
		$result = mysqli_query ( $conn, $query );
		if (! $result) {
			echo ("Unable to execute query: " . mysqli_error ( $conn ));
			exit();
		} else {
			if (! $row = mysqli_fetch_array ( $result )) {
				$insert_query = "INSERT INTO product(Barcode, Name, Price, StoreQty, WarehouseQty, Points) VALUES('" . $barcode . "', '" . $productName . "', '" . $price . "', '" . $storeQty . "', '" . $warehouseQty . "', '" . $points . "')";
				$result = mysqli_query ( $conn, $insert_query );
				if (! $result) {
					$failure = "Error in Registering --- Please try again later!";
				} else {
					$success = "A new product is entered successfully!";
				}
			} else
				$failure = "This product is already present!";
		}
	}
}
?>

<!DOCTYPE html>
<html>

<body class="times-new-roman">
	<div class="container-fluid">
		<div class="row rm">
			<div class="col-md-10">
				<div class="col-md-2"></div>
				<div class="col-md-6">
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
							
							
							<input type="submit" class="btn btn-success btn-text" name="addproduct" value="Add Product">
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
					<div class="col-md-2"></div>
			</div>
		</div>
			
	</div>
</body>
</html>