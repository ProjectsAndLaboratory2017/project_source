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
				<ul class="nav nav-pills nav-stacked">
					<li role="presentation">
						<form class="navbar-form" name="view_customers_form" action="<?php echo $_SERVER['PHP_SELF']; ?>" method="post">
							<div class="form-group">
								<input type="submit" class="btn btn-success btn-text" name="view_customers" value="View Customers">
							</div>
						</form>
					</li>
					<li role="presentation">
						<form class="navbar-form" name="view_products_form" action="<?php echo $_SERVER['PHP_SELF']; ?>" method="post">
							<div class="form-group">
								<input type="submit" class="btn btn-success btn-text" name="view_products" value="View Products">
							</div>
						</form>
					</li>
					<li role="presentation">
						<form class="navbar-form" name="view_purchases_form" action="<?php echo $_SERVER['PHP_SELF']; ?>" method="post">
							<div class="form-group">
								<input type="submit" class="btn btn-success btn-text" name="view_purchases" value="View Purchases">
							</div>
						</form>
					</li>
					<li role="presentation">
						<form class="navbar-form" name="add_product_form" action="<?php echo $_SERVER['PHP_SELF']; ?>" method="post">
							<div class="form-group">
								<input type="submit" class="btn btn-success btn-text" name="add_product" value="Add Product">
							</div>
						</form>
					</li>
					<li role="presentation">		
						<form class="navbar-form" name="remove_product_form" action="<?php echo $_SERVER['PHP_SELF']; ?>" method="post">
							<div class="form-group">
								<input type="submit" class="btn btn-success btn-text" name="remove_product" value="Remove Product">
							</div>
						</form>
					</li>
					<li role="presentation">
						<form class="navbar-form" name="update_product_form" action="<?php echo $_SERVER['PHP_SELF']; ?>" method="post">
							<div class="form-group">
								<input type="submit" class="btn btn-success btn-text" name="update_product" value="Update Product">
							</div>
						</form>
					</li>	
				</ul>
			</div>
			<div class="col-md-10">	
				<?php 
					if (isset ( $_POST ['view_customers'] )) {
						include 'view_customers.php';
					}
					else if(isset ( $_POST ['view_products'] )){
						//include 'view_products.php';
						header ( "Location: view_products.php" );
					}
					else if(isset ( $_POST ['view_purchases'] )){
						include 'view_purchases.php';
					}
					else if(isset ( $_POST ['add_product'] )){
						header ( "Location: add_product.php" );
					}
					else if(isset ( $_POST ['remove_product'] )){
						header ( "Location: remove_product.php" );
					}
					else if(isset ( $_POST ['update_product'] )){
						header ( "Location: update_product.php" );
					}
					else{
						include 'view_customers.php';
					}
					
				?>
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