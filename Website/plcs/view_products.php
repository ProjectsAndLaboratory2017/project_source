<!DOCTYPE html>
<html>
<body class="times-new-roman">
	<div class="container-fluid">
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
						<?php if($row["StoreQty"] < 5) { ?>
						<td class="text-center text-danger">
							<span class="glyphicon glyphicon-warning-sign text-danger" title="Warning! Very few pieces are available in store"></span>
							<?php echo $row["StoreQty"]; ?>
						</td>
						<?php } else { ?>
						<td class="text-center"><?php echo $row["StoreQty"]; ?></td>
						<?php } ?>
						<?php if($row["WarehouseQty"] < 10) { ?>
						<td class="text-center text-danger">
							<span class="glyphicon glyphicon-warning-sign text-danger" title="Warning! Very few pieces are available in warehouse"></span>
							<?php echo $row["WarehouseQty"]; ?>
						</td>
						<?php } else { ?>
						<td class="text-center"><?php echo $row["WarehouseQty"]; ?></td>
						<?php } ?>
						<td class="text-center"><?php echo $row["Points"]; ?></td>
					</tr>
					<?php } ?>
			</table>
				<?php } else {
					echo '<script type="text/javascript"> alert("You have not purchased anything yet!") </script>';
				 } ?>
	</div>
</body>
</html>