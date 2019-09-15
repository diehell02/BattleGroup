using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleGroup.Domain
{
    class Player
    {
        public string Name
        {
            get;
            set;
        }

        public List<Place> Places
        {
            get;
            set;
        }

        public Player()
        {
            Places = new List<Place>();
        }
    }
}
