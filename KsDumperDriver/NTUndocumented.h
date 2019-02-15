#pragma once
#include <ntddk.h>

typedef struct _KAPC_STATE {
	LIST_ENTRY ApcListHead[MaximumMode];
	struct _KPROCESS *Process;
	BOOLEAN KernelApcInProgress;
	BOOLEAN KernelApcPending;
	BOOLEAN UserApcPending;
} KAPC_STATE, *PKAPC_STATE, *PRKAPC_STATE;

typedef enum _SYSTEM_INFORMATION_CLASS
{
	SystemProcessInformation = 5
} SYSTEM_INFORMATION_CLASS;

NTKERNELAPI NTSTATUS IoCreateDriver(IN PUNICODE_STRING DriverName, OPTIONAL IN PDRIVER_INITIALIZE InitializationFunction);

NTKERNELAPI VOID KeStackAttachProcess(__inout struct _KPROCESS * PROCESS, __out PRKAPC_STATE ApcState);
NTKERNELAPI VOID KeUnstackDetachProcess(__in PRKAPC_STATE ApcState);

NTKERNELAPI NTSTATUS NTAPI MmCopyVirtualMemory(IN PEPROCESS FromProcess, IN PVOID FromAddress, IN PEPROCESS ToProcess, OUT PVOID ToAddress, IN SIZE_T BufferSize, IN KPROCESSOR_MODE PreviousMode, OUT PSIZE_T NumberOfBytesCopied);

NTSYSAPI NTSTATUS NTAPI ZwQuerySystemInformation(IN SYSTEM_INFORMATION_CLASS SystemInformationClass, OUT PVOID SystemInformation, IN ULONG SystemInformationLength, OUT PULONG ReturnLength OPTIONAL);

NTKERNELAPI NTSTATUS PsLookupProcessByProcessId(IN HANDLE ProcessId, OUT PEPROCESS *Process);
NTKERNELAPI PVOID PsGetProcessSectionBaseAddress(__in PEPROCESS Process);
NTKERNELAPI PPEB NTAPI PsGetProcessPeb(IN PEPROCESS Process);