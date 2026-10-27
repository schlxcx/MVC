using MeChatness.Models;
using MySqlConnector;

namespace MeChatness.DataAccess;

public class LoginSqlManager
{
    private readonly string _connString;

    public LoginSqlManager(IConfiguration config) { _connString = config.GetConnectionString("Default")!; }

    public async List<LoginDataModel> GetAllUserAsync()
    {
        List<LoginDataModel> userList = null;

        try
        {
            using var conn = new MySqlConnection(_connString);

            conn.Open();
        }
        catch (Exception ex) { }
    }
}
