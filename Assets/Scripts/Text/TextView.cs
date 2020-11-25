using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextView : MonoBehaviour
{
    private Text text;
    private void Start()
    {
        this.text = GetComponent<Text>();
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }
}
