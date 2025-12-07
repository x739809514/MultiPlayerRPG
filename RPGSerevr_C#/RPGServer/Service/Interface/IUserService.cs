using RPGServer.Modules;
using RPGServer.Protocol.Packs.Response;

namespace RPGServer.Service.Interface;

public interface IUserService
{
    UserResponse Register(string username, string password);
    UserResponse Login(string username, string password);
    UserModule GetUserByUsername(string username);
    bool Validation(long uid, string sessionId);
    void CloseSession(long uid);
}