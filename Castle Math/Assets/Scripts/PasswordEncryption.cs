using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;

public static class PasswordEncryption 
{
	public static string Md5Sum(string strToEncrypt)
	{
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding ();
		byte[] bytes = ue.GetBytes (strToEncrypt);

		MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider ();

		byte[] hashBytes = md5.ComputeHash (bytes);

		string hashString = "";

		for (int i = 0; i < hashBytes.Length; i++) 
		{
			hashString += System.Convert.ToString (hashBytes [i], 16).PadLeft (2, '0');

		}

		return hashString.PadLeft (32, '0');
	}
}
