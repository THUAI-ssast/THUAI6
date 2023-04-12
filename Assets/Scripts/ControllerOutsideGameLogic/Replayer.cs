using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class Replayer : MonoSingleton<Replayer>
{
    private dynamic replayData;
    private GameModel game;
    private MapPresenter mapPresenter;
    private int processIndex = 0;
    public void Init(string replayPath)
    {
        game = GameModel.Instance;
        mapPresenter = MapPresenter.Instance;

        if (!File.Exists(replayPath))
        {
            Debug.LogError("Replay file not found: " + replayPath);
            return;
        }

        string replayString = File.ReadAllText(replayPath);
        replayData = JsonConvert.DeserializeObject(replayString);
    }

    void Start()
    {
        dynamic initMapData = replayData.init.map;
        List<object> initPlayersData = replayData.init.players.ToObject<List<object>>();

        // Restore map obstacles
        Cell[,] map = mapPresenter.model.map;
        for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(1); j++)
                map[i, j].isObstacle = (initMapData[i][j] == 1);

        // Restore player states
        List<PlayerModel> players = mapPresenter.model.players;
        for (int i = 0; i < initPlayersData.Count; i++)
        {
            dynamic initPlayerData = initPlayersData[i];
            PlayerModel playerModel = players.Find(player => player.id == (int)initPlayerData.id);

            playerModel.SetPosition(new Vector2((float)initPlayerData.position[0], (float)initPlayerData.position[1]));
            playerModel.SetRotation((float)initPlayerData.rotation);
        }
    }

    void FixedUpdate()
    {
        dynamic processData = replayData.process;
        int frameNo = game.frame;

        while (processIndex < processData.Count && (int)processData[processIndex].frame < frameNo)
            processIndex++;

        if (processIndex < processData.Count && (int)processData[processIndex].frame == frameNo)
        {
            dynamic processFrameData = processData[processIndex];
            List<object> actionDataList = processFrameData.actions.ToObject<List<object>>();
            for (int i = 0; i < actionDataList.Count; i++)
            {
                dynamic actionData = actionDataList[i];
                int playerId = actionData.playerId;
                dynamic action = actionData.action;

                mapPresenter.InterpretAction(playerId, action);
                // Debug.Log("frameNo: " + frameNo + ", playerId: " + playerId + ", action: " + action.ToString());
            }
            processIndex++;
        }
    }
}