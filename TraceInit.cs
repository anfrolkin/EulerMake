/*
 * Created by SharpDevelop.
 * User: frolkinak
 * Date: 02.06.2014
 * Time: 15:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
		public void InitTransistors(List<string> inPlacedN, List<string> inPlacedP)
        {
            wide = inPlacedN.Count*2 + Params.leftBorder + Params.rightBorder;//+8
            criticAmountH = (wide*Params.topEdge*2)/3; // 33%
            
            layoutMap = new NodePoint[wide][][];
            for (int i = 0; i < wide; i++)
            {
            	layoutMap[i] = new NodePoint[Params.topEdge][];
                for (int j = 0; j < Params.topEdge; j++)
                {
                	layoutMap[i][j] = new NodePoint[Layers.count];
                }
            }
            
            for (int x = 0; x < wide; x++)
            {
                for (int y = 0; y < Params.topEdge; y++)
                {
                	layoutMap[x][y][Layers.contactTrace] = new NodePoint(blank);
                }
            }

            SetBlank();
            SetBlankForContact(inPlacedN, inPlacedP);
            SetBlankForSilicon(inPlacedN, inPlacedP);
        }
		
		public void InitNodesDict(Dictionary<string, NodeDistanceBase> inDict, List<Node> inNodeList)
		{
            nodeList = new List<Node>();
            foreach (Node nd in inNodeList)
            {
            	if ( (nd.arcCollection.Count > 1) || (diffusionException.FindIndex(el => el == nd.name) >= 0 && nd.arcCollection.Count > 0) )
                {
                    nodeList.Add(nd);
                }
            }

			nodeDistanceDict = inDict;
			//nodeList = inNodeList;
		}
		
		public void AddNamedPins(List<ContactNamed> inCts)
		{
        	foreach(ContactNamed cnt in inCts)
        	{
        		Node fndNode = nodeList.Find(nd => nd.name == cnt.namePoint);
        		if (diffusionException.FindIndex(name => name == cnt.namePoint) < 0 && fndNode != null)
        		{
        			ContactSimple smpl = new ContactSimple(cnt);
        			smpl.SetInOut();
        			fndNode.arcCollection.Add(smpl);
        		}
        	}
		}
		
		public List<Node> GetNodeList()
		{
			return nodeList;
		}
		
		public Node GetNodeByName(string inName)
		{
			return nodeList.Find(element => element.name == inName);
		}
		
		public void SetDiffusionExcep(List<string> inNames)
		{
			diffusionException = inNames;
		}
		
		private void SetBlankForContact(List<string> inPlacedN, List<string> inPlacedP)
        {
        	int coordX = Params.leftBorder;
            int coordY = Params.lineN;
                
        	foreach (string strName in inPlacedN)
        	{
        		if (strName != "")
		        {
        			layoutMap[coordX][coordY - 2][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX][coordY - 1][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX][coordY][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX][coordY + 1][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX][coordY + 2][Layers.contactTrace] = new NodePoint(diffusion);
        			layoutMap[coordX + 1][coordY - 2][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 1][coordY - 1][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 1][coordY][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 1][coordY + 1][Layers.contactTrace] = new NodePoint(diffusion);
        			layoutMap[coordX + 1][coordY + 2][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY - 2][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY - 1][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY + 1][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY + 2][Layers.contactTrace] = new NodePoint(diffusion);
		        }
	        	coordX += 2;
        	}
        	
        	coordX = Params.leftBorder;
            coordY = Params.lineP;
                
        	foreach (string strName in inPlacedP)
        	{
        		if (strName != "")
		        {
                    layoutMap[coordX][coordY - 2][Layers.contactTrace] = new NodePoint(diffusion);
        			layoutMap[coordX][coordY - 1][Layers.contactTrace] = new NodePoint(diffusion);
        			layoutMap[coordX][coordY][Layers.contactTrace] = new NodePoint(diffusion);
        			layoutMap[coordX][coordY + 1][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX][coordY + 2][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 1][coordY - 2][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 1][coordY - 1][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 1][coordY][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 1][coordY + 1][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 1][coordY + 2][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY - 2][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY - 1][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY + 1][Layers.contactTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY + 2][Layers.contactTrace] = new NodePoint(diffusion);
		        }
	        	coordX += 2;
        	}
        }

        private void SetBlankForSilicon(List<string> inPlacedN, List<string> inPlacedP)
        {
            int coordX = Params.leftBorder;
            int coordY = Params.lineN;

            foreach (string strName in inPlacedN)
            {
                if (strName != "")
                {
                    layoutMap[coordX][coordY - 2][Layers.siliconTrace] = new NodePoint(diffusion);
                    layoutMap[coordX][coordY - 1][Layers.siliconTrace] = new NodePoint(diffusion);
                    layoutMap[coordX][coordY][Layers.siliconTrace] = new NodePoint(diffusion);
                    layoutMap[coordX][coordY + 1][Layers.siliconTrace] = new NodePoint(diffusion);
                    layoutMap[coordX][coordY + 2][Layers.siliconTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY - 2][Layers.siliconTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY - 1][Layers.siliconTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY][Layers.siliconTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY + 1][Layers.siliconTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY + 2][Layers.siliconTrace] = new NodePoint(diffusion);
                }
                coordX += 2;
            }

            coordX = Params.leftBorder;
            coordY = Params.lineP;

            foreach (string strName in inPlacedP)
            {
                if (strName != "")
                {
                    layoutMap[coordX][coordY - 2][Layers.siliconTrace] = new NodePoint(diffusion);
                    layoutMap[coordX][coordY - 1][Layers.siliconTrace] = new NodePoint(diffusion);
                    layoutMap[coordX][coordY][Layers.siliconTrace] = new NodePoint(diffusion);
                    layoutMap[coordX][coordY + 1][Layers.siliconTrace] = new NodePoint(diffusion);
                    layoutMap[coordX][coordY + 2][Layers.siliconTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY - 2][Layers.siliconTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY - 1][Layers.siliconTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY][Layers.siliconTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY + 1][Layers.siliconTrace] = new NodePoint(diffusion);
                    layoutMap[coordX + 2][coordY + 2][Layers.siliconTrace] = new NodePoint(diffusion);
                }
                coordX += 2;
            }
        }

        public void SetBlank()
        {
            for (int x = 0; x < wide; x++)
            {
                for (int y = 0; y < Params.topEdge; y++)
                {
                	layoutMap[x][y][Layers.siliconTrace] = new NodePoint(blank);
                	layoutMap[x][y][Layers.metal1Trace] = new NodePoint(blank);
                    if (Params.IsModelBusM1InMiddle())
                    	layoutMap[x][y][Layers.metal2Trace] = new NodePoint(blank);
                    else
                    	layoutMap[x][y][Layers.metal2Trace] = new NodePoint(full);
                }
            }
        }
        
        public void SetLine(LineStruct inLine, string inName, int priority, int ndNumber, bool makeFixed)
        {
            int layer = inLine.type;
            NodePoint ndPoint = new NodePoint(inName, priority, ndNumber);
            if (inLine.Height > 0)
            {
                for (int y = inLine.Bottom; y <= inLine.Top; y++)
                {
                	bool isSrs = layoutMap[inLine.X][y][layer].isSource;
                	layoutMap[inLine.X][y][layer] = new NodePoint(ndPoint);
                	layoutMap[inLine.X][y][layer].isSource = isSrs;
                	layoutMap[inLine.X][y][layer].number = -1;//change!
                	layoutMap[inLine.X][y][layer].isFixed = makeFixed;
                }
            }
            if (inLine.Width > 0)
            {
                for (int x = inLine.Left; x <= inLine.Right; x++)
                {
                	bool isSrs = layoutMap[x][inLine.Y][layer].isSource;
                	layoutMap[x][inLine.Y][layer] = new NodePoint(ndPoint);
                	layoutMap[x][inLine.Y][layer].isSource = isSrs;
                	layoutMap[x][inLine.Y][layer].number = -1;
                	layoutMap[x][inLine.Y][layer].isFixed = makeFixed;
                }
            }
        }
        
        public void PrintMap(int inLayer, System.IO.StreamWriter inStream)
        {
        	if (inLayer == Layers.metal1Trace)
            	inStream.WriteLine("--------------ME-1-------------");
            if (inLayer == Layers.metal2Trace)
            	inStream.WriteLine("--------------ME-2-------------");
            if (inLayer == Layers.contactTrace)
            	inStream.WriteLine("--------------CONTACT-------------");
            if (inLayer == Layers.siliconTrace)
            	inStream.WriteLine("--------------SI-------------");
            
            string head = "   ";
            for (int x = 0; x < wide; x++)
        		head += AddingSpace(x.ToString()) + " ";
            inStream.WriteLine(head);
            for (int y = Params.topEdge - 1; y >= 0; y--)
            {
            	string str = Adding2Space(y.ToString()) + " ";
                for (int x = 0; x < wide; x++)
                	str += AddingSpace(layoutMap[x][y][inLayer].name) + " ";
                
                inStream.WriteLine(str);
            }
        }
        
        public void PrintNumb(int inStep, System.IO.StreamWriter inStream)
        {
        	inStream.WriteLine("________Step_" + inStep.ToString() + "________");
            inStream.WriteLine("--------------Number_Node_Met1-------------");
            
            string head = "   ";
            for (int x = 0; x < wide; x++)
        		head += AddingSpace(x.ToString()) + " ";
            inStream.WriteLine(head);
            
            for (int y = Params.topEdge - 1; y >= 0; y--)
            {
                string str = Adding2Space(y.ToString()) + " ";
                for (int x = 0; x < wide; x++)
                {
                    //str += NumberToStr6(layoutMap[x][y][Layers.metal1Trace].numberNode);//.ToString() + "     ";//"\t";
                    //ConvertInt(layoutMap[x][y][Layers.metal1Trace].number.ToString()) + "   ";
                	if (layoutMap[x][y][Layers.metal1Trace].isReplace)
                		str += "1     ";//ConvertInt(layoutMap[x][y][Layers.metal1Trace].) + "   ";
                	else
                		str += "0     ";
                }
                inStream.WriteLine(str);
            }
            
            head = "   ";
            for (int x = 0; x < wide; x++)
        		head += AddingSpace(x.ToString()) + " ";
            inStream.WriteLine(head);
            
            inStream.WriteLine("--------------Number_Node_Sil-------------");
            for (int y = Params.topEdge - 1; y >= 0; y--)
            {
                string str = Adding2Space(y.ToString()) + " ";
                for (int x = 0; x < wide; x++)
                {
                    //str += NumberToStr6(layoutMap[x][y][Layers.siliconTrace].numberNode);//.ToString() + "     ";
                        //ConvertInt(layoutMap[x][y][Layers.siliconTrace].priority) + "   ";
                    if (layoutMap[x][y][Layers.siliconTrace].isReplace)
                		str += "1     ";
                	else
                		str += "0     ";
                }
                inStream.WriteLine(str);
            }
        }
        
        
        public void PrintNumbM2(int inStep, System.IO.StreamWriter inStream)
        {
        	inStream.WriteLine("________Step_" + inStep.ToString() + "________");
            inStream.WriteLine("--------------Number_Node_Met2-------------");
            string head = "   ";
            for (int x = 0; x < wide; x++)
        		head += AddingSpace(x.ToString()) + " ";
            inStream.WriteLine(head);
            
            for (int y = Params.topEdge - 1; y >= 0; y--)
            {
                string str = Adding2Space(y.ToString()) + " ";
                for (int x = 0; x < wide; x++)
                {
                    str += NumberToStr6(layoutMap[x][y][Layers.metal2Trace].numberNode);//.ToString() + "     ";//"\t";
                    //ConvertInt(layoutMap[x][y][Layers.metal2Trace].number.ToString()) + "   ";
                	//if (layoutMap[x][y][Layers.metal2Trace].isUsed)
                	//	str += "1     ";//ConvertInt(layoutMap[x][y][Layers.metal2Trace].) + "   ";
                	//else
                	//	str += "0     ";
                }
                inStream.WriteLine(str);
            }
        }
        
        public void PrintUsed(int inStep, System.IO.StreamWriter inStream)
        {
        	inStream.WriteLine("________Step_" + inStep.ToString() + "________");
            inStream.WriteLine("--------------Number_Node_Met1-------------");
            string head = "   ";
            for (int x = 0; x < wide; x++)
        		head += AddingSpace(x.ToString()) + " ";
            inStream.WriteLine(head);
            
            for (int y = Params.topEdge - 1; y >= 0; y--)
            {
                string str = Adding2Space(y.ToString()) + " ";
                for (int x = 0; x < wide; x++)
                {
                    //str += NumberToStr6(layoutMap[x][y][Layers.metal1Trace].number);//.ToString() + "     ";//"\t";
                    //ConvertInt(layoutMap[x][y][Layers.metal1Trace].number.ToString()) + "   ";
                	if (layoutMap[x][y][Layers.metal1Trace].isUsed)
                		str += "1     ";//ConvertInt(layoutMap[x][y][Layers.metal1Trace].) + "   ";
                	else
                		str += "0     ";
                }
                inStream.WriteLine(str);
            }
            
            inStream.WriteLine("--------------Number_Node_Sil-------------");
            head = "   ";
            for (int x = 0; x < wide; x++)
        		head += AddingSpace(x.ToString()) + " ";
            inStream.WriteLine(head);
            
            for (int y = Params.topEdge - 1; y >= 0; y--)
            {
                string str = Adding2Space(y.ToString()) + " ";
                for (int x = 0; x < wide; x++)
                {
                    //str += NumberToStr6(layoutMap[x][y][Layers.siliconTrace].number);//.ToString() + "     ";
                        //ConvertInt(layoutMap[x][y][Layers.siliconTrace].priority) + "   ";
                    if (layoutMap[x][y][Layers.siliconTrace].isUsed)
                		str += "1     ";
                	else
                		str += "0     ";
                }
                inStream.WriteLine(str);
            }
        }

        public string NumberToStr6(int inNumb)
        {
            
            string numb = inNumb.ToString();
            for (int i = numb.Length; i < 6; i++)
                numb += " ";
            return numb;
        }

        public void PrintRule(System.IO.StreamWriter inStream)
        {
            inStream.WriteLine(conflictManager.GetParameters());
        }
        
        private string ConvertInt(int inVariable)
		{
			if (inVariable < 0)
				return inVariable.ToString("D2");
			return " " + inVariable.ToString("D2");
		}
        
        private string Adding2Space(string inName)
		{
        	string retName = inName;
			if (retName.Length < 2)
			{
				for (int i = 0; i < (2 - inName.Length); i++)
					retName += " ";
			}
			else
				retName = inName.Substring(inName.Length - 2, 2);
			
			return retName;
		}
        
        private string AddingSpace(string inName)
		{
        	string retName = inName;
			if (retName.Length < 5)
			{
				//int spaceCount = 5 - retName.Count;
				for (int i = 0; i < (5 - inName.Length); i++)
					retName += " ";
			}
			else
				retName = inName.Substring(inName.Length - 5, 5);
			
			return retName;
		}
	}
}
