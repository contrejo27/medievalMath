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

public class AmazonDynamoDBModule {
    public AmazonDynamoDBModule() {
        SetClient();
    }

    private CognitoAWSCredentials m_credentials;
    private IAmazonDynamoDB client;
    private DynamoDBContext m_context;

    private DynamoDBContext DBContext {
        get {
            if (m_context == null)
                m_context = new DynamoDBContext(client);
            return m_context;
        }
    }


    [DynamoDBTable(DB_TABLE)]
    public class PropertySchema {
        // TODO: Add User, DeviceType, TimeStamp, 
        [DynamoDBHashKey]
        public string Hash { get; set; }
        [DynamoDBProperty]
        public string UserId { get; set; }
        [DynamoDBProperty]
        public string DeviceType { get; set; }
        [DynamoDBProperty]
        public string DateTime { get; set; }
        [DynamoDBProperty]
        public string PropertyName { get; set; }
        [DynamoDBProperty]
        public float PropertyValue { get; set; }
    }

    public const string DB_TABLE = "RawData";
    public const string DB_URL = "us-east-1:30e8de39-1567-46d7-a033-02870c1d2102";

    public void SetClient() {
        /// <summary>
        /// Provides a client to interact with the database.
        /// This function is meant to be called by one of the Unity Managers
        /// </summary>
        var DB_REGION = Amazon.RegionEndpoint.USEast1;

        Amazon.AWSConfigs.HttpClient = Amazon.AWSConfigs.HttpClientOption.UnityWebRequest;
        m_credentials = new CognitoAWSCredentials(DB_URL, DB_REGION);
        client = new AmazonDynamoDBClient(m_credentials, DB_REGION);

        // TODO: Remove. Debug only.
        Debug.Log("Created DB Client.");
        RetrieveTable(DB_TABLE);
    }


    public void RetrieveTable(string TableName) {
        /// <summary>
        /// Reads table from DB
        /// </summary>
        m_credentials.GetIdentityIdAsync(delegate (AmazonCognitoIdentityResult<string> result) {

            var request = new DescribeTableRequest {
                TableName = @TableName
            };

            client.DescribeTableAsync(request, (r) => {
                if (r.Exception != null) {
                    Debug.Log(result.Exception.Message);
                    return;
                }

                Debug.Log(r.Response);
                var response = r.Response;

                // TODO: Remove. Debug only.
                TableDescription table = response.Table;
                Debug.Log("Table Name: " + table.TableName);
                Debug.Log("# of Items: " + table.ItemCount);
                Debug.Log("Schema: " + table.KeySchema[0].AttributeName);

            }, null);
        });
    }

    public void RetrieveMetric(string name) {
        /// <summary>
        /// Reads information from the database asynchonously
        /// </summary>

        Table.LoadTableAsync(client, DB_TABLE, (result) => {
            if (result.Exception != null) {
                Debug.Log("Failed to load Metric Table");
            }
            else {
                try {
                    var search = DBContext.ScanAsync<PropertySchema>(new ScanCondition("propertyName", ScanOperator.Contains, name));
                    Debug.Log("Scanning database...");

                    search.GetRemainingAsync(scanResult => {
                        if (scanResult.Exception == null) {
            			    var payload = scanResult.Result;
                            Debug.Log(payload);
                        }
                        else {
                            Debug.Log("Failed to get async table scan results: " + scanResult.Exception.Message);
                        }
                    }, null);
                }
                catch (AmazonDynamoDBException exception) {
                    Debug.Log(string.Concat("Exception fetching characters from table: {0}", exception.Message));
                    Debug.Log(string.Concat("Error code: {0}, error type: {1}", exception.ErrorCode, exception.ErrorType));
                }
            }
        });
    }

    public void CreateMetric(string name, float data) {
        /// <summary>
        /// Post metrics to the database.
        /// </summary>
        
        // TODO: Implement unique Ids
        string user = "user";
        string device = "Unity Editor";
        string time = System.DateTime.UtcNow.ToString();
        string hash = time;

        PropertySchema newMetric = new PropertySchema {
            Hash = hash,
            UserId = user,
            DeviceType = device,
            DateTime = time,
            PropertyName = name,
            PropertyValue = data,
        };

        // TODO: Get CreationDateTime
        DBContext.SaveAsync(newMetric, (result) => {
            if (result.Exception == null) {
                Debug.Log("Metric updated with:\nPropertyName: " + newMetric.PropertyName + "\nValue: " + newMetric.PropertyValue);
            }
            else{
                Debug.Log(result.Exception);
            }
        });
    }
}
