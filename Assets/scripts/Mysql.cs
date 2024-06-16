using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;

public class Mysql : MonoBehaviour
{
    // su dung singleton pattern de cai thien hieu suat va bo nho
    private static MySqlConnection mySqlConnection;
    static string host = "127.0.0.1";
    static string user = "root";
    static string pwd = "Viethoan2001";
    static string db = "youxianapp";
    static string port = "3306";

    public static MySqlConnection MysqlConnection()
    {

        if (mySqlConnection == null)
        {
            mySqlConnection = Open();
        }
        return mySqlConnection;
    }
    private static MySqlConnection Open()
    {
        string connectionString = string.Format("Server={0};port={4};Database={1};User ID={2};Password={3};", host, db, user, pwd, port);
        MySqlConnection conn = new MySqlConnection(connectionString);
        return conn;
    }

}
