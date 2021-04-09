using UnityEngine;
using UnityEngine.UI;
using System;
using Mirror;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Complete
{
    public class TankHealth : NetworkBehaviour
    {
        public float m_StartingHealth = 100f;               // The amount of health each tank starts with
        public Slider m_Slider;                             // The slider to represent how much health the tank currently has
        public Image m_FillImage;                           // The image component of the slider
        public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health
        public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health
        public GameObject m_ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies
        public int indexPosition;

        private AudioSource m_ExplosionAudio;               // The audio source to play when the tank explodes
        private ParticleSystem m_ExplosionParticles;        // The particle system the will play when the tank is destroyed

        [SyncVar(hook = "OnChangeHealth")]
        public float m_CurrentHealth;                      // How much health the tank currently has
        private bool m_Dead;                                // Has the tank been reduced beyond zero health yet?
        public CameraControl cameraControl;
        public int thisIndex;

        private NetworkStartPosition[] spawnPoints;

        private void Start()
        {
            if (isLocalPlayer)
            {
                spawnPoints = FindObjectsOfType<NetworkStartPosition>();
            }
        }

        private void Awake()
        {
            // Instantiate the explosion prefab and get a reference to the particle system on it
            m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();

            // Get a reference to the audio source on the instantiated prefab
            m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

            // Disable the prefab so it can be activated when it's required
            m_ExplosionParticles.gameObject.SetActive(false);


            /*GameObject[] startPositions = GameObject.FindGameObjectsWithTag("StartLocation");
            for (int i = 0; i < startPositions.Length; i++)
            {
                NetworkManager.RegisterStartPosition(startPositions[i].transform);
            }*/

            cameraControl = GameObject.Find("CameraRig").GetComponent<CameraControl>();

            


        }


        private void OnEnable()
        {
            cameraControl.TankList.Add(transform);
            thisIndex = cameraControl.TankList.LastIndexOf(transform);
            
            m_Slider.value = m_StartingHealth;
            m_FillImage.color = Color.green;
            m_CurrentHealth = m_StartingHealth;

            m_Dead = false;

        }


        public void TakeDamage(float amount)
        {
            if (!isServer)
            {
                return;
            }
            // Reduce current health by the amount of damage done
            m_CurrentHealth -= amount;

            // Change the UI elements appropriately
            //SetHealthUI();

            // If the current health is at or below zero and it has not yet been registered, call OnDeath
            if (m_CurrentHealth <= 0f && !m_Dead)
            {
                OnDeath();
            }
        }



        private void OnChangeHealth(float oldHealth, float newHealth)
        {
            //m_CurrentHealth = newHealth;
            // Set the slider's value appropriately
            m_Slider.value = newHealth;

            // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health
            m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
        }


        private void OnDeath()
        {

            
            // Move the instantiated explosion prefab to the tank's position and turn it on
            m_ExplosionParticles.transform.position = transform.position;
            m_ExplosionParticles.gameObject.SetActive(true);

            // Play the particle system of the tank exploding
            m_ExplosionParticles.Play();

            // Play the tank explosion sound effect
            m_ExplosionAudio.Play();

            m_CurrentHealth = m_StartingHealth;
            m_FillImage.color = Color.green;
            m_Slider.value = m_StartingHealth;
            
            
            RpcRespawn();
            
            
        }

        [ClientRpc]
        void RpcRespawn()
        {
            
            if (isLocalPlayer)
            {
                Vector3 spawnPoint = Vector3.zero;
                if(spawnPoint != null && spawnPoints.Length > 0)
                {
                    spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform.position;
                }
                transform.position = spawnPoint;

            }
            else if (gameObject.tag == "NPC")
            {
                m_Dead = true;
                gameObject.SetActive(false);
                if(cameraControl.TankList[thisIndex].gameObject.tag == "NPC")
                {
                    cameraControl.TankList.RemoveAt(thisIndex);
                }
                
            }
            
        }
        


    }
}