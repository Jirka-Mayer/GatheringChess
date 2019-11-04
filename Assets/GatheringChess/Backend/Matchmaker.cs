using System.Collections.Generic;
using Unisave;
using Unisave.Components.Matchmaking;

namespace GatheringChess
{
    public class MatchEntity : Entity
    {
        // players that play against each other,
        // null means a computer should play
        [X] public UnisavePlayer WhitePlayer { get; set; }
        [X] public UnisavePlayer BlackPlayer { get; set; }
        
        // chess sets that are used
        [X]
        public ChessHalfSet WhitePlayerSet { get; set; }
        [X] public ChessHalfSet BlackPlayerSet { get; set; }
    }

    public class MatchmakerTicket : BasicMatchmakerTicket
    {
        /// <summary>
        /// Client is not connected to photon if this is false,
        /// so he can only play against a computer
        /// </summary>
        public bool UsePhoton { get; set; }
        
        /// <summary>
        /// Chess set that the player wants to play with
        /// </summary>
        public ChessSet ChessSet { get; set; }
    }

    public class MatchmakerFacet : BasicMatchmakerFacet
        <MatchmakerTicket, MatchEntity>
    {
        /// <inheritdoc/>
        protected override void PrepareNewTicket(MatchmakerTicket ticket)
        {
            // Validate that the player owns this chess set:
            //     Well, really, how many people are gonna cheat...
            //     but you could do the validation here if you wanted.
        }
    
        /// <inheritdoc/>
        protected override void CreateMatches(List<MatchmakerTicket> tickets)
        {
            // take all players that cannot use photon and match them against AI
            for (int i = 0; i < tickets.Count; i++)
            {
                if (!tickets[i].UsePhoton)
                {
                    MatchAgainstComputer(tickets[i]);
                    tickets.RemoveAt(i);
                    i--;
                }
            }
            
            // one player waits too long for a match,
            // let them play against a computer
            if (tickets.Count == 1 && tickets[0].WaitingForSeconds > 20)
            {
                MatchAgainstComputer(tickets[0]);
                tickets.RemoveAt(0);
            }
            
            // we have enough players waiting, just pair them
            while (tickets.Count >= 2)
            {
                var selectedTickets = tickets.GetRange(index: 0, count: 2);
                tickets.RemoveRange(index: 0, count: 2);

                var match = new MatchEntity {
                    WhitePlayer = selectedTickets[0].Player,
                    WhitePlayerSet = selectedTickets[0].ChessSet.whiteHalf,
                    
                    BlackPlayer = selectedTickets[1].Player,
                    BlackPlayerSet = selectedTickets[1].ChessSet.blackHalf
                };
            
                SaveAndStartMatch(selectedTickets, match);
            }
        }

        private void MatchAgainstComputer(MatchmakerTicket ticket)
        {
            // TODO: randomize colors and pieces, now the AI is always black
                
            var match = new MatchEntity {
                WhitePlayer = ticket.Player,
                WhitePlayerSet = ticket.ChessSet.whiteHalf,
                    
                BlackPlayer = null,
                BlackPlayerSet = ChessHalfSet.CreateDefaultHalfSet(
                    PieceColor.Black
                )
            };
            
            SaveAndStartMatch(new[] { ticket }, match);
        }
    }
}