using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddCustomer : MonoBehaviour
{
    public InputField nameInput;
    public InputField phoneNumberInput;
    public InputField describeInput;

    public Toggle man;
    public Toggle woman;

    public Button AddDataBtn;

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
            Debug.Log("Th?ng tin ch?a ??y ??!");
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
        string checkCommand = $"SELECT PhoneNumber FROM Customer WHERE PhoneNumber='{phoneNumberInput.text}';";
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
        string command = $"INSERT INTO Customer (Name, PhoneNumber, Gender, `describe`) VALUES ('{nameInput.text}', '{phoneNumberInput.text}', '{gender}', '{describe}');";

        // Execute command
        try
        {
            MySqlCommand cmd = new MySqlCommand(command, connection);
            cmd.ExecuteNonQuery();
            Debug.Log("Th?m d? li?u th?nh c?ng!");
        }
        catch (MySqlException ex)
        {
            Debug.LogError("L?i khi th?m d? li?u: " + ex.Message);
        }
        finally
        {
            // Close connection
            connection.Dispose();
            connection.Close();
        }
    }

}
