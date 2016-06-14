-- phpMyAdmin SQL Dump
-- version 4.5.1
-- http://www.phpmyadmin.net
--
-- Client :  127.0.0.1
-- Généré le :  Lun 29 Février 2016 à 12:14
-- Version du serveur :  10.1.10-MariaDB
-- Version de PHP :  5.6.15

DROP DATABASE IF EXISTS `mli_prod`;

CREATE DATABASE `mli_prod`;

USE `mli_prod`;

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de données :  `mli`
--

-- --------------------------------------------------------

--
-- Structure de la table `amorces`
--

CREATE TABLE `amorces` (
  `ID_AMORCE` int(11) NOT NULL,
  `RNAME` varchar(30) NOT NULL,
  `FLAG` int(11) NOT NULL,
  `LNG` int(11) NOT NULL,
  `POS` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

ALTER TABLE `amorces` ADD COLUMN NOM varchar(30);
-- --------------------------------------------------------

--
-- Structure de la table `individu`
--

CREATE TABLE `individu` (
  `ID_INDIVIDU` int(11) NOT NULL,
  `LIBELLE` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `read_options`
--

CREATE TABLE `read_options` (
  `ID_READ_OPTIONS` int(11) NOT NULL,
  `ID_SEQUENCE_READ` int(11) NOT NULL,
  `VALEUR` varchar(200) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `sequence`
--

CREATE TABLE `sequence` (
  `ID_SEQUENCE` int(11) NOT NULL,
  `ID_INDIVIDU` int(11) NOT NULL,
  `RNAME` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `sequence_read`
--

CREATE TABLE `sequence_read` (
  `ID_SEQUENCE_READ` int(11) NOT NULL,
  `ID_SEQUENCE` int(11) NOT NULL,
  `QNAME` varchar(45) NOT NULL,
  `FLAG` int(11) NOT NULL,
  `POS` int(11) NOT NULL,
  `MAPQ` int(11) NOT NULL,
  `CIGAR` varchar(40) NOT NULL,
  `MRNM` int(11) NOT NULL COMMENT '= ID_SEQUENCE',
  `MPOS` int(11) NOT NULL,
  `ISIZE` int(11) NOT NULL,
  `SEQ` varchar(350) NOT NULL,
  `QUAL` varchar(350) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Index pour les tables exportées
--

--
-- Index pour la table `amorces`
--
ALTER TABLE `amorces`
  ADD PRIMARY KEY (`ID_AMORCE`);

--
-- Index pour la table `individu`
--
ALTER TABLE `individu`
  ADD PRIMARY KEY (`ID_INDIVIDU`);

--
-- Index pour la table `read_options`
--
ALTER TABLE `read_options`
  ADD PRIMARY KEY (`ID_READ_OPTIONS`),
  ADD KEY `ID_SEQUENCE_READ` (`ID_SEQUENCE_READ`);

--
-- Index pour la table `sequence`
--
ALTER TABLE `sequence`
  ADD PRIMARY KEY (`ID_SEQUENCE`),
  ADD KEY `ID_INDIVIDU` (`ID_INDIVIDU`);

--
-- Index pour la table `sequence_read`
--
ALTER TABLE `sequence_read`
  ADD PRIMARY KEY (`ID_SEQUENCE_READ`),
  ADD KEY `ID_SEQUENCE` (`ID_SEQUENCE`);

--
-- AUTO_INCREMENT pour les tables exportées
--

--
-- AUTO_INCREMENT pour la table `amorces`
--
ALTER TABLE `amorces`
  MODIFY `ID_AMORCE` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT pour la table `individu`
--
ALTER TABLE `individu`
  MODIFY `ID_INDIVIDU` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT pour la table `read_options`
--
ALTER TABLE `read_options`
  MODIFY `ID_READ_OPTIONS` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT pour la table `sequence`
--
ALTER TABLE `sequence`
  MODIFY `ID_SEQUENCE` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT pour la table `sequence_read`
--
ALTER TABLE `sequence_read`
  MODIFY `ID_SEQUENCE_READ` int(11) NOT NULL AUTO_INCREMENT;
--
-- Contraintes pour les tables exportées
--

--
-- Contraintes pour la table `read_options`
--
ALTER TABLE `read_options`
  ADD CONSTRAINT `read_options_ibfk_1` FOREIGN KEY (`ID_SEQUENCE_READ`) REFERENCES `sequence_read` (`ID_SEQUENCE_READ`) ON DELETE CASCADE;

--
-- Contraintes pour la table `sequence`
--
ALTER TABLE `sequence`
  ADD CONSTRAINT `sequence_ibfk_1` FOREIGN KEY (`ID_INDIVIDU`) REFERENCES `individu` (`ID_INDIVIDU`) ON DELETE CASCADE;

--
-- Contraintes pour la table `sequence_read`
--
ALTER TABLE `sequence_read`
  ADD CONSTRAINT `sequence_read_ibfk_1` FOREIGN KEY (`ID_SEQUENCE`) REFERENCES `sequence` (`ID_SEQUENCE`) ON DELETE CASCADE;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
