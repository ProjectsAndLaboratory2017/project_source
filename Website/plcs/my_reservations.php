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
<noscript> <meta http-equiv="refresh" content="0;url=test_javascript.php"> </noscript>

<meta charset="ISO-8859-1">
<title>My Reservations</title>

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
				<a class="btn btn-success btn-text" href="home.php">Add New Reservation</a> 
				<br /> <br />
				<a class="btn btn-success btn-text" href="my_reservations.php">Show My Reservations</a> 
				<br /> <br />
				<a class="btn btn-success btn-text" href="all_reservations.php">Show All Reservations</a>
			</div>
			<div class="col-md-10">	
				<h2 class="text-primary">My Purchases</h2>
				<?php
					$query =	"SELECT r.Date, p.Name, p.Price, r.Quantity, c.Points 
								FROM customer c, product p, receipt r
								WHERE c.CustomerID = r.CustomerID 
								AND p.ProductID = r.ProductID
								AND c.CustomerID = '".$_SESSION['userId']."'
								ORDER BY r.Date";
//					$query = "SELECT m.MachineName, r.RId, r.StartTime, r.EndTime, r.Duration FROM machine m, reservation r WHERE m.MachineId = r.MachineId AND UserId = '".$_SESSION['userId']."' ORDER BY StartTime";
					$result = mysqli_query($conn, $query);
					if(! $result){
						echo ("Unable to execute query: " . mysqli_error ( $conn ));
						exit();
					}
					$records = mysqli_num_rows($result);
					$i = 0;
					if($records > 0){ ?>
				<table class="table table-hover table-striped table-bordered text-center table-font">
					<tr class="text-primary">
						<th class="text-center">S.No.</th>
						<th class="text-center">Date</th>
						<th class="text-center">Product Name</th>
						<th class="text-center">Price</th>
						<th class="text-center">Quantity</th>
						<th class="text-center">Points</th>
					</tr>
					<?php while($row = mysqli_fetch_array($result, MYSQLI_ASSOC)){ ?>
					<tr>
						<td><?php $i = $i+1; echo $i; ?></td>
						<td><?php echo $row["Date"]; ?></td>
						<td><?php echo $row["Name"]; ?></td>
						<td><?php echo $row["Price"] .' euro'; ?></td>
						<td><?php echo $row["Quantity"]; ?></td>
						<td><?php echo $row["Points"]; ?></td>
					</tr>
					<?php } ?>
			</table>
				<?php } else {
					echo '<script type="text/javascript"> alert("You have not purchased anything yet!") </script>';
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