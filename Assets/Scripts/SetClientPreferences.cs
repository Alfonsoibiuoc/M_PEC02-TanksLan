using Complete;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetClientPreferences : MonoBehaviour
{
    public ColorPicker colorPicker;
    public Color initialPlayerColor { get; private set; }

    public void onSelectionChanged()
    {
        initialPlayerColor = colorPicker.GetSelectedColor();
    }
}
