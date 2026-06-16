using System;

namespace Playable
{
    public class PlayerWallet
    {
        private int _currentCoinsCount;

        public int CurrentCoinsCount
        {
            get => _currentCoinsCount;
            private set
            {
                if (value < 0)
                    throw new System.ArgumentOutOfRangeException(nameof(_currentCoinsCount), "Coins cannot be a negative!");

                _currentCoinsCount = value;
                OnCoinsChanged?.Invoke(_currentCoinsCount);
            }
        }

        public event Action<int> OnCoinsChanged;

        public PlayerWallet(int initialCoins)
        {
            CurrentCoinsCount = initialCoins;
        }

        public void RequestCurrentCoinsCount() => OnCoinsChanged?.Invoke(CurrentCoinsCount);

        public void AddCoins(int amount) => CurrentCoinsCount += amount;
        public void SpendCouns(int amount) => CurrentCoinsCount -= amount;
    }
}