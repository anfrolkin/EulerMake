
using System;
using System.Collections.Generic;
using System.Linq;

namespace eulerMake
{
    /// <summary>
    /// Description of Class1.
    /// </summary>
    public partial class TraceGlobe
    {
       
        /*private List<ContactSimple> NodeField(ContactSimple inStart, int numberNd)
        {
            List<ContactSimple> field = new List<ContactSimple>();
            List<ContactSimple> curWavePoint = new List<ContactSimple>();
            List<ContactSimple> tempWavePoint = new List<ContactSimple>();
            string curName = conflictManager.GetBestPriority();//bestHighPrior;

            tempWavePoint.Add(inStart);
            bool b  = false;

            do
            {
                foreach (ContactSimple cntUnit in tempWavePoint)
                {
                    foreach (ContactSimple pnt in cntUnit.GetArroundPoints(wide))
                    {
                        if ((GetPoint(pnt).name == curName) &&
                    	    (GetPoint(pnt).numberNode == numberNd) &&
                    	    (field.FindIndex(el => el == pnt) < 0))
                        {
                            field.Add(pnt);
                            curWavePoint.Add(pnt);
                        }
                    	foreach (int opposLayer in Params.LayersRange[pnt.layer])
                    	{
                    		ContactSimple opposPnt = new ContactSimple(pnt, opposLayer);
                    		if ((GetPoint(opposPnt).name == curName) &&
                    		    (GetPoint(opposPnt).numberNode == numberNd) &&
                    		    (field.FindIndex(el => el == opposPnt) < 0))
                    		{
                    			field.Add(opposPnt);
                    			curWavePoint.Add(opposPnt);
                    		}
                    		    
	                        
                    	}
                    }
                }
                
                if (curWavePoint.FindIndex(el => (el.x == 12 && el.y == 17 && el.layer == Layers.siliconTrace)) >= 0)
            	    b = true;
                
                tempWavePoint.Clear();
                tempWavePoint.AddRange(curWavePoint);
                curWavePoint.Clear();
            }
            while (tempWavePoint.Count > 0);

            return field;
        }*/

        private List<ContactSimple> FindInNearRing(List<ContactSimple> allArray, List<ContactSimple> lastRing)
        {
            List<ContactSimple> newArrea = new List<ContactSimple>();
            foreach (ContactSimple cntUnit in lastRing)
            {
                foreach (ContactSimple pnt in cntUnit.GetArroundPoints(wide))
                {
                	if ( (allArray.FindIndex(el => el == pnt) < 0) && 
                	    (newArrea.FindIndex(el => el == pnt) < 0) )
                    {
                        newArrea.Add(pnt);
                    }
                    /*if (allArray.FindIndex(el => el == GetOppositeContact(pnt)) < 0)
                    {
                        newArrea.Add(GetOppositeContact(pnt));
                        inArray.Add(GetOppositeContact(pnt));
                    }*/
                }
            }
            return newArrea;
        }

        private bool SearchNearest()
        {
            int allAcount = 2 * wide * Params.topEdge;
            //conflictManager = new ConflictPolitic();
            string highPrior = conflictManager.GetBestPriority();
            bool isNewRule = false;

            Node bestNode = nodeList.Find(el => (el.name == highPrior));//bestHighPrior));

            NodeDistance needTrace = nodeDistanceDict[highPrior].BestDist();

            int numberStart = nodeDistanceDict[highPrior].GetNumber(needTrace.names.x);
           // ContactSimple startOne = new ContactSimple(bestNode.arcCollection[numberStart]);


            List<ContactSimple> startWave = FindStartContacts(new ContactSimple(
            				bestNode.arcCollection[numberStart]), bestNode.name);
			SetContact(startWave, bestNode.name, numberStart, 1, 0);
			NodePointProcess proc = new NodePointProcess(GetPoint(startWave[0]), false);
			
			List<ContactSimple> startArray = ReturnAllWaveProcess(startWave, proc);
            
            //List<ContactSimple> startArray = NodeField(startOne, numberStart);
            List<ContactSimple> curArray = new List<ContactSimple>(startArray);
            List<ContactSimple> lastRing = new List<ContactSimple>(startArray);
            
            List<ContactSimple> findCnts = new List<ContactSimple>();


            //int numberToReach = nodeDistanceDict[highPrior].GetNumber(needTrace.names.y);

            do
            {
	            do
	            {
	            	findCnts.Clear();
	            	
		            do
		            {
		                List<ContactSimple> nextRing = FindInNearRing(curArray, lastRing);
		                foreach (ContactSimple cnt in nextRing)
		                {
		                	if (GetPoint(cnt).name == highPrior && GetPoint(cnt).numberNode != numberStart)//== numberToReach)
		                    {
		                        findCnts.Add(cnt);
		                    }
		                }
		                
		                {
		                    curArray.AddRange(nextRing);
		                    lastRing = nextRing;
		                }
		            }
		            while ((findCnts.Count <= 0) && (curArray.Count < allAcount) && (lastRing.Count > 0));
		
		            
		            conflictManager.ResetConnection();
		
		            foreach (ContactSimple cnt in findCnts)
		            {
		                ContactSimple cntNear = FindNearestPoint(startArray, cnt);
		                
		                ConflictParametr conflicts = FindIntersects(cntNear, cnt, highPrior);
		                //ConflictParametr conflicts = OptimizeIntersects(conflicts1);
		                
		                if (conflicts.GetCountConfl() >= 0)
		                {
		                	//bool isBlockade = false;
		                	int numberToReach = GetPoint(cnt).numberNode;
			                conflictManager.AddConnection(GetPoint(cnt), GetPoint(cntNear), cnt.layer,
		                	                              cntNear.GetDistance(cnt), conflicts, nodeDistanceDict[highPrior].countNode[numberToReach], 
		                	                              nodeDistanceDict[highPrior].countNode[numberStart]);
		                }
		            }
		            
		            bool b;
		            if (conflictManager.GetCountConflicts() <= 0)
		            	b = false;
	            }
	            while ((conflictManager.GetCountConflicts() <= 0) && (curArray.Count < allAcount) && (lastRing.Count > 0));
	
	            isNewRule = conflictManager.DefineRule();
	            
	        }
	        while ((!isNewRule)&& (curArray.Count < allAcount) && (lastRing.Count > 0));
            
            
            return isNewRule;
        }
/*
        private bool SearchNearestComplex()
        {
        	int allAcount = 2 * wide * Params.topEdge;
            //conflictManager = new ConflictPolitic();
            string highPrior = conflictManager.GetBestPriority();
            bool isNewRule = false;

            Node bestNode = nodeList.Find(el => (el.name == highPrior));//bestHighPrior));

            NodeDistance needTrace = nodeDistanceDict[highPrior].BestDist();

            int numberStart = nodeDistanceDict[highPrior].GetNumber(needTrace.names.x);
            ContactSimple startOne = new ContactSimple(bestNode.arcCollection[numberStart]);


            List<ContactSimple> startArray = NodeField(startOne, numberStart);
            List<ContactSimple> curArray = new List<ContactSimple>(startArray);
            List<ContactSimple> lastRing = new List<ContactSimple>(startArray);
            
            List<ContactSimple> findCnts = new List<ContactSimple>();


            int numberToReach = nodeDistanceDict[highPrior].GetNumber(needTrace.names.y);

            do
            {
	            do
	            {
	            	findCnts.Clear();
	            	
		            do
		            {
		                List<ContactSimple> nextRing = FindInNearRing(curArray, lastRing);
		                foreach (ContactSimple cnt in nextRing)
		                {
		                    if (GetPoint(cnt).name == highPrior && GetPoint(cnt).numberNode == numberToReach)
		                    {
		                        findCnts.Add(cnt);
		                    }
		                }
		                //if (findCnts.Count <= 0)
		                {
		                    curArray.AddRange(nextRing);
		                    lastRing = nextRing;
		                }
		            }
		            while ((findCnts.Count <= 0) && (curArray.Count < allAcount) && (lastRing.Count > 0));
		
		            
		            conflictManager.ResetConnection();
		
		            foreach (ContactSimple cnt in findCnts)
		            {
		                ContactSimple cntNear = FindNearestPoint(startArray, cnt);
		                
		                ConflictParametr conflicts = FindIntersectsComplex(cntNear, cnt, highPrior);
		                
		                if (conflicts.GetCountConfl() >= 0)
		                {
		                	conflictManager.AddConnection(GetPoint(cnt), GetPoint(cntNear), cnt.layer,
		                	                              cntNear.GetDistance(cnt), conflicts, nodeDistanceDict[highPrior].countNode[numberToReach], 
		                	                              nodeDistanceDict[highPrior].countNode[numberStart]);
		                }
		            }
		            
		            bool b;
		            if (conflictManager.GetCountConflicts() <= 0)
		            	b = false;
	            }
	            while ((conflictManager.GetCountConflicts() <= 0) && (curArray.Count < allAcount) && (lastRing.Count > 0));
	
	            isNewRule = conflictManager.DefineRule();
	            
	        }
	        while ((!isNewRule)&& (curArray.Count < allAcount) && (lastRing.Count > 0));
            
            
            return isNewRule;
        }*/

