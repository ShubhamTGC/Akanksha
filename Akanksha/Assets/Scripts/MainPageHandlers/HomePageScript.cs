using m2ostnextservice.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleSQL;
using System.Linq;
using System.IO;

public class HomePageScript : MonoBehaviour
{
    public GameObject HomePage, userSetupPage,LoginPage,AvatarSelectionPage,GamestoryPage;
    public GameObject PopupPage;
    public Text Msgbox;
    public SimpleSQLManager dbmanager;
    [Space(15)]
    [Header("User avatar selection page")]
    public Sprite HighlightedFrame;
    public Sprite NormalFrame;
    public List<GameObject> AvatarFrames;
    public Color Selected, Normalcolor;
    public Image Step1, Step2;

    [Header("Login task object")]
    public InputField Username;
    public InputField password;
    public GameObject UserLoginPage, userRegisterPage;
    private AESAlgorithm aeslog;

    void Start()
    {
        Debug.Log(Application.persistentDataPath);
    }
    private void OnEnable()
    {
        aeslog = new AESAlgorithm();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //METHOD FOR POP UP FOR ALL ISSUE OR STATUS
   public IEnumerator ShowPopUp(string msg)
    {
        PopupPage.SetActive(true);
        Msgbox.text = msg;
        yield return new WaitForSeconds(2.5f);
        iTween.ScaleTo(PopupPage, Vector3.zero, 0.3f);
        yield return new WaitForSeconds(0.35f);
        Msgbox.text = "";
        PopupPage.SetActive(false);
    }

    public void LetsPlay()
    {
        // CHECK FOR LAST LOGIN DATA AND IF IT MORE THEN 7 DAYS IT WILL LOGOUT USER
        if (!PlayerPrefs.HasKey("LastLogin"))
        {
            HomePage.SetActive(false);
            userSetupPage.SetActive(true);
            Step1.color = Selected;
            Step2.color = Normalcolor;
        }
        else
        {
            string datetime = PlayerPrefs.GetString("LastLogin");
            DateTime currentdate = Convert.ToDateTime(datetime);
            if (currentdate.Date.AddDays(7).Date.Equals(DateTime.Today.Date))
            {
                //LOGOUT SECTION
                HomePage.SetActive(false);
                userSetupPage.SetActive(true);
                Step1.color = Selected;
                Step2.color = Normalcolor;
            }
            else
            {
                //USER IS TILL LOGGED IN
                AfterLoginTask();
            }
        }
    }

   
    public void OpenLoginPage()
    {
        LoginPage.SetActive(true);
        userSetupPage.SetActive(false);
        Step1.color = Normalcolor;
        Step2.color = Selected;
    }

    // OPEN REGISTRAION PAGE
    public void CreateAccount()
    {
        Username.text = password.text = "";
        UserLoginPage.SetActive(false);
        userRegisterPage.SetActive(true);
    }

    // USER LOGIN TASK METHOD
    public void LoginTask()
    {
        if(Username.text == "" || password.text == "")
        {
            string msg = "Please fill your creditionals!";
            StartCoroutine(ShowPopUp(msg));
        }
        else
        {
            StartCoroutine(Loginprocess());
        }
    }

    //COROUTINE FOR LOGIN PROCESS AND DATA STORING TASK
    IEnumerator Loginprocess()
    {
     
        string HittingUrl = $"{MainUrls.BaseUrl}{MainUrls.LoginApi}";
        loginModel loginlog = new loginModel
        {
            Name = "",
            Email = Username.text,
            Password = aeslog.getEncryptedString(password.text),
            login_type = 1
        };

        string dataLog = Newtonsoft.Json.JsonConvert.SerializeObject(loginlog);
        AESAlgorithm Aes = new AESAlgorithm();
        string Encryptedlog = Aes.getEncryptedString(dataLog);

        CommonModel postlog = new CommonModel
        {
            Data = Encryptedlog
        };

        string datalog = Newtonsoft.Json.JsonConvert.SerializeObject(postlog);

        using (UnityWebRequest Request = UnityWebRequest.Put(HittingUrl, datalog))
        {
            Request.method = UnityWebRequest.kHttpVerbPOST;
            Request.SetRequestHeader("Content-Type", "application/json");
            Request.SetRequestHeader("Accept", "application/json");
            yield return Request.SendWebRequest();
            if (!Request.isNetworkError && !Request.isHttpError)
            {
                if (Request.responseCode.Equals(200))
                {
                    if (Request.downloadHandler.text != "")
                    {
                        var log = Request.downloadHandler.text.TrimStart('"').TrimEnd('"');
                        string response = Aes.getDecryptedString(log);
                        Debug.Log("Login response " + response);
                        LoginResModel LoginLog = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResModel>(response);
                        PlayerPrefs.SetInt("UID", LoginLog.Id_User);
                        PlayerPrefs.SetInt("OID", LoginLog.ID_ORGANIZATION);
                        PlayerPrefs.SetString("Username", LoginLog.Name);
                        StartCoroutine(ShowPopUp("Logged in Successfully!!!"));
                        int avatarid = Convert.ToInt32(LoginLog.Id_Avatar != null ? LoginLog.Id_Avatar : 0);
                        //int avatarid = 0;//Convert.ToInt32(LoginLog.Id_Avatar != null ? LoginLog.Id_Avatar : 0);
                        DateTime todate = DateTime.Now;
                        PlayerPrefs.SetString("LastLogin", todate.ToString());
                        Username.text = password.text = "";
                        yield return new WaitForSeconds(3f);
                        StartCoroutine(GetuserAvatar());
                        var localLog = dbmanager.Table<ProfileSetup>().FirstOrDefault();
                        if (localLog == null)
                        {
                            ProfileSetup profilelog = new ProfileSetup
                            {
                                UserId = LoginLog.Id_User,
                                Oid = LoginLog.ID_ORGANIZATION,
                                Username = LoginLog.Name,
                                EmailId = LoginLog.Email,
                                Mobileno = LoginLog.Phone_No,
                                Orgname = LoginLog.Organization_Name,
                                LoginType = 1,
                                AvatarId = avatarid
                            };
                            dbmanager.Insert(profilelog);
                      
                        }
                        else
                        {
                            localLog.UserId = LoginLog.Id_User;
                            localLog.Oid = LoginLog.ID_ORGANIZATION;
                            localLog.Username = LoginLog.Name;
                            localLog.EmailId = LoginLog.Email;
                            localLog.Mobileno = LoginLog.Phone_No;
                            localLog.Orgname = LoginLog.Organization_Name;
                            localLog.LoginType = 1;
                            localLog.AvatarId = avatarid;
                            dbmanager.UpdateTable(localLog);
                    

                        }
                        yield return new WaitForSeconds(3f);
                        AfterLoginTask();
                        
                    }
                }
                else
                {
                    Debug.Log("request error " + Request.downloadHandler.text);
                    string msg = "Invaild email id or password!";
                    StartCoroutine(ShowPopUp(msg));
                }
            }
            else
            {
                Debug.Log("request error " + Request.downloadHandler.text);
                string msg = "Invaild email id or password!";
                StartCoroutine(ShowPopUp(msg));
            }
        }
    }
    IEnumerator GetuserAvatar()
    {
        List<string> Urls = new List<string>();
        List<int> Ids = new List<int>();
        string AvatarDir = "AvatarFolder";
        string HittingUrl = $"{MainUrls.BaseUrl}{MainUrls.GetavatarPicApi}?OrgID={PlayerPrefs.GetInt("OID")}";
        WWW AvatarLog = new WWW(HittingUrl);
        yield return AvatarLog;
        if (AvatarLog.text != null)
        {
            string log = AvatarLog.text.TrimStart('"').TrimEnd('"');
            string Datalog = aeslog.getDecryptedString(log);
            List<AvatarLogModel> LogModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AvatarLogModel>>(Datalog);
            LogModel.ForEach(x =>
            {
                Urls.Add(x.url);
                Ids.Add(x.Id_Avatar);

            });
            for (int a = 0; a < Urls.Count; a++)
            {
                yield return new WaitForSeconds(0.2f);
                StartCoroutine(GetTexture(Ids[a].ToString(), Urls[a], AvatarDir, Urls));
                Debug.Log("url " + Urls[a]);
            }
            yield return new WaitForSeconds(1f);
           

        }
    }

    IEnumerator GetTexture(string id, string Url, string dirname, List<string> ImageList)
    {
        if (Url != null)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(Url, true);

            yield return www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                try
                {
                    Texture2D texture2d = new Texture2D(1, 1);
                    Sprite sprite = null;

                    if (www.isDone)
                    {
                        if (texture2d.LoadImage(www.downloadHandler.data))
                        {
                            var dirpath = Application.persistentDataPath + "/" + dirname;
                            if (!Directory.Exists(dirpath))
                            {
                                Directory.CreateDirectory(dirpath);
                                if (Directory.Exists(dirpath))
                                {
                                    dirpath = dirpath + "/" + id + ".png";
                                    byte[] bytes = texture2d.EncodeToPNG();
                                    File.WriteAllBytes(dirpath, bytes);
                                    Debug.Log("File saved " + dirpath);
                                }
                            }
                            else
                            {
                                dirpath = dirpath + "/" + id.ToString() + ".png";
                                byte[] bytes = texture2d.EncodeToPNG();
                                File.WriteAllBytes(dirpath, bytes);
                                Debug.Log("File saved " + dirpath);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("running");
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }
    }

    void AfterLoginTask()
    {
        var profileLog = dbmanager.Table<ProfileSetup>().FirstOrDefault();
        if(profileLog.AvatarId == 0)
        {
            LoginPage.SetActive(false);
            AvatarSelectionPage.SetActive(true);
        }
        else
        {
            LoginPage.SetActive(false);
            GamestoryPage.SetActive(true);
        }
     
    }
}
