using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Newtonsoft.Json;

class Observation {
    public MapPresenter map;
    public bool isGameOver;
    public int frameCount;
}

public class Vector2Converter : JsonConverter<Vector2>
{
    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, new { x = value.x, y = value.y });
    }

    public override Vector2 ReadJson(JsonReader reader, System.Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new System.NotImplementedException();
    }
}

public class Vector2IntConverter : JsonConverter<Vector2Int>
{
    public override void WriteJson(JsonWriter writer, Vector2Int value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, new { x = value.x, y = value.y });
    }

    public override Vector2Int ReadJson(JsonReader reader, System.Type objectType, Vector2Int existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new System.NotImplementedException();
    }
}

public abstract class Agent
{
    abstract public string getAction();
    abstract public void sendObservation(string observation);
}

public class PlayerAgent : Agent
{
    private Process p = null;
    private string buffer = "";
    public PlayerAgent(string fileName, string arguments)
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
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }

    void DataReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        buffer += eventArgs.Data + '\n';
    }

    public override string getAction()
    {
        string action = buffer;
        buffer = "";
        return action;
    }

    public override void sendObservation(string observation)
    {
        p.StandardInput.WriteLine(observation);
        p.StandardInput.Flush();
    }
}

public class RecordAgent : Agent
{
    private StreamReader recordReader = null;
    public RecordAgent(string recordFileName)
    {
        recordReader = new StreamReader(recordFileName);
    }

    public override string getAction()
    {
        return recordReader.ReadLine();
    }

    public override void sendObservation(string observation)
    {
        // do nothing
    }
}

public enum AgentType
{
    Player,
    Record,
    Human,
}


public class AiPlayer : MonoBehaviour
{
    public int reactionActionCount = 10; // 0.2s = 10 frames
    public AgentType agentType = AgentType.Player;
    PlayerPresenter player; // can read the player's state and call the player's action
    MapPresenter map;
    Agent agent = null;
    private int frameCount = 0;
    Queue<dynamic> actionQueue = new Queue<dynamic>();

    void Awake()
    {
    }

    // Assign the player that this AI should control
    public void Init(PlayerPresenter player, MapPresenter map)
    {
        this.player = player;
        this.map = map;
    }

    void Start()
    {
        // setup AI so that it can be called
        if (agentType == AgentType.Record)
        {
            agent = new RecordAgent(Application.dataPath + "/Records/record.txt");
        }
        else if (agentType == AgentType.Player)
        {
            this.agent = new PlayerAgent(
                "python",
                Application.dataPath + "/Player-SDK/Python/main.py"
            );
        }

    }

    // TODO: to be implemented
    void FixedUpdate()
    {
        frameCount++;

        // Consume action from agent
        string actionString = agent.getAction();
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
                Debug.Log("Cannot parse action: ");
                Debug.Log(action);
            }
        }

        // Send observation to agent
        string observation = EncodeObservation();
        agent.sendObservation(observation);

        // Invoke action
        if (actionQueue.Count > 0 && actionQueue.Peek().frameCount <= frameCount - reactionActionCount)
            InvokeAction(actionQueue.Dequeue());
        else if (actionQueue.Count > 0)
        {
            Debug.LogWarning("Action missing: " + (frameCount - reactionActionCount));
        }
    }

    string EncodeObservation()
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Error = delegate(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
            {
                // errors.Add(args.ErrorContext.Error.Message);
                args.ErrorContext.Handled = true;
            }
        };
        settings.Converters.Add(new Vector2Converter());
        settings.Converters.Add(new Vector2IntConverter());
        string observation_str = JsonConvert.SerializeObject(new Observation {
            map = map,
            isGameOver = false, // TODO
            frameCount = frameCount,
        }, Formatting.None, settings);
        // Write to tmp.txt
        // File.WriteAllText(Application.dataPath + "/tmp.txt", observation_str);
        return observation_str;
    }

    void InvokeAction(dynamic action)
    {
        Debug.Log("action.type: " + action.type);
        Debug.Log("action.direction: " + action.direction);
        if (action.type == "Move")
        {
            player.TryMove((ForwardOrBackward)action.direction);
        }
        else if (action.type == "Rotate")
        {
            player.TryRotate((LeftOrRight)action.direction);
        }
        else if (action.type == "Shoot")
        {
            player.TryShoot();
        }
        else if (action.type == "ChangeBullet")
        {
            player.TryChangeBullet();
        }
        else if (action.type == "PlaceBomb")
        {
            player.TryPlaceBomb(new Vector2Int((int)action.targetX, (int)action.targetY));
        }
        else if (action.type == "AddLine")
        {
            player.TryAddLine(new Vector2Int((int)action.targetX, (int)action.targetY), (LineInPortalPattern)action.line);
        }
        else if (action.type == "RemoveLine")
        {
            player.TryRemoveLine(new Vector2Int((int)action.targetX, (int)action.targetY), (LineInPortalPattern)action.line);
        }
        else if (action.type == "UsePortal")
        {
            // player.TryUsePortal();
        }
        else if (action.type == "Idle")
        {
            // do nothing
        }
        else
        {
            Debug.Log("Unknown action type: " + action.type);
        }
    }
}