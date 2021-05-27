FROM mcr.microsoft.com/dotnet/sdk:5.0

WORKDIR /bin
COPY test/OSECommand.Test/bin/Release/net5.0/linux-x64/ .
COPY test/OSEConfig.Test/bin/Release/net5.0/linux-x64/ .
COPY test/OSEConsole.Test/bin/Release/net5.0/linux-x64/ .
COPY test/OSECore.Test/bin/Release/net5.0/linux-x64/ .
COPY test/OSECoreUI.Test/bin/Release/net5.0/linux-x64/ .
COPY test/pathtool.Test/bin/Release/net5.0/linux-x64/ .

CMD ["dotnet","vstest","*.Test.dll"]
