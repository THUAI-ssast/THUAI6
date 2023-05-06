using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Config
{
    public float timeScale;
    public float gameTime;
    public dynamic data;
}

public class ProgramManager : MonoSingleton<ProgramManager>
{
    public Config configObject;
    public bool isBatchMode = false;

    public override void Init()
    {
        // Set the working directory
        Directory.SetCurrentDirectory(Application.dataPath + "/../");

        // Read the config
        string configString = null;
        List<string> aiCommands = new List<string>();
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            configString = ReadDefaultConfig();
        }
        else
        {
            string[] args = System.Environment.GetCommandLineArgs();
            bool hasConfigArg = false;
            // i = 1 to skip the first arg which is the executable path
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--config":
                        string path = args[i + 1];
                        configString = TryReadCustomConfig(path);
                        hasConfigArg = true;
                        // skip the next arg
                        i++;
                        break;
                    case "-batchmode":
                        isBatchMode = true;
                        break;
                    default:
                        // assume it's an AI command
                        aiCommands.Add(args[i]);
                        break;
                }
            }
            if (!hasConfigArg)
            {
                configString = TryReadCustomConfig();
            }
        }
        configObject = JsonConvert.DeserializeObject<Config>(configString);
        // override the data config if there are AI commands
        if (aiCommands.Count > 0)
        {
            configObject.data = new JObject();
            configObject.data.players = new JArray();
            for (int i = 0; i < aiCommands.Count; i++)
            {
                string command = aiCommands[i];
                if (command.StartsWith("isolate --run -b"))
                {
                    // old command: isolate --run -b <num> <other part>
                    // new command: isolate --run -b <playerId> <other part>
                    for (int j = 0; j < GameModel.PlayerCountEachTeam; j++)
                    {
                        int playerId = i * GameModel.PlayerCountEachTeam + j;
                        configObject.data.players.Add(new JObject()
                        {
                            { "command", Regex.Replace(command, "-b \\d+", "-b " + playerId) }
                        });
                    }
                }
                else
                {
                    // one team uses the same ai command
                    for (int j = 0; j < GameModel.PlayerCountEachTeam; j++)
                    {
                        configObject.data.players.Add(new JObject()
                        {
                            { "command", command }
                        });
                    }
                }
            }
        }
    }

    private void Start()
    {
        Time.timeScale = 0.0f;

        // Set up the game accordingly
        if (configObject.data.ContainsKey("replayPath"))
        {
            Replayer replayer = gameObject.AddComponent<Replayer>();
            replayer.Init((string)configObject.data.replayPath);
        }
        else if (configObject.data.ContainsKey("players"))
        {
            gameObject.AddComponent<Recorder>();
            if (configObject.gameTime != 0.0f)
            {
                GameModel.Instance.SetTimeLeft(configObject.gameTime);
            }
            // Set up player controllers
            for (int playerId = 0; playerId < configObject.data.players.Count; playerId++)
            {
                dynamic playerConfig = configObject.data.players[playerId];
                GameObject playerObject = MapPresenter.Instance.GetPlayerObject(playerId);
                if (playerConfig.ContainsKey("command"))
                {
                    // set up AI player
                    AiPlayer aiPlayer = playerObject.AddComponent<AiPlayer>();
                    aiPlayer.Init(playerId, playerConfig);
                }
                else if (playerConfig.type == "human")
                {
                    // TODO: set up human player
                }
            }
        }
        else
        {
            Debug.LogError("No data config found! Quitting...");
            Application.Quit();
        }
        if (configObject.timeScale != 0.0f)
        {
            Time.timeScale = configObject.timeScale;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
        GamePresenter.Instance.GameStart();
    }

    // read default config from Resources folder
    private string ReadDefaultConfig()
    {
        TextAsset configAsset = Resources.Load<TextAsset>("config");
        return configAsset.text;
    }

    // read custom config from data folder if it exists, otherwise read default config
    private string TryReadCustomConfig(string path = "config.json")
    {
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        else
        {
            return ReadDefaultConfig();
        }
    }
}
