using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StudentCounselling
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Window
    {
        int _studentId = 0;
        public HomePage(int studentId)
        {
            InitializeComponent();
            CenterWindowOnScreen();
            btnPrev.Visibility = Visibility.Hidden;
            _studentId = studentId;
            tbDOB.SelectedDate = DateTime.Now.AddYears(-18);
            FetchStudentDetails();
        }

        private void FetchStudentDetails()
        {
            // Initialization.  
            string strConn = "Data Source=LAPTOP-6RBMDIS7\\SQLEXPRESS;Database=DBMS_Project;Integrated Security=SSPI;";
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCommand = new SqlCommand();
            object rowCount;
            try
            {
                // Settings.  
                sqlCommand.CommandText = $"SELECT * from student_details where SID = {_studentId}";
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandTimeout = 12 * 3600; //// Setting timeeout for longer queries to 12 hours.  

                // Open.  
                sqlConnection.Open();

                // Result.  
                SqlDataReader reader = sqlCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                        tbStudentName.Text = reader.GetString(3);
                        tbFatherName.Text = reader.GetString(4);
                        tbMotherName.Text = reader.GetString(5);
                        tbDOB.SelectedDate = reader.GetDateTime(6);
                        tbSchool.Text = reader.GetString(7);
                        tbPassYear.Text = reader.GetInt32(8).ToString();
                        tbAddress.Text = reader.GetString(9);
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                reader.Close();
                // Close.  
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                // Close.  
                sqlConnection.Close();
            }
        }

        private void OnNextClicked(object sender, RoutedEventArgs e)
        {
            if (tabStudentDetail.IsSelected)
            {
                if (!UpdateStudentDetail())
                    MessageBox.Show("Unexpected Error");
                else
                {
                    tabDocDetail.IsSelected = true;
                    databaseFileRead(_studentId, 2, System.Reflection.Assembly.GetExecutingAssembly().Location);
                    return;
                }
                if (tabDocDetail.IsSelected)
                {
                    if (!UpdateDocumentDetail())
                        MessageBox.Show("Unexpected Error");
                    else
                        tabMarkDetails.IsSelected = true;
                }
            }
        }

        bool databaseFilePut(string varFilePath, int docID, string docName)
        {
            try
            {
                byte[] file;
                using (var stream = new FileStream(varFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        file = reader.ReadBytes((int)stream.Length);
                    }
                }
                string strConn = "Data Source=LAPTOP-6RBMDIS7\\SQLEXPRESS;Database=DBMS_Project;Integrated Security=SSPI;";
                SqlConnection sqlConnection = new SqlConnection(strConn);
                sqlConnection.Open();

                using (var sqlWrite = new SqlCommand($"INSERT INTO document_details Values({_studentId},{docID}, @File, '{docName}')", sqlConnection))
                {
                    sqlWrite.Parameters.Add("@File", SqlDbType.VarBinary, file.Length).Value = file;
                    sqlWrite.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        void databaseFileRead(int studentId, int docId, string varPathToNewLocation)
        {
            string strConn = "Data Source=LAPTOP-6RBMDIS7\\SQLEXPRESS;Database=DBMS_Project;Integrated Security=SSPI;";
            SqlConnection sqlConnection = new SqlConnection(strConn);
            sqlConnection.Open();
            string docName = string.Empty;
            using (var sqlQuery = new SqlCommand($"SELECT document, document_name FROM document_details WHERE SID = @studentId AND DID = @docId", sqlConnection))
            {
                sqlQuery.Parameters.AddWithValue("@studentId", studentId);
                sqlQuery.Parameters.AddWithValue("@docId", docId);
                using (var sqlQueryResult = sqlQuery.ExecuteReader())
                    if (sqlQueryResult != null)
                    {
                        sqlQueryResult.Read();
                        var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                        sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                        docName = sqlQueryResult.GetString(1);
                        using (var fs = new FileStream(".\\"+ docName, FileMode.Create, FileAccess.Write))
                            fs.Write(blob, 0, blob.Length);
                    }
            }
            hyperlinkText.Text = docName;
            tbTenthMLClick.Tag = ".\\" + docName;
        }
        bool UpdateDocumentDetail()
        {
            return databaseFilePut(tbTenthMLClick.Tag.ToString(), 2, hyperlinkText.Text);
        }

        private bool UpdateStudentDetail()
        {
            // Initialization.  
            string strConn = "Data Source=LAPTOP-6RBMDIS7\\SQLEXPRESS;Database=DBMS_Project;Integrated Security=SSPI;";
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCommand = new SqlCommand();
            object rowCount;
            try
            {
                var dob = DateTime.Parse(tbDOB.Text).ToString("dd-MMM-yyyy");
                // Settings.  
                sqlCommand.CommandText = $"UPDATE student_details SET " +
                    $"student_name = '{tbStudentName.Text}', " +
                    $"fathers_name = '{tbFatherName.Text}', " +
                    $"mothers_name = '{tbMotherName.Text}'," +
                    $"DOB = '{dob}'," +
                    $"school_studied_in = '{tbSchool.Text}', " +
                    $"year_passed_out = '{tbPassYear.Text}', " +
                    $"address = '{tbAddress.Text}' " +
                    $"WHERE SID = {_studentId}";
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandTimeout = 12 * 3600; //// Setting timeeout for longer queries to 12 hours.  

                // Open.  
                sqlConnection.Open();

                // Result.  
                rowCount = sqlCommand.ExecuteNonQuery();
                // Close.  
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                // Close.  
                sqlConnection.Close();

                return false;
            }
            if ((int)rowCount == 0)
                return false;
            return true;
        }
        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void OnTenthMLUpload(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == true)
            {
                hyperlinkText.Text = openFileDialog1.SafeFileName;
                tbTenthMLClick.Tag = openFileDialog1.FileName;
            }
        }
        private void TenthMLClicked(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(tbTenthMLClick.Tag.ToString())
            {
                UseShellExecute = true
            };
            p.Start();
        }

        private void OnTwelthMLUpload(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == true)
            {
                hyperlinkTextTwelthML.Text = openFileDialog1.SafeFileName;
                tbTwelfthMarkList.Tag = openFileDialog1.FileName;
            }
        }

        private void TwelthMLClicked(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(tbTwelfthMarkList.Tag.ToString())
            {
                UseShellExecute = true
            };
            p.Start();
        }

        private void OnCommCertUpload(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == true)
            {
                hyperlinkTextCommCert.Text = openFileDialog1.SafeFileName;
                tbCommCertClick.Tag = openFileDialog1.FileName;
            }
        }

        private void CommCertClicked(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(tbCommCertClick.Tag.ToString())
            {
                UseShellExecute = true
            };
            p.Start();
        }
    }
}
