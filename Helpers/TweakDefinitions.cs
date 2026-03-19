using System;
using System.Collections.Generic;
using System.Security.Principal;
using Win11Optimizer.Models;

namespace Win11Optimizer.Helpers;
/// <summary>
/// 全ツイーク定義を返すファクトリ
/// </summary>
public static class TweakDefinitions
{
    /// <summary>
    /// 現在のユーザーの SID を取得する（ロック画面 HKLM パス用）
    /// </summary>
    private static string CurrentUserSid =>
        WindowsIdentity.GetCurrent().User?.Value ?? "";

    public static List<TweakCategory> GetAllCategories()
    {
        return new List<TweakCategory>
        {
            BuildPrivacyCategory(),
            BuildTaskbarCategory(),
            BuildSystemCategory(),
            BuildAppearanceCategory(),
            BuildPerformanceCategory(),
            BuildMaintenanceCategory()
        };
    }

    // ========== プライバシー ==========
    private static TweakCategory BuildPrivacyCategory() => new()
    {
        Name = "🔒 プライバシー",
        Items = new List<TweakItem>
        {
            new()
            {
                Id          = "BingSearch",
                Category    = "プライバシー",
                Name        = "スタートメニュー Bing 検索連携を無効化",
                Description = "スタートメニューと検索バーの Bing / Web 検索を無効にします。",
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Search",
                            ValueName="BingSearchEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "AdvertisingId",
                Category    = "プライバシー",
                Name        = "広告識別子を無効化",
                Description = "アプリが広告 ID を使ったパーソナライズ広告を配信できないようにします。",
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo",
                            ValueName="Enabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\CPSS\Store\AdvertisingInfo",
                            ValueName="Value", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "SuggestedContent",
                Category    = "プライバシー",
                Name        = "おすすめコンテンツ・推奨事項・オファーを無効化",
                Description = "ContentDeliveryManager による広告・おすすめコンテンツをすべて無効にします。",
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                            ValueName="SubscribedContent-338393Enabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                            ValueName="SubscribedContent-353694Enabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                            ValueName="SubscribedContent-332596Enabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                            ValueName="SubscribedContent-353696Enabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                            ValueName="SystemPaneSuggestionsEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "CloudSearch",
                Category    = "プライバシー",
                Name        = "クラウド検索を無効化 (個人・組織)",
                Description = "Microsoft アカウントおよび Azure AD のクラウド検索連携を無効にします。",
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\SearchSettings",
                            ValueName="IsMSACloudSearchEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\SearchSettings",
                            ValueName="IsAADCloudSearchEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "DiagnosticData",
                Category    = "プライバシー",
                Name        = "オプションの診断データ送信を無効化",
                Description = "Windows がマイクロソフトへ送信する追加の診断データをオフにします。",
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Diagnostics\DiagTrack",
                            ValueName="DiagnosticDataOptIn", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Diagnostics\DiagTrack",
                            ValueName="ShowedToastAtLevel", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "AppTracking",
                Category    = "プライバシー",
                Name        = "アプリケーション追跡を無効化",
                Description = "スタートメニューがアプリの起動頻度を追跡しないようにします。",
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                            ValueName="Start_TrackProgs", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "RecentDocs",
                Category    = "プライバシー",
                Name        = "スタートメニューの「最近使ったファイル」追跡を無効化",
                Description = "最近開いたファイルをスタートメニューやタスクバーに表示しません。",
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                            ValueName="Start_TrackDocs", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "TailoredExperiences",
                Category    = "プライバシー",
                Name        = "「カスタマイズされたエクスペリエンス」情報送信をオフ",
                Description = "診断データに基づいたパーソナライズ提案を無効にします。",
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Privacy",
                            ValueName="TailoredExperiencesWithDiagnosticDataEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "UserEngagement",
                Category    = "プライバシー",
                Name        = "ユーザープロファイルエンゲージメント無効化",
                Description = "OOBE/Scoobe の設定確認ダイアログ（ScoobeSystemSettingEnabled）をオフにします。",
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\UserProfileEngagement",
                            ValueName="ScoobeSystemSettingEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "Tips",
                Category    = "プライバシー",
                Name        = "ヒント・ソフトランディング無効化",
                Description = "Windows が表示するヒントや新機能紹介をオフにします。",
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Privacy",
                            ValueName="SoftLandingEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "PreinstalledApps",
                Category    = "プライバシー",
                Name        = "プリインアプリの自動インストールを無効化",
                Description = "OEM や MS がサイレントにアプリをインストールしないようにします。",
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                            ValueName="FeatureManagementEnabled",      ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                            ValueName="OemPreInstalledAppsEnabled",    ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                            ValueName="PreInstalledAppsEnabled",       ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                            ValueName="PreInstalledAppsEverEnabled",   ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                            ValueName="SilentInstalledAppsEnabled",    ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "ProxyAutoDetect",
                Category    = "プライバシー",
                Name        = "プロキシの自動検出をオフ (WPAD 無効化)",
                Description = "WinHTTP Web Proxy Auto-Discovery (WPAD) を無効にします。LAN 設定の「自動検出」もオフになります。",
                RestartRequirement = RestartRequirement.None,
                RegistryEntries = new(), // PowerShell で処理
                PowerShellEnableCommand  = "New-ItemProperty -Path 'HKLM:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\WinHttp' -Name DisableWpad -Value 1 -PropertyType DWORD -Force; " +
                                           "$key='HKCU:\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Connections'; " +
                                           "[byte[]]$d=(Get-ItemProperty -Path $key).DefaultConnectionSettings; $d[8]=$d[8] -band 0xFD; Set-ItemProperty -Path $key -Name DefaultConnectionSettings -Value $d -Type Binary",
                PowerShellDisableCommand = "Remove-ItemProperty -Path 'HKLM:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\WinHttp' -Name DisableWpad -ErrorAction SilentlyContinue; " +
                                           "$key='HKCU:\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Connections'; " +
                                           "[byte[]]$d=(Get-ItemProperty -Path $key).DefaultConnectionSettings; $d[8]=$d[8] -bor 0x02; Set-ItemProperty -Path $key -Name DefaultConnectionSettings -Value $d -Type Binary"
            },
        }
    };