        private ContactSimple FindNearestPoint(List<ContactSimple> inArray, ContactSimple inCnt)
        {
            ContactSimple bestContact = inArray[0];
            int distance = 1000;//bestContact.GetDistance(inCnt);
            foreach (ContactSimple curCnt in inArray)
            {
            	if ((curCnt.layer == inCnt.layer) && (curCnt.GetDistance(inCnt) < distance))
                {
                    distance = curCnt.GetDistance(inCnt);
                    bestContact = curCnt;
                }
            }

            return bestContact;
        }

        private ConflictParametr OptimizeIntersects(ConflictParametr inConfl)
        {
        	ConflictParametr retConfl = new ConflictParametr(inConfl);
        	foreach (string oneName in inConfl.GetNames())
        	{
        		if (!nodeDistanceDict[oneName].IsTraced())
        			retConfl.DeleteName(oneName);
        	}
        	return retConfl;
        }
        
        private ConflictParametr FindIntersects(ContactSimple cnt1, ContactSimple cnt2, string inName)
        {
            ConflictParametr names1 = new ConflictParametr();
            ConflictParametr names2 = new ConflictParametr();

            bool name1Fixed = false;
            bool name2Fixed = false;

            foreach (PairInt curCnt in GetXConnect(cnt1, cnt2))
            {
                string curName = GetPoint(curCnt, cnt1.layer).name;

                if ((curName != Material.blankName) && (curName != Material.diffusionName) &&
                    (curName != inName))// && (names1.FindIndex(el => el == curName) < 0))
                    names1.AddName(curName, GetPoint(curCnt, cnt1.layer).number);

                if (GetPoint(curCnt, cnt1.layer).isFixed)
                    name1Fixed = true;
            }
            names1.SetCountConfl(names1.GetNameCount());

            if (names1.GetNameCount() < 1)
            {
                foreach (PairInt curCnt in GetXConnect(cnt1, cnt2))
                {
                    foreach (PairInt near in curCnt.GetArroundPoints(wide))
                    {
                        string curName = GetPoint(near, cnt1.layer).name;

                        if ((curName != Material.blankName) && (curName != Material.diffusionName) &&
                            (curName != inName))// && (names1.FindIndex(el => el == curName) < 0))
                            names1.AddName(curName, GetPoint(near, cnt1.layer).number);
                        
                        if (GetPoint(near, cnt1.layer).isFixed)
                    		name1Fixed = true;
                    }
                }
            }
                

            foreach (PairInt curCnt in GetYConnect(cnt1, cnt2))
            {
                string curName = GetPoint(curCnt, cnt1.layer).name;
                int curNumber = GetPoint(curCnt, cnt1.layer).number;

                if ((curName != Material.blankName) && (curName != Material.diffusionName) &&
                    (curName != inName))// && (names1.FindIndex(el => el == curName) < 0))
                    names2.AddName(curName, curNumber);
                
                if (GetPoint(curCnt, cnt1.layer).isFixed)
                    name2Fixed = true;
            }
            names2.SetCountConfl(names2.GetNameCount());

            if (names2.GetNameCount() < 1)
            {
                foreach (PairInt curCnt in GetYConnect(cnt1, cnt2))
                {
                    foreach (PairInt near in curCnt.GetArroundPoints(wide))
                    {
                        string curName = GetPoint(near, cnt1.layer).name;
                        int curNumber = GetPoint(near, cnt1.layer).number;

                        if ((curName != Material.blankName) && (curName != Material.diffusionName) &&
                            (curName != inName))// && (names1.FindIndex(el => el == curName) < 0))
                            names2.AddName(curName, curNumber);
                        
                        if (GetPoint(near, cnt1.layer).isFixed)
                    		name2Fixed = true;
                    }
                }
            }
            

            if ( ((names2.GetCountConfl() < names1.GetCountConfl()) && (!name2Fixed)) || (name1Fixed && !name2Fixed) )
                return names2;
            if (!name1Fixed)
            	return names1;
            names1.SetCountConfl(-1);
            return names1;
        }

        private ConflictParametr FindIntersectsComplex(ContactSimple cnt1, ContactSimple cnt2, string inName)
        {
            ConflictParametr names1 = new ConflictParametr();
            ConflictParametr names2 = new ConflictParametr();

            bool name1Fixed = false;
            bool name2Fixed = false;

            /*foreach (PairInt curCnt in GetXConnect(cnt1, cnt2))
            {
                string curName = GetPoint(curCnt, cnt1.layer).name;

                if ((curName != Material.blankName) && (curName != Material.diffusionName) &&
                    (curName != inName))// && (names1.FindIndex(el => el == curName) < 0))
                    names1.AddName(curName, GetPoint(curCnt, cnt1.layer).number);

                if (GetPoint(curCnt, cnt1.layer).isFixed)
                    name1Fixed = true;
            }
            names1.SetCountConfl(names1.GetNameCount());

            if (names1.GetNameCount() < 1)*/
            {
                foreach (PairInt curCnt in GetXConnect(cnt1, cnt2))
                {
                    foreach (PairInt near in curCnt.GetArroundPoints(wide))
                    {
                        string curName = GetPoint(near, cnt1.layer).name;

                        if ((curName != Material.blankName) && (curName != Material.diffusionName) &&
                            (curName != inName))// && (names1.FindIndex(el => el == curName) < 0))
                            names1.AddName(curName, GetPoint(near, cnt1.layer).number);

                        if (GetPoint(near, cnt1.layer).isFixed)
                            name1Fixed = true;
                    }
                }
                names1.SetCountConfl(names1.GetNameCount());
            }


            /*foreach (PairInt curCnt in GetYConnect(cnt1, cnt2))
            {
                string curName = GetPoint(curCnt, cnt1.layer).name;
                int curNumber = GetPoint(curCnt, cnt1.layer).number;

                if ((curName != Material.blankName) && (curName != Material.diffusionName) &&
                    (curName != inName))// && (names1.FindIndex(el => el == curName) < 0))
                    names2.AddName(curName, curNumber);

                if (GetPoint(curCnt, cnt1.layer).isFixed)
                    name2Fixed = true;
            }
            names2.SetCountConfl(names2.GetNameCount());

            if (names2.GetNameCount() < 1)*/
            {
                foreach (PairInt curCnt in GetYConnect(cnt1, cnt2))
                {
                    foreach (PairInt near in curCnt.GetArroundPoints(wide))
                    {
                        string curName = GetPoint(near, cnt1.layer).name;
                        int curNumber = GetPoint(near, cnt1.layer).number;

                        if ((curName != Material.blankName) && (curName != Material.diffusionName) &&
                            (curName != inName))// && (names1.FindIndex(el => el == curName) < 0))
                            names2.AddName(curName, curNumber);

                        if (GetPoint(near, cnt1.layer).isFixed)
                            name2Fixed = true;
                    }
                }
                names2.SetCountConfl(names2.GetNameCount());
            }


            if (((names2.GetCountConfl() < names1.GetCountConfl()) && (!name2Fixed)) || (name1Fixed && !name2Fixed))
                return names2;
            if (!name1Fixed)
                return names1;
            names1.SetCountConfl(-1);
            return names1;
        }

