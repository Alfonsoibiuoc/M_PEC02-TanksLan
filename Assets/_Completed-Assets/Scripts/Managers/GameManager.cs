using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Complete
{
    public class GameManager : NetworkBehaviour
    {
        public int m_NumRoundsToWin = 5;            // The number of rounds a single player has to win to win the game
        public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases
        public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases
        public CameraControl m_CameraControl;       // Reference to the CameraControl script for control during different phases
        public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc
        public List<TankManager> m_Tanks = new List<TankManager>();               // A collection of managers for enabling and disabling different aspects of the tanks

        [SyncVar]
        private int m_RoundNumber;                  // Which round the game is currently on
        private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts
        private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends
        [SyncVar]
        private int m_RoundWinner;          // Index of the winner of the current round.  Used to make an announcement of who won
        [SyncVar]
        private int m_GameWinner = -1;           // Reference to the winner of the game.  Used to make an announcement of who won


        private void Start()
        {
            // Create the delays so they only have to be made once
            m_StartWait = new WaitForSeconds (m_StartDelay);
            m_EndWait = new WaitForSeconds (m_EndDelay);


            if (isServer)
            {
                // Once the tanks have been created and the camera is using them as targets, start the game
                StartCoroutine(ServerGameLoop());
            }
            else
            {
                ResetAllTanks();
                m_MessageText.text = string.Empty;
            }
        }
        

        // This is called from start and will run each phase of the game one after another
        private IEnumerator ServerGameLoop()
        {
            // Start off by running the 'RoundStarting' coroutine but don't return until it's finished
            yield return StartCoroutine (ServerRoundStarting());

            // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished
            yield return StartCoroutine (ServerRoundPlaying());

            // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished
            yield return StartCoroutine (ServerRoundEnding());


            // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found
            if (m_GameWinner == -1)

            {
                // If there isn't a winner yet, restart this coroutine so the loop continues
                // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end
                StartCoroutine (ServerGameLoop());
            }
            else
            {
                NetworkServer.Shutdown();
                SceneManager.LoadScene(0);
            }

        }


        private IEnumerator ServerRoundStarting()
        {

            // Increment the round number and display text showing the players what round it is
            m_RoundNumber++;
            ClientRoundStarting(m_RoundNumber);
            yield return m_StartWait;
        }

        private IEnumerator ServerRoundPlaying()
        {
            ClientRoundPlaying();
            // While there is not one tank left...
            while (!OneTankLeft())
            {
                // ... return on the next frame
                yield return null;
            }
        }

        private IEnumerator ServerRoundEnding()
        {
            // Clear the winner from the previous round
            m_RoundWinner = -1;

            // See if there is a winner now the round is over
            m_RoundWinner = GetRoundWinner();

            // If there is a winner, increment their score
            if (m_RoundWinner != -1)
            {
                m_Tanks[m_RoundWinner].m_Wins++;
            }

            m_GameWinner = GetGameWinner();

            yield return null;
            ClientRoundEnding(m_RoundWinner);

             yield return m_EndWait;
        }

        [ClientRpc]
        private void ClientRoundStarting(int round)
        {
            // As soon as the round starts reset the tanks and make sure they can't move
            ResetAllTanks();
            DisableTankControl();

            //// Snap the camera's zoom and position to something appropriate for the reset tanks
            //m_CameraControl.SetStartPositionAndSize();

            m_MessageText.text = "ROUND " + round;

        }

        [ClientRpc]
        private void ClientRoundPlaying()
        {
            // As soon as the round begins playing let the players control the tanks
            EnableTankControl();

            // Clear the text from the screen
            m_MessageText.text = string.Empty;
        }

        [ClientRpc]
        private void ClientRoundEnding(int winner)
        {
            // Stop tanks from moving
            DisableTankControl();

            // Get a message based on the scores and whether or not there is a game winner and display it
            string message = EndMessage(winner);
            m_MessageText.text = message;

        }



        // This is used to check if there is one or fewer tanks remaining and thus the round should end
        private bool OneTankLeft()
        {
            if(m_Tanks.Count > 1)
            {
                // Start the count of tanks left at zero.
                int numTanksLeft = 0;

                // Go through all the tanks...
                for (int i = 0; i < m_Tanks.Count; i++)
                {
                    // ... and if they are active, increment the counter.
                    if (m_Tanks[i].gameObject.activeSelf)
                        numTanksLeft++;
                }

                // If there are one or fewer tanks remaining return true, otherwise return false.
                return numTanksLeft <= 1;
            }
            else
            {
                // Skip all rounds logic
                return false;
            }

        }
        
        
        // This function is to find out if there is a winner of the round
        // This function is called with the assumption that 1 or fewer tanks are currently active
        private int GetRoundWinner()
        {
            // Go through all the tanks...
            for (int i = 0; i < m_Tanks.Count; i++)
            {
                // ... and if one of them is active, it is the winner so return it
                if (m_Tanks[i].gameObject.activeSelf)
                {
                    return i;
                }
            }

            // If none of the tanks are active it is a draw so return null
            return -1;
        }


        // This function is to find out if there is a winner of the game
        private int GetGameWinner()
        {
            // Go through all the tanks...
            for (int i = 0; i < m_Tanks.Count; i++)
            {
                // ... and if one of them has enough rounds to win the game, return it
                if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                {
                    return i;
                }
            }

            // If no tanks have enough rounds to win, return null
            return -1;
        }


        // Returns a string message to display at the end of each round
        private string EndMessage(int winner)
        {
            // By default when a round ends there are no winners so the default end message is a draw
            string message = "DRAW!";

            // If there is a winner then change the message to reflect that
            if (winner != -1)
                message = m_Tanks[winner].m_ColoredPlayerText + " WINS THE ROUND!";

            // Add some line breaks after the initial message
            message += "\n\n\n\n";

            // Go through all the tanks and add each of their scores to the message
            for (int i = 0; i < m_Tanks.Count; i++)
            {
                message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
            }

            // If there is a game winner, change the entire message to reflect that
            if (m_GameWinner != -1)
                message = m_Tanks[m_GameWinner].m_ColoredPlayerText + " WINS THE GAME!";

            return message;
        }


        // This function is used to turn all the tanks back on and reset their positions and properties
        private void ResetAllTanks()
        {
            for (int i = 0; i < m_Tanks.Count; i++)
            {
                m_Tanks[i].gameObject.SetActive(false);
                m_Tanks[i].gameObject.SetActive(true);
            }
        }


        private void EnableTankControl()
        {
            for (int i = 0; i < m_Tanks.Count; i++)
            {
                m_Tanks[i].EnableControl();
            }
        }


        private void DisableTankControl()
        {
            for (int i = 0; i < m_Tanks.Count; i++)
            {
                m_Tanks[i].DisableControl();
            }
        }

    }
}