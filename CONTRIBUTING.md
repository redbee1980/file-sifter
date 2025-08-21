# Contributing to file-sifter

ようこそ！file-sifter への貢献方法を簡潔にまとめています。  
まだ初期段階 (MVP) のため、ガイドは最小構成です。

---

## 1. セットアップ

```bash
git clone https://github.com/redbee1980/file-sifter.git
cd file-sifter
dotnet build
dotnet run --project src/FileSifter
```

settings.json は `%APPDATA%/FileSifter/settings.json` に生成されます。

---

## 2. ブランチ戦略

- main: 安定 / リリース相当
- feature/*: 機能追加 (例: feature/exclude-patterns)
- fix/*: 不具合修正
- chore/*, docs/* なども可

PR は main 宛て。可能なら 1 PR = 1 トピック。

---

## 3. コミットメッセージ規約

基本は Conventional Commits を採用します（詳細は `docs/commits.md` 参照）。

例:
```
feat: add excludeExtensions groundwork
fix: correct null handling in DiffService
docs: add Japanese README
chore: bump Serilog version
refactor: simplify copy service branching
perf: reduce memory allocations in hashing
test: add unit tests for ExtensionFilter
build: add CI pipeline
ci: cache dotnet packages in Actions
```

Breaking change:
```
feat!: change default hash mode to sha256
```
または本文内:
```
BREAKING CHANGE: default hash mode changed to sha256
```

---

## 4. コードスタイル

- C# 10+ / .NET 8
- Nullable enable / implicit usings (既定)
- var の使用: 右辺型が明白なら var
- 名前: PascalCase (public), _camelCase (private fields)
- 150 行を超える巨大クラスは分割を検討

将来的に `.editorconfig` を追加予定。

---

## 5. テスト（将来）

現状ユニットテスト未導入。test プロジェクト追加予定:
`tests/FileSifter.Tests/`

---

## 6. Issue / PR

- Issue: バグは再現手順、改善は背景を明示
- PR: 概要 / 背景 / 変更点 / スクリーンショット（UI変更時）
- Draft PR を WIP に活用可
- CI が通ること（build job）

---

## 7. ライセンス

本プロジェクトは MIT License。コントリビューションは MIT の下で提供される点に同意してください。

---

## 8. セキュリティ

機密情報 (APIキー, パスワード) をコミットしないでください。  
誤って含めた場合は速やかに削除し履歴対応を検討。

---

## 9. ロードマップ優先例

1. 進捗割合の精緻化
2. 除外パターンサポート
3. includeExtensions UI
4. ハッシュキャッシュ
5. semantic normalization (json)
6. CLI モード

---

## 10. 相談

仕様の不明点や方針相談は Issue (question ラベル) で。

Happy contributing!