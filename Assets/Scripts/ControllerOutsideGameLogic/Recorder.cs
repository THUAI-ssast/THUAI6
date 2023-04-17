using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

// Note: Unity hasn't supported System.Text.Json yet, and its JsonUtility is not powerful enough.
// So we use Newtonsoft.Json instead.

public class Recorder : MonoSingleton<Recorder>
{
    public dynamic init { get; private set; }
    public List<dynamic> process { get; private set; }
    public List<dynamic> actions { get; private set; }
    public int[] result { get; private set; }

    private GameModel _game;

    public override void Init()
    {
        process = new List<dynamic>();
        _game = GameModel.Instance;
        actions = new List<dynamic>();
    }

    void OnEnable()
    {
        GamePresenter.GameStartEvent += OnGameStart;
        PlayerPresenter.PlayerActionEvent += OnPlayerAction;
        GamePresenter.GameEndEvent += OnGameEnd;
    }

    void OnDisable()
    {
        GamePresenter.GameStartEvent -= OnGameStart;
        PlayerPresenter.PlayerActionEvent -= OnPlayerAction;
        GamePresenter.GameStartEvent -= OnGameEnd;
    }

    void OnGameStart(object sender, EventArgs args)
    {
        Cell[,] map = MapModel.Instance.map;
        List<PlayerModel> players = MapModel.Instance.players;

        int[,] mapRecord = new int[map.GetLength(0), map.GetLength(1)];
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                mapRecord[i, j] = map[i, j].isObstacle ? 1 : 0;
            }
        }
        List<dynamic> playersRecord = new List<dynamic>();
        foreach (PlayerModel player in players)
        {
            playersRecord.Add(new
            {
                id = player.id,
                position = new float[] { player.position.x, player.position.y },
                rotation = player.rotation
            });
        }
        float gameTime = GameModel.Instance.timeLeft;
        int processSeed = MapModel.Instance.GetProcessSeed();
        init = new
        {
            gameTime = gameTime,
            processSeed = processSeed,
            map = mapRecord,
            players = playersRecord
        };
    }

    void OnPlayerAction(object sender, PlayerActionEventArgs args)
    {
        PlayerModel player = args.player;
        dynamic action = args.action;

        int frame = _game.frame;
        if (process.Count == 0 || process[process.Count - 1].frame != frame)
        {
            CreateNewProcessItem(frame);
        }

        List<dynamic> actions = process[process.Count - 1].actions;
        actions.Add(new
        {
            playerId = player.id,
            action = action
        });
    }

    void OnGameEnd(object sender, EventArgs args)
    {
        RecordResult(_game.teamScore);
        ExportToJson();
    }

    private void CreateNewProcessItem(int frame)
    {
        process.Add(new
        {
            frame = frame,
            actions = new List<dynamic>()
        });
    }

    private void RecordResult(int[] result)
    {
        this.result = result;
    }

    private void ExportToJson()
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
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

        string jsonString = JsonConvert.SerializeObject(new
        {
            init = init,
            process = process,
            result = result
        }, Formatting.None, settings);

        // save the data to a json file
        string path = "record.json";
        System.IO.File.WriteAllText(path, jsonString);
    }
}
