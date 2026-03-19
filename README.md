# Win11Optimizer

Windows 11 軽量化・プライバシー最適化ツール  
**バージョン 2.0** — .NET 10 + C# 13 + WPF

## 動作要件

| 項目 | 要件 |
|------|------|
| OS | Windows 11 (22H2 以降) |
| ランタイム | **.NET 10 Desktop Runtime** (x64) |
| 権限 | **管理者権限必須** |
| IDE | Visual Studio 2022 / 2026 Community |

> **ランタイムの入手**  
> `winget install Microsoft.DotNet.DesktopRuntime.10`  
> または [Microsoft .NET ダウンロードページ](https://dotnet.microsoft.com/download/dotnet/10.0)

## ビルド方法

```
git clone ...
cd Win11Optimizer
dotnet build -c Release
```

または Visual Studio で `Win11Optimizer.sln` を開いてビルド。

## .NET 10 モダナイズで変更した点

- `TargetFramework` を `net481` → `net10.0-windows` に変更
- `LangVersion` を `13` に設定、`ImplicitUsings` 有効化
- `Compatibility/IsExternalInit.cs` shim を削除（.NET 10 では不要）
- 全 `.cs` ファイルを **file-scoped namespace** に変換
- コレクション初期化を `new()` → `[]`（コレクション式）に統一
- `List.Find()` / `OrderDescending()` など .NET 6+ API を活用
- `File.WriteAllTextAsync` で非同期 I/O を利用
- `process.WaitForExit()` のタプル分解をより簡潔に記述

## 主要機能

- レジストリ一括ツイーク（プライバシー/タスクバー/外観/パフォーマンス/メンテナンス）
- UCPD ドライバー無効化によるウィジェットボタン非表示対策
- 壁紙・ロック画面カスタム画像設定
- ウィジェット完全アンインストール
- 適用前の JSON バックアップ・復元
- 設定の保存とクリーンインストール後の再ロード
- 復元ポイント作成オプション
