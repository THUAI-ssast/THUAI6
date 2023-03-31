using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// call AI to decide next action
// TODO: to be implemented

public class AiPlayer : MonoBehaviour
{
    // TODO: to be decided
    float reactionTime; // seconds. Not guaranteed to be a constant.
    PlayerPresenter player; // can read the player's state and call the player's action

    void Awake()
    {
        reactionTime = 0.2f;
    }

    // Assign the player that this AI should control
    public void Init(PlayerPresenter player)
    {
        this.player = player;
    }

    // TODO: to be implemented
    void Start()
    {
        // setup AI so that it can be called
    }

    // TODO: to be implemented
    void FixedUpdate()
    {
        // call AI with latest info to get next action
        // map, the player itself

        // maybe a time limit is needed

        // analyze the action. only one action is allowed

        // invoke the action, with delay of reactionTime if needed
        // call PlayerPresenter to invoke the action

    }   
}
