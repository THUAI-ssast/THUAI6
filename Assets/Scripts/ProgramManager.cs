using System.IO;
using Newtonsoft.Json;
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
        // Read the config
        string configString = null;
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            configString = ReadDefaultConfig();
        }
        else
        {
            
            string[] args = System.Environment.GetCommandLineArgs();
            bool hasConfigArg = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--config")
                {
                    string path = args[i + 1];
                    configString = TryReadCustomConfig(path);
                    hasConfigArg = true;
                }
                if (args[i] == "-batchmode")
                {
                    isBatchMode = true;
                }
            }
            if (!hasConfigArg)
            {
                configString = TryReadCustomConfig();
            }
        }
        configObject = JsonConvert.DeserializeObject<Config>(configString);
    }

    private void Start()
    {
        // Set up the game accordingly
        Time.timeScale = configObject.timeScale;
        if (configObject.data.replayPath != null)
        {
            // TODO: load the replay from the path
        }
        else if (configObject.data.players != null)
        {
            gameObject.AddComponent<Recorder>();
            GameModel.Instance.SetTimeLeft(configObject.gameTime);

            // Set up player controllers
            for (int playerId = 0; playerId < configObject.data.players.Count; playerId++)
            {
                dynamic playerConfig = configObject.data.players[playerId];
                GameObject playerObject = MapPresenter.Instance.GetPlayerObject(playerId);
                if (playerConfig.type == "ai")
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
        string fullPath = Application.dataPath + "/../" + path;
        if (File.Exists(fullPath))
        {
            return File.ReadAllText(fullPath);
        }
        else
        {
            return ReadDefaultConfig();
        }
    }
}
