#!/bin/bash

Samples/Chat/ChatTestSuite/bin/Debug/ChatTestSuite.dll


# Build FlowTesting Library, then Sample Chat application and then its test suite
(cd .. && xbuild /t:clean && xbuild) && mono ../ThirdPartyLib/NUnit.Console-3.6.1/nunit3-console.exe ../Samples/Chat/ChatTestSuite/bin/Debug/ChatTestSuite.dll --labels=On
