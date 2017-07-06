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
$wmax = 50; //max quantity in warehouse
$s_threshold = 5;
$w_threshold = 10;

if (isset ( $_POST ['order_from_warehouse'] )) {
		$order_query = "SELECT * FROM store_order";
		$order_result = mysqli_query($conn, $order_query);
			if(! $order_result){
				echo ("Unable to execute query: " . mysqli_error ( $conn ));
				exit();
			}
			while($order_row = mysqli_fetch_array($order_result, MYSQLI_ASSOC)){
				$prod_query = "SELECT * FROM product WHERE ProductId = '". $order_row['ProductID']."'";
				$prod_result = mysqli_query($conn, $prod_query);
			if(! $prod_result){
				echo ("Unable to execute query: " . mysqli_error ( $conn ));
				exit();
			}
			if($prod_row = mysqli_fetch_array($prod_result, MYSQLI_ASSOC)){
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
			}
			
			$update_query = "UPDATE product
							SET StoreQty = '" . $s . "' , WarehouseQty = '" . $w . "'
							WHERE ProductID = '". $prod_row['ProductID']."' ";
			$update_result = mysqli_query ( $conn, $update_query );
			if (! $update_result) {
				echo '<script type="text/javascript"> alert("Update failed!") </script>';
			}
			
			}
			
		}
		echo '<script type="text/javascript"> alert("Items have been placed in Store!") </script>';
}

if (isset ( $_POST ['order_from_supplier'] )) {
	echo '<script type="text/javascript"> alert("It will take 1 day to bring items in warehouse!") </script>';
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
			<div class="col-md-2">
				<br /> <br /> <br />
				<a class="btn btn-success btn-text" href="home_manager.php">Back to Home</a> 
			</div>
			<div class="col-md-10">
				<h2 class="text-primary">Orders for Store</h2>
				<?php
					$query = 	"SELECT product.Name, store_order.Quantity
								FROM product, store_order 
								WHERE product.ProductID = store_order.ProductID";
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
							<th>Product Name</th>
							<th>Quantity</th>
						</tr>
						<?php while($row = mysqli_fetch_array($result, MYSQLI_ASSOC)){ ?>
						<tr>
							<td class="text-center"><?php $i = $i+1; echo $i; ?></td>
							<td><?php echo $row["Name"]; ?></td>
							<td><?php echo $row["Quantity"]; ?></td>
						</tr>
						<?php } ?>
					</table>
				<form class="pull-right" name="orderform" action="<?php echo $_SERVER['PHP_SELF']; ?>" method="post">		
					<input type="submit" class="btn btn-success btn-text" name="order_from_warehouse" value="Order" />
				</form>
				<?php } else {
					echo "\nNothing to order yet!\n\n";
				} ?>
		
				<br /> <br /> <br />
				<h2 class="text-primary">Orders for Warehouse</h2>
				<?php
					$query = 	"SELECT product.Name, warehouse_order.Quantity
								FROM product, warehouse_order 
								WHERE product.ProductID = warehouse_order.ProductID";
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
						<th>Product Name</th>
						<th>Quantity</th>
					</tr>
					<?php while($row = mysqli_fetch_array($result, MYSQLI_ASSOC)){ ?>
					<tr>
						<td class="text-center"><?php $i = $i+1; echo $i; ?></td>
						<td><?php echo $row["Name"]; ?></td>
						<td><?php echo $row["Quantity"]; ?></td>
					</tr>
					<?php } ?>
				</table>
				<form class="pull-right" name="orderform" action="<?php echo $_SERVER['PHP_SELF']; ?>" method="post">		
					<input type="submit" class="btn btn-success btn-text" name="order_from_supplier" value="Order" />
				</form>
				<?php } else {
					echo "\nNothing to order yet!\n\n";
				} ?>
				<br /> <br /> <br /> <br />
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