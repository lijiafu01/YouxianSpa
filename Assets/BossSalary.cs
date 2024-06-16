using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;

public class BossSalary : MonoBehaviour
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
            string sqlRevenue = $"SELECT Price as Revenue,Category,TIMESTAMPDIFF(MINUTE, StartTime, EndTime) as WorkingMinutes FROM Receipt WHERE ";
            string sqlCustomers = $"SELECT COUNT(*) as Customers FROM Receipt WHERE ";

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

            int totalRevenue = 0;

            while (readerRevenue.Read())
            {
                string category = readerRevenue.GetString("Category");
                int workingMinutes = Convert.ToInt32(readerRevenue["WorkingMinutes"]);
                int price = int.Parse(readerRevenue["Revenue"].ToString());
                int priceMax = int.Parse(MainController.Instance.Price_you1.text);
                if (category == "油壓")
                {
                    if (workingMinutes == 60)
                    {
                        totalRevenue += (price - 400);
                    }
                    else if (workingMinutes == 120)
                    {
                        totalRevenue += (price - 800);
                    }
                }
                else if (category == "指壓")
                {
                    if (workingMinutes == 60)
                    {
                        totalRevenue += (price - 400);
                    }
                    else if (workingMinutes == 120)
                    {
                        totalRevenue += (price - 800);
                    }
                }
            }

            readerRevenue.Close();

            displayRevenue.text = "營收: " + totalRevenue.ToString() + "元";

            MySqlCommand cmdCustomers = new MySqlCommand(sqlCustomers, connection);
            MySqlDataReader readerCustomers = cmdCustomers.ExecuteReader();

            if (readerCustomers.Read())
            {
                displayCustomers.text = "客人: " + readerCustomers["Customers"].ToString() + "人";
            }
            readerCustomers.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
        finally
        {
            connection.Close();
        }
    }

    public void GetDataWithCustomRange()
    {
        if (!string.IsNullOrEmpty(startDay.text) && !string.IsNullOrEmpty(endDay.text) && !string.IsNullOrEmpty(monthInput.text))
        {
            int startDayValue, endDayValue, monthValue;

            if (int.TryParse(startDay.text, out startDayValue) && int.TryParse(endDay.text, out endDayValue) && int.TryParse(monthInput.text, out monthValue))
            {
                if (startDayValue >= 1 && startDayValue <= 31 && endDayValue >= 1 && endDayValue <= 31 && monthValue >= 1 && monthValue <= 12)
                {
                    MySqlConnection connection = Mysql.MysqlConnection();
                    connection.Open();

                    //string sqlRevenue = $"SELECT Price as Revenue FROM Receipt WHERE DAY(StartTime) >= {startDayValue} AND DAY(StartTime) <= {endDayValue} AND MONTH(StartTime) = {monthValue} AND YEAR(StartTime) = YEAR(CURDATE())";
                    string sqlQuery = $"SELECT Category, Price as Revenue, TIMESTAMPDIFF(MINUTE, StartTime, EndTime) as WorkingMinutes FROM Receipt WHERE DAY(StartTime) >= {startDayValue} AND DAY(StartTime) <= {endDayValue} AND MONTH(StartTime) = {monthValue} AND YEAR(StartTime) = YEAR(CURDATE())";
                    MySqlCommand cmdRevenue = new MySqlCommand(sqlQuery, connection);
                    MySqlDataReader readerRevenue = cmdRevenue.ExecuteReader();

                    int totalRevenue = 0;

                    while (readerRevenue.Read())
                    {
                        string category = readerRevenue.GetString("Category");
                        int workingMinutes = Convert.ToInt32(readerRevenue["WorkingMinutes"]);
                        int price = int.Parse(readerRevenue["Revenue"].ToString());
                        if(category == "油壓")
                        {
                            if(workingMinutes == 60)
                            {
                                totalRevenue += (price - 400);
                            }
                            else if(workingMinutes == 120)
                            {
                                totalRevenue += (price - 800);
                            }
                        }
                        else if(category == "指壓")
                        {
                            if (workingMinutes == 60)
                            {
                                totalRevenue += (price - 400);
                            }
                            else if (workingMinutes == 120)
                            {
                                totalRevenue += (price - 800);
                            }
                        }
                        /*// Subtract the staff salary from the price
                        if (price > priceMax)
                            totalRevenue += (price - 800);
                        else if (price <= priceMax)
                            totalRevenue += (price - 400);*/
                    }

                    readerRevenue.Close();

                    displayRevenue.text = "營收: " + totalRevenue.ToString() + "元";

                    string sqlCustomers = $"SELECT COUNT(*) as Customers FROM Receipt WHERE DAY(StartTime) >= {startDayValue} AND DAY(StartTime) <= {endDayValue} AND MONTH(StartTime) = {monthValue} AND YEAR(StartTime) = YEAR(CURDATE())";
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
}

