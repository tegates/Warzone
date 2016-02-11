using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoapPath
{
	
	public GoapPathNode Start;
	public GoapPathNode Goal;


	private HashSet<GoapPathNode> _allNodes;

	public void AddNode(GoapPathNode parent, GoapPathEdge edge, GoapPathNode child)
	{
		_allNodes.Add(child);
		parent.Child = edge;
		edge.Parent = parent;
		edge.Child = child;
		child.Parent = edge;
	}
}