        private List<PairInt> GetYConnect(ContactSimple cnt1, ContactSimple cnt2)
        {
            int dX = 1;
            int dY = 1;
            if (cnt2.y < cnt1.y)
                dY = -1;
            else if (cnt2.y == cnt1.y)
                dY = 0;
            if (cnt2.x < cnt1.x)
                dX = -1;
            else if (cnt2.x == cnt1.x)
                dX = 0;

            List<PairInt> chain = new List<PairInt>();
            PairInt curCnt = new PairInt(cnt1);
            chain.Add(new PairInt(curCnt));
            do
            {
                if (curCnt.y != cnt2.y)
                    curCnt.y += dY;
                else
                    curCnt.x += dX;

                chain.Add(new PairInt(curCnt));
            }
            while (curCnt != cnt2);

            return chain;
        }

        private List<PairInt> GetXConnect(ContactSimple cnt1, ContactSimple cnt2)
        {
            int dX = 1;
            int dY = 1;
            if (cnt2.y < cnt1.y)
                dY = -1;
            else if (cnt2.y == cnt1.y)
                dY = 0;
            if (cnt2.x < cnt1.x)
                dX = -1;

            List<PairInt> chain = new List<PairInt>();
            PairInt curCnt = new PairInt(cnt1);
            chain.Add(new PairInt(curCnt));
            do
            {
                if (curCnt.x != cnt2.x)
                    curCnt.x += dX;
                else
                    curCnt.y += dY;

                chain.Add(new PairInt(curCnt));
            }
            while (curCnt != cnt2);

            return chain;
        }

        public Dictionary<string, NodeTraces> GetTraces(System.IO.StreamWriter file)
        {
        	SetAllUnused();
        	ClearForAlign(file);
        	
        	file.WriteLine("---------GetTraces----------");
        	PrintMap(Layers.metal1Trace, file);
			PrintMap(Layers.siliconTrace, file);
			PrintMap(Layers.metal2Trace, file);
        	PrintNumb(100, file);
			PrintNumbM2(100, file);
        	
        	Dictionary<string, NodeTraces> dict = new Dictionary<string, NodeTraces>();
        	foreach (Node nd in nodeList)
        	{
        		NodeTraces curTrace = GetNodeTrace(nd, file);
                
        		dict.Add(nd.name, curTrace);
        	}
        	return dict;
        }
        
       
        
        public NodeTraces RecoveryPath(ContactSimple startPoint, ContactSimple endPoint, string curName)
        {
        	NodeTraces trace = new NodeTraces();
        	
        	List<ContactSimple> endPosition = GetSourceContacts(endPoint, curName);
        	
        	bool selectFirst = false;
        	if ( diffusionException.FindIndex(el => el == curName) >= 0 )
        		selectFirst = true;
        	List<ContactSimple> starts = GetSourceContacts(startPoint, curName);
        	ContactSimple curPoint = starts[0];
        	foreach (ContactSimple cnt in starts)
        	{
        		if (GetPoint(cnt).name == curName)
        		{
        			if (selectFirst)
        			{
	        			curPoint = cnt;
	        			break;
        			}
        			else
        			{
        				if ( (GetPoint(curPoint).name != curName) || (GetPoint(curPoint).number > GetPoint(cnt).number) )
        					curPoint = cnt;
        			}
        		}
        	}
        	
            
        	List<int> setOfLayers = new List<int>();//GetLayers(curPoint, curName);
        	setOfLayers.Add(startPoint.layer);
        	
            bool nextPoints = false;
            bool loopGo = true;
            int k = 0;
            
            while ( loopGo && k < 100)
            {
            	k++;
            	List<LineStruct> setOfLines = new List<LineStruct>();
            	
            	nextPoints = true;
            	foreach (int layerUnit in setOfLayers)
            	{
	            	setOfLines.Add(GetLineVertical(curPoint, layerUnit, curName));
	            	setOfLines.Add(GetLineHorizont(curPoint, layerUnit, curName));
            	}
            	
            	//bool bestFinded = false;
            	LineStruct curLine = setOfLines[0];
            	foreach (LineStruct searchLine in setOfLines)
            	{
            		ContactSimple oposPnt = new ContactSimple(searchLine.OpositePoint(curPoint));
            			if (GetPoint(oposPnt).isUsed && (GetPoint(oposPnt).name == curName))
            		{
            			curLine = searchLine;
            			loopGo = false;
            		}
            	}
            	
            	if (loopGo)
            		curLine = SelectBestLine(setOfLines, trace.lines);//setOfLines.Max();
            	trace.lines.Add(curLine);
                

            	/*if ( (curLine.type == Layers.siliconTrace && curPoint.layer == Layers.metal1Trace) ||
                    (curLine.type == Layers.metal1Trace && curPoint.layer == Layers.siliconTrace) )
            	{
            		trace.crossing.Add(new ContactSimple(curPoint, Params.csiType));
            	}*/
            	
            	curPoint = curLine.OpositePoint(curPoint);
            	setOfLayers = GetLayers(curPoint, curName);
        		
                if ( endPosition.FindIndex(element => (element == curPoint) &&
            	                           (element.layer == curPoint.layer)) >= 0 )
                    loopGo = false;
            }
            
            /*if ( (curPoint.layer == Params.met1Type) && (curPoint.y < Params.lineMiddle) )
        		trace.crossing.Add(new ContactSimple(curPoint, Params.cpaType));
            if ( (curPoint.layer == Params.met1Type) && (curPoint.y > Params.lineMiddle) )
        		trace.crossing.Add(new ContactSimple(curPoint, Params.cnaType));
            */
            return trace;
        }
        
        private LineStruct SelectBestLine(List<LineStruct> setOfLines, List<LineStruct> prevLines)
        {
        	LineStruct curLine = setOfLines[0];
        	curLine = setOfLines.Max();
        	if (curLine.Length() == 0)// && (prevLine.type == curLine.type))
        	{
        		for (int i = (prevLines.Count - 1); i >= 0; i--)
        		{
        			int idx = setOfLines.FindIndex(el => el.type != prevLines[i].type);
        			if (idx >= 0)
        				return setOfLines[idx];
        			if (prevLines[i].Length() > 0)
        				return setOfLines[0];
        		}
        	}
        	return curLine;
        }
        
