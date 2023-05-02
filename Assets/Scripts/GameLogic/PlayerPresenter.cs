using System;
using TMPro;
using UnityEngine;

public class PlayerActionEventArgs : EventArgs
{
    public PlayerModel player;
    public dynamic action;
}

[RequireComponent(typeof(PlayerView))]
public class PlayerPresenter : MonoBehaviour
{
    [SerializeField]
    private PlayerModel _model;
    public PlayerModel model
    {
        get { return _model; }
        set
        {
            if (_model != null)
            {
                _model.PositionChangedEvent -= OnPositionChanged;
                _model.RotationChangedEvent -= OnRotationChanged;
                _model.DiedEvent -= OnDied;
            }
            _model = value;
            if (_model != null)
            {
                _model.PositionChangedEvent += OnPositionChanged;
                _model.RotationChangedEvent += OnRotationChanged;
                _model.DiedEvent += OnDied;
            }
        }
    }

    public GameObject IDPrefab;
    public GameObject healthBarPrefab;
    private PlayerView _view;
    private Rigidbody2D _rb2D;
    private GameObject _canvas;

    private GameObject _ID;

    private GameObject _healthBar;

    public static event EventHandler<PlayerActionEventArgs> PlayerActionEvent;

    void Awake()
    {
        _view = GetComponent<PlayerView>();
        _rb2D = GetComponent<Rigidbody2D>();
        _canvas = GameObject.Find("CanvasInMap");
    }

    void Start()
    {
        _ID = Instantiate(IDPrefab, _canvas.transform);

        _ID.GetComponent<TextMeshProUGUI>().text = model.id.ToString();

        _healthBar = Instantiate(healthBarPrefab);

        UpdateHBAndIDPosition();
    }

    public void SetModel(PlayerModel model)
    {
        this.model = model;

        _rb2D.position = model.position;
        _rb2D.rotation = model.rotation;

        _view.SetColor(model.team);
    }

    void FixedUpdate()
    {
        // Preserve the model is consistent with the game object
        model.position = _rb2D.position;
        model.rotation = _rb2D.rotation;
        _rb2D.velocity = Vector2.zero;
        _rb2D.angularVelocity = 0;

        UpdateHB();
        UpdateHBAndIDPosition();
    }

    // 1. Check if the player can perform the action
    // 2. If yes, invoke the related events and perform the action
    // 3. Game object and model are both updated

    // return true if the action is performed successfully, false otherwise
    public bool TryMove(ForwardOrBackward direction)
    {
        if (!model.state.canMove) return false;
        Move(direction);
        return true;
    }

    public void Move(ForwardOrBackward direction)
    {
        PlayerActionEvent?.Invoke(this, new PlayerActionEventArgs { player = model, action = new MoveAction(direction) });

        // Make the game object move

        Vector2 directionVector = Quaternion.Euler(0, 0, model.rotation) * Vector2.up * (direction == ForwardOrBackward.Forward ? 1 : -1);
        _rb2D.MovePosition(_rb2D.position + directionVector * PlayerModel.MaxVelocity * Time.fixedDeltaTime);
        _rb2D.velocity = Vector2.zero;
        _rb2D.angularVelocity = 0;
        // Model is updated in FixedUpdate
    }

    public bool TryRotate(LeftOrRight direction)
    {
        if (!model.state.canRotate) return false;
        Rotate(direction);
        return true;
    }

    public void Rotate(LeftOrRight direction)
    {
        PlayerActionEvent?.Invoke(this, new PlayerActionEventArgs { player = model, action = new RotateAction(direction) });

        // Make the game object rotate
        float angle = PlayerModel.RotationSpeed * Time.fixedDeltaTime * (direction == LeftOrRight.Left ? 1 : -1);
        _rb2D.MoveRotation(_rb2D.rotation + angle);
        _rb2D.velocity = Vector2.zero;
        _rb2D.angularVelocity = 0;
        // Model is updated in FixedUpdate
    }

    public bool TryShoot()
    {
        bool toolsNotReady = model.ammo <= 0 || model.state.isShooting;
        if (!model.state.canShoot || toolsNotReady) return false;
        Shoot();
        return true;
    }

