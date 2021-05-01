using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDLL.Internal.DataAccess
{
    internal class SQLDataAccess : IDisposable
    {

        private bool _isClosed = false;
        private readonly IConfiguration _configuration;


        public SQLDataAccess(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString(string name)
        {
            return _configuration.GetConnectionString(name);
        }

        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                List<T> rows = connection.Query<T>(storedProcedure, parameters,
                               commandType: CommandType.StoredProcedure).ToList();

                return rows;
            }
        }
        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute(storedProcedure, parameters,
                  commandType: CommandType.StoredProcedure);
            }
        }


        #region Transaction
         private IDbConnection _connection;
         private IDbTransaction _transaction;

        public void StartTransaction(string connectionStringName)
        {
            //pas de using implicit pour se connecter 
            _connection = new SqlConnection(connectionStringName);
            _connection.Open();
            _transaction = _connection.BeginTransaction();

            _isClosed = false;
        }

        //param :value permet de ne pas suivre l'order of parameter dans la signature 
        //de la methode
        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
        {
            List<T> rows = _connection.Query<T>(storedProcedure, parameters,
                           commandType: CommandType.StoredProcedure, transaction: _transaction).ToList();

            return rows;
        }


        public void SaveDataInTransaction<T>(string storedProcedure, T parameters)
        {

            _connection.Execute(storedProcedure, parameters,
            commandType: CommandType.StoredProcedure, transaction: _transaction);
        }


    

        public void CommitTransaction()
        {
            _transaction?.Commit();
            _connection?.Close();
            _isClosed = true;
        }

        public void RoolBackTransaction()
        {
            _transaction?.Rollback();
            _connection?.Close();
            _isClosed = true;
        }

        #endregion

        public void Dispose()
        {
            if (_isClosed==false)
            {

                try
                {
                    CommitTransaction();
                }
                catch 
                {
                   
                }  
            }
            _transaction = null;
            _connection = null;
        }
    }

    //Pourqoui transaction?
    //=====================
    //le using le fait normalement appel le dispose pour fermeture de connection
    //ici on le rajout pour la transaction car dans une transaction on va pas
    //utiliser le using car on se sait pas quand elle finit la transaction si il
    //faut un rollback

    //implementation avant transaction
    //=================================
    //premier version sans transaction et sans IDisposable
    //internal class SQLDataAccess : IDisposable
    //{
    //    public string GetConnectionString(string name)
    //    {
    //        return ConfigurationManager.ConnectionStrings[name].ConnectionString;
    //    }

    //public List<T> LoadData<T,U>(string storedProcedure,U parameters,string connectionStringName)
    //{
    //    string connectionString = GetConnectionString(connectionStringName);
    //    using (IDbConnection connection = new SqlConnection(connectionString)) 
    //    {
    //        List<T> rows = connection.Query<T>(storedProcedure, parameters, 
    //                       commandType: CommandType.StoredProcedure).ToList();

    //        return rows;
    //    }
    //}

    //implementatio Save Data InTransaction
    //public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
    //{
    //    string connectionString = GetConnectionString(connectionStringName);
    //    using (IDbConnection connection = new SqlConnection(connectionString))
    //    {
    //        connection.Execute(storedProcedure, parameters,
    //          commandType: CommandType.StoredProcedure);
    //    }
    //}
    //}

}
