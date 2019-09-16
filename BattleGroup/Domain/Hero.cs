using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BattleGroup.Domain
{
    [DataContract]
    class Hero
    {
        public enum SpecialtyType
        {
            Protection,
            Control,
            Initiative,
            Counter,
            LineLeader,
            Burst,
            Consume,
            Reap,
        }

        [DataContract]
        public class Specialty
        {
            private SpecialtyType? type;

            [DataMember(Name ="Type")]
            public string TypeWrap
            {
                get;
                set;
            }

            [IgnoreDataMember]
            public SpecialtyType Type
            {
                get
                {
                    if (type == null)
                    {
                        SpecialtyType _type = SpecialtyType.Control;
                        Enum.TryParse(TypeWrap, out _type);

                        type = _type;
                    }

                    return type.Value;
                }
            }

            [DataMember]
            public int Value
            {
                get;
                set;
            }

            [OnDeserialized]
            void OnDeserialized(StreamingContext c)
            {
                Value = (Value == 0) ? 1 : Value;
            }
        }

        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public List<Specialty> Specialties
        {
            get;
            set;
        }

        public Hero()
        {
            Specialties = new List<Specialty>();
        }
    }
}
