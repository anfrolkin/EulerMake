/*
 * Created by SharpDevelop.
 * User: frolkinak
 * Date: 14.05.2014
 * Time: 15:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace eulerMake
{
	public class Node
    {
        public Node(string inName)
        {
            name = inName;
            arcCollection = new List<ContactSimple>();
        }
        public string name;
        public List<ContactSimple> arcCollection;
        
        public void AddContact(ContactSimple inNewCont)
        {
        	if (arcCollection.FindIndex(element => element == inNewCont) < 0)
        		arcCollection.Add(inNewCont);
        }
        /*
        public Contact GetOppositeContact(int inPosition, int inXLocation)
        {
        	foreach (Contact cnt in arcCollection)
        		if (cnt.levelY == inPosition && cnt.position.x <= (inXLocation + 2)
        		    && cnt.position.x >= (inXLocation - 2))
        			return cnt;
        	return null;
        }*/
        
        public List<ContactSimple> GetContacts(int inY)
        {
        	List<ContactSimple> returnConts = new List<ContactSimple>();
        	foreach (ContactSimple cnt in arcCollection)
        	{
        		if (inY > Params.lineMiddle && cnt.y > Params.lineMiddle)
        			returnConts.Add(cnt);
        		else if (inY < Params.lineMiddle && cnt.y < Params.lineMiddle)
        			returnConts.Add(cnt);
        	}
        	returnConts.Sort(PairInt.CompareByX);
        	return returnConts;
        }
    }
	
	public class NodePoint
	{
		public string name;
		public int priority;
		public int number;
		public bool isReplace;
		public bool isSource;
		public bool isUsed;
		public bool isFixed;
		
		public int numberNode;
		//public int materialPoint;
		
		public NodePoint(string inName)
		{
			name = inName;
			priority = 0;
			number = -1;
			numberNode = 0;
			isReplace = false;
			isSource = false;
			isUsed = false;
			isFixed = false;
		}
		/*
		public NodePoint(string inName, int inPriority)
		{
			name = inName;
			priority = inPriority;
			number = -1;
			numberNode = 0;
			isReplace = false;
			isSource = false;
		}*/
		
		public NodePoint(string inName, int inPriority, int inNumber)
		{
			name = inName;
			priority = inPriority;
			number = 0;
			numberNode = inNumber;
			isReplace = false;
			isSource = false;
			isUsed = false;
			isFixed = false;
		}
		
		public NodePoint(NodePoint inPoint)
		{
			name = inPoint.name;
			priority = inPoint.priority;
			number = inPoint.number;
			isReplace = inPoint.isReplace;
			isSource = inPoint.isSource;
			numberNode = inPoint.numberNode;
			isUsed = false;
			isFixed = false;
		}
		
		
		public void SetNextCont(NodePoint prevPoint)//(string inName, int inPrior, int )
		{
			name = prevPoint.name;
			priority = prevPoint.priority;
			number = prevPoint.number + 1;
			numberNode = prevPoint.numberNode;
			isReplace = false;
		}
		
		public void SetNextCont(string inName, int inPrior, int inNumber, int inNdNumber)
		{
			name = inName;
			priority = inPrior;
			number = inNumber;
			numberNode = inNdNumber;
			isReplace = false;
		}
		
		public bool IsEmpty(string inName, int inPriority)
		{
			if ( name == Material.blankName || name == inName || 
			    (priority < inPriority && isReplace) )
				return true;
			return false;
		}
		
		public int IsReach(string inName, List<int> inNumbers)
		{
			if ( name == inName )
				return inNumbers.FindIndex(element => element == number);
			return -1;
		}

        public static bool operator ==(NodePoint in1, NodePoint in2)
        {
            // If one is null, but not both, return false.
            if (((object)in1 == null) && ((object)in2 == null))
            {
                return false;
            }
            // If one is null, but not both, return false.
            if (((object)in1 == null) || ((object)in2 == null))
            {
                return false;
            }
            if ((in1.name == in2.name) && (in1.numberNode == in2.numberNode))
                return true;
            return false;
        }
        public static bool operator !=(NodePoint a, NodePoint b)
        {
            return !(a == b);
        }
		
	}

    public class NodePointLayer : NodePoint
    {
        public NodePointLayer(NodePoint inPnt, int inLayer) : base(inPnt)
        {
            layer = inLayer;
        }
        public NodePointLayer(NodePoint inPnt, int inLayer, string inName) : base(inPnt)
        {
            name = inName;
            layer = inLayer;
        }

        public int layer;
    }
	
	public class NodePaths
	{
		public NodePaths(string inName, ContactSimple inContact, int inNumberNode)
		{
			name = inName;
			startPoint = inContact;//new Contact(inContact)
			//currentEnds = new List<Contact>();
			//currentEnds.Add(inContact.GetHigherPoint(0));
			//currentEnds.Add(inContact.GetLowerPoint(0));
			numberNode = inNumberNode;
			//seekNumbers = new List<int>();
		}
		
		public void AddEndNumber(int inNumber)
		{
			//if (seekNumbers.FindIndex(element => element == inNumber) < 0)
			//	seekNumbers.Add(inNumber);
		}
		
		/*public bool IsReach(string inName, int inPriority)
		{
			foreach (Contact endUnit in currentEnds)
			if ( name == blank || (priority < inPriority) )
				return true;
			return false;
		}*/
		
		public string name;
		public ContactSimple startPoint;
		//public List<Contact> ConnectedLines;
		public int numberNode;
		//public List<int> seekNumbers;//-------------
		public List<int> connectedNumbers;
		//public List<Contact> currentEnds;//-----------
	}
	
	public class NodeDistance
	{
		public NodeDistance(int name1, int name2, int size, bool inCon, bool inFixed)
		{
			names = new PairInt(name1, name2);
			distance = size;
			connected = inCon;
            isFixed = inFixed;
		}
		public PairInt names;
		public int distance;
		public bool connected;
        public bool isFixed;
		
		// Compares by distance
        public static int CompareByDist(NodeDistance in1, NodeDistance in2)
		{
        	if (!in1.connected && in2.connected)
    			return -1;
        	else if (!in1.connected && !in2.connected && in1.distance < in2.distance)
    			return -1;
        	else if (!in1.connected && !in2.connected && in1.distance == in2.distance)
    			return 0;
        	else if (in1.connected && in2.connected)
    			return 0;
			return 1;
		}
	}
	
	/*public class NodePinInfo
	{
		public int numberNode;
		public List<int> connectedPin;
		public int startWave;
		
		public NodePinInfo()
		{
			connectedPin = new List<int>();
		}
	}*/
	
	public class NodeDistanceBase
	{
		private List<NodeDistance> distances;
		public string name;
        public List<int> countNode;
		
		//public Dictionary<int, int> nodePin;
        public static List<string> exceptionNames;
        public static string bestName = "";
		
		public NodeDistanceBase(int size, string inName)
		{
			distances = new List<NodeDistance>();
			name = inName;

            countNode = new List<int>();
			for (int i = 0; i < size; i++)
				countNode.Add(0);
            

            if (exceptionNames == null)
                exceptionNames = new List<string>();
		}
		public NodeDistanceBase(NodeDistanceBase inNodeDist)
		{
			distances = new List<NodeDistance>(inNodeDist.distances);
			name = inNodeDist.name;
			countNode = new List<int>(inNodeDist.countNode);
		}
		
		public void AddDistance(int name1, int name2, int size, bool inCon, bool inFixed)
		{
			if (distances.FindIndex( element => (element.names.x == name1 && element.names.y == name2) ||
	        	                    (element.names.x == name2 && element.names.y == name1) ) < 0 )
				distances.Add(new NodeDistance(name1, name2, size, inCon, inFixed));//!!!
		}
		
        public List<int> GetNodeNumbers(int size)
        {
            List<int> numbers = new List<int>();
            List<int> passedNumbers = new List<int>();

            for (int i = 0; i < size; i++)
            {
                if (passedNumbers.FindIndex(el => el == i) < 0)
                {
                    numbers.Add(i);
                    passedNumbers.AddRange(GetConnectedPoints(i));
                }
            }
            return numbers;
        }
		
		public List<int> GetClosestNode(int size)
		{
			List<int> closest = new List<int>();
			List<PairInt> ndRates = new List<PairInt>();
			List<int> rates = new List<int>();
			
			for (int i = 0; i < size; i++ )
				ndRates.Add(new PairInt(0, i));
			
			//distances.Sort(NodeDistance.CompareByDist);
			
			List<int> prevPrior = new List<int>();
			foreach (NodeDistance nd in distances)
			{
				if (!nd.connected)
				{
					ndRates.Find(el => el.y == nd.names.x).x++;
					ndRates.Find(el => el.y == nd.names.y).x++;
				}
			}
			
			ndRates.Sort(PairInt.CompareByX);
			int maxX = -1;
			//int numbBest = 0;
			foreach (PairInt rate in ndRates)
			{
				if ( (rate.x >= maxX) || (maxX < 0) )
				    //distances.FindIndex(el => ((el.names.x == rate.y || el.names.y == rate.y) && el.connected) )
				{
					maxX = rate.x;
					closest.Add(rate.y);
					//numbBest = rate.y;
				}
				/*else if ( (rate.x == maxX) && (rate.y < numbBest) )
				{
					maxX = rate.x;
					numbBest = rate.y;
				}*/
			}
			
			return closest;//ndRates.First().y;
		}

        public List<int> GetPriorites(int size, int startPrior)
        {
            distances.Sort(NodeDistance.CompareByDist);
            List<int> priorRet = new List<int>();
            for (int i = 0; i < size; i++ )
                priorRet.Add(-1);
            
            int count = 0;

            foreach (NodeDistance ndDis in distances)
            {
                bool findNdNum = false;
                if (priorRet[ndDis.names.x] == -1)
                {
                    List<int> onePrior = GetConnectedPoints(ndDis.names.x);
                    foreach (int j in onePrior)
                        priorRet[j] = startPrior - count;
                    findNdNum = true;
                }
                if (priorRet[ndDis.names.y] == -1)
                {
                    List<int> onePrior = GetConnectedPoints(ndDis.names.y);
                    foreach (int j in onePrior)
                        priorRet[j] = startPrior - count;
                    findNdNum = true;
                }
                if (findNdNum)
                    count--;
            }
            return priorRet;
        }

        public List<int> GetConnectedPoints(int inNodeNumber)
        {
            List<int> connectedPoints = new List<int>();
            connectedPoints.Add(inNodeNumber);
            //int curNumNode = inNodeNumber;
            bool nextSearch = false;

            do
            {
                nextSearch = false;
                //foreach (int curNumNode in connectedPoints)
                
                foreach (NodeDistance nd in distances)
                {
                    if (nd.connected && 
                	    (connectedPoints.FindIndex(el => el == nd.names.x) >= 0) &&
                        (connectedPoints.FindIndex(el => el == nd.names.y) < 0))
                    {
                        connectedPoints.Add(nd.names.y);
                        nextSearch = true;
                    }
                    if (nd.connected &&
                	    (connectedPoints.FindIndex(el => el == nd.names.y) >= 0) &&
                        (connectedPoints.FindIndex(el => el == nd.names.x) < 0))
                    {
                        connectedPoints.Add(nd.names.x);
                        nextSearch = true;
                    }
                }
            
            } while (nextSearch);

            return connectedPoints;
        }

        public int GetFixedPointTo(int inNumber)
        {
            int xNumb = distances.FindIndex(el => (el.names.x == inNumber) && el.isFixed);
            int yNumb = distances.FindIndex(el => (el.names.y == inNumber) && el.isFixed);
            if (xNumb >= 0)
                return distances[xNumb].names.y;
            if (yNumb >= 0)
                return distances[yNumb].names.x;
            return -1;
        }
		
		public NodeDistance BestDist()
		{
			//return distances.Min(NodeDistance.CompareByDist);
			distances.Sort(NodeDistance.CompareByDist);
			return distances.First();
		}
		
		// Compares by distance
        public static int CompareBaseByDist(NodeDistanceBase in1, NodeDistanceBase in2)
		{
            if (in1.distances.Count == 0 && in2.distances.Count == 0)
                return 0;
            else if (in1.distances.Count == 0)
                return 1;
            else if (in2.distances.Count == 0)
                return -1;
            

        	else if (!in1.BestDist().connected && in2.BestDist().connected)
    			return -1;
        	else if (!in1.BestDist().connected && !in2.BestDist().connected && in1.BestDist().distance < in2.BestDist().distance)
    			return -1;
        	else if (!in1.BestDist().connected && !in2.BestDist().connected && in1.BestDist().distance == in2.BestDist().distance)
    			return 0;
        	else if (in1.BestDist().connected && in2.BestDist().connected)
    			return 0;
			return 1;
		}

        // Compares by distance + execeptions
        public static int CompareExceptBase_old(NodeDistanceBase in1, NodeDistanceBase in2)
        {
            int number1 = exceptionNames.FindIndex(el => el == in1.name);
            int number2 = exceptionNames.FindIndex(el => el == in2.name);
            if ((number1 >= 0) && (number2 >= 0))
            {
                if (number1 < number2)
                    return -1;
                return 1;
            }
            if ((number1 >= 0) && (in2.name != bestName))
                return -1;
            if ((number2 >= 0) && (in1.name != bestName))
                return 1;
            if (in1.name == bestName)
            	return -1;
            if (in2.name == bestName)
            	return 1;
            return CompareBaseByDist(in1, in2);
        }
        
        // Compares by distance + execeptions
        public static int CompareExceptBase(NodeDistanceBase in1, NodeDistanceBase in2)
        {
        	if (in1.name == bestName)
        		return -1;
        	if (in2.name == bestName)
        		return 1;
        	
        	int number1 = exceptionNames.FindIndex(el => el == in1.name);
            int number2 = exceptionNames.FindIndex(el => el == in2.name);
            
            if (number1 >= 0) //&& (number2 >= 0))
            {
                if (number1 < number2)
                    return -1;
                else if (number2 >= 0)
                	return 1;
                else
                	return -1;
            }
            else if (number2 >= 0)
            {
            	return 1;
            }
            else return CompareBaseByDist(in1, in2);
        }
        
        public void UniteNodes(int node1, int node2)
        {
        	distances.Find( element => (element.names.x == node1 && element.names.y == node2) ||
	        	                    (element.names.x == node2 && element.names.y == node1) ).connected = true;
        	
        	List<int> connectedNumbs = new List<int>();
        	connectedNumbs.Add(node1);
        	connectedNumbs.Add(node2);
        	int prevCount = 0;
        	int curCount = 0;
        	do
        	{
        		prevCount = connectedNumbs.Count;
        		List<int> numbsOld = new List<int>(connectedNumbs);
        		
        		foreach (int number in numbsOld)
        			GetNxNummbs(number, connectedNumbs);
        		curCount = connectedNumbs.Count;
        	} while (prevCount < curCount);
        	
        	for (int i = 0; i < curCount; i++)
        	{
        		for (int j = i + 1; j < curCount; j++)
        		{
        			distances.Find( element => (element.names.x == connectedNumbs[i] && 
        			                            element.names.y == connectedNumbs[j]) ||
        			                            (element.names.x == connectedNumbs[j] && 
        			                             element.names.y == connectedNumbs[i]) ).connected = true;
        		}
        	}
        }
        
        private void GetNxNummbs(int start, List<int> retList)
        {
        	//List<int> retList = new List<int>();
        	foreach (NodeDistance ndDis in distances)
        	{
        		if (ndDis.connected && ndDis.names.x == start)
        		{
        			if (retList.FindIndex(el => el == ndDis.names.y) < 0)
        				retList.Add(ndDis.names.y);
        		}
        		if (ndDis.connected && ndDis.names.y == start)
        		{
        			if (retList.FindIndex(el => el == ndDis.names.x) < 0)
        				retList.Add(ndDis.names.x);
        		}
        	}
        }
        
        public bool IsTraced()
        {
        	if (distances.FindIndex(element => element.connected == false) < 0)
        		return true;
        	return false;
        }
        
        public int GetNumber(int inNumber)
        {
            return GetConnectedPoints(inNumber).Min();//nodePin[inNumber];
        }

        public void SetCountForNumber(int inNumber, int inCount)
        {
            foreach (int idx in GetConnectedPoints(inNumber))
                countNode[idx] = inCount;
        }
       

        /*public void SetPriority(int inPrior)
        {
            distances.Sort();
            int count = distances.Count;
            foreach (NodeDistance nd in distances)
            {
                nd
            }
        }*/
	}
	
	public class NodePriority
	{
		public List<int> nodeNumb;
		public string name;
		public int prior;
	}
	

    public class NodeTraces
    {
        public NodeTraces()
        {
            lines = new List<LineStruct>();
            crossing = new List<ContactSimple>();
        }

        public List<LineStruct> lines;
        public List<ContactSimple> crossing;
        
        public void AddTrace(NodeTraces inTrace)
        {
        	lines.AddRange(inTrace.lines);
        	crossing.AddRange(inTrace.crossing);
        }
        
        public void AddNeedTraces(NodeTraces inTrace)
        {
        	int i;
        	for (i = 0; i < inTrace.lines.Count; i++)
        	{
        		if (lines.FindIndex(el => el.IntersectsWith(inTrace.lines[i])) >= 0)
        			break;
        	}
        	for (int j = 0; j < i; j++)
        		lines.Add(inTrace.lines[j]);
        }
        
        /*public void DefineCrossContacts(TraceGlobe trace)
        {
        	for (int i = 0; i < lines.Count; i++)
            {
            	if (lines[i].Length() > 0)
            	{
            		if ((i + 1 < lines.Count) && (lines[i].type != lines[i + 1].type))
                	{
            			List<ContactSimple> inters = lines[i].IntersectsPointsLines(lines[i+1]);
            			if (inters.Count > 0)
            			{
                			int cntType = Params.DefineMaterial(lines[i].type, lines[i+1].type);
                			int idx = inters.FindIndex(el => trace.IsPointForContact(el, cntType));
            			
                			if (idx >= 0)
                			{
                				crossing.Add(new ContactSimple(inters[idx], cntType));
                			}
            			}
                	}
            	}
        	}
        }*/
    }
}
