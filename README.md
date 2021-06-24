## Description
Program to add MobileGuardAuthenticator to steam accounts created using [SteamAccountCreateHelper](https://github.com/Cappi1998/SteamAccountCreateHelper) program.

## Getting Started

### Prerequisites
[.NET Core 5.0](https://dotnet.microsoft.com/download) or higher required. 

- Run the program for the first time to create the configuration files.
- Configure the "Config.json" configuration file.
- folder "Database/Accounts_ToAdded_Guard" is where you add the .txt files of the accounts you want to add Mobile Guard Authenticator.
- folder "Database/Mobile_Guard_Added" this is where the account goes when it is processed and the Mobile Guard is successfully added, the .mafile file will also be created.


### Config.json
- <a href="#PhoneServiceToUse">PhoneServiceToUse</a> => Phone Service that will be used to obtain rental numbers.
- PhoneServiceApiKey => API Key for the chosen Phone Service.
- Country => check <a href="#PhoneServiceToUse">PhoneServiceToUse</a> to find out how to get the country code

## PhoneServiceToUse
- "sms-activate.ru" - I checked the desired Country Code [here](https://sms-activate.ru/en/api2).
- "onlinesim.ru" - Config.Country is the DDI of the desired country.


![](Screenshots/Gui1.2.2.png) 

## DISCLAIMER
This project is provided on AS-IS basis, without any guarantee at all. Author is not responsible for any harm, direct or indirect, that may be caused by using this plugin. You use this project at your own risk.
