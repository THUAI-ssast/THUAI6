using System;
using UnityEngine;

public class GamePresenter : MonoSingleton<GamePresenter>
{
    [SerializeField]
    private GameModel _model;

    public static event EventHandler GameStartEvent;
    public static event EventHandler GameEndEvent;

    public override void Init()
    {
        _model = GameModel.Instance;
    }

    // Doesn't use Start() to avoid bug from life cycle
    public void GameStart()
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

        if (ProgramManager.Instance.isBatchMode)
        {
            Application.Quit();
        }
    }
}
