# War3Trainer(WarCraft III Trainer)

这是我[blog](http://tctianchi.duapp.com/)文章中关于魔兽3内存修改器对应的代码。编译好的二进制也请到那里下载。由于是十多年前的旧物所以我也只是姑且搬运到github而已，不一定更新了。

![Screenshot](https://raw.githubusercontent.com/tctianchi/War3Trainer/master/Screenshot.png)

## 修改器简介
修改器不是作弊器，只能在单机上使用；战网上无法胡闹；想在宿舍里联机打RPG地图的话需要在每台电脑上做相同的修改动作才不会掉线。

## 支持的游戏版本
* 1.20e（1.20.4.6074）
* 1.21a（1.21.0.6263）
* 1.21b（1.21.1.6300）
* 1.22（1.22.0.6328）
* 1.23（1.23.0.6352）
* 1.24a（1.24.0.6372）
* 1.24b（1.24.1.6374）
* 1.24c（1.24.2.6378）
* 1.24d（1.24.3.6384）
* 1.24e（1.24.4.6387）
* 1.25b（1.25.1.6397）
* 1.26（1.26.0.6401）
* 1.27a（1.27.0.52240）

## 新版本出现后的更新方法（程序员看这里）
如果有下一个版本的魔兽3，我肯定不会马上更新修改器的，你可以用下面的方法更改修改器的代码，达到升级的目的。

升级的关键在于GameContext.cs最后部分，首先找到你game.dll的版本号，将其添加为一组case语句。我的switch (ProcessVersion)有两段，所以这两个switch都需要添加case。随后，逐一找到ThisGameAddress、UnitListAddress、MoveSpeedAddress的值，而AttackAttributesOffset、HeroAttributesOffset、ItemsListOffset、MoveSpeedOffset是一组不变量，不需要修改。

1. 找到ThisGameAddress
    1. 用通用修改器找到英雄的力量，4字节整数，唯一地址
    2. 查找谁访问了这个地址，该地址所在的函数我称为DrawHeroAttributes，定义是：
        ```
        __thiscall DrawHeroAttributes(int *GameContext, int **HeroAttributes, int *AttributeBias, unsigned int *GBuffer)
        ```
    3. 这个函数很有特点，一些颜色字符串的中间穿插了读取命令，其中一定有：
        1. [xxx + 94h]，这是力量
        2. [xxx + A8h]，这是敏捷
        3. 同理，Storm_578(… “%d” …)之前，必然还有一次函数调用，这是智力
    4. 这个函数稍微跟进1、2个函数就能看到常量值dword_xxx，这个xxx就是ThisGameAddress
2. UnitListAddress
    1. 查找字符串”LOCAL_PLAYER”。搜索时不必从头开始，从字符串段1/2处开始即可
    2. 引用该字符串的函数有很多，从最后一个开始看，没有的话看倒数第2个函数
    3. 一定会有一个函数，头部同时有”LOCAL_PLAYER”、”LOCAL_GAME”，末尾形如
        ```
       if ( !dword_6FAA2FFC )
          dword_6FAA2FFC = sub_6F0074F0();
        ```
3. MoveSpeedAddress
    1. 打开修改器源代码，在GameTrainer.cs中有一行注释：“… set breakpoint here …”，在此处设置断点（准确的说应该是这行注释的下面第2行，也就是if语句那里）
    2. 在游戏中选择一个单位，并在修改器中单击刷新按钮，程序会马上运行到这里中断
    3. 这是一个循环结构，tmpAddress2通常情况下是同一个数字，但是有一轮迭代时会是不同的数字，这个数字就是MoveSpeedAddress
