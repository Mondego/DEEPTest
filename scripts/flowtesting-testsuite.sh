#!/bin/bash

# Build FlowTesting Library and run test
(cd .. && msbuild /t:clean /verbosity:minimal && msbuild /verbosity:minimal) && mono ../ThirdPartyLib/NUnit.Console-3.6.1/nunit3-console.exe ../Test/bin/Debug/Test.dll --labels=On --noresult
