using BattleGroup.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace BattleGroup
{
    class HeroConfig
    {
        public static List<Hero> Heroes
        {
            get;
            private set;
        }

        static Dictionary<string, Hero> HeroDic = new Dictionary<string, Hero>(); 

        static HeroConfig()
        {
            var path = "hero.json";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(File.ReadAllText(path)));

            var ser = new DataContractJsonSerializer(typeof(List<Hero>));
            Heroes = ser.ReadObject(ms) as List<Hero>;

            Heroes.ForEach(hero =>
            {
                HeroDic.Add(hero.Name, hero);
            });
        }

        public static Hero GetHero(string heroName)
        {
            Hero hero = null;

            HeroDic.TryGetValue(heroName, out hero);

            return hero;
        }
    }
}
