/*
 * Created by SharpDevelop.
 * User: frolkinak
 * Date: 06.06.2013
 * Time: 13:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace eulerMake
{
    /// <summary>
    /// class of transistor structure
    /// </summary>
    public class TrUnit
    {
        public string name;
        public string portG;
        public string portS;
        public string portD;

        public string oppositePort(string inPort)
        {
            if (inPort == portS)
                return portD;
            if (inPort == portD)
                return portS;
            return "";
        }

        public void ReplacePortD(string inPort)
        {
            if (inPort == portS)
            {
                string portTemp = portD;
                portD = portS;
                portS = portTemp;
            }
        }

        public void ReplacePortDS()
        {
            string portTemp = portD;
            portD = portS;
            portS = portTemp;
        }
    }

    
    /*
    public class NamedContact : Contact
    {
    	public string name;
    	public NamedContact(int inX, int inY, string inType, int inLevel, string inName) :
    		base(inX, inY, inType, inLevel)
    	{
    		name = inName;
    	}
    }*/

    

    /// <summary>
    /// class of node structure
    /// </summary>
    public class TrPeak
    {
        public string name;
        public int number;
        public List<string> arcCollection;

        public TrPeak(string inName)
        {
            name = inName;
            arcCollection = new List<string>();
        }
    }

    /// <summary>
    /// class of transistors placing
    /// </summary>
    public class Transistors
    {
        private Dictionary<string, TrUnit> transListN;
        private Dictionary<string, TrUnit> transListP;
        private Dictionary<string, TrPeak> graphN;
        private Dictionary<string, TrPeak> graphP;
        private List<Node> nodeList;
        private List<HistoryTrans> histTransN;
        
        private HistoryTrans placedTransP;
        private HistoryTrans placedTransN;
        
        public Transistors()
        {
            transListN = new Dictionary<string, TrUnit>();
            transListP = new Dictionary<string, TrUnit>();

            graphN = new Dictionary<string, TrPeak>();
            graphP = new Dictionary<string, TrPeak>();
            nodeList = new List<Node>();
            //placedTransN = new Transistors.HistoryTrans();
            //placedTransP = new Transistors.HistoryTrans();
        }

        public Dictionary<string, TrUnit> getListN()
        {
            return transListN;
        }
        public Dictionary<string, TrUnit> getListP()
        {
            return transListP;
        }

        /// <summary>
        /// Add name of node
        /// </summary>
        public void setNode(string addName)
        {
            nodeList.Add(new Node(addName));
        }

        /// <summary>
        /// Add transistor
        /// </summary>
        public void addTrans(string inName, string typeTr)
        {
            if (typeTr == "MBREAKN_NORMAL")
            {
                transListN.Add(inName, new TrUnit());
            }
            if (typeTr == "MBREAKP_NORMAL")
            {
                transListP.Add(inName, new TrUnit());
            }
        }

        /// <summary>
        /// define relation of transistor port with last node
        /// </summary>
        public void setTrans(string inName, string typePort)
        {
            if (typePort == "G" || typePort == "S"
                || typePort == "D")
            {
                if (transListN.ContainsKey(inName))
                {
                    if (typePort == "G")
                        transListN[inName].portG = nodeList[nodeList.Count - 1].name;
                    if (typePort == "S")
                        transListN[inName].portS = nodeList[nodeList.Count - 1].name;
                    if (typePort == "D")
                        transListN[inName].portD = nodeList[nodeList.Count - 1].name;
                    transListN[inName].name = inName;
                }
                if (transListP.ContainsKey(inName))
                {
                    if (typePort == "G")
                        transListP[inName].portG = nodeList[nodeList.Count - 1].name;
                    if (typePort == "S")
                        transListP[inName].portS = nodeList[nodeList.Count - 1].name;
                    if (typePort == "D")
                        transListP[inName].portD = nodeList[nodeList.Count - 1].name;
                    transListP[inName].name = inName;
                }
            }
        }

        public TrUnit GetTransN(string name)
        {
            return transListN[name];
        }


        /// <summary>
        /// creation of N node dictionary
        /// </summary>
        private Dictionary<string, TrPeak> initNodesN()
        {
            Dictionary<string, TrPeak> graph = new Dictionary<string, TrPeak>();

            foreach (KeyValuePair<string, TrUnit> element in transListN)
            {
                if (!graph.ContainsKey(element.Value.portD))
                    graph.Add(element.Value.portD, new TrPeak(element.Value.portD));
                graph[element.Value.portD].arcCollection.Add(element.Key);

                if (!graph.ContainsKey(element.Value.portS))
                    graph.Add(element.Value.portS, new TrPeak(element.Value.portS));
                graph[element.Value.portS].arcCollection.Add(element.Key);
            }
            return graph;
        }

        /// <summary>
        /// creation of P node dictionary
        /// </summary>
        private Dictionary<string, TrPeak> initNodesP()
        {
            Dictionary<string, TrPeak> graph = new Dictionary<string, TrPeak>();

            foreach (KeyValuePair<string, TrUnit> element in transListP)
            {
                if (!graph.ContainsKey(element.Value.portD))
                    graph.Add(element.Value.portD, new TrPeak(element.Value.portD));
                graph[element.Value.portD].arcCollection.Add(element.Key);

                if (!graph.ContainsKey(element.Value.portS))
                    graph.Add(element.Value.portS, new TrPeak(element.Value.portS));
                graph[element.Value.portS].arcCollection.Add(element.Key);
            }
            return graph;
        }

        /// <summary>
        /// printing list of transistors on the screen
        /// </summary>
        public void PortsReport()
        {
            Console.WriteLine("  N");
            foreach (KeyValuePair<string, TrUnit> element in transListN)
                Console.WriteLine(element.Value.name + " D: " + element.Value.portD +
                              " S: " + element.Value.portS + " G: " + element.Value.portG);
            Console.WriteLine("  P");
            foreach (KeyValuePair<string, TrUnit> element in transListP)
                Console.WriteLine(element.Value.name + " D: " + element.Value.portD +
                              " S: " + element.Value.portS + " G: " + element.Value.portG);
        }

        public void PortsLinesReport()
        {
            Console.WriteLine("N Ports G:");
            foreach (string trN in placedTransN.passedTrans)
            {
                if (trN != "")
                    Console.Write(transListN[trN].portG + " ");
                else
                    Console.WriteLine("     ");
            }
            Console.WriteLine("");
            Console.WriteLine("P Ports G:");
            foreach (string trP in placedTransP.passedTrans)
            {
                if (trP != "")
                    Console.Write(transListP[trP].portG + " ");
                else
                    Console.WriteLine("     ");
            }
            Console.WriteLine("");

            Console.WriteLine("N nodes :");
            foreach (string nodeN in placedTransN.passedNodes)
                Console.Write(nodeN + " ");
            Console.WriteLine("");
            Console.WriteLine("P nodes :");
            foreach (string nodeP in placedTransP.passedNodes)
                Console.Write(nodeP + " ");
            Console.WriteLine("");
        }

        /// <summary>
        /// export list of nodes
        /// </summary>
        public List<Node> GetNodeList()
        {
        	//nodeList.Sort(CompareByPinsCount);
            return nodeList;
        }

        
        /// <summary>
        /// export list of placed N transistors
        /// </summary>
        public List<string> GetPlacedTransN()
        {
            return placedTransN.passedTrans;
        }

        /// <summary>
        /// export list of placed P transistors
        /// </summary>
        public List<string> GetPlacedTransP()
        {
            return placedTransP.passedTrans;
        }
        
        
        private HistoryTrans GetLastLine(HistoryTrans inHist)
        {
        	int indx = inHist.passedTrans.FindLastIndex(element => element == "");
        	if (indx < 0)
        		return inHist;
        	else
        		return inHist.GetLastArray(indx);
        }
        
        public void ChooseTransNSet(int inNumber)
        {
        	placedTransN = histTransN[inNumber];
        }

        /// <summary>
        /// creation of list placed transistors
        /// </summary>
        public int PlaceTransN()
        {
            graphN = initNodesN();
            graphP = initNodesP();

            HistoryTrans oneHist = new Transistors.HistoryTrans();
            //HistoryTrans allHist = new Transistors.HistoryTrans();
            List<string> allHist = new List<string>();
            
            //allHist.Add("");
            bool continueFinding = true;
            
            HistoryTrans emptyTrans = new Transistors.HistoryTrans();
            
            //HistoryTrans lastHist;// = new Transistors.HistoryTrans();
            
            histTransN = new List<Transistors.HistoryTrans>();
            
            do
            {
            	
	            	if (histTransN.FindIndex(element => element.countPortG == transListN.Count) >= 0)
	            		continueFinding = false;
	            	
	            	if  (histTransN.Count == 0)
            		{
	            		foreach (string peakName in graphN.Keys)
	            		{
		            		oneHist = FindTransN(peakName, new List<string>());
			            	if (oneHist.countNode > 0)
			            	{
				            	oneHist.AddNode(peakName);
				            	if (transListN.Count > oneHist.countPortG)
			            		{
				            		oneHist.passedTrans.Add("");
			            		}
			            		histTransN.Add(oneHist);
			            	}
	            		}
            		}
	            	else
	            	{
	            		List<HistoryTrans> addingHistN = new List<HistoryTrans>();
	            		for (int i = 0; i < histTransN.Count; ) //(HistoryTrans histUnit in histTransN)
	            		{
	            			bool isHistUpdated = false;
	            			foreach (string peakName in graphN.Keys)
	            			{
	            				oneHist = FindTransN(peakName, histTransN[i].passedTrans);
				            	if (oneHist.countNode > 0)
				            	{
				            		HistoryTrans newHist = new HistoryTrans(histTransN[i]);
					            	newHist.AddNode(peakName);
				            		//histTransN.Add(oneHist);
				            		newHist.passedTrans.AddRange(oneHist.passedTrans);
				            		newHist.passedNodes.AddRange(oneHist.passedNodes);
				            		newHist.countNode += (oneHist.countNode);
				            		newHist.countPortG += (oneHist.countPortG);
				            		if (transListN.Count > newHist.countPortG)
				            		{
					            		newHist.passedTrans.Add("");
				            		}
				            		addingHistN.Add(newHist);
				            		isHistUpdated = true;
				            	}
	            			}
	            			if (isHistUpdated)
	            				histTransN.RemoveAt(i);
	            			else
	            				i++;
	            		}
	            		histTransN.AddRange(addingHistN);
	            	}
            }
            while ( continueFinding );
            
            for (int i = 0; i < histTransN.Count; ) //(HistoryTrans histUnit in histTransN)
    		{
            	if (histTransN[i].countPortG < transListN.Count)
    				histTransN.RemoveAt(i);
            	else
            		i++;
            }
            return histTransN.Count;
        }
        
        private HistoryTrans FindTransN(string inNode, List<string> banTrans)
        {
        	List<HistoryTrans> listHist = new List<HistoryTrans>();
        	foreach (string transN in graphN[inNode].arcCollection)
        	{
        		if (!banTrans.Exists(element => element == transN))
        		{
        			string nextNode = transListN[transN].oppositePort(inNode);
        			
        			List<string> banNewHist = new List<string>(banTrans);
        			banNewHist.Add(transN);
        			
        			HistoryTrans passedNext = FindTransN(nextNode, banNewHist);

        			passedNext.AddTrans(transN, nextNode);
        			listHist.Add(passedNext);
        		}
        	}
        	if (listHist.Count > 0)
        	{
        		HistoryTrans returnHist = listHist[0];
        		foreach (HistoryTrans hist in listHist)
        			if (returnHist.countNode < hist.countNode)
        				returnHist = hist;
        		return returnHist;
        	}
        	HistoryTrans emptyHist = new Transistors.HistoryTrans();
        	
    		return emptyHist;
        }

        
        public class HistoryTrans
        {
        	public int countNode;
        	public int countPortG;
            public int approprTrans;
        	public List<string> passedTrans;
        	public List<string> passedNodes;
        	public HistoryTrans()
        	{
        		passedNodes = new List<string>();
        		passedTrans = new List<string>();
        		countNode = 0;
        		countPortG = 0;
                approprTrans = 0;
        	}
        	public HistoryTrans(HistoryTrans inHist)
        	{
        		passedNodes = new List<string>(inHist.passedNodes);
        		passedTrans = new List<string>(inHist.passedTrans);
        		countNode = inHist.countNode;
                countPortG = inHist.countPortG;
                approprTrans = inHist.approprTrans;
        	}
        	public HistoryTrans GetLastArray(int numberTransistor)
        	{
        		int amountTrans = passedTrans.Count - numberTransistor - 1;
        		int amountNodes = amountTrans + 1;
        		int startNode = passedNodes.Count - amountNodes;
        		HistoryTrans retHist = new Transistors.HistoryTrans();
        		retHist.passedNodes.AddRange(this.passedNodes.GetRange(startNode, amountNodes));
        		retHist.passedTrans.AddRange(this.passedTrans.GetRange(numberTransistor + 1, amountTrans));
        		retHist.countNode = amountNodes;
        		retHist.countPortG = amountTrans;
        		return retHist;
        	}
        	public void AddNode(string nodeNode)
        	{
        		passedNodes.Add(nodeNode);
        		countNode++;
        	}
        	public void AddTrans(string transName, string nodeName)
        	{
        		passedNodes.Add(nodeName);
        		passedTrans.Add(transName);
        		countNode++;
        		countPortG++;
        	}
        	public void SetFirstNode(string nodeName)
        	{
        		if (passedNodes.Count > 0)
        			passedNodes[0] = nodeName;
        		else
        			passedNodes.Add(nodeName);
        	}
            public int GetCountInLine(int inNumber)
            {
            	if (inNumber < passedTrans.Count)
            	{
	                int lastNumber = passedTrans.IndexOf("", inNumber);
	                int firstNumber = passedTrans.LastIndexOf("", inNumber);
	                if ( (lastNumber <= 0) && (firstNumber <= 0) )
	                    return (passedTrans.Count);
	                if ( (firstNumber > 0) && (lastNumber < 0) )
	                    return (passedTrans.Count - firstNumber - 1);
	                if ( (firstNumber < 0) && (lastNumber >= 0) )
	                    return lastNumber;
	                return (lastNumber - firstNumber - 1);
            	}
            	return -1;
            }
            /*public string GetTransNameFromLastChain()
            {
            	int numberEmpty = passedTrans.LastIndexOf("");
            	if (numberEmpty < 0)
            		return passedTrans[0];
            	return passedTrans[numberEmpty + 1];
            }*/
        }
        
        
		public static int CompareHist(Transistors.HistoryTrans x, Transistors.HistoryTrans y)
		{
			if (x.countNode > y.countNode)
    			return -1;
    		else if (x.countNode == y.countNode)
    			return 0;
			return 1;
		}
        
        
        private HistoryTrans FindTransP( string inNodeP, int numberN, List<string> banTrans )
        {
        	List<HistoryTrans> listHist = new List<Transistors.HistoryTrans>();

            if ((numberN >= 0) && (placedTransN.passedTrans[numberN] != ""))
            {
                foreach (string trP in graphP[inNodeP].arcCollection)
                {
                    if (!banTrans.Exists(element => element == trP))
                    {
                        List<string> banP = new List<string>(banTrans);
                        banP.Add(trP);

                        string nextNodeP = transListP[trP].oppositePort(inNodeP);
                        HistoryTrans oneHist = FindTransP(nextNodeP, numberN - 1, banP);
                        oneHist.AddTrans(trP, nextNodeP);
                        

                        if ( (numberN >= 0) && //(placedTransN.passedTrans[numberN] != "") &&
                            (transListN[placedTransN.passedTrans[numberN]].portG == transListP[trP].portG) )
                        {
                            oneHist.approprTrans += 3;
                        }
                        else
                        {
                        	if ((numberN > 0) && (placedTransN.passedTrans[numberN - 1] != "") &&
                                (transListN[placedTransN.passedTrans[numberN - 1]].portG == transListP[trP].portG))
                                oneHist.approprTrans++;
                            if (((numberN + 1) < placedTransN.countPortG) && (placedTransN.passedTrans[numberN + 1] != "") &&
                                (transListN[placedTransN.passedTrans[numberN + 1]].portG == transListP[trP].portG))
                                oneHist.approprTrans++;
                        }

                        if ((numberN >= 0) &&
                            (placedTransN.passedNodes[numberN] == nextNodeP))
                        {
                            oneHist.approprTrans += 2;
                        }
                        else
                        {
                            if ((numberN > 0) &&
                                (placedTransN.passedNodes[numberN - 1] == nextNodeP) )
                                oneHist.approprTrans++;
                            if (((numberN + 1) < placedTransN.countNode) &&
                                (placedTransN.passedNodes[numberN + 1] == inNodeP) )//nextNodeP
                                oneHist.approprTrans++;
                        }

                        listHist.Add(oneHist);
                    }
                }
            }
	        	
	        	if (listHist.Count > 0)
	        	{
		        	HistoryTrans returnHist = listHist[0];
	        		foreach (HistoryTrans hist in listHist)
	        			if (hist.approprTrans > returnHist.approprTrans)//(returnHist.countNode < hist.countNode || 
	        			   // (returnHist.countNode == hist.countNode && returnHist.countPortG < hist.countPortG))
	        				returnHist = hist;
	        		return returnHist;
	        	}
	        	HistoryTrans emptyHist = new Transistors.HistoryTrans();
	        	return emptyHist;
        	//}
        }
        
        public void InitTransistors()
        {
        	for (int x = 0; x < placedTransP.countNode; x++)
            {
            	int currentX = Params.leftBorder + x*2;
            	int ndxFist = nodeList.FindIndex(element => element.name == placedTransP.passedNodes[x]);
            	if (ndxFist >= 0)
            	{
        			nodeList[ndxFist].AddContact(new ContactSimple(currentX, Params.lineP, Layers.metal1Trace));
            	}
            	
            	currentX++;
            	if (placedTransP.passedTrans.Count > x && placedTransP.passedTrans[x] != "")
            	{
            		int ndxSecond = nodeList.FindIndex(element => element.name == transListP[placedTransP.passedTrans[x]].portG);
	            	if (ndxSecond >= 0)
	            	{
	        			nodeList[ndxSecond].AddContact(new ContactSimple(currentX, Params.lineP, Layers.siliconTrace));
	            	}
            	}
            }
        	for (int x = 0; x < placedTransN.countNode; x++)
            {
            	int currentX = Params.leftBorder + x*2;
            	int ndxFist = nodeList.FindIndex(element => element.name == placedTransN.passedNodes[x]);
            	if (ndxFist >= 0)
            	{
        			nodeList[ndxFist].AddContact(new ContactSimple(currentX, Params.lineN, Layers.metal1Trace));
            	}
            	
            	currentX++;
            	if (placedTransN.passedTrans.Count > x && placedTransN.passedTrans[x] != "")
            	{
            		int ndxSecond = nodeList.FindIndex(element => element.name == transListN[placedTransN.passedTrans[x]].portG);
	            	if (ndxSecond >= 0)
	            	{
	        			nodeList[ndxSecond].AddContact(new ContactSimple(currentX, Params.lineN, Layers.siliconTrace));
	            	}
            	}
            }
        }

       
        public void PlaceTransP()
        {
        	placedTransP = new Transistors.HistoryTrans();
        	//HistoryTrans emptyHist = new Transistors.HistoryTrans();
            HistoryTrans oneHist = new Transistors.HistoryTrans();
            List<HistoryTrans> listHist = new List<Transistors.HistoryTrans>();
            int currentNumberN = placedTransN.GetCountInLine(0) - 1;//passedTrans.Count - 1;
            List<string> banP = new List<string>();

            do
            {
                listHist = new List<Transistors.HistoryTrans>();

                foreach (string nodeP in graphP.Keys)
                {
            	    oneHist = FindTransP(nodeP, currentNumberN, banP);
            	    if (oneHist.countNode > 0)
                    {
                        oneHist.AddNode(nodeP);
            		    listHist.Add(oneHist);
                    }
                }
                
                /*foreach (HistoryTrans histTrans in listHist)
        			    if ( histTrans.countNode > oneHist.countNode || 
            	        (histTrans.countNode == oneHist.countNode && histTrans.countPortG > oneHist.countPortG) )
        				    oneHist = histTrans;
                */
                if (listHist.Count > 0)
            	{
            		//allHist.AddRange(lastHist.passedTrans);
                    oneHist = listHist[0];
                    foreach (HistoryTrans histTrans in listHist)
                    {
                        if ( (histTrans.passedTrans.Count == placedTransN.GetCountInLine(currentNumberN)) &&
                        (histTrans.approprTrans > oneHist.approprTrans) )
                            oneHist = histTrans;
                    }

                    if ( placedTransP.countNode > 0 )
                        placedTransP.passedTrans.Add("");
                    placedTransP.passedTrans.AddRange(oneHist.passedTrans);
                    placedTransP.passedNodes.AddRange(oneHist.passedNodes);
                    placedTransP.countNode += (oneHist.countNode);
                    placedTransP.countPortG += (oneHist.countPortG);
                    banP.AddRange(oneHist.passedTrans);
                    //placedTransP.countPortG += (oneHist.);

                    currentNumberN += placedTransN.GetCountInLine(currentNumberN + 2) + 1;//oneHist.passedTrans.Count;
            	}
            }
            while ((listHist.Count > 0) && (currentNumberN >= 0));
        }

    }



    class Program
    {

        public static void Main(string[] args)
        {
			string edfName = "adder";//"ha_2tg";
			
			edfName = edfName.Split('.')[0];
			Scanner scanner = new Scanner(edfName + ".edf");//xor2v8_1 adder
			Parser parser = new Parser(scanner);
			parser.Parse();
			Transistors tr = parser.transList;
			
			Console.WriteLine("Transistors:");
			tr.PortsReport();
			
			DateTime start1 = System.DateTime.Now;
			
			int countVars = tr.PlaceTransN();
			
			tr.ChooseTransNSet(0);
			
			tr.PlaceTransP();

            tr.PortsLinesReport();
			
			tr.InitTransistors();

			DateTime start2 = System.DateTime.Now;
            TimeSpan sp1 = start2 - start1;
            double delta1 = ((double)((int)sp1.TotalMilliseconds)) / 1000.0;
			Console.WriteLine("placing = " + delta1 + " seconds");
           
			int curModel = 0;//Params.ModelWithDif;
			Params.SetModel(curModel);
			string vlfName = edfName + "_" + Params.GetShortName(curModel);
			CompileOneModel(vlfName, tr);
            //Console.WriteLine("Press any key to continue . . . ");
            //Console.ReadKey(true);
        }
        
        private static void CompileOneModel(string vlfName, Transistors tr)
        {
            ConstructorLayout layout = new ConstructorLayout(tr.GetPlacedTransN(), tr.GetPlacedTransP(), tr.GetNodeList());
            DateTime start2 = System.DateTime.Now;
            

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"route.txt"))
            {
	        	Params.SetAllLayersRange();
	        	layout.InitAllDistances(file);
	
	        	bool isEnd = false;
	        	int loop = 0;
	            
	            while ((!isEnd) && loop < 200)//200
	            {
	            	Console.WriteLine("step " + loop.ToString());
	            	isEnd = layout.RouteLines(file);
	            	
	            	
	            	Console.WriteLine("not routed: " + layout.UntracedCount(file));
	            	if (loop < 7)
	            		loop++;
	            	else
	            		loop++;
	            }
	            
	            for (int i = 0; i < 1; i++)
	            {
	                layout.CorrectTrace(file);
	            }
	
	            
	            layout.CreateTopFile(file);
	            layout.CreateFile(vlfName + ".cpp", vlfName);
            }
            
            DateTime end2 = System.DateTime.Now;
            TimeSpan sp2 = end2 - start2;
            double delta2 = ((double)((int)sp2.TotalMilliseconds)) / 1000.0;
            
			
			Console.WriteLine("routing = " + delta2 + " seconds");
        }
    }
}