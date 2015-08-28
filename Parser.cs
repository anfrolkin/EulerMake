
using System;

namespace eulerMake {



public class Parser {
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _number = 2;
	public const int maxT = 16;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

public int result = 0;
  public Transistors transList;
  private bool flagNetBody = false;
/*--------------------------------------------------------------------------*/


	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void Ident(out string name) {
		Expect(1);
		name = t.val; 
	}

	void EdfBody3(out int retCount) {
		retCount = 0; 
		secondBody();
		while (la.kind == 3) {
			Get();
			EdfBody3(out retCount);
			result += 1; 
			Expect(4);
		}
	}

	void secondBody() {
		Expect(1);
		while (la.kind == 1 || la.kind == 2) {
			if (la.kind == 1) {
				Get();
			} else {
				Get();
			}
		}
	}

	void EdfBody4(out int retCount) {
		retCount = 0; string name = ""; 
		while (la.kind == 3) {
			Get();
			switch (la.kind) {
			case 1: {
				secondBody();
				EdfBody4(out retCount);
				break;
			}
			case 9: {
				netBody();
				break;
			}
			case 5: {
				joinedBody();
				break;
			}
			case 14: {
				instBody();
				break;
			}
			case 11: {
				propertyBody();
				break;
			}
			case 8: {
				alterName(out name);
				break;
			}
			case 13: {
				nameBody(out name);
				break;
			}
			case 12: {
				cellBody();
				break;
			}
			default: SynErr(17); break;
			}
			result += 1; 
			Expect(4);
		}
	}

	void netBody() {
		string name = ""; int count; 
		Expect(9);
		if (la.kind == 1) {
			Ident(out name);
		} else if (la.kind == 3) {
			Get();
			if (la.kind == 8) {
				alterName(out name);
			} else if (la.kind == 13) {
				nameBody(out name);
			} else SynErr(18);
			Expect(4);
		} else SynErr(19);
		transList.setNode(name); flagNetBody = true; 
		Expect(3);
		joinedBody();
		Expect(4);
		flagNetBody = false; 
		EdfBody4(out count);
	}

	void joinedBody() {
		string name, typePort; 
		Expect(5);
		while (la.kind == 6) {
			Get();
			Ident(out typePort);
			if (la.kind == 7) {
				Get();
				Ident(out name);
				Expect(4);
				if (flagNetBody)
				transList.setTrans(name, typePort); 
			}
			Expect(4);
		}
	}

	void instBody() {
		string altName = ""; string name = "", type; 
		int count; 
		Expect(14);
		if (la.kind == 1) {
			Ident(out name);
		}
		Expect(3);
		if (la.kind == 8) {
			alterName(out altName);
			Expect(4);
			Expect(3);
		}
		if (la.kind == 1) {
			secondBody();
			Expect(4);
			Expect(3);
		}
		Expect(15);
		Ident(out type);
		transList.addTrans(name, type); 
		while (la.kind == 3) {
			Get();
			EdfBody3(out count);
			Expect(4);
		}
		Expect(4);
		EdfBody4(out count);
	}

	void propertyBody() {
		string name = ""; int count; 
		Expect(11);
		if (la.kind == 1) {
			secondBody();
			EdfBody4(out count);
		} else if (la.kind == 3) {
			Get();
			if (la.kind == 8) {
				alterName(out name);
			} else if (la.kind == 13) {
				nameBody(out name);
			} else SynErr(20);
			Expect(4);
			EdfBody4(out count);
		} else SynErr(21);
	}

	void alterName(out string retName) {
		string netName = ""; int count; 
		Expect(8);
		if (la.kind == 1) {
			getFirstIdent(out netName);
			transList.setNode(netName); 
		} else if (la.kind == 3) {
			Get();
			nameBody(out netName);
			Expect(4);
			transList.setNode(netName); 
			if (la.kind == 1) {
				secondBody();
			}
		} else SynErr(22);
		retName = netName; 
	}

	void nameBody(out string retName) {
		string name = ""; int count; 
		Expect(13);
		Ident(out name);
		retName = name; 
		EdfBody4(out count);
	}

	void cellBody() {
		string name = ""; int count; 
		Expect(12);
		if (la.kind == 1) {
			secondBody();
			EdfBody4(out count);
		} else if (la.kind == 3) {
			Get();
			alterName(out name);
			Expect(4);
			EdfBody4(out count);
		} else SynErr(23);
	}

	void getFirstIdent(out string retName) {
		string name = ""; int count; 
		Ident(out name);
		while (la.kind == 1 || la.kind == 2) {
			if (la.kind == 1) {
				Get();
			} else {
				Get();
			}
		}
		retName = name; 
	}

	void libraryBody() {
		string name = ""; int count; 
		Expect(10);
		if (la.kind == 1) {
			Get();
		} else if (la.kind == 3) {
			Get();
			alterName(out name);
			Expect(4);
		} else SynErr(24);
		EdfBody4(out count);
	}

	void EdfBody(out int retCount) {
		retCount = 0; 
		if (la.kind == 10) {
			libraryBody();
		} else if (la.kind == 1) {
			secondBody();
			while (la.kind == 3) {
				Get();
				EdfBody(out retCount);
				result += 1; 
				Expect(4);
			}
		} else SynErr(25);
	}

	void Taste() {
		int retCount; 
		Expect(3);
		result = 1; transList = new Transistors(); 
		if (la.kind == 1 || la.kind == 10) {
			EdfBody(out retCount);
		}
		Expect(4);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Taste();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "ident expected"; break;
			case 2: s = "number expected"; break;
			case 3: s = "\"(\" expected"; break;
			case 4: s = "\")\" expected"; break;
			case 5: s = "\"joined\" expected"; break;
			case 6: s = "\"(portRef\" expected"; break;
			case 7: s = "\"(instanceRef\" expected"; break;
			case 8: s = "\"rename\" expected"; break;
			case 9: s = "\"net\" expected"; break;
			case 10: s = "\"library\" expected"; break;
			case 11: s = "\"property\" expected"; break;
			case 12: s = "\"cell\" expected"; break;
			case 13: s = "\"name\" expected"; break;
			case 14: s = "\"instance\" expected"; break;
			case 15: s = "\"viewRef\" expected"; break;
			case 16: s = "??? expected"; break;
			case 17: s = "invalid EdfBody4"; break;
			case 18: s = "invalid netBody"; break;
			case 19: s = "invalid netBody"; break;
			case 20: s = "invalid propertyBody"; break;
			case 21: s = "invalid propertyBody"; break;
			case 22: s = "invalid alterName"; break;
			case 23: s = "invalid cellBody"; break;
			case 24: s = "invalid libraryBody"; break;
			case 25: s = "invalid EdfBody"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}