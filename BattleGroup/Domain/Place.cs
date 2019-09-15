using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleGroup.Domain
{
    class Place
    {
        public enum PlaceType
        {
            Top,
            Jungle,
            Middle,
            Bottom,
            Support
        }

        public PlaceType Type
        {
            get;
            set;
        }

        public Hero Hero
        {
            get;
            set;
        }
    }
}
