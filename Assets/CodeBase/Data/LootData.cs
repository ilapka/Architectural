using System;
using System.Collections.Generic;
using System.Linq;

namespace Data
{
    [Serializable]
    public class LootData
    {
        public List<Loot> Loots;
        
        public int Collected;
        public event Action Changed;

        public LootData()
        {
            Loots = new List<Loot>();
        }

        public void Collect(Loot loot)
        {
            Collected += loot.Value;
            Remove(loot.Id);
            Changed?.Invoke();
        }
        
        public void Add(int value)
        {
            Collected += value;
            Changed?.Invoke();
        }

        public void AddLoot(Loot loot)
        {
            if(TryFindIndex(loot.Id, out int index))
            {
                throw new Exception($"Loot with id {loot.Id} already exist");
            }
            
            Loots.Add(loot);
        }

        public Loot GetLoot(string id)
        {
            Loot loot = Loots.FirstOrDefault(loot => loot.Id == id);
            return loot;
        }

        private void Remove(string id)
        {
            if(TryFindIndex(id, out int index))
                Loots.RemoveAt(index);
        }

        private bool TryFindIndex(string id, out int index)
        {
            for (int i = 0; i < Loots.Count; i++)
            {
                if (Loots[i].Id == id)
                {
                    index = i;
                    return true;
                }
            }

            index = 0;
            return false;
        }
    }
}