    // ========== タスクバー ==========
    private static TweakCategory BuildTaskbarCategory() => new()
    {
        Name = "📌 タスクバー",
        Items = new List<TweakItem>
        {
            new()
            {
                Id          = "TaskViewButton",
                Category    = "タスクバー",
                Name        = "タスクビューボタンを非表示",
                Description = "タスクバーのタスクビューボタンを非表示にします。",
                RestartRequirement = RestartRequirement.ExplorerRestart,
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                            ValueName="ShowTaskViewButton", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "TaskbarCornerDesktop",
                Category    = "タスクバー",
                Name        = "タスクバー隅クリックのデスクトップ表示をオフ",
                Description = "タスクバー右端クリックでデスクトップを表示する機能をオフにします。",
                RestartRequirement = RestartRequirement.ExplorerRestart,
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                            ValueName="TaskbarSd", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "SearchButton",
                Category    = "タスクバー",
                Name        = "タスクバー検索ボタンを非表示",
                Description = "タスクバーの検索アイコンを非表示にします。",
                RestartRequirement = RestartRequirement.ExplorerRestart,
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Search",
                            ValueName="SearchboxTaskbarMode", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Search",
                            ValueName="SearchboxTaskbarModeCache", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id              = "WidgetsButton",
                Category        = "タスクバー",
                Name            = "ウィジェットボタンを非表示 (UCPD 対策あり)",
                Description     = "UCPDドライバーを一時無効化してウィジェットボタンを非表示にします。再起動が必要です。",
                RestartRequirement = RestartRequirement.SystemRestart,
                RequiresUcpdDisable = true,
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                            ValueName="TaskbarDa", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    // Policies にも設定（GPO 相当、UCPD が有効でも効く場合あり）
                    new() { Hive="HKLM", KeyPath=@"SOFTWARE\Policies\Microsoft\Dsh",
                            ValueName="AllowNewsAndInterests", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "TaskbarAlignment",
                Category    = "タスクバー",
                Name        = "タスクバーを左揃えにする",
                Description = "タスクバーのアイコンを中央揃えから左揃えに変更します。",
                RestartRequirement = RestartRequirement.ExplorerRestart,
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                            ValueName="TaskbarAl", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "SearchHighlight",
                Category    = "タスクバー",
                Name        = "検索ハイライトをオフ",
                Description = "検索ボックスに表示される今日のハイライトをオフにします。",
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\SearchSettings",
                            ValueName="IsDynamicSearchBoxEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
        }
    };

    // ========== スタートメニュー ==========
    private static TweakCategory BuildSystemCategory() => new()
    {
        Name = "🚀 スタートメニュー・検索",
        Items = new List<TweakItem>
        {
            new()
            {
                Id          = "StartRecommendations",
                Category    = "スタートメニュー",
                Name        = "スタートメニューのおすすめコンテンツを無効化",
                Description = "スタートメニューの「おすすめ」セクションに AI 推薦コンテンツを表示しません。",
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                            ValueName="Start_IrisRecommendations", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "FrequentApps",
                Category    = "スタートメニュー",
                Name        = "よく使うアプリの表示を無効化",
                Description = "スタートメニューに頻度の高いアプリを表示しません。",
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Start",
                            ValueName="ShowFrequentList", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "RecoWebSites",
                Category    = "スタートメニュー",
                Name        = "閲覧履歴からの Web サイト表示をオフ",
                Description = "ブラウザ履歴に基づいてスタートメニューにサイトを推薦しません。",
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                            ValueName="Start_RecoPersonalizedSites", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "SearchHistory",
                Category    = "スタートメニュー",
                Name        = "検索履歴をオフ",
                Description = "デバイスの検索履歴の記録を無効にします。",
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\SearchSettings",
                            ValueName="IsDeviceSearchHistoryEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "FolderAutoDetect",
                Category    = "スタートメニュー",
                Name        = "フォルダ自動検出機能 (FolderType) を無効化",
                Description = "エクスプローラーがフォルダの種類を自動検出してレイアウトを変えないようにします。",
                RestartRequirement = RestartRequirement.ExplorerRestart,
                RegistryEntries = new()
                {
                    new() { Hive="HKCU",
                            KeyPath=@"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell",
                            ValueName="FolderType", ValueData="NotSpecified", DefaultData="", ValueType="STRING" }
                }
            },
        }
    };

    // ========== 外観 ==========
    private static TweakCategory BuildAppearanceCategory() => new()
    {
        Name = "🎨 外観・UI",
        Items = new List<TweakItem>
        {
            new()
            {
                Id          = "ClassicContextMenu",
                Category    = "外観",
                Name        = "従来の右クリックメニューに戻す",
                Description = "Windows 11 の新しいコンテキストメニューを Windows 10 風の旧式メニューに戻します。",
                RestartRequirement = RestartRequirement.ExplorerRestart,
                RegistryEntries = new()
                {
                    new() { Hive="HKCU",
                            KeyPath=@"SOFTWARE\CLASSES\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32",
                            ValueName="", ValueData="", DefaultData="DELETE", ValueType="STRING",
                            DeleteOnDisable = true }
                }
            },
            new()
            {
                Id          = "LockScreenTrivia",
                Category    = "外観",
                Name        = "ロック画面のトリビア・ヒントを無効化",
                Description = "ロック画面に表示されるスポットライト・豆知識をオフにします。",
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                            ValueName="RotatingLockScreenOverlayEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                            ValueName="RotatingLockScreenEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                            ValueName="SubscribedContent-338387Enabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKLM",
                            KeyPath=$@"SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\Creative\{CurrentUserSid}",
                            ValueName="RotatingLockScreenOverlayEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKLM",
                            KeyPath=$@"SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\Creative\{CurrentUserSid}",
                            ValueName="RotatingLockScreenEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "DefaultWallpaper",
                Category    = "外観",
                Name        = "壁紙を Windows デフォルトに設定",
                Description = @"壁紙を C:\Windows\Web\Wallpaper\Windows\img0.jpg に戻します。",
                RestartRequirement = RestartRequirement.None,
                RegistryEntries = new(),
                PowerShellEnableCommand  = @"Add-Type -TypeDefinition 'using System;using System.Runtime.InteropServices;public class W{[DllImport(""user32.dll"")]public static extern bool SystemParametersInfo(int a,int b,string c,int d);}'; [W]::SystemParametersInfo(0x14,0,$null,3); Start-Sleep 1; [W]::SystemParametersInfo(0x14,0,'C:\Windows\Web\Wallpaper\Windows\img0.jpg',3)",
                PowerShellDisableCommand = "" // 戻す手段なし
            },
            new()
            {
                Id          = "DefaultLockScreen",
                Category    = "外観",
                Name        = "ロック画面を既定画像に設定",
                Description = @"ロック画面を C:\Windows\Web\Screen\img100.jpg に設定します。",
                RestartRequirement = RestartRequirement.None,
                RegistryEntries = new(),
                PowerShellEnableCommand  = @"$regPath='HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\PersonalizationCSP'; " +
                                           @"New-Item -Path $regPath -Force | Out-Null; " +
                                           @"Set-ItemProperty -Path $regPath -Name LockScreenImagePath -Value 'C:\Windows\Web\Screen\img100.jpg' -Force; " +
                                           @"Set-ItemProperty -Path $regPath -Name LockScreenImageUrl -Value 'C:\Windows\Web\Screen\img100.jpg' -Force; " +
                                           @"Set-ItemProperty -Path $regPath -Name LockScreenImageStatus -Value 1 -Type DWORD -Force",
                PowerShellDisableCommand = @"Remove-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\PersonalizationCSP' " +
                                           @"-Name LockScreenImagePath,LockScreenImageUrl,LockScreenImageStatus -ErrorAction SilentlyContinue"
            },
            new()
            {
                Id               = "CustomWallpaper",
                Category         = "外観",
                Name             = "壁紙を任意の画像に設定",
                Description      = "「参照」ボタンでファイルを選択し、デスクトップ壁紙を即時変更します。",
                RestartRequirement = RestartRequirement.None,
                IsFilePickerItem = true,
                FilePickerFilter = "画像ファイル|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                RegistryEntries  = new(),
                PowerShellEnableCommand  = "",  // MainWindow 側でパスを埋め込んで生成
                PowerShellDisableCommand = ""
            },
            new()
            {
                Id               = "CustomLockScreen",
                Category         = "外観",
                Name             = "ロック画面を任意の画像に設定",
                Description      = "「参照」ボタンでファイルを選択し、ロック画面画像を変更します（PersonalizationCSP）。",
                RestartRequirement = RestartRequirement.None,
                IsFilePickerItem = true,
                FilePickerFilter = "画像ファイル|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                RegistryEntries  = new(),
                PowerShellEnableCommand  = "",  // MainWindow 側でパスを埋め込んで生成
                PowerShellDisableCommand = ""
            },
            new()
            {
                Id          = "WindowSnap",
                Category    = "外観",
                Name        = "ウィンドウスナップをオフ",
                Description = "ウィンドウを画面端にドラッグしても自動でスナップしないようにします。",
                RegistryEntries = new()
                {
                    new() { Hive="HKCU", KeyPath=@"Control Panel\Desktop",
                            ValueName="WindowArrangementActive", ValueData="0", DefaultData="1", ValueType="DWORD" }
                }
            },
        }
    };

    // ========== パフォーマンス ==========
    private static TweakCategory BuildPerformanceCategory() => new()
    {
        Name = "⚡ パフォーマンス・システム",
        Items = new List<TweakItem>
        {
            new()
            {
                Id          = "FastStartup",
                Category    = "パフォーマンス",
                Name        = "高速スタートアップを無効化",
                Description = "高速スタートアップ (Hiberboot) を無効にします。完全シャットダウンが確実になります。再起動が必要です。",
                RestartRequirement = RestartRequirement.SystemRestart,
                RegistryEntries = new()
                {
                    new() { Hive="HKLM",
                            KeyPath=@"SYSTEM\CurrentControlSet\Control\Session Manager\Power",
                            ValueName="HiberbootEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKLM",
                            KeyPath=@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FlyoutMenuSettings",
                            ValueName="ShowHibernateOption", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKLM",
                            KeyPath=@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FlyoutMenuSettings",
                            ValueName="ShowSleepOption", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKLM",
                            KeyPath=@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FlyoutMenuSettings",
                            ValueName="ShowLockOption", ValueData="1", DefaultData="1", ValueType="DWORD" }
                }
            },
            new()
            {
                Id          = "NtpNict",
                Category    = "パフォーマンス",
                Name        = "時刻の取得先を NICT に変更",
                Description = "Windows Time サービスの NTP サーバーを ntp.nict.jp（日本標準時）に変更します。",
                RestartRequirement = RestartRequirement.None,
                RegistryEntries = new()
                {
                    new() { Hive="HKLM",
                            KeyPath=@"SYSTEM\CurrentControlSet\Services\W32Time\Parameters",
                            ValueName="NtpServer", ValueData="ntp.nict.jp,0x9", DefaultData="time.windows.com,0x9", ValueType="STRING" },
                    new() { Hive="HKLM",
                            KeyPath=@"SYSTEM\CurrentControlSet\Services\W32Time\Parameters",
                            ValueName="Type", ValueData="NTP", DefaultData="NTP", ValueType="STRING" },
                    new() { Hive="HKLM",
                            KeyPath=@"SOFTWARE\Microsoft\Windows\CurrentVersion\DateTime\Servers",
                            ValueName="1", ValueData="ntp.nict.jp", DefaultData="time.windows.com", ValueType="STRING" }
                }
            },
        }
    };

    // ========== メンテナンス ==========
    private static TweakCategory BuildMaintenanceCategory() => new()
    {
        Name = "🛠️ メンテナンス・ツール",
        Items = new List<TweakItem>
        {
            new()
            {
                Id          = "WingetRemoveMsstore",
                Category    = "メンテナンス",
                Name        = "winget から msstore ソースを削除",
                Description = "winget のパッケージソース一覧から Microsoft Store を除去します（管理者権限必要）。",
                RegistryEntries = new(),
                PowerShellEnableCommand  = "winget source remove msstore --disable-interactivity 2>&1",
                PowerShellDisableCommand = "winget source add msstore 'https://storeedge.imp.microsoft.com/storecatalogppe.appx' --type Microsoft.Rest 2>&1"
            },
            new()
            {
                Id          = "InstallChocolatey",
                Category    = "メンテナンス",
                Name        = "Chocolatey パッケージマネージャーをインストール",
                Description = "Windows 向けパッケージマネージャー Chocolatey をインストールします。",
                RegistryEntries = new(),
                PowerShellEnableCommand  = "Set-ExecutionPolicy Bypass -Scope Process -Force; " +
                                           "[System.Net.ServicePointManager]::SecurityProtocol=[System.Net.ServicePointManager]::SecurityProtocol -bor 3072; " +
                                           "iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))",
                PowerShellDisableCommand = "" // アンインストールはユーザー判断
            },
            new()
            {
                Id          = "WindowsUpdateCleanup",
                Category    = "メンテナンス",
                Name        = "Windows Update ファイルのクリーンアップ",
                Description = "DISM によるコンポーネントストアクリーンアップと $Windows.~BT 等の残骸フォルダを削除します。",
                RegistryEntries = new(),
                PowerShellEnableCommand  = "dism.exe /Online /Cleanup-Image /StartComponentCleanup /ResetBase; " +
                                           "Remove-Item -Path 'C:\\Windows\\SoftwareDistribution\\Download\\*' -Recurse -Force -ErrorAction SilentlyContinue; " +
                                           "Remove-Item 'C:\\$Windows.~BT' -Recurse -Force -ErrorAction SilentlyContinue; " +
                                           "Remove-Item 'C:\\$Windows.~WS' -Recurse -Force -ErrorAction SilentlyContinue; " +
                                           "Write-Output 'クリーンアップ完了'",
                PowerShellDisableCommand = ""
            },
            new()
            {
                Id          = "UninstallWidgets",
                Category    = "メンテナンス",
                Name        = "ウィジェット (Windows Web Experience Pack) を完全アンインストール",
                Description = "Get-AppxPackage *WebExperience* | Remove-AppxPackage を実行してウィジェット機能を完全に削除します。再インストールは winget から可能です。",
                RegistryEntries = new(),
                PowerShellEnableCommand  = "Get-AppxPackage *WebExperience* | Remove-AppxPackage",
                PowerShellDisableCommand = "winget install --id 9MSSGKG348SP --accept-package-agreements --accept-source-agreements --silent"
            },
        }
    };
}
