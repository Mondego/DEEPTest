#!/bin/bash

(cd .. && xbuild) && mono ../packages/NUnit.ConsoleRunner.3.6.1/tools/nunit3-console.exe ../SampleServerTests/bin/Debug/SampleServerTests.dll --labels=On /fixture:SampleServerTests.Test_WeaveCustomSleepCode
