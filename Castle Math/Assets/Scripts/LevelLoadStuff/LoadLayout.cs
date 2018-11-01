using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLayout : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		TextAsset xmlFile = (TextAsset)Resources.Load("Resources/level", typeof(TextAsset));
		MemoryStream assetStream = new MemoryStream(xmlFile.bytes);
		XmlReader reader = XmlReader.Create(assetStream);
		XmlDocument xmlDoc = new XmlDocument();

		try
		{
			xmlDoc.Load(reader);
		}
		catch (Exception ex)
		{
			Debug.Log("Error loading " + xmlFile.name + ":\n" + ex);
		}
	}
}
