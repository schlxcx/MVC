using MeChatness.Models;
using MySqlConnector;

namespace MeChatness.DataAccess;

public class LoginSqlManager
{
    private readonly string _connString;

    public LoginSqlManager(IConfiguration config) { _connString = config.GetConnectionString("Default")!; }

    public async Task<List<LoginDataModel>?> GetAllUserAsync()
    {
        List<LoginDataModel> userList = null;

        try
        {
            using var conn = new MySqlConnection(_connString);

            conn.Open();

            using var cmd = new MySqlCommand("SELECT * FROM user", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            userList = new List<LoginDataModel>();

            while (await reader.ReadAsync())
            {
                userList.Add(new LoginDataModel
                {
                    Id = reader.GetInt32("id"),
                    Username = reader.GetString("username"),
                    Email = reader.GetString("email")
                });
            }
        }
        catch (Exception ex) { }
        return userList;
    }

    public async Task<LoginDataModel?> GetUserByUsernameAsync(string username)
    {
        LoginDataModel? user = null;
        try
        {
            using var conn = new MySqlConnection(_connString);
            conn.Open();
            using var cmd = new MySqlCommand($"SELECT * FROM user WHERE username = '{username}'", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                user = new LoginDataModel
                {
                    Id = reader.GetInt32("id"),
                    Username = reader.GetString("username"),
                    Email = reader.GetString("email"),
                    Password = reader.GetString("password")
                };
            }
        }
        catch (Exception ex) { }
        return user;
    }
}
