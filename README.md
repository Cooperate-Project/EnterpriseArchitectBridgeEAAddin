# IncrementalDBSync

[![Travis CI](https://img.shields.io/travis/Cooperate-Project/EnterpriseArchitectBridgeEAAddin.svg)](https://travis-ci.org/Cooperate-Project/EnterpriseArchitectBridgeEAAddin)
[![Bintray](https://img.shields.io/bintray/v/cooperate-project/EnterpriseArchitectBridgeEAAddin/installers.svg)](https://dl.bintray.com/cooperate-project/EnterpriseArchitectBridgeEAAddin/installers)
[![Issues](https://img.shields.io/github/issues/Cooperate-Project/EnterpriseArchitectBridgeEAAddin.svg)](https://github.com/Cooperate-Project/EnterpriseArchitectBridgeEAAddin/issues)
[![License](https://img.shields.io/github/license/Cooperate-Project/EnterpriseArchitectBridgeEAAddin.svg)](https://raw.githubusercontent.com/Cooperate-Project/EnterpriseArchitectBridgeEAAddin/master/LICENSE)

This is an Add-in for Enterprise Architect. It enables automatic refreshing of changes in the database by repeatedly polling logging tables.
These logging tables can be created by using a generator like [IncrementalDBSyncUtils](https://github.com/Cooperate-Project/IncrementalDBSyncUtils).

## Installation
Download the installer from our [https://dl.bintray.com/cooperate-project/EnterpriseArchitectBridgeEAAddin/installers](download repository). Choose the installer from the `latest` folder for up-to-date development versions and all other folders for stable release versions. Follow the instructions of the installer.

##Usage

To use this addin, first, use a active database and prepare it with triggers and logging tables, e.g. generated by [IncrementalDBSyncUtils](https://github.com/Cooperate-Project/IncrementalDBSyncUtils).
Then follow the steps [described in the wiki](https://github.com/Cooperate-Project/IncrementalDBSync/wiki/Enterprise-Architect-Addins) to build and enable the add-in.
