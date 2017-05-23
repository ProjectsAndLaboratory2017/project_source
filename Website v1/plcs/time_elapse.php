<?php

// Authentication remains valid for 2 minutes (60 seconds), after that the user is required to login again
if (isset ( $_SESSION ['lastActivity'] ) && (time () - $_SESSION ['lastActivity'] > 500)) {
	session_destroy ();
	unset ( $_SESSION ['userId'] );
	unset ( $_SESSION ['username'] );
	unset ( $_SESSION ['lastActivity'] );
	header ( "Location: expired.php" );
	exit();
} else {
	$_SESSION ['lastActivity'] = time (); // update last activity time stamp
}

?>