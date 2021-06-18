FROM mcr.microsoft.com/dotnet/sdk:5.0

SHELL ["/bin/bash","-c"]
RUN adduser --gid 100 tester
WORKDIR /home/tester
ENV SCRATCH=/home/tester/scratch
COPY test/OSECommand.Test/bin/Release/net5.0/linux-x64/ .
COPY test/OSEConfig.Test/bin/Release/net5.0/linux-x64/ .
COPY test/OSEConsole.Test/bin/Release/net5.0/linux-x64/ .
COPY test/OSECore.Test/bin/Release/net5.0/linux-x64/ .
COPY test/OSECoreUI.Test/bin/Release/net5.0/linux-x64/ .
COPY test/pathtool.Test/bin/Release/net5.0/linux-x64/ .

CMD ["dotnet","vstest","*.Test.dll"]
