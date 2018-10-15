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

    public const string METRIC_WAVELOST = "WaveLost";
    public const string METRIC_TIMEINVR = "TimeInVR";
    public const string METRIC_ARROWSLEFT = "ArrowsLeft";
    public const string TABLE_NAME = "GameMetricsData";
    public const string DB_URL = "us-east-1:30e8de39-1567-46d7-a033-02870c1d2102";

    [DynamoDBTable(TABLE_NAME)]
    public class MetricSchema {
        // TODO: Add User
        [DynamoDBHashKey]
        public string Metric { get; set; }
        [DynamoDBProperty]
        public float Value { get; set; }
        [DynamoDBProperty]
        public int NumberEntries { get; set; }
    }

    private List<MetricSchema> m_Metrics = new List<MetricSchema>();

    private void Awake() {
        instance = this;
        DontDestroyOnLoad(this);
        UnityInitializer.AttachToGameObject(this.gameObject);
    }

    private void Start() {
        client = CreateDBClient();
    }

    AmazonDynamoDBClient CreateDBClient() {
        Amazon.AWSConfigs.HttpClient = Amazon.AWSConfigs.HttpClientOption.UnityWebRequest;
        m_credentials = new CognitoAWSCredentials(DB_URL, Amazon.RegionEndpoint.USEast1);

        var ddbClient = new AmazonDynamoDBClient(m_credentials, Amazon.RegionEndpoint.USEast1);

        m_credentials.GetIdentityIdAsync(delegate (AmazonCognitoIdentityResult<string> result) {

            var request = new DescribeTableRequest {
                TableName = @TABLE_NAME
            };

            ddbClient.DescribeTableAsync(request, (ddbresult) => {
                if (result.Exception != null) {
                    Debug.Log(result.Exception.Message);
                    return;
                }

                var response = ddbresult.Response;

                TableDescription description = response.Table;
                Debug.Log("Table Name: " + description.TableName);
                Debug.Log("# of Items: " + description.ItemCount);
                Debug.Log("Schema: " + description.KeySchema[0].AttributeName);

            }, null);

            client = ddbClient;
            GetMetric(METRIC_TIMEINVR);
        });
        return ddbClient;
    }


    private void Update() {
        /// <summary>
        /// TODO: Confirm
        /// Check which key is being pressed and updates the corresponding metric
        /// </summary>
        // Number row 1
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            Debug.Log("Pressed 1");
            PostMetric(METRIC_ARROWSLEFT, Random.Range(1,5));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            Debug.Log("Pressed 2");
            PostMetric(METRIC_TIMEINVR, Random.Range(1, 5));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            Debug.Log("Pressed 3");
            PostMetric(METRIC_WAVELOST, Random.Range(1, 5));
        }
    }

    private void GetMetric(string metric) {

        Table.LoadTableAsync(client, TABLE_NAME, (result) => {
            if (result.Exception != null) {
                Debug.Log("Failed to load Metric Table");
            }
            else {
                Debug.Log("Result succesful");
                try {
                    Debug.Log("Trying to scan");

                    var search = DBContext.ScanAsync<MetricSchema>(new ScanCondition("Metric", ScanOperator.NotContains, "null"));
                    Debug.Log("Done Scanning");


                    search.GetRemainingAsync(scanResult => {
                        if (scanResult.Exception == null) {
                            Debug.Log("Scan Result complete");
			    m_Metrics = scanResult.Result;
			    foreach (MetricSchema item in m_Metrics) {
				Debug.Log("Name  Value  Entries\n" + item.Metric + " " + item.Value + " " + item.NumberEntries);
			    }
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

    public void PostMetric(string metric, float modifier) {
        /// <summary>
        /// Post metrics to the database.
        /// </summary>
        foreach (MetricSchema data in m_Metrics) {
            if (data.Metric == metric) {
                float newAverage;
                if (data.NumberEntries == 0) {
                    newAverage = modifier;
                }
                else {
                    float currentAverage = data.Value * data.NumberEntries;
                    newAverage = (currentAverage + modifier) / (data.NumberEntries + 1);
                }

                MetricSchema newMetric = new MetricSchema {
                    Metric = metric,
                    NumberEntries = data.NumberEntries + 1,
                    Value = newAverage,
                };

                DBContext.SaveAsync(newMetric, (result) => {
                    if (result.Exception == null) {
                        Debug.Log("Metric updated!");
                        m_Metrics.Clear();
                        GetMetric(metric);
                    }
                    else
                        Debug.Log(result.Exception);
                });
            }
        }
    }


}
