using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using HarnessTestBed.Data;

namespace HarnessTestBed.API
{
    [Serializable]
    public class ShopResponse
    {
        public ItemData[] items;
        public int totalCount;
    }

    [Serializable]
    public class PurchaseResponse
    {
        public bool success;
        public string message;
        public int remainingGold;
    }

    [Serializable]
    public class WalletResponse
    {
        public int gold;
        public int gems;
    }

    public class GameAPI : MonoBehaviour
    {
        public static GameAPI Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public IEnumerator FetchShopItems(Action<ShopResponse> onComplete)
        {
            var request = UnityWebRequest.Get($"{APIConfig.BASE_URL}/shop/items");
            yield return request.SendWebRequest();

            var data = JsonConvert.DeserializeObject<ShopResponse>(
                request.downloadHandler.text);
            onComplete?.Invoke(data);
        }

        public IEnumerator PurchaseItem(string itemId, Action<PurchaseResponse> onComplete)
        {
            var body = JsonConvert.SerializeObject(new { itemId = itemId });
            var request = new UnityWebRequest($"{APIConfig.BASE_URL}/shop/purchase", "POST");
            request.uploadHandler = new UploadHandlerRaw(
                System.Text.Encoding.UTF8.GetBytes(body));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            var response = JsonConvert.DeserializeObject<PurchaseResponse>(
                request.downloadHandler.text);
            onComplete?.Invoke(response);
        }

        public IEnumerator FetchWallet(Action<WalletResponse> onComplete)
        {
            var request = UnityWebRequest.Get($"{APIConfig.BASE_URL}/wallet");
            yield return request.SendWebRequest();

            var wallet = JsonConvert.DeserializeObject<WalletResponse>(
                request.downloadHandler.text);
            onComplete?.Invoke(wallet);
        }
    }
}
