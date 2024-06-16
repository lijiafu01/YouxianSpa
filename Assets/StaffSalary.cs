using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;

public class StaffSalary : MonoBehaviour
{
    public Button dayRevenueBtn;
    public Button monthRevenueBtn;
    public Button yearRevenueBtn;
    public Text displayRevenue;
    public Text displayCustomers;

    public InputField startDay;
    public InputField endDay;
    public InputField monthInput;
    public InputField staffIdInput;
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
        int staffId = int.Parse(staffIdInput.text);

        MySqlConnection connection = Mysql.MysqlConnection();
        connection.Open();
        try
        {
            int priceMax = int.Parse(MainController.Instance.Price_you1.text);
            string sqlSalary = $@"SELECT SUM(
                        CASE 
                            WHEN Category = '油壓' AND TIMESTAMPDIFF(MINUTE, StartTime, EndTime) = 60 THEN 400 
                            WHEN Category = '油壓' AND TIMESTAMPDIFF(MINUTE, StartTime, EndTime) = 120 THEN 800 
                            WHEN Category = '指壓' AND TIMESTAMPDIFF(MINUTE, StartTime, EndTime) = 60 THEN 400 
                            WHEN Category = '指壓' AND TIMESTAMPDIFF(MINUTE, StartTime, EndTime) = 120 THEN 800 
                            ELSE 0 
                        END) as Salary 
                    FROM Receipt 
                    WHERE StaffID = {staffId} AND ";
            string sqlCustomers = $"SELECT COUNT(*) as Customers FROM Receipt WHERE StaffID = {staffId} AND ";

            switch (timePeriod)
            {
                case "DAY":
                    sqlSalary += "DATE(StartTime) = CURDATE()";
                    sqlCustomers += "DATE(StartTime) = CURDATE()";
                    break;
                case "MONTH":
                    sqlSalary += "MONTH(StartTime) = MONTH(CURDATE()) AND YEAR(StartTime) = YEAR(CURDATE())";
                    sqlCustomers += "MONTH(StartTime) = MONTH(CURDATE()) AND YEAR(StartTime) = YEAR(CURDATE())";
                    break;
                case "YEAR":
                    sqlSalary += "YEAR(StartTime) = YEAR(CURDATE())";
                    sqlCustomers += "YEAR(StartTime) = YEAR(CURDATE())";
                    break;
            }

            MySqlCommand cmdSalary = new MySqlCommand(sqlSalary, connection);
            MySqlDataReader readerSalary = cmdSalary.ExecuteReader();

            if (readerSalary.Read())
            {
                displayRevenue.text = "Salary: " + readerSalary["Salary"].ToString() + "元";
            }

            readerSalary.Close();

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
        int staffId = int.Parse(staffIdInput.text);

        if (!string.IsNullOrEmpty(startDay.text) && !string.IsNullOrEmpty(endDay.text) && !string.IsNullOrEmpty(monthInput.text))
        {
            int startDayValue, endDayValue, monthValue;

            if (int.TryParse(startDay.text, out startDayValue) && int.TryParse(endDay.text, out endDayValue) && int.TryParse(monthInput.text, out monthValue))
            {
                if (startDayValue >= 1 && startDayValue <= 31 && endDayValue >= 1 && endDayValue <= 31 && monthValue >= 1 && monthValue <= 12)
                {
                    MySqlConnection connection = Mysql.MysqlConnection();
                    connection.Open();

                    // Calculate the salary based on the price of each receipt
                    string sqlSalary = $"SELECT Price,Category,TIMESTAMPDIFF(MINUTE, StartTime, EndTime) as WorkingMinutes FROM Receipt WHERE StaffID = {staffId} AND DAY(StartTime) >= {startDayValue} AND DAY(StartTime) <= {endDayValue} AND MONTH(StartTime) = {monthValue} AND YEAR(StartTime) = YEAR(CURDATE())";
                    MySqlCommand cmdSalary = new MySqlCommand(sqlSalary, connection);
                    MySqlDataReader readerSalary = cmdSalary.ExecuteReader();
                    int totalSalary = 0;
                    while (readerSalary.Read())
                    {
                        string category = readerSalary.GetString("Category");
                        int workingMinutes = Convert.ToInt32(readerSalary["WorkingMinutes"]);
                        int price = int.Parse(readerSalary["Price"].ToString());
                        // Check the price and add the corresponding salary
                        if (category == "油壓")
                        {
                            if (workingMinutes == 60)
                            {
                                totalSalary += 400;
                            }
                            else if (workingMinutes == 120)
                            {
                                totalSalary += 800;
                            }
                        }
                        else if (category == "指壓")
                        {
                            if (workingMinutes == 60)
                            {
                                totalSalary += 400;
                            }
                            else if (workingMinutes == 120)
                            {
                                totalSalary += 800;
                            }
                        }
                        /*if (price > priceMax)
                            totalSalary += 800;
                        else if (price <= priceMax)
                            totalSalary += 400;*/
                    }
                    readerSalary.Close();

                    displayRevenue.text = "工資: " + totalSalary.ToString() + "元";

                    string sqlCustomers = $"SELECT COUNT(*) as Customers FROM Receipt WHERE StaffID = {staffId} AND DAY(StartTime) >= {startDayValue} AND DAY(StartTime) <= {endDayValue} AND MONTH(StartTime) = {monthValue} AND YEAR(StartTime) = YEAR(CURDATE())";
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

