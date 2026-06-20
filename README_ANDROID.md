# Mario Game - Android Build Instructions

## 📱 Hỗ trợ Android

Game Mario C# hiện đã hỗ trợ chạy trên Android 5.0+ (API 21+).

## 🔧 Yêu cầu hệ thống

- ✅ .NET 6.0 SDK hoặc cao hơn
- ✅ Visual Studio 2022 (hoặc Visual Studio Code + .NET CLI)
- ✅ Android SDK Platform API 31+
- ✅ Android NDK
- ✅ Java Development Kit (JDK) 11+

## 📥 Cài đặt môi trường

### Windows:

1. **Cài đặt Visual Studio 2022**
   - Chọn workload "Mobile development with .NET"
   - Chọn "Android SDK Setup" và "Android Emulator"

2. **Kiểm tra cài đặt**
   ```bash
   dotnet --list-sdks
   ```

### macOS/Linux:

```bash
# Cài đặt Android SDK (nếu chưa có)
sudo apt-get install -y openjdk-11-jdk

# Thiết lập ANDROID_HOME
export ANDROID_HOME=$HOME/Android/Sdk
export PATH=$PATH:$ANDROID_HOME/tools:$ANDROID_HOME/platform-tools
```

## 🚀 Chạy trên Emulator

### 1. Tạo Android Virtual Device (AVD)
```bash
# Sử dụng Android Studio hoặc
avdmanager create avd -n MarioAVD -k "system-images;android-31;google_apis;x86_64"
```

### 2. Chạy game
```bash
# Restore dependencies
dotnet restore

# Build cho Android
dotnet build -f net6.0-android -c Release

# Chạy trên emulator
dotnet run -f net6.0-android
```

## 📦 Build APK

### Release APK:
```bash
dotnet publish -f net6.0-android -c Release
```

File APK sẽ được tạo tại:
```
bin/Release/net6.0-android/publish/*.apk
```

### Debug APK:
```bash
dotnet publish -f net6.0-android -c Debug
```

## 📱 Cài đặt trên điện thoại

### Bằng ADB:
```bash
# Kết nối điện thoại qua USB (bật Developer Mode)
adb install -r bin/Release/net6.0-android/publish/com.mario.game*.apk
```

### Hoặc qua Windows:
- Sao chép file APK vào điện thoại
- Mở tệp APK trên điện thoại
- Chấp nhận cài đặt

## 🎮 Điều khiển trên Android

### Touch Controls:
- **Nửa trái màn hình** (25% bên trái) - **Di chuyển sang trái**
- **Nửa phải màn hình** (25% bên phải) - **Di chuyển sang phải**
- **Phần trên cùng** (30% trên) - **Nhảy**
- **Vùng giữa** - Không có tác dụng (đơn giản)

### Visual Debug Zones:
Không debug, vùng chạm được hiển thị bằng màu xanh/vàng nhạt khi chạy trên Android.

## 🐛 Xử lý sự cố

### Lỗi: "Android SDK not found"
```bash
# Thiết lập path Android SDK
export ANDROID_HOME=/path/to/android-sdk
export PATH=$PATH:$ANDROID_HOME/tools
```

### Lỗi: "Java not found"
```bash
# Cài đặt JDK
# Windows: Download từ oracle.com
# Linux: sudo apt-get install default-jdk
# macOS: brew install openjdk
```

### Emulator quá chậm?
- Sử dụng x86_64 image (không ARM)
- Cấp thêm RAM cho emulator
- Sử dụng điện thoại thực thay vì emulator

## 📊 Cấu trúc dự án

```
Mario/
├── Player.cs              # Touch input support
├── MarioGame.cs           # Android fullscreen support
├── TileMap.cs
├── Camera.cs
├── Program.cs
├── Mario.csproj           # Multi-platform
├── Platforms/
│   └── Android/
│       ├── MainActivity.cs
│       └── AndroidManifest.xml
└── README_ANDROID.md
```

## 📝 Ghi chú

- Game tự động phát hiện platform (Windows/Android)
- Touch input chỉ hoạt động trên Android
- Keyboard input hoạt động trên cả hai
- Fullscreen tự động bật trên Android

## 🎯 Tiếp theo

- [ ] Thêm sprites thay vì hình chữ nhật
- [ ] Thêm âm thanh/nhạc
- [ ] Thêm enemies
- [ ] Thêm collectibles
- [ ] Tối ưu hóa performance
- [ ] Signed APK release

---

**Happy Gaming! 🍄🎮**