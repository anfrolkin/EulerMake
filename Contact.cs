/*
 * Created by SharpDevelop.
 * User: frolkinak
 * Date: 07.05.2014
 * Time: 11:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace eulerMake
{
	
	
	public class PairInt
    {
        public PairInt(int inX, int inY)
        {
            x = inX;
            y = inY;
        }
        public PairInt(PairInt inPair)
        {
            x = inPair.x;
            y = inPair.y;
        }
        
        public static bool operator == (PairInt in1, PairInt in2)
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
		    if ( (in1.x == in2.x) && (in1.y == in2.y) )
		    	return true;
		    return false;
        }
        public static bool operator !=(PairInt a, PairInt b)
		{
		    return !(a == b);
		}
        
        public bool IsPointNear(PairInt inCnt)
        {
        	if ( (Math.Abs(inCnt.x - x) == 1) && (inCnt.y == y) )
        		return true;
        	if ( (Math.Abs(inCnt.y - y) == 1) && (inCnt.x == x) )
        		return true;
        	if ( (inCnt.x == x) && (inCnt.y == y) )
        		return true;
        	return false;
        }
        
        public List<PairInt> GetBigArround(int inWide)
        {
        	List<PairInt> retList = new List<PairInt>();
            if (y < Params.topEdge - 1)
                retList.Add(new PairInt(x, y + 1));
            if (x < inWide - 1 && y < Params.topEdge - 1)
                retList.Add(new PairInt(x + 1, y + 1));
        	if (x < inWide - 1)
                retList.Add(new PairInt(x + 1, y));
        	if (x < inWide - 1 && y > 0)
                retList.Add(new PairInt(x + 1, y - 1));
        	if (y > 0)
                retList.Add(new PairInt(x, y - 1));
        	if (x > 0 && y > 0)
                retList.Add(new PairInt(x - 1, y - 1));
        	if (x > 0)
                retList.Add(new PairInt(x - 1, y));
        	if (x > 0 && y < Params.topEdge - 1)
                retList.Add(new PairInt(x - 1, y + 1));
        	if (y < Params.topEdge - 2)
                retList.Add(new PairInt(x, y + 2));
        	if (x < inWide - 2)
                retList.Add(new PairInt(x + 2, y));
        	if (y > 1)
                retList.Add(new PairInt(x, y - 2));
        	if (x > 1)
                retList.Add(new PairInt(x - 2, y));
        	return retList;
        }

        public List<PairInt> GetArroundPoints(int inWide)
        {
            List<PairInt> retList = new List<PairInt>();
            if (x < inWide - 1)
                retList.Add(new PairInt(x + 1, y));
            if (x > 0)
                retList.Add(new PairInt(x - 1, y));
            if (y < Params.topEdge - 1)
                retList.Add(new PairInt(x, y + 1));
            if (y > 0)
                retList.Add(new PairInt(x, y - 1));

            return retList;
        }
        
        public List<PairInt> GetAnyNeborPoints()
        {
            List<PairInt> retList = new List<PairInt>();
        
            retList.Add(new PairInt(x, y + 1));
            retList.Add(new PairInt(x + 1, y + 1));
            retList.Add(new PairInt(x + 1, y));
            retList.Add(new PairInt(x + 1, y - 1));
            retList.Add(new PairInt(x, y - 1));
            retList.Add(new PairInt(x - 1, y - 1));
            retList.Add(new PairInt(x - 1, y));
            retList.Add(new PairInt(x - 1, y + 1));
    	
        	return retList;
        }
        
        // Compares by x positions
        public static int CompareByX(PairInt in1, PairInt in2)
		{
        	if (in1.x > in2.x)
    			return -1;
        	else if (in1.x == in2.x)
    			return 0;
			return 1;
		}

        public int x;
        public int y;
    }
    
       /* public static bool operator == (Contact in1, Contact in2)
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
		    if ( (in1.x == in2.x) && (in1.y == in2.y) )
		    	return true;
		    return false;
        }
        public static bool operator !=(Contact a, Contact b)
		{
		    return !(a == b);
		}
        
        public Contact GetHigherPoint(int shift)
        {
        	//PairInt retPoint = new PairInt(position.typePoint, position.x, position.y + 1 + shift);
        	Contact higherCont = new Contact(this);
        	higherCont.y += (shift + 1);
        	return higherCont;
        }
        
        public Contact GetLowerPoint(int shift)
        {
        	//PairInt retPoint = new PairInt(position.typePoint, position.x, position.y - 1 - shift);
        	Contact lowerCont = new Contact(this);
        	lowerCont.y -= (shift + 1);
        	return lowerCont;
        }
        
        public Contact GetVerticalPosition(int inY)
        {
        	Contact retCont = new Contact(this);
        	retCont.y = inY;
        	return retCont;
        }
        
        public Contact GetHorizontPosition(int inX)
        {
        	Contact retCont = new Contact(this);
        	retCont.x = inX;
        	return retCont;
        }
        
        public int DifferenceX(Contact cntIn)
		{
			return Math.Abs( x - cntIn.x );
		}
        
        
        
        public List<Contact> GetArroundPoints(int inWide)
        {
        	List<Contact> retList = new List<Contact>();
            foreach (PairInt cnt in base.GetArroundPoints(inWide))
                retList.Add(new Contact(cnt, typePoint));
        	
        	
        	return retList;
        }
        
    }*/
    
    
    public class ContactSimple : PairInt
    {
    	public ContactSimple(int inX, int inY, int inLayer) : base(inX, inY)
        {
            layer = inLayer;
            inOut = false;
        }
    	
    	public ContactSimple(PairInt inPosition, int inLayer) : base(inPosition)
        {
            layer = inLayer;
            inOut = false;
        }

    	public ContactSimple(ContactSimple inCont) : base(inCont)
    	{
    		layer = inCont.layer;
    		inOut = inCont.inOut;
    	}
    	
    	public ContactSimple(ContactSimple inCont, int inLayer) : base(inCont)
    	{
    		layer = inLayer;
    		inOut = inCont.inOut;
    	}

        /*public ContactSimple(Contact inCont) : base(inCont.x, inCont.y)
        {
            layer = Layers.FromStrToNumber(inCont.typePoint);
            inOut = inCont.isInOut();
        }*/
    	
    	public void SetInOut()
    	{
    		inOut = true;
    	}
    	
    	public bool isInOut()
    	{
    		return inOut;
    	}
        
        public int layer;
        private bool inOut;
        
        public static bool operator == (ContactSimple in1, ContactSimple in2)
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
		    if ( (in1.x == in2.x) && (in1.y == in2.y) && (in1.layer == in2.layer) )
		    	return true;
		    return false;
        }
        public static bool operator !=(ContactSimple a, ContactSimple b)
		{
		    return !(a == b);
		}
        
        public ContactSimple GetHigherPoint(int shift)
        {
        	ContactSimple higherCont = new ContactSimple(this);
        	higherCont.y += (shift + 1);
        	return higherCont;
        }
        
        public ContactSimple GetLowerPoint(int shift)
        {
        	ContactSimple lowerCont = new ContactSimple(this);
        	lowerCont.y -= (shift + 1);
        	return lowerCont;
        }
        
        public ContactSimple GetVerticalPosition(int inY)
        {
        	ContactSimple retCont = new ContactSimple(this);
        	retCont.y = inY;
        	return retCont;
        }
        
        public ContactSimple GetHorizontPosition(int inX)
        {
        	ContactSimple retCont = new ContactSimple(this);
        	retCont.x = inX;
        	return retCont;
        }
        
        public int DifferenceX(ContactSimple cntIn)
		{
			return Math.Abs( x - cntIn.x );
		}
        
        // Compares by x positions
        public static int CompareByX(ContactSimple in1, ContactSimple in2)
		{
        	if (in1.x > in2.x)
    			return -1;
        	else if (in1.x == in2.x)
    			return 0;
			return 1;
		}
        
        public List<ContactSimple> GetArroundPoints(int inWide)
        {
            List<ContactSimple> retList = new List<ContactSimple>();
            foreach (PairInt cnt in base.GetArroundPoints(inWide))
                retList.Add(new ContactSimple(cnt, layer));
        	/*if (x < inWide - 1)
        		retList.Add(new ContactSimple(x + 1, y, layer));
        	if (x > 0)
                retList.Add(new ContactSimple(x - 1, y, layer));
        	if (y < Params.topEdge - 1)
                retList.Add(new ContactSimple(x, y + 1, layer));
        	if (y > 0)
                retList.Add(new ContactSimple(x, y - 1, layer));*/
        	
        	return retList;
        }
        
        public List<ContactSimple> GetNeborPoints(int inWide)
        {
            List<ContactSimple> retList = new List<ContactSimple>();
            if (y < Params.topEdge - 1)
                retList.Add(new ContactSimple(x, y + 1, layer));
            if (x < inWide - 1 && y < Params.topEdge - 1)
                retList.Add(new ContactSimple(x + 1, y + 1, layer));
        	if (x < inWide - 1)
                retList.Add(new ContactSimple(x + 1, y, layer));
        	if (x < inWide - 1 && y > 0)
                retList.Add(new ContactSimple(x + 1, y - 1, layer));
        	if (y > 0)
                retList.Add(new ContactSimple(x, y - 1, layer));
        	if (x > 0 && y > 0)
                retList.Add(new ContactSimple(x - 1, y - 1, layer));
        	if (x > 0)
                retList.Add(new ContactSimple(x - 1, y, layer));
        	if (x > 0 && y < Params.topEdge - 1)
                retList.Add(new ContactSimple(x - 1, y + 1, layer));
        	
        	return retList;
        }
        
        public List<ContactSimple> GetAnyNeborPoints()
        {
            List<ContactSimple> retList = new List<ContactSimple>();
        
            retList.Add(new ContactSimple(x, y + 1, layer));
            retList.Add(new ContactSimple(x + 1, y + 1, layer));
            retList.Add(new ContactSimple(x + 1, y, layer));
            retList.Add(new ContactSimple(x + 1, y - 1, layer));
            retList.Add(new ContactSimple(x, y - 1, layer));
            retList.Add(new ContactSimple(x - 1, y - 1, layer));
            retList.Add(new ContactSimple(x - 1, y, layer));
            retList.Add(new ContactSimple(x - 1, y + 1, layer));
    	
        	return retList;
        }
        
        static public List<VariantTrace> GetConnectionPoints(ContactSimple inCnt1, ContactSimple inCnt2, int wide)
        {
        	List<VariantTrace> retList = new List<VariantTrace>();
        	
        	
        	if (inCnt1.x == inCnt2.x)
        	{
        		VariantTrace trace1 = new VariantTrace();
        		trace1.pnt.Add(new ContactSimple(inCnt1.x, ((inCnt1.y + inCnt2.y) / 2), inCnt1.layer));
        		retList.Add(trace1);
        		
        		//if (inCnt1.x - 1 >= 0)
        		{
	        		VariantTrace trace2 = new VariantTrace();
	        		trace2.pnt.Add(new ContactSimple(inCnt1.x - 1, inCnt1.y, inCnt1.layer));
	        		trace2.pnt.Add(new ContactSimple(inCnt1.x - 1, ((inCnt1.y + inCnt2.y) / 2), inCnt1.layer));
	        		trace2.pnt.Add(new ContactSimple(inCnt1.x - 1, inCnt2.y, inCnt1.layer));
	        		retList.Add(trace2);
        		}
        		
        		//if (inCnt1.x + 1 < wide)
        		{
	        		VariantTrace trace3 = new VariantTrace();
	        		trace3.pnt.Add(new ContactSimple(inCnt1.x + 1, inCnt1.y, inCnt1.layer));
	        		trace3.pnt.Add(new ContactSimple(inCnt1.x + 1, ((inCnt1.y + inCnt2.y) / 2), inCnt1.layer));
	        		trace3.pnt.Add(new ContactSimple(inCnt1.x + 1, inCnt2.y, inCnt1.layer));
	        		retList.Add(trace3);
        		}
        		
        		return retList;
        	}
        	
        	if (inCnt1.y == inCnt2.y)
        	{
        		VariantTrace trace1 = new VariantTrace();
        		trace1.pnt.Add(new ContactSimple( ((inCnt1.x + inCnt2.x) / 2), inCnt1.y, inCnt1.layer ) );
        		retList.Add(trace1);
        		
        		//if (inCnt1.y - 1 >= 0)
        		{
	        		VariantTrace trace2 = new VariantTrace();
	        		trace2.pnt.Add(new ContactSimple(inCnt1.x, inCnt1.y - 1, inCnt1.layer));
	        		trace2.pnt.Add(new ContactSimple(((inCnt1.x + inCnt2.x) / 2), inCnt1.y - 1, inCnt1.layer));
	        		trace2.pnt.Add(new ContactSimple(inCnt2.x, inCnt1.y - 1, inCnt1.layer));
	        		retList.Add(trace2);
        		}
        		
        		//if (inCnt1.x + 1 < wide)
        		{
	        		VariantTrace trace3 = new VariantTrace();
	        		trace3.pnt.Add(new ContactSimple(inCnt1.x, inCnt1.y, inCnt1.layer));
	        		trace3.pnt.Add(new ContactSimple(((inCnt1.x + inCnt2.x) / 2), inCnt1.y, inCnt1.layer));
	        		trace3.pnt.Add(new ContactSimple(inCnt2.x, inCnt1.y, inCnt1.layer));
	        		retList.Add(trace3);
        		}
        		
        		return retList;
        	}
        	
        	if ( Math.Abs(inCnt1.x - inCnt2.x) == 1 )
        	{
        		if ( Math.Abs(inCnt1.y - inCnt2.y) == 1 )
        		{
        			VariantTrace trace1 = new VariantTrace();
	        		trace1.pnt.Add(new ContactSimple( inCnt1.x , inCnt2.y, inCnt1.layer ));
	        		retList.Add(trace1);
	        		VariantTrace trace2 = new VariantTrace();
	        		trace2.pnt.Add(new ContactSimple( inCnt2.x , inCnt1.y, inCnt1.layer ));
	        		retList.Add(trace2);
        		}
        		if ( Math.Abs(inCnt1.y - inCnt2.y) == 2 )
        		{
        			VariantTrace trace1 = new VariantTrace();
	        		trace1.pnt.Add(new ContactSimple( inCnt1.x , inCnt2.y, inCnt1.layer ));
	        		trace1.pnt.Add(new ContactSimple( inCnt1.x , ((inCnt1.y + inCnt2.y) / 2), inCnt1.layer ));
	        		retList.Add(trace1);
	        		VariantTrace trace2 = new VariantTrace();
	        		trace2.pnt.Add(new ContactSimple( inCnt2.x , inCnt1.y, inCnt1.layer ));
	        		trace2.pnt.Add(new ContactSimple( inCnt2.x , ((inCnt1.y + inCnt2.y) / 2), inCnt1.layer ));
	        		retList.Add(trace2);
	        		VariantTrace trace3 = new VariantTrace();
	        		trace3.pnt.Add(new ContactSimple( inCnt1.x , ((inCnt1.y + inCnt2.y) / 2), inCnt1.layer ));
	        		trace3.pnt.Add(new ContactSimple( inCnt2.x , ((inCnt1.y + inCnt2.y) / 2), inCnt1.layer ));
	        		retList.Add(trace3);
        		}
        	}
        	
        	if ( Math.Abs(inCnt1.x - inCnt2.x) == 2 )
        	{
        		if ( Math.Abs(inCnt1.y - inCnt2.y) == 1 )
        		{
        			VariantTrace trace1 = new VariantTrace();
	        		trace1.pnt.Add(new ContactSimple( inCnt1.x , inCnt2.y, inCnt1.layer ));
	        		trace1.pnt.Add(new ContactSimple( ((inCnt1.x + inCnt2.x) / 2), inCnt2.y, inCnt1.layer ));
	        		retList.Add(trace1);
	        		VariantTrace trace2 = new VariantTrace();
	        		trace2.pnt.Add(new ContactSimple( inCnt2.x , inCnt1.y, inCnt1.layer ));
	        		trace2.pnt.Add(new ContactSimple( ((inCnt1.x + inCnt2.x) / 2) , inCnt1.y, inCnt1.layer ));
	        		retList.Add(trace2);
	        		VariantTrace trace3 = new VariantTrace();
	        		trace3.pnt.Add(new ContactSimple( ((inCnt1.x + inCnt2.x) / 2) , inCnt1.y, inCnt1.layer ));
	        		trace3.pnt.Add(new ContactSimple( ((inCnt1.x + inCnt2.x) / 2) , inCnt2.y, inCnt1.layer ));
	        		retList.Add(trace3);
        		}
        		if ( Math.Abs(inCnt1.y - inCnt2.y) == 2 )
        		{
        			VariantTrace trace1 = new VariantTrace();
	        		trace1.pnt.Add(new ContactSimple( inCnt1.x, ((inCnt1.y + inCnt2.y) / 2), inCnt1.layer ));
	        		trace1.pnt.Add(new ContactSimple( ((inCnt1.x + inCnt2.x) / 2), ((inCnt1.y + inCnt2.y) / 2), inCnt1.layer ));
	        		trace1.pnt.Add(new ContactSimple( inCnt2.x, ((inCnt1.y + inCnt2.y) / 2), inCnt1.layer ));
	        		retList.Add(trace1);
	        		VariantTrace trace2 = new VariantTrace();
	        		trace1.pnt.Add(new ContactSimple( ((inCnt1.x + inCnt2.x) / 2), inCnt1.y, inCnt1.layer ));
	        		trace1.pnt.Add(new ContactSimple( ((inCnt1.x + inCnt2.x) / 2), ((inCnt1.y + inCnt2.y) / 2), inCnt1.layer ));
	        		trace1.pnt.Add(new ContactSimple( ((inCnt1.x + inCnt2.x) / 2), inCnt2.y, inCnt1.layer ));
	        		retList.Add(trace2);
	        		VariantTrace trace3 = new VariantTrace();
	        		trace3.pnt.Add(new ContactSimple( inCnt1.x , ((inCnt1.y + inCnt2.y) / 2), inCnt1.layer ));
	        		trace3.pnt.Add(new ContactSimple( inCnt1.x , inCnt2.y, inCnt1.layer ));
	        		trace3.pnt.Add(new ContactSimple( ((inCnt1.x + inCnt2.x) / 2), inCnt2.y, inCnt1.layer ));
	        		retList.Add(trace3);
	        		VariantTrace trace4 = new VariantTrace();
	        		trace4.pnt.Add(new ContactSimple( inCnt2.x , ((inCnt1.y + inCnt2.y) / 2), inCnt1.layer ));
	        		trace4.pnt.Add(new ContactSimple( inCnt2.x , inCnt1.y, inCnt1.layer ));
	        		trace4.pnt.Add(new ContactSimple( ((inCnt1.x + inCnt2.x) / 2), inCnt1.y, inCnt1.layer ));
	        		retList.Add(trace4);
	        		VariantTrace trace5 = new VariantTrace();
	        		trace5.pnt.Add(new ContactSimple( inCnt1.x , ((inCnt1.y + inCnt2.y) / 2), inCnt1.layer ));
	        		trace5.pnt.Add(new ContactSimple( ((inCnt1.x + inCnt2.x) / 2), ((inCnt1.y + inCnt2.y) / 2), inCnt1.layer ));
	        		trace5.pnt.Add(new ContactSimple( ((inCnt1.x + inCnt2.x) / 2), inCnt2.y, inCnt1.layer ));
	        		retList.Add(trace5);
	        		VariantTrace trace6 = new VariantTrace();
	        		trace6.pnt.Add(new ContactSimple( inCnt2.x , ((inCnt1.y + inCnt2.y) / 2), inCnt1.layer ));
	        		trace6.pnt.Add(new ContactSimple( ((inCnt1.x + inCnt2.x) / 2), ((inCnt1.y + inCnt2.y) / 2), inCnt1.layer ));
	        		trace6.pnt.Add(new ContactSimple( ((inCnt1.x + inCnt2.x) / 2), inCnt1.y, inCnt1.layer ));
	        		retList.Add(trace6);
        		}
        	}
        	return retList;
        }

        public List<ContactSimple> GetInRadius(int radius, int inWide)//ContactSimple inCnt
        {
        	List<ContactSimple> retRadius = new List<ContactSimple>();
            int curX = x + radius; //inCnt
        	int curY = y;
        	//int curY2 = inCnt.y;
        	int state = 1;
        	
        	do 
        	{
        		
        		if (state == 1)
        		{
        			curX--;
        			curY--;
        		}
        		else if (state == 2)
        		{
        			curX--;
        			curY++;
        		}
        		else if (state == 3)
        		{
        			curX++;
        			curY++;
        		}
        		else if (state == 4)
        		{
        			curX++;
        			curY--;
        		}
        		
        		if ( (curX == (x + radius)) || (curY == (y + radius)) || 
        		    (curX == (x - radius)) || (curY == (y - radius)) )
        		{
        			state++;
        		}
        		
        		if (curX > 0 && curX < inWide && curY < Params.topEdge && curY >= 0)
        			retRadius.Add(new ContactSimple(curX, curY, layer));
        	}
        	while(state < 5);
        	
        	return retRadius;
        }
        
        public ContactSimple GetInDiffusionEdge()
        {
        	if ((layer == Layers.metal1Trace) && Params.IsModelWithDif() && (y > Params.lineMiddle))
        		return new ContactSimple(x, y - 2, layer);
        	else if ((layer == Layers.metal1Trace) && Params.IsModelWithDif() && (y < Params.lineMiddle))
        		return new ContactSimple(x, y + 2, layer);
        	
        	if (y > Params.lineMiddle)
        		return new ContactSimple(x, y - 1, layer);
        	else
        		return new ContactSimple(x, y + 1, layer);
        }

        public List<ContactSimple> GetAnyInBigRadius(int inWide)
        {
            List<ContactSimple> retRadius = new List<ContactSimple>();
            int curX = x - 1;
            int curY = y - 1;
            int state = 1;

            do
            {
                if (state == 1)
                {
                    curX--;
                    state = 2;
                }
                else if (state == 2)
                {
                    curY++;
                    if (curY == y + 1)
                        state = 3;
                }
                else if (state == 3)
                {
                    curX++;
                    state = 4;
                }
                else if (state == 4)
                {
                    curY++;
                    state = 5;
                }
                else if (state == 5)
                {
                    curX++;
                    if (curX == x + 1)
                        state = 6;
                }
                else if (state == 6)
                {
                    curY--;
                    state = 7;
                }
                else if (state == 7)
                {
                    curX++;
                    state = 8;
                }
                else if (state == 8)
                {
                    curY--;
                    if (curY == y - 1)
                        state = 9;
                }
                else if (state == 9)
                {
                    curX--;
                    state = 10;
                }
                else if (state == 10)
                {
                    curY--;
                    state = 11;
                }
                else if (state == 11)
                {
                    curX--;
                    if (curX == x - 1)
                        state = 12;
                }
                else if (state == 12)
                {
                    curY++;
                    state = 13;
                }

                retRadius.Add(new ContactSimple(curX, curY, layer));
            }
            while (state < 13);

            return retRadius;
        }

        public int GetDistance(ContactSimple inCnt)
        {
            int dist = Math.Abs(inCnt.x - x);
            dist += Math.Abs(inCnt.y - y);
            return dist;
        }
        
    }
    
    public class VariantTrace
    {
    	public List<ContactSimple> pnt;
    	
    	public VariantTrace()
    	{
    		pnt = new List<ContactSimple>();
    	}
    }
    
    public class LineStruct : IComparable<LineStruct>
	{
		public int X;
		public int Y;
		private int width;
		private int height;
		private PairInt first;
		
		public int type;//string type;
		
		/*public Segment(int inX, int inY, int inWidth, int inHeight)
		{
			X = inX;
			Y = inY;
			width = inWidth;
			height = inHeight;
		}*/
		
		public LineStruct(PairInt firstPoint, ContactSimple secPoint)
		{
			first = new PairInt(firstPoint);
			type = secPoint.layer;
			if (firstPoint.y == secPoint.y)
			{
				X = firstPoint.x;
				if (firstPoint.x > secPoint.x)
					X = secPoint.x;
				Y = firstPoint.y;
				width = Math.Abs(firstPoint.x - secPoint.x);
				height = 0;
			}
			else
			{
				X = firstPoint.x;
				Y = firstPoint.y;
				if (firstPoint.y > secPoint.y)
					Y = secPoint.y;
				width = 0;
				height = Math.Abs(firstPoint.y - secPoint.y);
			}
		}
		
		public LineStruct(PairInt firstPoint, PairInt secPoint, int inType)
		{
			first = new PairInt(firstPoint);
			type = inType;
			if (firstPoint.y == secPoint.y)
			{
				X = firstPoint.x;
				if (firstPoint.x > secPoint.x)
					X = secPoint.x;
				Y = firstPoint.y;
				width = Math.Abs(firstPoint.x - secPoint.x);
				height = 0;
			}
			else
			{
				X = firstPoint.x;
				Y = firstPoint.y;
				if (firstPoint.y > secPoint.y)
					Y = secPoint.y;
				width = 0;
				height = Math.Abs(firstPoint.y - secPoint.y);
			}
		}
	/*
		public LineStruct(Contact inPoint, int inParam, bool isHorizont)
		{
			if (isHorizont)
			{
				int X = inParam;
				if (inParam > inPoint.x)
					X = inPoint.x;
				Y = inPoint.y;
				width = Math.Abs(inParam - inPoint.x);
				height = 0;
			}
			else
			{
				X = inPoint.x;
				Y = inParam;
				if (inParam > inPoint.y)
					Y = inPoint.y;
				width = 0;
				height = Math.Abs(inParam - inPoint.y);
			}
		}*/
		public int Bottom
		{
			get
			{
				return Y;
			}
		}
		public int Top
		{
			get
			{
				return Y + height;
			}
		}
		public int Right
		{
			get
			{
				return X + width;
			}
		}
		public int Left
		{
			get
			{
				return X;
			}
		}
		public int Width
		{
			get
			{
				return width;
			}
		}
		public int Height
		{
			get
			{
				return height;
			}
		}
	/*}
	public class LineStruct : Segment, IComparable<LineStruct>
	{
		public LineStruct(string inType, int inLevel)
		{
			//name = inName;
			type = inType;
			//levelY = inLevel;
		}
        //public Segment line;
		//public string name;
		
		//public int levelY;
*/
        public bool IntersectsWith(LineStruct inLine)
        {
        	if (type == inLine.type)
            {
        		if (Width == 0 && inLine.Width == 0 && (Math.Abs(X - inLine.X) <= 1))//(line.X - inLine.line.X) <= 1
                {
                    if (Bottom >= inLine.Bottom && Bottom <= inLine.Top)
                        return true;
                    if (Top >= inLine.Bottom && Top <= inLine.Top)
                        return true;
                    if (Top >= inLine.Bottom && Bottom <= inLine.Top)
                        return true;
                    if ( (Math.Abs(Top - inLine.Bottom) <= 1) || (Math.Abs(Bottom - inLine.Top) <= 1))
                        return true;
                }
        		if (Height == 0 && inLine.Height == 0 && (Math.Abs(Y - inLine.Y) <= 1))
                {
                    if (Right >= inLine.Left && Right <= inLine.Right)
                        return true;
                    if (Left >= inLine.Left && Left <= inLine.Right)
                        return true;
                    if (Right >= inLine.Right && Left <= inLine.Left)
                        return true;
                    if ( (Math.Abs(Right - inLine.Left) <= 1) || (Math.Abs(Left - inLine.Right) <= 1))
                        return true;
                }
                if (Height == 0 && inLine.Width == 0)
                {
                	if ((Right >= (inLine.X - 1)) && (Left <= (inLine.X + 1))
                	    && (inLine.Top >= (Y - 1)) && (inLine.Bottom <= (Y + 1)))
                        return true;
                }
                if (Width == 0 && inLine.Height == 0)
                {
                	if ((Top >= (inLine.Y - 1)) && (Bottom <= (inLine.Y + 1))
                	    && (inLine.Right >= (X - 1)) && (inLine.Left <= (X + 1)))
                        return true;
                }
            }

            return false;
        }
        
        public List<ContactSimple> IntersectsPointsLines(LineStruct inLine)
        {
        	List<ContactSimple> intersections = new List<ContactSimple>();
        	
        	List<ContactSimple> inCnt = inLine.GetPointArray();
        	
        	foreach (ContactSimple cnt in GetPointArray())
        	{
        		if (inCnt.FindIndex(el => (el.x == cnt.x) && (el.y == cnt.y)) >= 0)
        			intersections.Add(cnt);
        	}
        	
            return intersections;
        }
        
        public bool IntersectsWithPoint(int inX, int inY, int inLayer)
        {
        	if ( type == inLayer )
            {
        		if (Width == 0 && ( Math.Abs(X - inX) <= 1) )
                {
        			if ( ( inY > (Top + 1) ) || (inY < (Bottom - 1) ) )
                        return false;
                    return true;
                }
        		if (Height == 0 && ( Math.Abs(Y - inY) <= 1) )
                {
        			if ( ( inX > (Right + 1) ) || (inX < (Left - 1) ) )
                        return false;
                    return true;
                }
            }
            
            return false;
        }
        
        /*public bool OverlapWithPoint(Contact inCnt)//int inX, int inY, int inLevel, string inType)
        {
        	if ( (type == inCnt.typePoint) || (type == Params.naType &&
        	    inCnt.typePoint == Params.silType) || (type == Params.paType && inCnt.typePoint == Params.silType)  )
            {
        		if ( (Width == 0) && (X == inCnt.x) )
                {
        			if ( ( inCnt.y >= Bottom ) && (inCnt.y <= Top) )
                        return true;
                    return false;
                }
        		if ( (Height == 0) && (Y == inCnt.y) )
                {
        			if ( ( inCnt.x > Right ) || (inCnt.x < Left) )
                        return false;
                    return true;
                }
            }
            return false;
        }*/
        
        public bool OverlapWithPoint(int inX, int inY, int inLayer)
        {
        	if ( type == inLayer )
            {
        		if ( (Width == 0) && (X == inX) )
                {
        			if ( ( inY >= Bottom ) && (inY <= Top) )
                        return true;
                    return false;
                }
        		if ( (Height == 0) && (Y == inY) )
                {
        			if ( ( inX >=  Left) && (inX <= Right) )
                        return true;
                    return false;
                }
            }
            return false;
        }
        
        public bool OverlapWithPoint(ContactSimple inCnt)
        {
        	if ( type == inCnt.layer )
            {
        		if ( (Width == 0) && (X == inCnt.x) )
                {
        			if ( ( inCnt.y >= Bottom ) && (inCnt.y <= Top) )
                        return true;
                    return false;
                }
        		if ( (Height == 0) && (Y == inCnt.y) )
                {
        			if ( ( inCnt.x >= Right ) && (inCnt.x <= Left) )
                        return true;
                    return false;
                }
            }
            return false;
        }
        
        static public int Distance(PairInt point1, PairInt point2)
        {
            return (int)(Math.Sqrt(((point1.x - point2.x) * (point1.x - point2.x)) +
                ((point1.y - point2.y) * (point1.y - point2.y))));
        }
        
        public int Length()
        {
        	if (Height > Width)
        		return Height;
        	return Width;
        }
        
        public int CompareTo(LineStruct inLine)
        {
        	if (Length() > inLine.Length())
        		return 1;
        	if (Length() == inLine.Length())
        		return 0;
        	return -1;
        }
        
        public ContactSimple OpositePoint(ContactSimple inCont)
        {
        	ContactSimple oppositeCont = new ContactSimple(inCont);
        	oppositeCont.layer = type;
        	if (Height > 0)
        	{
        		if (inCont.y != Top)
        			oppositeCont.y = Top;
        		else
        			oppositeCont.y = Bottom;
        	}
        	else
        	{
        		if (inCont.x != Left)
        			oppositeCont.x = Left;
        		else
        			oppositeCont.x = Right;
        	}
        	return oppositeCont;
        }
        
        public ContactSimple GetNearContact(ContactSimple inCont)
        {
        	ContactSimple retCnt = new ContactSimple(inCont.x, inCont.y, type);
        	if (Top == Bottom)
        	{
        		if (Left > inCont.x)
        			retCnt.x = Left;
        		else if (Right < inCont.x)
        			retCnt.x = Right;
        		retCnt.y = Top;
        	}
        	if (Left == Right)
        	{
        		//if (levelY == inCont.levelY)
        		//{
	        		if (Bottom > inCont.y)
	        			retCnt.y = Bottom;
	        		else if (Top < inCont.y)
	        			retCnt.y = Top;
	        		retCnt.x = Left;
        		/*}
        		else if (levelY == Params.levelN)
        		{
        			retCnt.position.y = Bottom;
        		}
        		else if (levelY == Params.levelP)
        		{
        			retCnt.position.y = Top;
        		}*/
        	}
        	return retCnt;
        }
        
        public List<ContactSimple> GetPointArray()
        {
        	List<ContactSimple> pointArray = new List<ContactSimple>();
        	if (Height > 0)
        	{
        		if (first.y == Y)
        		{
	        		for (int i = Y; i <= Y + Height; i++)
	        			pointArray.Add(new ContactSimple(X, i, type));
        		}
        		else
        		{
        			for (int i = Y + Height; i >= Y; i--)
	        			pointArray.Add(new ContactSimple(X, i, type));
        		}
        	}
        	else
        	{
        		if (first.x == X)
        		{
	        		for (int i = X; i <= X + Width; i++)
	        			pointArray.Add(new ContactSimple(i, Y, type));
        		}
        		else
        		{
        			for (int i = X + Width; i >= X; i--)
	        			pointArray.Add(new ContactSimple(i, Y, type));
        		}
        	}
        	return pointArray;
        }
        
	}
    
    //public class TwoConnection
    //{
    
    
    public class ContactNamed : ContactSimple
    {
    	public ContactNamed(int inX, int inY, int inLayer, string inName) : base(inX, inY, inLayer)
        {
            namePoint = inName;
        }
    	
        public string namePoint;
    }
    
}
