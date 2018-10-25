using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;

public class TelemetryManager : MonoBehaviour {
    public static TelemetryManager instance;

    private static SQLDBModule DB;

    // TODO: Keep database details private and outside git
    public string DB_URL;
    public string DB_DATABASE;
    public string DB_USER;
    public string DB_PASSWORD;
    public string API_URL;

    private void Awake() {
        instance = this;
        DontDestroyOnLoad(this);
        // TODO: Understand why GameMetrics was attaching to UnityInitializer
        /* UnityInitializer.AttachToGameObject(this.gameObject); */
    }

    private void Start() {
        DB = new SQLDBModule();
        if (instance.API_URL == "") {
            instance.API_URL = "medieval-math.herokuapp.com/api/";
        }
    }

    private void Update() {
        /// <summary>
        /// Unity Function that runs after every screen update
        /// </summary>
        // TestAPI();
    }

    public void SaveTable(List<List<string>> list) {
        System.IO.File.WriteAllText(@"C:\Users\Home\database.txt", MakeTable(list));
    }

    public void PrintTable(List<List<string>> list) {
        Debug.Log(MakeTable(list));
    }

    public string MakeTable(List<List<string>> list) {
        string table = "";
        string header = string.Format("| {0,22} " +
                                      "| {1,22} " +
                                      "| {2,22} " +
                                      "| {3,22} " +
                                      "| {4,22} " +
                                      "| {5,22} " +
                                      "|"
				                      ,
                                      "Time",
                                      "User",
                                      "Device",
                                      "Property",
                                      "Value",
                                      "Modified"
        );

        foreach (List<string> row in list) {
            table += string.Format("| {4,22} " +
                                   "| {0,22} " +
                                   "| {1,22} " +
                                   "| {2,22} " +
                                   "| {3,22} " +
                                   "| {5,22} " +
                                   "|\n"
				                   ,
                                   row[0],
                                   row[1],
                                   row[2],
                                   row[3],
                                   row[4],
                                   row[5]
			);
        }

        return header + "\n" + table;
    }

    public void Print(List<string> item) {
        // Unity only allows 2 lines to be displayed on the console
        //   Remember to click to read the rest of it
        Debug.Log(string.Format("Time: {4}\n" +
                                "User: {0}\n" +
                                "Device: {1}\n" +
                                "Property: {2}\n" +
                                "Value: {3}\n" +
                                "Modified: {5}"
				                ,
                                item[0],
                                item[1],
                                item[2],
                                item[3],
                                item[4],
                                item[5]
				            )
		);
    }

    public static void APIPost(string key, string jsonPayload) {
        var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + instance.API_URL + key);
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";

        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) {
            streamWriter.Write(jsonPayload);
            streamWriter.Flush();
            streamWriter.Close();
        }

        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
            var result = streamReader.ReadToEnd();
            Debug.Log(result);
        }
    }

    private void TestAPI() {
        /// <summary>
        /// Check which key is being pressed and sends a request to the API
        /// </summary>

        // Number row above QWERTY
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            LogSession();
        }
    }

    private void TestDB() {
        /// <summary>
        /// Check which key is being pressed and updates the corresponding metric on the database
        /// </summary>

        // Number row above QWERTY
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            PrintTable(DB.Select());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            Write("fakeProperty2", "1");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            Write("fakeProperty3", 1.2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            Debug.Log("DELETING ALL ROWS!!!\nYou have 5 seconds to cancel!!!");
            System.Threading.Thread.Sleep(5000);

            DB.DeleteAll();
        }
    }

    public void LogSession() {
        GameStateManager m_gameState = GetComponent<GameStateManager>();

        string playerName = PlayerPrefs.GetString("PlayerName");
        string skillLevel = PlayerPrefs.GetInt("Skill Level").ToString();
        string stopTime = Time.time.ToString();
        string levelsUnlocked = m_gameState.levelsUnlocked.ToString();

        // TODO: Remove fake information for testing
        playerName = "eu";
        skillLevel = "9001";
        string fake_waves = Random.Range(1, 10).ToString();
        string fake_arrows = Random.Range(10, 50).ToString();

        string payload = string.Format(
                "\"playerName\":\"{0}\"," +
                "\"skillLevel\":\"{1}\"," +
                "\"stopTime\":\"{2}\"," +
                "\"levelsUnlocked\":\"{3}\"," +
                "\"deviceUniqueIdentifier\":\"{4}\"," +
                "\"deviceModel\":\"{5}\"," +
                "\"operatingSystem\":\"{6}\"," +
                "\"graphicsDeviceVendorId\":\"{7}\"," +
                "\"graphicsDeviceId\":\"{8}\"," +
                "\"graphicsDeviceVersion\":\"{9}\"," +
                "\"graphicsMultiTheaded\":\"{10}\"," +
                "\"graphicsShaderLevel\":\"{11}\"," +
                "\"maxTextureSize\":\"{12}\"," +
                "\"systemMemorySize\":\"{13}\"," +
                "\"graphicsMemorySize\":\"{14}\"," +
                "\"graphicsDeviceVendor\":\"{15}\"," +
                "\"processorCount\":\"{16}\"," +
                "\"processorType\":\"{17}\"," +
                "\"supportedRenderTargetCount\":\"{18}\"," +
                "\"supports2DArrayTextures\":\"{19}\"," +
                "\"supports3DRenderTextures\":\"{20}\"," +
                "\"supports3DTextures\":\"{21}\"," +
                "\"supportsComputeShaders\":\"{22}\"," +
                "\"supportsInstancing\":\"{23},\""
                ,
                playerName,
                skillLevel,
                stopTime,
                levelsUnlocked,
                SystemInfo.deviceUniqueIdentifier, 
                SystemInfo.deviceModel,
                SystemInfo.operatingSystem,
                SystemInfo.graphicsDeviceVendorID,
                SystemInfo.graphicsDeviceID,
                SystemInfo.graphicsDeviceVersion,
                SystemInfo.graphicsMultiThreaded,
                SystemInfo.graphicsShaderLevel,
                SystemInfo.maxTextureSize,
                SystemInfo.systemMemorySize,
                SystemInfo.graphicsMemorySize,
                SystemInfo.graphicsDeviceVendor,
                SystemInfo.processorCount,
                SystemInfo.processorType,
                SystemInfo.supportedRenderTargetCount,
                SystemInfo.supports2DArrayTextures,
                SystemInfo.supports3DRenderTextures,
                SystemInfo.supports3DTextures,
                SystemInfo.supportsComputeShaders,
                SystemInfo.supportsInstancing
            ); // End of JSON payload

        string jsonPayload = "{" + payload + "}";

        Debug.Log("Made JSON payload");
        Debug.Log(jsonPayload);

        APIPost("sessionEnd", jsonPayload);
    }

    public void EndRound() {
        // TODO: Fill
    }

    public void Write<T>(string propertyName, T propertyValue) {
        DB.Insert("eu", "Unity3D", propertyName, propertyValue.ToString());
    }

    public List<string> Read(string propertyName, string dataRange) {
        //TODO: Implement Read()
        /* DB.Select() */
        Debug.Log("TelemetryManager.Read() is not implemented yet...\nSorry o_o");
        return new List<string>();
    }
}
