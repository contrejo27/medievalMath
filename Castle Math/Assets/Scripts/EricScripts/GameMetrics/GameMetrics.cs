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

public class GameMetrics : MonoBehaviour
{
    public static GameMetrics m_instance;

    [DynamoDBTable("GameMetricsData")]
    public class GameMetricsData
    {
        [DynamoDBHashKey]
        public string Metric { get; set; }
        [DynamoDBProperty]
        public float Value { get; set; }
        [DynamoDBProperty]
        public int NumberEntries { get; set; }
    }

    #region METRICS - DONT MODIFY
    public const string METRIC_WAVELOST = "WaveLost";
    public const string METRIC_TIMEINVR = "TimeInVR";
    public const string METRIC_ARROWSLEFT = "ArrowsLeft";
    #endregion

    private DynamoDBContext Context
    {
        get
        {
            if (m_context == null)
                m_context = new DynamoDBContext(m_client);
            return m_context;
        }
    }

    private CognitoAWSCredentials m_credentials;
    private IAmazonDynamoDB m_client;
    private DynamoDBContext m_context;

    private List<GameMetricsData> m_gameMetrics = new List<GameMetricsData>();

    private void Awake()
    {
        m_instance = this;

        UnityInitializer.AttachToGameObject(this.gameObject);
        Amazon.AWSConfigs.HttpClient = Amazon.AWSConfigs.HttpClientOption.UnityWebRequest;
    }

    private void Start()
    {
        m_credentials = new CognitoAWSCredentials("us-east-1:30e8de39-1567-46d7-a033-02870c1d2102", RegionEndpoint.USEast1);
        m_credentials.GetIdentityIdAsync(delegate (AmazonCognitoIdentityResult<string> result)
        {
            if (result.Exception != null)
                Debug.Log("Exception hit: " + result.Exception.Message);

            var ddbClient = new AmazonDynamoDBClient(m_credentials, RegionEndpoint.USEast1);

            Debug.Log("Retrieveing Table information");

            var request = new DescribeTableRequest
            {
                TableName = @"GameMetricsData"
            };

            ddbClient.DescribeTableAsync(request, (ddbresult) =>
            {
                if (result.Exception != null)
                {
                    Debug.Log(result.Exception.Message);
                    return;
                }

                var response = ddbresult.Response;

                TableDescription description = response.Table;
                Debug.Log("Table Name: " + description.TableName);
                Debug.Log("# of Items: " + description.ItemCount);

            }, null);

            m_client = ddbClient;
            RetreiveMetricData();
        });
    }

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            UpdateMetric(METRIC_ARROWSLEFT, Random.Range(1,5));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UpdateMetric(METRIC_TIMEINVR, Random.Range(1, 5));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UpdateMetric(METRIC_WAVELOST, Random.Range(1, 5));
        }
	}


	private void RetreiveMetricData()
    {
        Debug.Log("Retrieving All User Datas");
        Table.LoadTableAsync(m_client, "GameMetricsData", (result) =>
        {
            if (result.Exception != null)
            {
                Debug.Log("Failed to load user data table");
            }
            else
            {
                Debug.Log("Result succesful");
                try
                {
                    Debug.Log("Trying to scan");
                    var context = Context;
                    var search = context.ScanAsync<GameMetricsData>(new ScanCondition("Metric", ScanOperator.NotContains, "null"));

                    search.GetRemainingAsync(scanResult =>
                    {
                        Debug.Log("Searching");
                        if (scanResult.Exception == null)
                        {
                            Debug.Log("Scan Result complete");
                            m_gameMetrics = scanResult.Result;
                            Debug.Log(m_gameMetrics.Count);
                        }
                        else
                            Debug.Log("Failed to get async table scane results: " + scanResult.Exception.Message);
                    }, null);
                }
                catch (AmazonDynamoDBException exception)
                {
                    Debug.Log(string.Concat("Exception fetching characters from table: {0}", exception.Message));
                    Debug.Log(string.Concat("Error code: {0}, error type: {1}", exception.ErrorCode, exception.ErrorType));
                }
            }
        });

        Debug.Log("Finished Reterieving Game Metrics");
    }

    public void UpdateMetric(string metric, float modifier)
    {
        foreach (GameMetricsData data in m_gameMetrics)
        {
            if (data.Metric == metric)
            {
                float newAverage;
                if (data.NumberEntries == 0)
                {
                    newAverage = modifier;
                }
                else
                {
                    float currentAverage = data.Value * data.NumberEntries;
                    newAverage = (currentAverage + modifier) / (data.NumberEntries + 1);
                }

                GameMetricsData newMetric = new GameMetricsData
                {
                    Metric = metric,
                    NumberEntries = data.NumberEntries + 1,
                    Value = newAverage,
                };

                Context.SaveAsync(newMetric, (result) =>
                {
                    if (result.Exception == null)
                    {
                        Debug.Log("Metric updated!");
                        m_gameMetrics.Clear();
                        RetreiveMetricData();
                    }
                    else
                        Debug.Log(result.Exception);
                });
            }
        }
    }
}
