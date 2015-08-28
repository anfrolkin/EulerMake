/*
 * Created by SharpDevelop.
 * User: frolkinak
 * Date: 25.06.2014
 * Time: 14:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace eulerMake
{
	/// <summary>
	/// Description of NodePointProcess.
	/// </summary>
	public class NodePointProcess
	{
		bool isUsed;
		//bool isPriority;
		//bool isNumber;
		//bool isBlank;
		int curNumber;
		int priority;
		int nodeNumber;
		
		public NodePointProcess( NodePoint inNode, bool inUsed)//int inNumber, int inPrior
		{
			curNumber = inNode.number;
			priority = inNode.priority;
			nodeNumber = inNode.numberNode;
			isUsed = inUsed;
		}
		
		public NodePointProcess( int inNumber, int inPrior, int inNodeNumb, bool inUsed)//int inNumber, int inPrior
		{
			curNumber = inNumber;
			priority = inPrior;
			nodeNumber = inNodeNumb;
			isUsed = inUsed;
		}
		
		public virtual void ProcessPoint(NodePoint inPoint)
		{
			//if (isSetUnused)
			//	inPoint.isUsed = false;
			//if (isNumber)
			inPoint.number = curNumber;
			inPoint.isUsed = isUsed;
			inPoint.priority = priority;
			inPoint.numberNode = nodeNumber;
            //if (isUsed)
            //    inPoint.isReplace = false;
		}
		
		public void IncrementNumber()
		{
			curNumber++;
		}
	}
	
	public class ReplaceUnused : NodePointProcess
	{
		public ReplaceUnused(NodePoint inNode, bool inUsed) : base(inNode, inUsed)
		{
		}
		
		public override void ProcessPoint(NodePoint inPoint)
		{
			if (!inPoint.isUsed)
			{
				inPoint.isReplace = true;
				inPoint.name = Material.blankName;
				inPoint.number = -1;
				inPoint.priority = 0;
				inPoint.numberNode = 0;
			}
		}
	}
	
	public class BestPointParams
	{
		//private List<int> layers;
		public List<NodePointLayer> points;
	}
	
	public class BestPointSet
	{
		public BestPointSet(NodePointLayer inPoint, int inLayer, bool inOneLayer)
		{
			point = inPoint;
			layer = inLayer;
			isOneLayer = inOneLayer;
		}
		public BestPointSet(BestPointSet inBest)
		{
			point = new NodePointLayer(inBest.point, inBest.point.layer);
			layer = inBest.layer;
			isOneLayer = inBest.isOneLayer;
		}
		//private List<int> layers;
		public NodePointLayer point;
		public bool isOneLayer;
		public int layer;
	}
}
