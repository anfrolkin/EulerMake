/*
 * Created by SharpDevelop.
 * User: frolkinak
 * Date: 07.05.2014
 * Time: 13:14
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace eulerMake
{
	public class Params
    {
        //private static Params instance;

        public const int lineN = 35; //25
        public const int lineP = 19;//9
        public const int lineMiddle = 27;//17
        public const int leftBorder = 13;//3
        public const int rightBorder = 13;
        public const int maxPriority = 99;

        public const int topEdge = 54;//34
        public const int bottomEdge = 0;
        public const int leftEdge = 0;
        
        //public const int lowPositionH = 0;
        //public const int highPositionL = 16;//12
        public static int VccPosition = 0;
        public static int GndPosition = 18;
        private static int TopModel = 0;
        
        
        public const string difType = "N_P";
        public const string silType = "SI";
        public const string met1Type = "M1";
        public const string met2Type = "M2";
        public const string naType = "NA";
        public const string paType = "PA";
        public const string csiType = "CSI";
        public const string cnaType = "CNA";
        public const string cpaType = "CPA";
        public const string cmType = "CM";
        public const string b1Type = "B1";
        public const string VccName = "VCC";
        public const string GndName = "&0";
        public const string errorType = "Error type";
        

        public const int ModelBusInEdge = 0;
        public const int ModelWithDif = 1;
        public const int ModelBusM1InMiddle = 2;
        public const int ModelBusM2InMiddle = 3;
        
        public static List<int> UsedLayers = new List<int> 
        {Layers.siliconTrace, Layers.metal1Trace};//, Layers.metal2Trace};
        public static Dictionary<int, List<int>> LayersRange = new Dictionary<int, List<int>>();
        public static Dictionary<int, List<int>> allLayersRange = new Dictionary<int, List<int>>();
        
        static public bool CoverDiffus(PairInt inCont)
        {
        	if ( Math.Abs(inCont.y - lineN) <= 1 )
        		return true;
        	if ( Math.Abs(inCont.y - lineP) <= 1 )
        		return true;
        	return false;
        }

        static public bool IsDiffusArea(PairInt inCont)
        {
        	if ( Math.Abs(inCont.y - lineN) <= 3 )
        		return true;
        	if ( Math.Abs(inCont.y - lineP) <= 3 )
        		return true;
        	return false;
        }

        static public bool IsDiffusExeptionLayer(int inLayer)
        {
        	if (inLayer == Layers.siliconTrace)
        		return false;
        	if (inLayer == Layers.metal1Trace)
        		return true;
        	if (inLayer == Layers.metal2Trace) //&& !IsModelBusM2InMiddle())
        		return false;
        	return false;
        }
        /*static public ContactSimple FromSimpleToCont(Contact inCnt)
        {
        	return new ContactSimple(inCnt);//inCnt.x, inCnt.y, Layers.FromStrToNumber(inCnt.typePoint));
        }
        
        /*static public ContactSimple FromSimpleToCont(Contact inCnt)
        {
            return new Contact(inCnt.x, inCnt.y, Layers.FromStrToNumber(inCnt.typePoint));
        }*/
        static public string GetShortName(int inModel)
        {
        	if (inModel == ModelBusInEdge)
        		return "edge";
        	if (inModel == ModelWithDif)
        		return "dif";
        	if (inModel == ModelBusM1InMiddle)
        		return "midM1";
        	if (inModel == ModelBusM2InMiddle)
        		return "midM2";
        	return "";
        }
        
        static public List<int> GetOppositeLayers(int inLayer)
        {
        	List<int> retLayers = new List<int>();
        	foreach (int lay in allLayersRange[inLayer])
        		if (lay != inLayer)
        			retLayers.Add(lay);
        	return retLayers;
        }

        static public void SetModel(int inModel)
        {
        	TopModel = inModel;
            if (inModel == ModelBusInEdge)
            {
                VccPosition = bottomEdge;
                GndPosition = topEdge - 1;
            }
            if (inModel == ModelWithDif)
            {
                VccPosition = lineP;
                GndPosition = lineN;
            }
            if (inModel == ModelBusM1InMiddle)
            {
            	UsedLayers.Add(Layers.metal2Trace);
                VccPosition = Params.lineMiddle - 1;
                GndPosition = Params.lineMiddle + 1;
            }
            if (inModel == ModelBusM2InMiddle)
            {
                VccPosition = Params.lineMiddle - 1;
                GndPosition = Params.lineMiddle + 1;
            }
            
            foreach (int layer1 in UsedLayers)
            {
            	List<int> oneRange = new List<int>();
            	oneRange.Add(layer1);
            	foreach (int layer2 in UsedLayers)
            	{
            		if ((layer1 != layer2) && (DefineMaterial(layer1, layer2) != -1))
            		{
            			oneRange.Add(layer2);
            		}
            	}
            	LayersRange.Add(layer1, oneRange);
            	allLayersRange.Add(layer1, oneRange);
            }
            
            //SetAllLayersRange();
        }
        
        static public void SetAllLayersRange()
        {
        	allLayersRange.Clear();
        	List<int> allLayers = new List<int> {Layers.siliconTrace, Layers.metal1Trace, Layers.metal2Trace};
        	foreach (int layer1 in allLayers)
            {
            	List<int> oneRange = new List<int>();
            	oneRange.Add(layer1);
            	foreach (int layer2 in allLayers)
            	{
            		if ((layer1 != layer2) && (DefineMaterial(layer1, layer2) != -1))
            		{
            			oneRange.Add(layer2);
            		}
            	}
            	allLayersRange.Add(layer1, oneRange);
            }
        }
        
        /*static public string DefineMaterial(int inFirstMat, int inSecondMat)
        {
        	if ( (inFirstMat == Layers.metal1Trace && inSecondMat == Layers.siliconTrace) ||
        	    (inFirstMat == Layers.siliconTrace && inSecondMat == Layers.metal1Trace) )
        		return Params.csiType;
        	if ( (inFirstMat == Layers.metal2Trace && inSecondMat == Layers.metal1Trace) ||
        	    (inFirstMat == Layers.metal1Trace && inSecondMat == Layers.metal2Trace) )
        		return Params.cmType;
        	return errorType;
        }*/
        
        static public int DefineMaterial(int inFirstMat, int inSecondMat)
        {
        	/*if ( (inFirstMat == Layers Params.naType && inSecondMat == Params.met1Type) ||
        	    (inFirstMat == Params.met1Type && inSecondMat == Params.naType) )
        		return Params.cnaType;
        	if ( (inFirstMat == Params.paType && inSecondMat == Params.met1Type) ||
        	    (inFirstMat == Params.met1Type && inSecondMat == Params.paType) )
        		return Params.cpaType;*/
        	if ( (inFirstMat == Layers.metal1Trace && inSecondMat == Layers.siliconTrace) ||
        	    (inFirstMat == Layers.siliconTrace && inSecondMat == Layers.metal1Trace) )
        		return Material.csi_;
        	if ( (inFirstMat == Layers.metal2Trace && inSecondMat == Layers.metal1Trace) ||
        	    (inFirstMat == Layers.metal1Trace && inSecondMat == Layers.metal2Trace) )
        		return Material.cm_;
        	return -1;//errorType;
        }
        
        static public string DefineMaterial(int inMaterial)
        {
        	if (inMaterial == Material.cm_)
        		return Params.cmType;
        	if (inMaterial == Material.cna_)
        		return Params.cnaType;
        	if (inMaterial == Material.cpa_)
        		return Params.cpaType;
        	if (inMaterial == Material.csi_)
        		return Params.csiType;
        	if (inMaterial == Material.na_)
        		return Params.naType;
        	if (inMaterial == Material.pa_)
        		return Params.paType;
        	if (inMaterial == Material.m1_)
        		return Params.met1Type;
        	if (inMaterial == Material.m2_)
        		return Params.met2Type;
        	if (inMaterial == Material.si_)
        		return Params.silType;
        	if (inMaterial == Material.b1_)
        		return Params.b1Type;
        	
        	return errorType;
        }
        
        static public bool IsModelWithDif()
        {
        	if (Params.TopModel == Params.ModelWithDif)
        		return true;
        	return false;
        }
        
        static public bool IsModelBusM1InMiddle()
        {
        	if (Params.TopModel == Params.ModelBusM1InMiddle)
        		return true;
        	return false;
        }
        
        static public bool IsModelBusM2InMiddle()
        {
        	if (Params.TopModel == Params.ModelBusM2InMiddle)
        		return true;
        	return false;
        }
        
        static public bool IsModelBusInEdge()
        {
        	if (Params.TopModel == Params.ModelBusInEdge)
        		return true;
        	return false;
        }
        
        static public int AllCountMatrix(int inWide)
        {
        	return 2 * inWide * topEdge; 
        }
    }

    public class Sequences
    {
    	public const int direct = 0;
    	public const int reverse = 1;
    	public const int fromMiddle = 2;
    	public const int fromEnds = 3;
    	public const int count = 4;
    }
    
    public enum TraceWay
    {
    	GetLowPoint,
    	GetHighPoint,
    	Down2,
    	Down4,
    	DownEdge,
    	DownToSecond,
    	Up2,
    	Up4,
    	UpEdge,
    	UpToSecond,
    	RightLeft,
    	ChangeType,
    	EndLine
    };
    
    public class Material
    {
        public const int diffusion = -3;
    	public const int full = -2;
    	public const int blank = -1;
    	public const int na_ = 2;
    	public const int pa_ = 5;
    	public const int si_ = 0;
    	public const int cna_ = 16;
    	public const int cpa_ = 19;
    	public const int csi_ = 22;
    	public const int m1_ = 1;
    	public const int cm_ = 24;
    	public const int m2_ = 3;
    	public const int b1_ = 17;
    	
        public const string diffusionName = "diffusion";
    	public const string fullName = "full";
    	public const string blankName = "blank";
    	
    	
    	public Material()
    	{
    	}
    	
    	
    	static public int FromStrToNumber(string inMaterial)
    	{
    		if ( (inMaterial == Params.naType) || (inMaterial == Params.paType) ||
    		    (inMaterial == Params.silType) )
    			return si_;
    		if (inMaterial == Params.met1Type)
    			return m1_;
    		if (inMaterial == Params.met2Type)
    			return m2_;
    		if (inMaterial == Params.b1Type)
    			return b1_;
    		
    		return -1;
    	}
    }
    
    public class Layers
    {
    	public const int contactTrace = 2;
    	public const int siliconTrace = 0;
    	public const int metal1Trace = 1;
    	public const int metal2Trace = 3;
    	public const int count = 5;
    	
    	static public int FromStrToNumber(string inLayer)
    	{
    		if ( (inLayer == Params.naType) || (inLayer == Params.paType) ||
    		    (inLayer == Params.silType) )
    			return siliconTrace;
    		if (inLayer == Params.met1Type)
    			return metal1Trace;
    		if (inLayer == Params.met2Type)
    			return metal2Trace;
    		
    		return -1;
    	}
    	
    	static public string FromNumberToStr(int inNumber)
    	{
    		if ( inNumber == siliconTrace )
    			return Params.silType;
    		if ( inNumber == metal1Trace )
    			return Params.met1Type;
    		if ( inNumber == metal2Trace )
    			return Params.met2Type;
    		return "errorLayer";
    	}
    	
    }
	
}
