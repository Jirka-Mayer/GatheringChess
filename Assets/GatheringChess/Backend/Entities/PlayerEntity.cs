using Unisave;

namespace GatheringChess.Entities
{
    public class PlayerEntity : Entity
    {
        /// <summary>
        /// Name of the player
        /// Not unique, just for displaying purposes
        /// </summary>
        [X] public string Name { get; set; }
    
        /// <summary>
        /// Unique number identifying this player
        /// </summary>
        [X] public int Number { get; set; }
        
        /// <summary>
        /// ID of the selected avatar image
        /// </summary>
        [X] public string AvatarId { get; set; }
        
        /// <summary>
        /// Amount of coins
        /// </summary>
        [X] public int Coins { get; set; }
        
        /// <summary>
        /// Amount of gems
        /// </summary>
        [X] public int Gems { get; set; }
    }
}
