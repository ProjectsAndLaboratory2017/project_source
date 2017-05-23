<!DOCTYPE html>
<html>
<body class="times-new-roman">
	<div class="container-fluid">
				<h2 class="text-primary">Products</h2>
				<?php
					$query = 	"SELECT r.Date, c.FirstName, c.LastName, p.Name, p.Price, r.Quantity 
								FROM customer c, product p, receipt r
								WHERE c.CustomerID = r.CustomerID
								AND p.ProductID = r.ProductID
								ORDER BY r.Date";
					$result = mysqli_query($conn, $query);
					if(! $result){
						echo ("Unable to execute query: " . mysqli_error ( $conn ));
						exit();
					}
					$records = mysqli_num_rows($result);
					$i = 0;
					if($records > 0){ $date2 = ""; ?>
				<table class="table table-hover table-striped table-bordered table-font">
				<?php while($row = mysqli_fetch_array($result, MYSQLI_ASSOC)){ 
						$date1 = $row['Date'];
						if($date1 != $date2){
				?>
				
				<tr><th class="text-success bg-info" colspan="5"><?php echo 'Dated: '. $date1; $date2 = $date1; ?></th></tr>
					<tr class="text-primary">
						<th class="text-center">S.No.</th>
						<th class="text-center">Customer</th>
						<th class="text-center">Product</th>
						<th class="text-center">Price</th>
						<th class="text-center">Quantity</th>
					</tr>
					<?php } ?>
					<tr>
						<td class="text-center"><?php $i = $i+1; echo $i; ?></td>
						<td><?php echo $row["FirstName"] .' '. $row["LastName"]; ?></td>
						<td><?php echo $row["Name"]; ?></td>
						<td class="text-center"><?php echo $row["Price"] .' euro'; ?></td>
						<td class="text-center"><?php echo $row["Quantity"]; ?></td>
					</tr>
					<?php } ?>
			</table>
				<?php } else {
					echo '<script type="text/javascript"> alert("You have not purchased anything yet!") </script>';
				 } ?>
	</div>
</body>
</html>