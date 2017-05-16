#!/bin/bash

# Build FlowTesting Library, then Sample Chat application and then its test suite
(cd ../FlowTestingLibrary/ && xbuild) && (cd ../SampleChatApplication/ && xbuild) && (cd ../SampleChatServerFlowTestSuite/ && xbuild) && mono ../ThirdPartyLib/NUnit.Console-3.6.1/nunit3-console.exe ../SampleChatServerFlowTestSuite/SampleChatServerTests/bin/Debug/SampleChatServerTests.dll --labels=On
