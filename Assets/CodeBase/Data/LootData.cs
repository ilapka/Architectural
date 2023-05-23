﻿using System;

namespace Data
{
    [Serializable]
    public class LootData
    {
        public int Collected;
        public event Action Changed;

        public void Collect(Loot loot)
        {
            Collected += loot.Value;
            Changed?.Invoke();
        }
    }
}