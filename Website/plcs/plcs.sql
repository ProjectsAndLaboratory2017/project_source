-- phpMyAdmin SQL Dump
-- version 4.5.1
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1
-- Generation Time: Apr 24, 2017 at 09:16 AM
-- Server version: 10.1.13-MariaDB
-- PHP Version: 7.0.5

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
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
  `Barcode` varchar(15) NOT NULL,
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
(6, '123456789', 'user1', 'user', 'u1@gmail.com', '24c9e15e52afc47c225b757e7bee1f9d', 0),
(7, '098765432', 'user2', 'user', 'u2@gmail.com', '7e58d63b60197ceb55a1c487989a3720', 0),
(8, '987654321', 'user3', 'user', 'u3@gmail.com', '92877af70a45fd6a2ed7fe81e1236b78', 0),
(9, '667890123', 'Sorath', 'Asnani', 'sorath@gmail.com', '69dfb711f10ea5980f4dc37ca069c2c5', 0),
(10, '876543210', 'Martino', 'Mensio', 'martino@gmail.com', '37ede6fa3d6089ba78da88aff24e1976', 0),
(11, '789012345', 'Giuseppe', 'Carella', 'giuseppe@gmail.com', '353f9bfab2d01dbb1db343fdaf9ab02e', 0),
(12, '111222333', 'Admin', 'Manager', 'msg@gmail.com', '6e2baaf3b97dbeef01c0043275f9a0e7', 0),
(13, '333666999', 'Mario', 'Rossi', 'mariorossi@mario.it', 'de2f15d014d40b93578d255e6221fd60', 0);

-- --------------------------------------------------------

--
-- Table structure for table `product`
--

CREATE TABLE `product` (
  `ProductID` int(11) NOT NULL,
  `Barcode` varchar(15) NOT NULL,
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
(1, '012345678', 'FEZ Spider II Mainboard', 15, 5, 20, 15),
(2, '123456789', 'Display TE35 Module', 10, 2, 18, 12),
(3, '234567890', 'USB Client EDP Module', 8, 1, 14, 5),
(4, '345678901', '2x Button Module', 5, 1, 16, 3),
(5, '456789012', 'Ethernet J11D Module', 9, 4, 1, 7),
(6, '567890123', 'Joystick Module', 6, 9, 10, 3),
(7, '678901234', 'LED Strip Module', 2, 16, 0, 3),
(8, '789012345', 'Light Sensor Module', 7, 3, 15, 9),
(9, '890123456', 'SD Card Module', 14, 4, 30, 4),
(10, '901234567', '128mb SD Card', 8, 5, 15, 7),
(11, '123456781', 'Tune Module', 4, 10, 3, 1),
(12, '234567822', 'USB Host Module', 9, 2, 10, 11),
(13, '345678321', 'Holey Board', 6, 2, 19, 8),
(14, '456784321', 'USB Cable', 2, 4, 2, 1),
(15, '567812345', 'Reusable Plastic Storage Box', 5, 10, 9, 2);

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
(1, 3, 6, '2017-04-10', 1),
(2, 4, 6, '2017-04-17', 3),
(3, 7, 6, '2017-04-17', 1),
(4, 5, 6, '2017-04-17', 6),
(5, 1, 6, '2017-04-10', 1),
(6, 15, 6, '2017-04-10', 2),
(7, 10, 6, '2017-04-10', 3),
(8, 8, 6, '2017-04-10', 9);

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
  ADD PRIMARY KEY (`ProductID`);

--
-- Indexes for table `receipt`
--
ALTER TABLE `receipt`
  ADD PRIMARY KEY (`ReceiptID`,`ProductID`,`CustomerID`),
  ADD KEY `ProductID` (`ProductID`),
  ADD KEY `CustomerID` (`CustomerID`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `customer`
--
ALTER TABLE `customer`
  MODIFY `CustomerID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;
--
-- AUTO_INCREMENT for table `product`
--
ALTER TABLE `product`
  MODIFY `ProductID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;
--
-- AUTO_INCREMENT for table `receipt`
--
ALTER TABLE `receipt`
  MODIFY `ReceiptID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;
--
-- Constraints for dumped tables
--

--
-- Constraints for table `receipt`
--
ALTER TABLE `receipt`
  ADD CONSTRAINT `receipt_ibfk_1` FOREIGN KEY (`ProductID`) REFERENCES `product` (`ProductID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `receipt_ibfk_2` FOREIGN KEY (`CustomerID`) REFERENCES `customer` (`CustomerID`) ON DELETE CASCADE ON UPDATE CASCADE;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
