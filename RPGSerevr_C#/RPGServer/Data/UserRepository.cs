using System.Runtime.InteropServices.JavaScript;
using RPGServer.Data.interfaces;
using RPGServer.Modules;

namespace RPGServer.Data;

/// <summary>
/// 对数据库做增删改查
/// </summary>
public class UserRepository: IUserRepository
{
    // temp: use dic to storage
    private Dictionary<int, UserModule> userStorages = new Dictionary<int, UserModule>();
    private Dictionary<string, int> userNameStorages = new Dictionary<string, int>();

    private int userIndex = 1;
    private readonly object _lock = new object();
    
    public void Add(UserModule userModule)
    {
        lock (_lock)
        {
            userModule.Id = userIndex;
            userStorages[userIndex] = userModule;
            userNameStorages[userModule.UserName.ToLower()] = userIndex;
            userIndex++;
        }
    }

    public void Update(UserModule userModule)
    {
        lock (_lock)
        {
            if (userStorages.ContainsKey(userModule.Id))
            {
                userStorages[userModule.Id] = userModule;
            }
        }
    }

    public UserModule GetById(int id)
    {
        lock (_lock)
        {
            return userStorages.ContainsKey(id) ? userStorages[id] : null;
        }
    }

    public UserModule GetByUsername(string username)
    {
        lock (_lock)
        {
            var str = username.ToLower();
            if (userNameStorages.ContainsKey(str))
            {
                int id = userNameStorages[str];
                return userStorages[id];
            }

            return null;
        }
    }
}