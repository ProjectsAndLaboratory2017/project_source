<!DOCTYPE html>
<html>
<body class="times-new-roman">
	<div class="container-fluid">
		<h2 class="text-primary">Customers</h2>
		<?php
			$query = "SELECT * FROM `customer` WHERE Email != 'msg@gmail.com' AND LastName != 'Admin'";
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
					<th>First Name</th>
					<th>Last Name</th>
					<th>Email</th>
					<th class="text-center">Points</th>
				</tr>
				<?php while($row = mysqli_fetch_array($result, MYSQLI_ASSOC)){ ?>
				<tr>
					<td class="text-center"><?php $i = $i+1; echo $i; ?></td>
					<td><?php echo $row["Barcode"]; ?></td>
					<td><?php echo $row["FirstName"]; ?></td>
					<td><?php echo $row["LastName"]; ?></td>
					<td><?php echo $row["Email"]; ?></td>
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