        private ContactSimple GetNextPoint(List<LineStruct> curLines, ContactSimple inPoint)
        {
    		if (GetPoint(inPoint).isSource)//, Layers.contactTrace).name ==
    		{
    			string curName = GetPoint(inPoint).name;
    			foreach (ContactSimple cnt in GetSourceContacts(inPoint, curName))
    				if ((GetPoint(cnt).name == curName) && (curLines.FindIndex(el => el.OverlapWithPoint(cnt.x, cnt.y, cnt.layer)) < 0))
    					return cnt;
    		}
    		return inPoint;
        }
        
        private void AddStartEndContacts(NodeTraces inTrace, ContactSimple startPoint)
        {
        	if ((startPoint.layer == Layers.metal1Trace) && ( (Math.Abs(startPoint.y - Params.lineN) < 4)
        	                                                 || (Math.Abs(startPoint.y - Params.lineP) < 4) ))
			{
        		if ( (Math.Abs(startPoint.y - Params.lineN) < 4) && FindTrueSource(startPoint) )
				{
					inTrace.crossing.Add(new ContactSimple(startPoint, Material.cna_));
					if (Math.Abs(startPoint.y - Params.lineN) > 1)
						inTrace.lines.Add(new LineStruct(new PairInt(startPoint.x, Params.lineN), startPoint, Material.na_));
				}
        		if ( (Math.Abs(startPoint.y - Params.lineP) < 4) && FindTrueSource(startPoint) )
				{
					inTrace.crossing.Add(new ContactSimple(startPoint, Material.cpa_));
					if (Math.Abs(startPoint.y - Params.lineP) > 1)
						inTrace.lines.Add(new LineStruct(new PairInt(startPoint.x, Params.lineP), startPoint, Material.pa_));
				}
			}
        	else if ((startPoint.layer == Layers.metal1Trace) && Params.IsModelBusM2InMiddle())
        		inTrace.crossing.Add(new ContactSimple(startPoint, Material.cm_));
        }
        
        private NodeTraces RecoveryUnicPath(List<LineStruct> prevLines, ContactSimple startPoint, ContactSimple endPoint, string curName)
        {
        	NodeTraces trace = new NodeTraces();
        	
        	//List<ContactSimple> endPosition = GetSourceContacts(endPoint, curName);
        	bool selectFirst = false;
        	if ( diffusionException.FindIndex(el => el == curName) >= 0 )
        		selectFirst = true;
        	List<ContactSimple> starts = GetSourceContacts(startPoint, curName);
        	ContactSimple curPoint = starts[0];
        	foreach (ContactSimple cnt in starts)
        	{
        		NodePoint pn = GetPoint(cnt);
        		if (GetPoint(cnt).name == curName)
        		{
        			if ( selectFirst && ( GetLineVertical(prevLines, cnt, cnt.layer, curName).Height > 0 ||
        			                    GetLineHorizont(prevLines, cnt, cnt.layer, curName).Width > 0 ) )
        			{
        				
	        			curPoint = cnt;
	        			break;
        			}
        			else
        			{
        				if ( (GetPoint(curPoint).name != curName) || (GetPoint(curPoint).number > GetPoint(cnt).number) )
        					curPoint = cnt;
        			}
        		}
        	}
        	
        	AddStartEndContacts(trace, curPoint);
        	
        	List<int> setOfLayers = new List<int>();//GetLayers(curPoint, curName);
        	setOfLayers.Add(startPoint.layer);
        	
            //bool nextPoints = false;
            bool loopGo = true;
            int k = 0;
            
            while ( loopGo && k < 100)
            {
            	k++;
            	List<LineStruct> setOfLines = new List<LineStruct>();
            	
            	
            	foreach (int layerUnit in setOfLayers)
            	{
	            	setOfLines.Add(GetLineVertical(prevLines, curPoint, layerUnit, curName));
	            	setOfLines.Add(GetLineHorizont(prevLines, curPoint, layerUnit, curName));
            	}
            	
            	LineStruct curLine = setOfLines[0];
            	foreach (LineStruct searchLine in setOfLines)
            	{
            		ContactSimple oposPnt = new ContactSimple(searchLine.OpositePoint(curPoint));
            		if ((searchLine.Length() > 0) && GetPoint(oposPnt).isUsed && (GetPoint(oposPnt).name == curName))
            		{
            			curLine = searchLine;
            			loopGo = false;
            		}
            		
            		foreach (ContactSimple cntUnit in searchLine.GetPointArray())
            		{
            			if ( prevLines.FindIndex(el => el.OverlapWithPoint(cntUnit)) >= 0 )
	            		{
	            			curLine = searchLine;
	            			loopGo = false;
	            		}
            		}
            	}
            	
            	if (loopGo)
            		curLine = SelectBestLine(setOfLines, trace.lines);
            	
            	if (curLine.type != curPoint.layer)
            		trace.crossing.Add(new ContactSimple(curPoint, Params.DefineMaterial(curLine.type, curPoint.layer)));
            	
            	curPoint = curLine.OpositePoint(curPoint);
            	
            	if (curLine.Length() > 0)
            		trace.lines.Add(curLine);
            	else
            	{
            		ContactSimple pnt = GetNextPoint(trace.lines, curPoint);
            		if (pnt != curPoint)
            			AddStartEndContacts(trace, curPoint);
            		curPoint = pnt;
            	}
                
            	setOfLayers = GetLayers(curPoint, curName);
        		
            	foreach (int layerUnit in setOfLayers)
            	{
	            	if ( (GetPoint(curPoint, layerUnit).name == curName) && (GetPoint(curPoint, layerUnit).number == 0) )//endPosition.FindIndex(element => (element == curPoint) &&
	            	       //                    (element.layer == curPoint.layer)) >= 0 )
	                    loopGo = false;
            	}
            	
            	SetUsedInLine(curLine);
            	
            	if (k > 97)
            		loopGo = true;
            }
            
            if (k < 100)
            	AddStartEndContacts(trace, curPoint);
            
            return trace;
        }

        public NodeTraces RecoveryFixedPath(ContactSimple startPoint, string curName)
        {
            NodeTraces trace = new NodeTraces();

            //List<ContactSimple> endPts = GetSourceContacts(endPoint, curName);
            ContactSimple sPoint = new ContactSimple(startPoint);
            /*foreach (ContactSimple cnt in GetSourceContacts(startPoint, curName))
            	if (GetPoint(cnt).isFixed)
            		sPoint = cnt;*/
            List<ContactSimple> passedPts = new List<ContactSimple>();
            
            if (!startPoint.isInOut())
            {
            	sPoint = startPoint.GetInDiffusionEdge();//.GetHigherPoint(0);
	            //if (startPoint.y > Params.lineMiddle)
	              //  sPoint = startPoint.GetLowerPoint(0);
            }

            NodePoint ndPoint = GetPoint(sPoint);
            passedPts.Add(sPoint);
            LineStruct ln = new LineStruct(startPoint, startPoint);
            do
            {
            	foreach (int  lay in Params.allLayersRange[sPoint.layer])//pin-pong + wrong layer!!!!!!
            	{
	            	ln = GetFixedLine(passedPts, lay, curName);
	            	if (ln.Length() > 0)// && (passedPts.FindIndex(el => el == ln.OpositePoint(sPoint)) < 0))
	            	{
	            		trace.lines.Add(ln);
	            		break;
	            	}
            	}
            	sPoint = ln.OpositePoint(sPoint);
            	passedPts.Add(sPoint);
            	ndPoint = GetPoint(sPoint);
            } while(!ndPoint.isSource && ndPoint.number > 0);//GetPoint(sPoint).isSource
            
            return trace;
        }
        
