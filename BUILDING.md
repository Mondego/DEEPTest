# Building on Windows

Steps:
 * runprebuild.bat
 * Load FlowTest.sln into Visual Studio .NET and build the solution.

# Building on Linux

Prereqs:
*	Mono >= 2.4.3
*	Nant >= 0.85
*	May use xbuild (included in mono distributions)
*	May use Monodevelop, a cross-platform IDE

From the distribution type:
 * ./runprebuild.sh
 * nant (or !* xbuild)
 
 !* xbuild option switches
 !*          clean:  xbuild /target:clean
 !*          debug: (default) xbuild /property:Configuration=Debug
 !*          release: xbuild /property:Configuration=Release

# Using Monodevelop

From the distribution type:
 * ./runprebuild.sh
 * type monodevelop FlowTest.sln
