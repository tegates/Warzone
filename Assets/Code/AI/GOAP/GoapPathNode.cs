using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoapPathNode
{
	public List<GoapWorldState> WorldStates;
	public GoapPathEdge Child;
	public GoapPathEdge Parent;

	public GoapPathNode(List<GoapWorldState> states)
	{
		WorldStates = states;
	}
}

public class GoapPathEdge
{
	public GoapAction Action;
	public GoapPathNode Child;
	public GoapPathNode Parent;

	public GoapPathEdge(GoapAction action)
	{
		Action = action;
	}
}