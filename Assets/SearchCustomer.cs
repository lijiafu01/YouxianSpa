using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchCustomer : MonoBehaviour
{
    public Dropdown userChoose;
    public InputField userInput;
    public Button searchBtn;
    [SerializeField] private GameObject template; // Template to show the result
    [SerializeField] private Transform content; // Content of the ScrollView

    private void Start()
    {
        searchBtn.onClick.AddListener(() => SearchData());

        // Add options to the dropdown
        List<string> options = new List<string>() { "電話號碼", "客戶姓名" };
        userChoose.ClearOptions();
        userChoose.AddOptions(options);
    }

    public void SearchData()
    {
        // Validate input
        if (string.IsNullOrEmpty(userInput.text))
        {
            Debug.Log("Vui l?ng nh?p th?ng tin ?? t?m ki?m!");
            return;
        }

        MySqlConnection connection = Mysql.MysqlConnection();
        connection.Open();

        // Prepare SQL command based on user choice
        string command = "";
        if (userChoose.value == 0) // If the user selected "電話號碼"
        {
            command = $"SELECT * FROM Customer WHERE PhoneNumber LIKE '%{userInput.text}%' LIMIT 3;";
        }
        else // If the user selected "客戶姓名"
        {
            command = $"SELECT * FROM Customer WHERE Name LIKE '%{userInput.text}%' LIMIT 3;";
        }

        // Execute command
        try
        {
            DestroyAllChildren(content.gameObject); // Clear previous results

            MySqlCommand cmd = new MySqlCommand(command, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            List<string> columnNames = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnNames.Add(reader.GetName(i));
            }

            SpawnColumnName(columnNames); // Display column names

            while (reader.Read())
            {
                for (int i = 0; i < columnNames.Count; i++)
                {
                    GameObject obj = Instantiate(template, content);
                    string columnName = columnNames[i];
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
        catch (MySqlException ex)
        {
            Debug.LogError("L?i khi t?m ki?m: " + ex.Message);
        }
        finally
        {
            // Close connection
            connection.Dispose();
            connection.Close();
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

    private void SpawnColumnName(List<string> columnNames)
    {
        foreach (string columnName in columnNames)
        {
            GameObject obj = Instantiate(template, content);
            obj.transform.GetComponent<Text>().text = columnName;
            obj.SetActive(true);
        }
    }
}
