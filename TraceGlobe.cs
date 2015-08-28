/*
 * Created by SharpDevelop.
 * User: frolkinak
 * Date: 30.04.2014
 * Time: 18:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace eulerMake
{
	/// <summary>
	/// Description of TraceGlobe.
	/// </summary>
	public partial class TraceGlobe
	{
		private int wide;
		
		private NodePoint[][][] layoutMap;
        
        private static NodePoint blank;
        private static NodePoint full;
        private static NodePoint diffusion;
        
        
        //private bool isTraceH;
        private int numberH;
        private int criticAmountH;
        private List<string> exceptionH;
        private string bestHighPrior;
        private string prevHighPrior;
        private int stepWithMax;
        private ConflictPolitic conflictManager;
        private List<string> diffusionException;
        
        private List<Node> nodeList;
        private Dictionary<string, NodeDistanceBase> nodeDistanceDict;
        private int countMaxPrior;
        
		public TraceGlobe()
		{
			blank = new NodePoint("blank");
			blank.isReplace = true;
			full = new NodePoint("full");
			diffusion = new NodePoint("diffusion");
			//pathSpred = new List<NodePaths>();
			//isTraceH = false;
			stepWithMax = 0;
            conflictManager = new ConflictPolitic();
            diffusionException = new List<string>();
		}
		
		
		public void PropagateAll()
        {
			countMaxPrior = 0;
			int yStart = 0;
            for (int x = 0; x < wide; x++)
            {
                for (int y = yStart; y < Params.topEdge; y += 2)
                {
                	PairInt coord = new PairInt(x, y);
                	SetPoint(coord);
                	
                	if (GetPoint(coord, Layers.metal1Trace).priority == 99)
                		countMaxPrior++;
                	if (GetPoint(coord, Layers.siliconTrace).priority == 99)
                		countMaxPrior++;
                }
                if (yStart == 0)
                	yStart = 1;
                else
                	yStart = 0;
            }
            
            yStart = 1;
            for (int x = 0; x < wide; x++)
            {
                for (int y = yStart; y < Params.topEdge; y += 2)
                {
                	PairInt coord = new PairInt(x, y);
                	SetPoint(coord);
                	
                	if (GetPoint(coord, Layers.metal1Trace).priority == 99)
                		countMaxPrior++;
                	if (GetPoint(coord, Layers.siliconTrace).priority == 99)
                		countMaxPrior++;
                }
                if (yStart == 0)
                	yStart = 1;
                else
                	yStart = 0;
            }
        }
		
		public void PropagateAllNotCross()
        {
			countMaxPrior = 0;
			int yStart = 0;
            for (int x = 0; x < wide; x++)
            {
                for (int y = yStart; y < Params.topEdge; y += 2)
                {
                	PairInt coord = new PairInt(x, y);
                	SetPointNotCross(coord);
                	
                }
                if (yStart == 0)
                	yStart = 1;
                else
                	yStart = 0;
            }
            
            yStart = 1;
            for (int x = 0; x < wide; x++)
            {
                for (int y = yStart; y < Params.topEdge; y += 2)
                {
                	PairInt coord = new PairInt(x, y);
                	SetPointNotCross(coord);
                	
                }
                if (yStart == 0)
                	yStart = 1;
                else
                	yStart = 0;
            }
        }
		
		public bool IsEnd()
		{
			return nodeDistanceDict[conflictManager.GetBestPriority()].IsTraced();
		}
		
		public void SetHighPriority(string inName)
		{
			//bestHighPrior = inName;
            conflictManager.SetBestPriority(inName);
		}

        public void SetReplace()
        {
            for (int x = 0; x < wide; x++)
            {
                for (int y = 0; y < Params.topEdge; y++)
                {
                    ContactSimple cntUnit = new ContactSimple(x, y, Layers.metal1Trace);
                    //if (CheckNebors(cntUnit))
                    layoutMap[x][y][Layers.metal1Trace].isReplace = CheckNebors(cntUnit);

                    cntUnit = new ContactSimple(x, y, Layers.siliconTrace);
                    //if (CheckNebors(cntUnit))
                    layoutMap[x][y][Layers.siliconTrace].isReplace = CheckNebors(cntUnit);
                }
            }
        }
        
        private bool CheckConnction()
        {
        	//CheckOnePoint();
        	bool isConnected = false;
        	for (int x = 0; x < wide; x++)
            {
                for (int y = 0; y < Params.topEdge; y++)
                {
                	foreach (int lay in Params.UsedLayers)
                	{
                		if (UniteWaves(new ContactSimple(x, y, lay)))
                        	isConnected = true;
                	}
                	/*if (UniteWaves(new ContactSimple(x, y, Layers.metal1Trace)))
                        isConnected = true;
                    if (UniteWaves(new ContactSimple(x, y, Layers.siliconTrace)))
                        isConnected = true;*/
                }
        	}
        	return isConnected;
        }
        
        public void MergeNodes(bool isPriority, System.IO.StreamWriter file)
        {
        	//CheckOnePoint();
        	bool isConnected = CheckConnction();
            
            if (isConnected && isPriority)
                SetPriority();
            
            if (conflictManager.IsNeedReconfig())
            {
                //ClearVccGnd();

                if (nodeDistanceDict[conflictManager.GetBestPriority()].IsTraced())//bestHighPrior
                    return;
                
                 if (!isConnected)
                	SetPriority();
                
                 CheckOnePoint();
                /*PrintMap(0, file);
                PrintMap(1, file);
                PrintNumb(999, file);*/
                /*file.WriteLine("--------------BEFOR CLEAR-------------");
                PrintMap(0, file);
                PrintMap(1, file);/*
                PrintNumb(999, file);
                 */
                CheckOnePoint();
                ClearTracedPath(file);
                CheckOnePoint();
                
                /*file.WriteLine("--------------CLEAR-------------");
                PrintMap(0, file);
                PrintMap(1, file);/*
                PrintNumb(999, file);*/
                
                SetUntracedNumbers();
                SetAllUnused();

                /*file.WriteLine("--------------CHECK m1-------------");
                PrintMap(1, file);
                PrintMap(0, file);
                PrintNumb(999, file);*/
                
                CheckOnePoint();
                if (!SearchNearest())
                {
                    conflictManager.SetWithoutBestPriority();
                    
                    /*file.WriteLine("--------------CHECK m1  again-------------");
                    PrintMap(1, file);
                    PrintMap(0, file);
                    PrintNumb(999, file);*/
                    
                    CheckOnePoint();
                    isConnected = CheckConnction();
                    
                    SetAllSamePriority();
                    
                    
                    CheckOnePoint();
                    
                    SpreadAllWaves();

                    
                    
                    ClearForAlign(file);
                    
                    

                    CheckOnePoint();
                    
                    PropagateAll();
                    CheckOnePoint();
                    
                    CheckConnction();
                    //if (!isConnected)
                    	PropagateAll();
                    //CheckConnction();
                    //if (!isConnected)
                    	PropagateAll();
                    CheckConnction();
                    
                    //SetPriority();
                    conflictManager.SetWithPriority();
                    
                    /*file.WriteLine("--------------CHECK MAP  again-------------");
                    PrintMap(1, file);
                    PrintMap(0, file);*/
                }
                else
                	 CheckConnction();
                
                SetPriorityWithException();

                PrintRule(file);
            }
            
            CheckOnePoint();
            
            if (isPriority)
            	conflictManager.IncrementStep();
        }
        
        public bool CheckOnePoint()
        {
        	bool b = false;
        	NodePoint pn = GetPoint(new ContactSimple(27, 33, 1));
        	NodePoint pn2 = GetPoint(new ContactSimple(27, 37, 1));
        	if ( pn.name != "N6864112" && pn2.name != "N6864112")//"N6859613" )//!= "N6861113" )//&& pn.name != Material.blankName)
				b = true;
        	return b;
        }
        
        /*
        private void ClearVccGnd()
        {
        	Node curNode = nodeList.Find(el => el .name == conflictManager.GetBestPriority());
        	int curX = curNode.arcCollection[conflictManager.GetBlocadedNumber()].x;
        	int curY = curNode.arcCollection[conflictManager.GetBlocadedNumber()].y;
        	List<string> namesException = new List<string>();
        	List<Contact> contactsException = new List<Contact>();
        	
        	foreach (Node nd in nodeList)
        	{
        		if (diffusionExeption.FindIndex(el => el == nd.name) >= 0)
        		{
	        		foreach (Contact cnt in nd.arcCollection)
	        		{
	        			if ( (Math.Abs(cnt.x -  curX) <= 2) && (cnt.y == curY) )
	        			{
	        				contactsException.Add(cnt);
	        				namesException.Add(nd.name);
	        			}
	        		}
        		}
        	}
        	
        	for (int i = 0; i < contactsException.Count; i++)
        	{
        		List<Contact> sources = GetSourceContacts(contactsException[i], namesException[i]);
        	}
        	
        }*/
        
        //public void TrySet
        
        
        
        public void SetAllSamePriority()
        {
        	foreach (NodeDistanceBase disBase in nodeDistanceDict.Values)
            {
        		Node processNode = nodeList.Find(element => element.name == disBase.name);

                List<int> idxPass = new List<int>();
        		for (int i = 0; i < processNode.arcCollection.Count; i++)
                {
            		int idx = nodeDistanceDict[disBase.name].GetNumber(i);
            		ContactSimple cnt = processNode.arcCollection[i];
            		
            		if ( idxPass.FindIndex(el => el == idx) < 0 )
            		{
            			idxPass.Add(idx);
            			List<ContactSimple> startWave = FindStartContacts(new ContactSimple(
            				processNode.arcCollection[idx]), processNode.name);
            			
            			SetContact(startWave, processNode.name, idx, 1, 0);
            			NodePointProcess proc = new NodePointProcess(GetPoint(startWave[0]), false);
            			int count = CompleteSpreadWaveProcess(startWave, proc);
                        disBase.SetCountForNumber(i, count);
            			
            		}
            		CheckOnePoint();
                }
        	}
        }
        
        public void SetPriority()
        {
            List<NodeDistanceBase> lst = nodeDistanceDict.Values.ToList();
            
            lst.Sort(NodeDistanceBase.CompareBaseByDist);
            int countNd = lst.Count;
            
            conflictManager.SetBestPriority(lst.First().name);
            
            int curPrior = Params.maxPriority;

            foreach (NodeDistanceBase disBase in lst)
            {
            	Node processNode = nodeList.Find(element => element.name == disBase.name);
            	            	
            	/*List<int> priorites = nodeDistanceDict[processNode.name].GetPriorites(processNode.arcCollection.Count, curPrior);
                if (priorites.Count > 0)
            	    curPrior = priorites.Last() - 1;
                else*/
                	//curPrior--;
                
                List<int> idxPass = new List<int>();
            	for (int i = 0; i < processNode.arcCollection.Count; i++)
                {
            		int idx = nodeDistanceDict[disBase.name].GetNumber(i);
            		ContactSimple cnt = processNode.arcCollection[i];
            		
            		if ( idxPass.FindIndex(el => el == idx) < 0 )
            		{
            			idxPass.Add(idx);
            			List<ContactSimple> startWave = FindStartPoint(new ContactSimple(
            				processNode.arcCollection[idx]), processNode.name);
            			SetContact(startWave, processNode.name, idx, curPrior, 0);
            			NodePointProcess proc = new NodePointProcess(GetPoint(startWave[0]), false);
            			int count = CompleteSpreadWaveProcess(startWave, proc);//SpreadWaveProcess
                        disBase.SetCountForNumber(i, count);
            		}
                }
                curPrior--;
            }
        }

        private void SetPriorityWithException()
        {
            if (!conflictManager.IsBestPriorityUsed())
            {
            	SetPriority();
                return;
            }

            List<NodeDistanceBase> lst = nodeDistanceDict.Values.ToList();

            NodeDistanceBase.exceptionNames = conflictManager.GetExceptions();
            NodeDistanceBase.bestName = conflictManager.GetBestPriority();

            //lst.Sort(NodeDistanceBase.CompareExceptBase);
            lst = BoobleSortExcept(lst);
            int countNd = lst.Count;

            conflictManager.SetBestPriority(lst.First().name);

            int curPrior = Params.maxPriority;

            foreach (NodeDistanceBase disBase in lst)
            {
                Node processNode = nodeList.Find(element => element.name == disBase.name);

                /*List<int> priorites = nodeDistanceDict[processNode.name].GetPriorites(processNode.arcCollection.Count, curPrior);
                if (priorites.Count > 0)
                    curPrior = priorites.Last() - 1;
                else*/
                    

                List<int> idxPass = new List<int>();
                for (int i = 0; i < processNode.arcCollection.Count; i++)
                {
                    int idx = nodeDistanceDict[disBase.name].GetNumber(i);
                    ContactSimple cnt = processNode.arcCollection[i];

                    if (idxPass.FindIndex(el => el == idx) < 0)
                    {
                        idxPass.Add(idx);
                        List<ContactSimple> startWave = FindStartPoint(new ContactSimple(
                            processNode.arcCollection[idx]), processNode.name);
                        SetContact(startWave, processNode.name, idx, curPrior, 0);
                        NodePointProcess proc = new NodePointProcess(GetPoint(startWave[0]), false);
                        int count = CompleteSpreadWaveProcess(startWave, proc);//SpreadWaveProcess
                        disBase.SetCountForNumber(i, count);
                    }
                }
                curPrior--;
            }
        }

        private void SetUntracedNumbers()
        {
            foreach (NodeDistanceBase disBase in nodeDistanceDict.Values)
            {
                if (!disBase.IsTraced())
                {
                    Node curNode = nodeList.Find(el => el.name == disBase.name);
                    foreach (int curNumber in disBase.GetNodeNumbers(curNode.arcCollection.Count()))
                    {
                        List<ContactSimple> startPoint = FindStartPoint(curNode.arcCollection[curNumber], disBase.name);
                        SetContactNumber(startPoint, 0);
                        NodePointProcess proc = new NodePointProcess(GetPoint(startPoint[0]), false);
                        int count = CompleteSpreadWaveProcess(startPoint, proc);//SpreadWaveProcess
                        disBase.SetCountForNumber(curNumber, count);
                    }
                }
            }
        }
        
        private void ClearTracedPath(System.IO.StreamWriter file)
        {
        	List<NodeDistanceBase> lst = nodeDistanceDict.Values.ToList();
        	lst.Sort(NodeDistanceBase.CompareBaseByDist);
        	
        	
        	
        	foreach (NodeDistanceBase disBase in lst)
            {
        		if (disBase.IsTraced())
        		{
        			//bool b;
        			//if (disBase.name  == "INC")
        			//	b = true;
        			/*if (AddingSpace(disBase.name) == "61113")
        			{
        				b = false;
        				PrintMap(Layers.metal1Trace, file);
        				PrintMap(Layers.siliconTrace, file);
        				PrintMap(Layers.metal2Trace, file);
        			}*/
        			CheckOnePoint();
        			
        			Node curNode = nodeList.Find(el => el.name == disBase.name);
        			List<ContactSimple> startPoint = FindStartPoint(curNode.arcCollection[0], disBase.name);
        			//GetPoint(startPoint).number = 0;
        			SetContactNumber(startPoint, 0);
        			NodePointProcess proc = new NodePointProcess(GetPoint(startPoint[0]), false);
        			CompleteSpreadWaveProcess(startPoint, proc);
        			
        			CheckOnePoint();
        			SpreadUsed(curNode);
        			
        			/*if (disBase.name  == "N6861401")//"N6859613")//"N6861401")
        			{
        				file.WriteLine("!!!-----------check numb N6861401-------------------");
        				PrintMap(Layers.metal1Trace, file);
						PrintMap(Layers.siliconTrace, file);
			        	PrintNumb(-2, file);
			        	PrintUsed(-2, file);
			        	file.WriteLine("!!!-----------end check numb N6861401-------------------");
        			}
        			//
        			if (disBase.name  == "INY")
        			{
        				file.WriteLine("!!!-----------check used INY-------------------");
        				PrintMap(Layers.metal1Trace, file);
						PrintMap(Layers.siliconTrace, file);
			        	PrintUsed(-2, file);
			        	file.WriteLine("!!!-----------end used INY-------------------");
        			}*/
        			CheckOnePoint();
        			
        			ReplaceUnused procRemove = new ReplaceUnused(GetPoint(startPoint[0]), false);
        			CompleteSpreadWaveProcessRem(startPoint, procRemove);
        			
        			CheckOnePoint();
        		}
        	}
        	
        	
        	
        	//inFile.WriteLine("");
        }
        
        private List<NodeDistanceBase> BoobleSortExcept(List<NodeDistanceBase> inLst)
        {
        	for (int i = 0; i < inLst.Count; i++)
        	{
        		for (int j = i + 1; j < inLst.Count; j++)
        		{
        			if (NodeDistanceBase.CompareExceptBase(inLst[j], inLst[i]) < 0)
	        		{
        				NodeDistanceBase nd = new NodeDistanceBase(inLst[j]);
        				inLst[j] = new NodeDistanceBase(inLst[i]);
        				inLst[i] = nd;
	        		}
        		}
        	}
        	return inLst;
        }
        
        public int GetUntracedNodes(System.IO.StreamWriter inFile)
        {
        	int count = 0;
        	List<NodeDistanceBase> lst = nodeDistanceDict.Values.ToList();
        	lst.Sort(NodeDistanceBase.CompareBaseByDist);
        	List<string> notTraced = new List<string>();
        	
        	inFile.WriteLine("");
        	foreach (NodeDistanceBase disBase in lst)
            {
        		if (!disBase.IsTraced())
        		{
        			count++;
        			//notTraced.Add(disBase.name);
        			inFile.Write(disBase.name + " ");
        		}
        		/*List<int> idxPass = new List<int>();
        		for (int i = 0; i < processNode.arcCollection.Count; i++)
                {
        			int idx = nodeDistanceDict[disBase.name].GetNumber(i);
        		}*/
        	}
        	//foreach (string curName)
        	return count;
        }
        
        public void SpreadAllWaves()
        {
        	foreach (Node curNode in nodeList)
        	{
        		List<int> idxPass = new List<int>();
	        	for (int i = 0; i < curNode.arcCollection.Count; i++)
	            {
	        		int idx = nodeDistanceDict[curNode.name].GetNumber(i);
	        		//Contact cnt = processNode.arcCollection[i];
	        		
	        		if ( idxPass.FindIndex(el => el == idx) < 0 )
	        		{
	        			idxPass.Add(idx);
	        			List<ContactSimple> startWave = FindStartContacts(new ContactSimple(
	        				curNode.arcCollection[idx]), curNode.name);
	        			
	        			SetContactNumber(startWave, 0);
	        			NodePointProcess proc = new NodePointProcess(GetPoint(startWave[0]), false);
	        			int count = CompleteSpreadWaveProcess(startWave, proc);
	        			nodeDistanceDict[curNode.name].SetCountForNumber(idx, count);
	        		}
	            }
        	}
        }
        
        private bool UniteWaves(ContactSimple inCnt)
        {
        	bool findMerge = false;
        	
        	/*if (inCnt.x == 31 && (inCnt.y == 21 || inCnt.y == 22) && inCnt.layer == 1)
        	{
        		NodePoint pn1 = GetPoint (new ContactSimple(31, 21, 1));
        		NodePoint pn2 = GetPoint (new ContactSimple(31, 22, 1));
        		findMerge = false;
        	}*/
        		
        	NodePoint inPoint = GetPoint(inCnt);
        	if (inPoint.name == Material.blankName)
        		return false;
        	int numb1 = -1, ndNum = -1;
        	if (inCnt.x < (wide - 1) && (GetPoint(inCnt).name == layoutMap[inCnt.x + 1][inCnt.y][inCnt.layer].name) &&
                (GetPoint(inCnt).numberNode != layoutMap[inCnt.x + 1][inCnt.y][inCnt.layer].numberNode))
            {
        		numb1 = GetPoint(inCnt).numberNode;
                ndNum = layoutMap[inCnt.x + 1][inCnt.y][inCnt.layer].numberNode;
                nodeDistanceDict[GetPoint(inCnt).name].UniteNodes(GetPoint(inCnt).numberNode, ndNum);
                findMerge = true;
            }
        	if (inCnt.y < (Params.topEdge - 1) && (GetPoint(inCnt).name == layoutMap[inCnt.x][inCnt.y + 1][inCnt.layer].name) &&
                (GetPoint(inCnt).numberNode != layoutMap[inCnt.x][inCnt.y + 1][inCnt.layer].numberNode))
            {
        		numb1 = GetPoint(inCnt).numberNode;
                ndNum = layoutMap[inCnt.x][inCnt.y + 1][inCnt.layer].numberNode;
                nodeDistanceDict[GetPoint(inCnt).name].UniteNodes(GetPoint(inCnt).numberNode, ndNum);
                findMerge = true;
            }
        	if ( inPoint.isSource && (diffusionException.FindIndex(el => el == inPoint.name) < 0) )//!!!
                //(GetPoint(inCnt).numberNode != layoutMap[inCnt.x][inCnt.y + 1][inCnt.layer].numberNode))
            {
        		NodePoint ndPoint = DefineSourcePoint(inCnt, inCnt.layer);
        		if ( (ndPoint.name == inPoint.name) && ( nodeDistanceDict[inPoint.name].GetNumber(inPoint.numberNode) !=
        		                                     nodeDistanceDict[inPoint.name].GetNumber(ndPoint.numberNode) ) )
        		{
	                nodeDistanceDict[inPoint.name].UniteNodes(inPoint.numberNode, ndPoint.numberNode);
	                findMerge = true;
        		}
            }
        	
        	/*int opposLayer = Layers.metal1Trace;
        	if (inCnt.layer == Layers.metal1Trace)
        		opposLayer = Layers.siliconTrace;*/
        	foreach (int opposLayer in Params.GetOppositeLayers(inCnt.layer))
        	{
        		if (inCnt.layer == Layers.siliconTrace || opposLayer == Layers.siliconTrace)
        		{
		        	if ( (GetPoint(inCnt).name == GetPoint(inCnt, opposLayer).name) &&
		                (GetPoint(inCnt, Layers.contactTrace).name != Material.diffusionName) &&
		        	    (GetPoint(inCnt).numberNode != GetPoint(inCnt, opposLayer).numberNode) )
		            {
		        		numb1 = GetPoint(inCnt).numberNode;
		        		ndNum = GetPoint(inCnt, opposLayer).numberNode;
		                nodeDistanceDict[GetPoint(inCnt).name].UniteNodes(GetPoint(inCnt).numberNode, ndNum);
		                findMerge = true;
		            }
        		}
        		else
        		{
        			if ( (GetPoint(inCnt).name == GetPoint(inCnt, opposLayer).name) &&
		        	    (GetPoint(inCnt).numberNode != GetPoint(inCnt, opposLayer).numberNode) )
		            {
		        		numb1 = GetPoint(inCnt).numberNode;
		        		ndNum = GetPoint(inCnt, opposLayer).numberNode;
		                nodeDistanceDict[GetPoint(inCnt).name].UniteNodes(GetPoint(inCnt).numberNode, ndNum);
		                findMerge = true;
		            }
        		}
        	}
        	
        	if (findMerge && GetPoint(inCnt).name == "N6859613")
        		findMerge = true;
        	
        	return findMerge;
        }

        public List<ContactSimple> FindStartPoint(ContactSimple inCnt, string inName)
        {
            List<ContactSimple> startPoints = new List<ContactSimple>();
            
            if ((diffusionException.FindIndex(nameNode => nameNode == inName) >= 0) && GetPoint(inCnt).isFixed)
            {
        		startPoints.AddRange(GetFixedPoints(inCnt));
            	return startPoints;
            }
            foreach (ContactSimple cnt in GetSourceContacts(inCnt, inName))
            {
                if (GetPoint(cnt).name == inName)
                	startPoints.Add(cnt);
            }
            if (startPoints.Count > 0)
            	return startPoints;
            startPoints.Add(new ContactSimple(inCnt.x, (inCnt.y), inCnt.layer));
            
        	return startPoints;
        }
        
        public List<ContactSimple> FindStartContacts(ContactSimple inCnt, string inName)
        {
        	List<ContactSimple> cnts = new List<ContactSimple>();
        	
        	if ((diffusionException.FindIndex(nameNode => nameNode == inName) >= 0) && GetPoint(inCnt).isFixed)
            {
        		cnts.AddRange(GetFixedPoints(inCnt));
            	return cnts;
            }
            foreach (ContactSimple cnt in GetSourceContacts(inCnt, inName))
            {
                if (GetPoint(cnt).name == inName)
                	cnts.Add(cnt);
            }
            
            if (cnts.Count > 0)
            	return cnts;
            
            cnts.Add(new ContactSimple(inCnt.x, (inCnt.y), inCnt.layer));
        	return cnts;
        }
        
        /*private bool SetOneOfBest_old(PairInt inCnt, int inLayer, NodePoint inSample)
        {
        	bool noLayer = false;
            int sourceBest = 0;
            
            ContactSimple simple = new ContactSimple(inCnt, inLayer);
        	List<ContactSimple> arround = simple.GetArroundPoints(wide);
            
            foreach (ContactSimple cntArround in arround)
            {
                NodePoint pn = GetPoint(cntArround);
                if ((GetPoint(cntArround).name != inSample.name) &&
                    (!GetPoint(cntArround).isReplace) &&
                    (GetPoint(cntArround).name != Material.diffusionName))
                    noLayer = true;
                if (!noLayer && GetPoint(cntArround).isSource &&
                   (GetPoint(cntArround).name != inSample.name))
                    sourceBest++;
            }
            int sourseCount = 0;
            if ((sourceBest > 1) && (diffusionExeption.FindIndex(el => el == inSample.name) >= 0))
            {
                foreach (ContactSimple cnt in GetSourceContacts(new ContactSimple(inCnt, inLayer), inSample.name))
                {
                	if (GetPoint(cnt).name == inSample.name)
                		sourseCount++;
                }
                if (sourseCount > sourceBest)
                	sourceBest = 1;
            }
            
            if (!noLayer && (sourceBest < 2) && (GetPoint(inCnt, inLayer).isReplace))
            {
                SetNextCont(inCnt, inLayer, inSample);//error gap in line
                return true;
            }
            return false;
        }*/

        
        private bool SetOneOfBest(PairInt inCnt, int inLayer, NodePointLayer inSample)
        {
        	NodePoint crPnt = GetPoint(inCnt, inLayer);
        	if (!crPnt.isReplace || inSample.name == Material.blankName || crPnt.name == inSample.name)
        		return false;
        	
        	ContactSimple curCentr = new ContactSimple(inCnt, inLayer);
        	SetContact(curCentr, Material.blankName, 0, 0, -1);
			/*layoutMap[inChanged.x][inChanged.y][layerChanged].isReplace = false;
			layoutMap[inChanged.x][inChanged.y][layerChanged].name = inSample.name;
			layoutMap[inChanged.x][inChanged.y][layerChanged].numberNode = inSample.numberNode;
			layoutMap[inChanged.x][inChanged.y][layerChanged].priority = inSample.priority;
			layoutMap[inChanged.x][inChanged.y][layerChanged].number = inSample.number;
			*/
			string curName = inSample.name;
			
			//ContactSimple curPoint = new ContactSimple(inChanged, layerChanged);
			List<ContactSimple> cntArround = curCentr.GetArroundPoints(wide);
			
			//List<PairInt> cntCheckRepl = inCnt.GetBigArround(wide);
			//cntCheckRepl.Add(inCnt);
			
			bool contSetting = true;
			bool retValue = true;
			//foreach (ContactSimple cntUnit in cntArround)
			while (contSetting)
			{
				contSetting = false;
				retValue = true;
				List<string> namesChanged = new List<string>();
				foreach (ContactSimple cntUnit in cntArround)
				{
					NodePoint unitPnt = GetPoint(cntUnit);
					string unitName = unitPnt.name;
					if (unitName != curName && unitPnt.isReplace && 
					    unitName!= Material.blankName && unitName != Material.diffusionName)
					{
						contSetting = true;
						if (cntUnit.x == 29 && (cntUnit.y == 38) && 
				    	cntUnit.layer == Layers.siliconTrace && unitName == "INC")//4 point !!!!! != 737
						{
							List<NodePoint> ps = new List<NodePoint>();
							foreach (ContactSimple cnt in cntUnit.GetNeborPoints(wide))
								ps.Add(GetPoint(cnt));
							contSetting = true;
						}
						
						namesChanged.Add(unitName);
						SetContact(cntUnit, Material.blankName, 0, 0, -1);
						//cntAdded.Add(cntUnit);
						if (GetPoint(cntUnit).isSource)
	                    {
	                        foreach (ContactSimple smp in GetSourceContacts(cntUnit, curName))
	                            if (smp != cntUnit)
	                        		layoutMap[smp.x][smp.y][smp.layer].isReplace =	CheckNebors(new ContactSimple(smp, smp.layer));
	                                //cntCheckRepl.Add(smp);
	                    }
	
	                    foreach (ContactSimple cnt in cntUnit.GetNeborPoints(wide))
	                    {
	                    	/*string bn = GetPoint(cnt).name;
	                    	if (cnt.x == 38 && cnt.y == 14 && cnt.layer == 1 && bn != Material.blankName)
								b = false;*/
	                    	foreach (int lay in Params.LayersRange[inLayer])
	                    	{
		                    	//if (GetPoint(cnt, lay).name == unitName)//curName) && (cntCheckRepl.FindIndex(el => el == cnt) < 0))
		                    		layoutMap[cnt.x][cnt.y][lay].isReplace =
									CheckNebors(new ContactSimple(cnt, lay));
		                            //cntCheckRepl.Add(cnt);
	                    	}
	                    }
					}
					else if (unitName != curName &&
					        unitName!= Material.blankName && unitName != Material.diffusionName)
					{
						retValue = false;
						if (namesChanged.FindIndex(el => el == unitPnt.name) >= 0)//--------delete
							layoutMap[cntUnit.x][cntUnit.y][cntUnit.layer].isReplace =
								CheckNebors(new ContactSimple(cntUnit, cntUnit.layer));
					}
				}
			}
			
			if (retValue)
			{
				layoutMap[inCnt.x][inCnt.y][inLayer].isReplace = false;
				layoutMap[inCnt.x][inCnt.y][inLayer].name = inSample.name;
				layoutMap[inCnt.x][inCnt.y][inLayer].numberNode = inSample.numberNode;
				layoutMap[inCnt.x][inCnt.y][inLayer].priority = inSample.priority;
				layoutMap[inCnt.x][inCnt.y][inLayer].number = inSample.number;
			}
			
			List<PairInt> checkRepl = inCnt.GetBigArround(wide);
			checkRepl.Add(inCnt);
			foreach (PairInt cntUnit in checkRepl)
			{
				foreach (int lay in Params.LayersRange[inLayer])
				{
					//if (GetPoint(cntUnit, lay).name == curName)
						layoutMap[cntUnit.x][cntUnit.y][lay].isReplace =
							CheckNebors(new ContactSimple(cntUnit, lay));
				}
			}
			
			return retValue;
        	//--------------------------
        	/*bool noLayer = false;
            int sourceBest = 0;
            
            ContactSimple simple = new ContactSimple(inCnt, inLayer);
        	List<ContactSimple> arround = simple.GetArroundPoints(wide);
            
            foreach (ContactSimple cntArround in arround)
            {
                NodePoint pn = GetPoint(cntArround);
                if ((GetPoint(cntArround).name != inSample.name) &&
                    (!GetPoint(cntArround).isReplace) &&
                    (GetPoint(cntArround).name != Material.diffusionName))
                    noLayer = true;
                if (!noLayer && GetPoint(cntArround).isSource &&
                   (GetPoint(cntArround).name != inSample.name))
                    sourceBest++;
            }
            int sourseCount = 0;
            if ((sourceBest > 1) && (diffusionException.FindIndex(el => el == inSample.name) >= 0))
            {
                foreach (ContactSimple cnt in GetSourceContacts(new ContactSimple(inCnt, inLayer), inSample.name))
                {
                	if (GetPoint(cnt).name == inSample.name)
                		sourseCount++;
                }
                if (sourseCount > sourceBest)
                	sourceBest = 1;
            }
            
            if (!noLayer && (sourceBest < 2) && (GetPoint(inCnt, inLayer).isReplace))
            {
            	if (!SetNextCont(inCnt, inLayer, inSample))
            		return false;
                return true;
            }*/
            //return false;
        }
        
        private bool IsDiffusionLimit(PairInt inCnt, int inLayer1, int inLayer2)
        {
        	if ( ((inLayer1 == Layers.metal1Trace) && (inLayer2 == Layers.siliconTrace)) ||
        	   ((inLayer1 == Layers.siliconTrace) && (inLayer2 == Layers.metal1Trace)) )
        	{
        		if (GetPoint(inCnt, Layers.contactTrace).name == Material.diffusionName)
        			return false;
        	}
        	return true;
        }

        /*private void SetPoint_old(PairInt inCnt)
        {
        	bool replased = false;
        	foreach (int curLayer in Params.UsedLayers)
        		if (GetPoint(inCnt, curLayer).isReplace)
        			replased = true;
        	if (!replased)
        		return;
            //if ((!GetPoint(inCnt, Layers.metal1Trace).isReplace) &&
              // (!GetPoint(inCnt, Layers.siliconTrace).isReplace))
              //  return;
        	
        	bool b;
        	if (inCnt.x == 20 && inCnt.y == 16 )
        		b = false;

            List<NodePoint> best = GetBest_old(inCnt);
            NodePoint bestPoint = best[0];
            NodePoint bestMet1 = best[1];
            NodePoint bestSil = best[2];

            if (diffusionExeption.FindIndex(el => el == bestPoint.name) >= 0)
            {
            	SetPointWithExept(inCnt, best);
            	return;
            }
            
            if (bestPoint == bestMet1)
            {
                bool useBestPoint = false;
                if (GetPoint(inCnt, Layers.metal1Trace).isReplace)
                {
                    if (SetOneOfBest_old(inCnt, Layers.metal1Trace, bestPoint) &&
                        GetPoint(inCnt, Layers.contactTrace).name != Material.diffusionName &&
                        IsJumpAble(inCnt, Layers.siliconTrace) &&
                        SetOneOfBest_old(inCnt, Layers.siliconTrace, bestPoint))
                        useBestPoint = true;
                }
                if (!useBestPoint)
                    SetOneOfBest_old(inCnt, Layers.siliconTrace, bestSil);
            }
            else
            {
                bool useBestPoint = false;
                if (GetPoint(inCnt, Layers.siliconTrace).isReplace)
                {
                    if (SetOneOfBest_old(inCnt, Layers.siliconTrace, bestPoint) &&
                        GetPoint(inCnt, Layers.contactTrace).name != Material.diffusionName &&
                        IsJumpAble(inCnt, Layers.metal1Trace) &&
                        SetOneOfBest_old(inCnt, Layers.metal1Trace, bestPoint))
                        useBestPoint = true;
                }
                if (!useBestPoint)
                    SetOneOfBest_old(inCnt, Layers.metal1Trace, bestMet1);
            }
        }*/
        
        private void SetPoint(PairInt inCnt)
        {
        	bool replased = false;
        	
        		
        	foreach (int curLayer in Params.UsedLayers)
        		if (GetPoint(inCnt, curLayer).isReplace)
        			replased = true;
        	if (!replased)
        		return;
        	
        	//if (inCnt.x == 12 && inCnt.y == 24 &&  (AddingSpace(layoutMap[13][24][Layers.metal2Trace].name) == "66204"))
        	//	replased = true;
        	bool b = false;
			if (inCnt.x == 27 && inCnt.y == 17 )//"N6859613")
				b = false;

            List<BestPointSet> best = GetBest(inCnt);
            List<int> closedLayers = new List<int>();
            if (Params.UsedLayers.Count > 2)
            	best = SortBestPoints(best);
            
            if ( inCnt.x == 14 && (inCnt.y == 38) )// || inCnt.y == 37) )
        	{
        		ContactSimple tmp = new ContactSimple(inCnt, 0);
        		List<ContactSimple> tp = tmp.GetAnyNeborPoints();
        		List<NodePoint> p = new List<NodePoint> ();
        		foreach(ContactSimple t in tp)
        			p.Add(GetPoint(t));
        		p.Add(GetPoint(tmp));
        	}

            foreach (BestPointSet bestPoint in best)
            {
            	if ((!bestPoint.isOneLayer) && (closedLayers.FindIndex(el => el == bestPoint.point.layer) < 0))
            	{
		            if (diffusionException.FindIndex(el => el == bestPoint.point.name) >= 0)
		            {
		            	if (Params.IsDiffusExeptionLayer(bestPoint.point.layer))
	            	    {
		            		SetOneOfBest(inCnt, bestPoint.point.layer, bestPoint.point);
		            		closedLayers.Add(bestPoint.point.layer);
	            	    }
		            }
		            else
		            {
		            	if (SetOneOfBest(inCnt, bestPoint.point.layer, bestPoint.point))
		                {
			            	foreach (int related in Params.LayersRange[bestPoint.point.layer])
		                	{
	                			if ((related != bestPoint.point.layer) &&
		                		    IsDiffusionLimit(inCnt, related, bestPoint.point.layer) &&
			                        IsJumpAble(inCnt, related) &&
			                        SetOneOfBest(inCnt, related, bestPoint.point))
		                			closedLayers.Add(related);
		                	}
		                }
		            	else 
		            	{
		            		foreach (int lay in  DefineSameLayers(inCnt, bestPoint, best))
		            		{
		            			if (SetOneOfBest(inCnt, lay, bestPoint.point))
				                {
					            	foreach (int related in Params.LayersRange[lay])
				                	{
			                			if ((related != lay) &&
				                		    IsDiffusionLimit(inCnt, related, lay) &&
					                        IsJumpAble(inCnt, related) &&
					                        SetOneOfBest(inCnt, related, bestPoint.point))
				                			closedLayers.Add(related);
				                	}
				                }
		            		}
		            	}
		            	closedLayers.Add(bestPoint.point.layer);
		            }
            	}
            }
            foreach (BestPointSet bestPoint in best)
            {
            	if ((bestPoint.isOneLayer) && (closedLayers.FindIndex(el => el == bestPoint.point.layer) < 0))
            	{
            		SetOneOfBest(inCnt, bestPoint.point.layer, bestPoint.point);
            	}
            }
        }
        
        private void SetPointNotCross(PairInt inCnt)
        {
        	bool replased = false;
        	foreach (int curLayer in Params.UsedLayers)
        		if (GetPoint(inCnt, curLayer).isReplace)
        			replased = true;
        	if (!replased)
        		return;
        	
        	List<BestPointSet> best = GetBest(inCnt);
        	
        	foreach (BestPointSet bestPoint in best)
            {
        		if (bestPoint.isOneLayer && IsPointFree(new ContactSimple(inCnt, bestPoint.point.layer), bestPoint.point.name))
            	{
            		SetOneOfBest(inCnt, bestPoint.point.layer, bestPoint.point);
            	}
            }
        }
        
        private bool IsPointFree(ContactSimple inPoint, string curName)
        {
        	if (GetPoint(inPoint).name != curName && GetPoint(inPoint).name != Material.blankName)
        		return false;
        	foreach (ContactSimple pn in inPoint.GetArroundPoints(wide))
        		if (GetPoint(pn).name != curName && GetPoint(pn).name != Material.blankName)
        			return false;
        	return true;
        }
        
        private List<int> DefineSameLayers(PairInt inCnt, BestPointSet pointUnit, List<BestPointSet> bestPoint)
        {
        	List<int> bestLayers = new List<int>();
        	foreach (BestPointSet pntSet in bestPoint)
        	{
        		if (pntSet.isOneLayer && (NodePoint)pntSet.point == (NodePoint)pointUnit.point)
        			bestLayers.Add(pntSet.point.layer);
        	}
        	return bestLayers;
        }
        
        
        private List<BestPointSet> SortBestPoints(List<BestPointSet> inLst)
        {
        	bool isExcept = conflictManager.IsExceptionContain(inLst);
        	for (int i = 0; i < inLst.Count; i++)
        	{
        		for (int j = i + 1; j < inLst.Count; j++)
        		{
        			if (isExcept && conflictManager.IsBestExecept(inLst[j].point) && conflictManager.IsBestExecept(inLst[i].point))
        			{
        				if (inLst[j].point.priority > inLst[i].point.priority)
        				{
        					BestPointSet nd = new BestPointSet(inLst[j]);
	        				inLst[j] = new BestPointSet(inLst[i]);
	        				inLst[i] = nd;
        				}
        			}
        			else if (isExcept && conflictManager.IsBestExecept(inLst[j].point) && (!conflictManager.IsBestHeight(inLst[i].point)))
        			{
        				BestPointSet nd = new BestPointSet(inLst[j]);
        				inLst[j] = new BestPointSet(inLst[i]);
        				inLst[i] = nd;
        			}
        			else if (inLst[j].point.priority > inLst[i].point.priority)
    				{
    					BestPointSet nd = new BestPointSet(inLst[j]);
        				inLst[j] = new BestPointSet(inLst[i]);
        				inLst[i] = nd;
    				}
        		}
        	}
        	return inLst;
        }
        
        /*private void SetPointWithExept(PairInt inCnt, List<NodePoint> best)
        {
            NodePoint bestPoint = best[0];
            NodePoint bestMet1 = best[1];
            NodePoint bestSil = best[2];
            
            SetOneOfBest_old(inCnt, Layers.metal1Trace, bestMet1);
            SetOneOfBest_old(inCnt, Layers.siliconTrace, bestSil);
        }

        /*private void SetPointWithDiffusExept(PairInt inCnt, BestPointSet pnt, List<BestPointSet> best)
        {
            foreach (int lay in Params.LayersRange[pnt.point.layer])
            {
            	BestPointSet bst = best.Find(el => el.isOneLayer && (el.layer == replased));
	            SetOneOfBest(inCnt, Layers.metal1Trace, bestMet1);
	            SetOneOfBest(inCnt, Layers.siliconTrace, bestSil);
            }
        }*/
        
        bool IsJumpAble(PairInt inCnt, int inLayer)
        {
        	string curName = GetPoint(inCnt, inLayer).name;
        	if (curName == Material.blankName)
        		return true;
        	if (curName == Material.diffusionName )
        		return false;
        	
        	ContactSimple centrCnt = new ContactSimple(inCnt, inLayer);
        	
        	foreach(ContactSimple cnt in centrCnt.GetNeborPoints(wide))//GetArroundPoints(wide))
        		if (GetPoint(cnt).name != curName)
        			return false;
        	return true;
        }
        	
        	
        private ContactSimple GetAnyNotDiffusion(PairInt inCnt)
        {
            if (GetPoint(inCnt, Layers.siliconTrace).name != Material.diffusionName)
                return new ContactSimple(inCnt, Layers.siliconTrace);
            foreach (PairInt curPoint in inCnt.GetArroundPoints(wide))
                if (GetPoint(curPoint, Layers.siliconTrace).name != Material.diffusionName)
                    return new ContactSimple(inCnt, Layers.siliconTrace);
            return new ContactSimple(inCnt, Layers.siliconTrace);
        }

       /* private List<NodePoint> GetBest_old(PairInt inCnt)
        {
            List<NodePointLayer> metArround = new List<NodePointLayer>();
            List<NodePointLayer> silArround = new List<NodePointLayer>();
            List<NodePointLayer> allArround = new List<NodePointLayer>();

            foreach (PairInt cnt in inCnt.GetArroundPoints(wide))
            {
                metArround.Add(GetPointLayer(cnt, Layers.metal1Trace));
                silArround.Add(GetPointLayer(cnt, Layers.siliconTrace));
            }
            metArround.Add(GetPointLayer(inCnt, Layers.metal1Trace));
            silArround.Add(GetPointLayer(inCnt, Layers.siliconTrace));
            allArround.AddRange(metArround);
            allArround.AddRange(silArround);
            //NodePoint bestMet = conflictManager.GetBest(metArround);
            //NodePoint bestSil = conflictManager.GetBest(silArround);
            bool b = false;
            if ((inCnt.x == 13 || inCnt.x == 21) && (inCnt.y == 33))
                b = true;

            List<NodePoint> best = new List<NodePoint>();
            best.Add(conflictManager.GetBest(allArround));
            best.Add(conflictManager.GetBest(metArround));
            best.Add(conflictManager.GetBest(silArround));
            return best;
        }*/
        
                
        private List<BestPointSet> GetBest(PairInt inCnt)//, List<int> layers)
        {
        	Dictionary<int, List<NodePointLayer>> oneRange = new Dictionary<int, List<NodePointLayer>>();
        	foreach (int layer1 in Params.UsedLayers)
        		oneRange.Add(layer1, new List<NodePointLayer>());
        	
            foreach (PairInt cnt in inCnt.GetArroundPoints(wide))
            {
            	foreach (int layer1 in Params.UsedLayers)
            		oneRange[layer1].Add(GetPointLayer(cnt, layer1));
            }
            foreach (int layer1 in Params.UsedLayers)
            {
            		oneRange[layer1].Add(GetPointLayer(inCnt, layer1));
            		
            }
            
            
            List<BestPointSet> bestSet = new List<BestPointSet>();
            foreach (int layer1 in Params.UsedLayers)
            {
            	List<NodePointLayer> allArround = new List<NodePointLayer>();
            	allArround.AddRange(oneRange[layer1]);
            	foreach(int layer2 in Params.LayersRange[layer1])
            	{
            		allArround.AddRange(oneRange[layer2]);
            	}
            	bestSet.Add(new BestPointSet(conflictManager.GetBest(allArround), layer1, false));
            	if (Params.UsedLayers.Count < 3)
            		break;
            }
            
            foreach (int layer1 in Params.UsedLayers)
            {
            	bestSet.Add(new BestPointSet(conflictManager.GetBest(oneRange[layer1]), layer1, true));
            	
            	NodePoint ndp = GetPoint(inCnt, layer1);
            	if ( ndp.isSource )
        		{
        			NodePoint ndPoint = DefineSourcePoint(inCnt, layer1);
        			if (ndPoint.name != ndp.name)
        				bestSet.Add(new BestPointSet(new NodePointLayer(ndPoint, layer1), layer1, true));
        				//oneRange[layer1].Add(new NodePointLayer(ndp, nameSource));
        		}	
        		/*if ( ndp.isSource )
        		{
        			string nameSource = DefineSourceName(inCnt, layer1);
        			if (nameSource != ndp.name)
        				bestSet.Add(new BestPointSet(new NodePointLayer(ndp, layer1, nameSource), layer1, true));
        				//oneRange[layer1].Add(new NodePointLayer(ndp, nameSource));
        		}*/
            }
            
            return bestSet;
        }
        
        private bool CheckBigArea(ContactSimple curCnt)//, string curName)
        {
            //ContactSimple curCnt = new ContactSimple(inCnt, inLayer);
            /*bool b = false;
            if (curCnt.x == 15 && curCnt.y == 33 && curCnt.layer == Layers.siliconTrace)
                b = true;*/

            string curName = GetPoint(curCnt).name;

            foreach(ContactSimple smp in curCnt.GetArroundPoints(wide))
            	if (GetPoint(smp).name != curName)
            		return true;
            
            List<ContactSimple> rhombus = curCnt.GetAnyInBigRadius(wide);

            int start = rhombus.FindIndex(el => GetAnyPoint(el).name == curName);
            int end = rhombus.FindIndex(el => GetAnyPoint(el).name != curName);

            if (end < 0)
                return true;
            if (end == 0 && start < 0)
                return true;

            List<ContactSimple> nebors = new List<ContactSimple>();
            int start2 = rhombus.FindIndex(end, el => GetAnyPoint(el).name == curName);
            if (end < start2)
            {
                nebors.AddRange(rhombus.GetRange(start2, rhombus.Count - start2));
                nebors.AddRange(rhombus.GetRange(0, start2));
            }
            else
                nebors = rhombus;

            bool nameThere = false;
            bool firstFilledArea = true;
            foreach (ContactSimple cntNebor in nebors)
            {
                if (GetAnyPoint(cntNebor).name == curName)
                {
                    nameThere = true;
                }
                else if (nameThere && firstFilledArea)
                {
                    firstFilledArea = false;
                    nameThere = false;
                }
                else if (nameThere)
                {
                    return false;
                }
            }
            return true;
        }

        /*private ContactSimple GetOppositeContact(ContactSimple inCnt)
        {
            int oppositeLayer = Layers.siliconTrace;
            if (inCnt.layer == Layers.siliconTrace)
                oppositeLayer = Layers.metal1Trace;

            return new ContactSimple(inCnt, oppositeLayer);
        }*/
				
		private NodePoint GetPoint(ContactSimple inCnt)
		{
			if (inCnt.x < 0 ||  inCnt.y < 0 || inCnt.y >= Params.topEdge)
				return layoutMap[0][Params.topEdge - 1][inCnt.layer];
			return layoutMap[inCnt.x][inCnt.y][inCnt.layer];
		}
		
		public bool IsPointForContact(ContactSimple inCnt, int inType)//string inType)
		{
			if ((inType == Material.csi_) && (GetPoint(inCnt, Layers.siliconTrace).name == Material.diffusionName))
				return false;
			return true;
		}

        /*private NodePoint GetOppositePoint(ContactSimple inCnt)
        {
            int oppositeLayer = Layers.siliconTrace;
            if (inCnt.layer == Layers.siliconTrace)
                oppositeLayer = Layers.metal1Trace;

            return layoutMap[inCnt.x][inCnt.y][oppositeLayer];
        }*/
		
		private NodePoint GetPoint(PairInt inCnt, int inLayer)
		{
			return layoutMap[inCnt.x][inCnt.y][inLayer];
		}

        private NodePointLayer GetPointLayer(PairInt inCnt, int inLayer)
        {
            return new NodePointLayer(layoutMap[inCnt.x][inCnt.y][inLayer], inLayer);
        }
		
		private NodePoint GetAnyPoint(ContactSimple inCnt)
		{
			if ( (inCnt.x >= 0) && (inCnt.x < wide) && (inCnt.y < Params.topEdge) && (inCnt.y >= 0) )
				return layoutMap[inCnt.x][inCnt.y][inCnt.layer];
			return blank;
		}
		
		private NodePoint GetAnyPoint(PairInt inCnt, int inLayer)
		{
			if ( (inCnt.x >= 0) && (inCnt.x < wide) && (inCnt.y < Params.topEdge) && (inCnt.y >= 0) )
				return layoutMap[inCnt.x][inCnt.y][inLayer];
			return blank;
		}
		
		private void SetNextCont_old(PairInt inChanged, int layerChanged, NodePoint inSample)
		{
			bool b = false;
			if (inChanged.x == 33 && (inChanged.y == 16 || inChanged.y == 18) && 
			    layerChanged == Layers.metal1Trace && inSample.name != "N6859613")
				b = false;
			
			if (inSample.name == Material.blankName)
				return;
			layoutMap[inChanged.x][inChanged.y][layerChanged].isReplace = false;
			layoutMap[inChanged.x][inChanged.y][layerChanged].name = inSample.name;
			layoutMap[inChanged.x][inChanged.y][layerChanged].numberNode = inSample.numberNode;
			layoutMap[inChanged.x][inChanged.y][layerChanged].priority = inSample.priority;
			layoutMap[inChanged.x][inChanged.y][layerChanged].number = inSample.number;
			
			string curName = inSample.name;
			
			
			ContactSimple curPoint = new ContactSimple(inChanged, layerChanged);
			List<ContactSimple> cntArround = curPoint.GetArroundPoints(wide);
			
			List<PairInt> cntArea = inChanged.GetBigArround(wide);
			cntArea.Add(inChanged);
			
			foreach (ContactSimple cntUnit in cntArround)
			{
				string unitName = GetPoint(cntUnit).name;
				if (unitName != curName && unitName!= Material.blankName &&
				    unitName != Material.diffusionName)
				{
					if (cntUnit.x == 33 && (cntUnit.y == 16 || cntUnit.y == 18) && 
			    	cntUnit.layer == Layers.metal1Trace)
						b = false;
					SetContact(cntUnit, Material.blankName, 0, 0, -1);
					//cntAdded.Add(cntUnit);
					if (GetPoint(cntUnit).isSource)
                    {
                        foreach (ContactSimple smp in GetSourceContacts(cntUnit, curName))
                            if (smp != cntUnit)
                                cntArea.Add(smp);
						//cntArea.Add(GetSourceContacts //GetNearSource(cntUnit));
                    }

                    foreach (ContactSimple cnt in cntUnit.GetNeborPoints(wide))
                    {
                    	string bn = GetPoint(cnt).name;
                    	if (cnt.x == 38 && cnt.y == 14 && cnt.layer == 1 && bn != Material.blankName)
							b = false;
                        if ((GetPoint(cnt).name != curName) && (cntArea.FindIndex(el => el == cnt) < 0))
                            cntArea.Add(cnt);
                    }
				}
			}
			
			
			foreach (PairInt cntUnit in cntArea)
			{
				foreach (int lay in Params.LayersRange[layerChanged])
				{
					layoutMap[cntUnit.x][cntUnit.y][lay].isReplace =
						CheckNebors(new ContactSimple(cntUnit, lay));
				}
			}
		}
		
		private bool SetNextCont(PairInt inChanged, int layerChanged, NodePoint inSample)
		{
			/*bool b = false;
			if (inChanged.x == 33 && (inChanged.y == 16 || inChanged.y == 18) && 
			    layerChanged == Layers.metal1Trace && inSample.name != "N6859613")
				b = false;*/
			
			if (inSample.name == Material.blankName)
				return true;
			
			string curName = inSample.name;
			List<string> deletedNames = new List<string>();
			
			ContactSimple curPoint = new ContactSimple(inChanged, layerChanged);
			List<ContactSimple> cntArround = curPoint.GetArroundPoints(wide);
			
			List<PairInt> cntArea = inChanged.GetBigArround(wide);
			cntArea.Add(inChanged);
			
			foreach (ContactSimple cntUnit in cntArround)
			{
				string unitName = GetPoint(cntUnit).name;
				if (unitName != curName && unitName!= Material.blankName &&
				    unitName != Material.diffusionName)
				{
					/*if (cntUnit.x == 33 && (cntUnit.y == 16 || cntUnit.y == 18) && 
			    	cntUnit.layer == Layers.metal1Trace)
						b = false;*/
					if (deletedNames.FindIndex(el => el == unitName) >= 0)
					{
						bool rem = CheckNebors(new ContactSimple(cntUnit, cntUnit.layer));
						if (!rem)
						{
							layoutMap[cntUnit.x][cntUnit.y][cntUnit.layer].isReplace = rem;
							return false;
						}
					}
							
					deletedNames.Add(unitName);
					SetContact(cntUnit, Material.blankName, 0, 0, -1);
					//cntAdded.Add(cntUnit);
					if (GetPoint(cntUnit).isSource)
                    {
                        foreach (ContactSimple smp in GetSourceContacts(cntUnit, curName))
                            if (smp != cntUnit)
                                cntArea.Add(smp);
						//cntArea.Add(GetSourceContacts //GetNearSource(cntUnit));
                    }
					
					//List<PairInt> cntForCheck = new List<PairInt>();
                    foreach (ContactSimple cnt in cntUnit.GetNeborPoints(wide))
                    {
                    	/*string bn = GetPoint(cnt).name;
                    	/*if (cnt.x == 38 && cnt.y == 14 && cnt.layer == 1 && bn != Material.blankName)
							b = false;*/
                        if ((GetPoint(cnt).name != curName) && (cntArea.FindIndex(el => el == cnt) < 0))
                        {
                            cntArea.Add(cnt);
                            //cntForCheck.Add(cnt);
                        }
                    }
				}
			}
			
			layoutMap[inChanged.x][inChanged.y][layerChanged].isReplace = false;
			layoutMap[inChanged.x][inChanged.y][layerChanged].name = inSample.name;
			layoutMap[inChanged.x][inChanged.y][layerChanged].numberNode = inSample.numberNode;
			layoutMap[inChanged.x][inChanged.y][layerChanged].priority = inSample.priority;
			layoutMap[inChanged.x][inChanged.y][layerChanged].number = inSample.number;
			
			foreach (PairInt cntUnit in cntArea)
			{
				foreach (int lay in Params.LayersRange[layerChanged])
				{
					layoutMap[cntUnit.x][cntUnit.y][lay].isReplace =
						CheckNebors(new ContactSimple(cntUnit, lay));
				}
				/*bool b;
				if (cntUnit.x == 20 && cntUnit.y == 16)
					b = true;
				layoutMap[cntUnit.x][cntUnit.y][Layers.metal1Trace].isReplace = 
					CheckNebors(new ContactSimple(cntUnit, Layers.metal1Trace));
				layoutMap[cntUnit.x][cntUnit.y][Layers.siliconTrace].isReplace = 
					CheckNebors(new ContactSimple(cntUnit, Layers.siliconTrace));*/
			}
			return true;
		}

       
		
		public void SetContact(List<ContactSimple> inCnt, string inName, int inNumNode, int inPrior, int inNumber)//int inX, int inY, int layer, ContactSimple inSample)
		{
			foreach (ContactSimple curContact in inCnt)
			{
				layoutMap[curContact.x][curContact.y][curContact.layer].isReplace = false;
				layoutMap[curContact.x][curContact.y][curContact.layer].name = inName;
				layoutMap[curContact.x][curContact.y][curContact.layer].numberNode = inNumNode;
				layoutMap[curContact.x][curContact.y][curContact.layer].priority = inPrior;
				layoutMap[curContact.x][curContact.y][curContact.layer].number = inNumber;
			}
		}
		
		public void SetContact(ContactSimple inCnt, string inName, int inNumNode, int inPrior, int inNumber)//int inX, int inY, int layer, ContactSimple inSample)
		{
			//if (inCnt.x < 0 || inCnt.y < 0)
			//	return;
			layoutMap[inCnt.x][inCnt.y][inCnt.layer].isReplace = false;
			layoutMap[inCnt.x][inCnt.y][inCnt.layer].name = inName;
			layoutMap[inCnt.x][inCnt.y][inCnt.layer].numberNode = inNumNode;
			layoutMap[inCnt.x][inCnt.y][inCnt.layer].priority = inPrior;
			layoutMap[inCnt.x][inCnt.y][inCnt.layer].number = inNumber;
		}
		
		public void SetContactNumber(List<ContactSimple> inCnt, int inNumber)
		{
			foreach (ContactSimple curContact in inCnt)
				layoutMap[curContact.x][curContact.y][curContact.layer].number = inNumber;
		}
		
		/*public void SetPinContact(ContactSimple inCnt, string inName, int inNumNode, int inPrior)//int inX, int inY, int layer, ContactSimple inSample)
		{
			layoutMap[inCnt.x][inCnt.y][inCnt.layer].isReplace = false;
			layoutMap[inCnt.x][inCnt.y][inCnt.layer].name = inName;
			layoutMap[inCnt.x][inCnt.y][inCnt.layer].numberNode = inNumNode;
			layoutMap[inCnt.x][inCnt.y][inCnt.layer].priority = inPrior;
			layoutMap[inCnt.x][inCnt.y][inCnt.layer].isSource = true;
			layoutMap[inCnt.x][inCnt.y][inCnt.layer].number = 0;
		}*/
		
		public void SetPinSource(ContactSimple inCnt)
		{
			layoutMap[inCnt.x][inCnt.y][inCnt.layer].isSource = true;
		}
		
		public void SetPinSource(List<ContactSimple> inCnts)
		{
			foreach (ContactSimple cnt in inCnts)
				layoutMap[cnt.x][cnt.y][cnt.layer].isSource = true;
		}
		
		private bool CheckNebors(ContactSimple inCnt)
		{
			List<NodePoint> ndPns = new List<NodePoint>();
			bool returnValue = true;
			
			string curName = layoutMap[inCnt.x][inCnt.y][inCnt.layer].name;
			
			List<NodePoint> neborsN = new List<NodePoint>();
			List<NodePoint> neborsOpposN = new List<NodePoint>();
			
			bool b = false;
			/*if (inCnt.x == 21  && inCnt.y == 39 &&
			    inCnt.layer == Layers.metal1Trace && curName == "N6864112")//"N6866204")//"N6862737")//"N6859613")
				b = false;
			/*
			b = false;
			if (inCnt.x == 39 && inCnt.y == 14)
				b = false;*/
			if (inCnt.x == 29 && inCnt.y == 38 &&
			    inCnt.layer == Layers.siliconTrace && curName == "INC")
			{
				foreach (int lay in Params.UsedLayers)
				{
					foreach (ContactSimple cs in inCnt.GetAnyNeborPoints())
					{
						neborsN.Add(GetPoint(cs, lay));
					}
	                neborsN.Add(GetPoint(inCnt, lay));
				}
                //neborsOpposN.Add(GetOppositePoint(inCnt));
                b=true;
			}
			
			if (curName == Material.blankName)
				return true;
            if ((curName == Material.diffusionName) && 
                (layoutMap[inCnt.x][inCnt.y][Layers.contactTrace].name == Material.diffusionName))
				return false;
			if (GetPoint(inCnt).isSource && !(FindSource(inCnt)))
				return false;
			if (GetPoint(inCnt).isFixed)
				return false;
			
			List<Section> sections = GetSections(inCnt, curName);

			/*if (end < 0)
				return CheckBigArea(inCnt);//true
			if (end == 0 && start < 0 && (!IsBridge(inCnt)))
				return true;*/
			if (sections.Count <= 1)
				returnValue = !IsBridge2(sections, inCnt, inCnt.layer, curName);//!!!!!!
			else
			{
				List<PairInt> area = ((PairInt)inCnt).GetAnyNeborPoints();//new List<PairInt>(nebors);
				area.Add(inCnt);
				//returnValue = IsSectionsConnected(sections, area, related, curName);
				foreach (int related in Params.LayersRange[inCnt.layer])
				{
					if (related != inCnt.layer)
						returnValue = IsSectionsConnected(sections, area, inCnt.layer, related, curName);//CheckRelatePoint(nebors, curName, related);
					if (!returnValue)
						break;
				}
			}

			//if (!returnValue && b)
			//	b = true;
			//bool diffus = ((nebors[0].layer == Layers.metal1Trace) && (opposLayer == Layers.siliconTrace)) ||
				//((nebors[0].layer == Layers.siliconTrace) && (opposLayer == Layers.metal1Trace));
			
			//if (returnValue && IsBridge(inCnt))//(Params.UsedLayers.Count == 2) && 
              //  returnValue = false;
			
			bool isCheckBigArea = true;
			if (returnValue)
			{
				foreach (int lay in Params.LayersRange[inCnt.layer])
				{
					if ((lay != inCnt.layer) && (GetPoint(inCnt, lay).name == curName))
						isCheckBigArea = false;
				}
			}
				
			if (returnValue && isCheckBigArea)//(GetPoint(inCnt, opposLayer).name != curName))
                returnValue = CheckBigArea(inCnt);
		/*
			if (returnValue && ((inCnt.x == 22 && inCnt.y == 47 && inCnt.layer == Layers.siliconTrace) || 
			                    (inCnt.x == 21 && inCnt.y == 46 && inCnt.layer == Layers.siliconTrace)) &&
			      curName == "INC")
				b = true;*/
		
			if (returnValue && inCnt.x == 20  && inCnt.y == 38 &&
			    inCnt.layer == Layers.siliconTrace && curName == "N6864112")//"N6859613")
				b = false;
            return returnValue;
		}
		
		private List<Section> GetSections(ContactSimple inCnt, string curName)
		{
			List<ContactSimple> nebors = new List<ContactSimple>();
			List<ContactSimple> neborsFst = inCnt.GetAnyNeborPoints();

            
			int start = neborsFst.FindIndex(el => GetAnyPoint(el).name == curName);
			int end = neborsFst.FindIndex(el => GetAnyPoint(el).name != curName);
			
			//if (b && ((end < 0) || (end == 0 && start < 0)))
			//	b = true;
			/*foreach (ContactSimple cnt in neborsFst)
			{
				if (cnt.x == 39 && cnt.y == 14 && cnt.layer == 1)
				{
					NodePoint pn = GetPoint(cnt);
					if (pn.name == Material.blankName)
						b = false;
				}
			}*/
			

			/*if (end < 0)
				return CheckBigArea(inCnt);//true
			if (end == 0 && start < 0 && (!IsBridge(inCnt)))
				return true;*/
			int start2 = 0;
			if (end >= 0)
				start2 = neborsFst.FindIndex(end, el => GetAnyPoint(el).name == curName);
			/*if (end == 0 && start > 0)
			{
				end = neborsFst.FindIndex(start, el => GetAnyPoint(el).name == curName);
				start2 = neborsFst.FindIndex(end, el => GetAnyPoint(el).name == curName);
			}
			else// 
			if (end == 0 && end < start) 
			{
				nebors.AddRange(neborsFst.GetRange(start, neborsFst.Count - start));
				nebors.AddRange(neborsFst.GetRange(0, start));
			}
			else*/
			if (end < start2)
			{
				nebors.AddRange(neborsFst.GetRange(start2, neborsFst.Count - start2));
				nebors.AddRange(neborsFst.GetRange(0, start2));
			}
			else
				nebors = neborsFst;
			
			List<Section> sections = new List<Section>();
			List<PairInt> oneSection = new List<PairInt>();
			bool sectEnd = true;
			bool isSection = false;
			
			for (int i = 0; i < nebors.Count; i++)
			{
				if ((nebors[i].y < Params.topEdge) && (nebors[i].x < wide) &&
				    (nebors[i].y >= 0) && (nebors[i].x >= 0) &&
				    (GetPoint(nebors[i]).name == curName))
				{
					if (sectEnd)
					{
						oneSection.Clear();
						isSection = false;
					}
					if (nebors[i].x == inCnt.x || nebors[i].y == inCnt.y)
						isSection = true;
						//sections.Add(new Section());
					//sections[sectCount].points.Add(nebors[i]);
					oneSection.Add(new PairInt(nebors[i]));
					sectEnd = false;
				}
				else if (!sectEnd)
				{
					sectEnd = true;
					if (isSection)
					{
						sections.Add(new Section(oneSection));
						isSection = false;
					}
				}
			}
			if (isSection)
				sections.Add(new Section(oneSection));
			
			return sections;
		}
		
		private bool CheckRelatePoint(List<ContactSimple> nebors, string curName, int opposLayer)//??????????
		{
			bool returnValue = true;
			ContactSimple opposPoint = new ContactSimple(0,0,0);
			ContactSimple prevOpposPoint = new ContactSimple(0,0,0);
			bool opposLayerThere = false;
			bool prevOpposLayer = false;
			bool firstFilledArea = true;
			bool nameThere = false;
			
			bool diffus = ((nebors[0].layer == Layers.metal1Trace) && (opposLayer == Layers.siliconTrace)) ||
				((nebors[0].layer == Layers.siliconTrace) && (opposLayer == Layers.metal1Trace));
			
			foreach ( ContactSimple cntNebor in nebors )
			{
				if ( GetAnyPoint(cntNebor).name == curName )
				{
					nameThere = true;
				}
				else if (nameThere && firstFilledArea)
				{
					firstFilledArea = false;
					prevOpposLayer = opposLayerThere;
					prevOpposPoint = new ContactSimple(opposPoint);
					opposLayerThere = false;
					nameThere = false;
				}
				else if (nameThere)
				{
					if ( (!opposLayerThere) || (!prevOpposLayer) )
						returnValue = false; //return false;
					if (!ConnectedPoints(prevOpposPoint, opposPoint))
						returnValue = false; //return false;
					
					prevOpposLayer = opposLayerThere;
					nameThere = false;
					opposLayerThere = false;
				}
				if ( nameThere && GetAnyPoint(cntNebor, opposLayer).name == curName &&
				    (!diffus || (GetAnyPoint(cntNebor, Layers.contactTrace).name != Material.diffusionName)))
				{
					opposLayerThere = true;
					opposPoint = new ContactSimple(cntNebor, opposLayer);
				}
			}
			
			return returnValue;
		}
		
		public class Section
		{
			public List<PairInt> points;
			public Section()
			{
				points = new List<PairInt>();
			}
			public Section(List<PairInt> inPoints)
			{
				points = new List<PairInt>(inPoints);
			}
			//public List<PairInt> area;
			//public List<ContactSimple> checkArea;
			//public string name;
			//public List<int> Get
		}
		
		private bool IsSectionsConnected(List<Section> inSections, List<PairInt> area, int inLayer1, int relatedLayer, string inName)
		{
			//for ( int i = 0; i < inSections.count; i++ )//(Section sec in sectionsA)
			
			List<PairInt> pointsInSection = inSections[0].points.FindAll(coord => GetPoint(coord, relatedLayer).name == inName);
			bool checkDif = (inLayer1 == Layers.siliconTrace || relatedLayer == Layers.siliconTrace);
			if (pointsInSection.Count > 0 && (inLayer1 == Layers.siliconTrace || relatedLayer == Layers.siliconTrace))
			{
				List<PairInt> correctPoints = new List<PairInt>();
				foreach (PairInt onePoint in pointsInSection)
				{
					if (GetPoint(onePoint, Layers.contactTrace).name != Material.diffusionName)
						correctPoints.Add(onePoint);
				}
				pointsInSection = correctPoints;
			}
			if (pointsInSection.Count <= 0)
				return false;
			
			List<PairInt> fndInLayer = new List<PairInt>();
			
			List<PairInt> area2 = new List<PairInt>(area);
			foreach(PairInt pnt in pointsInSection)
				area2.RemoveAll(el => el == pnt);
			//List<PairInt> fnd = new List<PairInt>();
			//bool continueSeak;
			do
			{
				//continueSeak = false;
				pointsInSection.Clear();
				foreach(PairInt pointInSec in pointsInSection)
				{
					List<PairInt> fnd = area2.FindAll(coord => ((GetPoint(coord, relatedLayer).name == inName) &&
					                                        (coord.x == pointInSec.x || coord.y == pointInSec.y)) );
					if (fnd.Count > 0)
					{
						//continueSeak = true;
						fndInLayer.AddRange(fnd);
						pointsInSection.AddRange(fnd);
						foreach(PairInt pnt in fnd)
							area2.RemoveAll(el => el == pnt);
					}
				}
				//pointsInSection = new List<PairInt>(fndInLayer);
			} while(fndInLayer.Count > 0);
			
			for(int i = 1; i < inSections.Count; i++)//PairInt coord in fndInLayer)//sec.sects)
			{
				bool isInSec = false;
				foreach(PairInt pr in inSections[i].points)
				{
					if (fndInLayer.FindIndex(el => el == pr) >= 0 && 
					    (!checkDif || GetPoint(pr, Layers.contactTrace).name != Material.diffusionName))
						isInSec = true;
				}
				if (!isInSec)
					return false;
				//int idx = area.FindIndex(GetPoint(coord, inLayer).name == inName
			}
			return true;
		}
		
		/*private List<PairInt> DeleteSome(List<PairInt> inList, List<PairInt> delList)
		{
			foreach(PairInt pr in delList)
			{
				int idx = inList.FindIndex(el => el == pr);
				inList.RemoveAll
			}
		}*/
		
		private bool ConnectedPoints(ContactSimple inCnt1, ContactSimple inCnt2)
		{
			string inName = GetAnyPoint(inCnt1).name;
			foreach (VariantTrace var in ContactSimple.GetConnectionPoints(inCnt1, inCnt2, wide))
			{
				bool connected = true;
				foreach (ContactSimple pnt in var.pnt)
					if (GetAnyPoint(pnt).name != inName)
						connected = false;
				if (connected)
					return true;
			}
			return false;
		}

        private bool IsBridgeM1Si(ContactSimple inCnt, int opposLayer)
        {
            /*int opposLayer = Layers.metal1Trace;
			if (inCnt.layer == Layers.metal1Trace)
				opposLayer = Layers.siliconTrace;*/

            string curName = GetPoint(inCnt).name;
            if (curName != GetPoint(inCnt, opposLayer).name ||
                GetPoint(inCnt, Layers.contactTrace).name == Material.diffusionName)
                return false;

            bool isSpreadThisL = false;
            //bool isSpredOpposL = false;
            List<ContactSimple> arround = inCnt.GetArroundPoints(wide);
            foreach (ContactSimple cnt in arround)
            {
                if (GetPoint(cnt).name == curName && GetPoint(cnt, opposLayer).name == curName &&
            	   GetPoint(cnt, Layers.contactTrace).name != Material.diffusionName)
                    return false;
                if (GetPoint(cnt).name == curName && (GetPoint(cnt, opposLayer).name != curName ||
            	                                      GetPoint(cnt, Layers.contactTrace).name == Material.diffusionName))
                    isSpreadThisL = true;
                //if (GetPoint(cnt).name != curName && (GetPoint(cnt, opposLayer).name == curName ||
            	  //                                    GetPoint(cnt, Layers.contactTrace).name == Material.diffusionName))
                  //  isSpredOpposL = true;
            }

            bool isBridgeBool = false;
            if (isSpreadThisL)// && isSpredOpposL) //isSpredOpposL - delete
                isBridgeBool = true;
            if (!isBridgeBool && isSpreadThisL && IsOnlyConnectionM1Si(inCnt))
            	isBridgeBool = true;
            
            return isBridgeBool;//false;
        }
        
        private bool IsBridge2(List<Section> sections, PairInt inPnt, int inLayer, string inName)
        {
        	if (sections.Count == 0)
        	{
        		if (inLayer == Layers.metal1Trace && Params.UsedLayers.Count > 2 && GetPoint(inPnt, Layers.siliconTrace).name == inName &&
        		   GetPoint(inPnt, Layers.contactTrace).name != Material.diffusionName && 
        		   GetPoint(inPnt, Layers.metal2Trace).name == inName)
        			return true;
        	}
        	else if (sections.Count == 1)//!!!!!!!!!!
        	{
        		foreach(int curLayer in Params.allLayersRange.Keys)//.UsedLayers)
        		{
        			if ( curLayer != inLayer && IsPointsConnected(inPnt, inLayer, curLayer, inName) && //GetPoint(inPnt, curLayer).name == inName
        			    (sections[0].points.FindIndex(el => IsPointsConnected(el, inLayer, curLayer, inName) && 
        			                                  ConnectedPoints(new ContactSimple(el, curLayer), new ContactSimple(inPnt, curLayer))) < 0) )
        			                                  //GetPoint(el, curLayer).name == inName) < 0) )
        				return true;
        		}
        		return false;
        	}
        	return false;
        }
        
        private bool IsPointsConnected(PairInt inPnt, int inLayer, int relatedLayer, string inName)
        {
        	if ((inLayer != relatedLayer) && (inLayer == Layers.siliconTrace || relatedLayer == Layers.siliconTrace))// &&
        	{
        	    if (GetPoint(inPnt, Layers.contactTrace).name == Material.diffusionName ||
        		   GetPoint(inPnt, relatedLayer).name != inName)
        			return false;
        		return true;
        	}
        	else
        	{
        		if (GetPoint(inPnt, relatedLayer).name == inName)
        			return true;
        		return false;
        	}
        }
        
        private List<ContactSimple> GetPointsInLayers(ContactSimple inCnt, string inName)
        {
        	List<ContactSimple> connectedPoints = new List<ContactSimple>();
        	bool loopLayer = true;
        	int curLayer = inCnt.layer;
        	while(loopLayer)
        	{
        		loopLayer = false;
        		foreach (int oneLayer in Params.allLayersRange[curLayer])
        		{
        			if ( (oneLayer != inCnt.layer) && IsPointsConnected(inCnt, inCnt.layer, oneLayer, inName) &&//(GetPoint(inCnt, oneLayer).name == inName) &&
        			    (connectedPoints.FindIndex(el => el.layer == oneLayer) < 0) )
        			{
        				connectedPoints.Add(new ContactSimple(inCnt, oneLayer));
        				curLayer = oneLayer;
        				loopLayer = true;
        			}
        		}
        	}
        	return connectedPoints;
        }
        
        private bool IsBridge(ContactSimple inCnt)
        {
        	bool isBridgeBool = false;
        	List<int> layers = Params.GetOppositeLayers(inCnt.layer);
        	foreach (int lay in layers)
        	{
        		bool curBridge = false;
        		if (lay == Layers.siliconTrace || inCnt.layer == Layers.siliconTrace)
        			curBridge = IsBridgeM1Si(inCnt, lay);
        		else
        			curBridge = IsBridgeM1M2(inCnt, lay);
        		isBridgeBool = isBridgeBool || curBridge;
        	}
        	if ((!isBridgeBool) && (layers.Count > 1))
        		isBridgeBool = IsTwoLayerBridge(inCnt, layers);
        	return isBridgeBool;
        }
        
        private bool IsTwoLayerBridge(ContactSimple inCnt, List<int> opposLayers)
        {
        	string curName = GetPoint(inCnt).name;
        	foreach(int lay in opposLayers)
        		if (GetPoint(inCnt, lay).name != curName)
        			return false;
        	List<ContactSimple> arround = inCnt.GetArroundPoints(wide);
        	foreach (ContactSimple cnt in arround)
            {
                if (GetPoint(cnt).name == curName)
                    return false;
            }
        	return true;
        }
        
        private bool IsBridgeM1M2(ContactSimple inCnt, int opposLayer)
        {
            /*int opposLayer = Layers.metal1Trace;
			if (inCnt.layer == Layers.metal1Trace)
				opposLayer = Layers.metal2Trace;*/

            string curName = GetPoint(inCnt).name;
            if (curName != GetPoint(inCnt, opposLayer).name)
                return false;

            bool isSpreadThisL = false;
            //bool isSpredOpposL = false;
            List<ContactSimple> arround = inCnt.GetArroundPoints(wide);
            foreach (ContactSimple cnt in arround)
            {
                if (GetPoint(cnt).name == curName && GetPoint(cnt, opposLayer).name == curName)
                    return false;
                if (GetPoint(cnt).name == curName && (GetPoint(cnt, opposLayer).name != curName))
                    isSpreadThisL = true;
                //if (GetPoint(cnt).name != curName && (GetPoint(cnt, opposLayer).name == curName))
                //    isSpredOpposL = true;
            }

            bool isBridgeBool = false;
            if (isSpreadThisL)// && isSpredOpposL)
                isBridgeBool = true;
          
            return isBridgeBool;//false;
        }
		
        public void SetStartLine(ContactSimple cnt1, ContactSimple cnt2, string inName, int inPriority, int inIdx)
        {
        	if (cnt1.layer == Layers.siliconTrace)
        		SetLine(new LineStruct(cnt1.GetInDiffusionEdge(), cnt2.GetInDiffusionEdge()),
        		        inName, inPriority, inIdx, true);
        	else
        	{
        		if (Params.IsModelBusM1InMiddle())
        		{
        			if (cnt1.y > cnt2.y)
        			{
        				SetLine(new LineStruct(cnt1.GetInDiffusionEdge(), cnt1.GetLowerPoint(2)),
	        		        inName, inPriority, inIdx, true);
        				SetLine(new LineStruct(cnt1.GetLowerPoint(2), cnt2.GetHigherPoint(2), Layers.metal2Trace),
	        		        inName, inPriority, inIdx, true);
        				SetLine(new LineStruct(cnt2.GetHigherPoint(2), cnt1.GetInDiffusionEdge()),
	        		        inName, inPriority, inIdx, true);
        			}
        			else
        			{
        				SetLine(new LineStruct(cnt2.GetInDiffusionEdge(), cnt2.GetLowerPoint(2)),
	        		        inName, inPriority, inIdx, true);
        				SetLine(new LineStruct(cnt2.GetLowerPoint(2), cnt1.GetHigherPoint(2), Layers.metal2Trace),
	        		        inName, inPriority, inIdx, true);
        				SetLine(new LineStruct(cnt1.GetHigherPoint(2), cnt1.GetInDiffusionEdge()),
	        		        inName, inPriority, inIdx, true);
        			}
        		}
        		else
        		{
        			SetLine(new LineStruct(cnt1.GetInDiffusionEdge(), cnt2.GetInDiffusionEdge()),
        		        inName, inPriority, inIdx, true);
        		}
        	}
        }
        
        private bool FindTrueSource(ContactSimple inCnt)
		{
        	NodePoint sourcePoint = GetPoint(inCnt);
			
			if (!sourcePoint.isSource || sourcePoint.name == Material.blankName)
				return false;
			Node nd = nodeList.Find(el => el.name == sourcePoint.name);
			int coordY = Params.lineN;
			if ( inCnt.y < Params.lineMiddle )
				coordY = Params.lineP;
			
			ContactSimple pin = new ContactSimple(inCnt.x, coordY, inCnt.layer);
            

            if (nd.arcCollection.FindIndex(el => el == pin) >= 0)//this node
                return true;
            
			return false;
		}
		
		private bool FindSource(ContactSimple inCnt)
		{
			string sourceName = GetPoint(inCnt).name;
			Node nd = nodeList.Find(el => el.name == sourceName);
			int coordY = Params.lineN;
			if ( inCnt.y < Params.lineMiddle )//(inCnt.y - Params.lineP) <= 3 )
				coordY = Params.lineP;
			
			ContactSimple pin = new ContactSimple(inCnt.x, coordY, inCnt.layer);
            

            if (nd.arcCollection.FindIndex(el => el == pin) < 0)//not this node
                return true;
            int countOneName = 0;
            bool nebourS = false;
            foreach (ContactSimple cnt in inCnt.GetArroundPoints(wide))
            {
            	if (GetPoint(cnt).name == sourceName && (cnt.x != inCnt.x))
            		countOneName++;
            	if ((GetPoint(cnt).name == sourceName) && (cnt.x == inCnt.x))
            	{
            		NodePoint ndPnt = GetPoint(new ContactSimple(cnt.x, cnt.y + (cnt.y - inCnt.y)*2, cnt.layer));
            		if ( (ndPnt.name == sourceName) && ndPnt.isSource )
            			nebourS = true;
            		else
            			countOneName++;
            	}
            }
            if (countOneName > 0)
            	return false;
            if (nebourS)
            	return true;
             
			countOneName = 0;            	
            foreach (ContactSimple cnt in GetSourceContacts(inCnt, sourceName))
            {
                if (GetPoint(cnt).name == sourceName)
                    countOneName++;
            }
            if (countOneName < 2)
                return false;
                /*if (layoutMap[inCnt.x][inCnt.y + 2][inCnt.layer].isSource &&
                    layoutMap[inCnt.x][inCnt.y + 2][inCnt.layer].name != sourceName &&
                    (nd.arcCollection.FindIndex(el => el == pin) >= 0))
                    return false;
                if (layoutMap[inCnt.x][inCnt.y - 2][inCnt.layer].isSource &&
                   layoutMap[inCnt.x][inCnt.y - 2][inCnt.layer].name != sourceName &&
                  (nd.arcCollection.FindIndex(el => el == pin) >= 0))
                    return false;*/
            
			return true;
		}
		
		private string DefineSourceName(PairInt inCnt, int inLayer)
		{
			ContactSimple fndCnt = new ContactSimple(inCnt, inLayer);
			fndCnt.y = Params.lineN;
			if ( inCnt.y < Params.lineMiddle )
				fndCnt.y = Params.lineP;
			
			foreach (Node nd in nodeList)
			{
				foreach(ContactSimple cnt in nd.arcCollection)
					if (cnt == fndCnt)
						return nd.name;
			}
			
			return "";
		}
		
		private NodePoint DefineSourcePoint(PairInt inCnt, int inLayer)
		{
			if (inCnt.y == Params.VccPosition || inCnt.y == Params.GndPosition)
				return GetPoint(new ContactSimple(inCnt, inLayer));
				
			ContactSimple fndCnt = new ContactSimple(inCnt, inLayer);
			fndCnt.y = Params.lineN;
			if ( inCnt.y < Params.lineMiddle )
				fndCnt.y = Params.lineP;
			
			foreach (Node nd in nodeList)
			{
				foreach(ContactSimple cnt in nd.arcCollection)
				{
					if (cnt == fndCnt)
					{
						List<ContactSimple> startWave = FindStartPoint(cnt, nd.name);
						NodePoint nd2 = GetPoint(startWave[0]);
						return nd2;//new NodePoint(nd2.name, nd2.priority, ); //nd.name;
					}
				}
			}
			
			return GetPoint(new ContactSimple(inCnt, inLayer));
		}

        public List<ContactSimple> GetSourceContacts(ContactSimple inCnt, string inName)
        {
            int coordY = Params.lineN;
            if (inCnt.y < Params.lineMiddle)//((inCnt.y - Params.lineP) <= 2)
                coordY = Params.lineP;

            //int layer = Layers.FromStrToNumber(inCnt.typePoint);
            List<ContactSimple> connectedCnt = new List<ContactSimple>();
            
            if (inCnt.isInOut())
            {
            	connectedCnt.Add(new ContactSimple(inCnt));
            	return connectedCnt;
            }
            
            if (Params.IsModelWithDif() && (inCnt.layer == Layers.metal1Trace))
            {
            	if (diffusionException.FindIndex(el => el == inName) >= 0)
	            {
	                connectedCnt.Add(new ContactSimple(inCnt.x, coordY, inCnt.layer));
	                connectedCnt.Add(new ContactSimple(inCnt.x, coordY, inCnt.layer));
	                return connectedCnt;
	            }
            	//if (coordY < Params.topEdge)
        		connectedCnt.Add(new ContactSimple(inCnt.x, coordY + 2, inCnt.layer));
            	//if (coordY >= Params.bottomEdge)
        		connectedCnt.Add(new ContactSimple(inCnt.x, coordY - 2, inCnt.layer));
            	return connectedCnt;
            }
            
            connectedCnt.Add(new ContactSimple(inCnt.x, coordY + 1, inCnt.layer));
            connectedCnt.Add(new ContactSimple(inCnt.x, coordY - 1, inCnt.layer));
            if (diffusionException.FindIndex(el => el == inName) >= 0)
            {
            	//if (coordY < Params.topEdge)
            	connectedCnt.Add(new ContactSimple(inCnt.x, coordY + 3, inCnt.layer));
            	//if (coordY > Params.bottomEdge)
            	connectedCnt.Add(new ContactSimple(inCnt.x, coordY - 3, inCnt.layer));
            }
            return connectedCnt;
        }
        
        /*public List<ContactSimple> GetSourceContacts(ContactSimple inCnt, string inName)
        {
            int coordY = Params.lineN;
            if ((inCnt.y - Params.lineP) <= 2)
                coordY = Params.lineP;

            List<Contact> connectedCnt = new List<Contact>();
            
            if (inCnt.isInOut())
            {
            	connectedCnt.Add(new Contact(inCnt));
            	return connectedCnt;
            }
            
            connectedCnt.Add(new Contact(inCnt.x, coordY + 1, inCnt.typePoint));
            connectedCnt.Add(new Contact(inCnt.x, coordY - 1, inCnt.typePoint));
            if (diffusionExeption.FindIndex(el => el == inName) >= 0)
            {
                connectedCnt.Add(new Contact(inCnt.x, coordY + 3, inCnt.typePoint));
                connectedCnt.Add(new Contact(inCnt.x, coordY - 3, inCnt.typePoint));
            }
            return connectedCnt;
        }*/
		
		
		private bool IsOnlyConnectionM1Si(ContactSimple inCnt)
		{
			if (GetPoint(inCnt, Layers.contactTrace).name == Material.diffusionName)
				return false;
			
			int layOppos = Params.LayersRange[inCnt.layer][0];
			if (layOppos == inCnt.layer)
				layOppos = Params.LayersRange[inCnt.layer][1];
			
			if (inCnt.y < (Params.topEdge - 1) &&
			    GetPoint(inCnt.GetHigherPoint(0), Layers.contactTrace).name == Material.diffusionName)
			{
				if ( (GetPoint(inCnt).name == GetPoint(inCnt.GetHigherPoint(0)).name) &&
				    (GetPoint(inCnt).name == GetPoint(inCnt.GetHigherPoint(0), layOppos).name) &&
				    (GetPoint(inCnt).name == GetPoint( inCnt.GetHigherPoint(5) ).name) &&
				    (GetPoint(inCnt).name == GetPoint( inCnt.GetHigherPoint(5), layOppos).name) )
					return false;
				return true;
			}
			if (inCnt.y > 0 &&
			    GetPoint(inCnt.GetLowerPoint(0), Layers.contactTrace).name == Material.diffusionName)
			{
				if ( (GetPoint(inCnt).name == GetPoint(inCnt.GetLowerPoint(0)).name) &&
				    (GetPoint(inCnt).name == GetPoint(inCnt.GetLowerPoint(0), layOppos).name) &&
				    (GetPoint(inCnt).name == GetPoint( inCnt.GetLowerPoint(5) ).name) &&
				    (GetPoint(inCnt).name == GetPoint( inCnt.GetLowerPoint(5), layOppos).name) )
					return false;
				return true;
			}
			return false;
		}
		
		/*public int SpreadWaveProcess(List<ContactSimple> inStart, NodePointProcess pntProcess)
        {
			string currentName = layoutMap[inStart[0].x][inStart[0].y][inStart[0].layer].name;

            List<ContactSimple> nextPoints = new List<ContactSimple>();//inStart.GetArroundPoints(wide);
            nextPoints.AddRange(inStart);
            List<ContactSimple> arround = new List<ContactSimple>();
            List<ContactSimple> passedPoints = new List<ContactSimple>();
            if (diffusionException.FindIndex(el => el == currentName) >= 0)
            {
            	List<ContactSimple> thisWave = SetOneWave(nextPoints, passedPoints, pntProcess, currentName);
            	SetOneWave(thisWave, passedPoints, pntProcess, currentName);
            	nextPoints.AddRange(thisWave);
            	passedPoints.AddRange(thisWave);
            }
            
            passedPoints.AddRange(inStart);
            
            int k1 = GetPoint(inStart[0]).number;

            do
            {
            	arround.Clear();
                foreach (ContactSimple cnt in nextPoints)
                {
                	foreach (ContactSimple cntNebor in cnt.GetArroundPoints(wide))
                	{
                		if ( (GetPoint(cntNebor).name == currentName) &&
                        (passedPoints.FindIndex(el => el == cntNebor) < 0) && 
                        (arround.FindIndex(el => el == cntNebor) < 0) )
                			arround.Add(cntNebor);
                	}
                }
                nextPoints.Clear();
                nextPoints.AddRange(arround);
                passedPoints.AddRange(arround);
            	
                pntProcess.IncrementNumber();
                
                bool continueMark  = false;
                do
                {
                	List<ContactSimple> wave = SetOneWave(arround, passedPoints, pntProcess, currentName);
                	nextPoints.AddRange(wave);
                	passedPoints.AddRange(wave);
                	
                	arround = wave;
                	continueMark = false;
                	if (wave.Count > 0)
                		continueMark = true;
                } while (continueMark);
                //passedPoints.AddRange(nextPoints);

                    //if (cntUnit.x == 14 && cntUnit.y == 5 && cntUnit.layer == Layers.siliconTrace)
                     //   b = true;            
            } while (nextPoints.Count > 0);

            int k = 1000;
            ContactSimple chk = new ContactSimple(27, 20, Layers.metal1Trace);
            int idx = passedPoints.FindIndex(el => el == chk);
            if (idx >= 0)
            	k = GetPoint(passedPoints[idx]).number;
            
            return passedPoints.Count;
        }*/
		
		public int CompleteSpreadWaveProcess(List<ContactSimple> inStart, NodePointProcess pntProcess)
        {
			string currentName = layoutMap[inStart[0].x][inStart[0].y][inStart[0].layer].name;

            List<ContactSimple> nextPoints = new List<ContactSimple>();//inStart.GetArroundPoints(wide);
            nextPoints.AddRange(inStart);
            List<ContactSimple> arround = new List<ContactSimple>();
            List<ContactSimple> passedPoints = new List<ContactSimple>();
            if (diffusionException.FindIndex(el => el == currentName) >= 0)
            {
            	List<ContactSimple> thisWave = SetOneWave(nextPoints, passedPoints, pntProcess, currentName);
            	SetOneWave(thisWave, passedPoints, pntProcess, currentName);
            	nextPoints.AddRange(thisWave);
            	passedPoints.AddRange(thisWave);
            }
            passedPoints.AddRange(inStart);
            
            int i = 0;
            do
            {
            	arround.Clear();
                foreach (ContactSimple cntOneLay in nextPoints)
                {
                	//foreach (int layer in Params.allLayersRange[cntOneLay.layer])
                	{
                		//ContactSimple cnt = new ContactSimple(cntOneLay, layer);
                		List<ContactSimple> relatedPoints = GetPointsInLayers(cntOneLay, currentName);
                		relatedPoints.Add(cntOneLay);
                		//if (IsPointsConnected(cnt, cntOneLay.layer, layer, currentName))
                		foreach (ContactSimple cnt in relatedPoints)
                		{
		                	foreach (ContactSimple cntNebor in cnt.GetArroundPoints(wide))
		                	{
		                		if ( (GetPoint(cntNebor).name == currentName) &&
		                        (passedPoints.FindIndex(el => el == cntNebor) < 0) && 
		                        (arround.FindIndex(el => el == cntNebor) < 0) )
		                		{
		                			arround.Add(cntNebor);//step 44 
		                			
		                			/*NodePoint pn1 = GetPoint(cntNebor);
	                				NodePoint pn2 = GetPoint(cntOneLay);
	                				if ( pn1.numberNode != pn2.numberNode)
	                					pn1 = GetPoint(cntNebor);*/
		                		}
		                		/*{
		                			if ((cntNebor.layer != cntOneLay.layer) && (cntNebor.layer == Layers.siliconTrace || 
		                		                                            cntOneLay.layer == Layers.siliconTrace) &&
		                		                                           (GetPoint(cntNebor, Layers.contactTrace).name == Material.diffusionName) )
		                				;
		                			else
		                			{
		                				NodePoint pn1 = GetPoint(cntNebor);
		                				NodePoint pn2 = GetPoint(cntOneLay);
		                				if ( pn1.numberNode != pn2.numberNode)
		                					pn1 = GetPoint(cntNebor);
		                				if ( cntNebor.x == 31 && (cntNebor.y == 21 || cntNebor.y == 22) && cntNebor.layer == 1)
		                					pn1 = GetPoint(cntNebor);
		                				
		                				arround.Add(cntNebor);
		                			}
		                		}*/
		                	}
		                	if (GetPoint(cnt).isSource)
		                	{
			                	foreach (ContactSimple cntNebor in GetSourceContacts(cnt, currentName))
			                	{
			                		//NodePoint np1 = GetPoint(cntNebor);
			                		//NodePoint np1 = GetPoint(cnt);
			                		if ( GetPoint(cntNebor).isSource && FindTrueSource(cntNebor) && (GetPoint(cntNebor).name == currentName) &&
			                        (passedPoints.FindIndex(el => el == cntNebor) < 0) && 
			                        (arround.FindIndex(el => el == cntNebor) < 0) )//&& FindSource(cnt) && FindSource(cntNebor) )
			                			arround.Add(cntNebor);
			                	}
		                	}
	                	}
                	}
                }
                CheckOnePoint();
                nextPoints.Clear();
                nextPoints.AddRange(arround);
                passedPoints.AddRange(arround);
            	
                pntProcess.IncrementNumber();
                
                if (currentName == "INC" && i == 30)//!!!!!!! step 44
                	i++;
                else
                	i++;
                
                bool continueMark  = false;
                do
                {
                	List<ContactSimple> wave = SetOneWave(arround, passedPoints, pntProcess, currentName);
                	nextPoints.AddRange(wave);
                	passedPoints.AddRange(wave);
                	
                	arround = wave;
                	continueMark = false;
                	if (wave.Count > 0)
                		continueMark = true;
                } while (continueMark);
                //passedPoints.AddRange(nextPoints);

                    /*if (cntUnit.x == 14 && cntUnit.y == 5 && cntUnit.layer == Layers.siliconTrace)
                        b = true;*/            
            } while (nextPoints.Count > 0);
            
            return passedPoints.Count;
        }
		
		public int CompleteSpreadWaveProcessRem(List<ContactSimple> inStart, NodePointProcess pntProcess)
        {
			string currentName = layoutMap[inStart[0].x][inStart[0].y][inStart[0].layer].name;

            List<ContactSimple> nextPoints = new List<ContactSimple>(inStart);//inStart.GetArroundPoints(wide);
            //nextPoints.AddRange(inStart);
            List<ContactSimple> arround = new List<ContactSimple>();
            List<ContactSimple> passedPoints = new List<ContactSimple>(inStart);
            //if (diffusionException.FindIndex(el => el == currentName) >= 0)
            {
            	List<ContactSimple> thisWave = SetOneWave(nextPoints, passedPoints, pntProcess, currentName);
            	SetOneWave(thisWave, passedPoints, pntProcess, currentName);
            	nextPoints.AddRange(thisWave);
            	passedPoints.AddRange(thisWave);
            }
            //passedPoints.AddRange(inStart); 22 step!!!!!!!!!!! VCC
            
            
            
            do
            {
            	/*bool continueMark  = false;
                do
                {
                	List<ContactSimple> wave = SetOneWave(nextPoints, passedPoints, pntProcess, currentName);
                	nextPoints.AddRange(wave);
                	passedPoints.AddRange(wave);
                	
                	arround = wave;
                	continueMark = false;
                	if (wave.Count > 0)
                		continueMark = true;
                } while (continueMark);
            	*/
            	
                foreach (ContactSimple cntOneLay in nextPoints)
                {
                		//if (GetPoint(cnt).name == currentName)
                		//{
                	foreach (ContactSimple cntNebor in cntOneLay.GetArroundPoints(wide))
                	{
                		if ( (GetPoint(cntNebor).name == currentName) &&
	                        (passedPoints.FindIndex(el => el == cntNebor) < 0) && 
	                        (arround.FindIndex(el => el == cntNebor) < 0) )
                		{
                			SetPoinProcess(cntNebor, cntNebor, pntProcess);
                			arround.Add(cntNebor);
                			
	                		//foreach (int layer in Params.GetOppositeLayers(cntNebor.layer))
	                		foreach (ContactSimple cnt in GetPointsInLayers(cntNebor, currentName))
		                	{
		                		//ContactSimple cnt = new ContactSimple(cntNebor, layer);
	                		
		                		if ( //IsPointsConnected(cnt, cntNebor.layer, layer, currentName) &&
		                        (passedPoints.FindIndex(el => el == cnt) < 0) && 
		                        (arround.FindIndex(el => el == cnt) < 0) )
		                		{
		                			if (SetPoinProcess(cntNebor, cnt, pntProcess))
		                			{
		                				arround.Add(cnt);
		                			
			                			if (GetPoint(cnt).isSource)
					                	{
						                	foreach (ContactSimple sourceContact in GetSourceContacts(cnt, currentName))
						                	{
						                		if ( GetPoint(sourceContact).isSource && FindTrueSource(sourceContact) && (GetPoint(sourceContact).name == currentName) &&
						                        (passedPoints.FindIndex(el => el == sourceContact) < 0) && 
						                        (arround.FindIndex(el => el == sourceContact) < 0) )//&& FindSource(cnt) && FindSource(cntNebor) )
						                		{
						                			SetPoinProcess(sourceContact, sourceContact, pntProcess);
						                			arround.Add(sourceContact);
						                		}
						                	}
					                	}
		                			}
		                		}
	                		}
	                		
	                		if (GetPoint(cntNebor).isSource)
		                	{
			                	foreach (ContactSimple sourceContact in GetSourceContacts(cntNebor, currentName))
			                	{
			                		if ( GetPoint(sourceContact).isSource && FindTrueSource(sourceContact) && (GetPoint(sourceContact).name == currentName) &&
			                        (passedPoints.FindIndex(el => el == sourceContact) < 0) && 
			                        (arround.FindIndex(el => el == sourceContact) < 0) )//&& FindSource(cnt) && FindSource(cntNebor) )
			                		{
			                			SetPoinProcess(sourceContact, sourceContact, pntProcess);
			                			arround.Add(sourceContact);
			                		}
			                	}
		                	}
                		}
                	}
                	
	                	
                	
                }
                
                
                nextPoints.Clear();
                nextPoints.AddRange(arround);
                passedPoints.AddRange(arround);
            	
                pntProcess.IncrementNumber();
                
                
                arround.Clear();
          
            } while (nextPoints.Count > 0);
            
            
            return passedPoints.Count;
        }
		
		public List<ContactSimple> ReturnAllWaveProcess(List<ContactSimple> inStart, NodePointProcess pntProcess)
        {
			string currentName = layoutMap[inStart[0].x][inStart[0].y][inStart[0].layer].name;

            List<ContactSimple> nextPoints = new List<ContactSimple>();//inStart.GetArroundPoints(wide);
            nextPoints.AddRange(inStart);
            List<ContactSimple> arround = new List<ContactSimple>();
            List<ContactSimple> passedPoints = new List<ContactSimple>();
            if (diffusionException.FindIndex(el => el == currentName) >= 0)
            {
            	List<ContactSimple> thisWave = SetOneWave(nextPoints, passedPoints, pntProcess, currentName);
            	SetOneWave(thisWave, passedPoints, pntProcess, currentName);
            	nextPoints.AddRange(thisWave);
            	passedPoints.AddRange(thisWave);
            }
            passedPoints.AddRange(inStart);
            
            do
            {
            	arround.Clear();
                foreach (ContactSimple cntOneLay in nextPoints)
                {
                	foreach (int layer in Params.allLayersRange[cntOneLay.layer])//GetOppositeLayers(cntOneLay.layer))
                	{
                		ContactSimple cnt = new ContactSimple(cntOneLay, layer);
                		if ( IsPointsConnected(cnt, cntOneLay.layer, layer, currentName) )//GetPoint(cnt).name == currentName)
                		{
		                	foreach (ContactSimple cntNebor in cnt.GetArroundPoints(wide))
		                	{
		                		if ( (GetPoint(cntNebor).name == currentName) &&
		                        (passedPoints.FindIndex(el => el == cntNebor) < 0) && 
		                        (arround.FindIndex(el => el == cntNebor) < 0) )
		                		{
		                			NodePoint pn1 = GetPoint(cntNebor);
	                				NodePoint pn2 = GetPoint(cntOneLay);
	                				if ( pn1.numberNode != pn2.numberNode)
	                					pn1 = GetPoint(cntNebor);
	                				
		                			arround.Add(cntNebor);
		                		}
		                		/*{
		                			if ((cntNebor.layer != cntOneLay.layer) && (cntNebor.layer == Layers.siliconTrace || 
		                		                                            cntOneLay.layer == Layers.siliconTrace) &&
		                		                                           (GetPoint(cntNebor, Layers.contactTrace).name == Material.diffusionName) )
		                				;
		                			else
		                			{
		                				NodePoint pn1 = GetPoint(cntNebor);
		                				NodePoint pn2 = GetPoint(cntOneLay);
		                				if ( pn1.numberNode != pn2.numberNode)
		                					pn1 = GetPoint(cntNebor);
		                				if ( cntNebor.x == 31 && (cntNebor.y == 21 || cntNebor.y == 22) && cntNebor.layer == 1)
		                					pn1 = GetPoint(cntNebor);
		                				
		                				arround.Add(cntNebor);
		                			}
		                		}*/
		                	}
		                	if (GetPoint(cnt).isSource)
		                	{
		                		NodePoint np2 = GetPoint(cnt);
		                		if (cnt.x == 19 && cnt.y == 33 && cnt.layer == 1)
		                			np2 = GetPoint(cnt);
			                		
			                	foreach (ContactSimple cntNebor in GetSourceContacts(cnt, currentName))
			                	{
			                		NodePoint np1 = GetPoint(cntNebor);
			                		if (cnt.x == 19 && cnt.y == 33 && cnt.layer == 1)
			                			np1 = GetPoint(cntNebor);
			                		
			                		if ( GetPoint(cntNebor).isSource && (GetPoint(cntNebor).name == currentName) &&
			                        (passedPoints.FindIndex(el => el == cntNebor) < 0) && 
			                        (arround.FindIndex(el => el == cntNebor) < 0) )//&& FindSource(cnt) && FindSource(cntNebor) )
			                		{
			                			NodePoint pn1 = GetPoint(cntNebor);
		                				NodePoint pn2 = GetPoint(cntOneLay);
		                				if ( pn1.numberNode != pn2.numberNode)
		                					pn1 = GetPoint(cntNebor);
	                				
			                			arround.Add(cntNebor);
			                		}
			                	}
		                	}
	                	}
                	}
                }
                nextPoints.Clear();
                nextPoints.AddRange(arround);
                passedPoints.AddRange(arround);
            	
                pntProcess.IncrementNumber();
                
                bool continueMark  = false;
                do
                {
                	List<ContactSimple> wave = SetOneWave(arround, passedPoints, pntProcess, currentName);
                	nextPoints.AddRange(wave);
                	passedPoints.AddRange(wave);
                	
                	arround = wave;
                	continueMark = false;
                	if (wave.Count > 0)
                		continueMark = true;
                } while (continueMark);
                //passedPoints.AddRange(nextPoints);

                    /*if (cntUnit.x == 14 && cntUnit.y == 5 && cntUnit.layer == Layers.siliconTrace)
                        b = true;*/            
            } while (nextPoints.Count > 0);
            
            NodePoint np3 = GetPoint(inStart[0]);
            foreach(ContactSimple cnt in passedPoints)
            {
            	if (cnt.x == 35 && cnt.y == 37 && cnt.layer == 1)
               			np3 = GetPoint(cnt);
            }
            
            
            return passedPoints;
        }
		
		private List<ContactSimple> SetOneWave(List<ContactSimple> inWave, List<ContactSimple> bandedConts, NodePointProcess pntProcess, string currentName)
		{
			List<ContactSimple> thisWave = new List<ContactSimple>();
			
			foreach (ContactSimple cntUnit in inWave)
			{
                //thisWave.Add(cntUnit);
                //passedPoints.Add(cntUnit);
                bool b;
                
                foreach (int opposLay in Params.GetOppositeLayers(cntUnit.layer))
                {
                	ContactSimple curCnt = new ContactSimple(cntUnit, opposLay);
                	if (bandedConts.FindIndex(el => el == curCnt) < 0)
                	{
	                	if ((opposLay == Layers.siliconTrace || cntUnit.layer == Layers.siliconTrace))
	                	{
	                    	if ((GetPoint(cntUnit, Layers.contactTrace).name != Material.diffusionName) &&
	                    	    (GetPoint(cntUnit, opposLay).name == currentName))
	                    	{
	                			//pntProcess.ProcessPoint(layoutMap[cntUnit.x][cntUnit.y][opposLay]);
	                			//passedPoints.Add(new ContactSimple(cntUnit,opposLay));
	                			if ( cntUnit.x == 31 && (cntUnit.y == 21 || cntUnit.y == 22))// && cntNebor.layer == 1)
	                				b = true;
	                					//pn1 = GetPoint(cntNebor);
	                    	
	                        	thisWave.Add(new ContactSimple(cntUnit,opposLay));
	                    	}
	                	}
	                	else
	                	{
	                		if (GetPoint(cntUnit, opposLay).name == currentName)
	                    	{
	                			//pntProcess.ProcessPoint(layoutMap[cntUnit.x][cntUnit.y][opposLay]);
	                			//passedPoints.Add(new ContactSimple(cntUnit,opposLay));
	                			if ( cntUnit.x == 31 && (cntUnit.y == 21 || cntUnit.y == 22))// && cntNebor.layer == 1)
	                				b = true;
	                			
	                        	thisWave.Add(new ContactSimple(cntUnit,opposLay));
	                    	}
	                	}
                	}
                }
                pntProcess.ProcessPoint(layoutMap[cntUnit.x][cntUnit.y][cntUnit.layer]);
			}
			
			return thisWave;
		}
		
		private bool SetPoinProcess(ContactSimple prevCnt, ContactSimple inCnt, NodePointProcess pntProcess)
		{
			if ( (prevCnt.layer != inCnt.layer) && ((prevCnt.layer == Layers.siliconTrace || inCnt.layer == Layers.siliconTrace)) )
        	{
            	if (GetPoint(inCnt, Layers.contactTrace).name != Material.diffusionName)
            	{
					pntProcess.ProcessPoint(layoutMap[inCnt.x][inCnt.y][inCnt.layer]);
					return true;
				}
			}
			else
        	{
    			pntProcess.ProcessPoint(layoutMap[inCnt.x][inCnt.y][inCnt.layer]);
			}
			return false;
		}
		/*
		public int SpreadWaveSimple(ContactSimple inStart)
        {
            int currentNdNumb = layoutMap[inStart.x][inStart.y][inStart.layer].numberNode;
            int currentNumber = layoutMap[inStart.x][inStart.y][inStart.layer].number;
            int currentPrior = layoutMap[inStart.x][inStart.y][inStart.layer].priority;
            string currentName = layoutMap[inStart.x][inStart.y][inStart.layer].name;

            List<ContactSimple> arround = inStart.GetArroundPoints(wide);
            List<ContactSimple> nextPoints = new List<ContactSimple>();
            List<ContactSimple> passedPoints = new List<ContactSimple>();
            passedPoints.Add(inStart);
            int count = 0;
            
            bool continueMark  = false;
            do
            {
                continueMark = false;
                currentNumber++;

                foreach (ContactSimple cntUnit in arround)
                {
                    
                    if ( (layoutMap[cntUnit.x][cntUnit.y][cntUnit.layer].name == currentName ) &&
                        (passedPoints.FindIndex(el => el == cntUnit) < 0) )
                    {
                        layoutMap[cntUnit.x][cntUnit.y][cntUnit.layer].numberNode = currentNdNumb;
                        layoutMap[cntUnit.x][cntUnit.y][cntUnit.layer].number = currentNumber;
                        layoutMap[cntUnit.x][cntUnit.y][cntUnit.layer].priority = currentPrior;
                			
                        continueMark = true;
                        nextPoints.Add(cntUnit);
                        passedPoints.Add(cntUnit);
                        count++;
                        
                        foreach (int opposLayer in Params.GetOppositeLayers(cntUnit.layer))
                        {
                        	if (opposLayer == Layers.siliconTrace || cntUnit.layer == Layers.siliconTrace)
                        	{
		                        if ( (GetPoint(cntUnit, Layers.contactTrace).name != Material.diffusionName) &&
		                            (GetPoint(cntUnit, opposLayer).name == currentName) )
		                        {
		                        	//layoutMap[cntUnit.x][cntUnit.y][opposLayer].numberNode = currentNdNumb;
		                        	//layoutMap[cntUnit.x][cntUnit.y][opposLayer].number = currentNumber;
		                            //layoutMap[cntUnit.x][cntUnit.y][opposLayer].priority = currentPrior;
		                        	
		                        	nextPoints.Add(new ContactSimple(cntUnit,opposLayer));
		                        	//passedPoints.Add(new ContactSimple(cntUnit,opposLayer));
		                            //count++;
		                        }
                        	}
                        	else
                        	{
                        		if (GetPoint(cntUnit, opposLayer).name == currentName)
		                        {
		                        	//layoutMap[cntUnit.x][cntUnit.y][opposLayer].numberNode = currentNdNumb;
		                        	//layoutMap[cntUnit.x][cntUnit.y][opposLayer].number = currentNumber;
		                            //layoutMap[cntUnit.x][cntUnit.y][opposLayer].priority = currentPrior;
		                        	
		                        	nextPoints.Add(new ContactSimple(cntUnit,opposLayer));
		                        	//passedPoints.Add(new ContactSimple(cntUnit,opposLayer));
		                            //count++;
		                        }
                        	}
                        }
                    }
                }
                arround.Clear();
                foreach (ContactSimple cnt in nextPoints)
                {
                    bool b = false;
                    if (cnt.x == 14 && cnt.y == 5 && cnt.layer == Layers.siliconTrace)
                        b = true;
                	foreach (ContactSimple cntNebor in cnt.GetArroundPoints(wide))
                		if (arround.FindIndex(el => el == cntNebor) < 0)
                			arround.Add(cntNebor);
                }
                nextPoints.Clear();
                
            } while (continueMark);

            return count;
        }*/
		/*
        public int SpreadWave(ContactSimple inStart)
        {
            int currentNdNumb = layoutMap[inStart.x][inStart.y][inStart.layer].numberNode;
            int currentNumber = layoutMap[inStart.x][inStart.y][inStart.layer].number;
            int currentPrior = layoutMap[inStart.x][inStart.y][inStart.layer].priority;
            string currentName = layoutMap[inStart.x][inStart.y][inStart.layer].name;

            List<ContactSimple> arround = inStart.GetArroundPoints(wide);
            List<ContactSimple> nextPoints = new List<ContactSimple>();
            int count = 0;
            
            bool continueMark  = false;
            do
            {
                continueMark = false;
                currentNumber++;

                foreach (ContactSimple cntUnit in arround)
                {
                    bool b = false;
                    if (cntUnit.x == 15 && cntUnit.y == 5 && cntUnit.layer == Layers.siliconTrace)
                        b = true;
                    if (cntUnit.x == 14 && cntUnit.y == 5 && cntUnit.layer == Layers.siliconTrace)
                        b = true;

                    int oppositeLayer = Layers.metal1Trace;
                    if (cntUnit.layer == oppositeLayer)
                    	oppositeLayer = Layers.siliconTrace;
                    
                    if ( (layoutMap[cntUnit.x][cntUnit.y][cntUnit.layer].name == currentName ) &&
                        ( (layoutMap[cntUnit.x][cntUnit.y][cntUnit.layer].number > currentNumber ||
                         layoutMap[cntUnit.x][cntUnit.y][cntUnit.layer].number < 0) ||
                		(layoutMap[cntUnit.x][cntUnit.y][cntUnit.layer].numberNode != currentNdNumb) ||
                        (layoutMap[cntUnit.x][cntUnit.y][cntUnit.layer].priority != currentPrior) ) )
                    {
                        layoutMap[cntUnit.x][cntUnit.y][cntUnit.layer].numberNode = currentNdNumb;
                        layoutMap[cntUnit.x][cntUnit.y][cntUnit.layer].number = currentNumber;
                        layoutMap[cntUnit.x][cntUnit.y][cntUnit.layer].priority = currentPrior;
                			
                        continueMark = true;
                        nextPoints.Add(cntUnit);
                        count++;
                        
                        if ( (GetPoint(cntUnit, Layers.contactTrace).name != Material.diffusionName) &&
                            (GetPoint(cntUnit, oppositeLayer).name == currentName) )
                        {
                        	layoutMap[cntUnit.x][cntUnit.y][oppositeLayer].numberNode = currentNdNumb;
                        	layoutMap[cntUnit.x][cntUnit.y][oppositeLayer].number = currentNumber;
                            layoutMap[cntUnit.x][cntUnit.y][oppositeLayer].priority = currentPrior;
                        	
                        	nextPoints.Add(new ContactSimple(cntUnit,oppositeLayer));
                            count++;
                        }
                    }
                }
                arround.Clear();
                foreach (ContactSimple cnt in nextPoints)
                {
                    bool b = false;
                    if (cnt.x == 14 && cnt.y == 5 && cnt.layer == Layers.siliconTrace)
                        b = true;
                	foreach (ContactSimple cntNebor in cnt.GetArroundPoints(wide))
                		if (arround.FindIndex(el => el == cntNebor) < 0)
                			arround.Add(cntNebor);
                }
                nextPoints.Clear();
                
            } while (continueMark);

            return count;
        }*/

        
    }
	
	
}
