using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace StudentCounselling
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            CenterWindowOnScreen();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var userName = tbUserName.Text;
            var password = tbPassword.Password;
            int studentId = IsValidUser();
            if (studentId != 0)
            {
                HomePage home = new HomePage(studentId);
                home.ShowDialog();
            }
            else
                MessageBox.Show("Invalid username or password. Please try again", "Invalid Credentials", MessageBoxButton.OKCancel, MessageBoxImage.Information);
        }

        private int IsValidUser()
        {
            // Initialization.  
            string strConn = "Data Source=LAPTOP-6RBMDIS7\\SQLEXPRESS;Database=DBMS_Project;Integrated Security=SSPI;";
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCommand = new SqlCommand();
            int studentId;
            try
            {
                // Settings.  
                sqlCommand.CommandText = $"Select SID from student_details where user_name = '{tbUserName.Text}' and password = '{tbPassword.Password}'";
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandTimeout = 12 * 3600; //// Setting timeeout for longer queries to 12 hours.  

                // Open.  
                sqlConnection.Open();

                // Result.  
                studentId = (int) sqlCommand.ExecuteScalar();
                // Close.  
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                // Close.  
                sqlConnection.Close();

                return 0;
            }
           
            return studentId;
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
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

        private void OnSignUpClicked(object sender, RoutedEventArgs e)
        {
            Signup signUp = new Signup();
            signUp.Show();
        }
    }
}
