-- phpMyAdmin SQL Dump
-- version 4.7.0
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jun 25, 2017 at 11:23 AM
-- Server version: 10.1.22-MariaDB
-- PHP Version: 7.1.4

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `plcs`
--

-- --------------------------------------------------------

--
-- Table structure for table `customer`
--

CREATE TABLE `customer` (
  `CustomerID` int(11) NOT NULL,
  `Barcode` bigint(20) NOT NULL,
  `FirstName` varchar(15) NOT NULL,
  `LastName` varchar(15) NOT NULL,
  `Email` varchar(30) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `Points` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `customer`
--

INSERT INTO `customer` (`CustomerID`, `Barcode`, `FirstName`, `LastName`, `Email`, `Password`, `Points`) VALUES
(1, 100000, 'u1', 'user', 'u1@gmail.com', '24c9e15e52afc47c225b757e7bee1f9d', 22),
(2, 100001, 'u2', 'user', 'u2@gmail.com', '7e58d63b60197ceb55a1c487989a3720', 0),
(3, 100002, 'u3', 'user', 'u3@gmail.com', '92877af70a45fd6a2ed7fe81e1236b78', 0),
(4, 100003, 'Sorath', 'Asnani', 'sorath@gmail.com', 'aad75c0f494544bb687a55a3ba4fa064', 0),
(5, 100004, 'Martino', 'Mensio', 'martino@gmail.com', 'a5c36f67faa5284c625a73b7ea473feb', 0),
(6, 100005, 'Giuseppe', 'Carella', 'giuseppe@gmail.com', '3e6a90c7a131518bfe47b04d67c0b4a2', 0),
(7, 100006, 'MSG', 'Admin', 'msg@gmail.com', '6e2baaf3b97dbeef01c0043275f9a0e7', 0);

--
-- Triggers `customer`
--
DELIMITER $$
CREATE TRIGGER `before_customer_insert` BEFORE INSERT ON `customer` FOR EACH ROW BEGIN
  DECLARE newBarcode bigint default 100000;
  IF (SELECT count(*) FROM customer) = 0 THEN
      SET NEW.Barcode = newBarcode; 
  ELSE 
      SELECT MAX(Barcode) + 1 INTO newBarcode FROM customer;
      SET NEW.Barcode = newBarcode;
  END IF;

  SET NEW.Password = md5(NEW.Password);

END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `product`
--

CREATE TABLE `product` (
  `ProductID` int(11) NOT NULL,
  `Barcode` bigint(13) NOT NULL,
  `Name` varchar(30) NOT NULL,
  `Price` float NOT NULL,
  `StoreQty` int(11) NOT NULL,
  `WarehouseQty` int(11) NOT NULL,
  `Points` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `product`
--

INSERT INTO `product` (`ProductID`, `Barcode`, `Name`, `Price`, `StoreQty`, `WarehouseQty`, `Points`) VALUES
(1, 8000340479919, 'SKIPPER kiwi mela', 1.5, 10, 40, 15),
(2, 90370762, 'coca cola', 1.5, 10, 38, 10),
(3, 5410041000900, 'TUC crackers', 1.2, 10, 7, 7),
(4, 80007951, 'Acqua san Benedetto', 0.25, 10, 10, 5),
(5, 8010333001850, 'Acqua Martina frizzante', 0.5, 10, 17, 5);

--
-- Triggers `product`
--
DELIMITER $$
CREATE TRIGGER `after_product_insert` AFTER INSERT ON `product` FOR EACH ROW BEGIN
	IF NEW.StoreQty < 5 || (NEW.StoreQty >= 5 && NEW.StoreQty < 10) THEN 
    	IF (SELECT COUNT(*) FROM store_order WHERE store_order.ProductID = NEW.ProductID) THEN 
        	UPDATE store_order 
        	SET store_order.Quantity = 10-NEW.StoreQty
            WHERE store_order.ProductID = NEW.ProductID;
        ELSE 
        	INSERT INTO store_order(ProductID, Quantity) VALUES (NEW.ProductID, 10-NEW.StoreQty);
        END IF;
    ELSEIF NEW.StoreQty = 10 THEN
    	IF (SELECT COUNT(*) FROM store_order WHERE store_order.ProductID = NEW.ProductID) THEN 
        	DELETE FROM store_order
       		WHERE store_order.ProductID = NEW.ProductID;
        END IF;
    END IF;    
   
   IF NEW.WarehouseQty < 10 || (NEW.WarehouseQty >= 10 && NEW.WarehouseQty < 50) THEN 
    	IF (SELECT COUNT(*) FROM warehouse_order WHERE warehouse_order.ProductID = NEW.ProductID) THEN 
        	UPDATE warehouse_order 
        	SET warehouse_order.Quantity = 50-NEW.WarehouseQty
            WHERE warehouse_order.ProductID = NEW.ProductID;
        ELSE 
        	INSERT INTO warehouse_order(ProductID, Quantity) VALUES (NEW.ProductID, 50-NEW.WarehouseQty);
        END IF;
    ELSEIF NEW.WarehouseQty = 50 THEN
    	IF (SELECT COUNT(*) FROM warehouse_order WHERE warehouse_order.ProductID = NEW.ProductID) THEN 
        	DELETE FROM warehouse_order
       		WHERE warehouse_order.ProductID = NEW.ProductID;
        END IF;
    END IF;
   
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `after_product_update` AFTER UPDATE ON `product` FOR EACH ROW BEGIN
	IF NEW.StoreQty < 5 || (NEW.StoreQty >= 5 && NEW.StoreQty < 10) THEN 
    	IF (SELECT COUNT(*) FROM store_order WHERE store_order.ProductID = NEW.ProductID) THEN 
        	UPDATE store_order 
        	SET store_order.Quantity = 10-NEW.StoreQty
            WHERE store_order.ProductID = NEW.ProductID;
        ELSE 
        	INSERT INTO store_order(ProductID, Quantity) VALUES (NEW.ProductID, 10-NEW.StoreQty);
        END IF;
    ELSEIF NEW.StoreQty = 10 THEN
    	IF (SELECT COUNT(*) FROM store_order WHERE store_order.ProductID = NEW.ProductID) THEN 
        	DELETE FROM store_order
       		WHERE store_order.ProductID = NEW.ProductID;
        END IF;
    END IF;    
   
   IF NEW.WarehouseQty < 10 || (NEW.WarehouseQty >= 10 && NEW.WarehouseQty < 50) THEN 
    	IF (SELECT COUNT(*) FROM warehouse_order WHERE warehouse_order.ProductID = NEW.ProductID) THEN 
        	UPDATE warehouse_order 
        	SET warehouse_order.Quantity = 50-NEW.WarehouseQty
            WHERE warehouse_order.ProductID = NEW.ProductID;
        ELSE 
        	INSERT INTO warehouse_order(ProductID, Quantity) VALUES (NEW.ProductID, 50-NEW.WarehouseQty);
        END IF;
    ELSEIF NEW.WarehouseQty = 50 THEN
    	IF (SELECT COUNT(*) FROM warehouse_order WHERE warehouse_order.ProductID = NEW.ProductID) THEN 
        	DELETE FROM warehouse_order
       		WHERE warehouse_order.ProductID = NEW.ProductID;
        END IF;
    END IF;
   
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `before_product_insert` BEFORE INSERT ON `product` FOR EACH ROW BEGIN

	IF NEW.Barcode < 10000000 OR NEW.Barcode > 9999999999999
    THEN
    	SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Cannot add into product: Barcode must be 8 to 13 digits long';
    END IF;
    
	IF NEW.Price <= 0.00 || NEW.Price = 0
    THEN
    	SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Cannot add into product: Price cannot be less than or equal to 0.00';
    END IF;
    
    IF NEW.StoreQty < 0 OR NEW.StoreQty > 10
    THEN
    	SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Cannot add into product: Store Quantity must be between 0 and 10';
    END IF;
    
    IF NEW.WarehouseQty < 0 OR NEW.WarehouseQty > 50
    THEN
    	SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Cannot add into product: Warehouse Quantity must be between 0 and 50';
    END IF;
    
    IF NEW.Points < 0 OR NEW.Points > 100
    THEN
    	SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Cannot add into product: Points must be between 0 and 100';
    END IF;
    
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `before_product_update` BEFORE UPDATE ON `product` FOR EACH ROW BEGIN

	IF NEW.Barcode < 10000000 OR NEW.Barcode > 9999999999999
    THEN
    	SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Cannot update product: Barcode must be 8 to 13 digits long';
    END IF;
    
	IF NEW.Price <= 0.00 || NEW.Price = 0
    THEN
    	SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Cannot update product: Price cannot be less than or equal to 0.00';
    END IF;
    
    IF NEW.StoreQty < 0 OR NEW.StoreQty > 10
    THEN
    	SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Cannot update product: Store Quantity must be in between 0 and 10';
    END IF;
    
    IF NEW.WarehouseQty < 0 OR NEW.WarehouseQty > 50
    THEN
    	SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Cannot update product: Warehouse Quantity must be between 0 and 50';
    END IF;
    
    IF NEW.Points < 0 OR NEW.Points > 100
    THEN
    	SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Cannot update product: Points must be between 0 and 100';
    END IF;
    
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `receipt`
--

CREATE TABLE `receipt` (
  `ReceiptID` int(11) NOT NULL,
  `ProductID` int(11) NOT NULL,
  `CustomerID` int(11) NOT NULL,
  `Date` date NOT NULL,
  `Quantity` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `receipt`
--

INSERT INTO `receipt` (`ReceiptID`, `ProductID`, `CustomerID`, `Date`, `Quantity`) VALUES
(1, 2, 1, '2017-06-23', 2),
(2, 4, 1, '2017-06-23', 3),
(3, 3, 1, '2017-06-18', 1);

--
-- Triggers `receipt`
--
DELIMITER $$
CREATE TRIGGER `after_receipt_insert` AFTER INSERT ON `receipt` FOR EACH ROW BEGIN
	UPDATE product
    SET product.StoreQty = product.StoreQty - NEW.Quantity
    WHERE product.ProductID = NEW.ProductID;
    
    UPDATE customer
    SET customer.Points = customer.Points + (SELECT product.Points FROM product WHERE product.ProductID = NEW.ProductID)
    WHERE customer.CustomerID = NEW.CustomerID;
    
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `before_receipt_insert` BEFORE INSERT ON `receipt` FOR EACH ROW BEGIN
	IF ((SELECT product.StoreQty FROM product WHERE product.ProductID = NEW.ProductID) - NEW.Quantity < 0)
    THEN
    	SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Cannot add into receipt: requested quantity is not available';
    END IF;
    IF NEW.Date > CURRENT_DATE()
    THEN
    	SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Cannot add into receipt: Invalid Date';
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `store_order`
--

CREATE TABLE `store_order` (
  `OrderID` int(11) NOT NULL,
  `ProductID` int(11) NOT NULL,
  `Quantity` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `warehouse_order`
--

CREATE TABLE `warehouse_order` (
  `OrderID` int(11) NOT NULL,
  `ProductID` int(11) NOT NULL,
  `Quantity` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `warehouse_order`
--

INSERT INTO `warehouse_order` (`OrderID`, `ProductID`, `Quantity`) VALUES
(1, 1, 10),
(2, 2, 12),
(3, 3, 43),
(4, 4, 40),
(5, 5, 33);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `customer`
--
ALTER TABLE `customer`
  ADD PRIMARY KEY (`CustomerID`);

--
-- Indexes for table `product`
--
ALTER TABLE `product`
  ADD PRIMARY KEY (`ProductID`),
  ADD UNIQUE KEY `ProductID` (`ProductID`),
  ADD UNIQUE KEY `Barcode` (`Barcode`);

--
-- Indexes for table `receipt`
--
ALTER TABLE `receipt`
  ADD PRIMARY KEY (`ReceiptID`,`ProductID`,`CustomerID`),
  ADD KEY `ProductID` (`ProductID`),
  ADD KEY `CustomerID` (`CustomerID`);

--
-- Indexes for table `store_order`
--
ALTER TABLE `store_order`
  ADD PRIMARY KEY (`OrderID`),
  ADD KEY `ProductID` (`ProductID`);

--
-- Indexes for table `warehouse_order`
--
ALTER TABLE `warehouse_order`
  ADD PRIMARY KEY (`OrderID`),
  ADD KEY `ProductID` (`ProductID`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `customer`
--
ALTER TABLE `customer`
  MODIFY `CustomerID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;
--
-- AUTO_INCREMENT for table `product`
--
ALTER TABLE `product`
  MODIFY `ProductID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;
--
-- AUTO_INCREMENT for table `receipt`
--
ALTER TABLE `receipt`
  MODIFY `ReceiptID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;
--
-- AUTO_INCREMENT for table `store_order`
--
ALTER TABLE `store_order`
  MODIFY `OrderID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;
--
-- AUTO_INCREMENT for table `warehouse_order`
--
ALTER TABLE `warehouse_order`
  MODIFY `OrderID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;
--
-- Constraints for dumped tables
--

--
-- Constraints for table `receipt`
--
ALTER TABLE `receipt`
  ADD CONSTRAINT `receipt_ibfk_1` FOREIGN KEY (`ProductID`) REFERENCES `product` (`ProductID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `receipt_ibfk_2` FOREIGN KEY (`CustomerID`) REFERENCES `customer` (`CustomerID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `store_order`
--
ALTER TABLE `store_order`
  ADD CONSTRAINT `store_order_ibfk_1` FOREIGN KEY (`ProductID`) REFERENCES `product` (`ProductID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `warehouse_order`
--
ALTER TABLE `warehouse_order`
  ADD CONSTRAINT `warehouse_order_ibfk_1` FOREIGN KEY (`ProductID`) REFERENCES `product` (`ProductID`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
