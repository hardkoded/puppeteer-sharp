Import-Module WebAdministration
Set-Location -Path cert:\LocalMachine\My
Import-Certificate -Filepath "C:\projects\puppeteer-sharp\lib\PuppeteerSharp.TestServer\testCert.cer"