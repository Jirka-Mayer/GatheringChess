using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GatheringChess.MatchResultScene
{
    /// <summary>
    /// Controls the match result scene
    /// </summary>
    public class MatchResultController : MonoBehaviour
    {
        public static MatchResult matchResultToDisplay;
        
        /// <summary>
        /// Reference to the text element
        /// </summary>
        public Text resultText;
        
        void Start()
        {
            if (resultText == null)
                throw new ArgumentNullException();

            if (false && PhotonNetwork.IsConnected)
            {
                // TODO: needs to wait for RPCs to finish
                
                Debug.Log("Disconnected from photon.");
                PhotonNetwork.Disconnect();
            }

            if (matchResultToDisplay != null)
            {
                resultText.text = matchResultToDisplay.FormatMessage();
                matchResultToDisplay = null;
            }
        }

        public void OnOkButtonClick()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
