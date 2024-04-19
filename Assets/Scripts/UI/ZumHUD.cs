
using System.Collections.Generic;
using UnityEngine;
using zapo;

namespace zum
{

    public class ZumHUD : ZapoSingleton<ZumHUD>
    {
        public List<ZumHeldCard> Cards = new();

        private Queue<ZumHeldCard> AvailableCards = new();
        [SerializeField]
        private List<ZumHeldCard> LeftActiveCards = new();
        [SerializeField]
        private List<ZumHeldCard> RightActiveCards = new();


        public void Start()
        {
            Cards.ForEach(c => UnassignCard(c));
        }

        public void LeftHandRefresh(ref List<ZumMineral> mins)
        {
            List<bool> IndicesFound = new List<bool>();
            for (int minIdx = 0; minIdx < mins.Count; ++minIdx)
            {
                IndicesFound.Add(false);
            }
            for (int cardIdx = LeftActiveCards.Count - 1; cardIdx >= 0; --cardIdx)
            {
                var card = LeftActiveCards[cardIdx];
                if (card.HeldState == HeldStateType.DISABLED)
                {
                    UnassignCard(card, true, false);
                    continue;
                }
                bool stillActive = false;
                for (int minIdx = 0; minIdx < mins.Count; ++minIdx)
                {
                    if (mins[minIdx].gameObject.name == card.minName)
                    {
                        stillActive = true;
                        IndicesFound[minIdx] = true;
                        card.SetSlot(minIdx, true);
                        break;
                    }
                }
                if (!stillActive)
                {
                    card.SetSlot(99, true);
                }

            }
            for (int i = 0; i < IndicesFound.Count; ++i)
            {
                if (!IndicesFound[i])
                {
                    TakeAvailableCard(mins[i], i, true);
                }
            }
        }
        public void RightHandRefresh(ref List<ZumMineral> mins)
        {
            List<bool> IndicesFound = new List<bool>();
            for (int minIdx = 0; minIdx < mins.Count; ++minIdx)
            {
                IndicesFound.Add(false);
            }
            for (int cardIdx = RightActiveCards.Count - 1; cardIdx >= 0; --cardIdx)
            {
                var card = RightActiveCards[cardIdx];
                bool stillActive = false;
                for (int minIdx = 0; minIdx < mins.Count; ++minIdx)
                {
                    if (mins[minIdx].gameObject.name == card.minName)
                    {
                        stillActive = true;
                        IndicesFound[minIdx] = true;
                        card.SetSlot(minIdx, false);
                        break;
                    }
                }
                if (!stillActive)
                {
                    card.SetSlot(99, false);
                }
            }
            for (int i = 0; i < IndicesFound.Count; ++i)
            {
                if (!IndicesFound[i])
                {
                    TakeAvailableCard(mins[i], i, false);
                }
            }
        }

        private ZumHeldCard TakeAvailableCard(ZumMineral min, int idx, bool toLeft)
        {
            ZumHeldCard card = AvailableCards.Dequeue();
            card.minName = min.gameObject.name;
            card.AssignValues(min.GetTargetColor());
            card.SetSlot(idx, toLeft);
            if (toLeft)
            {
                LeftActiveCards.Add(card);
            }
            else
            {
                RightActiveCards.Add(card);
            }
            return card;
        }

        private void UnassignCard(ZumHeldCard card, bool fromLeft = false, bool fromRight = false)
        {
            if (fromLeft)
            {
                LeftActiveCards.Remove(card);
            }
            else if (fromRight)
            {
                RightActiveCards.Remove(card);
            }
            card.SetSlot(-1, true);
            AvailableCards.Enqueue(card);
        }
        public void RightHandGrab(ZumMineral min)
        {
            ZumHeldCard foundCard = null;
            foreach (var card in LeftActiveCards)
            {
                if (min.name == card.minName)
                {
                    foundCard = card;
                    break;
                }
            }
            if (foundCard)
            {
                foundCard.SetSlot(0, false);
                LeftActiveCards.Remove(foundCard);
                RightActiveCards.Add(foundCard);
            }
        }

        public void RightHandConsume(ZumMineral min)
        {
            ZumHeldCard foundCard = null;
            foreach (var card in RightActiveCards)
            {
                if (min.name == card.minName)
                {
                    foundCard = card;
                    break;
                }
            }
            if (foundCard)
            {
                UnassignCard(
                    foundCard,
                    false, // fromLeft
                    true // fromRight
                );
            }
        }
    }
}