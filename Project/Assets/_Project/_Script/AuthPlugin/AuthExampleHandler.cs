using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
using TMPro;
using UnityEngine;

namespace FirebaseWebGL.Examples.Auth
{
    public class AuthExampleHandler : MonoBehaviour
    {
        /*public TMP_InputField emailInputField;
        public TMP_InputField passwordInputField;

        public TextMeshProUGUI outputText;
        public TextMeshProUGUI logText;

        private void Start()
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                DisplayError("The code is not running on a WebGL build; as such, the Javascript functions will not be recognized.");
                return;
            }
            
            FirebaseAuth.OnAuthStateChanged(gameObject.name, "DisplayUserInfo", "DisplayInfo");
        }

        public void CreateUserWithEmailAndPassword()
        {
            ShowLog("creating user");
            FirebaseAuth.CreateUserWithEmailAndPassword(emailInputField.text, passwordInputField.text, gameObject.name, "DisplayInfo", "DisplayErrorObject");
        }

        public void SignInWithEmailAndPassword()
        {
            ShowLog("btn pressed");
            FirebaseAuth.SignInWithEmailAndPassword(emailInputField.text, passwordInputField.text, gameObject.name, "DisplayInfoT", "DisplayErrorObject");
        }
            
        
        public void SignInWithGoogle() =>
            FirebaseAuth.SignInWithGoogle(gameObject.name, "DisplayInfo", "DisplayErrorObject");
        
        public void SignInWithFacebook() =>
            FirebaseAuth.SignInWithFacebook(gameObject.name, "DisplayInfo", "DisplayErrorObject");
        
        public void DisplayUserInfo(string user)
        {
            var parsedUser = StringSerializationAPI.Deserialize(typeof(AuthCallbackData), user) as AuthCallbackData;
            DisplayData($"Email: {parsedUser.email}, UserId: {parsedUser.uid}, EmailVerified: {parsedUser.isEmailVerified}");
            ShowLog($"Email: {parsedUser.email}, UserId: {parsedUser.uid}, EmailVerified: {parsedUser.isEmailVerified}");
        }

        public void DisplayData(string data)
        {
            outputText.color = outputText.color == Color.green ? Color.blue : Color.green;
            outputText.text = data;
            Debug.Log(data);
            ShowLog(data);
        }

        public void DisplayInfo(string info)
        {
            outputText.color = Color.white;
            outputText.text = info;
            Debug.Log(info);
            ShowLog(info);
        }

        public void DisplayInfoT(string info)
        {
            ShowLog("Signed in with email successfull");
            ShowLog(info);
        }

        public void DisplayErrorObject(string error)
        {
            var parsedError = StringSerializationAPI.Deserialize(typeof(FirebaseError), error) as FirebaseError;
            DisplayError(parsedError.message);
            ShowLog($"Error: {parsedError.message}");
        }

        public void DisplayError(string error)
        {
            outputText.color = Color.red;
            outputText.text = error;
            Debug.LogError(error);
            ShowLog(error);
        }

        public void ShowLog(string message)
        {
            logText.text += $"{message}\n";
        }*/
    }
}