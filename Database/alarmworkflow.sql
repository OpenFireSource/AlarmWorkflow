SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

CREATE SCHEMA IF NOT EXISTS `alarmworkflow` DEFAULT CHARACTER SET latin1 ;
USE `alarmworkflow` ;

-- -----------------------------------------------------
-- Table `alarmworkflow`.`operation`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `alarmworkflow`.`operation` ;

CREATE TABLE IF NOT EXISTS `alarmworkflow`.`operation` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `acknowledged` BIT(1) NOT NULL DEFAULT b'0',
  `operationguid` CHAR(36) NOT NULL,
  `operationnumber` VARCHAR(100) NULL DEFAULT NULL,
  `timestampincome` DATETIME NOT NULL,
  `timestampalarm` DATETIME NOT NULL,
  `messenger` VARCHAR(200) NULL DEFAULT NULL,
  `comment` VARCHAR(1000) NULL DEFAULT NULL,
  `plan` VARCHAR(1000) NULL DEFAULT NULL,
  `picture` VARCHAR(1000) NULL DEFAULT NULL,
  `priority` VARCHAR(100) NULL DEFAULT NULL,
  `einsatzortstreet` VARCHAR(100) NULL DEFAULT NULL,
  `einsatzortstreetnumber` VARCHAR(100) NULL DEFAULT NULL,
  `einsatzortzipcode` VARCHAR(100) NULL DEFAULT NULL,
  `einsatzortcity` VARCHAR(100) NULL DEFAULT NULL,
  `einsatzortintersection` VARCHAR(200) NULL DEFAULT NULL,
  `einsatzortlocation` VARCHAR(200) NULL DEFAULT NULL,
  `einsatzortproperty` VARCHAR(200) NULL DEFAULT NULL,
  `einsatzortlatlng` VARCHAR(200) NULL DEFAULT NULL,
  `zielortstreet` VARCHAR(100) NULL DEFAULT NULL,
  `zielortstreetnumber` VARCHAR(100) NULL DEFAULT NULL,
  `zielortzipcode` VARCHAR(100) NULL DEFAULT NULL,
  `zielortcity` VARCHAR(100) NULL DEFAULT NULL,
  `zielortintersection` VARCHAR(200) NULL DEFAULT NULL,
  `zielortlocation` VARCHAR(200) NULL DEFAULT NULL,
  `zielortproperty` VARCHAR(200) NULL DEFAULT NULL,
  `zielortlatlng` VARCHAR(200) NULL DEFAULT NULL,
  `keyword` VARCHAR(200) NULL DEFAULT NULL,
  `keywordmisc` VARCHAR(200) NULL DEFAULT NULL,
  `keywordb` VARCHAR(200) NULL DEFAULT NULL,
  `keywordr` VARCHAR(200) NULL DEFAULT NULL,
  `keywords` VARCHAR(200) NULL DEFAULT NULL,
  `keywordt` VARCHAR(200) NULL DEFAULT NULL,
  `loopscsv` VARCHAR(300) NULL DEFAULT NULL,
  `customdatajson` LONGTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`id`));


-- -----------------------------------------------------
-- Table `alarmworkflow`.`operationresource`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `alarmworkflow`.`operationresource` ;

CREATE TABLE IF NOT EXISTS `alarmworkflow`.`operationresource` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `operation_id` INT(11) NOT NULL,
  `timestamp` VARCHAR(50) NULL DEFAULT NULL,
  `fullname` VARCHAR(200) NULL DEFAULT NULL,
  `equipmentcsv` TEXT NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_{99BC450F-214E-4A06-8714-CA527129047D}` (`operation_id` ASC),
  CONSTRAINT `fk_{99BC450F-214E-4A06-8714-CA527129047D}`
    FOREIGN KEY (`operation_id`)
    REFERENCES `alarmworkflow`.`operation` (`id`));


-- -----------------------------------------------------
-- Table `alarmworkflow`.`usersetting`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `alarmworkflow`.`usersetting` ;

CREATE TABLE IF NOT EXISTS `alarmworkflow`.`usersetting` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `identifier` VARCHAR(100) NOT NULL,
  `name` VARCHAR(100) NOT NULL,
  `value` LONGTEXT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
