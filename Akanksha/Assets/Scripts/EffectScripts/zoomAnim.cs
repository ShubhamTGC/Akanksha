using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoomAnim : MonoBehaviour
{
    // Start is called before the first frame update
    public float delaytime;
    public float time;
    void Start()
    {
        StartCoroutine(zoomtask());
    }

    private void OnEnable()
    {
        StartCoroutine(zoomtask());
    }

    IEnumerator zoomtask()
    {
        yield return new WaitForSeconds(delaytime);
        iTween.ScaleTo(this.gameObject, Vector3.one, time);

    }

}
