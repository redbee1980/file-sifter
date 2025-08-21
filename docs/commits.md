# Conventional Commits ガイド (file-sifter)

コミット履歴の可読性向上 & 自動生成系（CHANGELOG など）対応を見据えて、基本的な Conventional Commits ルールを採用します。

## フォーマット

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

### type 一覧（主に使用）

| type | 用途例 |
|------|--------|
| feat | 新機能追加 |
| fix | バグ修正 |
| docs | ドキュメント（README, コメント） |
| style | フォーマットのみ（ロジック変更なし） |
| refactor | リファクタ（外部仕様変えず内部整理） |
| perf | パフォーマンス改善 |
| test | テスト追加 / 修正 |
| build | ビルド設定 / ツールチェーン変更 |
| ci | CI 設定 (.github/workflows) |
| chore | 雑多（依存更新, スクリプト, 設定） |
| revert | 直前コミットの取り消し |

scope は当面任意（例: `feat(scanner): ...`）。

## 例

```
feat: add progress estimation model
fix: handle empty includeExtensions gracefully
docs: add Japanese README
refactor: split DiffService into smaller components
perf: stream hashing to reduce memory usage
ci: add Windows build workflow
build: enable deterministic builds
```

## BREAKING CHANGE

仕様互換性を壊す場合:
```
feat!: change default export folder naming
```
または本文 / フッターに:
```
BREAKING CHANGE: rename ExportFolder field in summary.json to OutputFolder
```

## 複数行本文

説明が必要なときは 1 行空けて詳細:
```
fix: correct hash comparison logic

Previously we compared xxHash states incorrectly when file length was zero.
This patch ensures zero-length files are still hashed and compared deterministically.
```

## なぜこのルールか

- PR の目的が瞬時にわかる
- changelog 自動生成しやすい
- “何が機能追加で何が修正か” を分類可能
- 将来 semantic-release 等統合余地

## Anti-Pattern

| 悪い例 | 理由 |
|--------|------|
| update code | 何をしたか不明 |
| fix stuff | “stuff” で伝わらない |
| wip | 完了状態がわからない（Draft PR で代替） |
| temp | 一時的コミットは squash 推奨 |

## 運用ヒント

- 作業中は細かいコミットでも OK → 最後に必要なら squash / rebase で整理
- 複数の type が混在するなら分割を検討（例: feat + refactor）

Happy committing!