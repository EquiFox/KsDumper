#pragma once
#include <ntddk.h>

#pragma pack(push, 1)
typedef struct _PROCESS_SUMMARY
{
	INT32 ProcessId;
	PVOID MainModuleBase;
	WCHAR MainModuleFileName[256];
	UINT32 MainModuleImageSize;
	PVOID MainModuleEntryPoint;
	BOOLEAN WOW64;
} PROCESS_SUMMARY, *PPROCESS_SUMMARY;
#pragma pack(pop)

typedef struct _SYSTEM_PROCESS_INFORMATION
{
	ULONG NextEntryOffset;
	ULONG NumberOfThreads;
	LARGE_INTEGER SpareLi1;
	LARGE_INTEGER SpareLi2;
	LARGE_INTEGER SpareLi3;
	LARGE_INTEGER CreateTime;
	LARGE_INTEGER UserTime;
	LARGE_INTEGER KernelTime;
	UNICODE_STRING ImageName;
	KPRIORITY BasePriority;
	HANDLE UniqueProcessId;
	HANDLE InheritedFromUniqueProcessId;
	ULONG HandleCount;
	ULONG SessionId;
	ULONG_PTR PageDirectoryBase;
	SIZE_T PeakVirtualSize;
	SIZE_T VirtualSize;
	ULONG PageFaultCount;
	SIZE_T PeakWorkingSetSize;
	SIZE_T WorkingSetSize;
	SIZE_T QuotaPeakPagedPoolUsage;
	SIZE_T QuotaPagedPoolUsage;
	SIZE_T QuotaPeakNonPagedPoolUsage;
	SIZE_T QuotaNonPagedPoolUsage;
	SIZE_T PagefileUsage;
	SIZE_T PeakPagefileUsage;
	SIZE_T PrivatePageCount;
	LARGE_INTEGER ReadOperationCount;
	LARGE_INTEGER WriteOperationCount;
	LARGE_INTEGER OtherOperationCount;
	LARGE_INTEGER ReadTransferCount;
	LARGE_INTEGER WriteTransferCount;
	LARGE_INTEGER OtherTransferCount;
} SYSTEM_PROCESS_INFORMATION, *PSYSTEM_PROCESS_INFORMATION;

typedef struct _LDR_DATA_TABLE_ENTRY
{
	LIST_ENTRY InLoadOrderLinks;
	LIST_ENTRY InMemoryOrderLinks;
	CHAR Reserved0[0x10];
	PVOID DllBase;
	PVOID EntryPoint;
	ULONG SizeOfImage;
	UNICODE_STRING FullDllName;
	UNICODE_STRING BaseDllName;
} LDR_DATA_TABLE_ENTRY, *PLDR_DATA_TABLE_ENTRY;

typedef struct _PEB_LDR_DATA
{
	ULONG Length;
	BOOLEAN Initialized;
	PVOID SsHandler;
	LIST_ENTRY InLoadOrderModuleList;
	LIST_ENTRY InMemoryOrderModuleList;
	LIST_ENTRY InInitializationOrderModuleList;
	PVOID EntryInProgress;
} PEB_LDR_DATA, *PPEB_LDR_DATA;

typedef struct _PEB64 {
	CHAR Reserved[0x10];
	PVOID ImageBaseAddress;
	PPEB_LDR_DATA Ldr;
} PEB64, *PPEB64;

typedef struct _IMAGE_DOS_HEADER {
	USHORT   e_magic;
	USHORT   e_cblp;
	USHORT   e_cp;
	USHORT   e_crlc;
	USHORT   e_cparhdr;
	USHORT   e_minalloc;
	USHORT   e_maxalloc;
	USHORT   e_ss;
	USHORT   e_sp;
	USHORT   e_csum;
	USHORT   e_ip;
	USHORT   e_cs;
	USHORT   e_lfarlc;
	USHORT   e_ovno;
	USHORT   e_res[4];
	USHORT   e_oemid;
	USHORT   e_oeminfo;
	USHORT   e_res2[10];
	LONG   e_lfanew;
} IMAGE_DOS_HEADER, *PIMAGE_DOS_HEADER;

typedef struct _PE_HEADER {
	CHAR Signature[4];
	USHORT Machine;
	USHORT NumberOfSections;
	UINT32 TimeDateStamp;
	UINT32 PointerToSymbolTable;
	UINT32 NumberOfSymbols;
	USHORT SizeOfOptionalHeader;
	USHORT Characteristics;
	USHORT Magic;
} PE_HEADER, *PPE_HEADER;

#define PE_HEADER_MAGIC_OFFSET 0x18
#define IMAGE_NT_OPTIONAL_HDR32_MAGIC 0x10b

#define IS_WOW64_PE( baseAddress ) (*((USHORT*)((CHAR *)baseAddress + \
								((PIMAGE_DOS_HEADER)baseAddress)->e_lfanew + PE_HEADER_MAGIC_OFFSET)) \
								== IMAGE_NT_OPTIONAL_HDR32_MAGIC)

NTSTATUS GetProcessList(PVOID listedProcessBuffer, INT32 bufferSize, PINT32 requiredBufferSize, PINT32 processCount);