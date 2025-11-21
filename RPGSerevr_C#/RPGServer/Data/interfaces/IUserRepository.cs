using RPGServer.Modules;

namespace RPGServer.Data.interfaces;

public interface IUserRepository
{
    void Add(UserModule userModule);
    void Update(UserModule userModule);
    UserModule GetById(int id);
    UserModule GetByUsername(string username);
}