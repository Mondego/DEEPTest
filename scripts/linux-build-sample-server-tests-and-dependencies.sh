#!/bin/bash

# Build FlowTesting Library, then Sample Chat application and then its test suite
(cd .. && msbuild /t:clean && msbuild) && mono ../ThirdPartyLib/NUnit.Console-3.6.1/nunit3-console.exe ../Samples/Chat/ChatTestSuite/bin/Debug/ChatTestSuite.dll --labels=On --noresult
