using UnityEngine;
using TMPro;
using HarnessTestBed.API;

namespace HarnessTestBed.UI
{
    public class HUDPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI gemsText;

        private void Start()
        {
            RefreshWallet();
        }

        public void RefreshWallet()
        {
            if (GameAPI.Instance == null) return;

            StartCoroutine(GameAPI.Instance.FetchWallet(wallet =>
            {
                if (wallet != null)
                {
                    if (goldText != null) goldText.text = $"Gold: {wallet.gold}";
                    if (gemsText != null) gemsText.text = $"Gems: {wallet.gems}";
                }
            }));
        }

        public void SetPlayerName(string name)
        {
            if (playerNameText != null)
                playerNameText.text = name;
        }
    }
}
