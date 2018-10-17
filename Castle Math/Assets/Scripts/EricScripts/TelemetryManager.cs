using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TelemetryManager : MonoBehaviour {
    public static TelemetryManager instance;

    private static SQLDBModule DB;

    // TODO: Add values for user and deviceType from the program
    public static string user;
    public static string deviceType;
    // TODO: Keep database details private and outside git
    public string DB_URL;
    public string DB_DATABASE;
    public string DB_USER;
    public string DB_PASSWORD;

    private void Awake() {
        instance = this;
        DontDestroyOnLoad(this);
        // TODO: Understand why GameMetrics was attaching to UnityInitializer
        /* UnityInitializer.AttachToGameObject(this.gameObject); */
    }

    private void Start() {
        DB = new SQLDBModule();
        // TODO: Add values for user and deviceType from the program
        user = "user";
        deviceType = "Unity Editor";
        /* SaveTable(DB.Select()); */
    }

    private void Update() {
        /// <summary>
        /// Unity Function that runs after every screen update
        /// </summary>
        TestDB();
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
                                      "|",
                                      "Time", "User", "Device", "Property", "Value", "Modified");
        foreach (List<string> row in list) {
            table += string.Format("| {4,22} " +
                                   "| {0,22} " +
                                   "| {1,22} " +
                                   "| {2,22} " +
                                   "| {3,22} " +
                                   "| {5,22} " +
                                   "|\n",
                                   row[0], row[1], row[2], row[3], row[4], row[5]);
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
                                "Modified: {5}",
                                item[0], item[1], item[2], item[3], item[4], item[5]));
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
            Write("Property2", "1");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            Write("Property3", 1.2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            Debug.Log("DELETING ALL ROWS!!!\nYou have 5 seconds to cancel!!!");
            System.Threading.Thread.Sleep(5000);

            DB.DeleteAll();
        }
    }

    public void Write<T>(string propertyName, T propertyValue) {
        DB.Insert(user, deviceType, propertyName, propertyValue.ToString());
    }

    public List<string> Read(string propertyName, string dataRange) {
        //TODO: Implement Read()
        /* DB.Select() */
        return new List<string>();
    }
}
