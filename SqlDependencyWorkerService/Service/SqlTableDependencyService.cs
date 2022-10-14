using SqlDependencyWorkerService.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;

namespace SqlDependencyWorkerService.Service
{
    public class SqlTableDependencyService
    {
        private const string ConnectionString = "Data Source=.;Initial Catalog=SqlDepTest;Integrated Security=True";
        public void StartLesiner()
        {
            var mapper = new ModelToTableMapper<Person>();

            mapper.AddMapping(p => p.id, "id");
            mapper.AddMapping(p => p.name, "name");
            mapper.AddMapping(p => p.lastname, "lastname");

            using (var Sqltabledep = new SqlTableDependency<Person>(ConnectionString, "person", mapper: mapper, includeOldValues: true))
            {
                Sqltabledep.OnChanged += Sqltabledep_OnChanged;
               // Sqltabledep.TraceLevel = System.Diagnostics.TraceLevel.Verbose;
               // Sqltabledep.TraceListener = new TextWriterTraceListener(Console.Out);
                Sqltabledep.Start();
              
                Console.Read();
                Sqltabledep.Stop();
            }
          
        }

        private void Sqltabledep_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Person> e)
        {
            Console.WriteLine("DML operation: " + e.ChangeType);
            Console.WriteLine(e.Entity.name);
            Console.WriteLine(e.Entity.lastname);
            Console.WriteLine(e.Entity.id);

            Console.WriteLine("----------------------");
            Console.WriteLine(e.EntityOldValues.name);
            Console.WriteLine(e.EntityOldValues.lastname);
            Console.WriteLine(e.EntityOldValues.id);


        }
    }
}
