using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapView))]
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

    private List<GameObject> _playerObjects;
    private List<PlayerPresenter> _playerPresenters; // to control players

    public override void Init()
    {
        model = MapModel.Instance;

        // Initialize map view
        _view = GetComponent<MapView>();
        _view?.InitMap(model.map);

        // Instantiate player game objects
        playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
        _playerObjects = new List<GameObject>();
        _playerPresenters = new List<PlayerPresenter>();
        foreach (PlayerModel playerModel in model.players)
        {
            GameObject playerObject = Instantiate(playerPrefab, transform);
            PlayerPresenter playerPresenter = playerObject.GetComponent<PlayerPresenter>();
            playerPresenter.SetModel(playerModel);

            _playerObjects.Add(playerObject);
            _playerPresenters.Add(playerPresenter);
        }

        // preload prefabs
        bombPrefab = Resources.Load<GameObject>("Prefabs/Bomb");
        portalPrefab = Resources.Load<GameObject>("Prefabs/Portal");
    }

    public void CustomInit(dynamic initMapData, List<object> initPlayersData)
    {
        model.CustomInit(initMapData, initPlayersData);
        _view?.InitMap(model.map);
    }

    // private void Start()
    // {
    //     Test();
    // }

    private void Test()
    {
        // add portal lines
        var directions = new Direction[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
        for (int i = 0; i < 15; i++)
        {
            AddLine(model.GetRandomPosition(), directions[Random.Range(0, 4)]);
        }
        // activate portals
        var portals = model.portalsClassifiedByPattern;
        var portalsToActivate = new List<PortalModel>();
        foreach (var pattern in portals.Keys)
        {
            List<PortalModel> portalsSamePattern = portals[pattern];
            if (portalsSamePattern.Count >= 2)
            {
                ActivatePortal(portalsSamePattern[0], portalsSamePattern[1]);
                portalsToActivate.Add(portalsSamePattern[0]);
            }
        }
        // bombs
        for (int i = 0; i < 3; i++)
        {
            PlaceBomb(portalsToActivate[i].position);
            PlaceBomb(model.GetRandomPosition());
        }
    }

    public GameObject GetPlayerObject(int playerId)
    {
        return _playerObjects[playerId];
    }

    public void InterpretAction(int playerId, dynamic action)
    {
        PlayerPresenter playerPresenter = _playerPresenters[playerId];
        try
        {
            // Note: the dynamic action is actually a JObject and its values are stored as JValue.
            // So we need to cast JValue to the correct type.
            string actionType = (string)action.type;
            switch (actionType)
            {
                case "Move":
                    playerPresenter.TryMove((ForwardOrBackward)action.direction);
                    break;
                case "Rotate":
                    playerPresenter.TryRotate((LeftOrRight)action.direction);
                    break;
                case "Shoot":
                    playerPresenter.TryShoot();
                    break;
                case "ChangeBullet":
                    playerPresenter.TryChangeBullet();
                    break;
                case "PlaceBomb":
                    playerPresenter.TryPlaceBomb(new Vector2Int((int)action.target.x, (int)action.target.y));
                    break;
                case "AddLine":
                    playerPresenter.TryAddLine((Direction)action.direction);
                    break;
                case "RemoveLine":
                    playerPresenter.TryRemoveLine((Direction)action.direction);
                    break;
                case "ActivatePortal":
                    playerPresenter.TryActivatePortal(new Vector2Int((int)action.destination.x, (int)action.destination.y));
                    break;
                case "Idle":
                    break;
            }
        }
        catch (System.Exception)
        {
            Debug.LogError($"Player {playerId} tried to perform an invalid action: {action}");
        }
    }

    public void PlaceBomb(Vector2Int target)
    {
        // Instantiate a bomb
        GameObject bombObject = Instantiate(bombPrefab, transform);
        BombPresenter bombPresenter = bombObject.GetComponent<BombPresenter>();
        BombModel bombModel = new BombModel(target);
        bombPresenter.SetModelAndActivate(bombModel);

        // Update map model
        model.PlaceBomb(bombModel);
        bombPresenter.ExplodeEvent += (sender, args) => model.RemoveBomb(bombModel);
    }

    public void SetBombPosition(BombModel bomb, Vector2Int target)
    {
        model.SetBombPosition(bomb, target);
        bomb.SetPosition(target);
    }

    /// <summary>
    /// Apply line addition to all related portals
    /// </summary>
    public void AddLine(Vector2Int target, Direction direction)
    {
        model.AddLine(target, direction);
    }

    /// <summary>
    /// Apply line removal to all related portals
    /// </summary>
    public void RemoveLine(Vector2Int target, Direction direction)
    {
        model.RemoveLine(target, direction);
    }

    public void ActivatePortal(PortalModel portal, PortalModel destination)
    {
        // Instantiate a portal
        GameObject portalObject = Instantiate(portalPrefab, transform);
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
