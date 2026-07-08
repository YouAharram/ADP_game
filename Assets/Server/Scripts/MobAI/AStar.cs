using System;
using System.Collections.Generic;
using System.Diagnostics;

public class PriorityQueue<TPriority, TValue> where TPriority : IComparable<TPriority>
{
	private List<KeyValuePair<TPriority, TValue>> heap = new List<KeyValuePair<TPriority, TValue>>();

	public int Count => heap.Count;
	public bool IsEmpty => heap.Count == 0;

	public void Enqueue(TPriority priority, TValue value)
	{
		heap.Add(new KeyValuePair<TPriority, TValue>(priority, value));
		int i = heap.Count - 1;
		while (i > 0)
		{
			int parent = (i - 1) / 2;
			if (heap[parent].Key.CompareTo(heap[i].Key) <= 0) break;
			(heap[parent], heap[i]) = (heap[i], heap[parent]);
			i = parent;
		}
	}

	public TValue Dequeue()
	{
		if (IsEmpty) throw new InvalidOperationException("La coda è vuota!");

		var root = heap[0];
		int last = heap.Count - 1;
		heap[0] = heap[last];
		heap.RemoveAt(last);
		last--;

		int i = 0;
		while (true)
		{
			int left = 2 * i + 1, right = 2 * i + 2, smallest = i;
			if (left <= last && heap[left].Key.CompareTo(heap[smallest].Key) < 0) smallest = left;
			if (right <= last && heap[right].Key.CompareTo(heap[smallest].Key) < 0) smallest = right;
			if (smallest == i) break;
			(heap[i], heap[smallest]) = (heap[smallest], heap[i]);
			i = smallest;
		}

		return root.Value;
	}
}

/// <summary>
/// Generic Implementation of the A* algorithm.
/// </summary>
public class AStar {

	#region ProfilingCollection
	// Profiling Info
	static public bool CollectProfiling = false;
	static public Dictionary<string,float> LastRunProfilingInfo = new Dictionary<string, float>();
	//---------------
	#endregion

	/// <summary>
	/// Finds the optimal path between start and destionation TNode.
	/// </summary>
	/// <returns>The path.</returns>
	/// <param name="start">Starting Node.</param>
	/// <param name="destination">Destination Node.</param>
	/// <param name="distance">Function to compute distance beween nodes.</param>
	/// <param name="estimate">Function to estimate the remaining cost for the goal.</param>
	/// <typeparam name="TNode">Any class implement IHasNeighbours.</typeparam>
	static public Path<TNode> FindPath<TNode>(
		IHasNeighbours<TNode> dataStructure,
		TNode start,
		TNode destination,
		Func<TNode,TNode,double> distance,
		Func<TNode, double> estimate)
	{
		// Profiling Information
		float expandedNodes = 0;
		float elapsedTime = 0;
		Stopwatch st = new Stopwatch();
		//----------------------
		var closed = new HashSet<TNode>();
		var queue = new PriorityQueue<double, Path<TNode>>();
		queue.Enqueue(0, new Path<TNode>(start));
		if (CollectProfiling) st.Start();
		while (!queue.IsEmpty)
		{
			var path = queue.Dequeue();
			if (closed.Contains(path.LastStep))
				continue;
			if (path.LastStep.Equals(destination)) {
				if (CollectProfiling) {
					st.Stop();
					LastRunProfilingInfo["Expanded Nodes"] = expandedNodes;
					LastRunProfilingInfo["Elapsed Time"] = st.ElapsedTicks;
				}
				return path;
			}
			closed.Add(path.LastStep);
			expandedNodes++;
			foreach (TNode n in dataStructure.Neighbours(path.LastStep))
			{
				double d = distance(path.LastStep, n);
				if (n.Equals(destination))
					d = 0;
				var newPath = path.AddStep(n, d);
				queue.Enqueue(newPath.TotalCost + estimate(n), newPath);
			}
		}
		return null;
	}

}

/// <summary>
/// Interface that rapresent data structures that has the ability to find node neighbours.
/// </summary>
public interface IHasNeighbours<T>
{
	/// <summary>
	/// Gets the neighbours of the instance.
	/// </summary>
	/// <value>The neighbours.</value>
	IEnumerable<T> Neighbours(T node);
}