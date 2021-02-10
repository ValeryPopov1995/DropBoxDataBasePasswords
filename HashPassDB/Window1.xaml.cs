using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace HashPassDB
{
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}
		void login_Click(object sender, RoutedEventArgs e)
		{
			log.Content = DataConnection.Login(username.Text, password.Password);
		}
		void createuser_Click(object sender, RoutedEventArgs e)
		{
			log.Content = DataConnection.CreateUser(newusername.Text, newpassword.Password, confimpassword.Password);
		}
	}
}