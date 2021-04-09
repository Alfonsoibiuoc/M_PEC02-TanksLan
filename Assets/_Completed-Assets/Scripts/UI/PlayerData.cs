using Complete;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


public class PlayerData : NetworkBehaviour
{
    TankMovement jugadorLocal;
    public InputField inputNickName;

    public void CmdCambiarNombre()
    {
        
        jugadorLocal = GameObject.Find("LocalPlayer").GetComponent<TankMovement>();
        jugadorLocal.CmdCambiarNombreJugador(inputNickName.text);


        //jugadorLocal.nickname = inputNickName.text;

    }
}
