using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace TaskManager.Data
{
    public class UserRepository
    {
        private readonly GenericRepository<User> _genericRepository;

        public UserRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Task_Manager_DB"].ConnectionString;
            _genericRepository = new GenericRepository<User>(connectionString);
        }

        public Task<List<User>> GetAllUsersAsync()
        {
            string query = "SELECT * FROM USERS";
            return _genericRepository.GetAllAsync(query);
        }

        public Task<User> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            string query = "SELECT * FROM USERS WHERE PHONE_NUMBER = @PhoneNumber";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PhoneNumber", phoneNumber)
            };
            return _genericRepository.GetSingleAsync(query, parameters);
        }

        public Task AddUserAsync(User user)
        {
            string query = "INSERT INTO USERS (USER_NAME, USER_SURNAME, USER_ADRESS, TC_IDENTITY, IS_STATUS, IS_ADMIN, PASSWORD) VALUES (@UserName, @UserSurname, @UserAddress, @TcIdentity, @IsStatus, @IsAdmin, @Password)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserName", user.USER_NAME),
                new SqlParameter("@UserSurname", user.USER_SURNAME),
                new SqlParameter("@UserAddress", user.USER_ADRESS),
                new SqlParameter("@TcIdentity", user.TC_IDENTITY),
                new SqlParameter("@IsStatus", user.IS_STATUS),
                new SqlParameter("@IsAdmin", user.IS_ADMIN),
                new SqlParameter("@Password", user.PASSWORD)
            };
            return _genericRepository.AddAsync(query, parameters);
        }

        public Task UpdateUserAsync(User user)
        {
            string query = "UPDATE USERS SET USER_NAME = @UserName, USER_SURNAME = @UserSurname, USER_ADRESS = @UserAddress, TC_IDENTITY = @TcIdentity, IS_STATUS = @IsStatus, IS_ADMIN = @IsAdmin, PASSWORD = @Password WHERE USER_ID = @UserId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserName", user.USER_NAME),
                new SqlParameter("@UserSurname", user.USER_SURNAME),
                new SqlParameter("@UserAddress", user.USER_ADRESS),
                new SqlParameter("@TcIdentity", user.TC_IDENTITY),
                new SqlParameter("@IsStatus", user.IS_STATUS),
                new SqlParameter("@IsAdmin", user.IS_ADMIN),
                new SqlParameter("@Password", user.PASSWORD),
                new SqlParameter("@UserId", user.USER_ID)
            };
            return _genericRepository.UpdateAsync(query, parameters);
        }

        public Task DeleteUserAsync(int userId)
        {
            string query = "DELETE FROM USERS WHERE USER_ID = @UserId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", userId)
            };
            return _genericRepository.DeleteAsync(query, parameters);
        }
    }
}