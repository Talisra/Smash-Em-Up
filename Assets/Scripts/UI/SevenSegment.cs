using UnityEngine.UI;
using UnityEngine;

public class SevenSegment : MonoBehaviour
{
    public Text[] segments; // The segments must be arranged from small to high!
    public int number;
    
    public void SetNumber(int value)
    {
        number = value;
        if (value > Mathf.Pow(10, segments.Length))
        {
            Debug.LogWarning("The number " + value + " is too large for seven segment with " + segments.Length + " components to work properly!");
            return;
        }
        for(int i=0; i<segments.Length; i++)
        {
            segments[i].text = (value % 10).ToString();
            value /= 10;
        }
    }


}
