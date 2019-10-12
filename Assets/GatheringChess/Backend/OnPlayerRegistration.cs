using GatheringChess.Entities;
using Unisave;
using Random = System.Random;

namespace GatheringChess
{
    public class OnPlayerRegistration : PlayerRegistrationHook
    {
        /// <summary>
        /// Execution order against other registration hooks
        /// </summary>
        public override int Order => 1;

        /// <summary>
        /// This method gets executed during registration of new players
        /// </summary>
        public override void Run()
        {
            string name = GetArgument<string>("name");
            int number = GeneratePlayerNumber();

            var entity = new PlayerEntity {
                Name = name,
                Number = number,
                AvatarId = null,
                Coins = 200,
                Gems = 10
            };
            entity.Owners.Add(Player);
            entity.Save();

            var collection = PlayerCollectionEntity.CreateDefaultCollection();
            collection.Owners.Add(Player);
            collection.Save();
        }

        private int GeneratePlayerNumber()
        {
            while (true)
            {
                int number = new Random().Next(100_000, 999_999);

                PlayerEntity entity = GetEntity<PlayerEntity>
                    .OfAnyPlayers()
                    .Where("Number", number)
                    .First();

                if (entity == null)
                    return number;
            }
        }
    }
}
