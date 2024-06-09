using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;

public class AccountUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] InputField Input_Id;
    [SerializeField] InputField Input_Passward;
    [SerializeField] internal Text Text_DBResult;
    [SerializeField] internal Text Text_Log;

    [Header("LoginPopUp")]

    [SerializeField] InputField logInput_Id;
    [SerializeField] InputField logInput_Passward;

    [Header("RoomPopUp")]
    [SerializeField] GameObject room_UI;

    [Header("connectionInfo")]
    [SerializeField] string _ip = "127.0.0.1";
    [SerializeField] string _dbName = "test";
    [SerializeField] string _uid = "root";
    [SerializeField] string _pws = "1234";

    List<string> DB_UserName;

    private bool _isconnectTestComplete;

    private static MySqlConnection _dbConnection;

    private void Awake()
    {
        this.gameObject.SetActive(false);
    }

    private void SendQuery(string querystr, string tableName)
    {
        if (querystr.Contains("SELECT"))
        {
            DataSet dataSet = OnSelectRequest(querystr, tableName);
            if (dataSet != null)
            {
                Text_DBResult.text = DeformatResult(dataSet);
                Text_DBResult.gameObject.SetActive(true);
            }
            else
            {
                Text_DBResult.text = "데이터를 불러오는 중 오류가 발생했습니다.";
                Text_DBResult.gameObject.SetActive(true);
            }
        }
        else
        {
            Text_DBResult.gameObject.SetActive(true);
            Text_DBResult.text = OnInsertOnUpdateRequest(querystr) ? "성공" : "실패";
        }
    }

    public bool OnInsertOnUpdateRequest(string query)
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
        catch (Exception e)
        {
            string Log = e.ToString();
            if (Log.Contains("Duplicate"))
            {
                Debug.Log("중복");
            }
            _dbConnection.Close();
            return false;
        }
    }

    private string DeformatResult(DataSet dataSet)
    {
        string resultStr = string.Empty;
        foreach (DataTable table in dataSet.Tables)
        {
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
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
            sqlCmd.CommandText = query;

            MySqlDataAdapter sd = new MySqlDataAdapter(sqlCmd);
            DataSet dataSet = new DataSet();
            sd.Fill(dataSet, tableName);

            _dbConnection.Close();
            return dataSet;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            _dbConnection.Close();
            return null;
        }
    }

    public bool ConnectDB()
    {
        string connectStr = $"Server={_ip};Database={_dbName};Uid={_uid};Pwd={_pws};";
        Debug.Log(connectStr);
        Debug.Log("connectDB");
        try
        {
            _dbConnection = new MySqlConnection(connectStr);
            _dbConnection.Open();
            Text_Log.text = "DB 연결을 성공했습니다.";
            Text_Log.gameObject.SetActive(true);
            gameObject.SetActive(true);
            _dbConnection.Close();
            Debug.Log("connectDBsusekse");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Error : {e.ToString()}");
            Text_Log.text = "서버 연결 실패.";
            Debug.Log("fall");
            return false;
        }
    }

    public void OnClick_TestDBConnect()
    {
        _isconnectTestComplete = ConnectDB();
    }

    public void OnSubmit_SendQuery()
    {
        if (!_isconnectTestComplete)
        {
            Text_DBResult.text = "DB 연결을 먼저 시도해주세요.";
            return;
        }

        Text_DBResult.text = string.Empty;

        if (!string.IsNullOrEmpty(Input_Id.text) && !string.IsNullOrEmpty(Input_Passward.text))
        {
            //string query = "SELECT COUNT(*) FROM user_info WHERE U_Name = @UserName";
            //SendQuery(query, "user_info");

            string query2 = $"INSERT INTO user_info(U_Name, U_Pass) VALUES('{Input_Id.text}', '{Input_Passward.text}')";
            SendQuery(query2, "user_info");
            Debug.Log(query2);
        }
    }




    private void SendLoginQuery(string querystr, string tableName)
    {
        if (querystr.Contains("SELECT"))
        {
            DataSet dataSet = OnSelectRequest(querystr, tableName);
            if (dataSet != null)
            {
                Text_DBResult.text = DeformatResult(dataSet);
                Text_DBResult.gameObject.SetActive(true);
            }
            else
            {
                Text_DBResult.text = "데이터를 불러오는 중 오류가 발생했습니다.";
                Text_DBResult.gameObject.SetActive(true);
            }
        }
        else
        {
            Text_DBResult.gameObject.SetActive(true);
            Text_DBResult.text = OnInsertOnUpdateRequest(querystr) ? "성공" : "실패";
        }
    }

    public bool ConnectLogin()
    {
        string connectStr = $"Server={_ip};Database={_dbName};Uid={_uid};Pwd={_pws};";
        Debug.Log(connectStr);
        Debug.Log("connectDB");
        try
        {
            _dbConnection = new MySqlConnection(connectStr);
            _dbConnection.Open();
            _dbConnection.Close();
            Debug.Log("connectDBsusekse");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Error : {e.ToString()}");
            Debug.Log("fall");
            return false;
        }
    }


    public void OnClickLogin()
    {
        if (ConnectLogin())
        {
            if (!string.IsNullOrEmpty(logInput_Id.text) && !string.IsNullOrEmpty(logInput_Passward.text))
            {
                string query = $"SELECT COUNT(1) FROM user_info WHERE U_Name= '{logInput_Id.text}' AND U_Pass= '{logInput_Passward.text}'";
                bool loginSuccess = OnLoginRequest(query);
                Text_Log.text = loginSuccess ? "성공" : "실패";
                Text_Log.gameObject.SetActive(true);
                Debug.Log(loginSuccess ? "로그인 성공" : "로그인 실패");
            }
            else
            {
                Debug.Log("아이디와 비밀번호를 다시 입력하세요.");
            }
        }
        else
        {
            Debug.Log("데이터베이스 연결 실패");
        }
    }

    public bool OnLoginRequest(string query)
    {
        try
        {
            MySqlCommand sqlCommand = new MySqlCommand(query, _dbConnection);
            _dbConnection.Open();
            int count = Convert.ToInt32(sqlCommand.ExecuteScalar());
            _dbConnection.Close();
            if(count> 0)
            {
                room_UI.SetActive(true);
            }
            return count == 1;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Error : {e.ToString()}");
            _dbConnection.Close();
            return false;
        }
    }


    public void OnClick_OpenDatabaseUI()
    {
        this.gameObject.SetActive(true);
    }

    public void OnClick_CloseUI()
    {
        this.gameObject.SetActive(false);
    }
}
