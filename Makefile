windows:
	dotnet publish --configuration release --runtime win-x64

darwin:
	dotnet publish --configuration release --runtime osx-x64

linux:
	dotnet publish --configuration release --runtime linux-x64

clean:
	@rm ./bin/*

run:
	./bin/Debug/netcoreapp3.0/OpenApiCleaner clean -s api/specification -o output
