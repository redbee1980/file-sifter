# file-sifter （日本語版）

2つのフォルダ（Base と Current）を比較し、「新規」または「変更された」ファイルのみを元のディレクトリ構造を保ったまま 1 つの出力フォルダに抽出コピーする Windows 向け WPF アプリです。

> MVP: バイナリレベル（サイズ + 更新日時 + 必要時ハッシュ）での比較。初期対応拡張子は `.png .pdf .jpg .jpeg`（設定ファイルで拡張可能）。画像の視覚的差分表示や PDF レンダリングは行いません。

[English README → README.md](./README.md)

---

## 特徴 (MVP)

- 再帰的フォルダ走査（Base / Current）
- ハイブリッド差分判定: size+mtime が同じなら未変更判定・異なればハッシュ比較（xxHash64 / SHA-256）
- 分類: New / Changed / Unchanged / Removed / Error
- New + Changed のみ出力（相対パス構造保持）
- オプションで removed.txt 生成
- summary.json / summary.txt 出力
- 並列ハッシュ・進捗表示
- キャンセル対応（テンポラリフォルダでアトミック適用 / ロールバック）
- `settings.json` による設定（拡張子 UI は未実装）

### 非対象（MVP 外）

- 可視的画像差分 (overlay, heatmap など)
- PDF ページレンダリング
- ΔE / ピクセル単位色差
- HTML レポート
- CLI / CI 連携
- セマンティック正規化（json / svg など）
- ハッシュキャッシュ

---

## 動作環境

- Windows 10/11 (x64)
- .NET 8 SDK
- （任意）Visual Studio 2022 / Rider / VS Code (C# Dev Kit)

---

## ビルド & 実行

```bash
git clone https://github.com/redbee1980/file-sifter.git
cd file-sifter/src/FileSifter
dotnet build
dotnet run
```

起動後、WPF ウィンドウから操作します。

---

## 典型的な利用手順

1. アプリ起動  
2. Base フォルダ（前回スナップショットなど）選択  
3. Current フォルダ（最新アセットフォルダ）選択  
4. ハッシュアルゴリズム（既定 xxHash64）、既存ファイルポリシー、removed.txt 出力を選択  
5. （任意）出力先フォルダを指定（空なら自動命名）  
6. Start 実行 → 進捗を待機  
7. 完了後、自動でエクスプローラが開き New/Changed ファイルを確認  

---

## 設定ファイル

初回実行時に以下が生成されます:  
`%APPDATA%/FileSifter/settings.json`

デフォルト例:

```json
{
  "hashAlgorithm": "xxhash64",
  "onExisting": "overwrite",
  "generateRemovedList": true,
  "openExplorerAfterExport": true,
  "parallelism": 4,
  "includeExtensions": [".png", ".pdf", ".jpg", ".jpeg"],
  "recentPairs": []
}
```

拡張子を増やしたい場合は `includeExtensions` に追加します（例: ".json", ".docx"）。  
空配列または項目自体を削除するとデフォルト拡張子セットに戻ります。

---

## 出力構成

```
<ExportRoot>/
  summary.json
  summary.txt
  (removed.txt)   # 設定が有効な場合
  <New / Changed ファイル（相対構造保持）>
```

### summary.json 例

```json
{
  "base": "D:/assets/2025-07",
  "current": "D:/assets/2025-08",
  "export": "E:/output/2025-08_vs_2025-07",
  "hashAlgorithm": "xxhash64",
  "targetExtensions": [".png", ".pdf", ".jpg", ".jpeg"],
  "counts": { "new": 20, "changed": 35, "removed": 15, "unchanged": 505, "errors": 0 },
  "timing": { "startedUtc": "2025-08-21T12:00:00Z", "endedUtc": "2025-08-21T12:03:15Z", "durationMs": 195000 }
}
```

---

## ロードマップ（短期）

| フェーズ | 内容 |
|----------|------|
| 1 | 走査 & 分類コア |
| 2 | 並列ハッシュ / 進捗 / キャンセル |
| 3 | コピー & アトミック適用 |
| 4 | summary / removed / error |
| 5 | 設定永続化 / 最近ペア |
| 6 | 仕上げ（ログ / rename ポリシー） |

将来バックログ: 拡張子 UI、除外パターン、ハッシュキャッシュ、セマンティック差分、CLI、HTML レポート など。

---

## ディレクトリ構成（概略）

```
src/FileSifter/
  FileSifter.csproj
  App/
  Domain/
  Infrastructure/
  Services/
  Presentation/
  docs/
```

---

## 既知の注意点

- ハイブリッド方式は “サイズ + mtime 完全一致” の場合ハッシュを省くため、サイズ同一・mtime 同一の微小差分（バイト単位の極小変更）を取りこぼす可能性があります。厳密性が最優先なら将来的に hash_only モード導入予定。  
- JPEG の再エンコード（圧縮差異のみ）でも Changed 判定になります。  
- Office ファイル（docx等）を対象に拡張した場合、メタデータ更新のみでも Changed 判定されることがあります。

---

## ライセンス

MIT License（LICENSE 参照）

---

## 今後の拡張アイデア

- includeExtensions の UI 編集
- excludeExtensions（ブラックリスト）
- 大容量ファイル閾値 (size+mtime だけで判定)
- ハッシュ結果キャッシュ (size+mtime キー)
- セマンティック正規化 (json / svg / docx)
- CLI / CI 連携
- HTML / CSV レポート
- New / Changed 分割出力

---

## 貢献

現段階は内部 MVP。外部コントリビュートフローは今後策定予定。

---

file-sifter で差分抽出を高速 & シンプルに。  
バグ報告 / 改善提案 いつでも歓迎です。