using UnityEngine;

using PortalPattern = System.UInt32;

public class PortalModel
{
    // the time between activation and teleportation. seconds
    public const float WaitTime = 4.0f;

    public Vector2Int position { get; private set; }
    public PortalPattern pattern { get; private set; } = 0;
    public bool isBeingUsed { get; private set; } = false;

    public PortalModel(Vector2Int position)
    {
        this.position = position;
    }

    public bool isInPortal(Vector2 position)
    {
        // a portal is 1x2
        return position.x >= this.position.x && position.x < this.position.x + 1
            && position.y >= this.position.y && position.y < this.position.y + 2;
    }

    public void AddLine(LineInPortalPattern line)
    {
        pattern |= (PortalPattern)line;
    }

    public void RemoveLine(LineInPortalPattern line)
    {
        pattern &= ~((PortalPattern)line);
    }

    public void Activate()
    {
        isBeingUsed = true;
    }

    public void Teleport()
    {
        isBeingUsed = false;
    }
}
