---------------------------------------------------------------

Hier das SQL Script f�r die Datenbank von Alarmworkflow. Das Script einfach Kopieren und z.B. bei einem Xampp System im PHP MY ADMIN unter SQL einf�gen und mit OK ausf�hren lassen. Im Anschluss m�sste eine neue Datenbank vorhanden sein mit dem Namen Alarmworkflow und der Tabelle tb_einstaz.

CREATE DATABASE `alarmworkflow` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
USE `alarmworkflow`;

� �������������������

�
� Tabellenstruktur f�r Tabelle `tb_einsatz`
�

CREATE TABLE IF NOT EXISTS `tb_einsatz` (
  `id` int(11) NOT NULL auto_increment,
  `Einsatznr` varchar(255) default NULL,
  `Einsatzort` varchar(255) default NULL,
  `Einsatzplan` varchar(255) default NULL,
  `Hinweis` varchar(255) default NULL,
  `Kreuzung` varchar(255) default NULL,
  `Meldebild` varchar(255) default NULL,
  `Mitteiler` varchar(255) default NULL,
  `Objekt` varchar(255) default NULL,
  `Ort` varchar(255) default NULL,
  `Strasse` varchar(255) default NULL,
  `Fahrzeuge` varchar(255) default NULL,
  `Einsatzstichwort` varchar(255) default NULL,
  `Alarmtime` varchar(255) default NULL,  
  `Faxtime` varchar(255) default NULL,
  `Stichwort` varchar(20) default NULL,
  PRIMARY KEY  (`id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=129 ;