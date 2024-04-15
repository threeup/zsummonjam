
using System.Collections.Generic;
using UnityEngine;
using zapo;

namespace zum
{

    public class ZumHUD : ZapoSingleton<ZumHUD>
    {
        public List<ZumHeldCard> Cards = new();
        private Queue<ZumHeldCard> AvailableCards = new();
        private List<ZumHeldCard> LeftActiveCards = new();
        private List<ZumHeldCard> RightActiveCards = new();


        public void Start()
        {
            Cards.ForEach(c => UnassignCard(c));
        }

        public void LeftHandRefresh(ref List<ZumMineral> mins)
        {
            List<bool> IndicesFound = new List<bool>();
            for (int i = 0; i < mins.Count; ++i)
            {
                IndicesFound.Add(false);
            }
            foreach (var card in LeftActiveCards)
            {
                bool stillActive = false;
                for (int i = 0; i < mins.Count; ++i)
                {
                    if (mins[i].gameObject.name == card.minName)
                    {
                        stillActive = true;
                        IndicesFound[i] = true;
                        card.SetSlot(i, true);
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
                    var nextCard = TakeAvailableCard(mins[i]);
                    nextCard.SetSlot(i, true);
                    LeftActiveCards.Add(nextCard);
                }
            }
        }
        public void RightHandRefresh(ref List<ZumMineral> mins)
        {
            List<bool> IndicesFound = new List<bool>();
            for (int i = 0; i < mins.Count; ++i)
            {
                IndicesFound.Add(false);
            }
            foreach (var card in RightActiveCards)
            {
                bool stillActive = false;
                for (int i = 0; i < mins.Count; ++i)
                {
                    if (mins[i].gameObject.name == card.minName)
                    {
                        stillActive = true;
                        IndicesFound[i] = true;
                        card.SetSlot(i, false);
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
                    var nextCard = TakeAvailableCard(mins[i]);
                    nextCard.SetSlot(i, false);
                    RightActiveCards.Add(nextCard);
                }
            }
        }

        private ZumHeldCard TakeAvailableCard(ZumMineral min)
        {
            ZumHeldCard card = AvailableCards.Dequeue();
            card.minName = min.gameObject.name;
            card.AssignValues(min.GetTargetColor());
            return card;
        }

        private void UnassignCard(ZumHeldCard card)
        {
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
                RightActiveCards.Remove(foundCard);
                UnassignCard(foundCard);
            }
        }
    }
}