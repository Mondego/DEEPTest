#!/bin/bash

(cd /home/eugenia/Projects/opensim && nant clean && nant) && (cd .. && xbuild /t:clean && xbuild) && mono ../ThirdPartyLib/NUnit.Console-3.6.1/nunit3-console.exe ../OpenSimTestSuiteBackupProject/bin/Debug/OpenSimTestSuiteBackupProject.dll --labels=On --noresult --where "class == OpenSimTestSuiteBackupProject.Test_Login_pCampBot"
