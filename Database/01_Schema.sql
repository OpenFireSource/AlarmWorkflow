SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

-- -----------------------------------------------------
-- Schema alarmworkflow
-- -----------------------------------------------------
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
  `operationnumber` LONGTEXT NULL DEFAULT NULL,
  `timestampincome` DATETIME NOT NULL,
  `timestampalarm` DATETIME NOT NULL,
  `messenger` LONGTEXT NULL DEFAULT NULL,
  `comment` LONGTEXT NULL DEFAULT NULL,
  `plan` LONGTEXT NULL DEFAULT NULL,
  `picture` LONGTEXT NULL DEFAULT NULL,
  `priority` LONGTEXT NULL DEFAULT NULL,
  `einsatzortstreet` LONGTEXT NULL DEFAULT NULL,
  `einsatzortstreetnumber` LONGTEXT NULL DEFAULT NULL,
  `einsatzortzipcode` LONGTEXT NULL DEFAULT NULL,
  `einsatzortcity` LONGTEXT NULL DEFAULT NULL,
  `einsatzortintersection` LONGTEXT NULL DEFAULT NULL,
  `einsatzortlocation` LONGTEXT NULL DEFAULT NULL,
  `einsatzortproperty` LONGTEXT NULL DEFAULT NULL,
  `einsatzortlatlng` LONGTEXT NULL DEFAULT NULL,
  `zielortstreet` LONGTEXT NULL DEFAULT NULL,
  `zielortstreetnumber` LONGTEXT NULL DEFAULT NULL,
  `zielortzipcode` LONGTEXT NULL DEFAULT NULL,
  `zielortcity` LONGTEXT NULL DEFAULT NULL,
  `zielortintersection` LONGTEXT NULL DEFAULT NULL,
  `zielortlocation` LONGTEXT NULL DEFAULT NULL,
  `zielortproperty` LONGTEXT NULL DEFAULT NULL,
  `zielortlatlng` LONGTEXT NULL DEFAULT NULL,
  `keyword` LONGTEXT NULL DEFAULT NULL,
  `keywordmisc` LONGTEXT NULL DEFAULT NULL,
  `keywordb` LONGTEXT NULL DEFAULT NULL,
  `keywordr` LONGTEXT NULL DEFAULT NULL,
  `keywords` LONGTEXT NULL DEFAULT NULL,
  `keywordt` LONGTEXT NULL DEFAULT NULL,
  `loopscsv` LONGTEXT NULL DEFAULT NULL,
  `customdatajson` LONGTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`id`));


-- -----------------------------------------------------
-- Table `alarmworkflow`.`operationresource`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `alarmworkflow`.`operationresource` ;

CREATE TABLE IF NOT EXISTS `alarmworkflow`.`operationresource` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `operation_id` INT(11) NOT NULL,
  `timestamp` LONGTEXT NULL DEFAULT NULL,
  `fullname` LONGTEXT NULL DEFAULT NULL,
  `equipmentcsv` LONGTEXT NULL DEFAULT NULL,
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
  `identifier` LONGTEXT NOT NULL,
  `name` LONGTEXT NOT NULL,
  `value` LONGTEXT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `alarmworkflow`.`dispresource`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `alarmworkflow`.`dispresource` ;

CREATE TABLE IF NOT EXISTS `alarmworkflow`.`dispresource` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `operation_id` INT(11) NOT NULL,
  `timestamp` DATETIME NOT NULL,
  `emkresourceid` LONGTEXT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_dispresource_operation1_idx` (`operation_id` ASC),
  CONSTRAINT `fk_dispresource_operation1`
    FOREIGN KEY (`operation_id`)
    REFERENCES `alarmworkflow`.`operation` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
