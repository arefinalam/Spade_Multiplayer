using UnityEngine;

public class FakeAuth : MonoBehaviour
{
    public string username;

    private void Start()
    {
        if (username.Equals("")) username = "testUser" + Random.Range(10, 100);
        Login();
    }

    [ButtonLUFI]
    void Login()
    {
        string _username = username.Equals("") ? "TestUser" + Random.Range(10, 100) : username;
        string _uid = username.Equals("") ? "TestUserUID" + Random.Range(100, 1000) : username + "UID" + Random.Range(100, 1000);

        GameManager.Instance.User = new UserGameData(_username, _username+"@test.com", _uid);
        FindObjectOfType<HomeUIController>().ShowMainPanel();
    }
}
