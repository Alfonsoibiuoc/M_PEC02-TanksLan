Taks Lan!

Lobby

Podemos crear un servidor, una partida + servidor o unirnos a una partida.

NetworkDiscovery
He implementado la funcionalidad del networkdiscovery, entonces, en vez de poner la ip del servidor, si iniciamos un servidor en la red, al darle a buscar partida nos saldrá en el listado. Si le damos a cualquier servidor que salga en el listado nos unimos a la partida.
Para esto…
He añadido el componente NetworkDiscovery al NetworkManager. Después, he usado el componente NetworkDiscoveryHUD para comprobar que funcionase y, finalmente, he desactivado ese componente y he adaptado mi interfaz para que haga lo mismo, pero de una forma más vistosa…
También he editado el script LobbyMenu.cs

Partida

El GameManager lo he deshabilitado, ya que en el enunciado decía que no hacía falta nada del tema de rondas y demás…
En el NetworkManager he añadido las dos escenas (Lobby y _Complete-Game), el Telepathy transport, he puesto el máximo de conexiones a 4, el objeto del Player, he activado el “Auto Create Player”, he puesto el spawn method en RoundRobin y he añadido los Spawnable Prefabs (La bala y el tanque NPC)
El funcionamiento para isntanciar las balas, que se vean los tanques en los clientes y en el servidor y dema´s, está hecho siguiendo la práctica que colgó el profesor.

Enemigos NPC

He creado un prefab NPC. Simplemente el mismo tanque de color amarillo. Despues he creado un objeto en la escens (EnemySpawner) y le he añadido un script donde le decimos el número de NPCS que queremos que instancie aleatoriamente por el escenario.
Uso el NetworkServer.Spawn(enemy) para que todos los clientes vean los tanques.

Jugador de color Azul

En el script “TankMovement” del prefab del “Player”, dentro de la función “OnStartLocalPlayer” cambio todos los componenetes meshRenderer de los hijos del tanque al color azul.

Enemigos de color rojo

Los prefabs de los enemigos los he creado de color rojo.

Spawn de los jugadores

He creado 4 puntos en el escenario (SpawnPoint1, 2, 3 y 4) y le he añadido el componente “NetworkStartPosition”. En el NetworkManager ya había marcado el Spawn method a Round Robin.

La cámara

Para que la cámara muestre siempre todos los tanques y se vaya adaptando, he creado una lista dentro del script de la cámara.
Después, en el prefab del tanque, dentro del script “TankHealth” en la función “OnEnable” añadimos el tanque a la lista de la cámara.
Como los tanques de los jugadores se “Respwnean” (Que mal suena esta palabra jajaja) no los elimino de la lista, pero los NPC si. En el mismo Script al final, he creado la función RPCRespawn para hacer esto.

Circulo de salud

Cuando cambia la salud (Siguiendo el ejemplo de la practica que paso el profesor) se actualiza en todos los clientes con un [SyncVar(hook = “OnChangeHealth”)]
En la función “OnChangeHealth” cambiamos la salud y hacemos un Color.Lerp para que vaya cambiando el color poco a poco.

NickName

He añadido un inputField en la escena y un botón. Despues, en el GameManager he añadido un script (PlayerData).
Aquí lo que hacemos es pasarle al script TankMovement del prefab del jugador el nuevo nombre. Y en el script de TankMovement he creado el [SyncVar (hook = “Cambio de nombre”)] para que se actualice en todos los clientes.

Rondas y partida
Lo primero que he hecho ha sido modificar la clase TankManager para que herede de NetworkBehaviour. He pasado lo que había en el método Setup al método Start. He elimidado los campos spwanpoint y number player (ya que ahora nos referiremos al jugador por su nick).
También he eliminado m_Instance porque al ser ahora un MonoBehaviour( bueno, networkBehaviour bastará con hacer referencia a su gameObject.
Añadimos este script al prefab de los jugadores.
Tras esto, he modificado GameManager. EL array de tanques m_Tanks pasa a ser una lista y los tanques los añadimos desde cada TankManager cuando se crea.
Lo primero que he hecho ha sido separar la parte que se ejecuta en el servidor (logica de rondas y elegir al ganador) de la que se ejecuta en los clientes (mostrar los mensajes de quien ha ganado la ronda y reactivar los tanques). La lógica de los clientes la he pasado a llamadas ClientRpc.
He cambiado la funciones que seleccionan el ganador de la ronda y del juego para que en vez de referencias a TankManager sean ints que representan el índice de ese tanque en la lista y he hecho esas variables SyncVar.
Tambíen he marcado como SyncVar el número de ronda y el número de victorias de cada TankManager.  
También he cambiado la función OneTankLeft para que solo funcione si hay más de un tanque jugando. En caso contrario devuelve false y la ronda actual sigue hasta que entre algún tanque más en la partida.

Me he llevado a TankManager el código relacionado con el nickname para poder acceder más facil desde el GameManager y mostrar quien gana (y poque tenía más sentido que estuviese ahí).

Color Jugadores
Para el color de los jugadores, lo primero que he hecho ha sido crear selector de color sencillo mediante un script ColorPicker que con utiliza un array de colores y un elemento Dropdown.
Para cambiar el color de los jugadores durante la partida lo que he hecho es equivalente al cambio de nickname.
En la clase PlayerData he creado un método CambiarColor al cual se llama desde el evento OnChange del dropdown, este a su vez llama a un Command de la clase TankManager que cambia el m_PlayerColor, variable que tiene el tag [SyncVar(hook = "CambioDeColor")]
para replicar el cambio en los clientes.
Para poder elegir color también al inicio de la partida he hecho lo siguiente. He creado otro coloPicker en el lobby y un script muy sencillo llamado SetClientPreferences, que simplemente almacena el color seleccionado en el picker del lobby. Este script lo he añadido al gameObject del NetworkManager.
Luego, en el start del PlayerData, accedo a este scrip para ver que color eligió el jugador al conectarse y cambio el color en consecuencia, que se actualiza en todos los clientes.



