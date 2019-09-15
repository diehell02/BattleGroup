using BattleGroup.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleGroup
{
    class Exhaust
    {
        public List<Formation> Formations = new List<Formation>();

        public void AddFormation(List<Place> places)
        {
            Formations.Add(new Formation(places));
        }

        public static List<Formation> GetFormations(List<Player> players)
        {
            Exhaust exhaust = new Exhaust();

            //players.ForEach(player =>
            //{
            //    var temp = new List<Player>(players);
            //    temp.Remove(player);
            //    temp.Insert(0, player);

            //    LinkedList<Player> playerLinkedList = new LinkedList<Player>(temp);
            //    Traverse(playerLinkedList.First, new List<Place>(), exhaust);
            //});

            LinkedList<Player> playerLinkedList = new LinkedList<Player>(players);
            Traverse(playerLinkedList.First, new List<Place>(), exhaust);

            return exhaust.Formations;
        }

        private static void Traverse(LinkedListNode<Player> linkedListNode, List<Place> formation, Exhaust exhaust)
        {
            var player = linkedListNode.Value;

            player.Places.ForEach(place =>
            {
                formation.Add(place);

                if (linkedListNode.Next == null)
                {
                    if (Filter(formation))
                    {
                        exhaust.AddFormation(formation);
                    }                    
                }
                else
                {
                    Traverse(linkedListNode.Next, formation, exhaust);
                }

                formation.Remove(place);
            });
        }

        private static bool Filter(List<Place> formation)
        {
            Dictionary<Place.PlaceType, object> placeTypeDic = new Dictionary<Place.PlaceType, object>();
            Dictionary<string, object> heroDic = new Dictionary<string, object>();

            for(var i = 0; i < formation.Count; i++)
            {
                var place = formation[i];

                if (placeTypeDic.ContainsKey(place.Type))
                {
                    return false;
                }
                else
                {
                    placeTypeDic.Add(place.Type, null);
                }

                if (heroDic.ContainsKey(place.Hero.Name))
                {
                    return false;
                }
                else
                {
                    heroDic.Add(place.Hero.Name, null);
                }
            }

            return true;
        }
    }
}
