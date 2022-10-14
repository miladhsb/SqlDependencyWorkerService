using Newtonsoft.Json;
using SqlDependencyWorkerService.Service;
using System.Xml;

namespace SqlDependencyWorkerService
{
    public class Worker : BackgroundService
    {
     
        private const string ConnectionString = "Data Source=.;Initial Catalog=SqlDepTest;Integrated Security=True";
        public Worker()
        {
            
        }



        //فعال کردن بروکر در دیتابیس برای کتابخانه 
        // ALTER DATABASE[نام دیتابیس] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
        // ALTER DATABASE[نام دیتابیس] SET ENABLE_BROKER;
        //ALTER DATABASE[نام دیتابیس] SET MULTI_USER WITH ROLLBACK IMMEDIATE
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //کتابخانه MySqlDependency
            #region MySqlDependency
            //  MicrosoftSqlDependency MySqlDependency = new MicrosoftSqlDependency();
            //  MySqlDependency.StartMonitoring();
            #endregion

            //کتابخانه sqlTableDependency
            #region sqlTableDependency

            // SqlTableDependencyService sqlTableDependency = new SqlTableDependencyService();
            //  sqlTableDependency.StartLesiner();
            #endregion


            //کتابخانه SqlDependencyEx
            #region SqlDependencyEx
            SqlDependencyEx sqlDependencyEx = new SqlDependencyEx(ConnectionString, "SqlDepTest", "person", "dbo", SqlDependencyEx.NotificationTypes.Insert | SqlDependencyEx.NotificationTypes.Update, true, 1);

            sqlDependencyEx.TableChanged += (o, e) => {

                //هر رو وضعیت قبل و بعد دیتا را نشان میدهد
                Console.WriteLine(e.Data.ToString());
                //وضعیت بعد از تغییر دیتا
                Console.WriteLine(e.Data.Element("inserted"));
                //وضعیت قبل از تغییر دیتا
                Console.WriteLine(e.Data.Element("deleted"));

                //تبدیل دیتا به جیسون
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(e.Data.ToString());
                var JsonPerson = JsonConvert.SerializeXmlNode(xml);

            };
            sqlDependencyEx.Start();
            #endregion


        }
    }
}