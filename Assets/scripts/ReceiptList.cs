using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using UnityEngine.UI;
using System;

public class ReceiptList : MonoBehaviour
{
    [SerializeField] private GameObject template;
    [SerializeField] private Transform content;
    public InputField monthInput;
    public InputField dayInput;

    public Button monthBtn;
    public Button dayBtn;
    private void OnEnable()
    {
        // Set the current month and day as the default value
        monthInput.text = DateTime.Now.Month.ToString();
        dayInput.text = DateTime.Now.Day.ToString();
        monthInput.onValueChanged.AddListener(delegate { UserBorrowPanel(); });
        dayInput.onValueChanged.AddListener(delegate { UserBorrowPanel(); });
        monthBtn.onClick.AddListener(() => CurrentData("month"));
        dayBtn.onClick.AddListener(() => CurrentData("day"));

        UserBorrowPanel();
    }

    public void UserBorrowPanel()
    {
        DestroyAllChildren(content.gameObject);
        var connection = Mysql.MysqlConnection();
        connection.Open();

        try
        {
            string month = monthInput.text;
            string day = dayInput.text;
            string query = $"SELECT * FROM Receipt WHERE MONTH(StartTime) = {month} AND DAY(StartTime) = {day} AND YEAR(StartTime) = YEAR(CURDATE());";
            string query2 = "SHOW COLUMNS FROM Receipt;";
            MySqlCommand cmd2 = new MySqlCommand(query2, connection);
            MySqlDataReader columnNames = cmd2.ExecuteReader();
            List<string> columnNameList = new List<string>();

            while (columnNames.Read())
            {
                if (columnNames.GetString(0) != "id")  // If the column name is not 'id'
                {
                    columnNameList.Add(columnNames.GetString(0));
                }
            }

            columnNames.Close();

            SpawColumnName(columnNameList);

            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                Debug.Log("No data to display for this day.");
                return;
            }

            while (reader.Read())
            {
                for (int i = 0; i < columnNameList.Count; i++)
                {
                    GameObject obj = Instantiate(template, content);
                    string columnName = columnNameList[i];
                    if (!reader.IsDBNull(reader.GetOrdinal(columnName)))
                    {
                        if (!(columnName == "id"))
                        {
                            DateTime dt;
                            if (columnName == "StartTime" || columnName == "EndTime")
                            {
                                // Parse the date time from the database
                                dt = DateTime.Parse(reader.GetString(columnName));

                                // Format the DateTime object to 24 hours format including the date
                                string time24h = dt.ToString("yyyy-MM-dd\nHH:mm");
                                obj.transform.GetComponent<Text>().text = time24h;
                            }
                            else
                            {
                                obj.transform.GetComponent<Text>().text = reader.GetString(columnName);
                            }
                        }

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
    public void CurrentData(string type)
    {
        DestroyAllChildren(content.gameObject);
        var connection = Mysql.MysqlConnection();
        connection.Open();

        try
        {
            string query;
            if (type == "month")
            {
                string month = DateTime.Now.Month.ToString();
                query = $"SELECT * FROM Receipt WHERE MONTH(StartTime) = {month} AND YEAR(StartTime) = YEAR(CURDATE());";
            }
            else  // type == "day"
            {
                string day = DateTime.Now.Day.ToString();
                query = $"SELECT * FROM Receipt WHERE DAY(StartTime) = {day} AND YEAR(StartTime) = YEAR(CURDATE());";
            }

            string query2 = "SHOW COLUMNS FROM Receipt;";
            MySqlCommand cmd2 = new MySqlCommand(query2, connection);
            MySqlDataReader columnNames = cmd2.ExecuteReader();
            List<string> columnNameList = new List<string>();

            while (columnNames.Read())
            {
                if (columnNames.GetString(0) != "id")  // If the column name is not 'id'
                {
                    columnNameList.Add(columnNames.GetString(0));
                }
            }

            columnNames.Close();

            SpawColumnName(columnNameList);

            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                Debug.Log("No data to display for this " + type + ".");
                return;
            }

            while (reader.Read())
            {
                for (int i = 0; i < columnNameList.Count; i++)
                {
                    GameObject obj = Instantiate(template, content);
                    string columnName = columnNameList[i];
                    if (!reader.IsDBNull(reader.GetOrdinal(columnName)))
                    {
                        if (!(columnName == "id"))
                        {
                            DateTime dt;
                            if (columnName == "StartTime" || columnName == "EndTime")
                            {
                                // Parse the date time from the database
                                dt = DateTime.Parse(reader.GetString(columnName));

                                // Format the DateTime object to 24 hours format including the date
                                string time24h = dt.ToString("yyyy-MM-dd\nHH:mm");
                                obj.transform.GetComponent<Text>().text = time24h;
                            }
                            else
                            {
                                obj.transform.GetComponent<Text>().text = reader.GetString(columnName);
                            }
                        }

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
