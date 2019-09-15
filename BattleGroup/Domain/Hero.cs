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
        public enum SpecialityType
        {
            Protection,
            Control,
            Initiative,
            Counter,
            LineLeader,
            Brust,
            Consume,
            Reap,
        }

        [DataContract]
        public class Speciality
        {
            private SpecialityType? type;

            [DataMember(Name ="Type")]
            public string TypeWrap
            {
                get;
                set;
            }

            [IgnoreDataMember]
            public SpecialityType Type
            {
                get
                {
                    if (type == null)
                    {
                        SpecialityType _type = SpecialityType.Control;
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

            //public Speciality()
            //{
            //    Value = 1;
            //}
        }

        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public List<Speciality> Specialities
        {
            get;
            set;
        }

        public Hero()
        {
            Specialities = new List<Speciality>();
        }
    }
}
