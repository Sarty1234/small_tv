#include "Config.h"

std::string relativeConfigFilePath = "config.ini";
const std::string WHITESPACE = " \n\r\t\f\v";


std::vector<std::string> Config::ConfigFileAsVector() {
	std::ifstream ConfigFile(relativeConfigFilePath);
	std::vector<std::string> resultVector = std::vector<std::string>();


	std::string openedFileLine = "";
	while (std::getline(ConfigFile, openedFileLine)) {
		resultVector.push_back(std::move(openedFileLine));
		// move передає право власності поінтера
	}

	ConfigFile.close();
	return resultVector;
}


// Loads config data and returns dictionary containing data
std::map<std::string, std::string> Config::LoadConfig() {
	std::ifstream ConfigFile(relativeConfigFilePath);
	std::map<std::string, std::string> resultDictionary;


	std::string fileLine = "";
	std::string temp = "";
	std::string key = "";
	std::string value = "";
	while (std::getline(ConfigFile, fileLine)) {
		// Skip empty lines
		if (fileLine == "") continue;


		// Skip non variable lines
		if (fileLine.find("=") == std::string::npos) continue;


		// Cut everything after hashtag
		size_t charPos = fileLine.find('#');
		if (charPos != std::string::npos) {
			fileLine = fileLine.substr(0, charPos);
		}


		// Tris string from whitespaces
		charPos = fileLine.find_first_not_of(WHITESPACE);
		if (charPos != std::string::npos) {
			fileLine = fileLine.substr(charPos);
		}

		charPos = fileLine.find_last_not_of(WHITESPACE);
		if (charPos != std::string::npos) {
			fileLine = fileLine.substr(0, charPos + 1);
		}


		// Extracting key
		charPos = fileLine.find("=");
		key = fileLine.substr(0, charPos);

		// Triming key on right side
		charPos = key.find_last_not_of(WHITESPACE);
		if (charPos != std::string::npos) {
			key = key.substr(0, charPos + 1);
		}


		// Extracting value
		charPos = fileLine.find("=");
		value = fileLine.substr(charPos + 1);

		// Triming value on left side
		charPos = value.find_first_not_of(WHITESPACE);
		if (charPos != std::string::npos) {
			value = value.substr(charPos);
		}


		// finally saving them into dictionary
		resultDictionary[key] = value;
	}

	ConfigFile.close();
	return resultDictionary;
}


// Changes config parametres
void Config::ChangeConfig(std::map<std::string, std::string> params) {
	std::vector<std::string> oldConfigLines = Config::ConfigFileAsVector();


	std::ofstream ConfigFile(relativeConfigFilePath);
	std::map<std::string, std::string> resultDictionary;


	std::string temp = "";
	std::string key = "";
	std::string value = "";
	size_t charPos;
	size_t charPos2;
	for (int i = 0; i < oldConfigLines.size(); i++) {
		std::string& fileLine = oldConfigLines[i];

		// Skip if equasion is commented
		charPos = fileLine.find('=');
		charPos2 = fileLine.find('#');
		if (charPos2 < charPos) {
			continue;
		}

		key = fileLine.substr(0, charPos);

		// Skip if it's not modified key
		auto paramKey = params.find(key);
		if (paramKey == params.end()) continue;

		fileLine = fileLine.substr(0, charPos + 1);
		fileLine += params[key];
		params.erase(paramKey);
	}


	for (const auto& i : oldConfigLines) {
		ConfigFile << i << std::endl;
	}

	for (const auto& kv : params) {
		ConfigFile << kv.first << "=" << kv.second << std::endl;
	}

	ConfigFile.close();
}