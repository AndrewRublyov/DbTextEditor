using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Dapper;
using DbTextEditor.Model.Entities;
using DbTextEditor.Shared.Exceptions;

namespace DbTextEditor.Model.DAL
{
    public class DbFilesRepository : IDbFilesRepository
    {
        private const string FilesTable = "files";

        private readonly string _connectionString;

        public DbFilesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Exists(string name)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return connection
                    .Query<long>($"SELECT EXISTS(SELECT 1 FROM {FilesTable} WHERE Name = @Name);", new { Name = name })
                    .First() == 1;
            }
        }

        public void Create(DbFileEntity entity)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                connection.Execute($"INSERT INTO {FilesTable} " +
                                        "(Name, Revision, Contents) VALUES " +
                                        "(@Name, @Revision, @Contents); " +
                                        "SELECT last_insert_rowid();", entity);
            }
        }

        public DbFileEntity Get(string name)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return connection
                    .Query<DbFileEntity>($"SELECT * FROM {FilesTable} WHERE Name = @Name;", new { Name = name })
                    .FirstOrDefault();
            }
        }

        public void Update(DbFileEntity entity)
        {
            var currentEntity = Get(entity.Name);
            if (currentEntity != null && entity.Revision < currentEntity.Revision)
            {
                throw new BusinessLogicException("You trying to modify old version of this file in DB");
            }
            using (var connection = GetConnection())
            {
                connection.Open();
                connection.Execute($"UPDATE {FilesTable} SET " +
                                        "Name = @Name, Revision = @Revision + 1, Contents = @Contents " +
                                        "WHERE Name = @Name", entity);
            }
        }

        public void Delete(string name)
        {
            if (!Exists(name))
            {
                throw new BusinessLogicException($"File with name '{name}' doesn't exists in DB");
            }
            using (var connection = GetConnection())
            {
                connection.Open();
                connection.Execute($"DELETE FROM {FilesTable} WHERE Name = @Name;", new { Name = name });
            }
        }

        public IEnumerable<DbFileEntity> GetAll()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return connection.Query<DbFileEntity>($"SELECT * FROM {FilesTable};").ToList();
            }
        }

        private SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }
    }
}