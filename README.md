# Playwright.GenerateXConnectData
Simple console app to generate traffic for creating contacts and interactions in Sitecore XP

## Building

Just build VS solution, then go to the `\bin\Debug\net6.0\playwright.ps1` folder and install playwright by running following command:


`.\playwright.ps1 install`

The programm uses Playwright not in headless mode. When running in Headless mode, Sitecore does not record contacts and interactions.

Note: before running the program, make sure to put `flush.aspx` file to the Sitecore instance you are going to request. Can be found here for [Sitecore 10](https://gist.github.com/ivanbuzyka/212f6dd1c9b1208b83d4ad5cbddf2016) and for [Sitecore 9](https://gist.github.com/ivanbuzyka/94780aca7f5f90d83e4795c0d6670617)