        private List<int> GetLayers(ContactSimple inPoint, string inName)//, bool nextPoints)
        {
        	List<int> retLayers = new List<int>();
        	
        	if ( inPoint.layer == Layers.metal1Trace )
        	{
        		retLayers.Add(Layers.metal1Trace);
        		if ((GetPoint(inPoint, Layers.contactTrace).name != Material.diffusionName) && 
        		    (diffusionException.FindIndex(el => el == inName) < 0))
        			retLayers.Add(Layers.siliconTrace);
        		if (Params.IsModelBusM1InMiddle() && //(GetInstance(inPoint, Layers.contactTrace) != Material.diffusion) &&
        		    (diffusionException.FindIndex(el => el == inName) < 0))
        			retLayers.Add(Layers.metal2Trace);
        	}
        	else if ( inPoint.layer == Layers.siliconTrace)
        	{
        		retLayers.Add(Layers.siliconTrace);
        		if ( GetPoint(inPoint, Layers.contactTrace).name != Material.diffusionName )
        			retLayers.Add(Layers.metal1Trace);
        	}
        	else if ( inPoint.layer == Layers.metal2Trace)
        	{
        		retLayers.Add(Layers.metal2Trace);
        		//if ( (GetInstance(inPoint, Layers.contactTrace) != Material.diffusion) )//Params.IsModelBusM1InMiddle()
        			retLayers.Add(Layers.metal1Trace);
        	}
        	
        	return retLayers;
        }
        
        private void SetAllUnused()
        {
        	foreach (int lay in Params.UsedLayers)
        	{
	        	for (int x = 0; x < wide; x++)
	            {
	                for (int y = 0; y < Params.topEdge; y++)
	                	layoutMap[x][y][lay].isUsed = false;
	        	}
        	}
        }
        
        //private void 
        
        /*private void FillUnused()
        {
        	for (int x = 0; x < wide; x++)
            {
                for (int y = 0; y < Params.topEdge; y++)
                {
                	if ( !layoutMap[x][y][Layers.metal1Trace].isUsed )
                	{
                		SetContact(new ContactSimple(x,y,Layers.metal1Trace), Material.blankName, 0, 0, -1);
                		layoutMap[x][y][Layers.metal1Trace].isReplace = true;
                	}
                	if ( !layoutMap[x][y][Layers.siliconTrace].isUsed && 
                	    (layoutMap[x][y][Layers.siliconTrace].name != Material.diffusionName) )
                	{
                		SetContact(new ContactSimple(x,y,Layers.siliconTrace), Material.blankName, 0, 0, -1);
                		layoutMap[x][y][Layers.metal1Trace].isReplace = true;
                	}
                }
        	}
        }*/

        /*public void ClearForAlign_old()
        {
            foreach (Node nd in nodeList)
                SpreadUsed(nd);
            SetUsedForUntraced();

            foreach (Node nd in nodeList)
            {
                List<int> currentConnection = new List<int>();
                for (int i = 0; i < nd.arcCollection.Count; i++)
                {
                    if (currentConnection.FindIndex(el => el == i) < 0)
                    {
                        List<int> curList = nodeDistanceDict[nd.name].GetConnectedPoints(i);
                        //ContactSimple startPoint = new ContactSimple(nd.arcCollection[i]);
                        List<ContactSimple> startPoints = FindStartPoint(nd.arcCollection[i], nd.name);

                        SetContactNumber(startPoints, 0);
                        ReplaceUnused procRemove = new ReplaceUnused(GetPoint(startPoints[0]), false);
                        int count = SpreadWaveProcess(startPoints, procRemove);
                        nodeDistanceDict[nd.name].SetCountForNumber(i, count);

                        currentConnection.AddRange(curList);
                    }
                }
            }
            SetAllUnused();
        }*/
        
        public void ClearForAlign(System.IO.StreamWriter inFile)
        {
        	
        	foreach (Node nd in nodeList)
            {
        		if (nodeDistanceDict[nd.name].IsTraced())
        		{
	                List<int> currentConnection = new List<int>();
	                for (int i = 0; i < nd.arcCollection.Count; i++)
	                {
	                    if (currentConnection.FindIndex(el => el == i) < 0)
	                    {
	                        List<int> curList = nodeDistanceDict[nd.name].GetConnectedPoints(i);
	                        List<ContactSimple> startPoints = FindStartPoint(nd.arcCollection[i], nd.name);
	                        SetContactNumber(startPoints, 0);
	                        //ReplaceUnused procRemove = new ReplaceUnused(0, GetPoint(startPoint).priority, false);
	                        NodePointProcess proc = new NodePointProcess(GetPoint(startPoints[0]), false);
	                        int count = CompleteSpreadWaveProcess(startPoints, proc);
	                        nodeDistanceDict[nd.name].SetCountForNumber(i, count);
	
	                        currentConnection.AddRange(curList);
	                    }
	                }
        		}
            }
        	
        	CheckOnePoint();
            foreach (Node nd in nodeList)
            {
            	if (nodeDistanceDict[nd.name].IsTraced())
                	SpreadUsed(nd);
            }
            CheckOnePoint();
            //SetUsedForUntraced();
            
            CheckOnePoint();

            foreach (Node nd in nodeList)
            {
            	if (nodeDistanceDict[nd.name].IsTraced())
            	{
	                List<int> currentConnection = new List<int>();
	                for (int i = 0; i < nd.arcCollection.Count; i++)
	                {
	                    if (currentConnection.FindIndex(el => el == i) < 0)
	                    {
	                        List<int> curList = nodeDistanceDict[nd.name].GetConnectedPoints(i);
	                        //ContactSimple startPoint = new ContactSimple(nd.arcCollection[i]);
	                        List<ContactSimple> startPoints = FindStartPoint(nd.arcCollection[i], nd.name);
	
	                        SetContactNumber(startPoints, 0);
	                        ReplaceUnused procRemove = new ReplaceUnused(GetPoint(startPoints[0]), false);
	                        int count = CompleteSpreadWaveProcessRem(startPoints, procRemove);
	                        //if (CheckOnePoint())
	                        //	CompleteSpreadWaveProcessRem(startPoints, procRemove);
	                        
	                        nodeDistanceDict[nd.name].SetCountForNumber(i, count);
	
	                        currentConnection.AddRange(curList);
	                        
	                    }
	                }
            	}
            }
            CheckOnePoint();
            SetAllUnused();
        	
        }

        private void SetUsedForUntraced()
        {
            foreach (Node nd in nodeList)
            {
                for (int i = 0; i < nd.arcCollection.Count; i++)
                {
                    if (nodeDistanceDict[nd.name].GetConnectedPoints(i).Count < 2)
                    {
                        ContactSimple up = new ContactSimple(nd.arcCollection[i].GetHigherPoint(0));
                        if (GetPoint(up).name == nd.name)
                        {
                            GetPoint(up).isUsed = true;
                            GetPoint(up).isReplace = false;
                        }
                        //SetPinContact(up, nd.name, 0, 1);
                        ContactSimple down = new ContactSimple(nd.arcCollection[i].GetLowerPoint(0));
                        if (GetPoint(down).name == nd.name)
                        {
                            GetPoint(down).isUsed = true;
                            GetPoint(down).isReplace = false;
                        }
                        //SetPinContact(down, nd.name, 0, 1);
                    }
                }
            }
        }

       
        
        private void SetUsed(NodeTraces inTraces)
        {
        	foreach (LineStruct lnUnit in inTraces.lines)
        	{
        		bool isSuccess = SetUsedInLine(lnUnit);
        		//if (!isSuccess)
        		//	return;
        	}
        }
        
