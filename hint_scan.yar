rule Process_Injection
{
	meta:
		description = "process injection detection, note may catch normal processes and not catch dynamic imports"
	
	strings:
		// Handle Acquiring
		$create = "CreateProcess" nocase
		$create_older = "WinExec"
		
		// Hijacks existing process
		$hijack_snapshot = "CreateToolhelp32Snapshot" nocase
		$hijack_first = "Process32First" nocase
		$hijack_next = "Process32Next" nocase
		$hijack_handle = "OpenProcess" nocase
		$hijack_lists_process = "NtQuerySystemInformation" nocase
		
		// Hollowing or RunPE
		$hollow_unmapped = "NtUnmapViewOfSection" nocase
		
		// Atom Bombing
		$atom_globaladd = "GlobalAddAtom" nocase
		$atom_getname   = "GlobalGetAtomName" nocase
		$atom_apc       = "NtQueueApcThread" nocase
		$atom_apc_alt   = "QueueUserAPC" nocase

		// Process Doppelgänging
		$doppel_tx      = "NtCreateTransaction" nocase
		$doppel_rollback = "NtRollbackTransaction" nocase
		$doppel_create_process = "NtCreateProcess" nocase
		
		// DLL Injection
		$dll_loadlib    = "LoadLibraryA" nocase
		
		// Manual Transfer, used in both hollowing and code injection
		$transf_alloc  = "VirtualAllocEx" nocase
		$transf_ntalloc = "NtAllocateVirtualMemory" nocase
		$transf_write = "WriteProcessMemory" nocase
		$transf_ntwrite = "NtWriteVirtualMemory" nocase
		
		// Transfer via creating sections, used in both hollowing and doppelganger
		$transf_section = "NtCreateSection" nocase
		$transf_map = "NtMapViewOfSection" nocase
		
		// Thread method, used in both hollowing and code injection
		$exec_new_ntthread = "NtCreateThreadEx" nocase
		$exec_new_thread  = "CreateRemoteThread" nocase
		$exec_used_thread = "SetThreadContext" nocase
		$exec_used_thread_run = "ResumeThread" nocase

	condition:
		// process creation/hijack detection
		any of ($create*) or any of ($hijack*)

		// technique detection: at least 2 of APIs per technique
		or 2 of ($hollow_unmapped, $transf_section, $transf_map, $transf_alloc, $transf_ntalloc, $transf_write, $transf_ntwrite, $exec_new_thread, $exec_new_ntthread, $exec_used_thread, $exec_used_thread_run)
		or 2 of ($doppel_tx, $doppel_rollback, $doppel_create_process, $transf_section, $transf_map)
		or 2 of ($dll_loadlib, $exec_new_thread, $exec_new_ntthread)
		or 2 of ($atom_globaladd, $atom_getname, $atom_apc, $atom_apc_alt)
}