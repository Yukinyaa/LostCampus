using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatusBar : MonoBehaviour
{
    private float frameWidth;
    public GameObject Bar,Text;
    private RectTransform BarSize;
    // Start is called before the first frame update
    void Awake()
    {
        this.frameWidth = gameObject.GetComponent<RectTransform>().rect.width;
        this.BarSize = this.Bar.GetComponent<RectTransform>();
    }

    void Set(float[] point)
    {
        float now = point[0];
        float max = point[1];
        this.BarSize.offsetMax =new Vector2( this.frameWidth*now/max- this.frameWidth, 0);
        this.Text.GetComponent<Text>().text = (int)now+"/"+ (int)max;
    }
}
