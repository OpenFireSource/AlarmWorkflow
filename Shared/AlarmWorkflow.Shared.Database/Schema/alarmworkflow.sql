/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

DROP DATABASE IF EXISTS `alarmworkflow`;
CREATE DATABASE IF NOT EXISTS `alarmworkflow` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `alarmworkflow`;

DROP TABLE IF EXISTS `operation`;
CREATE TABLE IF NOT EXISTS `operation` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `acknowledged` bit(1) NOT NULL DEFAULT b'0',
  `operationguid` char(36) NOT NULL,
  `operationnumber` varchar(100) NULL,
  `timestampincome` datetime NOT NULL,
  `timestampalarm` datetime NOT NULL,
  `messenger` varchar(200) NULL,
  `comment` varchar(1000) NULL,
  `plan` varchar(1000) NULL,
  `picture` varchar(1000) NULL,
  `priority` varchar(100) NULL,

  /* Breaking the normalization-rules from here on for convenience and clarity. Sorry, E. Codd! */
  `einsatzortstreet` varchar(100) NULL,
  `einsatzortstreetnumber` varchar(100) NULL,
  `einsatzortzipcode` varchar(100) NULL,
  `einsatzortcity` varchar(100) NULL,
  `einsatzortintersection` varchar(200) NULL,
  `einsatzortlocation` varchar(200) NULL,
  `einsatzortproperty` varchar(200) NULL,
  `einsatzortlatlng` varchar(200) NULL,

  `zielortstreet` varchar(100) NULL,
  `zielortstreetnumber` varchar(100) NULL,
  `zielortzipcode` varchar(100) NULL,
  `zielortcity` varchar(100) NULL,
  `zielortintersection` varchar(200) NULL,
  `zielortlocation` varchar(200) NULL,
  `zielortproperty` varchar(200) NULL,
  `zielortlatlng` varchar(200) NULL,
  
  `keyword` varchar(200) NULL,
  `keywordmisc` varchar(200) NULL,
  `keywordb` varchar(200) NULL,
  `keywordr` varchar(200) NULL,
  `keywords` varchar(200) NULL,
  `keywordt` varchar(200) NULL,

  `loopscsv` varchar(300) NULL,
  `customdatajson` longtext NULL,

  PRIMARY KEY (`Id`)
);

DROP TABLE IF EXISTS `operationresource`;
CREATE TABLE IF NOT EXISTS `operationresource` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `operation_id` int(11) NOT NULL,
  `timestamp` varchar(50) NULL,
  `fullname` varchar(200) NULL,
  `equipmentcsv` text NULL,

  PRIMARY KEY (`id`),
  FOREIGN KEY (`operation_id`) REFERENCES operation(`id`)
);

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
