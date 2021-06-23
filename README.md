## Description
Program to add MobileGuardAuthenticator to steam accounts created using [SteamAccountCreateHelper](https://github.com/Cappi1998/SteamAccountCreateHelper) program

## Getting Started
- Run the program for the first time to create the configuration files.
- Configure the "Config.json" configuration file.
- folder "Database/Accounts_ToAdded_Guard" is where you add the .txt files of the accounts you want to add Mobile Guard Authenticator.
- folder "Database/Mobile_Guard_Added" this is where the account goes when it is processed and the Mobile Guard is successfully added, the .mafile file will also be created.


### Config.json
- PhoneServiceToUse => Phone Service that will be used to obtain rental numbers.
   -- Available "sms-activate.ru" or "onlinesim.ru".
- PhoneServiceApiKey => API Key for the chosen Phone Service.
- 



## DISCLAIMER
This project is provided on AS-IS basis, without any guarantee at all. Author is not responsible for any harm, direct or indirect, that may be caused by using this plugin. You use this plugin at your own risk.
