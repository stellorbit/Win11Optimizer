# Win11Optimizer

Windows 11 向けの軽量な最適化ツール。  
現在は機能を大幅に絞り、`プライバシー` と `クラウド連携機能のオフ` を中核にしています。

## 動作要件

| 項目 | 要件 |
|------|------|
| OS | Windows 11 (22H2 以降) |
| 権限 | **管理者権限必須** |
| IDE | Visual Studio 2022 / 2026 Community |

配布用の発行物は自己完結型 (`self-contained`) のため、通常利用時に .NET ランタイムの追加インストールは不要です。

## ビルド方法

```
git clone ...
cd Win11Optimizer
dotnet build -c Release
```

または Visual Studio で `Win11Optimizer.sln` を開いてビルド。

## 発行と配布

Visual Studio の `FolderProfile` で `Release` 発行すると、`J:\W11opt_202604` に自己完結型の単一 EXE が出力されます。

配布は ZIP 化を前提にしてください。

`FolderProfile` はファイルバージョンに連動したフォルダ名へ発行します。

例:
- 発行フォルダ: `J:\W11opt_2026.04\`
- ZIP ファイル: `J:\Win11Optimizer_2026.04.zip`

Visual Studio で `FolderProfile` を使って発行すると、発行完了後に ZIP も自動生成されます。

1. 発行後、`J:\W11opt_<バージョン>\` の中身を確認する
2. 同じドライブ直下に生成された `Win11Optimizer_<バージョン>.zip` を配布物として使う
3. 利用者には ZIP を展開して `Win11Optimizer.exe` を実行してもらう

`Release` 発行では `.pdb` を出力しない設定にしているため、配布物は EXE 中心の最小構成になります。

## 主要機能

- 中核機能: プライバシー関連ツイーク
- 中核機能: クラウド連携機能のオフ
- 必須機能: 高速スタートアップのオフ
- 必須機能: フォルダ自動検出機能のオフ
- 必須機能: スタートアップ遅延オフ
- オプション機能: 時刻取得先を NICT に設定
- 適用前の JSON バックアップ・復元
- 設定の保存とクリーンインストール後の再ロード
- 復元ポイント作成オプション
