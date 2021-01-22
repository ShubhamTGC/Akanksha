using m2ostnextservice.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameDataSetup : MonoBehaviour
{
    private AESAlgorithm aes;
    void Start()
    {
        CompleteGameDataCollection();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CompleteGameDataCollection()
    {
        aes = new AESAlgorithm();
        StartCoroutine(GameSetupresponse());
        StartCoroutine(GetTeamDetails());
        StartCoroutine(GetEquiementDeatils());
    }

    IEnumerator GameSetupresponse()
    {
        string HittingUrl = $"{MainUrls.BaseUrl}{MainUrls.GameSetupApi}?OrgID={PlayerPrefs.GetInt("OID")}";
        using (UnityWebRequest Request = UnityWebRequest.Get(HittingUrl))
        {
            Request.method = UnityWebRequest.kHttpVerbGET;
            Request.SetRequestHeader("Content-Type", "application/json");
            Request.SetRequestHeader("Accept", "application/json");
            yield return Request.SendWebRequest();
            if (!Request.isNetworkError && !Request.isHttpError)
            {
                if (Request.responseCode.Equals(200))
                {
                    string response = Request.downloadHandler.text.TrimStart('"').TrimEnd('"');
                    string decrypted = aes.getDecryptedString(response);
                    Debug.Log("game setup " + decrypted);
                }
                else
                {

                }
            }
        }
    }

    IEnumerator GetTeamDetails()
    {
        string HittingUrl = $"{MainUrls.BaseUrl}{MainUrls.TeamPlayerApi}?OrgID={PlayerPrefs.GetInt("OID")}";
        using (UnityWebRequest Request = UnityWebRequest.Get(HittingUrl))
        {
            Request.method = UnityWebRequest.kHttpVerbGET;
            Request.SetRequestHeader("Content-Type", "application/json");
            Request.SetRequestHeader("Accept", "application/json");
            yield return Request.SendWebRequest();
            if (!Request.isNetworkError && !Request.isHttpError)
            {
                if (Request.responseCode.Equals(200))
                {
                    string response = Request.downloadHandler.text.TrimStart('"').TrimEnd('"');
                    string decrypted = aes.getDecryptedString(response);
                    Debug.Log("Team setup " + decrypted);
                }
                else
                {

                }
            }
        }
    }

    IEnumerator GetEquiementDeatils()
    {
        string HittingUrl = $"{MainUrls.BaseUrl}{MainUrls.EquipmentApi}?OrgID={PlayerPrefs.GetInt("OID")}";
        using (UnityWebRequest Request = UnityWebRequest.Get(HittingUrl))
        {
            Request.method = UnityWebRequest.kHttpVerbGET;
            Request.SetRequestHeader("Content-Type", "application/json");
            Request.SetRequestHeader("Accept", "application/json");
            yield return Request.SendWebRequest();
            if (!Request.isNetworkError && !Request.isHttpError)
            {
                if (Request.responseCode.Equals(200))
                {
                    string response = Request.downloadHandler.text.TrimStart('"').TrimEnd('"');
                    string decrypted = aes.getDecryptedString(response);
                    Debug.Log("Equipment setup " + decrypted);
                }
                else
                {

                }
            }
        }
    }


}
