using System;
using GatheringChess.Playground;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GatheringChess.Matchmaker
{
    public class MatchmakerController : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// Reference to the matchmaker client
        /// </summary>
        public MatchmakerClient matchmakerClient;

        /// <summary>
        /// Reference to the photon client
        /// </summary>
        public PhotonClient photonClient;
        
        async void Start()
        {
            if (!matchmakerClient)
                throw new ArgumentNullException(nameof(matchmakerClient));
            
            if (!photonClient)
                throw new ArgumentNullException(nameof(photonClient));
            
            // try to connect to photon
            Debug.Log("Connecting to photon...");
            bool photonConnectionOk = await photonClient.ConnectToPhoton();
            Debug.Log("Photon result: " + photonConnectionOk);

            // perform matchmaking
            // TODO: catch network exceptions
            Debug.Log("Matchmaking...");
            MatchEntity match = await matchmakerClient.PerformMatchmaking(
                usePhoton: photonConnectionOk
            );
            Debug.Log("Match: " + match.EntityId);
            
            // join the match
            PlaygroundController.matchToStart = match;
            PhotonNetwork.LoadLevel("Playground");
        }
    }
}
