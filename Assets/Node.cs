using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool traversable;
    public Vector3 worldPos;

	public int gridX; // x index in grid.                                                              
	public int gridY; // y index in grid.                                                              

	public int gCost; // distance to parent node.                                                      
	public int hCost; // distance to target node.                                                                                                                                                         
	public int fCost // calculate f-cost.   
	{                                                           
		get
		{
			return gCost + hCost;
		}
	}

	public Node parent;

	public Node(bool _traversable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        traversable = _traversable;
        worldPos = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}
}
