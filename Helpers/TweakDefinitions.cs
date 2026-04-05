using System.Collections.Generic;
using Win11Optimizer.Models;

namespace Win11Optimizer.Helpers;

/// <summary>
/// 絞り込み後のツイーク定義
/// </summary>
public static class TweakDefinitions
{
    public static List<TweakCategory> GetAllCategories() =>
    [
        BuildRequiredCategory(),
        BuildCorePrivacyCategory(),
        BuildCoreCloudCategory(),
        BuildOptionalCategory()
    ];

    private static TweakCategory BuildCorePrivacyCategory() => new()
    {
        Name = "プライバシー設定",
        Items =
        [
            new()
            {
                Id = "AdvertisingId",
                Category = "プライバシー",
                Name = "広告識別子を無効化",
                Description = "アプリによるパーソナライズ広告用の広告 ID 利用を無効にします。",
                RegistryEntries =
                [
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", ValueName="Enabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\CPSS\Store\AdvertisingInfo", ValueName="Value", ValueData="0", DefaultData="1", ValueType="DWORD" }
                ]
            },
            new()
            {
                Id = "SuggestedContent",
                Category = "プライバシー",
                Name = "おすすめコンテンツ・オファーを無効化",
                Description = "ContentDeliveryManager による広告やおすすめ表示を抑止します。",
                RegistryEntries =
                [
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", ValueName="SubscribedContent-338393Enabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", ValueName="SubscribedContent-353694Enabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", ValueName="SubscribedContent-332596Enabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", ValueName="SubscribedContent-353696Enabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", ValueName="SystemPaneSuggestionsEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" }
                ]
            },
            new()
            {
                Id = "DiagnosticData",
                Category = "プライバシー",
                Name = "オプションの診断データ送信を無効化",
                Description = "Windows が送信する追加診断データをオフにします。",
                RegistryEntries =
                [
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Diagnostics\DiagTrack", ValueName="DiagnosticDataOptIn", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Diagnostics\DiagTrack", ValueName="ShowedToastAtLevel", ValueData="0", DefaultData="1", ValueType="DWORD" }
                ]
            },
            new()
            {
                Id = "AppTracking",
                Category = "プライバシー",
                Name = "アプリ起動追跡を無効化",
                Description = "スタートメニュー向けのアプリ使用頻度追跡を止めます。",
                RegistryEntries =
                [
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", ValueName="Start_TrackProgs", ValueData="0", DefaultData="1", ValueType="DWORD" }
                ]
            },
            new()
            {
                Id = "RecentDocs",
                Category = "プライバシー",
                Name = "最近使ったファイル追跡を無効化",
                Description = "最近開いたファイルの記録と表示を抑止します。",
                RegistryEntries =
                [
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", ValueName="Start_TrackDocs", ValueData="0", DefaultData="1", ValueType="DWORD" }
                ]
            },
            new()
            {
                Id = "TailoredExperiences",
                Category = "プライバシー",
                Name = "カスタマイズされたエクスペリエンスをオフ",
                Description = "診断データ由来のパーソナライズ提案を無効化します。",
                RegistryEntries =
                [
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Privacy", ValueName="TailoredExperiencesWithDiagnosticDataEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" }
                ]
            },
            new()
            {
                Id = "UserEngagement",
                Category = "プライバシー",
                Name = "ユーザープロファイルエンゲージメント無効化",
                Description = "設定確認や案内系ダイアログの表示を抑制します。",
                RegistryEntries =
                [
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\UserProfileEngagement", ValueName="ScoobeSystemSettingEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" }
                ]
            },
            new()
            {
                Id = "Tips",
                Category = "プライバシー",
                Name = "ヒント・新機能紹介を無効化",
                Description = "Windows のヒントやソフトランディング表示をオフにします。",
                RegistryEntries =
                [
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Privacy", ValueName="SoftLandingEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" }
                ]
            },
            new()
            {
                Id = "PreinstalledApps",
                Category = "プライバシー",
                Name = "プリインアプリの自動インストールを無効化",
                Description = "おすすめアプリやサイレント導入を抑止します。",
                RegistryEntries =
                [
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", ValueName="FeatureManagementEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", ValueName="OemPreInstalledAppsEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", ValueName="PreInstalledAppsEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", ValueName="PreInstalledAppsEverEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", ValueName="SilentInstalledAppsEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" }
                ]
            }
        ]
    };

    private static TweakCategory BuildCoreCloudCategory() => new()
    {
        Name = "クラウド連携設定",
        Items =
        [
            new()
            {
                Id = "BingSearch",
                Category = "クラウド連携",
                Name = "Bing / Web 検索連携を無効化",
                Description = "スタートメニューと検索バーの Web 検索連携を停止します。",
                RegistryEntries =
                [
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Search", ValueName="BingSearchEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" }
                ]
            },
            new()
            {
                Id = "CloudSearch",
                Category = "クラウド連携",
                Name = "クラウド検索を無効化",
                Description = "Microsoft アカウントおよび組織アカウントのクラウド検索を無効化します。",
                RegistryEntries =
                [
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\SearchSettings", ValueName="IsMSACloudSearchEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" },
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\SearchSettings", ValueName="IsAADCloudSearchEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" }
                ]
            },
            new()
            {
                Id = "StartRecommendations",
                Category = "クラウド連携",
                Name = "スタートメニューのおすすめコンテンツを無効化",
                Description = "おすすめ欄への推薦コンテンツ表示を止めます。",
                RegistryEntries =
                [
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", ValueName="Start_IrisRecommendations", ValueData="0", DefaultData="1", ValueType="DWORD" }
                ]
            },
            new()
            {
                Id = "RecoWebSites",
                Category = "クラウド連携",
                Name = "閲覧履歴からの Web サイト表示をオフ",
                Description = "ブラウザ履歴ベースのサイト推薦を無効化します。",
                RegistryEntries =
                [
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", ValueName="Start_RecoPersonalizedSites", ValueData="0", DefaultData="1", ValueType="DWORD" }
                ]
            },
            new()
            {
                Id = "SearchHistory",
                Category = "クラウド連携",
                Name = "検索履歴をオフ",
                Description = "検索履歴の記録を無効にします。",
                RegistryEntries =
                [
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\SearchSettings", ValueName="IsDeviceSearchHistoryEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" }
                ]
            }
        ]
    };

    private static TweakCategory BuildRequiredCategory() => new()
    {
        Name = "パフォーマンス向上",
        Items =
        [
            new()
            {
                Id = "FastStartup",
                Category = "必須機能",
                Name = "高速スタートアップを無効化",
                Description = "完全シャットダウンを優先するため Hiberboot を無効にします。",
                RestartRequirement = RestartRequirement.SystemRestart,
                RegistryEntries =
                [
                    new() { Hive="HKLM", KeyPath=@"SYSTEM\CurrentControlSet\Control\Session Manager\Power", ValueName="HiberbootEnabled", ValueData="0", DefaultData="1", ValueType="DWORD" }
                ]
            },
            new()
            {
                Id = "FolderAutoDetect",
                Category = "必須機能",
                Name = "フォルダ自動検出機能を無効化",
                Description = "フォルダ内容に応じた表示テンプレートの自動変更を抑止します。",
                RestartRequirement = RestartRequirement.ExplorerRestart,
                RegistryEntries =
                [
                    new() { Hive="HKCU", KeyPath=@"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell", ValueName="FolderType", ValueData="NotSpecified", DefaultData="", ValueType="STRING" }
                ]
            },
            new()
            {
                Id = "StartupDelay",
                Category = "必須機能",
                Name = "スタートアップ遅延を無効化",
                Description = "サインイン直後のスタートアップアプリ起動遅延を無効化します。",
                RegistryEntries =
                [
                    new() { Hive="HKCU", KeyPath=@"Software\Microsoft\Windows\CurrentVersion\Explorer\Serialize", ValueName="StartupDelayInMSec", ValueData="0", DefaultData="", ValueType="DWORD" }
                ]
            }
        ]
    };

    private static TweakCategory BuildOptionalCategory() => new()
    {
        Name = "オプション機能",
        Items =
        [
            new()
            {
                Id = "NtpNict",
                Category = "オプション",
                Name = "時刻取得先を NICT に設定",
                Description = "NTP サーバーを ntp.nict.jp に変更します。",
                RegistryEntries =
                [
                    new() { Hive="HKLM", KeyPath=@"SYSTEM\CurrentControlSet\Services\W32Time\Parameters", ValueName="NtpServer", ValueData="ntp.nict.jp,0x9", DefaultData="time.windows.com,0x9", ValueType="STRING" },
                    new() { Hive="HKLM", KeyPath=@"SYSTEM\CurrentControlSet\Services\W32Time\Parameters", ValueName="Type", ValueData="NTP", DefaultData="NTP", ValueType="STRING" },
                    new() { Hive="HKLM", KeyPath=@"SOFTWARE\Microsoft\Windows\CurrentVersion\DateTime\Servers", ValueName="1", ValueData="ntp.nict.jp", DefaultData="time.windows.com", ValueType="STRING" }
                ]
            }
        ]
    };
}
