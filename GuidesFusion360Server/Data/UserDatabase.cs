using System;
using GuidesFusion360Server.Models;

namespace GuidesFusion360Server.Data
{
    public static class UserDatabase
    {
        public static UserLoginData SelectUserLoginData(string email)
        {
            var sql = $"SELECT U.id, U.password FROM Users AS U WHERE email='{email}'";
            var query = Database.SelectRowData(sql);

            return new UserLoginData()
            {
                Id = Int32.Parse(query[0]), 
                Password = query[1]
            };
        }

        public static UserData SelectUserData(int id)
        {
            var sql = 
                "SELECT U.id, U.email, U.firstName, U.lastName, U.access, U.studyGroup " + 
                $"FROM Users AS U WHERE id={id}";
            var query = Database.SelectRowData(sql);

            return new UserData()
            {
                Id = Int32.Parse(query[0]),
                Email = query[1],
                FirstName = query[2],
                LastName = query[3],
                Access = query[4],
                StudyGroup = query[5]
            };
        }

        public static string SelectUserAccess(int id)
        {
            var sql = $"SELECT U.access FROM Users AS U WHERE id={id}";
            var query = Database.SelectRowData(sql);
            
            return query[0];
        }

        public static int InsertNewUser(string email, string firstName, string lastName, string group, string password)
        {
            var sql =
                "INSERT INTO Users (firstName, lastName, email, password, studyGroup, access) " +
                $"VALUES ('{firstName}', '{lastName}', '{email}', '{password}', '{group}', 'unknown')";
            var lastId = Database.ChangeData(sql);

            return lastId;
        }
    }
}
