using Mirror;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    [Serializable]
    public class TankManager: NetworkBehaviour
    {
        // This class is to manage various settings on a tank
        // It works with the GameManager class to control how the tanks behave
        // and whether or not players have control of their tank in the 
        // different phases of the game

        [SyncVar(hook = "CambioDeColor")]
        public Color m_PlayerColor;                             // This is the color this tank will be tinted

        [SyncVar]
        public int m_Wins;                    // The number of wins this player has so far

        [SyncVar(hook = "CambioDeNombre")]
        public string nickname;

        public string m_ColoredPlayerText
        {
            get
            {
                return "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + nickname + "</color>";
            }
        }

        private Text Label;

        private TankMovement m_Movement;                        // Reference to tank's movement script, used to disable and enable control
        private TankShooting m_Shooting;                        // Reference to tank's shooting script, used to disable and enable control
        private GameObject m_CanvasGameObject;                  // Used to disable the world space UI during the Starting and Ending phases of each round



        void Start ()
        {
            // Get references to the components
            m_Movement = GetComponent<TankMovement> ();
            m_Shooting = GetComponent<TankShooting> ();
            m_CanvasGameObject = GetComponentInChildren<Canvas> ().gameObject;

            Label = gameObject.transform.GetChild(5).GetChild(0).GetComponent<Text>();


            // Get all of the renderers of the tank
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer> ();

            FindObjectOfType<GameManager>().m_Tanks.Add(this);
        }

        public override void OnStartLocalPlayer()
        {
            setColor();
            gameObject.name = "LocalPlayer";

        }


        // Used during the phases of the game where the player shouldn't be able to control their tank
        public void DisableControl ()
        {
            if (isLocalPlayer)
            {
                m_Movement.enabled = false;
                m_Shooting.enabled = false;

                m_CanvasGameObject.SetActive(false);
            }

        }


        // Used during the phases of the game where the player should be able to control their tank
        public void EnableControl ()
        {
            if (isLocalPlayer)
            {
                m_Movement.enabled = true;
                m_Shooting.enabled = true;

                m_CanvasGameObject.SetActive(true);
            }

        }

        void OnDestroy()
        {
            var manager = FindObjectOfType<GameManager>();
            if(manager != null) manager.m_Tanks.Remove(this);

            var camera = FindObjectOfType<CameraControl>();
            if(camera != null) camera.TankList.Remove(this.transform);

        }

        [Command]
        public void CmdCambiarNombreJugador(string nick)
        {
            nickname = nick;
        }

        private void CambioDeNombre(string oldNick, string newNick)
        {
            Label.text = newNick;
        }

        [Command]
        public void CmdCambiarColorJugador(Color newColor)
        {
            m_PlayerColor = newColor;
        }

        private void CambioDeColor(Color oldColor, Color newColor)
        {
            setColor();
        }

        void setColor()
        {
            foreach (MeshRenderer child in GetComponentsInChildren<MeshRenderer>())
            {
                child.material.color = m_PlayerColor;
            }
        }

    }
}