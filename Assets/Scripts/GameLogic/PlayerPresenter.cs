using System;
using UnityEngine;

public class PlayerActionEventArgs : EventArgs
{
    public PlayerModel player;
    public dynamic action;
}

public class PlayerPresenter : MonoSingleton<PlayerPresenter>
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
    private Rigidbody _rigidbody;

    public static event EventHandler<PlayerActionEventArgs> PlayerActionEvent;

    public override void Init()
    {
        _view = GetComponent<PlayerView>();
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
        // Update time left
        if (!model.state.isAlive)
        {
            model.respawnTimeLeft -= Time.fixedDeltaTime;
            if (model.respawnTimeLeft <= 0.0f)
            {
                Respawn();
            }
        }
        // TODO: There are other time-related states to be updated
    }

    // TODO: methods to be implemented

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
        Vector3 directionVector = model.rotation * Vector3.up * (direction == ForwardOrBackward.Forward ? 1 : -1);
        const float MaxVelocity = PlayerModel.MaxVelocity;
        if (_rigidbody.velocity.magnitude > MaxVelocity)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * MaxVelocity;
        }
        _rigidbody.AddForce(directionVector * PlayerModel.Acceleration, ForceMode.Acceleration);

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

    private void OnDied(object sender, EventArgs e)
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
