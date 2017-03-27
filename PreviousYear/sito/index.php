<?php 
$servername = "localhost";
$username = "gadgeteer";
$password = "gadgeteer";
$dbname = "gadgeteer";

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
} 
//echo "Connected successfully";
$page = $_SERVER['PHP_SELF'];
$sec = "10";
header("Refresh: $sec; url=$page");
?>
<!doctype html>

<html lang="en">
<head>

	<script type="text/javascript" src="js/jquery-latest.js"></script> 
	<script type="text/javascript" src="js/jquery.tablesorter.js"></script> 
  <meta charset="utf-8">

  <title>Our Database</title>
  <meta name="description" content="Our Database">
  <meta name="author" content="SitePoint">

  <!-- <link rel="stylesheet" href="css/styles.css?v=1.0"> -->

  <!--[if lt IE 9]>
    <script src="http://html5shiv.googlecode.com/svn/trunk/html5.js"></script>
  <![endif]-->
<style>
table {
    border-collapse: collapse;
    width: 100%;
}

th, td {
    text-align: left;
    padding: 8px;
}

tr:nth-child(even){background-color: #f2f2f2}

th {
    background-color: #4CAF50;
    color: white;
}
</style>
</head>

<script>
$(document).ready(function() 
    { 
        $("#myTable").tablesorter(); 
    } 
); 

</script>

<body>
<table id="myTable" class="tablesorter">
<thead>
<tr>
	<th>ID</th>
	<th>PHOTO</th>
	<th>STATUS</th>
	<th>TIME</th>
</tr>
</thead>
<tbody>
<?php 
/*

$orderBy = array('id', 'status', 'time');
$order = 'id';
if (isset($_GET['orderBy']) && in_array($_GET['orderBy'], $orderBy)) {
    $order = $_GET['orderBy'];
}

echo $order;

*/

$sql = "SELECT id, photo, status, time FROM photos ORDER BY id desc";
$result = $conn->query($sql);



if ($result->num_rows > 0) {
    // output data of each row
    while($row = $result->fetch_assoc()) {
		echo "<tr>";
        echo "<td>" . $row["id"]. "</td>" . 
		'<td><img src="data:image/jpeg;base64,' . base64_encode($row['photo']) . '" width="320" height="240"></td>' .
		'<td>' . $row['status'] . '</td>' .
		"<td>" . date("Y-m-d H:i:s", strtotime($row['time'])) . "</td>";
		echo "</tr>";
    }
} else {
    echo "0 results";
}


?>
</tbody>
</table>
</body>
</html>

