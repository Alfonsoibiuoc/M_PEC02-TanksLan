using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    public Color[] colors;
    Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<Dropdown>();

        List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
        for (int i = 0; i < colors.Length; i++)
        {

            var texture = new Texture2D(1, 1); 
            texture.SetPixel(0, 0, colors[i]);
            texture.Apply(); 
            var item = new Dropdown.OptionData(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0)));
            dropdown.options.Add(item);

        }
        dropdown.captionImage.sprite = dropdown.options[0].image;
        dropdown.captionImage.enabled = true;
    }


    public Color GetSelectedColor()
    {
        int index = dropdown.value;
        return colors[index];
    }

    public void SetSelectedColor(Color color)
    {
        int index = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            if(color == colors[i])
            {
                index = i;
            }
        }
        dropdown.value = index;
    }
}
