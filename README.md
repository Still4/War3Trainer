# War3Trainer(WarCraft III Trainer)

这是我[blog](http://tctianchi.duapp.com/)文章中关于魔兽3内存修改器对应的代码。编译好的二进制也请到那里下载。由于是十多年前的旧物所以我也只是姑且搬运到github而已，不一定更新了。

## 修改器简介
修改器不是作弊器，只能在单机上使用；战网上无法胡闹；在宿舍里联机打RPG地图的童鞋需要在每台电脑上做相同的修改动作，才不会掉线。

## 支持的游戏版本
* 1.20.4.6074
* 1.21.0.6263
* 1.21.1.6300
* 1.22.0.6328
* 1.23.0.6352
* 1.24.0.6372
* 1.24.1.6374
* 1.24.2.6378
* 1.24.3.6384
* 1.24.4.6387
* 1.25.1.6397
* 1.26.0.6401
* 1.27.0.52240

## 新版本出现后的更新方法（程序员看这里）
1. War3AddressThisGame
  1. 找到力量
  2. 找到DrawHeroProperty
    int __thiscall DrawHeroProperty(int *GameContext, int **HeroAttributes, int *AttributeBias, unsigned int *GBuffer)
  3. 跟踪智力的显示过程
    辨别方法：
    1. +148改为+94，这是力量
    2. +168改为+A8，这是敏捷
    3. 同理，Storm_578(... "%d"之前，必然还有一次函数调用，这是智力
  4. 稍微跟进1、2个函数就能看到dword_xxx了
2. War3AddressSelectedUnitList
引用"LOCAL_PLAYER"最后几个个函数的下方，用C语言看，
搜索时不必从头开始，从字符串段1/2处开始即可

该函数头部同时有"LOCAL_PLAYER"、"LOCAL_GAME"，末尾形如
```
 if ( !dword_6FAA2FFC )
    dword_6FAA2FFC = sub_6F0074F0();
```
3. War3AddressMoveSpeed
带入修改器代码即可
多数情况下，每次迭代都是同一个数字，只有一次会有所不同
