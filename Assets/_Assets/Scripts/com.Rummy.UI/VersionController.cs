using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class VersionController : MonoBehaviour
{
    private void Start()
    {
        Text text = gameObject.GetComponent<Text>();
        text.text = Application.version;
    }
}
