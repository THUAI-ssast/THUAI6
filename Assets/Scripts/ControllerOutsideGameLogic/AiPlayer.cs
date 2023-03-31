using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

[System.Serializable]
public class Action
{
    public int frameCount;
    public string type;
    public int? targetX;
    public int? targetY;
    public int? direction;
    public int? line;
}

[System.Serializable]
public class PlayerObservation
{
    public int id;
    public int hp;
    public int ammo;
    public int x;
    public int y;
    public float directionAngle;
    public float respawnTimeLeft;
}

[System.Serializable]
public class BombObservation
{
    public int id;
    public int x;
    public int y;
    public int owner;
    public float timeLeft;
}

[System.Serializable]
public class PortalObservation
{
    public int id;
    public int x;
    public int y;
    public int pattern;
    public bool isBeingUsed;
}

[System.Serializable]
public class Observation
{
    public int frameCount;
    public PlayerObservation[] players;
    public BombObservation[] bombs;
    public PortalObservation[] portals;
    public bool isGameOver = false;
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
    private StreamWriter recordWriter = null;
    public PlayerAgent(string fileName, string arguments, string recordFileName)
    {
        try
        {
            // Create record file
            recordWriter = new StreamWriter(recordFileName);

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

        if (recordWriter != null)
        {
            recordWriter.Write(action);
            recordWriter.Flush();
        }
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
    Agent agent = null;
    private int frameCount = 0;
    Queue<Action> actionQueue = new Queue<Action>();

    void Awake()
    {
    }

    // Assign the player that this AI should control
    public void Init(PlayerPresenter player)
    {
        this.player = player;
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
                Application.dataPath + "/Player-SDK/Python/main.py",
                Application.dataPath + "/Records/record.txt"
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

            Action actionObj = JsonUtility.FromJson<Action>(action);
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
        // TODO: bombs, portals, players
        return JsonUtility.ToJson(new Observation
        {
            frameCount = frameCount,
            players = new PlayerObservation[]
            {
                new PlayerObservation
                {
                    id = 0,
                    hp = 100,
                    ammo = 30,
                    x = 42,
                    y = 17,
                    directionAngle = 1.57f,
                    respawnTimeLeft = 0,
                }
            },
            bombs = new BombObservation[0],
            portals = new PortalObservation[0],
        });
    }

    void InvokeAction(Action action)
    {
        Debug.Log("action.type: " + action.type);
        // if (action.type == "Move")
        // {
        //     player.TryMove((ForwardOrBackward)action.direction);
        // }
        // else if (action.type == "Rotate")
        // {
        //     player.TryRotate((LeftOrRight)action.direction);
        // }
        // else if (action.type == "Shoot")
        // {
        //     player.TryShoot();
        // }
        // else if (action.type == "ChangeBullet")
        // {
        //     player.TryChangeBullet();
        // }
        // else if (action.type == "PlaceBomb")
        // {
        //     player.TryPlaceBomb(new Vector2Int((int)action.targetX, (int)action.targetY));
        // }
        // else if (action.type == "AddLine")
        // {
        //     player.TryAddLine(new Vector2Int((int)action.targetX, (int)action.targetY), (LineInPortalPattern)action.line);
        // }
        // else if (action.type == "RemoveLine")
        // {
        //     player.TryRemoveLine(new Vector2Int((int)action.targetX, (int)action.targetY), (LineInPortalPattern)action.line);
        // }
        // else if (action.type == "UsePortal")
        // {
        //     // player.TryUsePortal();
        // }
        // else if (action.type == "Idle")
        // {
        //     // do nothing
        // }
        // else
        // {
        //     Debug.Log("Unknown action type: " + action.type);
        // }
    }
}
