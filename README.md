# Mario
# Restore & build cho Android
dotnet restore
dotnet build -f net6.0-android -c Release

# Chạy trên emulator
dotnet run -f net6.0-android

# Hoặc build APK release
dotnet publish -f net6.0-android -c Release
