using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eulerMake
{
    class ConflictParametr
    {
        public ConflictParametr()
        {
            conflicts = new List<string>();
            numbers = new List<int>();
        }

        public ConflictParametr(ConflictParametr inParams)
        {
            conflicts = new List<string>(inParams.conflicts);
            numbers = new List<int>(inParams.numbers);
            countConfl = inParams.countConfl;
        }

        public void AddName(string inName, int inNumber)
        {
            if (conflicts.FindIndex(el => el == inName) < 0)
            {
                conflicts.Add(inName);
                numbers.Add(inNumber);
            }
        }
        
        public void DeleteName(string inName)
        {
        	int idx = conflicts.FindIndex(el => el == inName);
            if (idx >= 0)
            {
                conflicts.RemoveAt(idx);
                numbers.RemoveAt(idx);
            }
        }

        public static bool operator ==(ConflictParametr in1, ConflictParametr in2)
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
            if ( in1.countConfl == in2.countConfl )
            {
                foreach (string curName in in1.conflicts)
                    if (in2.conflicts.FindIndex(el => el == curName) < 0)
                        return false;

                return true;
            }
            return false;
        }
        public static bool operator !=(ConflictParametr a, ConflictParametr b)
        {
            return !(a == b);
        }

        public List<string> GetNames()
        {
            return conflicts;
        }

        public int GetNameCount()
        {
            return conflicts.Count;
        }

        public int GetCountConfl()
        {
            return countConfl;
        }

        public void SetCountConfl(int inCount)
        {
            countConfl = inCount;
        }

        List<string> conflicts;
        List<int> numbers;
        int countConfl;
    }

    //class Conflict

    class ConflictNote
    {
        public string name;
        public int indexNumber1;
        public int indexNumber2;

        public int number1;
        public int number2;
        public int layer1;
        public int layer2;
        
        public int distance;
        public ConflictParametr conflicts;
        public int priority;

        public int border1Met;
        public int border2Met;
        public int border1Sil;
        public int border2Sil;
        public bool compare1Met;
        public bool compare1Sil;
        public bool compare2Met;
        public bool compare2Sil;

        public int iterationNumber;

        public ConflictNote()
        {
            
        }

        /*public ConflictNote(ConflictNote inNote)
        {
            name = inNote.name;
            indexNumber1 = inNote.indexNumber1;
            indexNumber2 = inNote.indexNumber2;

            number1 = inNote.number1;
            number2 = inNote.number2;
            layer1 = inNote.layer1;
            layer2 = inNote.layer2;

            distance = inNote.distance;
            priority = inNote.priority;

            border1Met = inNote.border1Met;
            border1Sil = inNote.border1Sil;
            border2Met = inNote.border2Met;
            border2Sil = inNote.border2Sil;
            compare1Met = inNote.compare1Met;
            compare2Met = inNote.compare2Met;
            compare1Sil = inNote.compare1Sil;
            compare2Sil = inNote.compare2Sil;

            iterationNumber = 0;
        }*/

        public static bool operator ==(ConflictNote in1, ConflictNote in2)
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
            if ( (in1.name == in2.name) && (in1.number1 == in2.number1) && (in1.number2 == in2.number2) && (Math.Abs(in1.indexNumber2 - in2.indexNumber2) < 3) &&
                (Math.Abs(in1.indexNumber1 - in2.indexNumber1) < 3) && (in1.conflicts == in2.conflicts) &&
                (in1.compare1Met == in2.compare1Met) && (in1.compare1Sil == in2.compare1Sil) && (in1.compare2Met == in2.compare2Met) && 
                (in1.compare2Sil == in2.compare2Sil) && (Math.Abs(in1.border1Met - in2.border1Met) < 3) && (Math.Abs(in1.border1Sil - in2.border1Sil) < 3) &&
                (Math.Abs(in1.border2Met - in2.border2Met) < 3) && (Math.Abs(in1.border2Sil - in2.border2Sil) < 3) )
            {
                
                return true;
            }
            return false;
        }
        public static bool operator !=(ConflictNote a, ConflictNote b)
        {
            return !(a == b);
        }

        /*public List<ConflictNote> GetAllRange(ConflictNote inNote)
        {
            List<ConflictNote> variants = new List<ConflictNote>();
            ConflictNote curVar = new ConflictNote(inNote);
            curVar.compare1 = true;
            curVar.compare2 = true;
            variants.Add(curVar);
            curVar = new ConflictNote(inNote);
            curVar.compare1 = true;
            curVar.compare2 = false;
            variants.Add(curVar);
            curVar = new ConflictNote(inNote);
            curVar.compare1 = false;
            curVar.compare2 = true;
            variants.Add(curVar);
            curVar = new ConflictNote(inNote);
            curVar.compare1 = false;
            curVar.compare2 = false;
            variants.Add(curVar);
        }*/
    }

    /*class ConflictHistory
    {
        private List<string> exceptionH;
        private int numberH;
        //bool 

        public ConflictHistory(List<string> inExceptions, int inNumb)
        {
            exceptionH = inExceptions;
            numberH = inNumb;
        }

        public static bool operator ==(ConflictHistory in1, ConflictHistory in2)
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
            if ((Math.Abs(in1.numberH - in2.numberH) < 3) && (in1.exceptionH.Count == in2.exceptionH.Count))
            {
                foreach (string curName in in1.exceptionH)
                    if (in2.exceptionH.FindIndex(el => el == curName) < 0)
                        return false;
                return true;
            }
            return false;
        }
        public static bool operator !=(ConflictHistory a, ConflictHistory b)
        {
            return !(a == b);
        }
    }*/

    class ConflictPolitic
    {
        bool isTraceH;
        //int numberH;
        private int criticAmountH;
        //private List<string> exceptionH;
        //private List<string> exceptionNumbs;
        private string bestHighPrior;
        private string prevHighPrior;
        private int stepWithMax;
        
        private bool isBlockade;
        private int blockadedNumber;
        //private int currentNumber1;
        //private int currentNumber2;
        //private int currentPriority;
        //private string currentName;
        private ConflictNote currentConflict;
        private List<ConflictNote> someConflicts;
        private List<ConflictNote> conflHist;
        
        //private int numberIteration;
        //private int conditionIteration;

        public ConflictPolitic()
        {
            someConflicts = new List<ConflictNote>();

            conflHist = new List<ConflictNote>();
            bestHighPrior = "";
            prevHighPrior = "";
        }

        public string GetParameters()
        {
            if (!isTraceH)
                return "";
            string prt = "Numbers: " + currentConflict.number1.ToString() + " " + currentConflict.number2.ToString() +  " Exceptions:  ";
            foreach (string ecp in currentConflict.conflicts.GetNames())//exceptionH)
                prt += ecp + "  ";
            prt += "index = " + currentConflict.iterationNumber;
            if (currentConflict.border1Met == 1000)
            	prt += " Met1  ";
            if (currentConflict.border1Sil == 1000)
            	prt += " Sil1  ";
            if (currentConflict.border2Met == 1000)
            	prt += " Met2  ";
            if (currentConflict.border2Sil == 1000)
            	prt += " Sil2  ";
            return prt;
        }

        public void SetBestPriority(string inName)
        {

            if (inName != bestHighPrior)//!!!
            {
                stepWithMax = 0;
                isTraceH = false;
                prevHighPrior = bestHighPrior;
                conflHist.Clear();

                //conditionIteration = 0;
            }

            bestHighPrior = inName;
        }
        
        /*public bool IsLongBlockade()
        {
        	if (!isBlockade && (conflHist.Count <= 0))
        		return false;
        	int countIterationWithBl = 0;
        	foreach (ConflictNote cfl in conflHist)
        		countIterationWithBl += cfl.iterationNumber;
        	countIterationWithBl += currentConflict.iterationNumber;
        	if ((countIterationWithBl == 0) || (countIterationWithBl % 4 != 0))
        		return false;
        	return true;
        }*/
        
        public int GetBlocadedNumber()
        {
        	return blockadedNumber;
        }

        public void SetWithoutBestPriority()
        {
            isTraceH = false;
        }

        public void SetWithPriority()
        {
            if (conflHist.Count > 0)
                isTraceH = true;
        }

        public bool IsBestPriorityUsed()
        {
            return isTraceH;
        }

        public string GetBestPriority()
        {
            return bestHighPrior;
        }

        public bool IsNeedReconfig()
        {
        	if (stepWithMax >= 4)// || (isTraceH && )
            {
                isTraceH = true;
                stepWithMax = 0;
                return true;
            }
            return false;
        }

        public List<string> GetExceptions()
        {
            return currentConflict.conflicts.GetNames();//exceptionNumbs;
        }

        public void IncrementStep()
        {
            stepWithMax++;
        }

        public void ResetConnection()//int inCurNumber1, int inPriority)
        {
            //currentNumber1 = inCurNumber1;
                //currentNumber2 = inCurNumber2;
            //currentName = bestHighPrior;
            //currentPriority = inPriority;

            someConflicts.Clear();
        }
        
        public int GetCountConflicts()
        {
        	return someConflicts.Count;
        }

        public void AddConnection(NodePoint pnt1, NodePoint pnt2, int inLayer,//int idxNumber1, int idxNumber2, int inNumber2,
            int inDistance, ConflictParametr param, int count1, int count2)//check repeat
        {
            ConflictNote nt = new ConflictNote();
            nt.name = pnt1.name;
            nt.number1 = pnt1.numberNode;
            nt.number2 = pnt2.numberNode;
            nt.indexNumber1 = pnt1.number;
            nt.indexNumber2 = pnt2.number;
            nt.conflicts = param;
            nt.distance = inDistance;
            nt.priority = pnt1.priority;
            nt.iterationNumber = 0;
            isBlockade = false;
            blockadedNumber = count2;
            
            if (inLayer == Layers.metal1Trace)
            {
	            nt.border1Met = 1000;
	            nt.border1Sil = 0;
	            nt.border2Met = 0;
	            nt.border2Sil = 0;
	            nt.compare1Met = true;
	            nt.compare2Met = true;
	            nt.compare1Sil = true;
	            nt.compare2Sil = true;
            }
            else
            {
	            nt.border1Met = 0;//pnt1.number;
	            nt.border1Sil = 1000;//pnt1.number;
	            nt.border2Met = 0;//pnt2.number;
	            nt.border2Sil = 0;//pnt2.number;
	            nt.compare1Met = true;
	            nt.compare2Met = true;
	            nt.compare1Sil = true;
	            nt.compare2Sil = true;
            }
            
            if ((count1*2) < count2)
            {
            	blockadedNumber = count1;
            	if (inLayer == Layers.metal1Trace)
	            {
		            nt.border1Met = 0;
		            nt.border1Sil = 0;
		            nt.border2Met = 1000;
		            nt.border2Sil = 0;
		            nt.compare1Met = true;
		            nt.compare2Met = true;
		            nt.compare1Sil = true;
		            nt.compare2Sil = true;
	            }
	            else
	            {
		            nt.border1Met = 0;
		            nt.border1Sil = 0;
		            nt.border2Met = 0;
		            nt.border2Sil = 1000;
		            nt.compare1Met = true;
		            nt.compare2Met = true;
		            nt.compare1Sil = true;
		            nt.compare2Sil = true;
	            }
	            isBlockade = true;
            }
            
            if ((count2*2) < count1)
            	 isBlockade = true;

            someConflicts.Add(nt);
        }

        private bool SetConflictNext()
        {
            if (currentConflict.iterationNumber >= 6)
                return false;
            if (currentConflict.iterationNumber < 4)
            {
	            if (currentConflict.border1Met == 1000 && (!isBlockade))
	            {
	                currentConflict.border1Met = 0;
	                currentConflict.border1Sil = 1000;
	                currentConflict.border2Met = 0;
	                currentConflict.border2Sil = 0;
	                currentConflict.compare1Met = true;
	                currentConflict.compare2Met = true;
	                currentConflict.compare1Sil = true;
	                currentConflict.compare2Sil = true;
	                currentConflict.iterationNumber++;
	            }
	            if (currentConflict.border1Met == 1000 && isBlockade && (currentConflict.iterationNumber%2 == 0))
	            {
	            	 currentConflict.iterationNumber++;
	            }
	            if (currentConflict.border1Met == 1000 && isBlockade && (currentConflict.iterationNumber%2 == 1))
	            {
	                currentConflict.border1Met = 0;
	                currentConflict.border1Sil = 1000;
	                currentConflict.border2Met = 0;
	                currentConflict.border2Sil = 0;
	                currentConflict.compare1Met = true;
	                currentConflict.compare2Met = true;
	                currentConflict.compare1Sil = true;
	                currentConflict.compare2Sil = true;
	                currentConflict.iterationNumber++;
	            }
	            
	            else if (currentConflict.border1Sil == 1000 && (!isBlockade))
	            {
	                currentConflict.border1Met = 0;
	                currentConflict.border1Sil = 0;
	                currentConflict.border2Met = 1000;
	                currentConflict.border2Sil = 0;
	                currentConflict.compare1Met = true;
	                currentConflict.compare2Met = true;
	                currentConflict.compare1Sil = true;
	                currentConflict.compare2Sil = true;
	                currentConflict.iterationNumber++;
	            }
	            else if (currentConflict.border1Sil == 1000 && isBlockade && (currentConflict.iterationNumber%2 == 0))
	            {
	                currentConflict.iterationNumber++;
	            }
	            else if (currentConflict.border1Sil == 1000 && isBlockade && (currentConflict.iterationNumber%2 == 1))
	            {
	            	currentConflict.border1Met = 1000;
	                currentConflict.border1Sil = 0;
	                currentConflict.border2Met = 0;
	                currentConflict.border2Sil = 0;
	                currentConflict.compare1Met = true;
	                currentConflict.compare2Met = true;
	                currentConflict.compare1Sil = true;
	                currentConflict.compare2Sil = true;
	                currentConflict.iterationNumber++;
	            }
	            
	            else if ((currentConflict.border2Met == 1000) && (!isBlockade))
	            {
	                currentConflict.border1Met = 0;
	                currentConflict.border1Sil = 0;
	                currentConflict.border2Met = 0;
	                currentConflict.border2Sil = 1000;
	                currentConflict.compare1Met = true;
	                currentConflict.compare2Met = true;
	                currentConflict.compare1Sil = true;
	                currentConflict.compare2Sil = true;
	                currentConflict.iterationNumber++;
	            }
	            else if ((currentConflict.border2Met == 1000) && isBlockade && (currentConflict.iterationNumber%2 == 0))
	            {
	                currentConflict.iterationNumber++;
	            }
	            else if ((currentConflict.border2Met == 1000) && isBlockade && (currentConflict.iterationNumber%2 == 1))
	            {
	            	currentConflict.border1Met = 0;
	                currentConflict.border1Sil = 0;
	                currentConflict.border2Met = 0;
	                currentConflict.border2Sil = 1000;
	                currentConflict.compare1Met = true;
	                currentConflict.compare2Met = true;
	                currentConflict.compare1Sil = true;
	                currentConflict.compare2Sil = true;
	                currentConflict.iterationNumber++;
	            }
	            
	            else if (currentConflict.border2Sil == 1000 && (!isBlockade))
	            {
	                currentConflict.border1Met = 1000;
	                currentConflict.border1Sil = 0;
	                currentConflict.border2Met = 0;
	                currentConflict.border2Sil = 0;
	                currentConflict.compare1Met = true;
	                currentConflict.compare2Met = true;
	                currentConflict.compare1Sil = true;
	                currentConflict.compare2Sil = true;
	                currentConflict.iterationNumber++;
	            }
	            else if ((currentConflict.border2Sil == 1000) && isBlockade && (currentConflict.iterationNumber%2 == 0))
	            {
	                currentConflict.iterationNumber++;
	            }
	            else if ((currentConflict.border2Sil == 1000) && isBlockade && (currentConflict.iterationNumber%2 == 1))
	            {
	            	currentConflict.border1Met = 0;
	                currentConflict.border1Sil = 0;
	                currentConflict.border2Met = 1000;
	                currentConflict.border2Sil = 0;
	                currentConflict.compare1Met = true;
	                currentConflict.compare2Met = true;
	                currentConflict.compare1Sil = true;
	                currentConflict.compare2Sil = true;
	                currentConflict.iterationNumber++;
	            }
            }
            else if ((currentConflict.iterationNumber == 4) && (!isBlockade))
            {
                currentConflict.border1Met = 0;
                currentConflict.border1Sil = 0;
                currentConflict.border2Met = 1000;
                currentConflict.border2Sil = 1000;
                currentConflict.compare1Met = true;
                currentConflict.compare2Met = true;
                currentConflict.compare1Sil = true;
                currentConflict.compare2Sil = true;
                currentConflict.iterationNumber++;
            }
            else if (((currentConflict.border2Sil == 1000) || (currentConflict.border2Met == 1000)) && isBlockade)
            {
            	currentConflict.border1Met = 0;
                currentConflict.border1Sil = 0;
                currentConflict.border2Met = 1000;
                currentConflict.border2Sil = 1000;
                currentConflict.compare1Met = true;
                currentConflict.compare2Met = true;
                currentConflict.compare1Sil = true;
                currentConflict.compare2Sil = true;
                currentConflict.iterationNumber++;
            }
            else if (currentConflict.iterationNumber == 5  && (!isBlockade))
            {
                currentConflict.border1Met = 1000;
                currentConflict.border1Sil = 1000;
                currentConflict.border2Met = 0;
                currentConflict.border2Sil = 0;
                currentConflict.compare1Met = true;
                currentConflict.compare2Met = true;
                currentConflict.compare1Sil = true;
                currentConflict.compare2Sil = true;
                currentConflict.iterationNumber++;
            }
            else if (((currentConflict.border1Sil == 1000) || (currentConflict.border1Met == 1000)) && isBlockade)
            {
            	currentConflict.border1Met = 1000;
                currentConflict.border1Sil = 1000;
                currentConflict.border2Met = 0;
                currentConflict.border2Sil = 0;
                currentConflict.compare1Met = true;
                currentConflict.compare2Met = true;
                currentConflict.compare1Sil = true;
                currentConflict.compare2Sil = true;
                currentConflict.iterationNumber++;
            }
            return true;
        }

        public bool DefineRule()
        {
            bool isNewRule = false;
            int minConfl = -1;
            
            
            //if (conflHist.Count > 0)
            	//minConfl = currentConflict.conflicts.GetCountConfl();
            //if (currentConflict != null)
            	//minConfl
            foreach (ConflictNote nt in someConflicts)
            {
                int curIdx = conflHist.FindIndex(el => el.conflicts == nt.conflicts);
                
                if (((minConfl < 0) || (nt.conflicts.GetCountConfl() < minConfl)) && ((curIdx < 0) || (conflHist[curIdx].iterationNumber < 6)))//> minConfl
                {
                    minConfl = nt.conflicts.GetCountConfl();

                    currentConflict = nt;
                }
                /*else if (nt.conflicts.GetCountConfl() < currentConflict.conflicts.GetCountConfl())
                {
                	currentConflict = nt;
                	isNewRule = SetConflictNext();
                }*/
            }

            if ((conflHist.Count <= 0) && (minConfl < 0))//((currentConflict == null) || (currentConflict.name == ""))
                return false;
            
            int idx = conflHist.FindIndex(el => el.conflicts == currentConflict.conflicts);
            if (idx >= 0)
            {
            	if (conflHist[idx].iterationNumber >= 6)
            		return false;
            	currentConflict = conflHist[idx];
            	isNewRule = SetConflictNext();
            }
            else
            {
            	isNewRule = true;
                conflHist.Add(currentConflict);
            }
            
            
            /*if (!isNewRule)
            {
                isNewRule = false;//SetConflictNext();
            }*/
            return isNewRule;
        }

        

        public bool IsBestExecept(NodePoint inPnt)
        {
        	if (isTraceH && (currentConflict.conflicts.GetNames().FindIndex(el => el == inPnt.name) >= 0))
                return true;
            return false;
        }

        public bool IsBestHeight(NodePointLayer inPnt)
        {
            if (isTraceH && (inPnt.name == currentConflict.name) && inPnt.numberNode == currentConflict.number1)
            {
                if (inPnt.layer == Layers.metal1Trace)// &&  currentConflict.border1Met )
                {
                    if ( currentConflict.compare1Met && (inPnt.number > currentConflict.border1Met))
                        return true;
                    if (!currentConflict.compare1Met && (inPnt.number < currentConflict.border1Met))
                        return true;
                }
                if (inPnt.layer == Layers.siliconTrace)
                {
                    if (currentConflict.compare1Sil && (inPnt.number > currentConflict.border1Sil))
                        return true;
                    if (!currentConflict.compare1Sil && (inPnt.number < currentConflict.border1Sil))
                        return true;
                }
                    //(inPnt.number > numberH))
            }
            if (isTraceH && (inPnt.name == currentConflict.name) && inPnt.numberNode == currentConflict.number2)//Params.maxPriority
            {
                if (inPnt.layer == Layers.metal1Trace)
                {
                    if (currentConflict.compare2Met && (inPnt.number > currentConflict.border2Met))
                        return true;
                    if (!currentConflict.compare2Met && (inPnt.number < currentConflict.border2Met))
                        return true;
                }
                if (inPnt.layer == Layers.siliconTrace)
                {
                    if (currentConflict.compare2Sil && (inPnt.number > currentConflict.border2Sil))
                        return true;
                    if (!currentConflict.compare2Sil && (inPnt.number < currentConflict.border2Sil))
                        return true;
                }
            }
            return false;
        }

        public NodePointLayer GetBest(List<NodePointLayer> inPoints)
        {
            NodePointLayer bestPnt = GetAnyNotDiffusion(inPoints);
            bool isExcept = false;
            foreach (NodePointLayer pnt in inPoints)
            {
                if (bestPnt.priority < pnt.priority)
                    bestPnt = pnt;
                isExcept = IsBestExecept(pnt);
            }
            if (isTraceH && (bestPnt.name == currentConflict.name) && (!IsBestHeight(bestPnt)))//!!!!
            {
                foreach (NodePointLayer pnt in inPoints)
                {
                    bestPnt = ChooseBestWithHigh(pnt, bestPnt);
                }
            }
            return bestPnt;
        }

        public NodePointLayer ChooseBestWithHigh(NodePointLayer inPnt1, NodePointLayer inPnt2)
        {
            bool fstExcept = IsBestExecept(inPnt1);
            bool scdExcept = IsBestExecept(inPnt2);
            //bool fstBestHeight = IsBestHeight(inPnt1);
            //bool scdBestHeight = IsBestHeight(inPnt2);

            if (fstExcept && scdExcept)
            {
                if (inPnt1.priority < inPnt2.priority)
                    return inPnt2;
                return inPnt1;
            }

            if (fstExcept)
                return inPnt1;
            return inPnt2;
        }
        
        public bool IsExceptionContain(List<BestPointSet> inPoints)
        {
        	if (!isTraceH)
        		return false; 
        	bool isExcept = false;
        	bool isBestHeighPrior = false;
            foreach (BestPointSet pnt in inPoints)
            {
                isExcept = IsBestExecept(pnt.point);
                if ((pnt.point.name == currentConflict.name) && (!IsBestHeight(pnt.point)))
                	isBestHeighPrior = true;
            }
            return (isExcept && isBestHeighPrior);
        }
        //public int  

        private NodePoint GetAnyNotDiffusion(List<NodePoint> inPoints)
        {
            foreach (NodePoint pnt in inPoints)
            {
                if (pnt.name != Material.diffusionName)
                    return pnt;
            }
            return inPoints[0];
        }

        private NodePointLayer GetAnyNotDiffusion(List<NodePointLayer> inPoints)
        {
            foreach (NodePointLayer pnt in inPoints)
            {
                if (pnt.name != Material.diffusionName)
                    return pnt;
            }
            return inPoints[0];
        }
    }
}
