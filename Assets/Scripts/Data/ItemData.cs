using System;

namespace HarnessTestBed.Data
{
    [Serializable]
    public class ItemData
    {
        public string id;
        public string displayName;
        public int cost;
        public ItemType itemType;
        public string description;
        public string iconPath;
    }
}
