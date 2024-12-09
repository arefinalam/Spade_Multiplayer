using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Examples.Utils;

public class FirebaseUserDatabaseController : MonoBehaviour
{
    string firebaseBasePath = "Users";
    Action<UserGameData> UserGameDataCallBack;
    private UserGameData userGameData;
    #region User Data

    public void CreateUser(UserGameData userData, Action<UserGameData> callback = null)
    {
        UserGameDataCallBack = callback;        
        string userDataString = JsonUtility.ToJson(userData);
        GameManager.Instance.User = userData;
        LogManager.Instance.ConsoleLog("Creating new user:" + userDataString);
        FirebaseFirestore.SetDocument(firebaseBasePath, userData.UserId, userDataString, gameObject.name, nameof(SignupSuccess), nameof(SignupFailed));
    }

    private void SignupSuccess(string data)
    {
        LogManager.Instance.ConsoleLog("New user created." + data);
        UserGameDataCallBack?.Invoke(GameManager.Instance.User);
    }
    private void SignupFailed(string data)
    {
        LogManager.Instance.ErrorLog("Failed to create new user:" + data);
        UserGameDataCallBack?.Invoke(null);
    }

    public void GetUserData(string userid, Action<UserGameData> callback = null)
    {
        UserGameDataCallBack = callback;
        LogManager.Instance.ConsoleLog("userID: " + userid);  
        FirebaseFirestore.GetDocument(firebaseBasePath, userid, gameObject.name, nameof(SigninSuccess), nameof(SigninFailed));
    }
    private void SigninSuccess(string data)
    {
        LogManager.Instance.ConsoleLog("User data fetch:" + data);
        UserGameData userdata = StringSerializationAPI.Deserialize(typeof(UserGameData), data) as UserGameData;
        UserGameDataCallBack?.Invoke(userdata);
    }
    private void SigninFailed(string data)
    {
        LogManager.Instance.ErrorLog("Failed to fetch user data:" + data);
        UserGameDataCallBack?.Invoke(null);
    }

    public void UpdateUserData(Action<UserGameData> callback = null)
    {
        UserGameDataCallBack = callback;
        string userDataString = JsonUtility.ToJson(GameManager.Instance.User);
        FirebaseFirestore.UpdateDocument(firebaseBasePath, GameManager.Instance.User.UserId, userDataString, gameObject.name, nameof(UpdateSuccess), nameof(UpdateFailed));
    }    

    private void UpdateSuccess(string data)
    {
        LogManager.Instance.ErrorLog("User data updated:" + data);
        UserGameData userdata = StringSerializationAPI.Deserialize(typeof(UserGameData), data) as UserGameData;
        UserGameDataCallBack?.Invoke(userdata);
    }
    private void UpdateFailed(string data)
    {
        LogManager.Instance.ErrorLog("Failed to update user data:" + data);
        UserGameDataCallBack?.Invoke(null);
    }
    #endregion

}
