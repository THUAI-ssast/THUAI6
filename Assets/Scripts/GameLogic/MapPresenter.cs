using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPresenter : MonoSingleton<MapPresenter>
{
    [SerializeField]
    private MapModel _model;
    public MapModel model
    {
        get { return _model; }
        set
        {
            if (_model != null)
            {
                _model.PortalLineModifiedEvent -= OnPortalLineModified;
            }
            _model = value;
            if (_model != null)
            {
                _model.PortalLineModifiedEvent += OnPortalLineModified;
            }
        }
    }
    private MapView _view;

    // Prefabs
    private GameObject playerPrefab;
    private GameObject bombPrefab;
    private GameObject portalPrefab;

    public override void Init()
    {
        model = new MapModel();

        // Initialize map view
        _view = GetComponent<MapView>();
        _view?.InitMap(model.map);

        // Instantiate player game objects
        playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
        foreach (PlayerModel playerModel in model.players)
        {
            GameObject playerObject = Instantiate(playerPrefab);
            PlayerPresenter playerPresenter = playerObject.GetComponent<PlayerPresenter>();
            playerPresenter.SetModel(playerModel);
        }

        // preload prefabs
        bombPrefab = Resources.Load<GameObject>("Prefabs/Bomb");
        portalPrefab = Resources.Load<GameObject>("Prefabs/Portal");
    }

    public void PlaceBomb(Vector2Int target)
    {
        // Instantiate a bomb
        GameObject bombObject = Instantiate(bombPrefab);
        BombPresenter bombPresenter = bombObject.GetComponent<BombPresenter>();
        BombModel bombModel = new BombModel(target);
        bombPresenter.SetModelAndActivate(bombModel);

        // Update map model
        model.PlaceBomb(bombModel);
        bombPresenter.ExplodeEvent += (sender, args) => model.RemoveBomb(bombModel);
    }

    public void SetBombPosition(BombModel bomb, Vector2Int target)
    {
        model.RemoveBomb(bomb);
        bomb.position = target;
        // TODO: update map model
        model.PlaceBomb(bomb);

        
    }

    /// <summary>
    /// Apply line addition to all related portals
    /// </summary>
    /// <param name="target"></param>
    /// <param name="line"></param>
    public void AddLine(Vector2Int target, LineInPortalPattern line)
    {
        model.AddLine(target, line);
    }

    public void RemoveLine(Vector2Int target, LineInPortalPattern line)
    {
        model.RemoveLine(target, line);
    }

    public void ActivatePortal(PortalModel portal, PortalModel destination)
    {
        // Instantiate a portal
        GameObject portalObject = Instantiate(portalPrefab);
        PortalPresenter portalPresenter = portalObject.GetComponent<PortalPresenter>();
        portalPresenter.SetModelAndActivate(portal, destination.position);

    }

    // from MapModel

    private void OnPortalLineModified(object sender, List<PortalModel> portals)
    {
        foreach (PortalModel portal in portals)
        {
            _view?.UpdatePortal(portal);
        }
    }

}