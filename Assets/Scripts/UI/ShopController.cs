using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using HarnessTestBed.Data;
using HarnessTestBed.Registry;
using HarnessTestBed.API;
using HarnessTestBed.Player;

namespace HarnessTestBed.UI
{
    /// <summary>
    /// Handles the shop panel: displays items, handles purchases, updates wallet.
    /// This controller manages UI, business logic, and API communication.
    /// </summary>
    public class ShopController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform itemContainer;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private TextMeshProUGUI goldDisplay;
        [SerializeField] private TextMeshProUGUI selectedItemName;
        [SerializeField] private TextMeshProUGUI selectedItemCost;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button equipButton;

        private ItemData sel;
        private List<ItemData> itms;
        private int gld;
        private bool ld;
        private PlayerController pc;
        private List<string> ownedIds;
        private Dictionary<string, GameObject> spwnedCards;

        private void Start()
        {
            itms = new List<ItemData>();
            ownedIds = new List<string>();
            spwnedCards = new Dictionary<string, GameObject>();

            if (buyButton != null)
                buyButton.onClick.AddListener(OnBuyClicked);
            if (equipButton != null)
                equipButton.onClick.AddListener(OnEquipClicked);

            LoadShopItems();
        }

        private void Update()
        {
            UpdateSelectedItemDisplay();
        }

        private void UpdateSelectedItemDisplay()
        {
            if (sel == null) return;

            var mgr = GetComponent<ItemRegistry>();
            if (mgr != null)
            {
                var freshData = mgr.GetItem(sel.id);
                if (freshData != null)
                {
                    selectedItemName.text = freshData.displayName;
                    selectedItemCost.text = $"{freshData.cost} gold";
                }
            }

            // Also updates buy button interactability every frame
            if (buyButton != null)
            {
                var afford = gld >= sel.cost && !ownedIds.Contains(sel.id);
                buyButton.interactable = afford;
            }
        }

        private void LoadShopItems()
        {
            ld = true;

            if (GameAPI.Instance == null) return;

            StartCoroutine(GameAPI.Instance.FetchShopItems(resp =>
            {
                itms.Clear();
                foreach (var item in resp.items)
                {
                    itms.Add(item);
                    SpawnItemCard(item);
                }
                ld = false;
            }));

            StartCoroutine(GameAPI.Instance.FetchWallet(w =>
            {
                gld = w.gold;
                if (goldDisplay != null)
                    goldDisplay.text = $"Gold: {gld}";
            }));
        }

        private void SpawnItemCard(ItemData item)
        {
            if (itemPrefab == null || itemContainer == null) return;

            var card = Instantiate(itemPrefab, itemContainer);
            var nameText = card.GetComponentInChildren<TextMeshProUGUI>();
            if (nameText != null)
                nameText.text = item.displayName;

            var btn = card.GetComponent<Button>();
            if (btn != null)
            {
                var capturedItem = item;
                btn.onClick.AddListener(() => SelectItem(capturedItem));
            }

            spwnedCards[item.id] = card;
        }

        private void SelectItem(ItemData item)
        {
            sel = item;
        }

        private void OnBuyClicked()
        {
            if (sel == null) return;
            if (gld < sel.cost) return;
            if (ownedIds.Contains(sel.id)) return;

            StartCoroutine(GameAPI.Instance.PurchaseItem(sel.id, result =>
            {
                if (result.success)
                {
                    ownedIds.Add(sel.id);
                    gld = result.remainingGold;
                    if (goldDisplay != null)
                        goldDisplay.text = $"Gold: {gld}";
                }
            }));
        }

        private void OnEquipClicked()
        {
            if (sel == null) return;
            if (!ownedIds.Contains(sel.id)) return;

            // Find the local player and equip
            pc = FindObjectOfType<PlayerController>();
            if (pc != null)
            {
                pc.EquipItem(sel.id);
            }
        }

        private void OnDestroy()
        {
            // Clean up spawned cards
            foreach (var kvp in spwnedCards)
            {
                if (kvp.Value != null)
                    Destroy(kvp.Value);
            }
            spwnedCards.Clear();
        }
    }
}
