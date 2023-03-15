using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
	Grid grid;

	void Awake()
	{
		grid = GetComponent<Grid>();
	}

	void FindPath(Vector3 startPos, Vector3 targetPos) // find path from start node to target node.
	{
		Node startNode = grid.NodeFromWorldPoint(startPos); // start node from start pos.
		Node targetNode = grid.NodeFromWorldPoint(targetPos); // target node from target pos.

		List<Node> openSet = new List<Node>(); // open nodes that are waiting to be evaluated in list.
		HashSet<Node> closedSet = new HashSet<Node>(); // closed nodes that have been evaluated in hash table.
		openSet.Add(startNode); // add start node to open list.

		while (openSet.Count > 0) // essentially if openSet isn't empty.
		{
			Node currentNode = openSet[0]; // set current node as start node.

			//find node with smallest f-cost, excluding the start node that is already selected as the current node.
			for (int i = 1; i > openSet.Count; i++)
			{
				// if next node's f-cost is lower than current node's, or if they are equal, is next node's h-cost lower than current node's.
				if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) 
				{
					currentNode = openSet[i]; // next node becomes current node.
				}
			}

			openSet.Remove(currentNode); // remove current node from openSet.
			closedSet.Add(currentNode); // add current node to closedSet.

			if (currentNode == targetNode) // target node has been found.
			{
				return; // will retrace path later.
			}

			foreach (Node neighbour in grid.GetNeighbours(currentNode)) // for each neighbour of parent node.
			{
				if (!neighbour.traversable || closedSet.Contains(neighbour)) // if neighbour is not traversable or is in closedSet.
				{
					continue; // skip to next neighbour.
				}

				int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour); // movement cost to neighbour.

				if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) // if movement cost to neighbour is less than neighbour's old g-cost, or if neighbour not in openSet.
				{
					// recalculate f-cost.
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);

					neighbour.parent = currentNode; // set neighbour's parent node.

					if (!openSet.Contains(neighbour)) // if openSet doesn't contain neighbour node.
					{
						openSet.Add(neighbour); // add neighbour node to openSet.
					}
				}
			}
		}
	}

	void RetracePath()
	{

	}

	int GetDistance(Node nodeA, Node nodeB) // calculate cost of getting from current node to neighbour node.
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
		{
			return 14 * dstY + 10 * (dstX - dstY);
		}

		return 14 * dstX + 10 * (dstY - dstX);
	}
}
