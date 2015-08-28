/*
 * Created by SharpDevelop.
 * User: frolkinak
 * Date: 31.10.2013
 * Time: 16:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Drawing;

namespace eulerMake
{
	/*
/*
    public class ContactStruct
    {
        public ContactStruct(PairInt inPoint, string inType, int inLevel)
        {
        	coordinate = new PairInt(inPoint);
            type = inType;
            levelY = inLevel;
        }
        public int levelY;
        public PairInt coordinate;
        public string type;
    }
	*/
	/*
	
	public class HistoryAreaConts : IComparable<HistoryAreaConts>
    {
        public HistoryAreaConts()
        {
        	isTraced = false;
            countInterfere = 0;
            interfereLines = new List<TwoContact>();
        }
        public bool isTraced;
        public int countInterfere;
        public List<TwoContact> interfereLines;
        private TwoContact replaceLine;
        
        public void SetReplaceLine(TwoContact inCnt)
        {
        	replaceLine = inCnt;
        	isTraced = true;
        }
        
        public TwoContact GetReplaceLine()
        {
        	return replaceLine;
        }
        
        // Compares by 
        public int CompareTo(HistoryAreaConts in2)//, HistoryAreaConts in2) static
		{
			if (countInterfere > in2.countInterfere)
    			return 1;
    		else if (countInterfere == in2.countInterfere)
    		{
    			if (replaceLine.Length() > in2.replaceLine.Length())
    				return 1;
    			if (replaceLine.Length() < in2.replaceLine.Length())
    				return -1;
    			return 0;
    		}
			return -1;
		}
    }
	
	public class HistoryPairs// : IComparable<HistoryAreaConts>
    {
        public HistoryPairs()
        {
        	interfereLines = new List<TwoContact>();
            allLines = new List<TwoContact>();
        }
        public List<TwoContact> interfereLines;
        public List<TwoContact> allLines;
	}
	
*/
	/*
	public class TwoContact
	{
		public TwoContact(TwoContact inCont)
		{
            firstContact = inCont.firstContact;
            secondContact = inCont.secondContact;
            name = inCont.name;
            lines = new List<LineStruct>(inCont.lines);
            crossing = new List<Contact>(inCont.crossing);
            isFixed = false;
            
            isToBus = false;
            isPin = false;
            if ( firstContact.isInOut() || secondContact.isInOut() )
            	isPin = true;
		}
		public TwoContact(Contact contact1, Contact contact2, string inName)
		{
            firstContact = contact1;
            secondContact = contact2;
            name = inName;
            lines = new List<LineStruct>();
            crossing = new List<Contact>();
            isFixed = false;
            
            isToBus = false;
            isPin = false;
            if ( firstContact.isInOut() || secondContact.isInOut() )
            	isPin = true;
		}
		
		
		public bool IsInOut()
		{
			return isPin;
		}
		public void MakeFix()
		{
			isFixed = true;
		}
		public void MakeConnectToBus()
		{
			isToBus = true;
		}
		public bool IsToBus()
		{
			return isToBus;
		}
		public bool IsFixed()
		{
			return isFixed;
		}

        public void SetLineForTwoPoint()
        {
        	if ( (DifferenceX() == 0) || (DifferenceY() == 0) )
            {
                lines.Add(new LineStruct(firstContact, secondContact));
            }
        }

		public string name;
		public Contact firstContact;
		public Contact secondContact;
		public List<LineStruct> lines;
        public List<Contact> crossing;
        private bool isFixed;
        private bool isPin;
        private bool isToBus;
		
		public void SwapContacts()
		{
			Contact tempCont = firstContact;
			firstContact = secondContact;
			secondContact = tempCont;
		}
		
		public List<Contact> GetAllContacts()
		{
			List<Contact> retList = new List<Contact>();
			foreach (LineStruct lineUnit in lines)
			{
				if (lineUnit.Height == 0)
				{
					for (int x = lineUnit.Left; x <= lineUnit.Right; x++)
						retList.Add(new Contact(x, lineUnit.Y, lineUnit.type));
				}
				else
				{
					for (int y = lineUnit.Bottom; y <= lineUnit.Bottom; y++)
						retList.Add(new Contact(lineUnit.X, y, lineUnit.type));
				}
			}
			return retList;
		}
		
		public HistoryAreaConts SetConect(TraceWay[][] inWay, List<TwoContact> inAreaList)
        {
			List<HistoryAreaConts> hist = new List<HistoryAreaConts>();
			
			for (int i = 0; i < inWay.GetLength(0); i++)
            {
				SetStepConect(inWay[i]);
        		HistoryAreaConts hist1 = FindInerfereLine(inAreaList);

        		if (hist1.countInterfere == 0)
        		{
                	return hist1;
        		}
                else
                	hist.Add(hist1);
			}
			return hist.Min();
		}
		
		public List<TwoContact> SetTestConect(TraceWay[][] inWay)
        {
			List<TwoContact> hist = new List<TwoContact>();
			
			for (int i = 0; i < inWay.GetLength(0); i++)
            {
				SetStepConect(inWay[i]);
				hist.Add(new TwoContact(this));
				secondContact.x += 4;
				firstContact.x += 4;
			}
			//firstContact.position.x += 2;
			return hist;
		}
		
		public int DifferenceX()
		{
			return Math.Abs( firstContact.x - secondContact.x );
		}
		
		public static int CompareByDifferenceX(TwoContact in1, TwoContact in2)
		{
			int dif1 = Math.Abs( in1.firstContact.x - in1.secondContact.x );
			int dif2 = Math.Abs( in2.firstContact.x - in2.secondContact.x );
			
			if (dif1 > dif2)
    			return 1;
    		else if (dif1 == dif2)
    			return 0;
			return -1;
		}

        public int DifferenceY()
        {
        	
        	return Math.Abs(firstContact.y - secondContact.y);
        }
        
        public int Length()
        {
        	int allLength = 0;
        	foreach (LineStruct lineUnit in lines)
        		allLength += lineUnit.Length();
        	return allLength;
        }

		public void SetStepConect(TraceWay[] inWay)
        {
        	CreationPath path = new CreationPath(firstContact);
            lines.Clear();
            crossing.Clear();
            bool startTrace = true;
            foreach (TraceWay way in inWay)
            {
                if (way == TraceWay.EndLine)
                    path.EndLine();
            	if (way == TraceWay.GetHighPoint && startTrace)
            	{
            		path = new CreationPath(firstContact.GetHigherPoint(0));
            		startTrace = false;
            	}
            	else if (way == TraceWay.GetLowPoint && startTrace)
            	{
            		path = new CreationPath(firstContact.GetLowerPoint(0));
            		startTrace = false;
            	}
            	else if (way == TraceWay.GetHighPoint && !startTrace)
            	{
            		path.SetPoint(secondContact.GetHigherPoint(0));
            	}
            	else if (way == TraceWay.GetLowPoint && !startTrace)
            	{
            		path.SetPoint(secondContact.GetLowerPoint(0));
            	}
            	if (way == TraceWay.Down2)
            		path.Down(2);
            	if (way == TraceWay.Down4)
            		path.Down(4);
            	if (way == TraceWay.Up2)
            		path.Up(2);
            	if (way == TraceWay.Up4)
            		path.Up(4);
            	//if (way == TraceWay.DownEdge)
            	//	path.DownTo(Params.lowPositionH);
            	if (way == TraceWay.DownToSecond)
            		path.DownTo(secondContact.y);
            	//if (way == TraceWay.UpEdge)
            	//	path.UpTo(Params.highPositionL);
            	if (way == TraceWay.UpToSecond)
            		path.UpTo(secondContact.y);
            	if (way == TraceWay.ChangeType)
            	{
            		if (path.currentType == Params.met1Type)
            			path.SetNewType(Params.silType);
            		else
            			path.SetNewType(Params.met1Type);
            	}
            	
                path.RightLeft(firstContact.x - secondContact.x);
            }
            lines.AddRange(path.GetCreatedPath());
            crossing.AddRange(path.GetCreatedCrossing());
        }
        
        
        public bool FindPlacingRestriction(int inX, int inY, string inType, int inLevel)
        {
        	foreach (LineStruct lineUnit in lines)
        	{
        		if (lineUnit.IntersectsWithPoint(inX, inY, inType))
        			return true;
        	}
        	return false;
        }
        
        public bool FindPlacingRestriction(Contact inCnt)
        {
        	foreach (LineStruct lineUnit in lines)
        	{
        		if (lineUnit.IntersectsWithPoint(inCnt.x, inCnt.y, inCnt.typePoint))
        			return true;
        	}
        	return false;
        }
        
        public bool FindPlacingOverlap(int inX, int inY, string inType, int inLevel)
        {
        	foreach (LineStruct lineUnit in lines)
        	{
        		if (lineUnit.OverlapWithPoint(inX, inY, inLevel, inType))
        		    //IntersectsWithPoint(inCnt.position.x, inCnt.position.y, inCnt.levelY, inCnt.typePoint))
        			return true;
        	}
        	return false;
        }
        
        public bool FindPlacingOverlap(Contact inCnt)
        {
        	foreach (LineStruct lineUnit in lines)
        	{
        		if (lineUnit.OverlapWithPoint(inCnt))
        		    //IntersectsWithPoint(inCnt.position.x, inCnt.position.y, inCnt.levelY, inCnt.typePoint))
        			return true;
        	}
        	return false;
        }
        
        public HistoryAreaConts FindInerfereLine(List<TwoContact> inAreaList)//, TwoContact inConts
        {
            HistoryAreaConts hist = new HistoryAreaConts();
            hist.SetReplaceLine( new TwoContact(this) );
            foreach (TwoContact trace1 in inAreaList)
            {
                bool crossing = false;
                if (name != trace1.name)
                {
	                foreach (LineStruct line1 in trace1.lines)
	                {
	                    foreach (LineStruct line2 in this.lines)
	                        if (line1.IntersectsWith(line2))//if (line1.type == line2.type && line1.line.IntersectsWith(line2.line))
	                        {
	                            crossing = true;
	                            break;
	                        }
	                    if (crossing)
	                        break;
	                }
                }
                if (crossing)
                {
                    hist.interfereLines.Add(trace1);
                    hist.countInterfere++;
                }
            }
            return hist;
        }
        
        public static bool operator == (TwoContact in1, TwoContact in2)
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
		    if ( (in1.firstContact == in2.firstContact) && (in1.secondContact == in2.secondContact) )
		    	return true;
		    return false;
        }
        public static bool operator !=(TwoContact a, TwoContact b)
		{
		    return !(a == b);
		}
        
	}*/
	
	
}