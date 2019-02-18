# KsDumper
![Demo](https://i.imgur.com/6XyMDxa.gif)

I always had an interest in reverse engineering. A few days ago I wanted to look at some game internals for fun, but it was packed & protected by EAC (EasyAntiCheat).
This means its handle were stripped and I was unable to dump the process from Ring3. I decided to try to make a custom driver that would allow me to copy the process memory without using OpenProcess.
I knew nothing about Windows kernel, PE file structure, so I spent a lot of time reading articles and forums to make this project.

## Features
- Dump any process main module using a kernel driver (both x86 and x64)
- Rebuild PE32/PE64 header and sections
- Works on protected system processes & processes with stripped handles (anti-cheats)

**Note**: Import table isn't rebuilt.

## Usage
Before using KsDumperClient, the KsDumper driver needs to be loaded.

It is unsigned so you need to load it however you want. I'm using drvmap for Win10.
Everything is provided in this release if you want to use it aswell.

- Run `Driver/LoadCapcom.bat` as Admin. Don't press any key or close the window yet !
- Run `Driver/LoadUnsignedDriver.bat` as Admin.
- Press enter in the `LoadCapcom` cmd to unload the driver.
- Run `KsDumperClient.exe`.
- Profit !

**Note**: The driver stays loaded until you reboot, so if you close KsDumperClient.exe, you can just reopen it !  
**Note2**: Even though it can dump both x86 & x64 processes, this has to run on x64 Windows.

## Disclaimer
This project was a way for me to learn about Windows kernel, PE file structure and kernel-user space interactions. It has been made available for informational and educational purposes only.

Considering the nature of this project, it is highly recommended to run it in a `Virtual Environment`. I am not responsible for any crash or damage that could happen to your system.

**Important**: This tool makes no attempt at hiding itself. If you target protected games, the anti-cheat might flag this as a cheat and ban you after a while. Use a `Virtual Environment` !

## References
- https://github.com/not-wlan/drvmap
- https://github.com/Zer0Mem0ry/KernelBhop
- https://github.com/NtQuery/Scylla/
- http://terminus.rewolf.pl/terminus/
- https://www.unknowncheats.me/

## Compile Yourself
- Requires Visual Studio 2017
- Requires Windows Driver Kit (WDK)
- Requires .NET 4.6.1
