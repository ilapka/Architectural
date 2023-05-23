using UnityEngine;

namespace Infrastructure.Services
{
    public class UnityRandomService : IRandomService
    {
        public int Next(int min, int max)
        {
            return Random.Range(min, max + 1);
        }
    }
}