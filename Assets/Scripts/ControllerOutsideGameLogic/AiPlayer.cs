using System;
using System.Collections;
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

    public void SendObservation(string observation)
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
    public void Init(int playerId, dynamic config)
    {
        this.playerId = playerId;

        adapter = new ExternalAiAdapter((string)config.command);

        // Send start observation
        adapter.SendObservation(EncodeStartObservation());
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

    IEnumerator StopCoroutineInSeconds(Coroutine myCoroutine, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StopCoroutine(myCoroutine);
    }

    private IEnumerator SendObservationCoroutineCore(string observation)
    {
        adapter.SendObservation(observation);
        yield return null;
    }

    private IEnumerator SendObservationCoroutine()
    {
        yield return new WaitForFixedUpdate();
        // Here we get the latest observation
        string observation = EncodeRoutineObservation();
        Coroutine sendObservationCoroutine = StartCoroutine(SendObservationCoroutineCore(observation));
        // set timeout for sending observation
        // In fact, even if a delay of 0.1s will make the game stop again and again. 0.01s will make it slow.
        // But that doesn't matter when headless as the game will end anyway.
        // TODO: it's good to make `timeout` a config. But it's not necessary for now and I'm lazy.
        float timeout = 0.5f;
        StartCoroutine(StopCoroutineInSeconds(sendObservationCoroutine, timeout));
    }

    private void FixedUpdate()
    {
        TryGetAndInterpretAction();
        // To preserve ai can see the effect of **its** action, we send observation after the action is interpreted

        // Send observation to agent
        StartCoroutine(SendObservationCoroutine());
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
