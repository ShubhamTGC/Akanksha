using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStoryPlayHandler : MonoBehaviour
{
    public int StoryPageCount=7;

    [Header("Down Menu handler")]
    public List<Image> PageIndications;
    public List<string> ConditionMsg;
    public List<Sprite> ConditionScenes;
    public Image SceneView;
    public Text MessageBox;
    private int PageCounter = 0;
    public Color Selected, Normal;
    public GameObject ExtraInfoObject;
    public Button NextSceneBtn, previousceneBtn;
    
    void Start()
    {
       
    }

    private void OnEnable()
    {
        SceneView.sprite = ConditionScenes[PageCounter];
        MessageBox.text = ConditionMsg[PageCounter];
        PageIndicationTask(PageCounter);


    }

    void PageIndicationTask(int counter)
    {
        for (int a = 0; a < PageIndications.Count; a++)
        {
            if (PageCounter == a)
            {
                PageIndications[a].color = Selected;
            }
            else
            {
                PageIndications[a].color = Normal;
            }
        }
        
    }

    IEnumerator SceneChanger(Sprite Scene)
    {
        NextSceneBtn.interactable = false;
        previousceneBtn.interactable = false;
        float value = SceneView.color.a;
        while(value > 0.5f)
        {
            value -= 0.1f;
            SceneView.color = new Color(1f, 1f, 1f, value);
            yield return new WaitForSeconds(0.05f);

        }
        SceneView.sprite = Scene;
        ExtraInfoObject.SetActive(Scene.name.Equals("Page4", System.StringComparison.OrdinalIgnoreCase) ? true : false);
        while (value < 1f)
        {
            value += 0.1f;
            SceneView.color = new Color(1f, 1f, 1f, value);
            yield return new WaitForSeconds(0.05f);
        }
        NextSceneBtn.interactable = true;
        previousceneBtn.interactable = true;

    }

    public void NextScene()
    {
        if(PageCounter < StoryPageCount-1)
        {
            PageCounter++;
            StartCoroutine(SceneChanger(ConditionScenes[PageCounter]));
            //SceneView.sprite = ConditionScenes[PageCounter];
            MessageBox.text = ConditionMsg[PageCounter];
            PageIndicationTask(PageCounter);
        }
    }

    public void PreviousScene()
    {
        if(PageCounter > 0)
        {
            PageCounter--;
            StartCoroutine(SceneChanger(ConditionScenes[PageCounter]));
            //SceneView.sprite = ConditionScenes[PageCounter];
            MessageBox.text = ConditionMsg[PageCounter];
            PageIndicationTask(PageCounter);
        }
    }

    
    void Update()
    {
        
    }

    
}
