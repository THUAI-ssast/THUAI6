using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PortalPattern = System.UInt32;

public class Map : MonoBehaviour
{
    BlockState[,] map;
    Player[] players;
    Dictionary<PortalPattern, Portal[]> portalsClassifiedByPattern;
    // Bombs, for now they needn't to be stored here

    // TODO: to be implemented

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
