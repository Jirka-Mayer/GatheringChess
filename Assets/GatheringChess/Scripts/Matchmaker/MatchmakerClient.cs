using Unisave;
using Unisave.Components.Matchmaking;
using UnityEngine;

namespace GatheringChess.Matchmaker
{
    public class MatchmakerClient : MonoBehaviourBasicMatchmakerClient
        <MatchmakerFacet, MatchmakerTicket, MatchEntity>
    {
        void Start()
        {
            StartWaitingForMatch(new MatchmakerTicket {
                //
            });
        }

        protected override void JoinedMatch(MatchEntity match)
        {
            Debug.Log("Match has been joined: " + match.ToJson());
        }
    }
}
