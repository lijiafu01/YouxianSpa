using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddBlackList : MonoBehaviour
{
    public InputField nameInput;
    public InputField phoneNumberInput;
    public InputField describeInput;

    public Toggle man;
    public Toggle woman;

    public Button AddDataBtn;
    public GameObject failTab;
    public GameObject successTab;

    private void Start()
    {
        woman.isOn = false;
        man.isOn = false;
        // Add listener to toggles
        man.onValueChanged.AddListener(OnManToggle);
        woman.onValueChanged.AddListener(OnWomanToggle);

        AddDataBtn.onClick.AddListener(() => InsertData());
    }

    public void OnManToggle(bool isOn)
    {
        if (isOn)
        {
            woman.isOn = false;
        }
    }
    public void OnWomanToggle(bool isOn)
    {
        if (isOn)
        {
            man.isOn = false;
        }
    }
    public void InsertData()
    {
        // Validate input
        if (string.IsNullOrEmpty(nameInput.text) || string.IsNullOrEmpty(phoneNumberInput.text))
        {
            failTab.SetActive(true);
            return;
        }

        string gender = "未知";
        if (man.isOn)
        {
            gender = "男";
        }
        else if (woman.isOn)
        {
            gender = "女";
        }

        string describe = string.IsNullOrEmpty(describeInput.text) ? "沒有資訊" : describeInput.text;

        MySqlConnection connection = Mysql.MysqlConnection();
        connection.Open();

        // Check if the phone number already exists
        string checkCommand = $"SELECT PhoneNumber FROM BlackList WHERE PhoneNumber='{phoneNumberInput.text}';";
        MySqlCommand checkCmd = new MySqlCommand(checkCommand, connection);
        MySqlDataReader reader = checkCmd.ExecuteReader();

        if (reader.HasRows)
        {
            Debug.LogError("PhoneNumber ?? t?n t?i!");
            reader.Close();
            connection.Close();
            return;
        }

        reader.Close();

        // Prepare SQL command
        string command = $"INSERT INTO BlackList (Name, PhoneNumber, Gender, `describe`) VALUES ('{nameInput.text}', '{phoneNumberInput.text}', '{gender}', '{describe}');";

        // Execute command
        try
        {
            MySqlCommand cmd = new MySqlCommand(command, connection);
            cmd.ExecuteNonQuery();
            successTab.SetActive(true);
        }
        catch 
        {
            failTab.SetActive(true);
        }
        finally
        {
            // Close connection
            connection.Dispose();
            connection.Close();
        }
    }

}
