using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class FPSText : MonoBehaviour {
    
    float fps;
    public TextMeshProUGUI fpsText;
    private void Awake()
    {
    }
    // Use this for initialization
    void Start () {
        StartCoroutine(CalcFPS());
	}
	
	// Update is called once per frame
	IEnumerator CalcFPS() {
        while (true)
        {
            fps = 1.0f / Time.unscaledDeltaTime;
            fpsText.text = "FPS : " + ((int)fps);
            yield return new WaitForSecondsRealtime(1);
        }
    }
}
