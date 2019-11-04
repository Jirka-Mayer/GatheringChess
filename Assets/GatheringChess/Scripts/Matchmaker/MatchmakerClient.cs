using System;
using System.Threading.Tasks;
using Unisave;
using Unisave.Components.Matchmaking;
using UnityEngine;

namespace GatheringChess.Matchmaker
{
    public class MatchmakerClient : MonoBehaviourBasicMatchmakerClient
        <MatchmakerFacet, MatchmakerTicket, MatchEntity>
    {
        private TaskCompletionSource<MatchEntity> matchmakingPromise;
        
        public Task<MatchEntity> PerformMatchmaking(bool usePhoton)
        {
            if (matchmakingPromise != null)
                throw new InvalidOperationException(
                    "Already performing matchmaking"
                );
            
            matchmakingPromise = new TaskCompletionSource<MatchEntity>();
            
            StartWaitingForMatch(new MatchmakerTicket {
                UsePhoton = usePhoton,
                ChessSet = ChessSet.CreateDefaultSet()
            });

            return matchmakingPromise.Task;
        }

        protected override void JoinedMatch(MatchEntity match)
        {
            if (matchmakingPromise == null)
                throw new Exception(
                    "Match has been joined but nobody waits for it"
                );
            
            matchmakingPromise.SetResult(match);
            matchmakingPromise = null;
        }
    }
}
