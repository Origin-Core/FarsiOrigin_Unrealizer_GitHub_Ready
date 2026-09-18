# FARSI ORIGIN — Unrealizer v0.1

ابزار پایه برای ساخت فارسی‌ساز امن بازی‌های Unreal Engine.

## این نسخه
- اسکن خودکار پوشه بازی
- پیدا کردن EXE و Content/Paks
- تشخیص PAK و IoStore
- پیدا کردن مسیرهای Subtitle / Localization / Widgets / UI
- ساخت Workspace جداگانه
- بانک ترجمه Spanish → Persian
- پردازش پایه Unicode فارسی
- اضافه کردن فونت TTF/OTF
- گزارش اسکن

### اصل امنیتی
فایل‌های اصلی بازی overwrite نمی‌شوند.
Assetهایی که نیاز به Mapping/نسخه‌بندی دارند تا زمانی که روش امنشان مشخص نشود، نباید کورکورانه بازنویسی شوند.

## بیلد محلی
این پروژه برای .NET 10 ساخته شده است.

```powershell
dotnet restore
dotnet build -c Release
dotnet run -c Release
```

ساخت EXE مستقل:

```powershell
dotnet publish src/FarsiOrigin.Unrealizer/FarsiOrigin.Unrealizer.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish
```

## بیلد GitHub
این Repository یک GitHub Actions آماده دارد.

1. کل پروژه را در Repository قرار بده.
2. Push کن.
3. به Actions برو.
4. Workflow `Build Windows` را باز کن.
5. بعد از اتمام، Artifact زیر را دانلود کن:

`FarsiOrigin-Unrealizer-win-x64`

داخل Artifact فایل EXE مستقل قرار دارد.

## وابستگی
CUE4Parse برای لایه Unreal در پروژه قرار داده شده است.
