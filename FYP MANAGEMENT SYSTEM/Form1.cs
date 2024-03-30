using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Windows.Forms;
using Guna.UI.WinForms;
using Microsoft.VisualBasic;

namespace FYP_MANAGEMENT_SYSTEM
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ReadGender()
        {
            var con1 = Configuration.getInstance().getConnection();

            string query = "Select Id FROM Lookup WHERE Category = 'GENDER'";
            SqlCommand command = new SqlCommand(query, con1);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int genderCode = reader.GetInt32(0);
                comboBox1.Items.Add(genderCode);
                comboBox2.Items.Add(genderCode);
            }

            reader.Close();
        }

        private void ReadDesignation()

        {
            var con1 = Configuration.getInstance().getConnection();

            string query = "Select Id FROM Lookup WHERE Category = 'DESIGNATION'";
            SqlCommand command = new SqlCommand(query, con1);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int designationCode = reader.GetInt32(0);
                comboBox3.Items.Add(designationCode);
            }

            reader.Close();
        }

        private void ReadStatus()
        {
            var con1 = Configuration.getInstance().getConnection();

            string query = "Select * FROM Lookup WHERE Category = 'Status'";
            SqlCommand command = new SqlCommand(query, con1);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int statusCode = reader.GetInt32(0);
                comboBox6.Items.Add(statusCode);
            }

            reader.Close();
        }

        
        private void ReadStudentId()
        {
            var con1 = Configuration.getInstance().getConnection();

            string query = "Select Id FROM Student ";
            SqlCommand command = new SqlCommand(query, con1);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int studentCode = reader.GetInt32(0);
                comboBox5.Items.Add(studentCode);
            }

            reader.Close();

        }

        private void ReadGroup()
        {
            try
            {
                var con1 = Configuration.getInstance().getConnection();

                string query = "SELECT ID FROM [Group]";
                SqlCommand command = new SqlCommand(query, con1);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int groupID = reader.GetInt32(0);
                    comboBox4.Items.Add(groupID);
                    comboBox7.Items.Add(groupID);
                }

                reader.Close();
                con1.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReadEvaluationId()
        {
            var con1 = Configuration.getInstance().getConnection();
            try
            {
                // Open the connection before executing the command
                if (con1.State == ConnectionState.Closed)
                {
                    con1.Open();
                }

                string query = "Select Id FROM Evaluation";
                SqlCommand command = new SqlCommand(query, con1);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int evaluationid = reader.GetInt32(0);
                    comboBox8.Items.Add(evaluationid);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                // Handle exceptions related to database connection
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                // Close the connection in the finally block to ensure it is closed even if an exception occurs
                if (con1.State == ConnectionState.Open)
                {
                    con1.Close();
                }
            }

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        // Function to show students list
        private void button5_Click(object sender, EventArgs e)
        {
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select * from Person JOIN Student ON Person.Id = Student.Id ", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //Delete Function of Advisor
        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                var con = Configuration.getInstance().getConnection();

                // Check if there is at least one row selected in the DataGridView
                if (dataGridView2.SelectedRows.Count > 0)
                {
                    // Assuming the ID is in a column named "ID" (adjust the column name accordingly)
                    int selectedId = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells["ID"].Value);

                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // Delete from Person table
                            using (SqlCommand cmdPerson = new SqlCommand("DELETE FROM Person WHERE ID = @ID", con, transaction))
                            {
                                cmdPerson.Parameters.AddWithValue("@ID", selectedId);
                                int rowsAffectedPerson = cmdPerson.ExecuteNonQuery();

                                // Delete from Advisor table
                                using (SqlCommand cmdAdvisor = new SqlCommand("DELETE FROM Advisor WHERE ID = @ID", con, transaction))
                                {
                                    cmdAdvisor.Parameters.AddWithValue("@ID", selectedId);
                                    int rowsAffectedAdvisor = cmdAdvisor.ExecuteNonQuery();

                                    // Check the number of affected rows and perform error handling or logging if needed
                                    if (rowsAffectedPerson > 0 || rowsAffectedAdvisor > 0)
                                    {
                                        // Commit the transaction if both deletes were successful
                                        transaction.Commit();
                                        MessageBox.Show("Record deleted successfully.");
                                    }
                                    else
                                    {
                                        // Rollback the transaction if no rows were affected in either table
                                        transaction.Rollback();
                                        MessageBox.Show("Failed to delete record. Record not found or an error occurred.");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Handle exceptions and log the error
                            MessageBox.Show($"Error: {ex.Message}");
                            transaction.Rollback();
                        }
                    }
                }
                else
                {
                    // Inform the user that no row is selected
                    MessageBox.Show("Please select a row in the DataGridView to delete.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions related to database connection
                MessageBox.Show($"Error connecting to the database: {ex.Message}");
            }


        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void monthCalendar5_DateChanged(object sender, DateRangeEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        // Student tab Insert Function
        private void button1_Click(object sender, EventArgs e)
        {
            // Getting the selected date from the month calendar component
            DateTime selectedDate = monthCalendar1.SelectionStart;

            // Converting the selected date to a string
            string dateString = selectedDate.ToString("dd-MM-yyyy");

            var con = Configuration.getInstance().getConnection();
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text)
                || string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text) ||
                string.IsNullOrEmpty(textBox5.Text) ||
                string.IsNullOrEmpty(comboBox1.Text))

            {
                MessageBox.Show("Your request cannot be processed. Please fill the entire form correctly...");

            }


            else
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("INSERT INTO Person (FirstName, LastName, Contact, Email, DateOfBirth, Gender) " +
                    "OUTPUT INSERTED.ID " +
                    "VALUES (@FirstName, @LastName, @Contact, @Email, @DOB, @Gender)", con);
                cmd.Parameters.AddWithValue("@FirstName", textBox2.Text);
                cmd.Parameters.AddWithValue("@LastName", textBox3.Text);
                cmd.Parameters.AddWithValue("@Contact", int.Parse(textBox5.Text));
                cmd.Parameters.AddWithValue("@Email", textBox4.Text);
                cmd.Parameters.AddWithValue("@DOB", dateString);
                cmd.Parameters.AddWithValue("@Gender", int.Parse(comboBox1.Text));

                int insertedPersonID = (int)cmd.ExecuteScalar();

                cmd = new SqlCommand("INSERT INTO Student (RegistrationNo, ID) " +
                    "VALUES (@RegistrationNo, @ID)", con);
                cmd.Parameters.AddWithValue("@RegistrationNo",textBox1.Text);
                cmd.Parameters.AddWithValue("@ID", insertedPersonID);

                try
                {
                    cmd.ExecuteNonQuery();
                    con.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    con.Close();

                }
                MessageBox.Show("Successfully saved");


            }
        }

        // Update Function In Student Tab
        private void button2_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = monthCalendar1.SelectionStart;
            string dateString = selectedDate.ToString("dd-MM-yyyy");

            var con = Configuration.getInstance().getConnection();

            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text)
                || string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text) ||
                string.IsNullOrEmpty(textBox5.Text) || string.IsNullOrEmpty(comboBox1.Text))
            {
                MessageBox.Show("Your request cannot be processed. Please fill the entire form correctly...");
            }
            else
            {
                int insertedPersonID = 0;
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    // Update the Person table
                    SqlCommand cmdPerson = new SqlCommand("UPDATE Person SET FirstName = @FirstName, LastName = @LastName, Contact = @Contact, Email = @Email, DateOfBirth = @DateOfBirth, Gender = @Gender WHERE FirstName = @FirstName", con);
                    cmdPerson.Parameters.AddWithValue("@FirstName", textBox2.Text);
                    cmdPerson.Parameters.AddWithValue("@LastName", textBox3.Text);
                    cmdPerson.Parameters.AddWithValue("@Contact", string.IsNullOrEmpty(textBox5.Text) ? (object)DBNull.Value : int.Parse(textBox5.Text));
                    cmdPerson.Parameters.AddWithValue("@Email", textBox4.Text);
                    cmdPerson.Parameters.AddWithValue("@DateOfBirth", dateString);
                    cmdPerson.Parameters.AddWithValue("@Gender", string.IsNullOrEmpty(comboBox1.Text) ? (object)DBNull.Value : int.Parse(comboBox1.Text));

                    cmdPerson.ExecuteNonQuery();

                    // Update the Student table
                    SqlCommand cmdStudent = new SqlCommand("UPDATE Student SET RegistrationNo = @RegistrationNo WHERE ID = @ID", con);
                    cmdStudent.Parameters.AddWithValue("@RegistrationNo", textBox1.Text);
                    cmdStudent.Parameters.AddWithValue("@ID", insertedPersonID);

                    cmdStudent.ExecuteNonQuery();

                    MessageBox.Show("Successfully Updated");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    con.Close();
                }
            }

        }

        // Function to search student in Students Tab
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Please fill all the fields");
            }
            else
            {
                var con = Configuration.getInstance().getConnection();
                try
                {
                    con.Open(); // Open the connection

                    SqlCommand cmd = new SqlCommand("Select * from Person JOIN Student ON Person.Id = Student.Id  where RegistrationNo ='" + textBox1.Text + "'", con);

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    // Check if there are any rows in the DataTable
                    if (dt.Rows.Count > 0)
                    {
                        MessageBox.Show("Student record is present");
                        dataGridView1.DataSource = dt;
                    }
                    else
                    {
                        MessageBox.Show("No student record found for the specified RegistrationNo.");
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions related to database connection
                    MessageBox.Show($"Error: {ex.Message}");
                }
                finally
                {
                    con.Close(); // Close the connection in the finally block to ensure it's closed even if an exception occurs
                }

            }
        }

       

        private void Form1_Load(object sender, EventArgs e)
        {
            ReadDesignation();
            ReadGender();
            ReadStatus();
            ReadStudentId();
            ReadGroup();
            ReadEvaluationId();
        }

        // Advisor Insert Function
        private void button6_Click(object sender, EventArgs e)
        {
            // Getting the selected date from the month calendar component
            DateTime selectedDate = monthCalendar2.SelectionStart;

            // Converting the selected date to a string in the format "yyyy-MM-dd"
            string dateString = selectedDate.ToString("yyyy-MM-dd");

            var con = Configuration.getInstance().getConnection();
            try
            {
                if (string.IsNullOrEmpty(comboBox2.Text) || string.IsNullOrEmpty(textBox6.Text) ||
                    string.IsNullOrEmpty(textBox7.Text) || string.IsNullOrEmpty(textBox8.Text) ||
                    string.IsNullOrEmpty(textBox9.Text) || string.IsNullOrEmpty(textBox10.Text) ||
                    string.IsNullOrEmpty(comboBox3.Text))
                {
                    MessageBox.Show("Please fill all fields");
                }
                else
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand("INSERT INTO Person (FirstName, LastName, Contact, Email, DateOfBirth, Gender) " +
                        "OUTPUT INSERTED.ID " +
                        "VALUES (@FirstName, @LastName, @Contact, @Email, @DOB, @Gender)", con);
                    cmd.Parameters.AddWithValue("@FirstName", textBox6.Text);
                    cmd.Parameters.AddWithValue("@LastName", textBox7.Text);
                    cmd.Parameters.AddWithValue("@Contact", int.Parse(textBox9.Text));
                    cmd.Parameters.AddWithValue("@Email", textBox8.Text);
                    cmd.Parameters.AddWithValue("@DOB", dateString);
                    cmd.Parameters.AddWithValue("@Gender", int.Parse(comboBox2.Text));

                    int insertedPersonID = (int)cmd.ExecuteScalar();

                    cmd = new SqlCommand("INSERT INTO Advisor (Designation,Salary,ID) " +
                        "VALUES (@Designation,@Salary, @ID)", con);
                    cmd.Parameters.AddWithValue("@Designation", int.Parse(comboBox3.Text));
                    cmd.Parameters.AddWithValue("@Salary", textBox10.Text);
                    cmd.Parameters.AddWithValue("@ID", insertedPersonID);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Successfully saved");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }

        }

        // Advisor Update Function
        private void button7_Click(object sender, EventArgs e)
        {
            // Getting the selected date from the month calendar component
            DateTime selectedDate = monthCalendar2.SelectionStart;

            // Converting the selected date to a string
            string dateString = selectedDate.ToString("dd-MM-yyyy");


            var con = Configuration.getInstance().getConnection();
            if (string.IsNullOrEmpty(comboBox2.Text) || string.IsNullOrEmpty(textBox6.Text) ||
                string.IsNullOrEmpty(textBox7.Text) || string.IsNullOrEmpty(textBox8.Text) ||
                string.IsNullOrEmpty(textBox9.Text) || string.IsNullOrEmpty(textBox10.Text) ||
                 string.IsNullOrEmpty(comboBox3.Text))
            {
                MessageBox.Show("Please fill all fields");
            }


            else
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("UPDATE Person SET Firstname = @FirstName, lastname = @LastName, contact = @Contact, dateofbirth = @DateOfBirth, gender = @Gender WHERE FirstName = @FirstName", con);
                cmd.Parameters.AddWithValue("@FirstName", textBox6.Text);
                cmd.Parameters.AddWithValue("@LastName", textBox7.Text);
                cmd.Parameters.AddWithValue("@Contact", int.Parse(textBox9.Text));
                cmd.Parameters.AddWithValue("@Email", textBox8.Text);
                cmd.Parameters.AddWithValue("@DOB", dateString);
                cmd.Parameters.AddWithValue("@Gender", int.Parse(comboBox2.Text));

                int insertedPersonID = (int)cmd.ExecuteScalar();

                cmd = new SqlCommand("UPDATE Advisor SET Designation = @Designation , Salary = @Salary WHERE Designation = @Designation", con);
                cmd.Parameters.AddWithValue("@Designation", int.Parse(comboBox3.Text));
                cmd.Parameters.AddWithValue("@Salary", textBox10.Text);
                cmd.Parameters.AddWithValue("@ID", insertedPersonID);

                MessageBox.Show("Successfully Updated");
                try
                {
                    cmd.ExecuteNonQuery();


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        // Advisor Find Function
        private void button8_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox6.Text))
            {
                MessageBox.Show("Please enter first name to find person");
            }
            else
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("Select * from Person JOIN Advisor ON Person.Id = Advisor.Id  where FirstName ='" + textBox6.Text + "'", con);
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                dataGridView2.DataSource = dt;
            }
        }

        // Advisor Show List Function
        private void button9_Click(object sender, EventArgs e)
        {
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select * from Person JOIN Advisor ON Person.Id = Advisor.Id ", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView2.DataSource= dt;
        }

        // Student Delete Function
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                var con = Configuration.getInstance().getConnection();

                // Check if there is at least one row selected in the DataGridView
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Assuming the ID is in a column named "ID" (adjust the column name accordingly)
                    int selectedId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);

                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                    }

                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // Delete from Student table
                            using (SqlCommand cmdStudent = new SqlCommand("DELETE FROM Student WHERE ID = @ID", con, transaction))
                            {
                                cmdStudent.Parameters.AddWithValue("@ID", selectedId);
                                int rowsAffectedStudent = cmdStudent.ExecuteNonQuery();

                                // Delete from Person table
                                using (SqlCommand cmdPerson = new SqlCommand("DELETE FROM Person WHERE ID = @ID", con, transaction))
                                {
                                    cmdPerson.Parameters.AddWithValue("@ID", selectedId);
                                    int rowsAffectedPerson = cmdPerson.ExecuteNonQuery();

                                    // Check the number of affected rows and perform error handling or logging if needed
                                    if (rowsAffectedPerson > 0 || rowsAffectedStudent > 0)
                                    {
                                        // Commit the transaction if both deletes were successful
                                        transaction.Commit();
                                        MessageBox.Show("Record deleted successfully.");
                                    }
                                    else
                                    {
                                        // Rollback the transaction if no rows were affected in either table
                                        transaction.Rollback();
                                        MessageBox.Show("Failed to delete record. Record not found or an error occurred.");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Handle exceptions and log the error
                            MessageBox.Show($"Error: {ex.Message} \nStackTrace: {ex.StackTrace}");
                            transaction.Rollback();
                        }
                        finally
                        {
                            con.Close();
                        }
                    }
                }
                else
                {
                    // Inform the user that no row is selected
                    MessageBox.Show("Please select a row in the DataGridView to delete.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions related to database connection
                MessageBox.Show($"Error connecting to the database: {ex.Message}");
            }


        }

        // Projects Insertion Function
        private void button11_Click(object sender, EventArgs e)
        {
            var con = Configuration.getInstance().getConnection();
            if (string.IsNullOrEmpty(textBox12.Text) || string.IsNullOrEmpty(textBox13.Text))
            {
                MessageBox.Show("Please fill all fields");
            }


            else
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("INSERT INTO Project (Title, Description) " +
                    "OUTPUT INSERTED.ID " +
                    "VALUES (@Title, @Description)", con);
                cmd.Parameters.AddWithValue("@Title", textBox12.Text);
                cmd.Parameters.AddWithValue("@Description", textBox13.Text);

                try
                {
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    con.Close();
                }

                MessageBox.Show("Successfully saved");
            }
        }

        // Projects Update Function
        private void button12_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox12.Text) || string.IsNullOrEmpty(textBox13.Text))
            {
                MessageBox.Show("Please fill all fields");
            }
            else
            {

                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("UPDATE Project SET Title = @Title, Description = @Description  WHERE Title = @Title", con);
                cmd.Parameters.AddWithValue("@Title", textBox12.Text);
                cmd.Parameters.AddWithValue("@Description", textBox13.Text);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Successfully Updated");
                try
                {
                    cmd.ExecuteNonQuery();


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);


                }

            }
        }

        // Projects Search Function
        private void button13_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox12.Text))
            {
                MessageBox.Show("Please enter title to find project");
            }
            else
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("Select * from Project where Title ='" + textBox12.Text + "'", con);
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                MessageBox.Show("Project with this title exists in the database");
               dataGridView3.DataSource = dt;
            }
        }

        // Projects Show List Function
        private void button14_Click(object sender, EventArgs e)
        {
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select * from Project ", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView3.DataSource = dt;
        }

        // Projects delete Function
        private void button15_Click(object sender, EventArgs e)
        {
            try
            {
                var con = Configuration.getInstance().getConnection();

                // Check if there is at least one row selected in the DataGridView
                if (dataGridView3.SelectedRows.Count > 0)
                {
                    // Assuming the ID is in a column named "ID" (adjust the column name accordingly)
                    int selectedId = Convert.ToInt32(dataGridView3.SelectedRows[0].Cells["ID"].Value);

                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // Delete from Student table
                            using (SqlCommand cmdProject = new SqlCommand("DELETE FROM Project WHERE ID = @ID", con, transaction))
                            {
                                cmdProject.Parameters.AddWithValue("@ID", selectedId);
                                int rowsAffectedProject = cmdProject.ExecuteNonQuery();

                                    // Check the number of affected rows and perform error handling or logging if needed
                                    if (rowsAffectedProject > 0 )
                                    {
                                        // Commit the transaction if both deletes were successful
                                        transaction.Commit();
                                        MessageBox.Show("Record deleted successfully.");
                                    }
                                    else
                                    {
                                        // Rollback the transaction if no rows were affected in either table
                                        transaction.Rollback();
                                        MessageBox.Show("Failed to delete record. Record not found or an error occurred.");
                                    }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Handle exceptions and log the error
                            MessageBox.Show($"Error: {ex.Message}");
                            transaction.Rollback();
                        }
                    }
                }
                else
                {
                    // Inform the user that no row is selected
                    MessageBox.Show("Please select a row in the DataGridView to delete.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions related to database connection
                MessageBox.Show($"Error connecting to the database: {ex.Message}");
            }
        }

        private bool GroupIdExists(int groupId, SqlConnection con)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM [Group] WHERE Id = @GroupId", con))
                {
                    cmd.Parameters.AddWithValue("@GroupId", groupId);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            finally
            {
                con.Close();
            }
        }

        //(Student) Group Insertion Function
        private void button16_Click(object sender, EventArgs e)
        {
            var con = Configuration.getInstance().getConnection();
            try
            {
                DateTime selectedDate = monthCalendar3.SelectionStart;

                if (string.IsNullOrEmpty(comboBox5.Text) || string.IsNullOrEmpty(comboBox6.Text))
                {
                    MessageBox.Show("Please fill all fields");
                }
                else
                {
                    int groupId = int.Parse(comboBox4.Text);

                    if (GroupIdExists(groupId, con))
                    {
                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }

                        using (SqlCommand cmd = new SqlCommand("INSERT INTO GroupStudent (GroupId, StudentId, Status, AssignmentDate) " +
                                                                "VALUES (@GroupId, @StudentId, @Status, @AssignmentDate)", con))
                        {
                            cmd.Parameters.AddWithValue("@GroupId", groupId);
                            cmd.Parameters.AddWithValue("@StudentId", int.Parse(comboBox5.Text));
                            cmd.Parameters.AddWithValue("@Status", int.Parse(comboBox6.Text));
                            cmd.Parameters.AddWithValue("@AssignmentDate", selectedDate);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Successfully saved");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid GroupId. Please select a valid GroupId.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }
        }


        //(Student) Group Function
        private void button17_Click(object sender, EventArgs e)
        {

        }

        //(Student) Group Show List Function
        private void button19_Click(object sender, EventArgs e)
        {
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select * from GroupStudent", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView4.DataSource = dt;
        }

        //(Student) Group Update Function
        private void button20_Click(object sender, EventArgs e)
        {
            var con = Configuration.getInstance().getConnection();

            try
            {
                DateTime selectedDate = monthCalendar3.SelectionStart;

                if (string.IsNullOrEmpty(comboBox5.Text) || string.IsNullOrEmpty(comboBox6.Text))
                {
                    MessageBox.Show("Please fill all fields");
                }
                else
                {
                    int groupId = int.Parse(comboBox4.Text);

                    if (GroupIdExists(groupId, con))
                    {
                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }

                        using (SqlCommand cmd = new SqlCommand("UPDATE GroupStudent " +
                                                                "SET StudentId = @StudentId, Status = @Status, AssignmentDate = @AssignmentDate " +
                                                                "WHERE GroupId = @GroupId", con))
                        {
                            cmd.Parameters.AddWithValue("@GroupId", groupId);
                            cmd.Parameters.AddWithValue("@StudentId", int.Parse(comboBox5.Text));
                            cmd.Parameters.AddWithValue("@Status", int.Parse(comboBox6.Text));
                            cmd.Parameters.AddWithValue("@AssignmentDate", selectedDate);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Successfully updated");
                            }
                            else
                            {
                                MessageBox.Show("Update failed. GroupId not found or no changes made.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid GroupId. Please select a valid GroupId.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }
        }

        // (Evaluation) Group Insertion Function
        private void button21_Click(object sender, EventArgs e)
        {
            // Get the selected date from the month calendar component
            DateTime selectedDate = monthCalendar4.SelectionStart;

            // Convert the selected date to a string
            string dateString = selectedDate.ToString("dd-MM-yyyy");


            var con = Configuration.getInstance().getConnection();
            if (string.IsNullOrEmpty(comboBox7.Text) || string.IsNullOrEmpty(comboBox8.Text) ||
                string.IsNullOrEmpty(textBox15.Text))
            {
                MessageBox.Show("Please fill all fields");
            }


            else
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("INSERT INTO GroupEvaluation (GroupId, EvaluationId, ObtainedMarks, EvaluationDate) " +

                    "VALUES (@GroupId, @EvaluationId, @ObtainedMarks, @EvaluationDate)", con);
                cmd.Parameters.AddWithValue("@GroupId", int.Parse(comboBox7.Text));
                cmd.Parameters.AddWithValue("@EvaluationId", int.Parse(comboBox8.Text));
                cmd.Parameters.AddWithValue("@ObtainedMarks", int.Parse(textBox15.Text));
                cmd.Parameters.AddWithValue("@EvaluationDate", dateString);


                try
                {
                    cmd.ExecuteNonQuery();
                    con.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    con.Close();

                }
                MessageBox.Show("Successfully saved");


            }
        }

        // (Evaluation) Group Update Function
        private void button22_Click(object sender, EventArgs e)
        {
            // Get the selected date from the month calendar component
            DateTime selectedDate = monthCalendar4.SelectionStart;

            // Convert the selected date to a string
            string dateString = selectedDate.ToString("dd-MM-yyyy");

            var con = Configuration.getInstance().getConnection();

            if (string.IsNullOrEmpty(comboBox7.Text) || string.IsNullOrEmpty(comboBox8.Text) || string.IsNullOrEmpty(textBox15.Text))
            {
                MessageBox.Show("Please fill all fields");
            }
            else
            {
                SqlCommand cmd = new SqlCommand("UPDATE Groupevaluation SET GroupId = @GroupId, EvaluationId = @EvaluationId, ObtainedMarks = @ObtainedMarks, EvaluationDate = @EvaluationDate WHERE EvaluationID = @EvaluationId", con);

                cmd.Parameters.AddWithValue("@GroupId", int.Parse(comboBox7.Text));
                cmd.Parameters.AddWithValue("@EvaluationId", int.Parse(comboBox8.Text));
                cmd.Parameters.AddWithValue("@ObtainedMarks", int.Parse(textBox15.Text));
                cmd.Parameters.AddWithValue("@EvaluationDate", dateString);

                try
                {
                    con.Open(); // Open the connection before executing the command
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Record updated successfully");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    con.Close(); // Close the connection in the finally block to ensure it's closed even if an exception occurs
                }
            }

        }

        private void button23_Click(object sender, EventArgs e)
        {
        }

        // (Evaluation) Group Show List Function
        private void button24_Click(object sender, EventArgs e)
        {
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select * from GroupEvaluation ", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView5.DataSource = dt;
        }

        //(Student) Group Delete Function
        private void button17_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Get the selected database connection
                var con = Configuration.getInstance().getConnection();

                // Check if there is at least one row selected in the DataGridView
                if (dataGridView4.SelectedRows.Count > 0)
                {
                    int selectedGroupId = 0;

                    // Check if the column exists before accessing its value
                    if (dataGridView4.Columns.Contains("GroupId"))
                    {
                        selectedGroupId = Convert.ToInt32(dataGridView4.SelectedRows[0].Cells["GroupId"].Value);

                        // Open the connection here to ensure it is open before executing the command
                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }

                        using (var transaction = con.BeginTransaction())
                        {
                            try
                            {
                                // Delete from GroupStudent table
                                using (SqlCommand cmdGroupStudent = new SqlCommand("DELETE FROM GroupStudent WHERE GroupId = @GroupId", con, transaction))
                                {
                                    cmdGroupStudent.Parameters.AddWithValue("@GroupId", selectedGroupId);
                                    int rowsAffectedGroupStudent = cmdGroupStudent.ExecuteNonQuery();

                                    // Check the number of affected rows and perform error handling or logging if needed
                                    if (rowsAffectedGroupStudent > 0)
                                    {
                                        // Commit the transaction if the delete was successful
                                        transaction.Commit();
                                        MessageBox.Show("Record deleted successfully.");
                                    }
                                    else
                                    {
                                        // Rollback the transaction if no rows were affected
                                        transaction.Rollback();
                                        MessageBox.Show("Failed to delete record. Record not found or an error occurred.");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // Handle exceptions and log the error
                                MessageBox.Show($"Error: {ex.Message}");
                                transaction.Rollback();
                            }
                            finally
                            {
                                // Close the connection in the finally block to ensure it is closed even if an exception occurs
                                con.Close();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Column 'GroupId' not found in the DataGridView.");
                    }
                }
                else
                {
                    // Inform the user that no row is selected
                    MessageBox.Show("Please select a row in the DataGridView to delete.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions related to database connection
                MessageBox.Show($"Error connecting to the database: {ex.Message}");
            }

        }

       // Clear Button Functions
        private void button18_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            comboBox1.Text = "";
        }

        private void button23_Click_1(object sender, EventArgs e)
        {
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            textBox9.Text = "";
            textBox10.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
        }

        private void button26_Click(object sender, EventArgs e)
        {
            textBox11.Text = "";
            textBox12.Text = "";
            textBox13.Text = "";
        }

        private void button27_Click(object sender, EventArgs e)
        {
            textBox14.Text = "";
            comboBox4.Text = "";
            comboBox5.Text = "";
            comboBox6.Text = "";
        }

        private void button28_Click(object sender, EventArgs e)
        {
            textBox15.Text = "";
            comboBox7.Text = "";
            comboBox8.Text = "";
        }
    }
}
