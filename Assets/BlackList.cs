using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using UnityEngine.UI;
using System;


public class BlackList : MonoBehaviour
{
    [SerializeField] private GameObject template;
    [SerializeField] private Transform content;
    

    private void OnEnable()
    {
        UserBorrowPanel();
    }

    public void UserBorrowPanel()
    {
        DestroyAllChildren(content.gameObject);
        var connection = Mysql.MysqlConnection();
        connection.Open();

        try
        {
            string query = "SELECT * FROM BlackList;";
            string query2 = "SHOW COLUMNS FROM BlackList;";
            MySqlCommand cmd2 = new MySqlCommand(query2, connection);
            MySqlDataReader columnNames = cmd2.ExecuteReader();
            List<string> columnNameList = new List<string>();

            while (columnNames.Read())
            {
                columnNameList.Add(columnNames.GetString(0));
            }

            columnNames.Close();

            SpawColumnName(columnNameList);

            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                for (int i = 0; i < columnNameList.Count; i++)
                {
                    GameObject obj = Instantiate(template, content);
                    string columnName = columnNameList[i];
                    if (!reader.IsDBNull(reader.GetOrdinal(columnName)))
                    {
                        obj.transform.GetComponent<Text>().text = reader.GetString(columnName);
                    }
                    else
                    {
                        obj.transform.GetComponent<Text>().text = "NULL";
                    }
                    obj.SetActive(true);
                }
            }

            reader.Close();
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.Message);
        }
        finally
        {
            connection.Close();
            connection.Dispose();
        }
    }

    private void SpawColumnName(List<string> columnNames)
    {
        foreach (string columnName in columnNames)
        {
            GameObject obj = Instantiate(template, content);
            obj.transform.GetComponent<Text>().text = columnName;
            obj.SetActive(true);
        }
    }

    public void DestroyAllChildren(GameObject parent)
    {
        for (int i = parent.transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            Destroy(child);
        }
    }
}
