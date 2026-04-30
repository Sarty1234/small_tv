#pragma once
#include <vector>
#include <string>
#include <fstream>
#include <map>



class Config {
private:
	static std::vector<std::string> ConfigFileAsVector();
public:
	// Loads config data and returns dictionary containing data
	static std::map<std::string, std::string> LoadConfig();


	// Changes config parametres
	static void ChangeConfig(std::map<std::string, std::string> params);
};

