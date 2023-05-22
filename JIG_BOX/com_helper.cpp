#include "stdafx.h"
#include "com_helper.h"
#include <setupapi.h>
#include <stdio.h>
#include <tchar.h>

//#define COM_PORT_NAME "Prolific USB-to-Serial Comm Port"
#define COM_PORT_NAME L"Arduino Due"



typedef struct tagSERIALPORTINFO {
    int                         nPortNumber;
    LPTSTR                      pszPortName;
    LPTSTR                      pszFriendlyName;
    struct tagSERIALPORTINFO    *next;
} SERIALPORTINFO, *LPSERIALPORTINFO;

BOOL GetSerialPortInfo(LPSERIALPORTINFO *ppInfo);
HANDLE SetupEnumeratePorts();
BOOL EndEnumeratePorts(HANDLE hDevices);

int
iFindArduinoCDC(int ComPorts[MAX_COM_PORTS]) {

    int ndevice =0;

    LPSERIALPORTINFO            pPortInfo       = 0;

    GetSerialPortInfo(&pPortInfo);

    while (pPortInfo) {
        LPSERIALPORTINFO        p       = pPortInfo;
        pPortInfo = pPortInfo->next;

        char buf[MAX_PATH];
        sprintf_s(buf, "Port %d '%ls'\n", p->nPortNumber, p->pszFriendlyName);
        OutputDebugString((LPCWSTR)(buf));

        if(wcsncmp((p->pszFriendlyName), COM_PORT_NAME, wcslen(COM_PORT_NAME))==0){
            ComPorts[ndevice] = p->nPortNumber;
            ndevice++;
        }
        
        HeapFree(GetProcessHeap(), 0, p->pszPortName);
        HeapFree(GetProcessHeap(), 0, p->pszFriendlyName);
        HeapFree(GetProcessHeap(), 0, p);
    }

	return ndevice;
}

BOOL
GetSerialPortInfo(LPSERIALPORTINFO *ppInfo) {
    HANDLE                      hDevices        = SetupEnumeratePorts();
    BOOL                        br              = TRUE;
    LPSERIALPORTINFO            pHead = 0, pTail = 0;
    DWORD                       nDevice         = 0;

    for (nDevice = 0; br ; nDevice++) {
        DWORD           cbData  = 0;
        SP_DEVINFO_DATA deviceData;

        deviceData.cbSize = sizeof(SP_DEVINFO_DATA);
        br = SetupDiEnumDeviceInfo(hDevices, nDevice, &deviceData);
        if (br) {
            TCHAR       sz[16]  = {
                0
            };
            HKEY        hkey    = SetupDiOpenDevRegKey(hDevices, &deviceData,
                                                       DICS_FLAG_GLOBAL, 0,
                                                       DIREG_DEV, KEY_READ);
            if (hkey) {
                DWORD   cbSize  = 16 * sizeof(TCHAR);
                RegQueryValueEx(hkey, _T("PortName"), NULL, NULL, (LPBYTE) sz,
                                &cbSize);
                RegCloseKey(hkey);
            }
            CharUpper(sz);
            if (sz[0] == _T('C') && sz[1] == _T('O') && sz[2] == _T('M')) {
                LPSERIALPORTINFO        pInfo   = (LPSERIALPORTINFO)
                                                  HeapAlloc(GetProcessHeap(),
                                                            0,
                                                            sizeof(SERIALPORTINFO));
                pInfo->next = 0;
                pInfo->pszPortName = (LPTSTR)
                                     HeapAlloc(GetProcessHeap(), 0,
                                               sizeof(TCHAR) * (lstrlen(sz) +
                                                                1));
                lstrcpy(pInfo->pszPortName, sz);
                pInfo->nPortNumber = _ttoi(&sz[3]);

                SetupDiGetDeviceRegistryProperty(hDevices, &deviceData,
                                                 SPDRP_FRIENDLYNAME, NULL,
                                                 NULL, 0, &cbData);
                if (cbData) {
                    pInfo->pszFriendlyName = (LPTSTR)
                                             HeapAlloc(GetProcessHeap(),
                                                                0,
                                                                cbData +
                                                                sizeof(TCHAR));
                    br = SetupDiGetDeviceRegistryProperty(hDevices,
                                                          &deviceData,
                                                          SPDRP_FRIENDLYNAME,
                                                          NULL,
                                                          (LPBYTE)
                                                          pInfo->pszFriendlyName,
                                                          cbData, NULL);
                    pInfo->pszFriendlyName[cbData] = 0;
                }

                if (pTail == 0) {
                    pHead = pTail = pInfo;
                } else {
                    pTail->next = pInfo;
                    pTail = pInfo;
                }
            }
        }
    }
    EndEnumeratePorts(hDevices);
    *ppInfo = pHead;
    return br;
}


HANDLE
SetupEnumeratePorts() {
    HDEVINFO            hDevices        = INVALID_HANDLE_VALUE;
    DWORD               dwGuids         = 0;    
    BOOL                br              = SetupDiClassGuidsFromName(_T("Ports"),
                                                                    0, 0,
                                                                    &dwGuids);
    if (dwGuids) {
        LPGUID  pguids  = (LPGUID) HeapAlloc(GetProcessHeap(), 0,
                                             sizeof(GUID) * dwGuids);
        if (pguids) {
            br = SetupDiClassGuidsFromName(_T("Ports"), pguids, dwGuids,
                                           &dwGuids);
            if (br) {
                hDevices = SetupDiGetClassDevs(pguids, NULL, NULL,
                                               DIGCF_PRESENT);
            }
            HeapFree(GetProcessHeap(), 0, pguids);
        }
    }
    return hDevices;
}
BOOL
EndEnumeratePorts(HANDLE hDevices) {
    if (SetupDiDestroyDeviceInfoList(hDevices)) {
        return TRUE;
    }
    return FALSE;
}

