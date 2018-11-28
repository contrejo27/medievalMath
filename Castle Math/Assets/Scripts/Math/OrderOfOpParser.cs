using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Text;
using System;
public class OrderOfOpParser : MonoBehaviour {
	

// TODO: inputProblems string
// TODO: Parse by + and -
// TODO: Parse by * and /
// TODO: Parse by ( and )
	int a;
	int b;
	char opperand;

	void Start () {
		if (TestOrderOfOperations()) {
			Debug.Log("Test Successful: Order of Operations");
		}
	}

	int ExecuteOperation(string _operator, int a, int b) {
		/// <summary>
		/// Returns the mathematical result of "a operator b".
		/// </summary>

		switch (_operator) {
			case "+":
				return a + b;
			case "-":
				return a - b;
			case "*":
				return a * b;
			case "/":
				return a / b;
			default:
				Debug.Log(_operator);
				throw new ArgumentException("_operator");
		}
	}

	int Answer(int[] operandArray, string[] operatorArray) {
		/// <summary>
		/// Operates on operands in the correct order. Currently supports */+-
		/// </summary>

		List<int> operands = new List<int>(operandArray);
		List<string> operators = new List<string>(operatorArray);

		// Multiplication and Division
		for (int i = 0; i < operators.Count; i++) {
			if (operators[i] == "*" || operators[i] == "/") {
				int result = ExecuteOperation(operators[i], operands[i], operands[i+1]);
				// Debug.Log(string.Format("i={0}, result={1}", i, result));
				operands[i] = result;
				operators.RemoveAt(i);
				operands.RemoveAt(i+1);
			}
		}

		// Addition and Subtraction
		for (int i = 0; i < operators.Count; i++) {
			if (operators[i] == "+" || operators[i] == "-") {
				int result = ExecuteOperation(operators[i], operands[i], operands[i+1]);
				// Debug.Log(string.Format("i={0}, result={1}", i, result));
				operands[i] = result;
				operators.RemoveAt(i);
				operands.RemoveAt(i+1);
			}
		}

		// Return last element remaining in the operand list
		return operands[0];
	}

	public class Problem {
		public int[] operands { get; set; }
		public string[] operators { get; set; }
		public int answer { get; set; }

		public Problem (int[] operands, string[] operators, int answer) {
			this.operands = operands;
			this.operators = operators;
			this.answer = answer;
		}

		public string ToString () {
			string problem = "";
			for (int i = 0; i < operands.Length; i++) {
				problem += operands[i].ToString();
				if (i == operands.Length - 1) {
					break;
				}
				problem += operators[i].ToString();
			}
			return problem;
		}
	}

	bool TestOrderOfOperations() {
		// inputProblems = {"12+3*3", "3*2+4"};
		List<Problem> testProblems = new List<Problem>();
		testProblems.Add(new Problem(new int[] {12,3,3}, new string[] {"+", "*"}, 21));
		testProblems.Add(new Problem(new int[] {3,2,4}, new string[] {"*", "+"}, 10));

		int errors = 0;

		foreach (Problem problem in testProblems) {
			foreach (int operand in problem.operands) {
				// Debug.Log(operand);
			}

			foreach (string _operator in problem.operators) {
				// Debug.Log(_operator);
			}

			int answer = Answer(problem.operands, problem.operators);

			if (answer != problem.answer) {
				Debug.Log("WARNING: Test Failed: Order of Operations");
				Debug.Log(string.Format("{0} = {1}, but returned {2}", problem.ToString(), problem.answer, answer));
				errors++;
			}
		}

		if (errors == 0) {
			return true;
		}
		else {
			return false;
		}

		// string[] inputProblems = {"12+3*3", "3*2+4"};
		// char[] separators = {'+','-','*','/'};
		// string[] substrings;
		// List<string> OutputProblems = new List<string>();
 
		// foreach (string str in inputProblems) {
			// foreach (string s in str.Split(',')) {
				// OutputProblems.Add (s);
			// }
		// }
 
		// print (OutputProblems);
 
		// foreach (string s in OutputProblems) {
			// ParseProblem(s,separators,true,true);
		// }
 	}
		
	void print(List<string> p){
		foreach (string s in p) {
			Debug.Log(s);
		}
	}


	void ParseProblem(string s, char[] c, bool includeC, bool Trim)	{		
		//pre-conditions: problem as a string ie 'a + b' / array of opperads to parse with /
		//				  bool for whether you want to keep opperands or remove them / bool to trim white space
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