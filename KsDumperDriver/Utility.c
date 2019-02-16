#include "NTUndocumented.h"
#include "Utility.h"

NTSTATUS DriverSleep(int ms)
{
	LARGE_INTEGER li;
	li.QuadPart = -10000;

	for (int i = 0; i < ms; i++)
	{
		KeDelayExecutionThread(KernelMode, FALSE, &li);
		return STATUS_SUCCESS;
	}
	return STATUS_UNSUCCESSFUL;
}

PVOID SanitizeUserPointer(PVOID pointer, SIZE_T size)
{
	MEMORY_BASIC_INFORMATION memInfo;

	if (NT_SUCCESS(ZwQueryVirtualMemory(ZwCurrentProcess(), pointer, MemoryBasicInformation, &memInfo, sizeof(MEMORY_BASIC_INFORMATION), NULL)))
	{
		if (!(((uintptr_t)memInfo.BaseAddress + memInfo.RegionSize) < (((uintptr_t)pointer + size))))
		{
			if (memInfo.State & MEM_COMMIT || !(memInfo.Protect & (PAGE_GUARD | PAGE_NOACCESS)))
			{
				if (memInfo.Protect & PAGE_EXECUTE_READWRITE || memInfo.Protect & PAGE_EXECUTE_WRITECOPY || memInfo.Protect & PAGE_READWRITE || memInfo.Protect & PAGE_WRITECOPY)
				{
					return pointer;
				}
			}
		}
	}
	return NULL;
}
