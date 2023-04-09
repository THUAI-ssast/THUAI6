# THUAI6

## 部署与运行

以所需平台为 target，使用 Unity 打包。打包后的文件夹中包含可执行文件，双击即可运行。

配置文件默认名为 `config.json`，放在可执行文件同目录下。可根据需要修改配置文件，配置文件格式详见 [`config.schema.json`](https://raw.githubusercontent.com/THUAI6-ssast/Game/main/docs/config.schema.json)。也可查看源码中负责这部分的 [`ProgramManager.cs`](https://github.com/THUAI6-ssast/Game/blob/main/Assets/Scripts/ProgramManager.cs) 文件。参考样例：[默认config](https://github.com/THUAI6-ssast/Game/blob/main/Assets/Resources/config.json)。

> **Note**
> 似乎 Windows 平台下用命令行启动播放器必须使用 `PowerShell`，不能使用 `cmd` 或者 `git bash`，否则 AI 无法正常运行。

可通过命令行参数 `--config <path>` 指定配置文件路径，路径相对于可执行文件目录。

可传入 `-batchmode` 参数，使播放器在运行时不显示窗口。

在非回放模式下，游戏结束后会在可执行文件目录下生成 `record.json` 文件，其中包含了本局游戏的回放数据。可在回放模式下使用该文件进行回放。

### SDK

[SDK文档](docs/SDK文档.md)

## 架构分析

评测、回放、接入人类玩家 这些功能都可通过同一个播放器程序使用，只需使用不同配置文件内容、传入不同的命令行参数。

### 与AI的通信

播放器启动AI程序，二者通过标准输入输出、使用 JSON 格式通信。

### 关于回放

回放文件使用 JSON 格式，记录初始游戏状态与各玩家的操作。
