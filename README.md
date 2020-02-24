# Fougerite project

## About the Project
Fougerite is a "fork" or conversion of Magma that was created by EquiFox and xEnt, back in 2013 after furnace mod.
The project however became abandoned in 2014, and has been decompiled, and refactored by Riketta.
Later on Alex, balu92, mikec, DreTaX has joined the project, and the mod was renamed from Zumwalt to
Fougerite. On 19th of November, 2014 the contributions have been undertaken by Team Pluton.
Fougerite provides not only a modding platform for the original Rust Legacy Server, but also provides
many bugfixes that were left over by the original developer team.
There is also a client side client available for everyone to use, and develop on called RustBuster.
Fougerite provides C#, Python, Javascript, and Lua language support, and also an easy to use API.

Fougerite a fully compatible with Magma server mod, featuring better performance with Python and C# plugins.

## Compilation
1. First you need to decide wheather you are going to modify the patcher or not. If you are only here to modify or compile
the Fougerite project, or one of the engines skip to step 7.
2. Open the SLN, and compile the Fougerite patcher.
3. Please go to the Fougerite\References\CleanPatchTargetDlls\ directory, and read the ReadMe.txt file.
4. Select the 3 dlls that you are going to patch (Assembly-CSharp.dll, uLink.dll, Facepunch.MeshBatch.dll)
, and copy to the patcher's output directory. You may need other files as reference
such as UnityEngine.dll
5. Run the patcher, and enter 0. If all is well, then the dlls that you have copied to the directory are now patched.
If something went wrong, try to find out from the patcher's logs.
6. Copy the 3 patched dlls to \Fougerite\References\PatchedRustDlls\ directory, and overwrite the existing ones.
7. You can now build all the projects using JetBrains Rider, or Visual Studio as you like.

## Installation
Please see a release file from the releases tab to see how a release file should look like.
The release file also contains extra Python files, that you may want to grab.
It's in the Save\Lib folder. Not all of the python files can be used due to the version of 
mono or IronPython. The issue has been resolved on a newer IronPython version, however when I tried It
It didn't work. (The issue page doesn't exist, they moved to github.)
Once you have copied all of your files accordingly, you may run your server. (You also have to overwrite some server files)
There is also a clean steam server available on this repository for you to download.

Use Git Issues system to report bugs, please. 
Please visit [our forum](http://fougerite.com/) for more information.

***
###### Developed by EquiFox & xEnt (Rust++ and Magma)
###### Forked by Riketta (Zumwalt Project)
###### Renamed by Alexknvl (from "Zumwalt" to "Fougerite")
###### 19-NOV-2014: Contributions and on-going maintenance undertaken by Team Pluton
