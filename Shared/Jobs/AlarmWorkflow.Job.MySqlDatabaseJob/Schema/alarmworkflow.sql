/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

DROP DATABASE IF EXISTS `alarmworkflow`;
CREATE DATABASE IF NOT EXISTS `alarmworkflow` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `alarmworkflow`;

DROP TABLE IF EXISTS `operation`;
CREATE TABLE IF NOT EXISTS `operation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `IsAcknowledged` bit(1) NOT NULL DEFAULT b'0',
  `Timestamp` datetime NOT NULL,
  `OperationId` char(36) NOT NULL,
  `Serialized` longblob NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

DROP TABLE IF EXISTS `tb_einsatz`;
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
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

INSERT INTO `tb_einsatz` (`id`, `Einsatznr`, `Einsatzort`, `Einsatzplan`, `Hinweis`, `Kreuzung`, `Meldebild`, `Mitteiler`, `Objekt`, `Ort`, `Strasse`, `Fahrzeuge`, `Einsatzstichwort`, `Alarmtime`, `Faxtime`, `Stichwort`) VALUES (1, '1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