        private void SpreadUsed(Node nd)
        {
        	NodeTraces curTrace = new NodeTraces();
        	
        	List<int> fixedConnection = new List<int>();
    		List<int> currentConnection = new List<int>();
    		for (int i = 0; i < nd.arcCollection.Count; i++)
    		{
    			if (currentConnection.FindIndex(el => el == i) < 0)
    			{
        			List<int> curList = nodeDistanceDict[nd.name].GetConnectedPoints(i);
        			int curStart = curList.Min();
        			currentConnection.Add(curStart);

        			foreach (int curEnd in curList)
        			{
                        if (currentConnection.FindIndex(el => el == curEnd) < 0) //(curStart != curEnd)
                        {
                            int numbFixedWith = nodeDistanceDict[nd.name].GetFixedPointTo(curEnd);
                            if ((numbFixedWith >= 0))
                            	fixedConnection.Add(curEnd);
                            	//curTrace.AddTrace(RecoveryFixedPath(nd.arcCollection[curEnd], nd.name));
                            
                            if (numbFixedWith != curStart)
                            {
                            	NodeTraces recPath = RecoveryUnicPath(curTrace.lines, nd.arcCollection[curEnd],
                                                           nd.arcCollection[curStart], nd.name);
                            	
                            	if ((numbFixedWith >= 0) && (recPath.lines.Count > 0) &&
                            	    (recPath.lines.FindIndex(el => (el.type != Material.na_ && el.type != Material.pa_)) >= 0) )
                            	{
                            		currentConnection.Add(numbFixedWith);
                            	}
                        		curTrace.AddTrace(recPath);
                            }
                            currentConnection.Add(curEnd);//!!!!!!!                            
                        }
        			}
    			}
    		}
    		foreach(int curEnd in fixedConnection)
            	curTrace.AddTrace(RecoveryFixedPath(nd.arcCollection[curEnd], nd.name));
    		
    		SetUsed(curTrace);
        }
        /*
        private NodeTraces GetNodeTrace_old(Node nd, System.IO.StreamWriter file)//better
        {
        	NodeTraces allNodeTrace = new NodeTraces();
        	
    		List<int> currentConnection = new List<int>();
    		for (int i = 0; i < nd.arcCollection.Count; i++)
    		{
    			if (currentConnection.FindIndex(el => el == i) < 0)
    			{
        			List<int> curList = nodeDistanceDict[nd.name].GetConnectedPoints(i);
        			int curStart = curList.Min();
        			
        			
        			if (nd.name == "VCC")
        			{
        				//PrintUsed(0, file);
        				PrintNumb(0, file);
        			}

        			foreach (int curEnd in curList)
        			{
                        if (curStart != curEnd)
                        {
                        	NodeTraces curTrace = new NodeTraces();
                        	
                            int numbFixedWith = nodeDistanceDict[nd.name].GetFixedPointTo(curEnd);
                            if ((numbFixedWith >= 0) && (currentConnection.FindIndex(el => el == numbFixedWith) >= 0))
                                curTrace.AddTrace(RecoveryFixedPath(nd.arcCollection[curEnd], nd.name));
                            else
                                curTrace.AddTrace(RecoveryPath(nd.arcCollection[curEnd],
                                                           nd.arcCollection[curStart], nd.name));
                            currentConnection.Add(curEnd);
                            
                            //SetUsed(curTrace);
                            
                            //ReplaceUnused procRemove = new ReplaceUnused(0, GetPoint(startPoint).priority, false);
        					//SpreadWaveProcess(startPoint, procRemove);
                            
                            allNodeTrace.AddTrace(curTrace);
                        }
        			}
        			
        			if (nd.name == "VCC")
        			{
        				PrintUsed(0, file);
        				//PrintNumb(0, file);
        			}
    			}
    		}
    		return allNodeTrace;
        }*/
        
        private NodeTraces GetNodeTrace(Node nd, System.IO.StreamWriter file)
        {
        	SetAllUnused();
        	
        	NodeTraces allNodeTrace = new NodeTraces();
        	//NodeTraces extraNodeTrace = new NodeTraces();
        	
        	List<ContactSimple> inters = new List<ContactSimple>();
        	inters.Add(nd.arcCollection[0]);
        	//List<LineStruct> lines = new List<LineStruct>();
        	//if (diffusionException.FindIndex(nameUnit => nameUnit == nd.name) >= 0)
        	//	allNodeTrace.lines.Add( GetFixedLine(inters, inters[0].layer, nd.name) );
        	
    		List<int> currentConnection = new List<int>();
    		for (int i = 0; i < nd.arcCollection.Count; i++)
    		{
    			
    			if (currentConnection.FindIndex(el => el == i) < 0)
    			{
        			List<int> curList = nodeDistanceDict[nd.name].GetConnectedPoints(i);
        			int curStart = curList.Min();

        			foreach (int curEnd in curList)
        			{
                        if (curStart != curEnd)
                        {
                        	//NodeTraces curTrace = new NodeTraces();
                        	
                            int numbFixedWith = nodeDistanceDict[nd.name].GetFixedPointTo(curEnd);
                            if ((curStart == numbFixedWith) && (diffusionException.FindIndex(nameUnit => nameUnit == nd.name) >= 0))
                            	allNodeTrace.AddTrace(RecoveryFixedPath(nd.arcCollection[curEnd], nd.name));
                            else
                            	allNodeTrace.AddTrace(RecoveryUnicPath(allNodeTrace.lines, nd.arcCollection[curEnd],
                                                           nd.arcCollection[curStart], nd.name));
                            currentConnection.Add(curEnd);
                            
                            //allNodeTrace.AddTrace(curTrace);
                        }
        			}
    			}
    		}
    		
    		//allNodeTrace.DefineCrossContacts(this);
    		//allNodeTrace.AddTrace(extraNodeTrace);
    		
    		return allNodeTrace;
        }
        

        /*private void SpreadUsedNode(Node nd)
        {
            
    		List<int> currentConnection = new List<int>();
    		for (int i = 0; i < nd.arcCollection.Count; i++)
    		{
    			if (currentConnection.FindIndex(el => el == i) < 0)
    			{
                    NodeTraces curTrace = new NodeTraces();
                    

                    List<int> curList = nodeDistanceDict[nd.name].GetConnectedPoints(i);
                    int curStart = curList.Min();
                    currentConnection.Add(curStart);

                    List<ContactSimple> startPoints = FindStartPoint(nd.arcCollection[curStart], nd.name);
                    SetContactNumber(startPoints, 0);
                    NodePointProcess proc = new NodePointProcess(GetPoint(startPoints[0]), false);
                    SpreadWaveProcess(startPoints, proc);

                    foreach (int curEnd in curList)
                    {
                    	if (currentConnection.FindIndex(el => el == curEnd) < 0)//(curStart != curEnd)
                        {
                            int numbFixedWith = nodeDistanceDict[nd.name].GetFixedPointTo(curEnd);
                            if ((numbFixedWith >= 0)) //&& (currentConnection.FindIndex(el => el == numbFixedWith) >= 0))
                                curTrace.AddTrace(RecoveryFixedPath(nd.arcCollection[curEnd], nd.name));
                            
                            if (numbFixedWith != curStart)
                            {
                                curTrace.AddTrace(RecoveryUnicPath(curTrace.lines, nd.arcCollection[curEnd],
                                                           nd.arcCollection[curStart], nd.name));
                            	if (numbFixedWith >= 0)
                            		currentConnection.Add(numbFixedWith);
                            }
                            currentConnection.Add(curEnd);
                        }
                    }

                    SetUsed(curTrace);

                    SetContactNumber(startPoints, 0);
                    ReplaceUnused procRemove = new ReplaceUnused(GetPoint(startPoints[0]), false);
                    int count = SpreadWaveProcess(startPoints, procRemove);
                    nodeDistanceDict[nd.name].SetCountForNumber(i, count);
                }
            }
        }*/

        
       
        
        private bool SetUsedInLine(LineStruct inLine)
        {
        	bool firstPass = false;
        	foreach (ContactSimple cnt in inLine.GetPointArray())
        	{
        		//if (firstPass && GetPoint(cnt).isUsed)
        		//	return false;
        		if (cnt.layer == Layers.metal1Trace || cnt.layer == Layers.metal2Trace || cnt.layer == Layers.siliconTrace)
        		{
	        		GetPoint(cnt).isUsed = true;
	                GetPoint(cnt).isReplace = false;
        		}
                //firstPass = true;
        	}
        	return true;
        }

