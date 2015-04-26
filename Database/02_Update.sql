USE `alarmworkflow`;

ALTER TABLE `operation` MODIFY `operationnumber` LONGTEXT;
ALTER TABLE `operation` MODIFY `messenger` LONGTEXT;
ALTER TABLE `operation` MODIFY `comment` LONGTEXT;
ALTER TABLE `operation` MODIFY `plan` LONGTEXT;
ALTER TABLE `operation` MODIFY `picture` LONGTEXT;
ALTER TABLE `operation` MODIFY `priority` LONGTEXT;
ALTER TABLE `operation` MODIFY `einsatzortstreet` LONGTEXT;
ALTER TABLE `operation` MODIFY `einsatzortstreetnumber` LONGTEXT;
ALTER TABLE `operation` MODIFY `einsatzortzipcode` LONGTEXT;
ALTER TABLE `operation` MODIFY `einsatzortcity` LONGTEXT;
ALTER TABLE `operation` MODIFY `einsatzortintersection` LONGTEXT;
ALTER TABLE `operation` MODIFY `einsatzortlocation` LONGTEXT;
ALTER TABLE `operation` MODIFY `einsatzortproperty` LONGTEXT;
ALTER TABLE `operation` MODIFY `einsatzortlatlng` LONGTEXT;
ALTER TABLE `operation` MODIFY `zielortstreet` LONGTEXT;
ALTER TABLE `operation` MODIFY `zielortstreetnumber` LONGTEXT;
ALTER TABLE `operation` MODIFY `zielortzipcode` LONGTEXT;
ALTER TABLE `operation` MODIFY `zielortcity` LONGTEXT;
ALTER TABLE `operation` MODIFY `zielortintersection` LONGTEXT;
ALTER TABLE `operation` MODIFY `zielortlocation` LONGTEXT;
ALTER TABLE `operation` MODIFY `zielortproperty` LONGTEXT;
ALTER TABLE `operation` MODIFY `zielortlatlng` LONGTEXT;
ALTER TABLE `operation` MODIFY `keyword` LONGTEXT;
ALTER TABLE `operation` MODIFY `keywordmisc` LONGTEXT;
ALTER TABLE `operation` MODIFY `keywordb` LONGTEXT;
ALTER TABLE `operation` MODIFY `keywordr` LONGTEXT;
ALTER TABLE `operation` MODIFY `keywords` LONGTEXT;
ALTER TABLE `operation` MODIFY `keywordt` LONGTEXT;
ALTER TABLE `operation` MODIFY `loopscsv` LONGTEXT;

ALTER TABLE `operationresource` MODIFY `timestamp` LONGTEXT;
ALTER TABLE `operationresource` MODIFY `fullname` LONGTEXT;
ALTER TABLE `operationresource` MODIFY `equipmentcsv` LONGTEXT;

ALTER TABLE `usersetting` MODIFY `identifier` LONGTEXT;
ALTER TABLE `usersetting` MODIFY `name` LONGTEXT;

ALTER TABLE `dispresource` MODIFY `emkresourceid` LONGTEXT;

-- Set unicode for all database objects
ALTER SCHEMA `alarmworkflow` DEFAULT CHARACTER SET utf8mb4 DEFAULT COLLATE utf8mb4_unicode_ci;

ALTER TABLE `operation` CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE `operationresource` CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE `usersetting` CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE `dispresource` CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
