using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextIndicatorView : MonoBehaviour
{
    private Text text;
    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        // インジケータ―の回転
        transform.Rotate(2f, 0, 0);
    }

    public void SetIndicator(bool isActive)
    {
        enabled = isActive;
        text.text = isActive ? "▶" : string.Empty;
    }
}
