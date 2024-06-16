using UnityEngine;
using System;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
    public GameObject timeWarningTab;

    public InputField Price_you1;
    public InputField Price_you2;
    public InputField Price_zhi1;
    public InputField Price_zhi2;

    public Toggle raiseToggle;

    public InputField saleInput;
    public InputField raiseInput;

    public InputField F_you1;
    public InputField F_you2;
    public InputField F_zhi1;
    public InputField F_zhi2;

    public Toggle funnowToggle;
    public int funnow;
    public int hour;
    public int minute;
    public InputField dayInput;
    public InputField monthInput;
    public Toggle sale;
    public Button quickTimeBtn;
    public Dropdown[] dropdowns;
    public int Price;
    public bool isQuickTime = false;
    public Text warningText;
    public GameObject warningTab;
    public Text successText;
    public Text successText2;
    public GameObject successTab;
    public Button loadsceneBtn;

    public Text id1;
    public Text id2;
    public Text id3;

    public Text time1;
    public Text time2;
    public Text time3;

    public Text id4;
    public Text id5;
    public Text id6;

    public Text time4;
    public Text time5;
    public Text time6;

    private int saleLimit;
    bool isModifyTime = false;
    private int FunNowTax = 0;
    public static MainController Instance { get; private set; } // Singleton instance

    public string Category { get; set; }    // Massage type category
    public int WorkTime { get; set; }    // Work duration
    public int EmployeeID { get; set; }     // Employee ID
    public DateTime StartTime { get; set; } // Start time
    public DateTime EndTime { get; set; }   // End time


    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            EmployeeID = 0;
        }
        else
        {
            Destroy(gameObject); // If an instance already exists, destroy this one
        }
    }
    private void Start()
    {
        GetSetting();
        int zhi1h= int.Parse(Price_zhi1.text);
        saleLimit = 500;
        raiseToggle.isOn = false;
        saleInput.onValueChanged.AddListener(delegate { CheckInput(); });
        raiseInput.onValueChanged.AddListener(delegate { CheckInput(); });
        funnowToggle.isOn = false;
        loadsceneBtn.onClick.AddListener(() => LoadSceneMain());
        InvokeRepeating(nameof(UpdateWorkingEmployees), 0, 1);  // Call UpdateWorkingEmployees every 1 second
        warningText.color = Color.red;
        successText.color = Color.green;
        sale.isOn = false;
        dayInput.text = DateTime.Now.Day.ToString();
        Instance.monthInput.text = DateTime.Now.Month.ToString();

        
        
    }
    void CheckInput()
    {
        if(saleInput.text == "")
        {
            return;
        }
        if (raiseInput.text == "") return;
        UpDateSettingBtn();
    }
    
    private bool CheckInvoiceTable()
    {
        bool isFull = false;

        // Connect to the database
        var connection = Mysql.MysqlConnection();
        connection.Open();

        // Prepare SQL command to count the records in the Invoice table
        string sql = "SELECT COUNT(*) FROM Invoice;";

        using (MySqlCommand cmd = new MySqlCommand(sql, connection))
        {
            // Execute the command and get the count of records
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            // Check if the count is at least 15
            if (count >= 15)
            {
                isFull = true;
            }
        }

        connection.Dispose();
        connection.Close();

        return isFull;
    }

    public void LoadSceneMain()
    {
        SceneManager.LoadScene("SampleScene");
    }

    private void UpdateWorkingEmployees()
    {
        // Reset all displayed IDs and times
        id1.text = "";
        id2.text = "";
        id3.text = "";
        id4.text = "";
        id5.text = "";
        id6.text = "";
        time1.text = "";
        time2.text = "";
        time3.text = "";
        time4.text = "";
        time5.text = "";
        time6.text = "";
        var connection = Mysql.MysqlConnection();
        connection.Open();
        try
        {
           

            // Prepare SQL command to get working employees and their end times
            string sql = "SELECT StaffID, EndTime FROM Receipt WHERE EndTime > NOW() " +
                 "UNION " +
                 "SELECT StaffID, EndTime FROM Invoice WHERE EndTime > NOW() " +
                 "ORDER BY EndTime ASC LIMIT 6;";

            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                MySqlDataReader reader = cmd.ExecuteReader();

                // List of text fields to display IDs and times
                Text[] idFields = new Text[] { id1, id2, id3, id4, id5, id6 };
                Text[] timeFields = new Text[] { time1, time2, time3, time4, time5, time6 };

                int index = 0;  // The index of the currently processed employee

                // Read the result
                while (reader.Read())
                {
                    string id = reader.GetString(0);
                    DateTime endTime = reader.GetDateTime(1);

                    // Calculate remaining time (in hh:mm:ss format)
                    TimeSpan remainingTime = endTime - DateTime.Now;
                    string time = string.Format("{0:D2}:{1:D2}:{2:D2}", remainingTime.Hours, remainingTime.Minutes, remainingTime.Seconds);

                    // Display the ID and remaining time
                    idFields[index].text = id;
                    timeFields[index].text = time;

                    // Check if the remaining time is less than 15 minutes. If so, change the color to red.
                    if (remainingTime.TotalMinutes <= 15)
                    {
                        idFields[index].color = Color.red;
                        timeFields[index].color = Color.red;
                    }
                    else
                    {
                        idFields[index].color = Color.black; // Or any other color you prefer
                        timeFields[index].color = Color.black; // Or any other color you prefer
                    }

                    index++;
                }
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

    public void QuickTimeBtn()
    {
        isQuickTime = !isQuickTime;
        foreach (Dropdown dropdown in dropdowns)
        {
            dropdown.gameObject.SetActive(!dropdown.gameObject.activeSelf);
        }
    }
    bool CheckTime()
    {
        /*DateTime startOfDay = StartTime.Date;
        DateTime targetTimeStart = startOfDay.AddHours(0);  // This will represent 00:00 of the current date
        DateTime targetTimeEnd = startOfDay.AddHours(4);    // This will represent 04:00 of the current date

        if (StartTime >= targetTimeStart && StartTime <= targetTimeEnd)
        {
            return true;
        }
        return false;*/
        if (StartTime.Hour < 4)
        {
            return true;
        }
        return false;
    }
    
    public void ModiFyTimeAndSave()
    {
        StartTime = StartTime.Date.AddDays(-1).AddHours(23).AddMinutes(59);
        isModifyTime = true;
        SaveData();
    }
    public void Save()
    {
        isModifyTime = true;
        SaveData();
    }
    public void SaveData()
    {
        
        SetPrice();
        int saleNum = int.Parse(saleInput.text);
        int raiseNum = int.Parse(raiseInput.text);
        
        
        if (raiseNum > 10000 && raiseToggle.isOn||saleNum > saleLimit && sale.isOn || CheckInvoiceTable() || string.IsNullOrEmpty(Category) || WorkTime == 0 || EmployeeID == 0)
        {
            warningText.text = "";
           
           
            if(CheckInvoiceTable())
            {
                warningText.text += "暫存記錄表已滿\n";
            }
            if(string.IsNullOrEmpty(Category))
            {
                warningText.text += "未選類型\n";
            }
            if(WorkTime == 0)
            {
                warningText.text += "未選服務時間\n";
            }
            if (EmployeeID == 0)
            {
                warningText.text += "未選員工編號\n";
            }
            if(saleNum > saleLimit && sale.isOn)
            {
                saleInput.text = saleLimit.ToString();
                warningText.text += $"打折不可以超過{saleLimit}元\n";
            }
            if(raiseNum > 9999 && raiseToggle.isOn)
            {
                raiseInput.text = "9999";
                warningText.text += "額外增加不可以超過9999元\n";
            }
            
            warningTab.SetActive(true);
            return;
        }
        if (!isModifyTime)
        {
            if (isQuickTime)
            {
                StartTime = DateTime.Now;
                EndTime = DateTime.Now.AddHours(WorkTime);
            }
            else
            {
                int day = int.Parse(dayInput.text);
                int month = int.Parse(monthInput.text);
                StartTime = new DateTime(DateTime.Now.Year, month, day, hour, minute, 0);
                EndTime = StartTime.AddHours(WorkTime);
            }
            if (CheckTime())
            {
                string text = $"你的開始時間是{StartTime}\n這個時間已超過0點，要不要把開始時間改成 23:59 ?";
                timeWarningTab.SetActive(true);
                timeWarningTab.transform.GetChild(1).GetComponent<Text>().text = text;
                return;
            }
        }


        var connection = Mysql.MysqlConnection();
        connection.Open();
        SetPrice();
        SetSale();
        SetFunNow();
        SetRaise();

        string sql = "INSERT INTO Invoice (StaffID, category, StartTime, EndTime,Price) VALUES (@staffID, @Category, @startTime, @endTime,@Price)";

        using (MySqlCommand cmd = new MySqlCommand(sql, connection))
        {
            cmd.Parameters.AddWithValue("@staffID", EmployeeID);
            cmd.Parameters.AddWithValue("@Category", Category);
            cmd.Parameters.AddWithValue("@startTime", StartTime);
            cmd.Parameters.AddWithValue("@endTime", EndTime);
            cmd.Parameters.AddWithValue("@Price", Price);

            cmd.ExecuteNonQuery();
        }
        connection.Dispose();
        connection.Close();

        int saleNum2;
        int raiseNum2;
        if(sale.isOn) saleNum2 = saleNum;
        else saleNum2 = 0;
        if (raiseToggle.isOn) raiseNum2 = raiseNum;
        else raiseNum2 = 0;

        successText.text = "資料已儲存 !";
        successText2.text = $"FunNow折扣: -{FunNowTax}\n優惠折扣: -{saleNum2}\n額外增加: +{raiseNum2}\n總價:{Price}";
        successTab.SetActive(true);
    }
    void SetRaise()
    {
        if(raiseToggle.isOn)
        {
            int value;

            if (int.TryParse(raiseInput.text, out value))
            {
                Price += value;
            }
        }
    }
    void SetFunNow()
    {
        if (!funnowToggle.isOn) return;
        if(Category == "油壓")
        {
            if(WorkTime == 1)
            {
                int value;

                if (int.TryParse(F_you1.text, out value))
                {
                    Price -= value;
                    FunNowTax = value;
                }
            }
            else if(WorkTime == 2)
            {
                int value;

                if (int.TryParse(F_you2.text, out value))
                {
                    Price -= value;
                    FunNowTax = value;
                }
            }
        }
        else if (Category == "指壓")
        {
            if(WorkTime == 1)
            {
                int value;

                if (int.TryParse(F_zhi1.text, out value))
                {
                    Price -= value;
                    FunNowTax = value;
                }
            }
            else if(WorkTime == 2)
            {
                int value;

                if (int.TryParse(F_zhi2.text, out value))
                {
                    Price -= value;
                    FunNowTax = value;
                }
            }

        }
    }
    public void SetSale()
    {
        if(sale.isOn)
        {
            int value;

            if (int.TryParse(saleInput.text, out value))
            {              
                Price -= value;
            }
           
        }
    }
    public void SetPrice()
    {
        if(Category == "油壓" && WorkTime == 1)
        {
            int value;

            if (int.TryParse(Price_you1.text, out value))
            {
                Price = value;
            }
        }
        else if (Category == "油壓" && WorkTime == 2)
        {
            int value;

            if (int.TryParse(Price_you2.text, out value))
            {
                Price = value;

                int you1h = int.Parse(Price_you1.text);
            }
        }
        else if (Category == "指壓" && WorkTime == 1)
        {
            int value;

            if (int.TryParse(Price_zhi1.text, out value))
            {
                Price = value;
            }
        }
        else if (Category == "指壓" && WorkTime == 2)
        {
            int value;

            if (int.TryParse(Price_zhi2.text, out value))
            {
                Price = value;

                int you1h = int.Parse(Price_you1.text);
            }
        }

    }
    public void UpDateSettingBtn()
    {
        // Get the connection
        var connection = Mysql.MysqlConnection();
        connection.Open();

        try
        {
            int F_you1Val = int.Parse(F_you1.text);
            int F_you2Val = int.Parse(F_you2.text);
            int F_zhi1Val = int.Parse(F_zhi1.text);
            int F_zhi2Val = int.Parse(F_zhi2.text);
            int F_saleVal = int.Parse(saleInput.text);
            int F_raiseVal = int.Parse(raiseInput.text);
            int Price_you1Val = int.Parse(Price_you1.text);
            int Price_you2Val = int.Parse(Price_you2.text);
            int Price_zhi1Val = int.Parse(Price_zhi1.text);
            int Price_zhi2Val = int.Parse(Price_zhi2.text);

            // Create the SQL query to update data in the table
            string query = string.Format("UPDATE settings SET F_you1 = {0}, F_you2 = {1}, F_zhi1 = {2}, F_zhi2 = {3}, sale = {4}, raise = {5}, Price_you1 = {6}, Price_you2 = {7}, Price_zhi1 = {8}, Price_zhi2 = {9}",
                                     F_you1Val, F_you2Val, F_zhi1Val, F_zhi2Val, F_saleVal, F_raiseVal, Price_you1Val, Price_you2Val, Price_zhi1Val, Price_zhi2Val);

            // Create a Command object
            MySqlCommand cmd = new MySqlCommand(query, connection);

            // Execute the query
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occur here
            Debug.Log("Exception: " + ex.Message);
        }
        finally
        {
            // Ensure the connection is closed
            connection.Close();
            connection.Dispose();
        }
        GetSetting();
    }



    public void GetSetting()
    {
        // Get the connection
        var connection = Mysql.MysqlConnection();
        connection.Open();

        try
        {
            // Create the SQL query to retrieve data from the table
            string query = "SELECT * FROM settings";

            // Create a Command object
            MySqlCommand cmd = new MySqlCommand(query, connection);

            // Execute the query and receive the result
            MySqlDataReader dataReader = cmd.ExecuteReader();

            // Check if there are any records
            if (dataReader.Read())
            {
                // Set the values for the InputField objects
                F_you1.text = dataReader.GetInt32("F_you1").ToString();
                F_you2.text = dataReader.GetInt32("F_you2").ToString();
                F_zhi1.text = dataReader.GetInt32("F_zhi1").ToString();
                F_zhi2.text = dataReader.GetInt32("F_zhi2").ToString();
                saleInput.text = dataReader.GetInt32("sale").ToString();
                raiseInput.text = dataReader.GetInt32("raise").ToString();
                Price_you1.text = dataReader.GetInt32("Price_you1").ToString();
                Price_you2.text = dataReader.GetInt32("Price_you2").ToString();
                Price_zhi1.text = dataReader.GetInt32("Price_zhi1").ToString();
                Price_zhi2.text = dataReader.GetInt32("Price_zhi2").ToString();
            }

            dataReader.Close();
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occur here
            Debug.Log("Exception: " + ex.Message);
        }
        finally
        {
            // Ensure the connection is closed
            connection.Close();
            connection.Dispose();
        }
    }






}
