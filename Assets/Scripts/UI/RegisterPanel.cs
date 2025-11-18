using System;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : MonoBehaviour
{
    private string accountTxt;
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
        
        UpdateState(RegisterType.Register);
    }

    private void RegisterBtnOnClick()
    {
        
    }

    private void LoginBtnOnClick()
    {
        
    }

    private void UpdateAccount(string str)
    {
        accountTxt = str;
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
}