using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
	public Transform seeker, target;

	Grid grid;

	void Awake()
	{
		grid = GetComponent<Grid>();
	}

	void Update()
	{
		FindPath(seeker.position, target.position); // start pathfinding.
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
				RetracePath(startNode, targetNode); // retrace path.
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

	// calculate movement cost to target node by completing all movement in one direction diagonally, and leaving only movement in one direction. 
	// direct move cost = 10.
	// diagonal move cost = 14.
	// formula: 14 * diagonal moves + 10 * remainging horizontal/vertical moves.
	int GetDistance(Node nodeA, Node nodeB)
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX); // horizontal distance to target node.
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY); // vertical distance to target node.

		if (dstX > dstY)
		{
			return 14 * dstY + 10 * (dstX - dstY); // horizontal movement done diagonally.
		}

		return 14 * dstX + 10 * (dstY - dstX); // vertical movement done diagonally.
	}

	void RetracePath(Node startNode, Node targetNode) // work back through nodes to construct path.
	{
		List<Node> path = new List<Node>();
		Node currentNode = targetNode;

		while (currentNode != startNode) // if path is incomplete.
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}

		path.Reverse();
		grid.path = path; // pass path list to Gizmos function in Grid.cs.
	}
}
