using System;
using Network;
using Network.Protocol.Request;
using Network.Protocol.Response;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : MonoBehaviour
{
    private string username;
    private string pwdTxt;
    private RegisterType type;

    public InputField inputAccount;
    public InputField inputPwd;
    public Button registerBtn;
    public Button loginBtn;

    public static RegisterPanel Instance;

    public enum RegisterType
    {
        Login,
        Register   
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        inputAccount.onEndEdit.AddListener(UpdateAccount);
        inputPwd.onEndEdit.AddListener(UpdatePwd);
        registerBtn.onClick.AddListener(RegisterBtnOnClick);
        loginBtn.onClick.AddListener(LoginBtnOnClick);
        NetworkManager.Instance.onRegisterResponse += OnRegisterCallback;
        NetworkManager.Instance.onLoginResponse += OnLoginCallback;
        
        UpdateState(RegisterType.Register);
    }

    private void RegisterBtnOnClick()
    {
        var registerPacket = new RegisterRequest();
        registerPacket.userName = username;
        registerPacket.password = pwdTxt;
        
        NetworkManager.Instance.SendAsync(registerPacket);
    }

    private void LoginBtnOnClick()
    {
        var loginPacket = new LoginRequest();
        loginPacket.username = username;
        loginPacket.password = pwdTxt;
        
        NetworkManager.Instance.SendAsync(loginPacket);
    }

    private void UpdateAccount(string str)
    {
        username = str;
    }

    private void UpdatePwd(string str)
    {
        pwdTxt = str;
    }

    public void UpdateState(RegisterType rt)
    {
       loginBtn.gameObject.SetActive(rt==RegisterType.Login);
       registerBtn.gameObject.SetActive(rt == RegisterType.Register);
    }
    
    private void OnRegisterCallback(UserResponse response)
    {
        if (response.Code == ResponseCode.Success)
        {
            Debug.Log("注册成功");
            registerBtn.gameObject.SetActive(false);
            loginBtn.gameObject.SetActive(true);
        }
    }
    
    private void OnLoginCallback(UserResponse response)
    {
        if (response.Code == ResponseCode.Success)
        {
            Debug.Log("登陆成功");
            gameObject.SetActive(false);
        }
    }
}