using Unisave;

namespace GatheringChess
{
    /// <summary>
    /// Represents a player account remembered on this device
    /// </summary>
    public class Account
    {
        // NOTE: implementation is temporary,
        // will change with new unisave features

        public bool isAnonymous;
        public string email;
        public string anonymousToken;
    }
}