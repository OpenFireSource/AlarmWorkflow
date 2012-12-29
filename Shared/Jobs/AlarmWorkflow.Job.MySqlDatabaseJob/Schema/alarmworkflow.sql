-- phpMyAdmin SQL Dump
-- version 3.4.5
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Erstellungszeit: 17. Dez 2012 um 20:07
-- Server Version: 5.5.16
-- PHP-Version: 5.3.8

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Datenbank: `alarmworkflow`
--

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `accessright`
--

CREATE TABLE IF NOT EXISTS `accessright` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Name_UNIQUE` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `address`
--

CREATE TABLE IF NOT EXISTS `address` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Person_Id` int(11) NOT NULL,
  `AddressType` varchar(100) NOT NULL,
  `AddressValue` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Person_Id_idx` (`Person_Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `operation`
--

CREATE TABLE IF NOT EXISTS `operation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `IsAcknowledged` bit(1) NOT NULL DEFAULT b'0',
  `Timestamp` datetime NOT NULL,
  `OperationId` char(36) NOT NULL,
  `Serialized` longblob NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `person`
--

CREATE TABLE IF NOT EXISTS `person` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `FirstName` varchar(200) NOT NULL,
  `LastName` varchar(200) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `tb_einsatz`
--

CREATE TABLE IF NOT EXISTS `tb_einsatz` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `Einsatznr` varchar(255) DEFAULT NULL,
  `Einsatzort` varchar(255) DEFAULT NULL,
  `Einsatzplan` varchar(255) DEFAULT NULL,
  `Hinweis` varchar(255) DEFAULT NULL,
  `Kreuzung` varchar(255) DEFAULT NULL,
  `Meldebild` varchar(255) DEFAULT NULL,
  `Mitteiler` varchar(255) DEFAULT NULL,
  `Objekt` varchar(255) DEFAULT NULL,
  `Ort` varchar(255) DEFAULT NULL,
  `Strasse` varchar(255) DEFAULT NULL,
  `Fahrzeuge` varchar(255) DEFAULT NULL,
  `Einsatzstichwort` varchar(255) DEFAULT NULL,
  `Alarmtime` varchar(255) DEFAULT NULL,
  `Faxtime` varchar(255) DEFAULT NULL,
  `Stichwort` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `user`
--

CREATE TABLE IF NOT EXISTS `user` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Person_Id` int(11) DEFAULT NULL,
  `Name` varchar(100) NOT NULL,
  `Password` blob,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Name_UNIQUE` (`Name`),
  KEY `FK_User_Person_JI_idx` (`Person_Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `user_accessright`
--

CREATE TABLE IF NOT EXISTS `user_accessright` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `User_Id` int(11) NOT NULL,
  `AccessRight_Id` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_User_AccessRight_User_Id_idx` (`User_Id`),
  KEY `FK_User_AccessRight_AccessRight_Id_idx` (`AccessRight_Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

--
-- Constraints der exportierten Tabellen
--

--
-- Constraints der Tabelle `address`
--
ALTER TABLE `address`
  ADD CONSTRAINT `FK_Address_Person_Id` FOREIGN KEY (`Person_Id`) REFERENCES `person` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints der Tabelle `user`
--
ALTER TABLE `user`
  ADD CONSTRAINT `FK_User_Person_Id` FOREIGN KEY (`Person_Id`) REFERENCES `person` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints der Tabelle `user_accessright`
--
ALTER TABLE `user_accessright`
  ADD CONSTRAINT `FK_User_AccessRight_User_Id` FOREIGN KEY (`User_Id`) REFERENCES `user` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `FK_User_AccessRight_AccessRight_Id` FOREIGN KEY (`AccessRight_Id`) REFERENCES `accessright` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
