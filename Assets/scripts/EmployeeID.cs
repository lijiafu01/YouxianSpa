using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;

public class EmployeeID : MonoBehaviour
{
    public Dropdown employeeDropDown;

    void Start()
    {
        GetData();
        employeeDropDown.onValueChanged.AddListener(delegate { DropdownValueChanged(employeeDropDown); });
    }

    private void GetData()
    {

        var connection = Mysql.MysqlConnection();  // You should replace this with correct connection string.
        try
        {
            connection.Open();
            string sql = "SELECT StaffID FROM Staff";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            employeeDropDown.options.Clear();
            employeeDropDown.options.Add(new Dropdown.OptionData("0"));
            while (reader.Read())
            {
                int id = reader.GetInt32("StaffID");
                employeeDropDown.options.Add(new Dropdown.OptionData(id.ToString()));
            }
        }
        catch
        {

        }
        finally
        {
            connection.Dispose();
            connection.Close();
        }
    }
    void DropdownValueChanged(Dropdown employeeDropDown)
    {
        MainController.Instance.EmployeeID = int.Parse(employeeDropDown.options[employeeDropDown.value].text);
    }

}
