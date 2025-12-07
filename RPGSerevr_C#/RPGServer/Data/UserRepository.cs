using System.Runtime.InteropServices.JavaScript;
using RPGServer.Data.interfaces;
using RPGServer.Modules;
using RPGServer.Utilities;

namespace RPGServer.Data;

/// <summary>
/// 对数据库做增删改查
/// </summary>
public class UserRepository: IUserRepository
{
    // temp: use dic to storage
    private Dictionary<long, UserModule> userStorages = new Dictionary<long, UserModule>();
    private Dictionary<string, long> userNameStorages = new Dictionary<string, long>();

    private readonly object _lock = new object();
    
    public void Add(UserModule userModule)
    {
        lock (_lock)
        {
            var uid = SnowFlake.GenerateId();
            userModule.Id = uid;
            userStorages[uid] = userModule;
            userNameStorages[userModule.UserName.ToLower()] = uid;
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
                long id = userNameStorages[str];
                return userStorages[id];
            }

            return null;
        }
    }
}