        private LineStruct GetFixedLine(List<ContactSimple> passedPoints, int layer, string curName)
        {
        	ContactSimple curPoint = passedPoints.Last();
            int x = curPoint.x;
            int y = curPoint.y;
            int iUp, iDown, iRight, iLeft;
            int d = 0;//layoutMap[x][y][layer].number;
            for (iUp = y + 1; iUp < Params.topEdge; iUp++)
            {
                if ((layoutMap[x][iUp][layer].isFixed) &&
                    (layoutMap[x][iUp][layer].name == curName))
                    d--;
                else
                {
                    break;
                }
            }
            for (iDown = y - 1; iDown >= 0; iDown--)
            {
                if ((layoutMap[x][iDown][layer].isFixed) &&
                    (layoutMap[x][iDown][layer].name == curName))
                    d--;
                else
                    break;
            }
            for (iRight = x + 1; iRight < wide; iRight++)
            {
                if ((layoutMap[iRight][y][layer].isFixed) &&
                    (layoutMap[iRight][y][layer].name == curName))
                    d--;
                else
                    break;
            }
            for (iLeft = x - 1; iLeft >= 0; iLeft--)
            {
                if ((layoutMap[iLeft][y][layer].isFixed) &&
                    (layoutMap[iLeft][y][layer].name == curName))
                    d--;
                else
                    break;
            }

            /*string typeLine = Params.silType;
        	if (layer == Layers.metal1Trace)
        		typeLine = Params.met1Type;
        	else if (layer == Layers.metal2Trace)
        		typeLine = Params.met2Type;*/

            List<LineStruct> lines = new List<LineStruct>();
            lines.Add(new LineStruct(new PairInt(x, y), new PairInt(x, iDown + 1), layer));
            lines.Add(new LineStruct(new PairInt(x, y), new PairInt(x, iUp - 1), layer));
            lines.Add(new LineStruct(new PairInt(x, y), new PairInt(iRight - 1, y), layer));
            lines.Add(new LineStruct(new PairInt(x, y), new PairInt(iLeft + 1, y), layer));
            LineStruct longest = new LineStruct(new PairInt(x, y), new PairInt(x, y), layer);//lines[0];
            foreach (LineStruct ln in lines)
            	if ((ln.Length() > longest.Length()) && 
            	    (passedPoints.FindIndex(el => (PairInt)el == (PairInt)ln.OpositePoint(curPoint)) < 0))
                    longest = ln;
            return longest;
        }
        
        private List<ContactSimple> GetFixedPoints(ContactSimple inPoint)
        {
        	List<ContactSimple> points = new List<ContactSimple>();
        	points.Add(inPoint);
        	string curName = GetPoint(inPoint).name;
        	
        	int layer = inPoint.layer;
        	int x = inPoint.x;
            int y = inPoint.y;
            int iUp, iDown, iRight, iLeft;
        	for (iUp = y + 1; iUp < Params.topEdge; iUp++)
            {
                if ((layoutMap[x][iUp][layer].isFixed) &&
                    (layoutMap[x][iUp][layer].name == curName))
        		{
        			points.Add(new ContactSimple(x, iUp, layer));
        		}
                else
                {
                    break;
                }
            }
            for (iDown = y - 1; iDown >= 0; iDown--)
            {
                if ((layoutMap[x][iDown][layer].isFixed) &&
                    (layoutMap[x][iDown][layer].name == curName))
            	{
            		points.Add(new ContactSimple(x, iDown, layer));
            	}
                else
                    break;
            }
            for (iRight = x + 1; iRight < wide; iRight++)
            {
                if ((layoutMap[iRight][y][layer].isFixed) &&
                    (layoutMap[iRight][y][layer].name == curName))
            	{
            		points.Add(new ContactSimple(iRight, y, layer));
            	}
                else
                    break;
            }
            for (iLeft = x - 1; iLeft >= 0; iLeft--)
            {
                if ((layoutMap[iLeft][y][layer].isFixed) &&
                    (layoutMap[iLeft][y][layer].name == curName))
            	{
            		points.Add(new ContactSimple(iLeft, y, layer));
            	}
                else
                    break;
            }
            return points;
        }

        private LineStruct GetLineVertical_old(PairInt curPoint, int layer, string curName)
        {
        	int x = curPoint.x;
        	int y = curPoint.y;
        	//string curName = layoutMap[x][y][layer].name;
        	//NodePoint np = layoutMap[x][y][layer];
        	
        	int iUp, iDown;
    		int d = layoutMap[x][y][layer].number;
    		for (iUp = y + 1; iUp < Params.topEdge; iUp++)
    		{
    			if ( (layoutMap[x][iUp][layer].number == (d - 1)) &&
    			    (layoutMap[x][iUp][layer].name == curName) )
    				d--;
    			else
    			{
    				//np = layoutMap[x][iUp][layer];
    				break;
    			}
    		}
    		for (iDown = y - 1; iDown >= 0; iDown--)
    		{
    			if ( (layoutMap[x][iDown][layer].number == (d - 1)) &&
    			    (layoutMap[x][iDown][layer].name == curName) )
    				d--;
    			else
    				break;
    		}
        	
        	int heightSeg = y - iDown - 1;
        	int startY = y;
        	if ((y - iDown) > (iUp - y))
        		startY = iDown + 1;
        	else
        		heightSeg = iUp - y - 1;
        	//LineStruct retLine = new LineStruct(x, 
        	//Segment retSegment = new Segment(x, startY, 0, heightSeg);
        	
        	/*string typeLine = Params.silType;
        	if (layer == Layers.metal1Trace)
        		typeLine = Params.met1Type;
        	else if (layer == Layers.metal2Trace)
        		typeLine = Params.met2Type;*/
        	
        	LineStruct retLineStr = new LineStruct(new PairInt(x, startY), 
        	                                       new PairInt(x, startY + heightSeg), layer);
        	//retLineStr.line = retSegment;
        	return retLineStr;
        }
        
