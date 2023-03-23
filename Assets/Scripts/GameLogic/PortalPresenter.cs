using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PortalView))]
public class PortalPresenter : MonoBehaviour
{
    [SerializeField]
    private PortalModel _model;
    public PortalModel model
    {
        get { return _model; }
        set { _model = value; }
    }
    private PortalView _view;
    public PortalView view
    {
        get { return _view; }
        set
        {
            _view = value;
        }
    }

    private void Awake()
    {
        view = GetComponent<PortalView>();
    }

    public void SetModelAndActivate(PortalModel model, Vector2Int destination)
    {
        this.model = model;

        transform.position = new Vector3(model.position.x, model.position.y, 0);
        
        Activate(destination);
    }

    public void Activate(Vector2Int destination)
    {
        model.Activate();

        view?.OnPortalActivated();
        
        DelayedFunctionCaller.CallAfter(PortalModel.WaitTime, () => Teleport(destination));
    }

    private void Teleport(Vector2Int destination)
    {
        model.Teleport();

        // TODO: to be implemented
        var players = MapModel.Instance.players;
        var bombs = MapModel.Instance.bombs;
        // Update the position of all entities in the portal.
        // eg. player.position = destination);

        // Call the view here to do the teleportation animation if needed.
        
        Destroy(gameObject);
    }
}
