using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Security.Permissions;

namespace SqlDependencyWorkerService.Service
{
    public class MicrosoftSqlDependency
    {
        private readonly SqlConnection _connection;
        private readonly SqlCommand _command;
        private SqlDependency _dependency;
        private const string ConnectionString = "Data Source=.;Initial Catalog=SqlDepTest;Integrated Security=True";
        private const string SelectQuery = @"SELECT [name],[lastname] FROM [dbo].[person]";
        public MicrosoftSqlDependency()
        {
            _connection = new SqlConnection(ConnectionString);
            _command = new SqlCommand(SelectQuery, _connection);
        }

        public void StartMonitoring()
        {
           SqlDependency.Stop(ConnectionString);
            SqlDependency.Start(ConnectionString);

            RegisterSqlDependency();

        }




        private void RegisterSqlDependency()
        {
            if (!DoesUserHavePermission())
                return;

            _command.Notification = null;
            _dependency = new SqlDependency(_command);
            _dependency.OnChange += _dependency_OnChange;

            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            _command.ExecuteNonQuery();
            _connection.Close();
        }

        private void _dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            var dependency = (SqlDependency)sender;
            dependency.OnChange -= _dependency_OnChange;
            Console.WriteLine(e.Info);
            RegisterSqlDependency();
        }


        private static bool DoesUserHavePermission()
        {
            try
            {
                var clientPermission = new SqlClientPermission(PermissionState.Unrestricted);
                clientPermission.Demand();
                return true;
            }
            catch
            {
                return false;
            }
        }


        void Termination()
        {
            // Release the dependency.
            SqlDependency.Stop(_connection.ConnectionString);
        }
    }

}



