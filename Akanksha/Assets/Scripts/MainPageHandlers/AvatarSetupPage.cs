using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleSQL;
using m2ostnextservice.Models;
using System.Linq;

public class AvatarSetupPage : MonoBehaviour
{
    public List<Image> Avatars;
    public SimpleSQLManager dbmanager;
    private List<Sprite> Avatarsprites = new List<Sprite>();
    private AESAlgorithm aes;
    public HomePageScript HomepageTask;
    public List<GameObject> AvatarFrames;
    public Sprite HighlightedFrame, NormalFrame;
    public GameObject FinishPage,AvatarPage,userMainPage;
    private int AvatarID=0;
    public Text UserName;
    public GameObject GamestoryPage;
    void Start()
    {
        
    }

    private void OnEnable()
    {
        HomepageTask.Step1.color = HomepageTask.Normalcolor;
        HomepageTask.Step2.color = HomepageTask.Selected;
        UserName.text = PlayerPrefs.GetString("Username");
        AvatarPage.SetActive(true);
        FinishPage.SetActive(false);
        aes = new AESAlgorithm();
        StartCoroutine(AvatarSelection());
    }

    public void GenderSelection(GameObject Page)
    {
        AvatarFrames.ForEach(x =>
        {
            x.GetComponent<Image>().sprite = x.name.Equals(Page.name, System.StringComparison.OrdinalIgnoreCase) ? HighlightedFrame : NormalFrame;
        });
        string id = Page.transform.GetChild(0).gameObject.GetComponent<Image>().sprite.name;
        AvatarID = int.Parse(id);

    }

    IEnumerator AvatarSelection()
    {
        yield return new WaitForSeconds(0.05f);
        string path = Application.persistentDataPath + "/AvatarFolder";
        foreach (string file in Directory.GetFiles(path))
        {
            string spritename = Path.GetFileNameWithoutExtension(file);
            Sprite avatar = GetAvatarSprite(file, spritename);
            Avatarsprites.Add(avatar);
        }

        for(int a=0;a< Avatarsprites.Count; a++)
        {
            Avatars[a].sprite = Avatarsprites[a];
        }
    }

    private Sprite GetAvatarSprite(string path, string spritename)
    {
        if (path.Length > 0)
        {
            byte[] imagedata = File.ReadAllBytes(path);
            Texture2D texture2d = new Texture2D(1, 1);
            Sprite sprite;
            texture2d.LoadImage(imagedata);
            sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f));
            sprite.name = spritename;
            return sprite;
        }
        return null;
    }

    public void SelectAvatar()
    {
        if(AvatarID == 0)
        {
            StartCoroutine(HomepageTask.ShowPopUp("Please select a avatar!"));
        }
        else
        {
            StartCoroutine(SaveProfile());
        }
    }

    IEnumerator SaveProfile()
    {
        
        yield return new WaitForSeconds(0.1f);
        string HittingUrl = $"{MainUrls.BaseUrl}{MainUrls.SetAvatarInfo}";
        ProfilePostModel postlog = new ProfilePostModel
        {
            Id_User = PlayerPrefs.GetInt("UID"),
            Name ="",
            Email="",
            Phone_No="",
            Password ="",
            IsActive = "",
            ID_ORGANIZATION = PlayerPrefs.GetInt("OID"),
            Id_Avatar = AvatarID
        };

        string Logdata = Newtonsoft.Json.JsonConvert.SerializeObject(postlog);
        string EncryptedLog = aes.getEncryptedString(Logdata);
        CommonModel commonLog = new CommonModel
        {
            Data = EncryptedLog
        };
        string FinalLog = Newtonsoft.Json.JsonConvert.SerializeObject(commonLog);
        using (UnityWebRequest Request = UnityWebRequest.Put(HittingUrl, FinalLog))
        {
            Request.method = UnityWebRequest.kHttpVerbPOST;
            Request.SetRequestHeader("Content-Type", "application/json");
            Request.SetRequestHeader("Accept", "application/json");
            yield return Request.SendWebRequest();
            if (!Request.isNetworkError && !Request.isHttpError)
            {
                if (Request.responseCode.Equals(200))
                {
                    if (Request.downloadHandler.text != null)
                    {
                        var log = Request.downloadHandler.text.TrimStart('"').TrimEnd('"');
                        string response = aes.getDecryptedString(log);
                        LoginResModel LoginLog = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResModel>(response);
                        string msg = "Profile Updated Successfully!!!";
                        StartCoroutine(HomepageTask.ShowPopUp(msg));
                        int avatarid = Convert.ToInt32(LoginLog.Id_Avatar != null ? LoginLog.Id_Avatar : 0);
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
                        yield return new WaitForSeconds(3.5f);
                        HomepageTask.Step1.gameObject.SetActive(false);
                        HomepageTask.Step2.gameObject.SetActive(false);
                        AvatarPage.SetActive(false);
                        FinishPage.SetActive(true);
                       
                       // StartCoroutine(AfterProfileDone());
                    }
                    else
                    {
                        string msg = "Something Went Wrong Please try again!";
                        StartCoroutine(HomepageTask.ShowPopUp(msg));
                    }
                }
                else
                {
                    string msg = "Something Went Wrong Please try again!";
                    StartCoroutine(HomepageTask.ShowPopUp(msg));
                }
            }
            else
            {
                string msg = "Check Your internet connection and try again!";
                StartCoroutine(HomepageTask.ShowPopUp(msg));
            }
        }
    }


    public void OpenStoryPAge()
    {
        FinishPage.SetActive(false);
        GamestoryPage.SetActive(true);
        userMainPage.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
