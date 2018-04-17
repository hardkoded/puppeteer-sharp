$cert = New-SelfSignedCertificate -DnsName ("localhost","127.0.0.1") -FriendlyName "Puppeteer" -CertStoreLocation cert:\LocalMachine\My
$rootStore = Get-Item cert:\LocalMachine\Root
$rootStore.Open("ReadWrite")
$rootStore.Add($cert)
$rootStore.Close();
Import-Module WebAdministration
Get-ChildItem -Path cert:\LocalMachine\my | where { $_.friendlyname -eq "Puppeteer" } | Export-Certificate -FilePath C:\projects\puppeteer-sharp\lib\PuppeteerSharp.TestServer\testCert.cer
