// RtxRunUtility.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"
#include "SRMRtx.h"

#include <string>
#include <map>

BOOL APIENTRY DllMain(HANDLE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
    return TRUE;
}

SRMRTX_API int nRtxRunUtility=0;

using namespace std;
// Event List
map<string, CRtEvent*> m_EventList;

// Vision COM Smemory
SRMRTX_API SMP_COM smpCOM;
SRMRTX_API HANDLE m_smhCOM;
map<string, char(*_w64)[50]> m_COMSMemoryStringList;



// Start rtss process
SRMRTX_API long RunUtility(void)
{
	STARTUPINFO		sinfo;
	PROCESS_INFORMATION	pinfo;
	ULONG		exitCode = 0;

	// Initialize startup info to all zeros.
	ZeroMemory(&sinfo, sizeof(sinfo));
	sinfo.cb=sizeof(sinfo);	

	// Create the process to run the utility. 
	if (CreateProcess(NULL, "RTSSrun RTXSim.rtss", NULL, NULL, FALSE, 0, NULL,
	NULL, &sinfo, &pinfo) == FALSE)
	{
	return -1;
	}

	// Wait for the utility to complete.
	WaitForSingleObject(pinfo.hProcess, INFINITE);

	// Get the exit code (RTSSrun returns the process slot) and close handles.
	GetExitCodeProcess(pinfo.hProcess, &exitCode);

	CloseHandle(pinfo.hThread);
	CloseHandle(pinfo.hProcess);

	return exitCode;
}

// Kill rtss process
SRMRTX_API long KillRunUtility(long lRtssPid)
{
	STARTUPINFO		sinfo;
	PROCESS_INFORMATION	pinfo;
	ULONG		exitCode = 0;

	// Initialize startup info to all zeros.
	ZeroMemory(&sinfo, sizeof(sinfo));
	sinfo.cb=sizeof(sinfo);	

	// Create the process to run the utility. 
	char szRtsskill[50];
	sprintf_s(szRtsskill, "RTSSkill %d", lRtssPid);

	if (CreateProcess(NULL, szRtsskill, NULL, NULL, FALSE, 0, NULL,
	NULL, &sinfo, &pinfo) == FALSE)
	{
	return -1;
	}

	// Wait for the utility to complete.
	WaitForSingleObject(pinfo.hProcess, INFINITE);

	// Get the exit code (RTSSrun returns the process slot) and close handles.
	GetExitCodeProcess(pinfo.hProcess, &exitCode);

	CloseHandle(pinfo.hThread);
	CloseHandle(pinfo.hProcess);
  
	return exitCode;
}

// This is the constructor of a class that has been exported.
// see RtxRunUtility.h for the class definition
CRtxRunUtility::CRtxRunUtility()
{ 
	return; 
}

// Create Event
SRMRTX_API void SRMCreateEvent(BOOL bInitiallyOwn, BOOL bManualReset, LPSTR eventName)
{
	m_EventList.insert(pair<string, CRtEvent*>(eventName, new CRtEvent(bInitiallyOwn, bManualReset, eventName)));
}

// Set Event
SRMRTX_API void SRMSetEvent(LPSTR eventName)
{
	m_EventList[eventName]->SetEvent();
}

// Reset Event
SRMRTX_API void SRMResetEvent(LPSTR eventName)
{
	m_EventList[eventName]->ResetEvent();
}

// Wait for singlelock event
SRMRTX_API bool SRMSingleLock(LPSTR eventName)
{
	CRtSingleLock sSingleLock(m_EventList[eventName], FALSE);
	if (sSingleLock.Lock(0))
		return true;
	else
		return false;
}

// Create All SMemory
SRMRTX_API void SRMCreateAllSMemory(void)
{
	// -------------- Create COM SMemory ------------------------
	m_smhCOM = RtCreateSharedMemory(PAGE_READWRITE, 0, sizeof(SM_COM), "COMSMemory", (LPVOID*) &smpCOM);

	m_COMSMemoryStringList.insert(pair<string, char(*_w64)[50]>("uCOMSendData1", &smpCOM->uCOMSendData1));
	m_COMSMemoryStringList.insert(pair<string, char(*_w64)[50]>("uCOMSendData2", &smpCOM->uCOMSendData2));
	m_COMSMemoryStringList.insert(pair<string, char(*_w64)[50]>("uCOMSendData3", &smpCOM->uCOMSendData3));
	m_COMSMemoryStringList.insert(pair<string, char(*_w64)[50]>("uCOMSendData4", &smpCOM->uCOMSendData4));
	m_COMSMemoryStringList.insert(pair<string, char(*_w64)[50]>("uCOMSendData5", &smpCOM->uCOMSendData5));
	m_COMSMemoryStringList.insert(pair<string, char(*_w64)[50]>("uCOMSendData6", &smpCOM->uCOMSendData6));
	m_COMSMemoryStringList.insert(pair<string, char(*_w64)[50]>("uCOMSendData7", &smpCOM->uCOMSendData7));

}

// COM SMemory
SRMRTX_API void SRMSetCOMString(LPSTR valueName,  LPSTR szValue)
{
 	wsprintf(*(m_COMSMemoryStringList[valueName]), "%s", szValue);
}

SRMRTX_API LPSTR SRMGetCOMString(LPSTR valueName)
{
	return *(m_COMSMemoryStringList[valueName]);
}

