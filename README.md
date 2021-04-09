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

Color Jugadores
