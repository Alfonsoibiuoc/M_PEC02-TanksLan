using UnityEngine;
using Mirror;
using System.Collections.Generic;
using Mirror.Discovery;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(NetworkDiscovery))]
public class LobbyMenu : MonoBehaviour {

    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
    public GameObject buttonPrefab;         //Boton que instanciamos cuando encuentra un servidor
    public GameObject listaServidores;      //Scroll view donde listamos los botones.
    public int segundosBusqueda = 5;        //Intervalo de comprobación de servidores disponibles.
    private NetworkManager manager;
    public string serverIP = "localhost";
    public NetworkDiscovery networkDiscovery;
    List<string> ServidoresListados;        //lista de los servidores que hemos encontrado.
    public GameObject BotonBuscar;
    public GameObject TextoBuscando;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (networkDiscovery == null)
        {
            networkDiscovery = GetComponent<NetworkDiscovery>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
            UnityEditor.Undo.RecordObjects(new Object[] { this, networkDiscovery }, "Set NetworkDiscovery");
        }
    }
#endif

    void Awake(){
        manager = FindObjectOfType<NetworkManager>();
        ServidoresListados = new List<string>();
    }

    //Iniciar Servidor******************************************
    public void RunServer() {
        if (!NetworkClient.isConnected && !NetworkServer.active) {
            if (!NetworkClient.active) {
                manager.StartServer();
                networkDiscovery.AdvertiseServer();
            }
        }
        AddressData();
    }

    //Iniciar Partida*******************************************
    public void CreateGame() {
        if (!NetworkClient.isConnected && !NetworkServer.active) {
            if (!NetworkClient.active) {
                manager.StartHost();
                networkDiscovery.AdvertiseServer();
            }
        }
        AddressData();
    }

    //Buscar Partida********************************************
    public void Discovery()
    {
        BotonBuscar.SetActive(false);
        TextoBuscando.SetActive(true);
        discoveredServers.Clear();
        networkDiscovery.StartDiscovery();
        StartCoroutine(BuscarServidores());
        
    }

    //Buscar Servidores*****************************************
    private IEnumerator BuscarServidores()
    {
        
        while (true)
        {

            foreach (ServerResponse info in discoveredServers.Values)
            {
                if (!ServidoresListados.Contains(info.EndPoint.Address.ToString())) //Si el servidor encontrado no está en la lista.
                {

                    ServidoresListados.Add(info.EndPoint.Address.ToString()); //Lo añadimos a la lista de servidores encontrados.
                    GameObject botonServer = (GameObject)Instantiate(buttonPrefab, listaServidores.transform);  //Instanciamos el botón
                    botonServer.transform.GetChild(0).GetComponent<Text>().text = info.EndPoint.Address.ToString(); //Cambiamos el texto del botón para que muestre la Ip del servidor
                }
            }
            yield return new WaitForSeconds(segundosBusqueda);
        }    
    }

    //Unirse a la partida***************************************
    //Llamamos a esta función desde cada uno de los botones instanciados
    public void JoinGame() {
        if (!NetworkClient.isConnected && !NetworkServer.active) {
            if (!NetworkClient.active) {
                manager.networkAddress = serverIP;
                manager.StartClient();
            }
        }
        AddressData();
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        // Note that you can check the versioning to decide if you can connect to the server or not using this method
        discoveredServers[info.serverId] = info;
    }

    private void AddressData() {
        if (NetworkServer.active) {
            Debug.Log ("Server: active. IP: " + manager.networkAddress + " - Transport: " + Transport.activeTransport);
        }
        else {
            Debug.Log ("Attempted to join server " + serverIP);
        }

        Debug.Log ("Local IP Address: " + GetLocalIPAddress());

        Debug.Log ("//////////////");
    }

    private static string GetLocalIPAddress() {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList) {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                return ip.ToString();
            }
        }

        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
}
