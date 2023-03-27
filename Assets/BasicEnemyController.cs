using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    public Transform target;
    float speed = 1f;
    Vector3[] path;
    int targetIndex;

	void Start()
	{
		PathRequestManager.RequestPath(transform.position, target.position, OnPathFound); // request path.
	}

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful) // if path has been found.
	{
		if (pathSuccessful)
		{
			path = newPath;

			//restart FollowPath coroutine.
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator FollowPath()
	{
		Vector3 currentWaypoint = path[0]; // set initial waypoint as first waypoint in path array.

		while (true)
		{
			if (transform.position == currentWaypoint) // waypoint reached.
			{
				targetIndex++; // next waypoint in array.
				
				if (targetIndex >= path.Length) // prevent index out of bounds error.
				{
					yield break;
				}

				currentWaypoint = path[targetIndex]; // set next waypoint.
			}

			transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed); // move to waypoint.
			yield return null;
		}
	}
}
