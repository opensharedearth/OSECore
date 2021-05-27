$ErrorActionPreference = 'Stop'

Set-Location -LiteralPath $PSScriptRoot

$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_NOLOGO = '1'

dotnet build -c Release -r linux-x64 test\OSECommand.Test
dotnet build -c Release -r linux-x64 test\OSEConfig.Test
dotnet build -c Release -r linux-x64 test\OSEConsole.Test
dotnet build -c Release -r linux-x64 test\OSECore.Test
dotnet build -c Release -r linux-x64 test\OSECoreUI.Test
dotnet build -c Release -r linux-x64 test\pathtool.Test
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
