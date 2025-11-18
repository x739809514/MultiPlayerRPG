namespace RPGServer.Protocol;

public enum ResponseCode
{
    Success=0,
    Failed=1,
    InvalidData=2,
    UserExists=3,
    UserNotFound=4,
    WrongPassword=5,
    ServerError=6
}