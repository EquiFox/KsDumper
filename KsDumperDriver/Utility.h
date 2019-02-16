#pragma once
#include <ntddk.h>

NTSTATUS DriverSleep(int ms);

PVOID SanitizeUserPointer(PVOID pointer, SIZE_T size);