using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using MySql.Data.MySqlClient;

public class SQLDBModule {
    private MySqlConnection connection;
    private string url;
    private string database;
    private string user;
    private string password;

    public SQLDBModule() {
        Initialize();
    }

    private void Initialize() {
        url = TelemetryManager.instance.DB_URL;
        database = TelemetryManager.instance.DB_DATABASE;
        user = TelemetryManager.instance.DB_USER;
        password = TelemetryManager.instance.DB_PASSWORD;

        string connectionString = "Data Source=" + url + ";" +
                                  "Initial Catalog=" + database + ";" +
                                  "User id=" + user + ";" +
                                  "Password=" + password + ";";

        connection = new MySqlConnection(connectionString);
    }

    private bool OpenConnection() {
        try {
            connection.Open();
            return true;
        }
        catch (MySqlException ex) {
            //When handling errors, you can your application's response based 
            //on the error number.
            //The two most common error numbers when connecting are as follows:
            //0: Cannot connect to server.
            //1045: Invalid user name and/or password.
            switch (ex.Number) {
                case 0:
                    Debug.Log("Cannot connect to server.  Contact administrator");
                    break;

                case 1045:
                    Debug.Log("Invalid username/password, please try again");
                    break;
            }
            return false;
        }
    }        

    private bool CloseConnection() {
        try {
            connection.Close();
            return true;
        }
        catch (MySqlException ex) {
            Debug.Log(ex.Message);
            return false;
        }
    }

    public void Insert(string userId, string deviceType, string propertyName, string propertyValue) {
        string TableName = "property";
        string query = string.Format("INSERT INTO {0} (userId, deviceType, name, value) VALUES ('{1}', '{2}', '{3}', '{4}')", TableName, userId, deviceType, propertyName, propertyValue);
        Debug.Log("Sending QUERY: " + query);

        if (this.OpenConnection() == true) {
            //create command and assign the query and connection from the constructor
            MySqlCommand cmd = new MySqlCommand(query, connection);
            
            //Execute command
            //  NonQuery means it does not return anything.
            //  TODO: Have the DB return uniqueId
            cmd.ExecuteNonQuery();

            this.CloseConnection();
        }
    }

    public void Update() {
        // TODO: Change query
        string query = "UPDATE tableinfo SET name='Joe', age='22' WHERE name='John Smith'";
        Debug.Log("Sending QUERY: " + query);

        if (this.OpenConnection() == true) {
            //create mysql command
            MySqlCommand cmd = new MySqlCommand();
            //Assign the query using CommandText
            cmd.CommandText = query;
            //Assign the connection using Connection
            cmd.Connection = connection;

            //Execute query
            cmd.ExecuteNonQuery();

            this.CloseConnection();
        }
    }

    public void Delete() {
        // TODO: Change query
        string query = "DELETE FROM tableinfo WHERE name='John Smith'";

        if (this.OpenConnection() == true) {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            this.CloseConnection();
        }
    }

    public void DeleteAll() {
        string TableName = "property";
        string query = "DELETE FROM " + TableName;

        if (this.OpenConnection() == true) {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            this.CloseConnection();
        }

        Debug.Log("Deleted all entries for " + TableName + " table.");
    }

    public List<List<string>> Select() {
        // TODO: Make new function to select a single item
        string TableName = "property";
        // string query = string.Format("INSERT INTO {0} (userId, deviceType, propertyName, propertyValue) VALUES ('{1}', '{2}', '{3}', '{4}')", TableName, userId, deviceType, propertyName, propertyValue);
        string query = "SELECT * FROM property";
        Debug.Log("Sending QUERY: " + query);

        List<List<string>> payload = new List<List<string>>();

        if (this.OpenConnection() == true) {
            //Create Command
            MySqlCommand cmd = new MySqlCommand(query, connection);
            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();
            
            while (dataReader.Read()) {
                payload.Add(new List<string> {
                        dataReader["userId"].ToString(),
                        dataReader["deviceType"].ToString(),
                        dataReader["name"].ToString(), 
                        dataReader["value"].ToString(), 
                        dataReader["created"].ToString(), 
                        dataReader["modified"].ToString()
                });
            }

            dataReader.Close();
            this.CloseConnection();

            // Transpose rows and columns
            /* List<List<string>> result = payload */
            /*     .SelectMany(inner => inner.Select((item, index) => new { item, index })) */
            /*     .GroupBy(i => i.index, i => i.item) */
            /*     .Select(g => g.ToList()) */
            /*     .ToList(); */
        }
        else {
            Debug.Log("Couldn't read form database.");
        }
        return payload;
    }

    public int Count() {
        // TODO: Change tablename
        string query = "SELECT Count(*) FROM tableinfo";
        int Count = -1;

        if (this.OpenConnection() == true) {
            //Create Mysql Command
            MySqlCommand cmd = new MySqlCommand(query, connection);

            //ExecuteScalar will return one value
            Count = int.Parse(cmd.ExecuteScalar()+"");
            
            this.CloseConnection();

            return Count;
        }
        else {
            return Count;
        }
    }

    // TODO: Implement backup and restore

    /*
    public void Backup() {
        try {
            DateTime Time = DateTime.Now;
            int year = Time.Year;
            int month = Time.Month;
            int day = Time.Day;
            int hour = Time.Hour;
            int minute = Time.Minute;
            int second = Time.Second;
            int millisecond = Time.Millisecond;

            //Save file to C:\ with the current date as a filename
            string path;
            path = "C:\\MySqlBackup" + year + "-" + month + "-" + day + "-" + hour + "-" + minute + "-" + second + "-" + millisecond + ".sql";
            StreamWriter file = new StreamWriter(path);
            
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "mysqldump";
            psi.RedirectStandardInput = false;
            psi.RedirectStandardOutput = true;
            psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", 
                uid, password, server, database);
            psi.UseShellExecute = false;

            Process process = Process.Start(psi);

            string output;
            output = process.StandardOutput.ReadToEnd();
            file.WriteLine(output);
            process.WaitForExit();
            file.Close();
            process.Close();
        }
        catch (IOException ex) {
            Debug.Log("Error , unable to backup!");
        }
    }

    public void Restore() {
        try {
            //Read file from C:\
            string path;
            path = "C:\\MySqlBackup.sql";
            StreamReader file = new StreamReader(path);
            string input = file.ReadToEnd();
            file.Close();

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "mysql";
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = false;
            psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", 
                uid, password, server, database);
            psi.UseShellExecute = false;
            
            Process process = Process.Start(psi);
            process.StandardInput.WriteLine(input);
            process.StandardInput.Close();
            process.WaitForExit();
            process.Close();
        }
        catch (IOException ex) {
            Debug.Log("Error , unable to Restore!");
        }
    }
*/
}
