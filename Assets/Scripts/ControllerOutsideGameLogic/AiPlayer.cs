using System;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;
using UnityEngine;

using Debug = UnityEngine.Debug;
using PortalPattern = System.UInt32;

struct StartObservation
{
    public int[,] map;
    public int myId;
    public Team myTeam;
}

struct RoutineObservation
{
    public int frame;
    public List<PlayerModel> players;
    public List<BombModel> bombs;
    public Dictionary<PortalPattern, List<PortalModel>> portalsClassifiedByPattern;
}

class ExternalAiAdapter
{
    private Process p = null;
    private string buffer = "";
    private List<string> actionList = new List<string>();
    public ExternalAiAdapter(string fileName, string arguments)
    {
        try
        {
            p = new Process();

            p.EnableRaisingEvents = false;
            p.OutputDataReceived += new DataReceivedEventHandler(DataReceived);

            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;

            p.StartInfo.FileName = fileName;
            p.StartInfo.Arguments = arguments;

            p.Start();
            p.BeginOutputReadLine();

            // Promise to close the process
            AppDomain.CurrentDomain.ProcessExit += new EventHandler((sender, args) =>
            {
                Close();
            });
            GamePresenter.GameEndEvent += new EventHandler((sender, args) =>
            {
                Close();
            });
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to start AI: " + e);
        }
    }

    void DataReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        buffer += eventArgs.Data + '\n';
    }

    public string getAction()
    {
        string action = buffer;
        buffer = "";
        actionList.Add(action);
        return action;
    }

    public void sendObservation(string observation)
    {
        if (p.HasExited)
        {
            return;
        }
        p.StandardInput.WriteLine(observation);
        p.StandardInput.Flush();
    }

    public void Close()
    {
        if (!p.HasExited)
        {
            p.Kill();
        }
    }
}

public class AiPlayer : MonoBehaviour
{
    private JsonSerializerSettings settings;

    int playerId;
    ExternalAiAdapter adapter;

    Queue<dynamic> actionQueue;

    void Awake()
    {
        settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
            {
                // errors.Add(args.ErrorContext.Error.Message);
                args.ErrorContext.Handled = true;
            }
        };
        settings.Converters.Add(new Vector2Converter());
        settings.Converters.Add(new Vector2IntConverter());

        actionQueue = new Queue<dynamic>();
    }

    // Assign the player that this AI should control
    public void Init(int playerId, dynamic config)
    {
        this.playerId = playerId;

        if (config.language == "python")
        {
            adapter = new ExternalAiAdapter(
                "python",
                Application.dataPath + "/../" + (string)config.entryPoint
            );
        }
        else if (config.language == "cpp")
        {
            // TODO: implement
        }
        else
        {
            Debug.LogError("Unknown language: " + config.language);
        }

        // Send start observation
        adapter.sendObservation(EncodeStartObservation());
    }

    void FixedUpdate()
    {
        // Consume action from agent
        string actionString = adapter.getAction();
        foreach (string action in actionString.Split('\n'))
        {
            if (action == "")
                continue;

            dynamic actionObj = JsonConvert.DeserializeObject(action);
            if (actionObj != null)
            {
                actionQueue.Enqueue(actionObj);
            }
            else
            {
                Debug.LogWarning("Invalid action: " + action);
            }
        }

        // Send observation to agent
        adapter.sendObservation(EncodeRoutineObservation());

        // Invoke action
        if (actionQueue.Count > 0 && actionQueue.Peek().frame <= GameModel.Instance.frame)
        {
            MapPresenter.Instance.InterpretAction(playerId, actionQueue.Dequeue());
        }
    }

    private string EncodeStartObservation()
    {
        int[,] map = new int[MapModel.Instance.map.GetLength(0), MapModel.Instance.map.GetLength(1)];
        for (int i = 0; i < MapModel.Instance.map.GetLength(0); i++)
        {
            for (int j = 0; j < MapModel.Instance.map.GetLength(1); j++)
            {
                map[i, j] = MapModel.Instance.map[i, j].isObstacle ? 1 : 0;
            }
        }
        string observation_str = JsonConvert.SerializeObject(new StartObservation
        {
            map = map,
            myId = playerId,
            myTeam = MapModel.Instance.players[playerId].team
        }, Formatting.None, settings);
        return observation_str;
    }

    private string EncodeRoutineObservation()
    {
        string observation_str = JsonConvert.SerializeObject(new RoutineObservation
        {
            frame = GameModel.Instance.frame,
            players = MapModel.Instance.players,
            bombs = MapModel.Instance.bombs,
            portalsClassifiedByPattern = MapModel.Instance.portalsClassifiedByPattern
        }, Formatting.None, settings);
        return observation_str;
    }
}
