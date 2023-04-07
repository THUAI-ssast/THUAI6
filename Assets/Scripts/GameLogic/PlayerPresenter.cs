using System;
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
                _model.DiedEvent -= OnDied;
            }
            _model = value;
            if (_model != null)
            {
                _model.PositionChangedEvent += OnPositionChanged;
                _model.DiedEvent += OnDied;
            }
        }
    }
    private PlayerView _view;
    private Rigidbody2D _rb2D;

    public static event EventHandler<PlayerActionEventArgs> PlayerActionEvent;

    void Awake()
    {
        _view = GetComponent<PlayerView>();
        _rb2D = GetComponent<Rigidbody2D>();
    }

    public void SetModel(PlayerModel model)
    {
        this.model = model;

        transform.position = new Vector3(model.position.x, model.position.y, 0);
        transform.rotation = Quaternion.Euler(0, 0, model.rotation);

        _view.SetColor(model.team);
    }

    void FixedUpdate()
    {
        // Preserve the model is consistent with the game object
        model.position = transform.position;
        // model.rotation is float, transform.rotation is Quaternion
        model.rotation = transform.rotation.eulerAngles.z;
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
        _rb2D.velocity += directionVector * PlayerModel.Acceleration * Time.fixedDeltaTime;
        if (_rb2D.velocity.magnitude > PlayerModel.MaxVelocity)
        {
            _rb2D.velocity = _rb2D.velocity.normalized * PlayerModel.MaxVelocity;
        }

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
        transform.Rotate(0, 0, angle);

        // Model is updated in FixedUpdate
    }

    public bool TryShoot()
    {
        return true;
    }

    public void Shoot()
    {
    }

    public bool TryChangeBullet()
    {
        return true;
    }

    public void ChangeBullet()
    {
    }

    public bool TryPlaceBomb(Vector2Int target)
    {
        return true;
    }

    public void PlaceBomb(Vector2Int target)
    {
        // Something before the bomb is placed. Mainly, update player state
        // TODO

        // Place the bomb
        MapPresenter.Instance.PlaceBomb(target);

        // Something after the bomb is placed
        // TODO
    }

    public bool TryAddLine(Vector2Int target, LineInPortalPattern line)
    {
        return true;
    }

    public void AddLine(Vector2Int target, LineInPortalPattern line)
    {
    }

    public bool TryRemoveLine(Vector2Int target, LineInPortalPattern line)
    {
        return true;
    }

    public void RemoveLine(Vector2Int target, LineInPortalPattern line)
    {
    }

    public bool TryActivatePortal(Vector2Int target, Vector2Int destination)
    {
        return true;
    }

    public void ActivatePortal(Vector2Int target, Vector2Int destination)
    {
    }

    private void Respawn()
    {
        // TODO: to be implemented
        // update the model
        // update game object
    }

    // from model

    private void OnPositionChanged(object sender, Vector2 position)
    {
        transform.position = new Vector3(position.x, position.y, 0);
    }

    private void OnDied(object sender, Team team)
    {
        // TODO
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
    public Vector2Int target;
    public LineInPortalPattern line;

    public AddLineAction(Vector2Int target, LineInPortalPattern line)
    {
        this.target = target;
        this.line = line;
    }
}

public class RemoveLineAction
{
    public string type = "RemoveLine";
    public Vector2Int target;
    public LineInPortalPattern line;

    public RemoveLineAction(Vector2Int target, LineInPortalPattern line)
    {
        this.target = target;
        this.line = line;
    }
}
