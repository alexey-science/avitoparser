using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace AvitoInformer
{
     static class avitoDB
    {
       
        private static SQLiteCommand cmdDB;


        public static SQLiteConnection Con()
        {
            var condb = new SQLiteConnection("Data Source=data/avitodatafile.db;Version=3;");
            
            return condb;
        }

        public static void ExecNonQuery(string query, SQLiteConnection con) 
        {
            cmdDB = new SQLiteCommand(query, con);
            cmdDB.ExecuteNonQuery();
            cmdDB.Dispose();
        }

        public static SQLiteDataReader ExecQuery(string query, SQLiteConnection con)
        {
            cmdDB = new SQLiteCommand(query, con);
            return cmdDB.ExecuteReader();
        }

        public static object ExecScalar(string query, SQLiteConnection con)
        {
            cmdDB = new SQLiteCommand(query, con);
            return cmdDB.ExecuteScalar();
        }

        public static void closeDispoe()
        {
            cmdDB.Dispose();
        }

    }
}
