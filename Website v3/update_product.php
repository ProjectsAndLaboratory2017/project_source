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

$barcode = "";
$record = 0;


// check if form submitted

if(isset($_POST['form'])){

	switch ($_POST['form']) {
        case "barcodeHidden":
            $barcode = mysqli_real_escape_string ( $conn, $_POST ['barcode'] );
			$select_barcode = "SELECT * FROM product WHERE Barcode = '" . $barcode . "'";
			$result_barcode = mysqli_query ( $conn, $select_barcode );
		
			if (! $result_barcode) {
				echo ("Unable to execute query: " . mysqli_error ( $conn ));
				exit();
			}
			$record = mysqli_num_rows($result_barcode);
			if(! $record)
				$product_failure = "Product does not exist!";
		break;

        case "updateProductHidden":
			$productName = mysqli_real_escape_string ( $conn, $_POST ['productName'] );
			$price = mysqli_real_escape_string ( $conn, $_POST ['price'] );
			$storeQty = mysqli_real_escape_string ( $conn, $_POST ['storeQty'] );
			$warehouseQty = mysqli_real_escape_string ( $conn, $_POST ['warehouseQty'] );
			$points = mysqli_real_escape_string ( $conn, $_POST ['points'] );
					
				if(isset($_POST['barcode_dis'])){
			
					$update_query = "UPDATE product
						SET Name = '" . $productName . "' , Price = '" . $price . "' , StoreQty = '" . $storeQty . "' , WarehouseQty = '" . $warehouseQty . "', Points = '" . $points . "'
						WHERE Barcode = '".$_POST ['barcode_dis']."'";
					$update_result = mysqli_query ( $conn, $update_query );
					if (! $update_result) {
						$update_failure = "Error in Updating --- Please try again later!";
					} else {
						$success = "Product has been updated successfully!";
					}
		
			}
			break;
		} 

}

?>

<!DOCTYPE html>
<html>
<head>

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
				<h1 class="text-primary">Update Product</h1>
					<br /> <br />
					
					<form name="barcodeForm" action="<?php echo $_SERVER['PHP_SELF']; ?>" method="post">
						<div class="form-group">
							<label>Enter the barcode of the product to be updated</label><br />
							<input type="text" class="form-control" name="barcode" id="barcode" placeholder="Barcode" required="required" />
							<br /> <br />
							<input type="hidden" name="form" value="barcodeHidden">
							<input type="submit" class="btn btn-success btn-text" name="show_details" value="Show Details">
							<br /> <br />
						</div>
					</form>
					
					<?php if($record > 0){ ?>
						<form name="updateProductForm" action="<?php echo $_SERVER['PHP_SELF']; ?>" method="post">
						<div class="form-group">
						<table class="table table-hover table-striped table-bordered table-font">
					<?php while($row = mysqli_fetch_array($result_barcode, MYSQLI_ASSOC)){ ?>
					<tr>
						<td class="text-primary">Barcode</td>
						<td> <input type="text" class="form-control" name="barcode_dis" id="barcode_dis" value="<?php echo $row['Barcode']; ?>" readonly /> </td>
					</tr>
					<tr>
						<td class="text-primary">Product Name</td>
						<td> <input type="text" class="form-control" name="productName" id="productName" value="<?php echo $row['Name']; ?>" pattern="[A-Za-z0-9_ ]{3,100}" title="Product Name must be atleast 3 characters long and must contain alphabets and/or digits only" />
						</td>							
					</tr>
					<tr>
						<td class="text-primary">Price</td>
						<td> <input type="number" step="0.01" min="0.00" class="form-control" name="price" id="price" value="<?php echo $row['Price']; ?>" /> </td>
					</tr>
					<tr>
						<td class="text-primary">Quantity in Store</td>
						<td> <input type="number" min="0" max="10" class="form-control" name="storeQty" id="storeQty" value="<?php echo $row['StoreQty']; ?>" /> </td>
					</tr>
					<tr>
						<td class="text-primary">Quantity in Warehouse</td>
						<td> <input type="number" min="0" class="form-control" name="warehouseQty" id="warehouseQty" value="<?php echo $row['WarehouseQty']; ?>" /> </td>
					</tr>
					<tr>
						<td class="text-primary">Points</td>
						<td> <input type="number" min="0" max="100" class="form-control" name="points" id="points" value="<?php echo $row['Points']; ?>" /> </td>
					</tr>
						
					<?php } ?>
			</table>
				<br />
				<input type="hidden" name="form" value="updateProductHidden">
				<input type="submit" class="btn btn-success btn-text" name="update_product" value="Update">
			</div>
			</form>
					<?php } ?>
										
					<?php  
						if(isset($success))
							echo "<script type='text/javascript'> alert('$success') </script>";
						else {
							if(isset($product_failure))
								echo "<script type='text/javascript'> alert('$product_failure') </script>";
							else if(isset($update_failure))
								echo "<script type='text/javascript'> alert('$update_failure') </script>";
						}
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