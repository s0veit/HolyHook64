# Quake 3 Arena - Pure C# Internal Base

|-> zero-SHITAPI internal hook and packet manipulator for quake 3, written entirely in pure c# (no pure winapi, why the fuck would you even brag about that v952? dambass) 

|-> raw x64 assembly trampoline jumpist and network packet manipulation without any cpp or external memory wrappers  
|-> utilizes stack allocations and intermediate language level unsafe memblocks -> no heap alloc, no gc fuckage

## Features

* **manual x64 trampoline hook:** hooks engine functions (like `CL_CreateCmd` in the example code given) dynamically in memory by allocating pages and writing raw __asm jmps with return jmps handling
* **packet manipulation:** intercepts and modifies the `usercmd_t` network packet before its sent to the server
* **bunnyhop code example:** overrides the engines `upmove` to achieve a working bunnyhop, comments included on how to make it tickperfect to keep gainign velocity
* **struct alignment & memory safety:** utilizes packing to mirror the original quake c compiler networking optimizations, explains how it works and why it works
* **unique architecture:** hooking logic of (`_ASMHOOK`) - completely bypasses standard array arithmetic and uses raw pointer math and unsafe blocks
*	*-> execution shall thus be as fast as cpp (dont quote me)

## Injection & Usage

|-> compiles into native dll
	|-> you will need a standard dll injector to attach it to the process
		|-> all development and testing was done on `ioquake3.x86_64.exe`

### IMPORTANT: Injectors
|-> this is a c# dll utilizing `[UnmanagedCallersOnly]`, the injector needs to know what to execute first

|-> all of my testing was done with Xenos, which requires the following steps:  
|	1. add the native dll  
|	2. go to advanced  
|	3. set init routine to: `Attach`  

|-> i assume most other injectors would work similarly  

## Structure

* `EntryPoint.cs` - main payload and hook callback with the packet manipulation logic
* `_ASMHOOK.cs` - raw memory allocator, unmanaged pointer arithmetics, and x64 asm trampoline builder
* `Structs.cs` - structs perfectly aligned to the engine
	*	|-> the reason structs and offsets dont follow the same naming scheme is because structs is basically a 'copy' of the q3 source code, just snaked (unless it makes sense to not do so)
* `_OFFSETS.cs` - global static class of engine pointers, func limiting, and game state consts

## Nutshell
|-> this repository should be a resource for game engine architecture, memory alignment, and x64 assembly hooking via c# (with STILL no asm blocks in 2026)  
|-> and yes there is a syscall and a single console alloc for debug purposes fuck you if you think thats 'using winapi for this project'. just get rid of the window if you hate it. bitch.  

---
