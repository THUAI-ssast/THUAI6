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
        int processSeed = replayData.init.processSeed;
        int[,] initMapData = replayData.init.map.ToObject<int[,]>();
        List<object> initPlayersData = replayData.init.players.ToObject<List<object>>();
        mapPresenter.CustomInit(processSeed, initMapData, initPlayersData);
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