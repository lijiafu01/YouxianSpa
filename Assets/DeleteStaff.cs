using System;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;

public class DeleteStaff : MonoBehaviour
{
    public InputField idInput;
    public Button deleteDataBtn;

    private void Start()
    {
        deleteDataBtn.onClick.AddListener(DeleteDataBtn);
    }

    public void DeleteDataBtn()
    {
        var connection = Mysql.MysqlConnection();
        connection.Open();
        try
        {
            // Check for empty fields
            if (string.IsNullOrEmpty(idInput.text))
            {
                Debug.Log("StaffID field is empty. Please enter the StaffID to delete.");
                return;
            }

            // Check for correct ID
            if (!int.TryParse(idInput.text, out int id))
            {
                Debug.Log("StaffID must be an integer.");
                return;
            }

            string sql = "DELETE FROM Staff WHERE StaffID = @id";

            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@id", id);

                int affectedRows = cmd.ExecuteNonQuery();

                if (affectedRows > 0)
                {
                    Debug.Log("Staff with ID " + id + " deleted from the database.");
                }
                else
                {
                    Debug.Log("No Staff found with ID " + id + ".");
                }
            }
        }
        catch
        {
            Debug.Log("Failed to delete data.");
        }
        finally
        {
            connection.Dispose();
            connection.Close();
        }
    }
}
