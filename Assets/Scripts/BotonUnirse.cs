using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotonUnirse : MonoBehaviour
{
    LobbyMenu lm;

    private void Start()
    {
        lm = GameObject.Find("NetworkManager").GetComponent<LobbyMenu>();
    }
    public void pulsarBotonUnirse()
    {
        lm.serverIP = GetComponentInChildren<Text>().text;  //Obtenemos la Ip del servidor
        lm.JoinGame();  //Lanzamos la función para unirnos a la partida
    }


}
