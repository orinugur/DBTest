using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Runtime.InteropServices;

public class DatabaseUI : MonoBehaviour
{
   [Header("UI")]
   [SerializeField] InputField Input_Query;
   [SerializeField] Text Text_DBResult;
   [SerializeField] Text Text_Log;

    [Header("connectionInfo")]
    [SerializeField] string _ip = "127.0.0.1";
    [SerializeField] string _dbName = "test";
    [SerializeField] string _uid = "root";
    [SerializeField] string _pws = "1234";

    private bool _isconnectTestComplete;

    private static MySqlConnection _dbConnection;

    private void Awake()
    {
        this.gameObject.SetActive(false);
    }
    private void SendQuery(string querystr, string tableName)
    {
        if(querystr.Contains("SELECT")) //있으면 Select 관련 함수 호출 
        {
            DataSet dataSet = OnSelectRequest(querystr, tableName);
            Text_DBResult.text = DeformatResult(dataSet);
        }
        else //없다면 Insert 또는 Update 관련 커리
        {
            Text_DBResult.text = OnInsertOnUpdateRequest(querystr) ? "성공" : "실패";
        }
        //return dataSet.GetXml().ToString();

    }

    public static bool OnInsertOnUpdateRequest(string query)
    {
        try
        {
            MySqlCommand sqlCommand = new MySqlCommand();
            sqlCommand.Connection = _dbConnection;
            sqlCommand.CommandText = query;



            _dbConnection.Open();
            sqlCommand.ExecuteNonQuery();
            _dbConnection.Close();
            return true;
        }
        catch
        {
            return false;
        }
        
    }
    private string DeformatResult(DataSet dataSet)
    {
        string resultStr = string.Empty;
        foreach(DataTable table in dataSet.Tables)
        {
            foreach(DataRow row in table.Rows)
            {
                foreach(DataColumn column in table.Columns)
                {
                    resultStr += $"{column.ColumnName} : {row[column]}\n";
                }
            }
        }
        return resultStr;
    }
    public static DataSet OnSelectRequest(string query, string tableName)
    {
        try
        {
            _dbConnection.Open();
            MySqlCommand sqlCmd = new MySqlCommand();
            sqlCmd.Connection = _dbConnection;
            sqlCmd.CommandText= query;

            MySqlDataAdapter sd = new MySqlDataAdapter(sqlCmd);
            DataSet dataSet = new DataSet();
            sd.Fill(dataSet, tableName);
            
            _dbConnection.Close();
            return dataSet;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return null;
        }
    }
    public bool ConnectTest()
    {
        string connectStr = $"Server={_ip};Database={_dbName};Uid={_uid};Pwd={_pws};";

        try
        {
            using (MySqlConnection conn = new MySqlConnection(connectStr))
            {
                _dbConnection = conn;
                conn.Open();
            }
            Text_Log.text = "DB 연결을 성공했습니다.";
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Error : {e.ToString()}");
            Text_Log.text = "DB 연결 실패.";
            return false;
        }
    }

    public void OnClick_TestDBConnect()
    {
        _isconnectTestComplete = ConnectTest();
    }

    public void OnSubmit_SendQuery()
    {
        if(_isconnectTestComplete==false)
        {
            Text_Log.text = "DB 연결을 먼저 시도해주세요.";
            return;
        }
        Text_Log.text=string.Empty;
        string query = string.IsNullOrWhiteSpace(Input_Query.text) ? "SELECT U_Name, U_Pass FROM user_info"
            :Input_Query.text;
        SendQuery(query, "user_info");
        
    }

    public void OnClick_OpenDatabaseUI()
    {
        this.gameObject.SetActive(true);
    }

    public void OnClick_CloseDatabaseUI()
    {
        this.gameObject.SetActive(false);
    }

}