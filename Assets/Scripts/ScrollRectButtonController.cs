using UnityEngine;
using UnityEngine.UI;

public class ScrollRectButtonController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollAmount = 0.1f; // Adjust this value to control scroll speed

    public void ScrollUp()
    {
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition += scrollAmount;
        }
    }

    public void ScrollDown()
    {
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition -= scrollAmount;
        }
    }

}