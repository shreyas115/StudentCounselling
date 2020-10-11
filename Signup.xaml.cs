using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StudentCounselling
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Signup : Window
    {
        public Signup()
        {
            InitializeComponent();
            CenterWindowOnScreen();
        }
        public bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        private void OnCreateClicked(object sender, RoutedEventArgs e)
        {
            if(!IsValidEmail(tbEmail.Text))
            {
                MessageBox.Show("Please enter valid email", "Invalid Email Id", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }
            if(tbPassword.Password != tbConfirmPassword.Password)
            {
                MessageBox.Show("Password and Confirm password doesnt match", "Password error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }
            if(IsUserExists())
            {
                MessageBox.Show("User name already exists, please chose a different user name", "User name exists", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }
            // Initialization.  
            string strConn = "Data Source=LAPTOP-6RBMDIS7\\SQLEXPRESS;Database=DBMS_Project;Integrated Security=SSPI;";
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCommand = new SqlCommand();
                
            try
            {
                // Settings.  
                sqlCommand.CommandText = $"Insert into student_details (user_name, password,email, phone_no) values ('{tbUserName.Text}','{tbPassword.Password}','{tbEmail.Text}',{tbPhoneNo.Text})";
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandTimeout = 12 * 3600; //// Setting timeeout for longer queries to 12 hours.  

                // Open.  
                sqlConnection.Open();

                // Result.  
                var rowCount = sqlCommand.ExecuteNonQuery();

                // Close.  
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                // Close.  
                sqlConnection.Close();
                MessageBox.Show("Unable to create user, please try again", "System Issue", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }
            var result = MessageBox.Show("Account created successfully. Would you like to login?", "Account Created", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
                MessageBox.Show("Selected Yes", "Yes!!!", MessageBoxButton.OKCancel, MessageBoxImage.Information);

            Close();
        }

        private bool IsUserExists()
        {
            // Initialization.  
            string strConn = "Data Source=LAPTOP-6RBMDIS7\\SQLEXPRESS;Database=DBMS_Project;Integrated Security=SSPI;";
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCommand = new SqlCommand();
            object rowCount;
            try
            {
                // Settings.  
                sqlCommand.CommandText = $"Select Count(*) from student_details where user_name = '{tbUserName.Text}'";
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandTimeout = 12 * 3600; //// Setting timeeout for longer queries to 12 hours.  

                // Open.  
                sqlConnection.Open();

                // Result.  
                rowCount = sqlCommand.ExecuteScalar();
                // Close.  
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                // Close.  
                sqlConnection.Close();

                return true;
            }
            if ((int)rowCount == 0)
                return false;
            return true;
        }
        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {

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
    }
}
