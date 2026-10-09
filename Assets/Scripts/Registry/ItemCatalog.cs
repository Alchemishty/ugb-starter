using System.Collections.Generic;
using UnityEngine;
using HarnessTestBed.Data;

namespace HarnessTestBed.Registry
{
    [CreateAssetMenu(fileName = "ItemCatalog", menuName = "HarnessTestBed/Item Catalog")]
    public class ItemCatalog : ScriptableObject
    {
        [SerializeField] private List<ItemData> items = new List<ItemData>();

        private Dictionary<string, ItemData> itemLookup;

        private void BuildLookup()
        {
            itemLookup = new Dictionary<string, ItemData>();
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.id))
                {
                    itemLookup[item.id] = item;
                }
            }
        }

        public ItemData GetById(string itemId)
        {
            if (itemLookup == null)
                BuildLookup();

            if (itemLookup.TryGetValue(itemId, out var item))
                return item;

            return null;
        }

        public List<ItemData> GetByType(ItemType type)
        {
            var result = new List<ItemData>();
            foreach (var item in items)
            {
                if (item.itemType == type)
                    result.Add(item);
            }
            return result;
        }

        public List<ItemData> GetAll()
        {
            return new List<ItemData>(items);
        }
    }
}
