using m2ostnextservice.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class RegistrationPage : MonoBehaviour
{
   
    public InputField Username, Emailid, Password, ConfirmPassword, mobileNo;
    public GameObject PopupPage;
    public Text Msgbox;
    private AESAlgorithm aes;
    public GameObject Registerpage, Loginpage;
    public Scrollbar Formpage;
    public Button Hidepasswordbtn, HideConfmpasswordbtn;
    public Sprite Closeeye, Openeye;
    void Start()
    {

    }




    void Update()
    {

    }

    void ResetForm()
    {
        Username.text = "";
        Emailid.text = "";
        Password.text = "";
        ConfirmPassword.text = "";
        mobileNo.text = "";
        //orgname.text = "";
        Formpage.value = 1;
    }

    void OnEnable()
    {
        ResetForm();
        aes = new AESAlgorithm();
    }

    public void BackToLogin()
    {
        ResetForm();
        Loginpage.SetActive(true);
        Registerpage.SetActive(false);
    }


    IEnumerator ShowPopUp(string msg)
    {
        PopupPage.SetActive(true);
        Msgbox.text = msg;
        yield return new WaitForSeconds(2.5f);
        iTween.ScaleTo(PopupPage, Vector3.zero, 0.3f);
        yield return new WaitForSeconds(0.35f);
        Msgbox.text = "";
        PopupPage.SetActive(false);
    }

    public void registerYourself()
    {
        if (Username.text != "" && Emailid.text != "" && Password.text != "" && ConfirmPassword.text != "")
        {
            if (Password.text.Equals(ConfirmPassword.text, System.StringComparison.OrdinalIgnoreCase))
            {
                StartCoroutine(RegsiterTask());
            }
            else
            {
                string msg = "Password not matched!";
                StartCoroutine(ShowPopUp(msg));
            }
        }
        else
        {
            string msg = "please fill required details!!!";
            StartCoroutine(ShowPopUp(msg));
        }
    }

    IEnumerator RegsiterTask()
    {
        yield return new WaitForSeconds(0.1f);
        string HittingUrl = $"{MainUrls.BaseUrl}{MainUrls.RegistrationApi}";
        userRegistrationModel Model = new userRegistrationModel
        {
            Name = Username.text,
            Email = Emailid.text,
            Password = aes.getEncryptedString(Password.text),
            Phone_No = mobileNo.text,
            login_type = 1,
        };



        string NormalLog = Newtonsoft.Json.JsonConvert.SerializeObject(Model);
        string encryptedlog = aes.getEncryptedString(NormalLog);
        CommonModel postlog = new CommonModel
        {
            Data = encryptedlog
        };
        string Finallog = Newtonsoft.Json.JsonConvert.SerializeObject(postlog);
        //Debug.Log("encrypted data " + Finallog);
        //Debug.Log("Normal data " + NormalLog);

        using (UnityWebRequest Request = UnityWebRequest.Put(HittingUrl, Finallog))
        {
            Request.method = UnityWebRequest.kHttpVerbPOST;
            Request.SetRequestHeader("Content-Type", "application/json");
            Request.SetRequestHeader("Accept", "application/json");
            yield return Request.SendWebRequest();
            Debug.Log(Request.downloadHandler.text);
            if (!Request.isNetworkError && !Request.isHttpError)
            {
                if (Request.downloadHandler.text != null)
                {
                    if (Request.downloadHandler.text != "")
                    {
                        Debug.Log("msg " + Request.downloadHandler.text);
                        string msg = "User registered Successfully !";
                        StartCoroutine(ShowPopUp(msg));
                        yield return new WaitForSeconds(3.5f);
                        ResetForm();
                        Loginpage.SetActive(true);
                        Registerpage.SetActive(false);
                    }
                }
                else
                {
                    string msg = "Something went wrong! Please try again.";
                    StartCoroutine(ShowPopUp(msg));
                }

            }
            else
            {
                string msg = "Something went wrong! Please try again.";
                StartCoroutine(ShowPopUp(msg));
            }
        }
    }

    public void Showpassword()
    {
        if (Hidepasswordbtn.image.sprite.name.Equals("closeeye", System.StringComparison.OrdinalIgnoreCase))
        {
            Hidepasswordbtn.image.sprite = Openeye;
            Password.inputType = InputField.InputType.Standard;
            Password.ForceLabelUpdate();
        }
        else
        {
            Hidepasswordbtn.image.sprite = Closeeye;
            Password.inputType = InputField.InputType.Password;
            Password.ForceLabelUpdate();
        }
    }
    public void ShowConfrmpassword()
    {
        if (HideConfmpasswordbtn.image.sprite.name.Equals("closeeye", System.StringComparison.OrdinalIgnoreCase))
        {
            HideConfmpasswordbtn.image.sprite = Openeye;
            ConfirmPassword.inputType = InputField.InputType.Standard;
            ConfirmPassword.ForceLabelUpdate();
        }
        else
        {
            HideConfmpasswordbtn.image.sprite = Closeeye;
            ConfirmPassword.inputType = InputField.InputType.Password;
            ConfirmPassword.ForceLabelUpdate();
        }
    }
}
