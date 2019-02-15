#include "NTUndocumented.h"
#include "ProcessLister.h"
#include "UserModeBridge.h"
#include <wdf.h>

DRIVER_INITIALIZE DriverEntry;
#pragma alloc_text(INIT, DriverEntry)

NTSTATUS CopyVirtualMemory(PEPROCESS targetProcess, PVOID sourceAddress, PVOID targetAddress, SIZE_T size)
{
	PSIZE_T readBytes;
	return MmCopyVirtualMemory(targetProcess, sourceAddress, PsGetCurrentProcess(), targetAddress, size, UserMode, &readBytes);
}

NTSTATUS IoControl(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
	NTSTATUS Status;
	ULONG BytesIO = 0;
	PIO_STACK_LOCATION stack = IoGetCurrentIrpStackLocation(Irp);
	ULONG ControlCode = stack->Parameters.DeviceIoControl.IoControlCode;

	if (ControlCode == IO_COPY_MEMORY)
	{
		PKERNEL_COPY_MEMORY_OPERATION request = (PKERNEL_COPY_MEMORY_OPERATION)Irp->AssociatedIrp.SystemBuffer;
		PEPROCESS targetProcess;		

		if (NT_SUCCESS(PsLookupProcessByProcessId(request->targetProcessId, &targetProcess)))
		{
			CopyVirtualMemory(targetProcess, request->targetAddress, request->bufferAddress, request->bufferSize);
		}

		Status = STATUS_SUCCESS;
		BytesIO = sizeof(KERNEL_COPY_MEMORY_OPERATION);
	}
	else if (ControlCode == IO_GET_PROCESS_LIST)
	{
		PKERNEL_PROCESS_LIST_OPERATION request = (PKERNEL_PROCESS_LIST_OPERATION)Irp->AssociatedIrp.SystemBuffer;

		GetProcessList(request->bufferAddress, request->bufferSize, &request->bufferSize, &request->processCount);

		Status = STATUS_SUCCESS;
		BytesIO = sizeof(KERNEL_PROCESS_LIST_OPERATION);
	}
	else
	{
		Status = STATUS_INVALID_PARAMETER;
		BytesIO = 0;
	}

	Irp->IoStatus.Status = Status;
	Irp->IoStatus.Information = BytesIO;
	IoCompleteRequest(Irp, IO_NO_INCREMENT);

	return Status;
}

NTSTATUS UnsupportedDispatch(_In_ struct _DEVICE_OBJECT *DeviceObject, _Inout_ struct _IRP *Irp)
{
	UNREFERENCED_PARAMETER(DeviceObject);

	Irp->IoStatus.Status = STATUS_NOT_SUPPORTED;
	IoCompleteRequest(Irp, IO_NO_INCREMENT);
	return Irp->IoStatus.Status;
}

NTSTATUS CreateDispatch(_In_ struct _DEVICE_OBJECT *DeviceObject, _Inout_ struct _IRP *Irp)
{
	UNREFERENCED_PARAMETER(DeviceObject);

	IoCompleteRequest(Irp, IO_NO_INCREMENT);
	return Irp->IoStatus.Status;
}

NTSTATUS CloseDispatch(_In_ struct _DEVICE_OBJECT *DeviceObject, _Inout_ struct _IRP *Irp)
{
	UNREFERENCED_PARAMETER(DeviceObject);

	IoCompleteRequest(Irp, IO_NO_INCREMENT);
	return Irp->IoStatus.Status;
}

NTSTATUS DriverInitialize(_In_  struct _DRIVER_OBJECT *DriverObject, _In_  PUNICODE_STRING RegistryPath)
{
	NTSTATUS status;
	UNICODE_STRING deviceName, symLink;
	PDEVICE_OBJECT  deviceObject;

	UNREFERENCED_PARAMETER(RegistryPath);

	RtlInitUnicodeString(&deviceName, L"\\Device\\KsDumper");
	RtlInitUnicodeString(&symLink, L"\\DosDevices\\KsDumper");

	status = IoCreateDevice(DriverObject, 0, &deviceName, FILE_DEVICE_UNKNOWN, FILE_DEVICE_SECURE_OPEN, FALSE, &deviceObject);

	if (!NT_SUCCESS(status))
	{
		return status;
	}
	status = IoCreateSymbolicLink(&symLink, &deviceName);

	if (!NT_SUCCESS(status))
	{
		return status;
	}
	deviceObject->Flags |= DO_BUFFERED_IO;

	for (ULONG t = 0; t <= IRP_MJ_MAXIMUM_FUNCTION; t++)
		DriverObject->MajorFunction[t] = &UnsupportedDispatch;

	DriverObject->MajorFunction[IRP_MJ_CREATE] = &CreateDispatch;
	DriverObject->MajorFunction[IRP_MJ_CLOSE] = &CloseDispatch;
	DriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL] = &IoControl;
	DriverObject->DriverUnload = NULL;	
	deviceObject->Flags &= ~DO_DEVICE_INITIALIZING;

	return status;
}

NTSTATUS DriverEntry(_In_ PDRIVER_OBJECT DriverObject, _In_ PUNICODE_STRING RegistryPath)
{
	UNREFERENCED_PARAMETER(DriverObject);
	UNREFERENCED_PARAMETER(RegistryPath);

	return IoCreateDriver(NULL, &DriverInitialize);
}