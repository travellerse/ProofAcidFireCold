# ProofAcidFireCold

[中文](#中文) | [English](#english) | [日本語](#日本語)

## 中文

本mod为游戏物品添加以下特性：

- 🔥 耐火：免疫火焰伤害（可配置是否影响地图烤肉及垃圾防火）
- 🧪 耐酸：免疫酸性伤害
- ❄️ 耐寒：防止冷冻破坏
- 🛡️ 防盗：免疫偷窃效果

## 安装

### 自动安装

1. 订阅[Steam创意工坊](https://steamcommunity.com/sharedfiles/filedetails/?id=3365085645)

### 手动安装

1. 自行构建mod或下载[Release]
2. 将`ProofAcidFireCold`文件夹放入游戏目录的`Elin/Package`文件夹
3. 启动游戏自动生成配置文件

## 配置

配置文件路径：`Elin/BepInEx/config/com.travellerse.plugins.ProofAcidFireCold.cfg`

```ini
[ProofAcidFireCold]
# 是否启用特性（true=启用，false=禁用）

## 基础防护
ProofAcid = true    # 耐酸特性
ProofFire = true    # 耐火特性
ProofCold = true    # 耐寒特性
ProofSteal = true   # 防盗特性

## 特殊规则
MeatOnMapProofFire = false # 设为true时地图火元素无法烤肉
GarbageProofFire = false   # 设为true时垃圾无法燃烧
DisableBlanketsCost = ture # 设为true时毛毯不消耗
```

## 兼容性

- ✅ 不修改存档数据
- ✅ 与大部分物品类mod兼容

## 问题反馈

[GitHub Issues](https://github.com/travellerse/ProofAcidFireCold/issues)

---

## English

This mod adds following protections to in-game items:

- 🔥 Fireproof: Immune to fire damage (configurable for map roasting and garbage(and junk) fireproof)
- 🧪 Acidproof: Immune to acid damage
- ❄️ Coldproof: Prevent freezing damage
- 🛡️ Stealproof: Block stealing effects

## Installation

### Auto Installation

1. Subscribe on [Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3365085645)

### Manual Installation

1. Build the mod yourself or download from [Release]
2. Put the `ProofAcidFireCold` folder into the `Elin/Package` folder of the game directory
3. Start the game to generate the configuration file

## Configuration

Config path: `Elin/BepInEx/config/com.travellerse.plugins.ProofAcidFireCold.cfg`

```ini
[ProofAcidFireCold]
# Toggle features (true=enable, false=disable)

## Basic Protections
ProofAcid = true    # Acid immunity
ProofFire = true    # Fire immunity
ProofCold = true    # Freezing prevention
ProofSteal = true   # Steal prevention

## Special Rules
MeatOnMapProofFire = false # When true, disable meat roasting by map fire
GarbageProofFire = false   # When true, disable garbage burning
DisableBlanketsCost = ture # When true, blankets do not consume
```

## Compatibility

- ✅ No save game modification
- ✅ Compatible with most item mods

## Issues & Support

[GitHub Issues](https://github.com/travellerse/ProofAcidFireCold/issues)

---

## 日本語

本MODはアイテムに以下の特性を追加します：

- 🔥 耐火：火傷ダメージ無効（マップ焼肉設定可）
- 🧪 耐酸：酸性ダメージ無効
- ❄️ 耐寒：凍結破壊防止
- 🛡️ 防犯：盗難効果防止

## インストール

### 自動インストール

1. [Steamワークショップ](https://steamcommunity.com/sharedfiles/filedetails/?id=3365085645)でサブスクライブ

### 手動インストール

1. 自分でMODをビルドするか、[Release]からダウンロード
2. `ProofAcidFireCold`フォルダをゲームディレクトリの`Elin/Package`フォルダに配置
3. ゲームを起動して設定ファイルを生成

## 設定

設定ファイル：`Elin/BepInEx/config/com.travellerse.plugins.ProofAcidFireCold.cfg`

```ini
[ProofAcidFireCold]
# 機能制御（true=有効，false=無効）

## 基本保護
ProofAcid = true    # 耐酸特性
ProofFire = true    # 耐火特性
ProofCold = true    # 耐寒特性
ProofSteal = true   # 盗難防止

## 特殊規則
MeatOnMapProofFire = false # true時マップ火元素で肉焼不可
GarbageProofFire = false   # true時ゴミ燃焼不可
DisableBlanketsCost = ture # true時毛布消費無効
```

## 互換性

- ✅ セーブデータ変更なし
- ✅ 大半のアイテムMODと互換

## 問題報告

[GitHub Issues](https://github.com/travellerse/ProofAcidFireCold/issues)
