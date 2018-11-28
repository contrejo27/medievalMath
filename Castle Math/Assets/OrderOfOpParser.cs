using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Text;
public class OrderOfOpParser : MonoBehaviour {
	

// TODO: inputProblems string
// TODO: Parse by + and -
// TODO: Parse by * and /
// TODO: Parse by ( and )
	int a;
	int b;
	char opperand;

	void Start () {
		Parse();
	}


void Parse(){
		string[] inputProblems = {"12+3*3", "3*2+4"};
		char[] separators = {'+','-','*','/'};
		string[] substrings;
		List<string> OutputProblems = new List<string>();

		foreach (string str in inputProblems) {
			foreach(string s in str.Split(','))
			OutputProblems.Add(s);
		}

		print (OutputProblems);

		foreach (string s in OutputProblems) {
			ParseProblem(s,separators,true,true);
		}
 	}
		

	
	void print(List<string> p){
		foreach (string s in p){
			Debug.Log(s);
		}
	}



	//pre-conditions: problem as a string ie 'a + b' / array of opperads to parse with /
	//				  bool for whether you want to keep opperands or remove them / bool to trim white space

	void ParseProblem(string s, char[] c, bool includeC, bool Trim)
	{		
		List<string> strL = new List<string>();
		StringBuilder sBuilder = new StringBuilder ();

		for(int i = 0; i < s.Length; i++) {
			sBuilder.Append (s [i]);
			//problem is in here----------------
			foreach(char cha in c) {
				if (s[i] == cha) {
					if (!includeC) {
						sBuilder.Remove (sBuilder.Length - 1, 1);
					}

					if (Trim) {
						strL.Add (sBuilder.ToString ().Trim ());
					}
					else {
						strL.Add (sBuilder.ToString());
					}
					//clear sbuilder
					sBuilder.Length = 0;
					break;
				}
			}
			//----------------------
		}
		foreach(string str in strL)
			Debug.Log (str);
	}
}