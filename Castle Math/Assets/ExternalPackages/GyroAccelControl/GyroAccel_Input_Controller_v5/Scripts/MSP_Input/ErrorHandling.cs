using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MSP_Input 
{
	public class ErrorHandling : MonoBehaviour 
	{
		static private List<string> _errorList = new List<string>();	

		//================================================================================

		void Awake () 
		{
			_errorList.Clear();
		}

		//================================================================================

		public static void LogError (string incomingError) {
			if (Time.time > Time.fixedTime)
			{
				foreach (string error in _errorList)
				{
					if (error == incomingError)
					{
						return;
					}
				}
				_errorList.Add(incomingError);
				Debug.Log(incomingError);
				return;
			} else {
				return;
			}
		}

		//================================================================================

	} // public class ErrorHandling : MonoBehaviour

} // namespace MSP_Input 