# Win11Optimizer

Windows 11 軽量化・プライバシー最適化ツール  
**バージョン 2026.03** — .NET 10 + C# 13 + WPF

## 生成AI使用表明

このツールは、以下の生成AIツールを使用して作成されたものです。
- **Claude Sonnet 4.6** via Perplexity Computer(Visual Studio プロジェクト作成)
- **GPT-5 mini** via GitHub Copilot（内部コード修正）
- **NVIDIA Nemotron 3 Super** etc... via Perplexity （コード修正・自己完結オプション設定）

## ダウンロード

## 動作要件

| 項目 | 要件 |
|------|------|
| OS | Windows 11 (22H2 以降) |
| ランタイム | **.NET 10 Desktop Runtime** (x64) ※バイナリ版の場合はインストール不要 |
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



## 主要機能

- レジストリ一括ツイーク（プライバシー/タスクバー/外観/パフォーマンス/メンテナンス）
- UCPD ドライバー無効化によるウィジェットボタン非表示対策
- 壁紙・ロック画面カスタム画像設定
- ウィジェット完全アンインストール
- 適用前の JSON バックアップ・復元
- 設定の保存とクリーンインストール後の再ロード
- 復元ポイント作成オプション
