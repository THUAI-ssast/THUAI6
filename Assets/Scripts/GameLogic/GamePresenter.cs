using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePresenter : MonoSingleton<GamePresenter>
{
    [SerializeField]
    private GameModel _model;
    private GameView _view;

    public static event EventHandler GameStartEvent;
    public static event EventHandler GameEndEvent;

    public override void Init()
    {
        _model = GameModel.Instance;
        _view = GetComponent<GameView>();
    }

    void Start()
    {
        GameStartEvent?.Invoke(this, EventArgs.Empty);
    }

    void FixedUpdate()
    {
        // The fixedDeltaTime doesn't change with Time.timeScale. It's not real time, but game time.
        _model.UpdateTime(Time.fixedDeltaTime);
        if (_model.timeLeft <= 0.0f)
        {
            EndGame();
            return;
        }
    }

    private void EndGame()
    {
        // pause the game
        Time.timeScale = 0.0f;

        GameEndEvent?.Invoke(this, EventArgs.Empty);

        _view?.ShowResult(_model.teamScore);
    }
}
