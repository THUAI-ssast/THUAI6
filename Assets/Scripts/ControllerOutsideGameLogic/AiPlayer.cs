using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
    private List<string> actionList = new List<string>();
    public ExternalAiAdapter(string command)
    {
        try
        {
            // get string fileNames and string arguments from string command
            string[] commandParts = command.Split(' ');
            string fileName = commandParts[0];
            string arguments = string.Join(" ", commandParts.Skip(1));
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
            };

            p = new Process();
            p.StartInfo = startInfo;

            p.EnableRaisingEvents = false;
            p.OutputDataReceived += new DataReceivedEventHandler(DataReceived);

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

    void DataReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
        {
            lock (actionList)
            {
                actionList.Add(e.Data);
            }
        }
    }

    public List<string> getActions()
    {
        lock (actionList)
        {
            List<string> actions = new List<string>(actionList);
            actionList.Clear();
            return actions;
        }
    }

    // Use async to avoid blocking the main thread
    public async Task SendObservationAsync(string observation)
    {
        if (p.HasExited)
        {
            return;
        }

        await Task.Run(() =>
        {
            try
            {
                p.StandardInput.WriteLineAsync(observation);
                p.StandardInput.FlushAsync();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to send observation: " + e);
            }
        });
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

        adapter = new ExternalAiAdapter((string)config.command);

        // Send start observation
        adapter.SendObservationAsync(EncodeStartObservation());
    }

    void FixedUpdate()
    {
        // Consume action from agent
        List<string> actionStringList = adapter.getActions();
        foreach (string actionString in actionStringList)
        {
            dynamic action = JsonConvert.DeserializeObject(actionString);
            if (action != null)
            {
                actionQueue.Enqueue(action);
            }
            else
            {
                Debug.LogWarning("Invalid action: " + actionString);
            }
        }

        // Send observation to agent
        adapter.SendObservationAsync(EncodeRoutineObservation());

        // Clear actions outdated too much
        // The more actions in the queue, the less tolerance we have, to compensate for the lag
        // Note: but if the agent is always lagging, the bottleneck could be not here but input buffer
        while (actionQueue.Count > 0 && actionQueue.Peek().frame < GameModel.Instance.frame - 50 / actionQueue.Count)
        {
            actionQueue.Dequeue();
        }
        // Invoke one action if it's time
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
