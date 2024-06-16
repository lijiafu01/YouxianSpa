using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using UnityEngine.UI;
using System;


public class UserApproval : MonoBehaviour
{
    [SerializeField] private GameObject warningTab;
    [SerializeField] private GameObject BtnO_template;
    [SerializeField] private GameObject BtnX_template;
    [SerializeField] private GameObject template;
    [SerializeField] private Transform content;
    private Dictionary<string, GameObject> btnDict = new Dictionary<string, GameObject>();
    private void OnEnable()
    {
        UserApprovalPanel();
    }

    public void UserApprovalPanel()
    {
        DestroyAllChildren(content.gameObject);
        var connection = Mysql.MysqlConnection();
        connection.Open();

        try
        {
            string query = "SELECT * FROM Invoice;";
            string query2 = "SHOW COLUMNS FROM Invoice;";
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

                int id = reader.GetInt32("id");

                for (int i = 1; i < columnNameList.Count; i++)
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
                for (int i = 1; i < columnNameList.Count; i++)
                {
                    if (i == 4)
                    {
                        GameObject obj = Instantiate(BtnX_template, content);
                        Button button = obj.GetComponent<Button>();
                        obj.SetActive(true);
                        if (button != null)
                        {

                            button.onClick.AddListener(() =>
                            {
                                
                                var connection = Mysql.MysqlConnection();
                                connection.Open();
                                string query3 = $"DELETE FROM invoice WHERE id = {id}";
                                MySqlCommand cmd3 = new MySqlCommand(query3, connection);
                                cmd3.ExecuteNonQuery();
                                connection.Close();
                                connection.Dispose();
                                UserApprovalPanel();
                            });
                        }
                    }
                    else if (i == 5)
                    {
                        GameObject obj = Instantiate(BtnO_template, content);
                        Button button = obj.GetComponent<Button>();
                        obj.SetActive(true);

                        if (button != null)
                        {
                            int staffID = reader.GetInt32("StaffID");
                            string category = reader.GetString("Category");
                            DateTime startTime = reader.GetDateTime("StartTime");
                            DateTime endTime = reader.GetDateTime("EndTime");
                            int price = reader.GetInt32("Price");

                            button.onClick.AddListener(() =>
                            {
                                var connection = Mysql.MysqlConnection();
                                connection.Open();
                                try
                                {
                                    string checkQuery = $"SELECT COUNT(*) FROM Receipt WHERE StaffID = @StaffID AND ((@startTime BETWEEN StartTime AND EndTime) OR (@endTime BETWEEN StartTime AND EndTime)) AND DATE(StartTime) = DATE(@startTime) AND DATE(EndTime) = DATE(@endTime)";
                                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection);
                                    checkCmd.Parameters.AddWithValue("@StaffID", staffID);
                                    checkCmd.Parameters.AddWithValue("@startTime", startTime);
                                    checkCmd.Parameters.AddWithValue("@endTime", endTime);
                                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                                    if (count > 0)
                                    {
                                        warningTab.SetActive(true);
                                        return;
                                    }
                                    else
                                    {
                                        // Save the data
                                        string insertQuery = "INSERT INTO Receipt (id, StaffID, Category, StartTime, EndTime, Price) VALUES (@id,@staffID, @category, @startTime, @endTime, @price)";
                                        using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                                        {
                                            command.Parameters.AddWithValue("@id", id);
                                            command.Parameters.AddWithValue("@staffID", staffID);
                                            command.Parameters.AddWithValue("@category", category);
                                            command.Parameters.AddWithValue("@startTime", startTime);
                                            command.Parameters.AddWithValue("@endTime", endTime);
                                            command.Parameters.AddWithValue("@price", price);

                                            command.ExecuteNonQuery();
                                        }
                                        string query3 = $"DELETE FROM invoice WHERE id = {id}";
                                        MySqlCommand cmd3 = new MySqlCommand(query3, connection);
                                        cmd3.ExecuteNonQuery();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.Log("Error: " + ex.ToString());
                                }
                                finally
                                {
                                    connection.Close();
                                    connection.Dispose();
                                    // Refresh the list to display new data
                                    UserApprovalPanel();
                                }
                            });

                        }
                    }
                    else
                    {
                        GameObject obj = Instantiate(template, content);
                        obj.SetActive(true);
                        obj.transform.GetComponent<Text>().text = "";

                    }
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
            if(!(columnName == "id"))
            {
                GameObject obj = Instantiate(template, content);
                obj.transform.GetComponent<Text>().text = columnName;
                obj.SetActive(true);
            }
            
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
