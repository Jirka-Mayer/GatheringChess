using System.Linq;
using Unisave;
using Unisave.Authentication;
using Unisave.Serialization;
using Unisave.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GatheringChess
{
    public class LoginController : MonoBehaviour
    {
        private AccountManager accountManager;
    
        void Start()
        {
            accountManager = AccountManager.GetInstance();
            
            // display the account list
            Debug.Log(
                Serializer
                    .ToJson(accountManager.Accounts.ToList())
                    .ToString()
            );

            // login primary account
            if (accountManager.PrimaryAccount != null)
            {
                LoginAccount(accountManager.PrimaryAccount);
            }
        }

        /// <summary>
        /// Log into an account
        /// </summary>
        private void LoginAccount(Account account)
        {
            string email;

            if (account.isAnonymous)
                email = account.anonymousToken + "@anonymous";
            else
                email = account.email;
        
            Auth.Login(email, "password")
                .Then(() => {
                    SceneManager.LoadScene("MainMenu");
                })
                .Catch(e => {
                    Debug.LogException(e);
                    if (e is LoginFailure f)
                    {
                        Debug.LogError(f.type + " " + f.message);
                    }
                });
        }

        /// <summary>
        /// User wants to play on this device only,
        /// without providing any credentials
        /// </summary>
        public void CreateAnonymousAccount()
        {
            string token = Str.Random(8);
            Auth.Register(token + "@anonymous", "password")
                .Then(() => {
                    var account = new Account {
                        isAnonymous = true,
                        anonymousToken = token
                    };
                    accountManager.AddAccount(account);
                    accountManager.SetPrimaryAccount(account);
                    accountManager.Save();
                
                    LoginAccount(account);
                })
                .Done();
        }

        /// <summary>
        /// DEBUG
        /// Logs into the first remembered account
        /// </summary>
        public void PlayFirstRemembered()
        {
            Account account = accountManager.Accounts.FirstOrDefault();

            if (account == null)
            {
                Debug.LogError("There are no remembered accounts.");
                return;
            }
            
            LoginAccount(account);
        }
    }
}
