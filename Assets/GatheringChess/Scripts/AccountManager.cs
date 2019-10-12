using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LightJson;
using LightJson.Serialization;
using Unisave.Serialization;
using UnityEngine;

namespace GatheringChess
{
    /// <summary>
    /// Remembers player accounts on this device
    /// </summary>
    public class AccountManager : MonoBehaviour
    {
        /// <summary>
        /// PlayerPrefs key
        /// </summary>
        private static readonly string StorageKey
            = typeof(AccountManager).FullName;

        /// <summary>
        /// Cache for the singleton instance
        /// </summary>
        private static AccountManager instanceCache;
        
        /// <summary>
        /// Returns the singleton instance of the account manager
        /// </summary>
        public static AccountManager GetInstance()
        {
            // get from cache
            if (instanceCache != null)
                return instanceCache;
            
            // find by name
            var foundComponent = GameObject.Find(
                typeof(AccountManager).FullName
            )?.GetComponent<AccountManager>();

            if (foundComponent != null)
            {
                instanceCache = foundComponent;
                return foundComponent;
            }

            // create new
            var managerObject = new GameObject {
                // ReSharper disable once AssignNullToNotNullAttribute
                name = typeof(AccountManager).FullName
            };
            var managerComponent = managerObject.AddComponent<AccountManager>();
            DontDestroyOnLoad(managerObject);
            
            managerComponent.Initialize();

            instanceCache = managerComponent;
            return managerComponent;
        }
        
        
        /// <summary>
        /// The account that is primary and should be logged into
        /// once the game starts
        /// </summary>
        public Account PrimaryAccount { get; private set; }

        /// <summary>
        /// List of remembered accounts
        /// </summary>
        private List<Account> accounts = new List<Account>();

        /// <summary>
        /// Read-only list of remembered accounts
        /// </summary>
        public ReadOnlyCollection<Account> Accounts => accounts.AsReadOnly();

        private void Initialize()
        {
            Load();
        }

        /// <summary>
        /// Load from storage
        /// </summary>
        private void Load()
        {
            JsonObject json = null;
            
            try
            {
                string raw = PlayerPrefs.GetString(StorageKey);
                
                if (string.IsNullOrEmpty(raw))
                    raw = "{}";
                
                json = JsonReader.Parse(raw);
            }
            catch (JsonParseException)
            {
                Debug.LogError(
                    "Failed to parse account manager data:\n" +
                    PlayerPrefs.GetString(StorageKey)
                );
            }
            
            if (json == null)
                json = new JsonObject();

            accounts = Serializer.FromJson<List<Account>>(
               json["accounts"]
            ) ?? new List<Account>();
            
            int primaryAccountIndex = Serializer.FromJson<int>(
                json["primary_account_index"]
            );
            PrimaryAccount =
                primaryAccountIndex >= 0 && primaryAccountIndex < accounts.Count
                ? accounts[primaryAccountIndex]
                : null;
        }

        /// <summary>
        /// Save to storage
        /// </summary>
        public void Save()
        {
            PlayerPrefs.SetString(
                StorageKey,
                new JsonObject()
                    .Add(
                        "accounts",
                        Serializer.ToJson(accounts)
                    )
                    .Add(
                        "primary_account_index",
                        accounts.IndexOf(PrimaryAccount)
                    )
                    .ToString()
            );
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Set an account as primary
        /// </summary>
        public void SetPrimaryAccount(Account account)
        {
            if (!accounts.Contains(account))
                throw new ArgumentException(
                    "Account is not known by the manager"
                );

            PrimaryAccount = account;
        }
        
        /// <summary>
        /// Remember a new account
        /// </summary>
        public void AddAccount(Account account)
        {
            if (accounts.Contains(account))
                return;
            
            accounts.Add(account);
        }
    }
}
