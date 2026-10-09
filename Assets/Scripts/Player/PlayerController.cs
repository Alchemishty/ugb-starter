using UnityEngine;
using Unity.Netcode;
using Newtonsoft.Json;
using System.Collections.Generic;
using HarnessTestBed.Data;

namespace HarnessTestBed.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;

        private NetworkVariable<PlayerState> playerState = new NetworkVariable<PlayerState>(
            new PlayerState("{}"),
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

        private List<string> equippedItemIds = new List<string>();

        private void Awake()
        {
            playerState.OnValueChanged += OnPlayerStateChanged;
        }

        private void Update()
        {
            if (!IsOwner) return;

            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            var movement = new Vector3(horizontal, 0f, vertical) * moveSpeed * Time.deltaTime;
            transform.Translate(movement, Space.World);
        }

        private void OnPlayerStateChanged(PlayerState previous, PlayerState current)
        {
            var items = JsonConvert.DeserializeObject<List<string>>(current.JsonPayload);
            if (items != null)
            {
                equippedItemIds = items;
                RefreshVisuals();
            }
        }

        public void EquipItem(string itemId)
        {
            if (!IsOwner) return;

            if (!equippedItemIds.Contains(itemId))
                equippedItemIds.Add(itemId);

            var json = JsonConvert.SerializeObject(equippedItemIds);
            playerState.Value = new PlayerState(json);
        }

        public void UnequipItem(string itemId)
        {
            if (!IsOwner) return;

            equippedItemIds.Remove(itemId);

            var json = JsonConvert.SerializeObject(equippedItemIds);
            playerState.Value = new PlayerState(json);
        }

        private void RefreshVisuals()
        {
            // Placeholder — would instantiate/destroy visual representations
            // of equipped items on the player model
        }

        private void OnDestroy()
        {
            playerState.OnValueChanged -= OnPlayerStateChanged;
        }
    }
}
