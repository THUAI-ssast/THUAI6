using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

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

    void Start()
    {
        GamePresenter.GameStartEvent += OnGameStart;
        // PlayerPresenter.PlayerActionEvent += OnPlayerAction;
        AiPlayer.AiPlayerRecordsEvent += OnAiPlayerRecords;
        GamePresenter.GameEndEvent += OnGameEnd;
    }

    void OnDisable()
    {
        GamePresenter.GameStartEvent -= OnGameStart;
        // PlayerPresenter.PlayerActionEvent -= OnPlayerAction;
        AiPlayer.AiPlayerRecordsEvent -= OnAiPlayerRecords;
        GamePresenter.GameStartEvent -= OnGameEnd;
    }

    void OnAiPlayerRecords(object sender, AiPlayerRecordsEventArgs args)
    {
        actions.Add(args);
    }

    void OnGameStart(object sender, EventArgs args)
    {
        var map = MapModel.Instance.map;
        var players = MapModel.Instance.players;
        init = new
        {
            map = map,
            players = players
        };
    }

    void OnPlayerAction(object sender, PlayerActionEventArgs args)
    {
        PlayerModel player = args.player;
        dynamic action = args.action;

        // if the last process item is not the current time, create a new process item
        float timeLeft = _game.timeLeft;
        if (process.Count == 0 || process[process.Count - 1].timeLeft != timeLeft)
        {
            CreateNewProcessItem(timeLeft);
        }

        var actions = process[process.Count - 1].actions;
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

    private void CreateNewProcessItem(float timeLeft)
    {
        process.Add(new
        {
            timeLeft = timeLeft,
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

        string jsonString = JsonConvert.SerializeObject(this, Formatting.None, settings);

        // save the data to a json file
        string path = Application.dataPath + "/../record.json";
        System.IO.File.WriteAllText(path, jsonString);
    }
}
