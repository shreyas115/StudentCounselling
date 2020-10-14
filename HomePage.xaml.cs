using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
                    MessageBox.Show("Student details saved successfully!!!", "Data saved", MessageBoxButton.OK);
                    tabDocDetail.IsSelected = true;
                    databaseFileRead(_studentId, 2, System.Reflection.Assembly.GetExecutingAssembly().Location);
                    databaseFileRead(_studentId, 3, System.Reflection.Assembly.GetExecutingAssembly().Location);
                    databaseFileRead(_studentId, 4, System.Reflection.Assembly.GetExecutingAssembly().Location);
                    databaseFileRead(_studentId, 5, System.Reflection.Assembly.GetExecutingAssembly().Location);
                    databaseFileRead(_studentId, 6, System.Reflection.Assembly.GetExecutingAssembly().Location);
                    databaseFileRead(_studentId, 7, System.Reflection.Assembly.GetExecutingAssembly().Location);
                    databaseFileRead(_studentId, 8, System.Reflection.Assembly.GetExecutingAssembly().Location);
                    databaseFileRead(_studentId, 9, System.Reflection.Assembly.GetExecutingAssembly().Location);
                    return;
                }
            }
            else if (tabDocDetail.IsSelected)
            {
                if (!UpdateDocumentDetail())
                    MessageBox.Show("Unexpected Error");
                else
                {
                    MessageBox.Show("Document details saved successfully!!!", "Data saved", MessageBoxButton.OK);
                    tabMarkDetails.IsSelected = true;
                    FetchMarkDetails();
                }
            }
            else if (tabMarkDetails.IsSelected)
            {
                if (!UpdateMarkDetails())
                    MessageBox.Show("Unexpected Error");
                else
                {
                    MessageBox.Show("Mark details saved successfully!!!", "Data saved", MessageBoxButton.OK);
                    tabChoice.IsSelected = true;
                    PopulateCourses();
                }
            }
        }

        private void PopulateCourses()
        {
            cbCollegeName.Items.Add(" ");
            // Initialization.  
            string strConn = "Data Source=LAPTOP-6RBMDIS7\\SQLEXPRESS;Database=DBMS_Project;Integrated Security=SSPI;";
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCommand = new SqlCommand();
            object rowCount;
            try
            {
                // Settings.  
                sqlCommand.CommandText = $"SELECT DISTINCT(colleges_participating) FROM course_master";
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

                        cbCollegeName.Items.Add(reader.GetString(0));
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

        private void FetchMarkDetails()
        {
            // Initialization.  
            string strConn = "Data Source=LAPTOP-6RBMDIS7\\SQLEXPRESS;Database=DBMS_Project;Integrated Security=SSPI;";
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCommand = new SqlCommand("uspFetchMarkDetails", sqlConnection);
            object rowCount;
            try
            {
                // Settings.  
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandTimeout = 12 * 3600; //// Setting timeeout for longer queries to 12 hours.  

                sqlCommand.Parameters.Add("@student_id", SqlDbType.Int).Value = _studentId;

                // Open.  
                sqlConnection.Open();

                SqlDataReader reader = sqlCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                        if (reader.GetInt32(0) == 1)
                            tbPhysics.Text = reader.GetValue(1).ToString();
                        else if (reader.GetInt32(0) == 2)
                            tbChemistry.Text = reader.GetValue(1).ToString();
                        else if (reader.GetInt32(0) == 3)
                            tbMath.Text = reader.GetValue(1).ToString();
                        else if (reader.GetInt32(0) == 4)
                            tbJEE.Text = reader.GetValue(1).ToString();
                        else if (reader.GetInt32(0) == 5)
                            tbEnglish.Text = reader.GetValue(1).ToString();
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                // Close.  
                sqlConnection.Close();
            }
        }

        private bool UpdateMarkDetails()
        {
            // Initialization.  
            string strConn = "Data Source=LAPTOP-6RBMDIS7\\SQLEXPRESS;Database=DBMS_Project;Integrated Security=SSPI;";
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCommand = new SqlCommand("uspSaveMarkDetails", sqlConnection);
            object rowCount;
            try
            {
                // Settings.  
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandTimeout = 12 * 3600; //// Setting timeeout for longer queries to 12 hours.  

                sqlCommand.Parameters.Add("@student_id", SqlDbType.Int).Value = _studentId;
                sqlCommand.Parameters.Add("@physics", SqlDbType.Float).Value = float.Parse(tbPhysics.Text);
                sqlCommand.Parameters.Add("@chemistry", SqlDbType.Float).Value = float.Parse(tbChemistry.Text);
                sqlCommand.Parameters.Add("@maths", SqlDbType.Float).Value = float.Parse(tbMath.Text);
                sqlCommand.Parameters.Add("@jeemain", SqlDbType.Float).Value = float.Parse(tbJEE.Text);
                sqlCommand.Parameters.Add("@english", SqlDbType.Float).Value = float.Parse(tbEnglish.Text);

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
                SqlCommand sqlCommand = new SqlCommand("uspSaveDocumentDetails", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add("@student_id", SqlDbType.Int).Value = _studentId;
                sqlCommand.Parameters.Add("@docId", SqlDbType.Int).Value = docID;
                sqlCommand.Parameters.Add("@doc", SqlDbType.VarBinary).Value = file;
                sqlCommand.Parameters.Add("@docName", SqlDbType.VarChar).Value = docName;
                sqlConnection.Open();

                sqlCommand.ExecuteNonQuery();
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
                    if (sqlQueryResult != null && sqlQueryResult.HasRows)
                    {
                        sqlQueryResult.Read();
                        var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                        sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                        docName = sqlQueryResult.GetString(1);
                        using (var fs = new FileStream(".\\"+ docName, FileMode.Create, FileAccess.Write))
                            fs.Write(blob, 0, blob.Length);
                    }
            }
            if (docId == 2)
            {
                hyperlinkText.Text = docName;
                tbTenthMLClick.Tag = ".\\" + docName;
            }
            else if(docId == 3)
            {
                hyperlinkTextTwelthML.Text = docName;
                tbTwelfthMLClick.Tag = ".\\" + docName;
            }
            else if (docId == 4)
            {
                hyperlinkTextCommCert.Text = docName;
                tbCommCertClick.Tag = ".\\" + docName;
            }
            else if (docId == 5)
            {
                hyperLinkBirthCert.Text = docName;
                tbBirthCert.Tag = ".\\" + docName;
            }
            else if (docId == 6)
            {
                idProofHyperlinkText.Text = docName;
                tbIdProof.Tag = ".\\" + docName;
            }
            else if (docId == 7)
            {
                nativityHyperLink.Text = docName;
                tbNativity.Tag = ".\\" + docName;
            }
            else if (docId == 8)
            {
                transferHyperLink.Text = docName;
                tbTransferCert.Tag = ".\\" + docName;
            }
            else if (docId == 9)
            {
                jeeMainHyperlink.Text = docName;
                tbJeeMain.Tag = ".\\" + docName;
            }
        }
        bool UpdateDocumentDetail()
        {
            if (!string.IsNullOrEmpty(hyperlinkText.Text))
                databaseFilePut(tbTenthMLClick.Tag.ToString(), 2, hyperlinkText.Text);
            if (!string.IsNullOrEmpty(hyperlinkTextTwelthML.Text))
                databaseFilePut(tbTwelfthMLClick.Tag.ToString(), 3, hyperlinkTextTwelthML.Text);
            if (!string.IsNullOrEmpty(hyperlinkTextCommCert.Text))
                databaseFilePut(tbCommCertClick.Tag.ToString(), 4, hyperlinkTextCommCert.Text);
            if (!string.IsNullOrEmpty(hyperLinkBirthCert.Text))
                databaseFilePut(tbBirthCert.Tag.ToString(), 5, hyperLinkBirthCert.Text);
            if (!string.IsNullOrEmpty(idProofHyperlinkText.Text))
                databaseFilePut(tbIdProof.Tag.ToString(), 6, idProofHyperlinkText.Text);
            if (!string.IsNullOrEmpty(nativityHyperLink.Text))
                databaseFilePut(tbNativity.Tag.ToString(), 7, nativityHyperLink.Text);
            if (!string.IsNullOrEmpty(transferHyperLink.Text))
                databaseFilePut(tbTransferCert.Tag.ToString(), 8, transferHyperLink.Text);
            if (!string.IsNullOrEmpty(jeeMainHyperlink.Text))
                databaseFilePut(tbJeeMain.Tag.ToString(), 9, jeeMainHyperlink.Text);

            return true;
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
                tbTwelfthMLClick.Tag = openFileDialog1.FileName;
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

        private void OnBirthCertClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == true)
            {
                hyperLinkBirthCert.Text = openFileDialog1.SafeFileName;
                tbBirthCert.Tag = openFileDialog1.FileName;
            }
        }

        private void hlBirthCertClicked(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(tbBirthCert.Tag.ToString())
            {
                UseShellExecute = true
            };
            p.Start();
        }

        private void OnIdProofUpload(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == true)
            {
                idProofHyperlinkText.Text = openFileDialog1.SafeFileName;
                tbIdProof.Tag = openFileDialog1.FileName;
            }
        }

        private void IDProofLink(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(tbIdProof.Tag.ToString())
            {
                UseShellExecute = true
            };
            p.Start();
        }

        private void nativityLinkClick(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(tbNativity.Tag.ToString())
            {
                UseShellExecute = true
            };
            p.Start();
        }

        private void OnNativityUpload(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == true)
            {
                nativityHyperLink.Text = openFileDialog1.SafeFileName;
                tbNativity.Tag = openFileDialog1.FileName;
            }
        }

        private void OnTransferCertUpload(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == true)
            {
                transferHyperLink.Text = openFileDialog1.SafeFileName;
                tbTransferCert.Tag = openFileDialog1.FileName;
            }
        }

        private void transferCertLinkClick(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(tbTransferCert.Tag.ToString())
            {
                UseShellExecute = true
            };
            p.Start();
        }

        private void jeeMainLinkClick(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(tbJeeMain.Tag.ToString())
            {
                UseShellExecute = true
            };
            p.Start();
        }

        private void OnJeeMainUpload(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == true)
            {
                jeeMainHyperlink.Text = openFileDialog1.SafeFileName;
                tbJeeMain.Tag = openFileDialog1.FileName;
            }
        }

        private void cbCollegeName_Selected(object sender, RoutedEventArgs e)
        {
            cbCourseName.Items.Clear();
            cbCourseName.Items.Add(" ");

            // Initialization.  
            string strConn = "Data Source=LAPTOP-6RBMDIS7\\SQLEXPRESS;Database=DBMS_Project;Integrated Security=SSPI;";
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCommand = new SqlCommand();
            
            try
            {
                // Settings.  
                sqlCommand.CommandText = $"SELECT branch FROM course_master WHERE colleges_participating = '{cbCollegeName.SelectedItem}'";
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

                        cbCourseName.Items.Add(reader.GetString(0));
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

        private void btnAddCourse_Click(object sender, RoutedEventArgs e)
        {
            int priority = 0;
            if (string.IsNullOrWhiteSpace(cbCourseName.SelectedItem.ToString()))
            {
                MessageBox.Show("Please select course name", "Invalid course name", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(cbCollegeName.SelectedItem.ToString()))
            {
                MessageBox.Show("Please select college name", "Invalid college name", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(udPriority.Value.ToString(), out priority) || priority < 1)
            {
                MessageBox.Show("Please enter value more than or equal to 1", "Invalid priority", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var course = new SelectedCourse() { CollegeName = cbCollegeName.SelectedItem.ToString(), Priority = priority, CourseName = cbCourseName.SelectedItem.ToString() };
            foreach(var courseItem in lvSelectedCourse.Items)
            {
                if(((SelectedCourse)courseItem).CollegeName == course.CollegeName 
                    && ((SelectedCourse)courseItem).CourseName == course.CourseName )
                {
                    MessageBox.Show("Same combination exists, please enter different combination", "Combination exists", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (((SelectedCourse)courseItem).Priority == course.Priority)
                {
                    MessageBox.Show("Same priority exists, please enter different priority", "Priority exists", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            
            lvSelectedCourse.Items.Add(course);
        }

        private void RemoveCourse(object sender, RoutedEventArgs e)
        {
            var selectCourse = (SelectedCourse)((Button)sender).DataContext;
            lvSelectedCourse.Items.Remove(selectCourse);
        }
    }
    public class SelectedCourse
    {
        public string CollegeName { get; set; }

        public int Priority { get; set; }

        public string CourseName { get; set; }
    }
}
