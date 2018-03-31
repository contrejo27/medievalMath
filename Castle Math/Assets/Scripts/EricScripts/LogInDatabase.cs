using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDataEditor;
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

public class LogInDatabase : MonoBehaviour 
{
    private CognitoAWSCredentials m_credentials;
    private IAmazonDynamoDB client;
    private DynamoDBContext m_context;

    private List<UserData> m_userDatas = new List<UserData>();

    private string m_currentEmail = "";
    private DynamoDBContext Context 
    {
        get
        {
            if (m_context == null)
                m_context = new DynamoDBContext(client);
            return m_context;
        }
    }

    [DynamoDBTable("UserLoginData")]
    public class UserData
    {
        [DynamoDBHashKey]
        public string UserEmail { get; set; }
        [DynamoDBProperty]
        public string UserPassword { get; set; }
    }

	private void Awake()
	{
        PlayerPrefs.SetInt("LoggedIn", 0);

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
                TableName = @"UserLoginData"
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

            client = ddbClient;
            RetrieveUserData();
        });
	}

	AmazonDynamoDBClient CreateDBClient()
    {
        m_credentials = new CognitoAWSCredentials("us-east-1:30e8de39-1567-46d7-a033-02870c1d2102", Amazon.RegionEndpoint.USEast1);
        return new AmazonDynamoDBClient(m_credentials, RegionEndpoint.USEast1);
    }

    private void RetrieveUserData()
    {
        Debug.Log("Retrieving All User Datas");
        Table.LoadTableAsync(client, "UserLoginData", (result) =>
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
                    var search = context.ScanAsync<UserData>(new ScanCondition("UserEmail", ScanOperator.NotContains, "null"));

                    search.GetRemainingAsync(scanResult =>
                    {
                        Debug.Log("Searching");
                        if (scanResult.Exception == null)
                        {
                            Debug.Log("Scan Result complete");
                            m_userDatas = scanResult.Result;
                            Debug.Log(m_userDatas.Count);
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

        Debug.Log("Finished Reterieving User Datas");
    }

    public void CreateUserData(string password)
    {
        Debug.Log("Creating new user...");
        UserData newUser = new UserData
        {
            UserEmail = m_currentEmail,
            UserPassword = password,
        };

        // Save Data
        Context.SaveAsync(newUser, (result) =>
        {
            if (result.Exception == null)
            {
                Debug.Log("New User Saved");
                m_userDatas.Clear();
                RetrieveUserData();
            }

            else
                Debug.Log(result.Exception.Message);
        });
    }

    private void LoadUserData(UserData data)
    {
        Debug.Log("Loading user data");
        Debug.Log("Loading user email: " + data.UserEmail);
        Debug.Log("Loading user pass: " + data.UserPassword);
    }
	public bool IsEmailValid(string email)
    {
        foreach (UserData data in m_userDatas)
        {
            if(data.UserEmail.ToLower() == email.ToLower())
            {
                m_currentEmail = data.UserEmail;
                return true;
            }
        }

        m_currentEmail = "";
        return false;
    }
	
    public bool IsPasswordValid(string password)
    {
        if (m_currentEmail == "") return false;

        foreach(UserData data in m_userDatas)
        {
            if (data.UserEmail.ToLower() == m_currentEmail.ToLower())
                return data.UserPassword == password;
        }
        return false;
    }

    public bool DoesEmailHavePassword()
    {
        foreach(UserData data in m_userDatas)
        {
            if (data.UserEmail.ToLower() == m_currentEmail.ToLower())
                return data.UserPassword != null;
        }
        return true;
    }

}
