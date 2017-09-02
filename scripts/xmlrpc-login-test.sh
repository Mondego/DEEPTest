#!/bin/bash

echo "Building OpenSim..."
(cd /home/eugenia/Projects/opensim && nant clean && nant) > /dev/null
if [ $? -eq 0 ]; then
    echo "OK"
else
    echo "Failed. Exiting."
fi

echo "Building OpenSim FlowTest example"
(cd .. && xbuild /t:clean && xbuild) > /dev/null 
if [ $? -eq 0 ]; then
    echo "OK"
else
    echo "Failed. Exiting."
fi

mono ../ThirdPartyLib/NUnit.Console-3.6.1/nunit3-console.exe ../OpenSimTestSuiteBackupProject/bin/Debug/OpenSimTestSuiteBackupProject.dll --labels=On --noresult --where "class == OpenSimTestSuiteBackupProject.Test_Login_XMLRPC"
