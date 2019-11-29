################################################################################
##  File:  Install-EdgePreview.ps1
##  Team:  Automated Testing
##  Desc:  Install Microsoft Edge preview
################################################################################
#Save the current value in the $p variable.
Copy-Item -Path .\ImageHelpers -Destination $home\Documents\WindowsPowerShell\Modules\ImageHelpers -Recurse -Force

Import-Module -Name ImageHelpers -Force

$temp_install_dir = 'C:\Windows\Installer'
New-Item -Path $temp_install_dir -ItemType Directory -Force

Install-EXE -Url "https://go.microsoft.com/fwlink/?linkid=2069324&Channel=Dev&language=en-us&Consent=1" -Name "MicrosoftEdgeSetup.exe" -ArgumentList "/silent /install"

# Add some things to stop Edge from auto updating.
New-NetFirewallRule -DisplayName "BlockEdgeUpdate" -Direction Outbound -Action Block -Program "C:\Program Files (x86)\Microsoft\EdgeUpdate\MicrosoftEdgeUpdate.exe"

New-Item -Path "HKLM:\SOFTWARE\Policies\Microsoft\MicrosoftEdgeUpdate" -Force
New-ItemProperty "HKLM:\SOFTWARE\Policies\Microsoft\MicrosoftEdgeUpdate" -Name "AutoUpdateCheckPeriodMinutes" -Value 00000000 -Force
New-ItemProperty "HKLM:\SOFTWARE\Policies\Microsoft\MicrosoftEdgeUpdate" -Name "UpdateDefault" -Value 00000000 -Force
New-ItemProperty "HKLM:\SOFTWARE\Policies\Microsoft\MicrosoftEdgeUpdate" -Name "DisableAutoUpdateChecksCheckboxValue" -Value 00000001 -Force
New-ItemProperty "HKLM:\SOFTWARE\Policies\Microsoft\MicrosoftEdgeUpdate" -Name "Update{8A69D345-D564-463C-AFF1-A69D9E530F96}" -Value 00000000 -Force
