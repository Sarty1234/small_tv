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


// Loads config data and returns dictionary
std::map<std::string, std::string> Config::LoadConfigLines() {


	// Make sure config file exists
	if (!std::filesystem::exists(relativeConfigFilePath)) {
		std::ofstream outInAInBFileCreation(relativeConfigFilePath);
		outInAInBFileCreation.close();
	}
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


Config::Config() {
	LoadConfig();
}


void Config::LoadConfig() {
	std::map<std::string, std::string> configData = LoadConfigLines();



	// Translating parametres from string to their types
	auto TVKeysPos = configData.find("TVKeys");
	auto MenuKeysPos = configData.find("MenuKeys");
	auto PanicKeysPos = configData.find("PanicKeys");
	auto SpyCheckPos = configData.find("SpyCheck");
	auto SpyPanicPos = configData.find("SpyPanic");

	if (TVKeysPos != configData.end()) {
		std::istringstream paramSS(configData["TVKeys"]);
		int val;

		TVKeys.clear();
		while (paramSS >> val) {
			TVKeys.push_back(val);
		}
	}

	if (MenuKeysPos != configData.end()) {
		std::istringstream paramSS(configData["MenuKeys"]);
		int val;

		MenuKeys.clear();
		while (paramSS >> val) {
			MenuKeys.push_back(val);
		}
	}

	if (PanicKeysPos != configData.end()) {
		std::istringstream paramSS(configData["PanicKeys"]);
		int val;

		PanicKeys.clear();
		while (paramSS >> val) {
			PanicKeys.push_back(val);
		}
	}

	if (SpyCheckPos != configData.end()) {
		if (configData["SpyCheck"] == "true") {
			SpyCheck = true;
		}
		else {
			SpyCheck = false;
		}
	}

	if (SpyPanicPos != configData.end()) {
		if (configData["SpyPanic"] == "true") {
			SpyPanic = true;
		}
		else {
			SpyPanic = false;
		}
	}
}


std::string joinKeysInString(std::vector<int> keys) {
	std::ostringstream ss;
	for (size_t i = 0; i < keys.size(); i++) {
		ss << keys[i] << (i == keys.size() - 1 ? "" : " ");
	}
	return ss.str();
}

// Changes config parametres
void Config::SaveConfig() {
	// Creating param strings
	std::string tvKeysSaveString = joinKeysInString(TVKeys);
	std::string MenuKeysSaveString = joinKeysInString(MenuKeys);
	std::string PanicKeysSaveString = joinKeysInString(PanicKeys);
	std::string SpyCheckSaveString = (SpyCheck ? "true" : "false");
	std::string SpyPanicSaveString = (SpyPanic ? "true" : "false");
	std::map<std::string, std::string> params = {
		{ "TVKeys", tvKeysSaveString },
		{ "MenuKeys", MenuKeysSaveString },
		{ "PanicKeys", PanicKeysSaveString },
		{ "SpyCheck", SpyCheckSaveString },
		{ "SpyPanic", SpyPanicSaveString },
	};


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


	const std::string vklink = "# virtual key codes - https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes";
	if (oldConfigLines.size() != 0 && oldConfigLines[0] != vklink) {
		ConfigFile << vklink << "\n" << std::endl;
	}

	for (const auto& i : oldConfigLines) {
		ConfigFile << i << std::endl;
	}

	for (const auto& kv : params) {
		ConfigFile << kv.first << "=" << kv.second << std::endl;
	}

	ConfigFile.close();
}