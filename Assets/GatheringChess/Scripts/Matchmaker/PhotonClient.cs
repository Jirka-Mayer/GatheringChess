using System;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GatheringChess.Matchmaker
{
    /// <summary>
    /// Contains photon interactions for the matchmaker scene
    /// </summary>
    public class PhotonClient : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// Promise on connection to photon master server
        /// </summary>
        private TaskCompletionSource<bool> photonConnectionPromise;

        /// <summary>
        /// Connects to photon and returns true on success
        /// </summary>
        public Task<bool> ConnectToPhoton()
        {
            if (photonConnectionPromise != null)
                throw new InvalidOperationException(
                    "Already connecting to photon"
                );
            
            photonConnectionPromise = new TaskCompletionSource<bool>();
            var task = photonConnectionPromise.Task;
            
            // connect to photon master server
            if (PhotonNetwork.IsConnected)
            {
                OnConnectedToMaster();
            }
            else
            {
                // connect to photon master server
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.GameVersion = Application.version;
                PhotonNetwork.ConnectUsingSettings();
            }

            return task;
        }
        
        public override void OnConnectedToMaster()
        {
            if (photonConnectionPromise == null)
                return;
            
            photonConnectionPromise.SetResult(true);
            photonConnectionPromise = null;
        }
        
        public override void OnDisconnected(DisconnectCause cause)
        {
            if (photonConnectionPromise == null)
                return;
            
            photonConnectionPromise.SetResult(false);
            photonConnectionPromise = null;
        }

        private void OnDestroy()
        {
            if (photonConnectionPromise == null)
                return;

            photonConnectionPromise.SetCanceled();
            photonConnectionPromise = null;
        }
    }
}
