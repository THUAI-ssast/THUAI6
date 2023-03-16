using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: to be implemented

using PortalPattern = System.UInt32;

public class Portal : MonoBehaviour
{
    Coordinate coordinate;
    PortalPattern pattern;
    bool isBeingUsed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddLine(LineInPortalPattern line)
    {
        pattern |= (PortalPattern)line;
    }

    public void RemoveLine(LineInPortalPattern line)
    {
        pattern &= ~((PortalPattern)line);
    }

    // Teleport all entities in the portal to another portal
    // TODO: to be implemented
    public void Teleport(Coordinate coordinate)
    {
    }
}
