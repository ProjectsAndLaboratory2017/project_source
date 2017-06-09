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

$smax = 10; //max quantity in store
$wmax = 20; //max quantity in warehouse
$s_threshold = 5;
$w_threshold = 10;

if (isset ( $_POST ['order_from_warehouse'] )) {
	if(isset($_POST['prodId'])){
		//$prod_query = "SELECT * FROM `product` where ProductId = 2 ";
		$prod_query = "SELECT * FROM `product` where ProductId = '". $_POST['prodId']."' ";
		$prod_result = mysqli_query($conn, $prod_query);
			if(! $prod_result){
				echo ("Unable to execute query: " . mysqli_error ( $conn ));
				exit();
			}
			$prod_row = mysqli_fetch_array($prod_result, MYSQLI_ASSOC);
			$s = $prod_row["StoreQty"];
			$w = $prod_row["WarehouseQty"];
			
			if($w > ($smax - $s)){
				while($s < $smax){
					$s++;
					$w--;	
				}
			}
			else{
				while($s <= $smax && $w > 0){
					$s++;
					$w--;
					if($w == 0)
						break;
				}
				echo '<script type="text/javascript"> alert("Item finished in Warehouse!") </script>';
			}
			
			//echo "s = " .$s;
			//echo "w = " .$w;
			
			$update_query = "UPDATE product
						SET StoreQty = '" . $s . "' , WarehouseQty = '" . $w . "'
						WHERE ProductID = '". $_POST['prodId']."' ";
			$update_result = mysqli_query ( $conn, $update_query );
			if (! $update_result) {
				echo '<script type="text/javascript"> alert("Update failed!") </script>';
				//$update_failure = "Error in Updating --- Please try again later!";
			} else {
				echo '<script type="text/javascript"> alert("Update successful!") </script>';
				//$success = "Product has been updated successfully!";
			}
			
	}
}

if (isset ( $_POST ['order_from_supplier'] )) {
	$update_query = "UPDATE product
						SET WarehouseQty = '" . $wmax . "'
						WHERE ProductID = '". $_POST['prodId']."' ";
			$update_result = mysqli_query ( $conn, $update_query );
			if (! $update_result) {
				echo '<script type="text/javascript"> alert("Update failed!") </script>';
				//$update_failure = "Error in Updating --- Please try again later!";
			} else {
				echo '<script type="text/javascript"> alert("Update warehouse successful!") </script>';
				//$success = "Product has been updated successfully!";
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
			<div class="col-md-2">
				<br /> <br /> <br />
				<a class="btn btn-success btn-text" href="home_manager.php">Back to Home</a> 
				<br /> <br /> <br />
				<a class="btn btn-success btn-text" href="order_products.php">Order Products</a> 
			</div>
		<div class="col-md-10">
				<h2 class="text-primary">Products</h2>
				<?php
					$query = "SELECT * FROM `product`";
					$result = mysqli_query($conn, $query);
					if(! $result){
						echo ("Unable to execute query: " . mysqli_error ( $conn ));
						exit();
					}
					$records = mysqli_num_rows($result);
					$i = 0;
					if($records > 0){ ?>
				<table class="table table-hover table-striped table-bordered table-font">
					<tr class="text-primary">
						<th class="text-center">S.No.</th>
						<th>Barcode</th>
						<th>Product Name</th>
						<th class="text-center">Price</th>
						<th class="text-center">Quantity in Store</th>
						<th class="text-center">Quantity in Warehouse</th>
						<th class="text-center">Points</th>
					</tr>
					<?php while($row = mysqli_fetch_array($result, MYSQLI_ASSOC)){ ?>
					<tr>
						<td class="text-center"><?php $i = $i+1; echo $i; ?></td>
						<td><?php echo $row["Barcode"]; ?></td>
						<td><?php echo $row["Name"]; ?></td>
						<td class="text-center"><?php echo $row["Price"] .' euro'; ?></td>
						<?php if($row["StoreQty"] < $s_threshold) { ?>
						<td class="text-center text-danger">
							<span class="glyphicon glyphicon-warning-sign text-danger" title="Warning! Not enough items"></span>
							<?php echo $row["StoreQty"]; ?>
						</td>
							
						<?php } else { ?>
						<td class="text-center"><?php echo $row["StoreQty"]; ?></td>
						<?php } ?>
						<?php if($row["WarehouseQty"] < $w_threshold) { ?>
						<td class="text-center text-danger">
							<span class="glyphicon glyphicon-warning-sign text-danger" title="Warning! Not enough items"></span>
							<?php echo $row["WarehouseQty"]; ?>
						</td>
						</form>
						<?php } else { ?>
						<td class="text-center"><?php echo $row["WarehouseQty"]; ?></td>
						<?php } ?>
						<td class="text-center"><?php echo $row["Points"]; ?></td>
					</tr>
					<?php } ?>
			</table>
				<?php } else {
					echo '<script type="text/javascript"> alert("Products are not available yet!") </script>';
				 } ?>
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