    public void Shoot()
    {
        PlayerActionEvent?.Invoke(this, new PlayerActionEventArgs { player = model, action = new ShootAction() });
        ShootBullet();
        model.Shoot();
    }

    public bool TryChangeBullet()
    {
        if (!model.state.canChangeBullet) return false;
        if (model.ammo == PlayerModel.MaxAmmo) return false;
        ChangeBullet();
        return true;
    }

    public void ChangeBullet()
    {
        PlayerActionEvent?.Invoke(this, new PlayerActionEventArgs { player = model, action = new ChangeBulletAction() });
        model.ChangeBullet();
    }

    public bool TryPlaceBomb(Vector2Int target)
    {
        // target is valid
        bool isTargetValid = MapModel.Instance.IsRoad(target);
        if (!isTargetValid) return false;
        // target is in range
        float distance = Vector2.Distance(target, model.position);
        if (distance > PlayerModel.MaxBombDistance) return false;
        // player is ready to place bomb
        bool toolsNotReady = model.bombCount <= 0;
        if (!model.state.canPlaceBomb || toolsNotReady) return false;

        PlaceBomb(target);
        return true;
    }

    public void PlaceBomb(Vector2Int target)
    {
        PlayerActionEvent?.Invoke(this, new PlayerActionEventArgs { player = model, action = new PlaceBombAction(target) });
        model.PlaceBombBegin();
        DelayedFunctionCaller.CallAfter(PlayerModel.PlaceBombTime, () =>
        {
            MapPresenter.Instance.PlaceBomb(target);
            model.PlaceBombEnd();
        });
    }

    public bool TryAddLine(Direction direction)
    {
        // target is valid
        bool canModifyPortalLine = MapModel.Instance.CanModifyPortalLine(Vector2Int.FloorToInt(model.position), direction);
        if (!canModifyPortalLine) return false;
        // player is ready to add line
        if (!model.state.canModifyPortal) return false;
        AddLine(direction);
        return true;
    }

    public void AddLine(Direction direction)
    {
        PlayerActionEvent?.Invoke(this, new PlayerActionEventArgs { player = model, action = new AddLineAction(direction) });
        model.ModifyPortalBegin();

        Vector2Int target = Vector2Int.FloorToInt(model.position);
        DelayedFunctionCaller.CallAfter(PlayerModel.ModifyPortalTime, () =>
        {
            MapPresenter.Instance.AddLine(target, direction);
            model.ModifyPortalEnd();
        });
    }

    public bool TryRemoveLine(Direction direction)
    {
        // target is valid
        bool canModifyPortalLine = MapModel.Instance.CanModifyPortalLine(Vector2Int.FloorToInt(model.position), direction);
        if (!canModifyPortalLine) return false;
        // player is ready to remove line
        if (!model.state.canModifyPortal) return false;
        RemoveLine(direction);
        return true;
    }

    public void RemoveLine(Direction direction)
    {
        PlayerActionEvent?.Invoke(this, new PlayerActionEventArgs { player = model, action = new RemoveLineAction(direction) });
        model.ModifyPortalBegin();

        Vector2Int target = Vector2Int.FloorToInt(model.position);
        DelayedFunctionCaller.CallAfter(PlayerModel.ModifyPortalTime, () =>
        {
            MapPresenter.Instance.RemoveLine(target, direction);
            model.ModifyPortalEnd();
        });
    }

    public bool TryActivatePortal(Vector2Int destination)
    {
        // target is valid
        bool canActivatePortal = MapModel.Instance.CanActivatePortal(Vector2Int.FloorToInt(model.position), destination);
        if (!canActivatePortal) return false;
        // player is ready to activate portal
        if (!model.state.canActivatePortal) return false;
        ActivatePortal(destination);
        return true;
    }

