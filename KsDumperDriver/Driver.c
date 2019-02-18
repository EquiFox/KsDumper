#include "NTUndocumented.h"
#include "ProcessLister.h"
#include "UserModeBridge.h"
#include <wdf.h>

DRIVER_INITIALIZE DriverEntry;
#pragma alloc_text(INIT, DriverEntry)

UNICODE_STRING deviceName, symLink;

NTSTATUS CopyVirtualMemory(PEPROCESS targetProcess, PVOID sourceAddress, PVOID targetAddress, SIZE_T size)
{
	PSIZE_T readBytes;
	return MmCopyVirtualMemory(targetProcess, sourceAddress, PsGetCurrentProcess(), targetAddress, size, UserMode, &readBytes);
}

NTSTATUS IoControl(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
	NTSTATUS status;
	ULONG bytesIO = 0;
	PIO_STACK_LOCATION stack = IoGetCurrentIrpStackLocation(Irp);
	ULONG controlCode = stack->Parameters.DeviceIoControl.IoControlCode;

	if (controlCode == IO_COPY_MEMORY)
	{
		if (stack->Parameters.DeviceIoControl.InputBufferLength == sizeof(KERNEL_COPY_MEMORY_OPERATION))
		{
			PKERNEL_COPY_MEMORY_OPERATION request = (PKERNEL_COPY_MEMORY_OPERATION)Irp->AssociatedIrp.SystemBuffer;
			PEPROCESS targetProcess;

			if (NT_SUCCESS(PsLookupProcessByProcessId(request->targetProcessId, &targetProcess)))
			{
				CopyVirtualMemory(targetProcess, request->targetAddress, request->bufferAddress, request->bufferSize);
				ObDereferenceObject(targetProcess);
			}

			status = STATUS_SUCCESS;
			bytesIO = sizeof(KERNEL_COPY_MEMORY_OPERATION);
		}
		else
		{
			status = STATUS_INFO_LENGTH_MISMATCH;
			bytesIO = 0;
		}
	}
	else if (controlCode == IO_GET_PROCESS_LIST)
	{
		if (stack->Parameters.DeviceIoControl.InputBufferLength == sizeof(KERNEL_PROCESS_LIST_OPERATION) &&
			stack->Parameters.DeviceIoControl.OutputBufferLength == sizeof(KERNEL_PROCESS_LIST_OPERATION))
		{
			PKERNEL_PROCESS_LIST_OPERATION request = (PKERNEL_PROCESS_LIST_OPERATION)Irp->AssociatedIrp.SystemBuffer;

			GetProcessList(request->bufferAddress, request->bufferSize, &request->bufferSize, &request->processCount);

			status = STATUS_SUCCESS;
			bytesIO = sizeof(KERNEL_PROCESS_LIST_OPERATION);
		}
		else
		{
			status = STATUS_INFO_LENGTH_MISMATCH;
			bytesIO = 0;
		}
	}
	else
	{
		status = STATUS_INVALID_PARAMETER;
		bytesIO = 0;
	}

	Irp->IoStatus.Status = status;
	Irp->IoStatus.Information = bytesIO;
	IoCompleteRequest(Irp, IO_NO_INCREMENT);

	return status;
}

NTSTATUS UnsupportedDispatch(_In_ PDEVICE_OBJECT DeviceObject, _Inout_ PIRP Irp)
{
	UNREFERENCED_PARAMETER(DeviceObject);

	Irp->IoStatus.Status = STATUS_NOT_SUPPORTED;
	IoCompleteRequest(Irp, IO_NO_INCREMENT);
	return Irp->IoStatus.Status;
}

NTSTATUS CreateDispatch(_In_ PDEVICE_OBJECT DeviceObject, _Inout_ PIRP Irp)
{
	UNREFERENCED_PARAMETER(DeviceObject);

	IoCompleteRequest(Irp, IO_NO_INCREMENT);
	return Irp->IoStatus.Status;
}

NTSTATUS CloseDispatch(_In_ PDEVICE_OBJECT DeviceObject, _Inout_ PIRP Irp)
{
	UNREFERENCED_PARAMETER(DeviceObject);

	IoCompleteRequest(Irp, IO_NO_INCREMENT);
	return Irp->IoStatus.Status;
}

NTSTATUS Unload(IN PDRIVER_OBJECT DriverObject)
{
	IoDeleteSymbolicLink(&symLink);
	IoDeleteDevice(DriverObject->DeviceObject);
}

NTSTATUS DriverInitialize(_In_ PDRIVER_OBJECT DriverObject, _In_ PUNICODE_STRING RegistryPath)
{
	NTSTATUS status;
	PDEVICE_OBJECT deviceObject;

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
		IoDeleteDevice(deviceObject);
		return status;
	}
	deviceObject->Flags |= DO_BUFFERED_IO;

	for (ULONG t = 0; t <= IRP_MJ_MAXIMUM_FUNCTION; t++)
		DriverObject->MajorFunction[t] = &UnsupportedDispatch;

	DriverObject->MajorFunction[IRP_MJ_CREATE] = &CreateDispatch;
	DriverObject->MajorFunction[IRP_MJ_CLOSE] = &CloseDispatch;
	DriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL] = &IoControl;
	DriverObject->DriverUnload = &Unload;
	deviceObject->Flags &= ~DO_DEVICE_INITIALIZING;

	return status;
}

NTSTATUS DriverEntry(_In_ PDRIVER_OBJECT DriverObject, _In_ PUNICODE_STRING RegistryPath)
{
	UNREFERENCED_PARAMETER(DriverObject);
	UNREFERENCED_PARAMETER(RegistryPath);

	return IoCreateDriver(NULL, &DriverInitialize);
}
