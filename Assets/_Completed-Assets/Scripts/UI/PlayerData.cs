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
        if (!isServerOnly)
        {
            jugadorLocal = GameObject.Find("LocalPlayer").GetComponent<TankManager>();
            if (jugadorLocal) inputNickName.text = jugadorLocal.nickname;

            var prefs = FindObjectOfType<SetClientPreferences>();
            picker.SetSelectedColor(prefs.initialPlayerColor);
        }

        inputNickName.gameObject.SetActive(!isServerOnly);
        picker.gameObject.SetActive(!isServerOnly);
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
