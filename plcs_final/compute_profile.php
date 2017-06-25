<!DOCTYPE html>
<html>
<body class="times-new-roman">
	<div class="container-fluid">
		<h2 class="text-primary">My Profile</h2>
		<?php
			$query =	"SELECT * 
						FROM customer
						WHERE CustomerID = '".$_SESSION['userId']."'";
			$result = mysqli_query($conn, $query);
			if(! $result){
				echo ("Unable to execute query: " . mysqli_error ( $conn ));
				exit();
			}
			$records = mysqli_num_rows($result);
			$i = 0;
			if($records > 0){ ?>
				<table class="table table-hover table-striped table-bordered table-font">
				<?php while($row = mysqli_fetch_array($result, MYSQLI_ASSOC)){ ?>
					<tr>
						<td class="text-primary">Barcode</td>
						<td><?php echo $row["Barcode"]; ?></td>
					</tr>
					<tr>
						<td class="text-primary">First Name</td>
						<td><?php echo $row["FirstName"]; ?></td>
					</tr>
					<tr>
						<td class="text-primary">Last Name</td>
						<td><?php echo $row["LastName"]; ?></td>
					</tr>
					<tr>
						<td class="text-primary">Email</td>
						<td><?php echo $row["Email"]; ?></td>
					</tr>
					<tr>
						<td class="text-primary">Points</td>
						<td><?php echo $row["Points"]; ?></td>
					</tr>
						
				<?php } ?>
				</table>
		<?php } ?>
	</div>
</body>
</html>