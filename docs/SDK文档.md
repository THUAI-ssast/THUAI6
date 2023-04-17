# SDK文档

每个队伍的多名AI由同一份代码「分别」操控，不能直接共享信息。

提供 C++、Python 的 SDK，选手可自行选择：

* [C++ SDK](https://github.com/THUAI-ssast/THUAI6-SDK-Cpp)
* [Python SDK](https://github.com/THUAI-ssast/THUAI6-SDK-Python/releases)

这里以 Python SDK 为例，其目录结构略如：

```
|-- contestant_code.py
|-- sdk/
    |-- actions.py
    |-- datatypes.py
    |-- main.py
    |-- utils.py
```

* 选手需要实现 `contestant_code.py` 中的 `get_action` 函数。
* `main.py` 是程序的入口，在配置文件里配置 AI 时 `entryPoint` 就是该文件的路径。

下面简单介绍 SDK 中提供的信息与工具。可以参考随 SDK 提供的 示例AI。

### 数据类型

`datatypes`文件。

含可能用到的**数据类型**以及相应**常数**（如伤害数值），使选手在开发时能够用上 IDE 的智能提示，且减少对数值的硬编码。

### 信息

信息通过`contestant_code`文件中相应函数的参数提供。

- 身份信息。自己的ID与所属队伍。id为`int`类型的，队伍为`Team`类型。
- 初始地图。含障碍物信息。其类型为 `int[WIDTH][HEIGHT]`，1表示有障碍物，0表示为空地。

- 场上所有玩家的信息。为一个列表，以玩家id作为索引。
- 场上所有炸弹的信息。为一个列表。
- 场上所有传送阵的信息。为一个字典，其键为「传送阵的pattern」，相应的值为「所有该pattern的传送阵」的列表。

### 行动

`actions`文件。

- 移动。
- 转向。
- 射击。
- 修改传送阵的线段。
- 使用传送阵。
- 设置定时炸弹。

### 辅助工具包

utils文件。其中包含：

- 方格中心坐标 与 方格在整张地图中的坐标 的转换
- 计算坐标间的距离、角度。
- 判断某坐标是否在某传送阵上/某炸弹爆炸范围内。
- 某方格是否在地图内/是否可通行
- 指定方格的指定方向能否修改传送阵线段

选手如果有更多需求或建议，可以在交流群中提出。
