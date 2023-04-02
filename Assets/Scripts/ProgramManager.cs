using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class Config
{
    public bool render;
    public dynamic data;
}

public class ProgramManager : MonoSingleton<ProgramManager>
{
    private string configString;

    public override void Init()
    {
        // Read the config
        if (Application.platform == RuntimePlatform.WebGLPlayer) {
            configString = ReadDefaultConfig();
        } else {
            configString = TryReadCustomConfig();
        }
        Config configObject = JsonConvert.DeserializeObject<Config>(configString);

        // Set up the game accordingly
        if (!configObject.render)
        {
            Camera.main.enabled = false;
        }
        if (configObject.data.replayPath != null)
        {
            // TODO: load the replay from the path
        }
        else if (configObject.data.players != null)
        {
            foreach (var player in configObject.data.players)
            {
                if (player.type == "human")
                {
                    // TODO: create a human player
                }
                else if (player.type == "ai")
                {
                    string path = player.path;
                    // TODO: create an AI player
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
    private string TryReadCustomConfig()
    {
        string path = Application.dataPath + "/config.json";
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
