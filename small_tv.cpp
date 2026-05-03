// HelloWindowsDesktop.cpp
// compile with: /D_UNICODE /DUNICODE /DWIN32 /D_WINDOWS /c

#include <windows.h>
#include <stdlib.h>
#include <string.h>
#include <tchar.h>
#include <vector>
#include <string>
#include <atlstr.h>
#include "Config.h"

// Global variables

// The main window class name.
static TCHAR szWindowClass[] = _T("DesktopApp");

// The string that appears in the application's title bar.
static TCHAR szTitle[] = _T("Windows Desktop Guided Tour Application");

// Stored instance handle for use in Win32 API calls such as FindResource
HINSTANCE hInst;

// Forward declarations of functions included in this code module:
LRESULT CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);


// Loading configs
static Config appConfig;
int WINAPI WinMain(
    _In_ HINSTANCE hInstance,
    _In_opt_ HINSTANCE hPrevInstance,
    _In_ LPSTR     lpCmdLine,
    _In_ int       nCmdShow
)
{
    WNDCLASSEX wcex;

    wcex.cbSize = sizeof(WNDCLASSEX);
    wcex.style = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc = WndProc;
    wcex.cbClsExtra = 0;
    wcex.cbWndExtra = 0;
    wcex.hInstance = hInstance;
    wcex.hIcon = LoadIcon(wcex.hInstance, IDI_APPLICATION);
    wcex.hCursor = LoadCursor(NULL, IDC_ARROW);
    wcex.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
    wcex.lpszMenuName = NULL;
    wcex.lpszClassName = szWindowClass;
    wcex.hIconSm = LoadIcon(wcex.hInstance, IDI_APPLICATION);

    if (!RegisterClassEx(&wcex))
    {
        MessageBox(NULL,
            _T("Call to RegisterClassEx failed!"),
            _T("Windows Desktop Guided Tour"),
            NULL);

        return 1;
    }

    // Store instance handle in our global variable
    hInst = hInstance;

    // The parameters to CreateWindowEx explained:
    // WS_EX_OVERLAPPEDWINDOW : An optional extended window style.
    // szWindowClass: the name of the application
    // szTitle: the text that appears in the title bar
    // WS_OVERLAPPEDWINDOW: the type of window to create
    // CW_USEDEFAULT, CW_USEDEFAULT: initial position (x, y)
    // 500, 100: initial size (width, height)
    // NULL: the parent of this window
    // NULL: this application does not have a menu bar
    // hInstance: the first parameter from WinMain
    // NULL: not used in this application
    HWND hWnd = CreateWindowEx(
        WS_EX_OVERLAPPEDWINDOW,
        szWindowClass,
        szTitle,
        WS_OVERLAPPEDWINDOW,
        CW_USEDEFAULT, CW_USEDEFAULT,
        500, 100,
        NULL,
        NULL,
        hInstance,
        NULL
    );

    if (!hWnd)
    {
        MessageBox(NULL,
            _T("Call to CreateWindowEx failed!"),
            _T("Windows Desktop Guided Tour"),
            NULL);

        return 1;
    }

    // The parameters to ShowWindow explained:
    // hWnd: the value returned from CreateWindowEx
    // nCmdShow: the fourth parameter from WinMain
    ShowWindow(hWnd,
        nCmdShow);
    UpdateWindow(hWnd);





    // Main message loop:
    MSG msg;
    while (GetMessage(&msg, NULL, 0, 0))
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    return (int)msg.wParam;
}






std::string GetVirtualKeyName(UINT vkCode) {
    // 1. Map virtual key to scan code
    UINT scanCode = MapVirtualKey(vkCode, MAPVK_VK_TO_VSC);

    // 2. Prepare lParam for GetKeyNameText
    // Bits 16-23 must contain the scan code.
    LONG lParam = scanCode << 16;

    // Handle extended keys (like arrow keys, right Alt, etc.)
    // These often require the "extended" bit (bit 24) to be set.
    switch (vkCode) {
    case VK_INSERT: case VK_DELETE: case VK_HOME: case VK_END:
    case VK_PRIOR:  case VK_NEXT:   case VK_LEFT: case VK_UP:
    case VK_RIGHT:  case VK_DOWN:   case VK_DIVIDE: case VK_NUMLOCK:
        lParam |= (1 << 24);
        break;
    }

    char keyName[256];
    if (GetKeyNameTextA(lParam, keyName, sizeof(keyName)) > 0) {
        return std::string(keyName);
    }
    return "Unknown Key";
}


std::string KeyVectorToString(std::vector<int> keys) {
    std::string keystring = "";
    for (size_t i = 0; i < keys.size(); i++) {
        keystring += GetVirtualKeyName(keys[i]) + (i == keys.size() - 1 ? "" : " + ");
    }

    return keystring;
}


//  FUNCTION: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the main window.
//
//  WM_PAINT    - Paint the main window
//  WM_DESTROY  - post a quit message and return
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    static enum ProgramState
    {
        TV,
        HIDDEN
    };






    PAINTSTRUCT ps;
    HDC hdc;
    TCHAR messageStringBuffer[256];
    POINT mouseposition;


    // ***************** UI elements data
    
    std::string TVKeysString = KeyVectorToString(appConfig.TVKeys);
    std::string MenuKeysString = KeyVectorToString(appConfig.MenuKeys);
    std::string PanicKeysString = KeyVectorToString(appConfig.PanicKeys);


    int UIELEMENT_DrawNewFieldButton_X = 10;
    int UIELEMENT_DrawNewFieldButton_CX = 100;
    int UIELEMENT_DrawNewFieldButton_Y = 10;
    int UIELEMENT_DrawNewFieldButton_CY = 20;
    HBRUSH UIELEMENT_DrawNewFieldButton_Brush = CreateSolidBrush(RGB(0, 0, 0));
    



    


    switch (message)
    {
    case WM_PAINT:
        hdc = BeginPaint(hWnd, &ps);

        // Here your application is laid out.
        // For this introduction, we just print out "Hello, Windows desktop!"
        // in the top left corner.
        TextOut(hdc,
            50, 50,
            CA2T(TVKeysString.c_str()), _tcslen(CA2T(TVKeysString.c_str())));
        SetPixel(hdc, 5, 5, RGB(255, 0, 0));
        



        // ****************  Drawing UI  ****************
        // Draw new field button
        FillRgn(hdc, 
            CreateRectRgn(
                UIELEMENT_DrawNewFieldButton_X, 
                UIELEMENT_DrawNewFieldButton_Y,
                UIELEMENT_DrawNewFieldButton_X + UIELEMENT_DrawNewFieldButton_CX,
                UIELEMENT_DrawNewFieldButton_Y + UIELEMENT_DrawNewFieldButton_CY
            ),
            UIELEMENT_DrawNewFieldButton_Brush
        );
        

        

        EndPaint(hWnd, &ps);
        break;

    case WM_DESTROY:
        PostQuitMessage(0);
        break;

    case WM_LBUTTONDOWN:
        _stprintf_s(messageStringBuffer, 256, _T("No click data"));

        if (!GetCursorPos(&mouseposition)) {
            break;
        }
        if (!ScreenToClient(hWnd, &mouseposition)) {
            break;
        }
        _stprintf_s(messageStringBuffer, 256, _T("x=%d, y=%d"), mouseposition.x, mouseposition.y);

        // MessageBox(NULL, messageStringBuffer, L"Click occured", MB_OK);
        break;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
        break;
    }

    return 0;
}
