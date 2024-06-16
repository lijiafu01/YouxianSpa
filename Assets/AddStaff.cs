using System;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;

public class AddStaff : MonoBehaviour
{
    public InputField idInput;
    public InputField nameInput;
    public InputField birthInput;
    public InputField describeInput;
    public Toggle manToggle;
    public Toggle womanToggle;
    public Button saveDataBtn;
    public Text erollText;
    public GameObject errorTab;
    public GameObject successTab;

    private void Start()
    {
        womanToggle.isOn = false;
        manToggle.isOn = false;
        manToggle.onValueChanged.AddListener(OnManToggle);
        womanToggle.onValueChanged.AddListener(OnWomanToggle);
        saveDataBtn.onClick.AddListener(SaveDataBtn);
    }

    public void OnManToggle(bool isOn)
    {
        if (isOn)
        {
            womanToggle.isOn = false;
        }
    }

    public void OnWomanToggle(bool isOn)
    {
        if (isOn)
        {
            manToggle.isOn = false;
        }
    }

    public void SaveDataBtn()
    {
        var connection = Mysql.MysqlConnection();
        connection.Open();

        try
        {
            string errors = "";

            if (string.IsNullOrEmpty(idInput.text))
                errors += "編號未填, ";
            if (string.IsNullOrEmpty(nameInput.text))
                errors += "姓名未填, ";
            if (string.IsNullOrEmpty(birthInput.text))
                errors += "生日未填, ";
            if (string.IsNullOrEmpty(describeInput.text))
                errors += "聯絡方式未填, ";
            if (!manToggle.isOn && !womanToggle.isOn)
                errors += "性別未填, ";

            if (errors != "")
            {
                errors = errors.Remove(errors.Length - 2); // Removing last comma and space
                erollText.text =  errors;
                erollText.color =  Color.red;
                errorTab.SetActive(true);
                return;
            }

            string sex = manToggle.isOn ? "男" : "女";

            string sql = "INSERT INTO Staff (StaffID, Name, birth, sex, ContactInfo) VALUES (@id, @name, @birth, @sex, @describe)";

            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@id", idInput.text);
                cmd.Parameters.AddWithValue("@name", nameInput.text);
                cmd.Parameters.AddWithValue("@birth", birthInput.text);
                cmd.Parameters.AddWithValue("@sex", sex);
                cmd.Parameters.AddWithValue("@describe", describeInput.text);

                cmd.ExecuteNonQuery();
            }

            successTab.SetActive(true);
            idInput.text = "";
            nameInput.text = "";
            birthInput.text = "";
            describeInput.text = "";
            manToggle.isOn = false;
            womanToggle.isOn = false;

        }
        catch (Exception ex)
        {
            Debug.Log("An error occurred: " + ex.Message);
        }
        finally
        {
            connection.Dispose();
            connection.Close();
        }
    }
}
