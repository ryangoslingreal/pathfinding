using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using System.Linq;

public class Pathfinding : MonoBehaviour
{
	PathRequestManager requestManager;
	Grid grid;

	void Awake()
	{
		requestManager = GetComponent<PathRequestManager>();
		grid = GetComponent<Grid>();
	}

	public void StartFindPath(Vector3 startPos, Vector3 targetPos)
	{
		StartCoroutine(FindPath(startPos, targetPos));
	}

	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) // find path from start node to target node.
	{
		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;

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
				pathSuccess = true;
				break;
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

		yield return null;

		if (pathSuccess) // if path found.
		{
			waypoints = RetracePath(startNode, targetNode); // retrace path.
		}

		requestManager.FinishedProcessingPath(waypoints, pathSuccess); // return waypoints array to request manager.
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

	Vector3[] RetracePath(Node startNode, Node targetNode) // work back through nodes to construct path.
	{
		List<Node> path = new List<Node>();
		Node currentNode = targetNode;

		while (currentNode != startNode) // if path is incomplete.
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}

		Vector3[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);
		grid.path = path;
		return waypoints;
	}

	Vector3[] SimplifyPath(List<Node> path)
	{
		List<Vector3> waypoints = new List<Vector3>();
		Vector2 directionOld = Vector2.zero;

		for (int i = 1; i < path.Count; i++) // for each node in list.
		{
			Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY); // direction of movement to get to next node.

			if (directionNew != directionOld) // if direction has changed.
			{
				waypoints.Add(path[i].worldPos); // add position vector of new waypoint to list.
			}

			directionOld = directionNew; // reset.
		}

		return waypoints.ToArray(); // return list as array.
	}
}
