<!DOCTYPE html>
<html>
<body class="times-new-roman">
	<div class="container-fluid">
				<h2 class="text-primary">My Purchases</h2>
				<?php
					$query =	"SELECT r.Date, p.Name, p.Price, r.Quantity 
								FROM customer c, product p, receipt r
								WHERE c.CustomerID = r.CustomerID 
								AND p.ProductID = r.ProductID
								AND c.CustomerID = '".$_SESSION['userId']."'
								ORDER BY r.Date";
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
					</tr>
					<?php while($row = mysqli_fetch_array($result, MYSQLI_ASSOC)){ ?>
					<tr>
						<td><?php $i = $i+1; echo $i; ?></td>
						<td><?php echo $row["Date"]; ?></td>
						<td><?php echo $row["Name"]; ?></td>
						<td><?php echo $row["Price"] .' euro'; ?></td>
						<td><?php echo $row["Quantity"]; ?></td>
					</tr>
					<?php } ?>
			</table>
				<?php } else {
					echo '<script type="text/javascript"> alert("You have not purchased anything yet!") </script>';
				 } ?>
	</div>
</body>
</html>