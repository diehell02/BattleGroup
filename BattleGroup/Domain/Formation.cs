using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BattleGroup.Domain.Place;

namespace BattleGroup.Domain
{
    class Formation
    {
        List<Hero> Heroes;

        public List<Place> Places
        {
            get;
            set;
        }

        public Formation(List<Place> places)
        {
            Places = new List<Place>(places);
            Heroes = new List<Hero>();

            places.ForEach(place =>
            {
                Heroes.Add(place.Hero);
            });
        }

        public bool ContainsHero(Hero hero)
        {
            return Heroes.Contains(hero);
        }
    }
}