        private LineStruct GetLineVertical(PairInt curPoint, int layer, string curName)
        {
        	int x = curPoint.x;
        	int y = curPoint.y;
        	
        	//bool upUsed = false, downUsed = false;
        	int iUp, iDown;
    		int d = layoutMap[x][y][layer].number;
    		for (iUp = y + 1; iUp < Params.topEdge; iUp++)
    		{
    			if ( (layoutMap[x][iUp][layer].number == (d - 1)) &&
    			    (layoutMap[x][iUp][layer].name == curName) )
    				d--;
    			else
    			{
    				break;
    			}
    		}
    		for (iDown = y - 1; iDown >= 0; iDown--)
    		{
    			if ( (layoutMap[x][iDown][layer].number == (d - 1)) &&
    			    (layoutMap[x][iDown][layer].name == curName) )
    				d--;
    			else
    				break;
    		}
        	
        	int heightSeg = iDown - y + 1;
        	int startY = y;
        	if ( ((y - iDown) < (iUp - y)) )//|| ((y - iDown) < 0) )
        		heightSeg = iUp - y - 1;
        		//startY = iDown + 1;
        	//else
        		
        	
        	/*string typeLine = Params.silType;
        	if (layer == Layers.metal1Trace)
        		typeLine = Params.met1Type;
        	else if (layer == Layers.metal2Trace)
        		typeLine = Params.met2Type;*/
        	
        	LineStruct retLineStr = new LineStruct(new PairInt(x, startY), 
        	                                       new PairInt(x, startY + heightSeg), layer);
        	
        	//if (retLineStr.Y < 0)
        	//	return retLineStr;
        	return retLineStr;
        }
            	
		private LineStruct GetLineVertical(List<LineStruct> curLines, PairInt curPoint, int layer, string curName)
        {
        	int x = curPoint.x;
        	int y = curPoint.y;
        	
        	bool nextCheck = true;// downUsed = false;
        	int iUp, iDown;
    		int d = layoutMap[x][y][layer].number;
    		for (iUp = y + 1; (iUp < Params.topEdge) && nextCheck; iUp++)
    		{
    			if ( (layoutMap[x][iUp][layer].number == (d - 1)) &&
    			    (layoutMap[x][iUp][layer].name == curName) )
    				d--;
    			else
    			{
    				break;
    			}
    			
    			foreach (LineStruct ln in curLines)
    				if (ln.OverlapWithPoint(x, iUp, layer))
    					nextCheck = false;
    		}
    		nextCheck = true;
    		for (iDown = y - 1; (iDown >= 0) && nextCheck; iDown--)
    		{
    			if ( (layoutMap[x][iDown][layer].number == (d - 1)) &&
    			    (layoutMap[x][iDown][layer].name == curName) )
    				d--;
    			else
    				break;
    			
    			foreach (LineStruct ln in curLines)
    				if (ln.OverlapWithPoint(x, iDown, layer))
    					nextCheck = false;
    		}
        	
        	int heightSeg = iDown - y + 1;
        	int startY = y;
        	if ( ((y - iDown) < (iUp - y)) )//|| ((y - iDown) < 0) )
        		heightSeg = iUp - y - 1;
        		
        	        	
        	LineStruct retLineStr = new LineStruct(new PairInt(x, startY), 
        	                                       new PairInt(x, startY + heightSeg), layer);
        	
        	return retLineStr;
        }
        
        private LineStruct GetLineHorizont(List<LineStruct> curLines, PairInt curPoint, int layer, string curName)
        {
        	int x = curPoint.x;
        	int y = curPoint.y;
        	//string curName = layoutMap[x][y][layer].name;
        	
        	bool nextCheck = true;
        	int iRight, iLeft;
        	
    		int d = layoutMap[x][y][layer].number;
    		for (iRight = x + 1; (iRight < wide) && nextCheck; iRight++)
    		{
    			if ( (layoutMap[iRight][y][layer].number == (d - 1)) &&
    			    (layoutMap[iRight][y][layer].name == curName) )
    				d--;
    			else
    				break;
    			
    			foreach (LineStruct ln in curLines)
    				if (ln.OverlapWithPoint(iRight, y, layer))
    					nextCheck = false;
    		}
    		nextCheck = true;
    		for (iLeft = x - 1; (iLeft >= 0) && nextCheck; iLeft--)
    		{
    			if ( (layoutMap[iLeft][y][layer].number == (d - 1)) &&
    			    (layoutMap[iLeft][y][layer].name == curName) )
    				d--;
    			else
    				break;
    			
    			foreach (LineStruct ln in curLines)
    				if (ln.OverlapWithPoint(iLeft, y, layer))
    					nextCheck = false;
    		}
        	
        	int wideSeg = iLeft - x + 1;
        	int startX = x;
        	if ((x - iLeft) < (iRight - x))
        		wideSeg = iRight - x - 1;
        	
        	
        	LineStruct retLineStr = new LineStruct(new PairInt(startX, y), 
        	                                       new PairInt(startX + wideSeg, y), layer);
        	
        	return retLineStr;
        }
        
        private LineStruct GetLineHorizont(PairInt curPoint, int layer, string curName)
        {
        	int x = curPoint.x;
        	int y = curPoint.y;
        	//string curName = layoutMap[x][y][layer].name;
        	
        	int iRight, iLeft;
        	
    		int d = layoutMap[x][y][layer].number;
    		for (iRight = x + 1; iRight < wide; iRight++)
    		{
    			if ( (layoutMap[iRight][y][layer].number == (d - 1)) &&
    			    (layoutMap[iRight][y][layer].name == curName) )
    				d--;
    			else
    				break;
    		}
    		for (iLeft = x - 1; iLeft >= 0; iLeft--)
    		{
    			if ( (layoutMap[iLeft][y][layer].number == (d - 1)) &&
    			    (layoutMap[iLeft][y][layer].name == curName) )
    				d--;
    			else
    				break;
    		}
        	
        	int wideSeg = iLeft - x + 1;
        	int startX = x;
        	if ((x - iLeft) < (iRight - x))
        		wideSeg = iRight - x - 1;
        	
        	
        	LineStruct retLineStr = new LineStruct(new PairInt(startX, y), 
        	                                       new PairInt(startX + wideSeg, y), layer);
        	
        	return retLineStr;
        }
        
        private LineStruct GetLineHorizont_bad(PairInt curPoint, int layer, string curName)
        {
        	int x = curPoint.x;
        	int y = curPoint.y;
        	
        	bool rightUsed = false, leftUsed = false;
        	int iRight, iLeft;
        	
    		int d = layoutMap[x][y][layer].number;
    		for (iRight = x + 1; iRight < wide; iRight++)
    		{
    			if (layoutMap[iRight][y][layer].isUsed)
    			{
    				rightUsed = true;
    				break;
    			}
    			if ( (layoutMap[iRight][y][layer].number == (d - 1)) &&
    			    (layoutMap[iRight][y][layer].name == curName) )
    				d--;
    			else
    				break;
    		}
    		for (iLeft = x - 1; iLeft >= 0; iLeft--)
    		{
    			if (layoutMap[iLeft][y][layer].isUsed)
    			{
    				leftUsed = true;
    				break;
    			}
    			if ( (layoutMap[iLeft][y][layer].number == (d - 1)) &&
    			    (layoutMap[iLeft][y][layer].name == curName) )
    				d--;
    			else
    				break;
    		}
        	
        	int wideSeg = x - iLeft + 1;
        	int startX = x;
        	if ( ((x - iLeft) > (iRight - x)) || (leftUsed && !rightUsed) )
        		startX = iLeft + 1;
        	else
        		wideSeg = iRight - x - 1;
        	
        	/*string typeLine = Params.silType;
        	if (layer == Layers.metal1Trace)
        		typeLine = Params.met1Type;
        	else if (layer == Layers.metal2Trace)
        		typeLine = Params.met2Type;*/
        	
        	LineStruct retLineStr = new LineStruct(new PairInt(startX, y), 
        	                                       new PairInt(startX + wideSeg, y), layer);
        	
        	return retLineStr;
        }
    }
}