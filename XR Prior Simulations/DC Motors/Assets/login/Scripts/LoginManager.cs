using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField usernameInputField;
    [SerializeField]
    private TMP_InputField passwordInputField;

    [SerializeField]
    private TextMeshProUGUI errorText;

    private string passFilePath = "/Resources/pass.txt";
    private bool isTeacher = false;
    private bool isStudent = false;
    private bool isAcademicEmail = false;

    public string u;
    public string p;

    private void Update()
    {
        // Easy inputs for quick testing
        if (Input.GetKeyDown(KeyCode.Q))
        {
            usernameInputField.text = "T1john@durjo.ac.uk";
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            passwordInputField.text = "hello123";
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnSubmitLogin();
        }
    }

    public void OnSubmitLogin()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (CheckLogin(username, password))
        {
            Debug.Log("LOGIN SUCCESSFUL");

            PlayerPrefs.SetInt("isTeacher", isTeacher ? 1 : 0);
            PlayerPrefs.Save();

            SceneManager.LoadScene(1); // Load next scene on successful login
        }
        else
        {
            Debug.Log("ERROR: Invalid Login Credentials");
            errorText.text = "ERROR: Invalid Login Credentials";
        }
    }

    private bool CheckLogin(string username, string password)
    {
        bool loginSuccess = false;

        // Read lines from pass file
        //string[] lines = File.ReadAllLines(Application.dataPath + passFilePath);

        string[] lines = Resources.Load<TextAsset>("pass").text.Split("\n");


        foreach (var line in lines)
        {
            // Split each line by ":" to get username and password
            var parts = line.Split(':');
            if (parts.Length == 2 && parts[0].Trim() == username && parts[1].Trim() == password)
            {
                isAcademicEmail = username.EndsWith("ac.uk");
                isTeacher = username.StartsWith("T1");
                isStudent = username.StartsWith("S2");
                loginSuccess = true;
                break;
            }
        }

        return loginSuccess;
    }

    public void RemoveErrorText()
    {
        errorText.text = "";
    }
}