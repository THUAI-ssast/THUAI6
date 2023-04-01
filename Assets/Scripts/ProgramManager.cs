using Newtonsoft.Json;
using UnityEngine;

public class Config
{
    public bool render;
    public dynamic data;
}

public class ProgramManager : MonoSingleton<ProgramManager>
{
    public override void Init()
    {
        // Read the config
        TextAsset config = Resources.Load<TextAsset>("config");
        var configObject = JsonConvert.DeserializeObject<Config>(config.text);

        // Set up the game accordingly
        if (!configObject.render)
        {
            Camera.main.enabled = false;
        }
        if (configObject.data.replayPath != null)
        {
            // TODO: load the replay from the path
        } else if (configObject.data.players != null)
        {
            foreach (var player in configObject.data.players)
            {
                if (player.type == "human")
                {
                    // TODO: create a human player
                } else if (player.type == "ai")
                {
                    string path = player.path;
                    // TODO: create an AI player
                }
            }
        }
    }
}
