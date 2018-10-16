using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon.S3;
using Amazon.CognitoIdentity;
using Amazon;
using System.IO;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using Amazon.SecurityToken;
using Amazon.DynamoDBv2.Model;

public class TelemetryManager : MonoBehaviour {
    public static TelemetryManager instance;

    private AmazonDynamoDBModule DB;

    private void Awake() {
        instance = this;
        DontDestroyOnLoad(this);
        UnityInitializer.AttachToGameObject(this.gameObject);
    }

    private void Start() {
        DB = new AmazonDynamoDBModule();
    }

    private void TestDB() {
        /// <summary>
        /// Check which key is being pressed and updates the corresponding metric on the database
        /// </summary>

        // Number row above QWERTY
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            Debug.Log("Pressed 1");
            DB.CreateMetric("PressedKey1", Random.Range(1,5));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            Debug.Log("Pressed 2");
            DB.CreateMetric("PressedKey2", Random.Range(1, 5));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            Debug.Log("Pressed 3");
            DB.CreateMetric("PressedKey3", Random.Range(1, 5));
        }
    }

    private void Update() {
        /// <summary>
        /// Unity Function that runs after every screen update
        /// </summary>
        TestDB();
    }
}
