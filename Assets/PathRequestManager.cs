using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>(); // queue of find path requests.
    PathRequest currentPathRequest; // path request currently being processed.

    static PathRequestManager instance;
	Pathfinding pathfinding;

	bool isProcessingPath;

	void Awake()
	{
		instance = this;
		pathfinding = GetComponent<Pathfinding>();
	}

	public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) // new path request.
	{
		PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback); // create new path request.
		instance.pathRequestQueue.Enqueue(newRequest); // add new request to queue.
		instance.TryProcessNext(); // process next path request in queue.
	}

	void TryProcessNext()
	{
		if (!isProcessingPath && pathRequestQueue.Count > 0) // if not currently processing a request and queue is not empty.
		{
			currentPathRequest = pathRequestQueue.Dequeue(); // current path request = next path request in queue.
			isProcessingPath = true;
			pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd); // start finding path.
		}
	}

	public void FinishedProcessingPath(Vector3[] path, bool success)
	{
		currentPathRequest.callback(path, success);
		isProcessingPath = false;
		TryProcessNext();
	}

	struct PathRequest // path request data structure.
	{
		public Vector3 pathStart; // start pos.
		public Vector3 pathEnd; // target pos.
		public Action<Vector3[], bool> callback; // result.

		public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
		{
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
		}
	}
}
