using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.TestConsole
{
    public class Program
    {
        static void Main(string[] args)
        {

            var cfg = new Configuration()
                .DataBaseIntegration(db =>
                {
                    db.ConnectionString = "";
                    db.Dialect<MySQLDialect>();
                });

            /* Add the mapping we defined: */
            var mapper = new ModelMapper();
            mapper.AddMapping(typeof(Nuclear.EventSourcing.MySql.RecordedEventMap));

            HbmMapping mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
            cfg.AddMapping(mapping);



            string outfile = System.Environment.CurrentDirectory + "\\Nuclear.EventSourcing.Schema.mysql";

            Console.WriteLine(outfile);


            StringBuilder data = new StringBuilder();

            SchemaExport schema = new SchemaExport(cfg);

            schema.SetOutputFile(outfile)
                .Create((a) =>
                {
                    data.AppendLine(a);
                }, false);


            System.IO.File.WriteAllText(outfile, data.ToString());

            schema.SetOutputFile(outfile).Create(false, false);
            // schema.Create(true, false);

            Console.ReadKey();
        }
    }
}
