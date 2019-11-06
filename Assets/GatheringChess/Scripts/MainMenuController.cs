using System;
using GatheringChess.Entities;
using GatheringChess.Facets;
using Unisave;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GatheringChess
{
    public class MainMenuController : MonoBehaviour
    {
        public Text playerDescription;
        public Text currencies;
        
        private PlayerEntity playerEntity;
        
        async void Start()
        {
            playerEntity = await OnFacet<PlayerFacet>
                .CallAsync<PlayerEntity>("GetPlayerEntity");
            
            if (playerEntity == null)
                throw new Exception("Player entity does not exist?");

            playerDescription.text =
                (playerEntity.Name ?? "anonymous") +
                " #" + playerEntity.Number;

            currencies.text = "<b>Coins:</b> " + playerEntity.Coins + "\n" +
                              "<b>Gems:</b> " + playerEntity.Gems;
        }

        public void OpenMyCollection()
        {
            SceneManager.LoadScene("Collection");
        }

        public void StartGame()
        {
            SceneManager.LoadScene("Matchmaker");
        }

        public void Logout()
        {
            Auth.Logout().Then(() => {
                var manager = AccountManager.GetInstance();
                manager.SetPrimaryAccount(null);
                SceneManager.LoadScene("Login");
            }).Done();
        }
    }
}
