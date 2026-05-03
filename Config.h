#pragma once
#include <vector>
#include <string>
#include <fstream>
#include <map>
#include <filesystem>
#include <sstream>



class Config {
private:
	static std::vector<std::string> ConfigFileAsVector();

    // Loads config data and returns dictionary
    static std::map<std::string, std::string> LoadConfigLines();

public:

    // *****************  PROGRAMM SETTINGS  *****************
    // virtual key codes - https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
    std::vector<int> TVKeys = { 0x11, 0x47 }; // ctrl+g
    std::vector<int> MenuKeys = { 0x11 };  // ctrl
    std::vector<int> PanicKeys = { 0x11, 0x43 };  // ctrl+c
    bool SpyCheck = true;
    bool SpyPanic = false;

    Config();

    // Loads config data in class
    void LoadConfig();

    // Changes config parametres
    void SaveConfig();
};

