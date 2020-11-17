dotnet publish ./src/DeckParser.csproj `
	-r win-x64 `
	-c Release `
	-o "dist" `
	--self-contained true `
	-p:PublishSingleFile=true `
	-p:PublishTrimmed=true `
	-p:IncludeNativeLibrariesForSelfExtract=true `
	-p:DebugType=embedded