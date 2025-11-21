using System.Security.Cryptography;
using System.Text;
using RPGServer.Data.interfaces;
using RPGServer.Modules;
using RPGServer.Protocol;
using RPGServer.Protocol.Packs.Response;
using RPGServer.Service.Interface;

namespace RPGServer.Service;

/// <summary>
/// 具体服务的逻辑处理类
/// </summary>
public class UserServices: IUserService
{
    private readonly IUserRepository _userRepository;

    public UserServices(IUserRepository repository)
    {
        _userRepository = repository;
    }

    /// <summary>
    /// 注册
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public UserResponse Register(string username, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(password))
            {
                return new UserResponse()
                {
                    Code = ResponseCode.InvalidData,
                    Message = "用户名和密码不能为空"
                };
            }

            UserModule user = GetUserByUsername(username);
            if (user != null)
            {
                return new UserResponse()
                {
                    Code = ResponseCode.UserExists,
                    Message = "该用户已存在"
                };
            }

            user = new UserModule();
            user.UserName = username;
            user.PwdHash = HashPassword(password);
            user.CreateTime=DateTime.Now;
            user.LastLogin=DateTime.Now;
            
            _userRepository.Add(user);
            return new UserResponse()
            {
                Code = ResponseCode.Success,
                Message = "注册成功",
                UserId = user.Id
            };
        }
        catch (Exception e)
        {
           Console.WriteLine("注册失败");
           Console.ForegroundColor = ConsoleColor.Red;
           return new UserResponse()
           {
               Code = ResponseCode.Failed,
               Message = "注册失败"
           };
        }
    }

    /// <summary>
    /// 登陆
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public UserResponse Login(string username, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(password))
            {
                return new UserResponse()
                {
                    Code = ResponseCode.InvalidData,
                    Message = "用户名和密码不能为空"
                };
            }

            UserModule user = _userRepository.GetByUsername(username);
            if (user==null)
            {
                return new UserResponse()
                {
                    Code = ResponseCode.UserNotFound,
                    Message = "没有找到该用户"
                };
            }

            if (VerifyPassword(password,user.PwdHash)==false)
            {
                return new UserResponse()
                {
                    Code = ResponseCode.WrongPassword,
                    Message = "密码错误"
                };
            }
            
            user.LastLogin = DateTime.Now;
            _userRepository.Update(user);

            return new UserResponse()
            {
                Code = ResponseCode.Success,
                Message = "登录成功",
                UserId = user.Id
            };
        }
        catch (Exception e)
        {
            Console.WriteLine("登录失败");
            Console.ForegroundColor = ConsoleColor.Red;
            return new UserResponse()
            {
                Code = ResponseCode.Failed,
                Message = "登录失败"
            };
        }
    }

    public UserModule GetUserByUsername(string username)
    {
        return _userRepository.GetByUsername(username);
    }

    private bool VerifyPassword(string password, string hash)
    {
        var pwdHash = HashPassword(password);
        return pwdHash.Equals(hash);
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}