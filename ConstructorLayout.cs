/*
 * Created by SharpDevelop.
 * User: Толя
 * Date: 01.07.2013
 * Time: 12:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
//using System.Drawing;

namespace eulerMake
{
    

	/// <summary>
	/// Constructor CPP file of Layout.
	/// </summary>
	public class ConstructorLayout
	{
        private List<string> placedN;
        private List<string> placedP;
        private List<LineStruct> fixedConnections;
        
        private Dictionary<string, NodeDistanceBase> nodeDistanceDict;
        private Dictionary<string, NodeTraces> nodeConnectionDict;
        
        
        private Node processNode;
        
        private int wide;
        private TraceGlobe trace;
        private int step;
        private List<string> diffusionExcep;
        
        /*public ConstructorLayout()
        {
        	placedN = new List<string>();
        	placedP = new List<string>();
        	bestRouting = new HistoryPairs();
        	InitSampleTraces();
        }*/
        
        public ConstructorLayout(List<string> inPlacedN, List<string> inPlacedP, List<Node> inNodeList)
		{
            placedN = inPlacedN;
            placedP = inPlacedP;
            //nodeList = inNodeList;
            
            nodeDistanceDict = new Dictionary<string, NodeDistanceBase>();
            nodeConnectionDict = new Dictionary<string, NodeTraces>();
            
        	//Params.SetModel(Params.ModelBusM2InMiddle);
            //TraceVccGndBus();
            fixedConnections = new List<LineStruct>();
            
            wide = placedN.Count*2 + Params.leftBorder + Params.rightBorder;
            
            diffusionExcep = new List<string>();
            diffusionExcep.Add("&0");
            diffusionExcep.Add("VCC");
            //-----------------
            trace = new TraceGlobe();

            trace.SetDiffusionExcep(diffusionExcep);
            trace.InitTransistors(placedN, placedP);
            trace.InitNodesDict(nodeDistanceDict, inNodeList);
            step = 0;
		}
        
        
        public bool RouteLines(System.IO.StreamWriter file)
        {
        	file.WriteLine("________Step_" + step.ToString() + "________");
        	
            trace.PropagateAll();
            trace.CheckOnePoint();
            trace.MergeNodes(true, file);//35step
            
            //if (step > 40)
            //if (step % 2 == 0)
            {
	            trace.PrintMap(Layers.metal1Trace, file);
	            trace.PrintMap(Layers.siliconTrace, file);
	            trace.PrintMap(Layers.metal2Trace, file);
	            trace.PrintNumb(step, file);
	            trace.PrintNumbM2(step, file);
            }
            
            step++;
            
            return trace.IsEnd();
        }
        
        /*public void AddNamedPins(List<ContactNamed> inCts)
        {
        	foreach(ContactNamed cnt in inCts)
        	{
        		if (diffusionExcep.FindIndex(name => name == cnt.namePoint) < 0)
        		{
        			trace
        		}
        	}
        }*/

        public void CorrectTrace(System.IO.StreamWriter file)
        {
        	trace.ClearForAlign(file);//GetTraces();
        	
        	trace.SetAllSamePriority();
        	trace.SpreadAllWaves();
        	
        	file.WriteLine("");
        	file.WriteLine("---------Correction----------");
            trace.PrintMap(Layers.metal1Trace, file);
            trace.PrintMap(Layers.siliconTrace, file);
            if (Params.IsModelBusM1InMiddle())
            	trace.PrintMap(Layers.metal2Trace, file);
            trace.PrintNumb(step, file);
            trace.PrintNumbM2(step,file);

            trace.ClearForAlign(file);
            
            trace.CheckOnePoint();
            
            for (int i = 0; i < 2; i++)
                trace.PropagateAllNotCross();
            trace.CheckOnePoint();
            
            trace.SpreadAllWaves();
            
            trace.CheckOnePoint();
            /*file.WriteLine("");
        	file.WriteLine("---------Correction_step1---------");
            trace.PrintMap(Layers.metal1Trace, file);
            trace.PrintMap(Layers.siliconTrace, file);
            if (Params.IsModelBusM1InMiddle())
            	trace.PrintMap(Layers.metal2Trace, file);
            trace.PrintNumb(step, file);
            trace.PrintNumbM2(step,file);*/

            trace.ClearForAlign(file);
            trace.CheckOnePoint();
            
            
            for (int i = 0; i < 4; i++)
                trace.PropagateAllNotCross();
            
            trace.CheckOnePoint();
            
            trace.ClearForAlign(file);
            for (int i = 0; i < 2; i++)
                trace.PropagateAllNotCross();
            
            trace.CheckOnePoint();
            /*file.WriteLine("");
        	file.WriteLine("---------Correction_step2---------");
            trace.PrintMap(Layers.metal1Trace, file);
            trace.PrintMap(Layers.siliconTrace, file);
            if (Params.IsModelBusM1InMiddle())
            	trace.PrintMap(Layers.metal2Trace, file);
            trace.PrintNumb(step, file);
            trace.PrintNumbM2(step,file);*/
            

            trace.SpreadAllWaves();
            
            trace.CheckOnePoint();
        }

        public void CreateTopFile(System.IO.StreamWriter file)
        {
            trace.PrintMap(Layers.metal1Trace, file);
            trace.PrintMap(Layers.siliconTrace, file);

            if (Params.IsModelBusM1InMiddle())
            	trace.PrintMap(Layers.metal2Trace, file);
            
            trace.SetPriority();
        	nodeConnectionDict = trace.GetTraces(file);
        }
        
        public int UntracedCount(System.IO.StreamWriter file)
        {
        	return trace.GetUntracedNodes(file);
        }
        
        private void TraceVccGndBus()
        {
        	int vccGndType = Layers.metal1Trace;// Params.met1Type;
        	if (Params.IsModelBusM2InMiddle())
        		vccGndType = Layers.metal2Trace;//Params.met2Type;

            ContactSimple left = new ContactSimple(0, Params.GndPosition, vccGndType);
            ContactSimple right = new ContactSimple(wide - 1, Params.GndPosition, vccGndType);
            fixedConnections.Add(new LineStruct(left, right));
            
            left = new ContactSimple(0, Params.VccPosition, vccGndType);
            right = new ContactSimple(wide - 1, Params.VccPosition, vccGndType);
            fixedConnections.Add(new LineStruct(left, right));
        }
        
        public void InitAllDistances(System.IO.StreamWriter file)//List<Node> inPlaceNodes)
        {
        	foreach (Node curNode in trace.GetNodeList())
    		{
    			processNode = curNode;
    			InitOneNodeConts();
                //SetInitPoint();
    		}
            
            SetStartPriority(trace);
            trace.SetReplace();

            //trace.PrintNumb();
            
        	trace.PrintMap(Layers.metal1Trace, file);
            trace.PrintMap(Layers.siliconTrace, file);
            //if (Params.IsModelBusM1InMiddle())
            {
            	trace.PrintMap(Layers.metal2Trace, file);
            	trace.PrintNumbM2(-1, file);
            }
            trace.PrintMap(Layers.contactTrace, file);
            trace.PrintNumb(-1, file);
        }
        
        private void InitOneNodeConts()
        {
        	if (diffusionExcep.FindIndex(el => el == processNode.name) >= 0)
        	{
        		InitGndVccConts();
        		return;
        	}
        	
        	int idxLast = processNode.arcCollection.Count;
        	
        	NodeDistanceBase dictUnit = new NodeDistanceBase(idxLast, processNode.name);
        	
        	
        	for (int i = 0; i < idxLast; i++)
        	{
        		for (int j = i + 1; j < idxLast; j++)
        		{
        			bool connected = false;
                    bool fixedConnection = false;
        			if (processNode.arcCollection[i].x == processNode.arcCollection[j].x)
        			{
        				connected = true;
                        fixedConnection = true;
        			}
        			if (i != j)
        				dictUnit.AddDistance(i, j, LineStruct.Distance(processNode.arcCollection[i],
                                                                    processNode.arcCollection[j]), connected, fixedConnection);
        		}
        	}

            if (idxLast > 1)
                nodeDistanceDict.Add(processNode.name, dictUnit);
        }
        
        private void InitGndVccConts()
        {
        	int lay = Layers.metal1Trace;
        	if (Params.IsModelBusM2InMiddle())
        		lay = Layers.metal2Trace;
        	
        	ContactSimple busStart = new ContactSimple(Params.leftEdge, Params.VccPosition, lay);
        	ContactSimple busEnd = new ContactSimple(wide - 1, Params.VccPosition, lay);
        	if (processNode.name == Params.GndName)
        	{
        		busStart.y = Params.GndPosition;
        		busEnd.y = Params.GndPosition;
        	}
        	busEnd.SetInOut();
        	busStart.SetInOut();
        	processNode.arcCollection.Insert(0, busEnd);
        	processNode.arcCollection.Insert(0, busStart);
        	
        	int idxLast = processNode.arcCollection.Count;
        	NodeDistanceBase dictUnit = new NodeDistanceBase(idxLast, processNode.name);
        	
        	for (int i = 0; i < idxLast; i++)
        	{
        		for (int j = i + 1; j < idxLast; j++)
        		{
        			bool connected = true;
                    bool fixedConnection = false;
                    if (i == 0 && j == 1)
                    	fixedConnection = true;
        			
        			if (i != j)
        				dictUnit.AddDistance(i, j, LineStruct.Distance(processNode.arcCollection[i],
                                                                    processNode.arcCollection[j]), connected, fixedConnection);
        		}
        	}

            if (idxLast > 1)
                nodeDistanceDict.Add(processNode.name, dictUnit);
        }
        
        /*private void SetInitPoint()
        {
            for (int i = 0; i < processNode.arcCollection.Count; i++)
            {
                trace.SetContact(Params.FromSimpleToCont(processNode.arcCollection[i].GetHigherPoint(0)), processNode.name, i, 0);
                trace.SetContact(Params.FromSimpleToCont(processNode.arcCollection[i].GetLowerPoint(0)), processNode.name, i, 0);
            }
        }*/

        private void SetStartPriority(TraceGlobe trace)
        {
            List<NodeDistanceBase> lst = nodeDistanceDict.Values.ToList();
            
            lst.Sort(NodeDistanceBase.CompareBaseByDist);
            int countNd = lst.Count;
            
            //bestHighPrior = lst.First().name;
            trace.SetHighPriority(lst.First().name);
            
            int curPrior = Params.maxPriority;

            foreach (NodeDistanceBase disBase in lst)//Node curNode in nodeList)
            {
            	processNode = trace.GetNodeByName(disBase.name);
            	
            	if (diffusionExcep.FindIndex(el => el == processNode.name) >= 0)
            		SetStartVccGndConnections(curPrior);
                else
                {
	                //List<int> markedPin = new List<int>();
	            	for (int i = 0; i < processNode.arcCollection.Count; i++)
	                {
	            		int idx = nodeDistanceDict[processNode.name].GetNumber(i);
	            		ContactSimple cnt = processNode.arcCollection[i];
	            		
	            		//List<Contact> sours = trace.GetSourceContacts(processNode.arcCollection[i])
	            		
	            		trace.SetPinSource(trace.GetSourceContacts(processNode.arcCollection[i], processNode.name));
	            		
	            		bool notOpposite = true;
	                	for (int j = i + 1; j < processNode.arcCollection.Count; j++)
	        			{
	                		if (processNode.arcCollection[i].x == processNode.arcCollection[j].x)
	                		{
	                			//trace.SetLine(new LineStruct(processNode.arcCollection[i].GetInDiffusionEdge(), processNode.arcCollection[j].GetInDiffusionEdge()),
	        				    //	          processNode.name, curPrior, idx, true);
	        				    trace.SetStartLine(processNode.arcCollection[i], processNode.arcCollection[j], 
	        				                       processNode.name, curPrior, idx);
	                			
	                			
	        				    trace.SetContact(processNode.arcCollection[i].GetInDiffusionEdge(), processNode.name, idx, curPrior, 0);
	                			
	        				    List<ContactSimple> startWave = new List<ContactSimple>();
	        				    startWave.Add( new ContactSimple(processNode.arcCollection[i].GetInDiffusionEdge()) );
	        				    NodePointProcess proc = new NodePointProcess(0, curPrior, idx, false);
	                			trace.CompleteSpreadWaveProcess(startWave, proc);//SpreadWaveProcess
	                			
	                			notOpposite = false;
	                		}
	                	}
	                	
	                	if (notOpposite)//markedPin.FindIndex(el => el == i) < 0)
	                	{
	                		trace.SetContact(processNode.arcCollection[i].GetInDiffusionEdge(), processNode.name, idx, curPrior, 0);
		                	/*trace.SetPinContact(Params.FromSimpleToCont(processNode.arcCollection[i].GetHigherPoint(0)), 
		            		                    processNode.name, idx, curPrior);
		                	trace.SetPinContact(Params.FromSimpleToCont(processNode.arcCollection[i].GetLowerPoint(0)), 
		            		                    processNode.name, idx, curPrior);*/
	                	}
	                }
                }
                curPrior--;
            }
        }
        
        private void SetStartVccGndConnections(int curPrior)
        {
        	int middleLine = Layers.metal1Trace;
        	if (Params.IsModelBusM2InMiddle())
        		middleLine = Layers.metal2Trace;
        	else if (Params.IsModelWithDif())
        	{
        		SetStartVccGndWithDiff(curPrior);
        		return;
        	}
        	if (processNode.name == Params.GndName)
        	{
        		for (int i = 0; i < processNode.arcCollection.Count; i++)
                {
        			trace.SetPinSource(trace.GetSourceContacts(processNode.arcCollection[i], processNode.name));
        			
        			ContactSimple transistorPoint = processNode.arcCollection[i].GetHigherPoint(2);
        			if (processNode.arcCollection[i].y > Params.GndPosition)
        				transistorPoint = processNode.arcCollection[i].GetLowerPoint(2);
        			
        			if (!processNode.arcCollection[i].isInOut())
        				trace.SetLine(new LineStruct(transistorPoint, 
        			                             new ContactSimple(processNode.arcCollection[i].x, Params.GndPosition, Layers.metal1Trace)),
        				    	          processNode.name, curPrior, 0, false);
        		}
        		
        		trace.SetLine(new LineStruct(new PairInt(Params.leftEdge, Params.GndPosition), 
        		                             new ContactSimple(wide - 1, Params.GndPosition, middleLine)),
        		                             processNode.name, curPrior, 0, true);
        	}
        	if (processNode.name == Params.VccName)
        	{
        		for (int i = 0; i < processNode.arcCollection.Count; i++)
                {
        			trace.SetPinSource(trace.GetSourceContacts(processNode.arcCollection[i], processNode.name));
        			
        			ContactSimple transistorPoint = processNode.arcCollection[i].GetHigherPoint(2);
        			if (processNode.arcCollection[i].y > Params.VccPosition)
        				transistorPoint = processNode.arcCollection[i].GetLowerPoint(2);
        			
        			if (!processNode.arcCollection[i].isInOut())
        				trace.SetLine(new LineStruct(transistorPoint, 
        			                             new ContactSimple(processNode.arcCollection[i].x, Params.VccPosition, Layers.metal1Trace)),
        				    	          processNode.name, curPrior, 0, false);
        		}
        		
        		trace.SetLine(new LineStruct(new PairInt(Params.leftEdge, Params.VccPosition), 
        		                             new ContactSimple(wide - 1, Params.VccPosition, middleLine)),
        		                             processNode.name, curPrior, 0, true);
        	}
        }
        
        private void SetStartVccGndWithDiff(int curPrior)
        {
        	int middleLine = Layers.metal1Trace;

        	if (processNode.name == Params.GndName)
        	{
        		for (int i = 0; i < processNode.arcCollection.Count; i++)
                {
        			trace.SetPinSource(processNode.arcCollection[i]);
        		}
        		
        		trace.SetLine(new LineStruct(new PairInt(Params.leftEdge, Params.GndPosition), 
        		                             new ContactSimple(wide - 1, Params.GndPosition, middleLine)),
        		                             processNode.name, curPrior, 0, true);
        	}
        	if (processNode.name == Params.VccName)
        	{
        		for (int i = 0; i < processNode.arcCollection.Count; i++)
                {
        			trace.SetPinSource(processNode.arcCollection[i]);
        		}
        		
        		trace.SetLine(new LineStruct(new PairInt(Params.leftEdge, Params.VccPosition), 
        		                             new ContactSimple(wide - 1, Params.VccPosition, middleLine)),
        		                             processNode.name, curPrior, 0, true);
        	}
        }
        
        /*
        private void SetPriority(TraceGlobe trace)
        {
            List<NodeDistanceBase> lst = nodeDistanceDict.Values.ToList();
            
            lst.Sort(NodeDistanceBase.CompareBaseByDist);
            int countNd = lst.Count;
            
            int curPrior = 99;

            foreach (NodeDistanceBase disBase in lst)
            {
            	processNode = nodeList.Find(element => element.name == disBase.name);
            	List<int> priorites = nodeDistanceDict[processNode.name].GetPriorites(processNode.arcCollection.Count, curPrior);
                if (priorites.Count > 0)
            	    curPrior = priorites.Last() - 1;
                List<int> idxPass = new List<int>();
            	for (int i = 0; i < processNode.arcCollection.Count; i++)
                {
            		int idx = nodeDistanceDict[disBase.name].GetNumber(i);
            		Contact cnt = processNode.arcCollection[i];
            		
            		if ( idxPass.FindIndex(el => el == idx) < 0 )
            		{
            			idxPass.Add(idx);
            			trace.SpreadWave(new ContactSimple(processNode.arcCollection[idx]));
            		}
                }
            }
        }*/
        public void AddContactsFromPRN(string inName)
	    {
        	string fileName = inName + "_SMALL.prn";
	    	if (!System.IO.File.Exists(fileName))
	    	{
	    		Console.WriteLine("No extra file with contacts");
	    		return;
	    	}
	    	List<ContactNamed> allCotacts = new List<ContactNamed>();
	    	
	    	System.Text.Encoding enc = System.Text.Encoding.GetEncoding(1251);
	    	string[] fileStrings = System.IO.File.ReadAllLines(fileName, enc);
	    	
	    	List<LineStruct> borders = FindCellBorder(fileStrings);
	    	List<ContactNamed> namedCotacts = FindInputOutput(fileStrings, borders);
	    	
	    	trace.AddNamedPins(namedCotacts);
        }
        
        public List<ContactNamed> FindInputOutput(string[] fileStrings, List<LineStruct> inBorder)
	    {
	    	List<ContactNamed> allCotacts = new List<ContactNamed>();
	    	for(int i = 0; i < fileStrings.Length; i++)
	    	{
	    		if (fileStrings[i].IndexOf("СПИСОК ВЕКТОРНЫХ ТЕКСТОВ") >= 0)
	    		{
	    			allCotacts.AddRange(FindVectorText(fileStrings, i));
	    		}
	    	}
	    	
	    	List<ContactNamed> namedCotacts = new List<ContactNamed>();
	    	foreach (LineStruct line in inBorder)
	    	{
	    		foreach(ContactNamed cnt in allCotacts)
	    		{
	    			if (line.IntersectsWithPoint(cnt.x, cnt.y, Material.b1_))
	    				namedCotacts.Add(cnt);
	    		}
	    	}
	    	
	    	return namedCotacts;
	    }
	    
	    private List<ContactNamed> FindVectorText(string[] fileStrings, int idx)
	    {
	    	List<ContactNamed> namedCotacts = new List<ContactNamed>();
	    	for(int i = idx; i < fileStrings.Length; )
	    	{
	    		int startIdx = fileStrings[i].IndexOf("name=");// + 5;
	    		if (startIdx >= 0)
	    		{
	    			startIdx += 5;
	    			int endIdx = fileStrings[i].IndexOf(" ", startIdx);
	    			string name = fileStrings[i].Substring(startIdx, endIdx - startIdx);
	    			
	    			int typeSt = fileStrings[i + 1].IndexOf("sl=");
	    			if (typeSt >= 0)
	    			{
	    				int typeEn = fileStrings[i + 1].IndexOf("_");
	    				typeSt += 3;
	    				string type = fileStrings[i + 1].Substring(typeSt, typeEn - typeSt);
	    				if (type[0] == 'T')
	    					type = type.Substring(1);
	    				
		    			
	    				for (int k = i + 1; k < fileStrings.Length; )
	    				{
		    				int coordXst = fileStrings[k].IndexOf("(BASE_CRD,");
		    				if (coordXst >= 0)
		    				{
			    				coordXst += 10;
			    				int coordXen = fileStrings[k].IndexOf(",", coordXst);
			    				int coordYen = fileStrings[k].IndexOf(")", coordXen);
			    				
			    				string coordXstr = fileStrings[k].Substring(coordXst, coordXen - coordXst);
			    				string coordYstr = fileStrings[k].Substring(coordXen + 1, coordYen - coordXen - 1);
			    				
			    				double coordX = double.Parse(coordXstr.Replace('.', ','));
			    				double coordY = double.Parse(coordYstr.Replace('.', ','));
			    				
			    				int ndxSecond = trace.GetNodeList().FindIndex(element => element.name == name);
				            	if (ndxSecond >= 0 || type == Params.b1Type)
				            	{
				            		ContactNamed cnt = new ContactNamed((int)(coordX * 2.0), (int)(coordY * 2.0), Material.FromStrToNumber(type),
				            		                                   name);
				            		//cnt.SetInOut();
				            		namedCotacts.Add(cnt);
				        			//nodeList[ndxSecond].AddContact(cnt);
				            	}
				            	k++;
				            	i++;
		    				}
		    				else if (fileStrings[k].IndexOf("СПИСОК") >= 0 )
		    					return namedCotacts;
		    				else if ( fileStrings[k].IndexOf("name=") >= 0)
		    				{
		    					i = k;
		    					break;
		    				}
		    				else
		    				{
		    					k++;
		    					i++;
		    				}
	    				}
	    			}
	    		}
	    		else
	    			i++;
	    		//int endIdx = fileStrings[i].IndexOf(" ", startIdx);
	    		// TM1(0.00, 26.50, "VCC");
	    		
	    		//if (fileStrings[i].IndexOf("СПИСОК ВЕКТОРНЫХ ТЕКСТОВ") >= 0)
	    		//{}
	    	}
	    	return namedCotacts;
	    }
	    
	    private List<LineStruct> FindCellBorder(string[] fileStrings)
	    {
	    	List<ContactNamed> namedCotacts = new List<ContactNamed>();
	    	
	    	for(int i = 0; i < fileStrings.Length; i++)
	    	{
	    		if (fileStrings[i].IndexOf("СПИСОК ШИН") >= 0)
	    		{
	    			namedCotacts.AddRange(FindVectorText(fileStrings, i));
	    		}
	    	}
	    	
	    	
	    	List<ContactNamed> contactB1 = new List<ContactNamed>();
	    	foreach (ContactNamed cnt in namedCotacts)
	    	{
	    		if (cnt.layer == Material.b1_)
	    			contactB1.Add(cnt);
	    	}
	    	
	    	List<LineStruct> borderLines = new List<LineStruct>();
	    	
	    	if (contactB1.Count < 4)
	    		return borderLines;
	    	
	    	for(int i = 0; i < 3; i++)
	    	{
    			borderLines.Add( new LineStruct( contactB1[i], contactB1[i + 1], Material.b1_) );
	    	}
	    	borderLines.Add( new LineStruct( contactB1[0], contactB1[3], Material.b1_) );
	    	
	    	return borderLines;
	    }
	    
		public void CreateFile(string inPath, string nameFrag)
		{
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(inPath))
        	{
                file.WriteLine("#include \"stdafx.h\" ");
                file.WriteLine("layout& SUM(void)");
                file.WriteLine("{");
                file.WriteLine("    FRAG(" + nameFrag + ")");
                file.WriteLine("    COMPACTION_MODE(X_Y)");
                //file.WriteLine("    // Транзисторы");
                CultureInfo  culture = CultureInfo.CreateSpecificCulture("en-CA");
                double coordX = ((double)Params.leftBorder + 1.0) / 2.0;// 2.0;//4.0
                double coordY = ((double)Params.lineP + 1.0) / 2.0;
                foreach (string str in placedP)
                {
                    if (str != "")
                        file.WriteLine("    W(1.00) L(1.00) OR(SOUTH) SIMM TP(" + coordX.ToString("F2", culture) +
                            ", " + coordY.ToString("F2", culture) + ");");
                    coordX += 1.0;//2.0
                }
                coordX = ((double)Params.leftBorder + 1.0) / 2.0;
                coordY = ((double)Params.lineN - 1.0) / 2.0;
                foreach (string str in placedN)
                {
                    if (str != "")
                        file.WriteLine("    W(1.00) L(1.00) OR(NORTH) SIMM TN(" + coordX.ToString("F2", culture) + 
                            ", " + coordY.ToString("F2", culture) + ");");
                    coordX += 1.0;
                }
                
                //file.WriteLine("    // Шины, линии");
                foreach (NodeTraces cnt in nodeConnectionDict.Values)
                {
                	for (int i = 0; i < cnt.lines.Count; i++)//each (LineStruct ln in cnt.lines)
                    {
                    	if (cnt.lines[i].Length() > 0)
                    	{
	                    	coordX = ((double)cnt.lines[i].Left) / 2.0;
	                		coordY = ((double)cnt.lines[i].Top) / 2.0 + CorrectTop(cnt.lines[i]);

	                    	string moving = "";
	                    	double coordNext = 0.0;
	                    	if (cnt.lines[i].Height == 0)
	                    	{
	                    		moving = "X";
	                    		coordNext = ((double)cnt.lines[i].Right) / 2.0;
	                    	}
	                    	else
	                    	{
	                    		moving = "Y";
	                    		coordNext = ((double)cnt.lines[i].Bottom) / 2.0 + CorrectBottom(cnt.lines[i]);
	                    	}
	                    	
	                    	//for (int j = i; j <  cnt
	                    	//if (ln.//IntersectsPointsLines
	                    	/*if ( ( (cnt.name == Params.GndName) || (cnt.name == Params.VccName) ) && 
	                    	    (ln.type == Params.met1Type) && (ln.Width == 0) )
	                    	{
	                    		file.WriteLine("    W_WIRE(-2.00) " + ln.type + "(" + 
	                    	               coordX.ToString("F2", culture) + ", " + coordY.ToString("F2", culture) + 
	                                 ") " + moving + "(" + coordNext.ToString("F2", culture) + ");");
	                    	}
	                    	else*/
	                    	file.WriteLine("    W_WIRE(" + GetLineW(cnt.lines[i]) + ") " + Params.DefineMaterial(cnt.lines[i].type) + "(" +
	                    	               coordX.ToString("F2", culture) + ", " + coordY.ToString("F2", culture) + 
	                                 ") " + moving + "(" + coordNext.ToString("F2", culture) + ");");
	                    	
	                    	/*
	                    	if ((i + 1 < cnt.lines.Count) && (cnt.lines[i].type != cnt.lines[i + 1].type))
	                    	{
                    			List<ContactSimple> inters = cnt.lines[i].IntersectsPointsLines(cnt.lines[i+1]);
                    			if (inters.Count > 0)
                    			{
	                    			string cntType = Params.DefineMaterial(cnt.lines[i].type, cnt.lines[i+1].type);
	                    			int idx = inters.FindIndex(el => trace.IsPointForContact(el, cntType));
                    			
	                    			if (idx >= 0)
	                    			{
	                    				coordX = ((double)inters[idx].x) / 2.0;
	                    				coordY = ((double)inters[idx].y) / 2.0;
	                    				file.WriteLine("    " + cntType + "(" + coordX.ToString("F2", culture) +
	                    				               ", " + coordY.ToString("F2", culture) + ");");
	                    			}
                    			}
	                    	}*/
	                    	
                    	}
                    	else
                    		file.WriteLine("	//Error line");
                    }
                	
                	foreach (ContactSimple contactUnit in cnt.crossing)
                	{
                		string cntType = Params.DefineMaterial(contactUnit.layer);
                		coordX = ((double)contactUnit.x) / 2.0;
        				coordY = ((double)contactUnit.y) / 2.0;
        				file.WriteLine("    " + cntType + "(" + coordX.ToString("F2", culture) +
        				               ", " + coordY.ToString("F2", culture) + ");");
                	}
                	
                	//for (int i = 0; i <  
                	/*List<int> readyConnects = new List<int>();
                	foreach(int lay in Params.UsedLayers)
                	{
                		readyConnects.Add(lay);
                		foreach (int lay2 in Params.UsedLayers)
                		{
                			if (readyConnects.FindIndex(el => el == lay2) < 0)
                			{
                				foreach (LineStruct ln in cnt.lines)
                				{
                					
                				}
                			}
                		}
                	}*/
                    
                }
                
                file.WriteLine(AddKnBorder());
                file.WriteLine(AddKpBorder());
                file.WriteLine(AddB1Border());

                /*file.WriteLine("    // Контакты");
                foreach (NodeTraces cnt in nodeConnectionDict.Values)
                {
                    foreach (Contact pnt in cnt.crossing)
                    {
                    	{
	                        coordX = ((double)pnt.x) / 2.0;
	                        coordY = ((double)pnt.y) / 2.0;
	                        
	                        file.WriteLine("    " + pnt.typePoint + "(" + coordX.ToString("F2", culture) +
	                            ", " + coordY.ToString("F2", culture) + ");");
                    	}
                    }
                }*/
                    
                file.WriteLine("    ENDF");
                file.WriteLine("    return " + nameFrag + ";");
                file.WriteLine("}");
            }
		}
		
		
		public void CreateFileWithNames(string inPath, string nameFrag)
		{
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(inPath))
        	{
                file.WriteLine("#include \"stdafx.h\" ");
                file.WriteLine("layout& SUM(void)");
                file.WriteLine("{");
                file.WriteLine("    FRAG(" + nameFrag + ")");
                file.WriteLine("    COMPACTION_MODE(X_Y)");
                //file.WriteLine("    // Транзисторы");
                CultureInfo  culture = CultureInfo.CreateSpecificCulture("en-CA");
                double coordX = ((double)Params.leftBorder + 1.0) / 2.0;// 2.0;//4.0
                double coordY = ((double)Params.lineP + 1.0) / 2.0;
                foreach (string str in placedP)
                {
                    if (str != "")
                        file.WriteLine("    W(1.00) L(1.00) OR(SOUTH) SIMM TP(" + coordX.ToString("F2", culture) +
                            ", " + coordY.ToString("F2", culture) + ");");
                    coordX += 1.0;//2.0
                }
                coordX = ((double)Params.leftBorder + 1.0) / 2.0;
                coordY = ((double)Params.lineN - 1.0) / 2.0;
                foreach (string str in placedN)
                {
                    if (str != "")
                        file.WriteLine("    W(1.00) L(1.00) OR(NORTH) SIMM TN(" + coordX.ToString("F2", culture) + 
                            ", " + coordY.ToString("F2", culture) + ");");
                    coordX += 1.0;
                }
                
                //file.WriteLine("    // Шины, линии");
                foreach (string curName in nodeConnectionDict.Keys)
                	//NodeTraces cnt in nodeConnectionDict.Values)
                {
                	NodeTraces cnt = nodeConnectionDict[curName];
                	//TM1(0.00, 26.50, "VCC");
                	if (cnt.lines.Count > 0)
                	{
                		string layerPoint = "T" + Params.DefineMaterial(cnt.lines[0].type);
                		coordX = ((double)cnt.lines[0].Left) / 2.0;
                		coordY = ((double)cnt.lines[0].Top) / 2.0 + CorrectTop(cnt.lines[0]);
                		if (cnt.lines.Count > 1)
                		{
                			layerPoint = "T" + Params.DefineMaterial(cnt.lines[0].type);
                			coordX = ((double)cnt.lines[1].Left) / 2.0;
                			coordY = ((double)cnt.lines[1].Top) / 2.0 + CorrectTop(cnt.lines[1]);
                		}
                		file.WriteLine(layerPoint + "(" + coordX.ToString("F2", culture) + ", " + coordY.ToString("F2", culture) +
                		              ", \"" + curName + "\");");
                	}
                	
                	for (int i = 0; i < cnt.lines.Count; i++)//each (LineStruct ln in cnt.lines)
                    {
                    	if (cnt.lines[i].Length() > 0)
                    	{
	                    	coordX = ((double)cnt.lines[i].Left) / 2.0;
	                		coordY = ((double)cnt.lines[i].Top) / 2.0 + CorrectTop(cnt.lines[i]);

	                    	string moving = "";
	                    	double coordNext = 0.0;
	                    	if (cnt.lines[i].Height == 0)
	                    	{
	                    		moving = "X";
	                    		coordNext = ((double)cnt.lines[i].Right) / 2.0;
	                    	}
	                    	else
	                    	{
	                    		moving = "Y";
	                    		coordNext = ((double)cnt.lines[i].Bottom) / 2.0 + CorrectBottom(cnt.lines[i]);
	                    	}
	                    	
	                    	file.WriteLine("    W_WIRE(" + GetLineW(cnt.lines[i]) + ") " + Params.DefineMaterial(cnt.lines[i].type) + "(" +
	                    	               coordX.ToString("F2", culture) + ", " + coordY.ToString("F2", culture) + 
	                                 ") " + moving + "(" + coordNext.ToString("F2", culture) + ");");
	                    	
                    	}
                    	else
                    		file.WriteLine("	//Error line");
                    }
                	
                	foreach (ContactSimple contactUnit in cnt.crossing)
                	{
                		string cntType = Params.DefineMaterial(contactUnit.layer);
                		coordX = ((double)contactUnit.x) / 2.0;
        				coordY = ((double)contactUnit.y) / 2.0;
        				file.WriteLine("    " + cntType + "(" + coordX.ToString("F2", culture) +
        				               ", " + coordY.ToString("F2", culture) + ");");
                	}
                	
                }
                
                file.WriteLine(AddKnBorder());
                file.WriteLine(AddKpBorder());
                file.WriteLine(AddB1Border());
                    
                file.WriteLine("    ENDF");
                file.WriteLine("    return " + nameFrag + ";");
                file.WriteLine("}");
            }
		}
		
		private string AddKnBorder()
		{
			CultureInfo  culture = CultureInfo.CreateSpecificCulture("en-CA");
			double leftBorder = ((double)(Params.leftEdge - 1)) / 2.0;
			double rightBorder = ((double)(wide + 1)) / 2.0;
			double bottomBorder = ((double)(Params.lineP - 4)) / 2.0;
			double topBorder = ((double)(Params.lineP + 4)) / 2.0;
			string retStr = "    KN(" + leftBorder.ToString("F2", culture) + ", " + bottomBorder.ToString("F2" , culture) +
				") X(" + rightBorder.ToString("F2", culture) + ") Y(" + topBorder.ToString("F2", culture) + ") X(" + 
				leftBorder.ToString("F2", culture) + ");";
			return retStr;
		}
		
		private string AddKpBorder()
		{
			CultureInfo  culture = CultureInfo.CreateSpecificCulture("en-CA");
			double leftBorder = ((double)(Params.leftEdge - 1)) / 2.0;
			double rightBorder = ((double)(wide + 1)) / 2.0;
			double bottomBorder = ((double)(Params.lineN - 4 )) / 2.0;
			double topBorder = ((double)(Params.lineN + 4 )) / 2.0;
			string retStr = "    KP(" + leftBorder.ToString("F2", culture) + ", " + bottomBorder.ToString("F2" , culture) +
				") X(" + rightBorder.ToString("F2", culture) + ") Y(" + topBorder.ToString("F2", culture) + ") X(" + 
				leftBorder.ToString("F2", culture) + ");";
			return retStr;
		}
		
		private string AddB1Border()
		{
			CultureInfo  culture = CultureInfo.CreateSpecificCulture("en-CA");
			double leftBorder = ((double)(Params.leftEdge + 1)) / 2.0;
			double rightBorder = ((double)(wide - 1)) / 2.0;
			double bottomBorder = ((double)(Params.bottomEdge + 1 )) / 2.0;
			double topBorder = ((double)(Params.topEdge - 2 )) / 2.0;
			string retStr = "    B1(" + leftBorder.ToString("F2", culture) + ", " + bottomBorder.ToString("F2" , culture) +
				") X(" + rightBorder.ToString("F2", culture) + ") Y(" + topBorder.ToString("F2", culture) + ") X(" + 
				leftBorder.ToString("F2", culture) + ") Y(" + bottomBorder.ToString("F2", culture) + ");";
			return retStr;
		}
		
		private string AddAlloying(ContactSimple inCnt)
		{
			CultureInfo  culture = CultureInfo.CreateSpecificCulture("en-CA");
			double coordX = ((double)inCnt.x) / 2.0;
            double coordY = ((double)inCnt.y) / 2.0;
            
            double moveY = coordY;
            string retStr = "";
            
            /*if (inCnt.typePoint == Params.cnaType)
            {
				if ( Params.IsModelBusInEdge() )
					moveY += 0.5;
				else
					moveY -= 0.5;
				retStr = "    OR(NORTH) CENAPE(" + coordX.ToString("F2", culture) + "," + coordY.ToString("F2", culture) + 
					", " + coordX.ToString("F2", culture) + "," + moveY.ToString("F2", culture) + ");";
            }
            else
            {
				if ( Params.IsModelBusInEdge() )
					moveY -= 0.5;
				else
					moveY += 0.5;
				
				retStr = "    OR(NORTH) CEPANE(" + coordX.ToString("F2", culture) + "," + coordY.ToString("F2", culture) + 
					", " + coordX.ToString("F2", culture) + "," + moveY.ToString("F2", culture) + ");";
            }*/
            
            return retStr;
		}
		
		private double CorrectTop(LineStruct inLine)
		{
			if ( (inLine.type == Layers.siliconTrace) && (inLine.Width == 0) )
			{
				if ( (inLine.Top == (Params.lineN - 1)) || (inLine.Top == (Params.lineP - 1)) )
					return (-0.1);
			}
			return 0.0;
		}
		private double CorrectBottom(LineStruct inLine)
		{
			if ( (inLine.type == Layers.siliconTrace) && (inLine.Width == 0) )
			{
				if ( (inLine.Bottom == (Params.lineN + 1)) || (inLine.Bottom == (Params.lineP + 1)) )
					return 0.1;
			}
			return 0.0;
		}
		private string GetLineW(LineStruct inLine)
		{
			if ( (inLine.Length() <= 1)  || (inLine.type == Material.na_) || (inLine.type == Material.pa_) )
			{
				return "-2.00";
			}
			return "1.00";
		}
		
		
	}
}
