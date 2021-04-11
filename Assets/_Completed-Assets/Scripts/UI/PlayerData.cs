using Complete;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


public class PlayerData : NetworkBehaviour
{
    TankManager jugadorLocal;
    public InputField inputNickName;
    public ColorPicker picker;
    public void Start()
    {
        jugadorLocal = GameObject.Find("LocalPlayer").GetComponent<TankManager>();
        var prefs = FindObjectOfType<SetClientPreferences>();
        inputNickName.text = jugadorLocal.nickname;
        picker.SetSelectedColor(prefs.initialPlayerColor);
    }

    public void CmdCambiarNombre()
    {       

        jugadorLocal.CmdCambiarNombreJugador(inputNickName.text);        
        //jugadorLocal.nickname = inputNickName.text;

    }

    public void CambiarColor()
    {
        Color color = picker.GetSelectedColor();
        if (jugadorLocal is null) jugadorLocal = GameObject.Find("LocalPlayer").GetComponent<TankManager>();
        jugadorLocal.CmdCambiarColorJugador(color);
    }
}
