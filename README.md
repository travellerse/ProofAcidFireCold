# ProofAcidFireCold

## Description
这个mod为物品添加了耐酸、耐火的特性，同时阻止了物品被冷冻破坏。同时添加了一个值configMeatOnMapProofFire，值为true表示不能在地图上用火元素烤肉，默认为false。

This mod adds acid and fire resistance to items while preventing them from being damaged by freezing.A value configMeatOnMapProofFire is also added, with a value of true indicating that fire elements cannot be used to roast meat on the map. The default value is false.

このmodは物品に耐酸性、耐火性の特性を加えて、同時に物品が冷凍破壊されることを阻止します。また、「configMeatOnMapProofFire」という値も追加されました。「true」はマップ上で火の要素でバーベキューができないことを示し、デフォルトは「false」です。

## Config
配置文件位于`Elin\BepInEx\config\com.travellerse.plugins.ProofAcidFireCold.cfg`，可以对耐酸、耐火、耐寒、防盗特性单独进行配置。

Configuration file located in `Elin\BepInEx\config\com.travellerse.plugins.ProofAcidFireCold.cfg`, ProofAcid, ProofFire, ProofCold, ProofSteal feature can be configured separately.

ファイル配置に`Elin\BepInEx\config\com.travellerse.plugins.ProofAcidFireCold.cfg`，が酸に強く、深夜、极寒、防犯の特性を単独で配置した。
```ini
[ProofAcidFireCold]
# ture/false
ProofAcid = true
ProofFire = true
configMeatOnMapProofFire = false
ProofCold = true
ProofSteal = true
```

## Compatibility
这个mod不对存档进行任何修改，仅在读取物品特性时拦截并添加耐酸、耐火，同时阻止冷冻破坏。这意味着这个mod的任何更改是不保留的。

This mod does not make any changes to the game save, only intercepts and adds acid and fire resistance when reading item characteristics, and prevents freezing damage. This means that any changes to this mod are not retained.

このmodはゲームのアーカイブには何の修正も行われていません。ただオブジェクトの特性を読み取る時に遮断し、耐酸性、耐火性を追加し、同時に冷凍破壊を阻止します。これはmodの変更は保留されません

## Source
[[GitHub] https://github.com/travellerse/ProofAcidFireCold](https://github.com/travellerse/ProofAcidFireCold)