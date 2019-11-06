using System;
using System.Threading.Tasks;
using GatheringChess.Matchmaker;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GatheringChess.Playground
{
    public class PhotonOpponent : MonoBehaviourPunCallbacks, IOpponent
    {
        /// <summary>
        /// View ID that is used for communication between
        /// the two PhotonOpponent instances
        /// </summary>
        private const int PhotonViewId = 8; // just a number between 1 and 999
        
        public event Action<ChessMove> OnMoveFinish;
        public event Action OnGiveUp;

        /// <summary>
        /// Any long running tasks should be killed
        /// </summary>
        private bool kill;

        /// <summary>
        /// Name of the photon room to join
        /// </summary>
        private string roomName;

        // promises
        private TaskCompletionSource<bool> roomJoiningPromise;

        /// <summary>
        /// Creates new instance of the photon opponent
        /// (this class lives in the scene because it uses a photon view)
        /// </summary>
        public static PhotonOpponent CreateInstance(string roomName)
        {
            GameObject go = new GameObject(nameof(PhotonClient));
            
            var photonView = go.AddComponent<PhotonView>();
            photonView.ViewID = PhotonViewId;
            
            var photonOpponent = go.AddComponent<PhotonOpponent>();
            photonOpponent.Initialize(roomName);
            
            return photonOpponent;
        }

        /// <summary>
        /// Called right after instantiation (before start even)
        /// Acts as a constructor
        /// </summary>
        private void Initialize(string roomName)
        {
            if (!PhotonNetwork.IsConnected)
                throw new InvalidOperationException();
            
            if (PhotonNetwork.InRoom)
                throw new InvalidOperationException();
            
            this.roomName = roomName
                ?? throw new ArgumentNullException(nameof(roomName));
        }
        
        public async Task WaitForReady()
        {
            await JoinRoom();
            
            while (PhotonNetwork.PlayerList.Length < 2)
            {
                if (kill)
                    throw new TaskCanceledException();
                
                await Task.Delay(100); // ms
            }
        }

        private Task JoinRoom()
        {
            if (roomJoiningPromise != null)
                throw new InvalidOperationException();
            
            roomJoiningPromise = new TaskCompletionSource<bool>();
            Task task = roomJoiningPromise.Task;
            
            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions {
                MaxPlayers = 2
            }, null, null);

            return task;
        }

        public void OurMoveWasFinished(ChessMove ourMove)
        {
            photonView.RPC(
                nameof(OtherSideFinishedMove),
                RpcTarget.Others,
                ourMove.from.x, ourMove.from.y,
                ourMove.to.x, ourMove.to.y
            );
        }

        [PunRPC]
        public void OtherSideFinishedMove(int fromX, int fromY, int toX, int toY)
        {
            OnMoveFinish?.Invoke(new ChessMove() {
                from = new Vector2Int(fromX, fromY),
                to = new Vector2Int(toX, toY)
            });
        }

        public void WeGiveUp()
        {
            photonView.RPC(
                nameof(OtherSideGaveUp),
                RpcTarget.Others
            );
        }

        [PunRPC]
        public void OtherSideGaveUp()
        {
            OnGiveUp?.Invoke();
        }

        private void OnDestroy()
        {
            kill = true;

            OnMoveFinish = null;
            OnGiveUp = null;
            
            roomJoiningPromise?.SetCanceled();
        }

        // === photon callbacks ===
        
        public override void OnJoinedRoom()
        {
            if (roomJoiningPromise != null)
            {
                roomJoiningPromise.SetResult(true);
                roomJoiningPromise = null;
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            if (roomJoiningPromise != null)
            {
                roomJoiningPromise.SetException(new Exception(
                    $"Joining room failed: ({returnCode}) {message}"
                ));
                roomJoiningPromise = null;
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            OnGiveUp?.Invoke();
        }
    }
}