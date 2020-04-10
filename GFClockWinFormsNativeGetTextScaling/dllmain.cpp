// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"

#include <windows.h>
#include <roapi.h>
#include <iostream>
#include <cwchar>

// For Windows.UI.ViewManagement.UISettings
#include <windows.ui.viewmanagement.h>

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
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

double GetTextScaling()
{
    std::cout << "Barker: In GetTextScaling" << std::endl;

    ::RoInitialize(RO_INIT_MULTITHREADED);

    HSTRING_HEADER header;
    HSTRING hstring;

    ::WindowsCreateStringReference(
        RuntimeClass_Windows_UI_ViewManagement_UISettings,
        wcslen(RuntimeClass_Windows_UI_ViewManagement_UISettings),
        &header,
        &hstring);

    // TODO: Use an RAII container/wrapper (such as Microsoft::WRL::ComPtr from the WRL library) to prevent leaks.
    IInspectable* uiSettingsAsInspectable = nullptr;

    std::cout << "Barker: Call RoActivateInstance" << std::endl;
    HRESULT hr = ::RoActivateInstance(hstring, &uiSettingsAsInspectable);

    if (FAILED(hr)) {
        std::cout << std::hex << hr << std::endl;
        return 0;
    }

    ::ABI::Windows::UI::ViewManagement::IUISettings2* uiSettings = nullptr;

    std::cout << "Barker: Call QueryInterface" << std::endl;
    hr = uiSettingsAsInspectable->QueryInterface(__uuidof(uiSettings), reinterpret_cast<void**>(&uiSettings));
    if (FAILED(hr)) {
        std::cout << std::hex << hr << std::endl;
        return 0;
    }

    double factor = 0;

    std::cout << "Barker: Call get_TextScaleFactor" << std::endl;
    hr = uiSettings->get_TextScaleFactor(&factor);
    if (FAILED(hr)) {
        std::cout << std::hex << hr << std::endl;
        return 0;
    }

    std::cout << factor << std::endl;

    return factor;
}
