_Please ensure that [pre-requisites](prerequisites-for-building.md) are installed for a successful build of the repo._

# Build ILCompiler #

Build your repo by issuing the following command at repo root:

```
build.cmd
```

_Note_:

* Instructions below assume `c:\corert` is the repo root.

## Using RyuJIT ##

1. Open c:\corert\src\ILCompiler\ILCompiler.sln in VS

  - Set "desktop" project in solution explorer as your startup project

  - Set startup command line to:
`c:\corert\src\ILCompiler\repro\bin\Debug\repro.exe -r c:\corert\bin\Product\Windows_NT.x64.Debug\System.Private.CoreLib.dll -r C:\corert\bin\Product\Windows_NT.x64.Debug\.nuget\publish1\toolchain.win7-x64.Microsoft.DotNet.AppDep.1.0.4-prerelease-00001\*.dll -out c:\corert\src\ILCompiler\reproNative\repro.obj`

  - Build & run using **F5**
    - This will run the compiler. The output is `c:\corert\src\ILCompiler\reproNative\repro.obj` file.

  - The repro project has a dummy program that you can modify for ad-hoc testing

  - To suppress spew from NuGet during the build, go to NuGet Package Manager in Options, and uncheck `Allow NuGet to download missing packages`.

2. Open `c:\corert\src\ILCompiler\reproNative\reproNative.vcxproj`

  - Set breakpoint at ```__managed__Main``` in main.cpp
  - Build & run using **F5**
    - Once you hit the breakpoint, go to disassembly and step into - you are looking at the code generated by RyuJIT


## Using CPP Code Generator ##

1. Open `c:\corert\src\ILCompiler\ILCompiler.sln` in VS

  - Set "desktop" project in solution explorer as your startup project

  - Set startup command line to:
`c:\corert\src\ILCompiler\repro\bin\Debug\repro.exe -r c:\corert\bin\Product\Windows_NT.x64.Debug\System.Private.CoreLib.dll -r C:\corert\bin\Product\Windows_NT.x64.Debug\.nuget\publish1\toolchain.win7-x64.Microsoft.DotNet.AppDep.1.0.4-prerelease-00001\*.dll -out c:\corert\src\ILCompiler\reproNative\repro.cpp -cpp`

    - `-nolinenumbers` command line option can be used to suppress generation of line number mappings in C++ files - useful for debugging

  - Build & run using **F5**
    - This will run the compiler. The output is `c:\corert\src\ILCompiler\reproNative\repro.cpp` file.

  - The repro project has a dummy program that you can modify for ad-hoc testing

2. Open `c:\corert\src\ILCompiler\reproNative\reproNativeCpp.vcxproj`

  - Set breakpoint at repro::Program::Main in main.cpp
  - Build, run & step through as with any other C++ program
