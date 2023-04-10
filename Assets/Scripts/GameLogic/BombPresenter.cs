using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BombView))]
public class BombPresenter : MonoBehaviour
{
    [SerializeField]
    private BombModel _model;
    public BombModel model
    {
        get { return _model; }
        set
        {
            if (_model != null)
            {
                _model.PositionChangedEvent -= OnPositionChanged;
            }
            _model = value;
            if (_model != null)
            {
                _model.PositionChangedEvent += OnPositionChanged;
            }
        }
    }
    private BombView _view;
    public BombView view
    {
        get { return _view; }
        set
        {
            _view = value;
        }
    }

    public event EventHandler ExplodeEvent;

    private void Awake()
    {
        view = GetComponent<BombView>();
    }

    public void SetModelAndActivate(BombModel model)
    {
        this.model = model;
        OnPositionChanged(null, model.position);

        Activate();
    }

    public void Activate()
    {
        DelayedFunctionCaller.CallAfter(BombModel.ExplosionTime, Explode);
    }

    private void Explode()
    {
        ExplodeEvent?.Invoke(this, EventArgs.Empty);

        // hurt all players in the explosion radius
        List<PlayerModel> players = MapModel.Instance.players;
        foreach (PlayerModel player in players)
        {
            if (Vector2.Distance(player.position, model.position + new Vector2(0.5f, 0.5f)) <= BombModel.ExplosionRadius)
            {
                player.Hurt(BombModel.ExplosionDamage);
            }
        }

        view?.OnExplode();
        Destroy(gameObject, BombView.ExplosionDuration);
    }

    // from model

    private void OnPositionChanged(object sender, Vector2Int position)
    {
        transform.position = new Vector3(position.x + 0.65f, position.y + 0.5f, 0);
    }
}
