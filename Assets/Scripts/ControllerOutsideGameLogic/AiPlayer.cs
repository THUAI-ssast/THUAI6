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
    const int ACTIONS_CAPACITY = 5;

    private Process p = null;
    private Queue<string> actionsCached = new Queue<string>(ACTIONS_CAPACITY);
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
            lock (actionsCached)
            {

                actionsCached.Enqueue(e.Data);
                if (actionsCached.Count > ACTIONS_CAPACITY)
                {
                    actionsCached.Dequeue();
                }
            }
        }
    }

    public string getAction()
    {
        lock (actionsCached)
        {
            if (actionsCached.Count > 0)
            {
                return actionsCached.Dequeue();
            }
            else
            {
                return null;
            }
        }
    }

    // Use async to avoid blocking the main thread
    public async Task SendObservationAsync(string observation)
    {
        if (p.HasExited)
        {
            return;
        }

        try
        {
            await p.StandardInput.WriteLineAsync(observation);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to send observation: " + e);
        }
        try
        {
            await p.StandardInput.FlushAsync();
        }
        catch (System.Exception _)
        {
            // ignore
        }
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

    void Awake()
    {
        settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
            {
                args.ErrorContext.Handled = true;
            }
        };
        settings.Converters.Add(new Vector2Converter());
        settings.Converters.Add(new Vector2IntConverter());
    }

    // Assign the player that this AI should control
    public async void Init(int playerId, dynamic config)
    {
        this.playerId = playerId;

        adapter = new ExternalAiAdapter((string)config.command);

        // Send start observation
        await adapter.SendObservationAsync(EncodeStartObservation());
    }

    private void TryGetAndInterpretAction()
    {
        // Get action from agent
        string actionString = adapter.getAction();
        if (string.IsNullOrEmpty(actionString))
        {
            return;
        }
        dynamic action = JsonConvert.DeserializeObject(actionString);
        MapPresenter.Instance.InterpretAction(playerId, action);
    }

    private async void FixedUpdate()
    {
        TryGetAndInterpretAction();
        // To preserve ai can see the effect of **its** action, we send observation after the action is interpreted

        // Send observation to agent
        await adapter.SendObservationAsync(EncodeRoutineObservation());
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