    public void ActivatePortal(Vector2Int destination)
    {
        PlayerActionEvent?.Invoke(this, new PlayerActionEventArgs { player = model, action = new ActivatePortalAction(destination) });
        model.ActivatePortalBegin();

        Vector2Int origin = Vector2Int.FloorToInt(model.position);
        PortalModel originPortal = MapModel.Instance.map[origin.x, origin.y].portal;
        PortalModel destinationPortal = MapModel.Instance.map[destination.x, destination.y].portal;
        DelayedFunctionCaller.CallAfter(PlayerModel.ActivatePortalTime, () =>
        {
            MapPresenter.Instance.ActivatePortal(originPortal, destinationPortal);
            model.ActivatePortalEnd();
        });
    }

    private void ShootBullet()
    {
        Vector2 direction = Quaternion.Euler(0, 0, model.rotation) * Vector2.up;
        RaycastHit2D hit = Physics2D.Raycast(model.position, direction, PlayerModel.BulletRange);

        if (hit.collider == null)
        {
            _view.ShootBullet(_rb2D.position + direction * PlayerModel.BulletRange);
            return;
        }
        _view.ShootBullet(hit.point);

        // If a player is hit, the player is damaged
        if (hit.collider.gameObject.TryGetComponent<PlayerPresenter>(out PlayerPresenter playerPresenter))
        {
            playerPresenter.model.Hurt(PlayerModel.BulletDamage);
        }
    }

    private void Respawn(Vector2Int position)
    {
        if (model.state.isAlive)
        {
            return;
        }
        model.Respawn(position);
        gameObject.SetActive(true);
        _ID.SetActive(true);
        _healthBar.SetActive(true);
    }

    // from model

    private void OnPositionChanged(object sender, Vector2 position)
    {
        _rb2D.position = position;
        _rb2D.velocity = Vector2.zero;
        _rb2D.angularVelocity = 0;

        UpdateHBAndIDPosition();
    }

    private void OnRotationChanged(object sender, float rotation)
    {
        _rb2D.rotation = rotation;
        _rb2D.velocity = Vector2.zero;
        _rb2D.angularVelocity = 0;
    }

    private void OnDied(object sender, Team team)
    {
        gameObject.SetActive(false);
        _ID.SetActive(false);
        _healthBar.SetActive(false);

        // The enemy team gets a point
        GameModel.Instance.AddScore(team.GetOppositeTeam(), 1);

        // Respawn after a delay
        Vector2Int position = MapModel.Instance.GetRandomPosition();
        DelayedFunctionCaller.CallAfter(PlayerModel.RespawnTime, () => Respawn(position));
    }

    private void UpdateHBAndIDPosition()
    {
        _ID.transform.position = gameObject.transform.position + new Vector3(-0.8f, 0.2f, 0);
        _healthBar.transform.position = gameObject.transform.position + new Vector3(0, 0.5f, 0);
    }

    private void UpdateHB()
    {
        float newLength = 1f * model.hp / PlayerModel.MaxHp;
        if (newLength < 0) newLength = 0;
        _healthBar.transform.localScale = new Vector3(newLength, 0.05f, 1);
    }
}


public class MoveAction
{
    public string type = "Move";
    public ForwardOrBackward direction;

    public MoveAction(ForwardOrBackward direction)
    {
        this.direction = direction;
    }
}

public class RotateAction
{
    public string type = "Rotate";
    public LeftOrRight direction;

    public RotateAction(LeftOrRight direction)
    {
        this.direction = direction;
    }
}

public class ShootAction
{
    public string type = "Shoot";
}

public class ChangeBulletAction
{
    public string type = "ChangeBullet";
}

public class PlaceBombAction
{
    public string type = "PlaceBomb";
    public Vector2Int target;

    public PlaceBombAction(Vector2Int target)
    {
        this.target = target;
    }
}

public class AddLineAction
{
    public string type = "AddLine";
    public Direction direction;

    public AddLineAction(Direction direction)
    {
        this.direction = direction;
    }
}

public class RemoveLineAction
{
    public string type = "RemoveLine";
    public Direction direction;

    public RemoveLineAction(Direction direction)
    {
        this.direction = direction;
    }
}

public class ActivatePortalAction
{
    public string type = "ActivatePortal";
    public Vector2Int destination;

    public ActivatePortalAction(Vector2Int destination)
    {
        this.destination = destination;
    }
}
