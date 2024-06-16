using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;

public class Revenue : MonoBehaviour
{
    public Button dayRevenueBtn;
    public Button monthRevenueBtn;
    public Button yearRevenueBtn;
    public Text displayRevenue;
    public Text displayCustomers;

    public InputField startDay;
    public InputField endDay;
    public InputField monthInput;
    public Button setDay;



    public void GetDataWithCustomRange()
    {
        // Check if input fields are not empty and in correct format
        if (!string.IsNullOrEmpty(startDay.text) && !string.IsNullOrEmpty(endDay.text) && !string.IsNullOrEmpty(monthInput.text))
        {
            int startDayValue, endDayValue, monthValue;

            // Try parsing input field values to int
            if (int.TryParse(startDay.text, out startDayValue) && int.TryParse(endDay.text, out endDayValue) && int.TryParse(monthInput.text, out monthValue))
            {
                if (startDayValue >= 1 && startDayValue <= 31 && endDayValue >= 1 && endDayValue <= 31 && monthValue >= 1 && monthValue <= 12)
                {
                    MySqlConnection connection = Mysql.MysqlConnection();
                    connection.Open();

                    string sqlRevenue = $"SELECT SUM(Price) as Revenue FROM Receipt WHERE DAY(StartTime) >= {startDayValue} AND DAY(StartTime) <= {endDayValue} AND MONTH(StartTime) = {monthValue} AND YEAR(StartTime) = YEAR(CURDATE())";
                    string sqlCustomers = $"SELECT COUNT(*) as Customers FROM Receipt WHERE DAY(StartTime) >= {startDayValue} AND DAY(StartTime) <= {endDayValue} AND MONTH(StartTime) = {monthValue} AND YEAR(StartTime) = YEAR(CURDATE())";

                    MySqlCommand cmdRevenue = new MySqlCommand(sqlRevenue, connection);
                    MySqlDataReader readerRevenue = cmdRevenue.ExecuteReader();

                    if (readerRevenue.Read())
                    {
                        displayRevenue.text = "營收: " + readerRevenue["Revenue"].ToString() + "元";
                    }

                    readerRevenue.Close();

                    MySqlCommand cmdCustomers = new MySqlCommand(sqlCustomers, connection);
                    MySqlDataReader readerCustomers = cmdCustomers.ExecuteReader();

                    if (readerCustomers.Read())
                    {
                        displayCustomers.text = "客人: " + readerCustomers["Customers"].ToString() + "人";
                    }

                    readerCustomers.Close();
                    connection.Close();
                }
            }
        }
    }

    private void Start()
    {
        dayRevenueBtn.onClick.AddListener(() => GetData("DAY"));
        monthRevenueBtn.onClick.AddListener(() => GetData("MONTH"));
        yearRevenueBtn.onClick.AddListener(() => GetData("YEAR"));
        setDay.onClick.AddListener(GetDataWithCustomRange);
    }


    public void GetData(string timePeriod)
    {
        MySqlConnection connection = Mysql.MysqlConnection();
        connection.Open();
        try
        {
            string sqlRevenue = "SELECT SUM(Price) as Revenue FROM Receipt WHERE ";
            string sqlCustomers = "SELECT COUNT(*) as Customers FROM Receipt WHERE ";

            switch (timePeriod)
            {
                case "DAY":
                    sqlRevenue += "DATE(StartTime) = CURDATE()";
                    sqlCustomers += "DATE(StartTime) = CURDATE()";
                    break;
                case "MONTH":
                    sqlRevenue += "MONTH(StartTime) = MONTH(CURDATE()) AND YEAR(StartTime) = YEAR(CURDATE())";
                    sqlCustomers += "MONTH(StartTime) = MONTH(CURDATE()) AND YEAR(StartTime) = YEAR(CURDATE())";
                    break;
                case "YEAR":
                    sqlRevenue += "YEAR(StartTime) = YEAR(CURDATE())";
                    sqlCustomers += "YEAR(StartTime) = YEAR(CURDATE())";
                    break;
            }

            MySqlCommand cmdRevenue = new MySqlCommand(sqlRevenue, connection);
            MySqlDataReader readerRevenue = cmdRevenue.ExecuteReader();

            if (readerRevenue.Read())
            {
                displayRevenue.text = "營收: " + readerRevenue["Revenue"].ToString() + "元";
            }

            readerRevenue.Close();

            MySqlCommand cmdCustomers = new MySqlCommand(sqlCustomers, connection);
            MySqlDataReader readerCustomers = cmdCustomers.ExecuteReader();

            if (readerCustomers.Read())
            {
                displayCustomers.text = "客人: " + readerCustomers["Customers"].ToString() + "人";
            }
            readerCustomers.Close();
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
}
