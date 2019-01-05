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

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;

    private CognitoAWSCredentials m_credentials;
    private IAmazonDynamoDB client;
    private DynamoDBContext m_context;

    private List<UserData> m_userDatas = new List<UserData>();

    private DynamoDBContext Context
    {
        get
        {
            if (m_context == null)
                m_context = new DynamoDBContext(client);
            return m_context;
        }
    }

    [DynamoDBTable("ProfileDatabase")]
    public class UserData
    {
        [DynamoDBHashKey]
        public string UserEmail { get; set; }
        [DynamoDBProperty]
        public string UserName { get; set; }
        [DynamoDBProperty]
        public string UserPassword { get; set; }
        [DynamoDBProperty]
        public int DaysLeft { get; set; }
    }

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);

        UnityInitializer.AttachToGameObject(this.gameObject);
    }

    private void Start()
    {
		Amazon.AWSConfigs.HttpClient = Amazon.AWSConfigs.HttpClientOption.UnityWebRequest;

        m_credentials = new CognitoAWSCredentials("us-east-1:30e8de39-1567-46d7-a033-02870c1d2102", RegionEndpoint.USEast1);
        m_credentials.GetIdentityIdAsync(delegate (AmazonCognitoIdentityResult<string> result)
        {
            var ddbClient = new AmazonDynamoDBClient(m_credentials, RegionEndpoint.USEast1);

            var request = new DescribeTableRequest
            {
                TableName = @"ProfileDatabase"
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

    #region Get User Info
    public string GetUserName(string userEmail)
    {
        foreach (UserData data in m_userDatas)
        {
            if (data.UserEmail.ToLower() == userEmail.ToLower())
                return data.UserName;
        }

        return "";
    }

    public int GetDaysLeftOfSub(string userEmail)
    {
        foreach (UserData data in m_userDatas)
        {
            if (data.UserEmail.ToLower() == userEmail.ToLower())
                return data.DaysLeft;
        }

        return 0;
    }
    #endregion
    private void RetrieveUserData()
    {
        //Debug.Log("Retrieving All User Datas");
        Table.LoadTableAsync(client, "ProfileDatabase", (result) =>
        {
            if (result.Exception != null)
            {
                Debug.Log("Failed to load user data table");
            }
            else
            {
                //Debug.Log("Result succesful");
                try
                {
                    //Debug.Log("Trying to scan");
                    var context = Context;
                    var search = context.ScanAsync<UserData>(new ScanCondition("UserEmail", ScanOperator.NotContains, "null"));

                    search.GetRemainingAsync(scanResult =>
                    {
                        //Debug.Log("Searching");
                        if (scanResult.Exception == null)
                        {
                            //Debug.Log("Scan Result complete");
                            m_userDatas = scanResult.Result;
                            //Debug.Log(m_userDatas.Count);
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

        // Debug.Log("Finished Reterieving User Datas");
    }

    public void CreateNewProfile(UserData userData)
    {
		Debug.Log ("creating new profile...");
        // Save Data
        Context.SaveAsync(userData, (result) =>
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

    #region Handle User Login
    public bool IsEmailValid(string email)
    {
        foreach (UserData data in m_userDatas)
        {
            if (data.UserEmail.ToLower() == email.ToLower())
                return true;
        }

        return false;
    }

    public bool IsPasswordValid(string email, string password)
    {
        foreach (UserData data in m_userDatas)
        {
			if (data.UserEmail.ToLower () == email.ToLower ()) 
			{
				if (data.UserEmail.ToLower () == "dev@lucernastudios.com")
					return password == data.UserPassword;
				else
					return PasswordEncryption.Md5Sum (password) == data.UserPassword;
			}
        }
        return false;
    }
#endregion
}
