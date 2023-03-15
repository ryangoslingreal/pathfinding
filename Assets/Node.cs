using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool traversable;
    public Vector3 worldPos;

    public Node(bool _traversable, Vector3 _worldPos)
    {
        traversable = _traversable;
        worldPos = _worldPos;
    }
}
