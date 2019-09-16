using BattleGroup.Domain;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleGroup
{
    class FormationGroup : INotifyPropertyChanged
    {
        List<Formation> Formations = new List<Formation>();

        List<Hero> BanList = new List<Hero>();

        List<Hero> PickOfCompetitor = new List<Hero>();

        List<Hero> PickList = new List<Hero>();

        public event PropertyChangedEventHandler PropertyChanged;

        public string BanAndPick
        {
            get
            {
                var text = new StringBuilder();

                text.Append("BAN:").Append(" ");

                BanList.ForEach(ban =>
                {
                    text.Append(ban.Name).Append(", ");
                });

                text.Append("\n对手Pick:").Append(" ");

                PickOfCompetitor.ForEach(pick =>
                {
                    text.Append(pick.Name).Append(", ");
                });
                
                Dictionary<Hero.SpecialtyType, int> placeSum = new Dictionary<Hero.SpecialtyType, int>()
                {
                    { Hero.SpecialtyType.Protection, 0 },
                    { Hero.SpecialtyType.Control, 0 },
                    { Hero.SpecialtyType.Initiative, 0 },
                    { Hero.SpecialtyType.Counter, 0 },
                    { Hero.SpecialtyType.LineLeader, 0 },
                    { Hero.SpecialtyType.Burst, 0 },
                    { Hero.SpecialtyType.Consume, 0 },
                    { Hero.SpecialtyType.Reap, 0 },
                };

                PickOfCompetitor.ForEach(pick =>
                {
                    pick.Specialties.ForEach(specialty =>
                    {
                        placeSum[specialty.Type]++;
                    });
                });

                text.Append("\n对手阵容:").Append(" ");

                foreach(var place in placeSum)
                {
                    text.Append(place.Key).Append(": ").Append(place.Value).Append(", ");
                }

                text.Append("\n己方Pick:").Append(" ");

                PickList.ForEach(pick =>
                {
                    text.Append(pick.Name).Append(", ");
                });

                return text.ToString();
            }
        }

        public string Recommend
        {
            get
            {
                return GetRecommendPickHero();
            }
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Load(string path)
        {
            var players = LoadFromFile(path);

            var formations = Exhaust.GetFormations(players);

            formations.ForEach(formation =>
            {
                if (Filter(formation))
                {
                    Formations.Add(formation);
                }
            });
        }

        private bool Filter(Formation formation)
        {
            bool result = true;

            Dictionary<Hero.SpecialtyType, int> placeSum = new Dictionary<Hero.SpecialtyType, int>()
            {
                { Hero.SpecialtyType.Protection, 0 },
                { Hero.SpecialtyType.Control, 0 },
                { Hero.SpecialtyType.Initiative, 0 },
                { Hero.SpecialtyType.Counter, 0 },
                { Hero.SpecialtyType.LineLeader, 0 },
                { Hero.SpecialtyType.Burst, 0 },
                { Hero.SpecialtyType.Consume, 0 },
                { Hero.SpecialtyType.Reap, 0 },
            };

            formation.Places.ForEach(place =>
            {
                place.Hero.Specialties?.ForEach(speciality =>
                {
                    placeSum[speciality.Type] += speciality.Value;
                });                
            });

            if (placeSum[Hero.SpecialtyType.Control] > 0 &&
                placeSum[Hero.SpecialtyType.Protection] > 0 &&
                placeSum[Hero.SpecialtyType.LineLeader] > 0)
            {
                if (placeSum[Hero.SpecialtyType.Burst] > 0 && placeSum[Hero.SpecialtyType.Control] < 2)
                {
                    result = false;
                }

                if (placeSum[Hero.SpecialtyType.Reap] > 0 && placeSum[Hero.SpecialtyType.Consume] == 0)
                {
                    result = false;
                }

                if (placeSum[Hero.SpecialtyType.Counter] == 0 &&
                    placeSum[Hero.SpecialtyType.Initiative] == 0)
                {
                    result = false;
                }

                if (placeSum[Hero.SpecialtyType.Counter] > 0 &&
                    placeSum[Hero.SpecialtyType.Initiative] == 0 && 
                    placeSum[Hero.SpecialtyType.Control] < 2)
                {
                    result = false;
                }

                if (placeSum[Hero.SpecialtyType.Counter] == 0 &&
                    placeSum[Hero.SpecialtyType.Initiative] > 0 &&
                    placeSum[Hero.SpecialtyType.Control] < 2)
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        private List<Player> LoadFromFile(string path)
        {
            var fileStream = File.OpenRead(path);
            var workbook = new XSSFWorkbook(fileStream);
            List<Player> players = new List<Player>();

            var sheetIndex = 0;

            while(sheetIndex < workbook.NumberOfSheets)
            {
                var sheet = workbook.GetSheetAt(sheetIndex++);
                var player = new Player()
                {
                    Name = sheet.SheetName
                };

                var rowIndex = 0;

                while(rowIndex <= sheet.LastRowNum)
                {
                    var row = sheet.GetRow(rowIndex++);

                    var placeName = row.GetCell(0).StringCellValue;
                    Place.PlaceType placeType = Place.PlaceType.Top;

                    switch(placeName)
                    {
                        case "上路":
                            placeType = Place.PlaceType.Top;
                            break;
                        case "打野":
                            placeType = Place.PlaceType.Jungle;
                            break;
                        case "中路":
                            placeType = Place.PlaceType.Middle;
                            break;
                        case "下路":
                            placeType = Place.PlaceType.Bottom;
                            break;
                        case "辅助":
                            placeType = Place.PlaceType.Support;
                            break;
                    }

                    var colIndex = 1;
                    while(colIndex < row.LastCellNum)
                    {
                        var heroName = row.GetCell(colIndex++).StringCellValue;

                        if (string.IsNullOrEmpty(heroName))
                        {
                            continue;
                        }

                        Place place = new Place()
                        {
                            Type = placeType,
                            Hero = HeroConfig.GetHero(heroName)
                        };

                        player.Places.Add(place);
                    }
                }

                players.Add(player);
            }

            return players;
        }

        public void AddBan(Hero hero)
        {
            if (!BanList.Contains(hero))
            {
                BanList.Add(hero);

                OnPropertyChanged("BanAndPick");
                OnPropertyChanged("Recommend");
            }
        }

        public void RemoveBan(Hero hero)
        {
            if (BanList.Contains(hero))
            {
                BanList.Remove(hero);

                OnPropertyChanged("BanAndPick");
                OnPropertyChanged("Recommend");
            }
        }

        public void AddPickOfCompetitor(Hero hero)
        {
            if (!PickOfCompetitor.Contains(hero))
            {
                PickOfCompetitor.Add(hero);

                OnPropertyChanged("BanAndPick");
                OnPropertyChanged("Recommend");
            }                
        }

        public void RemovePickOfCompetitor(Hero hero)
        {
            if (PickOfCompetitor.Contains(hero))
            {
                PickOfCompetitor.Remove(hero);

                OnPropertyChanged("BanAndPick");
                OnPropertyChanged("Recommend");
            }
        }

        public void AddPick(Hero hero)
        {
            if (!PickList.Contains(hero))
            {
                PickList.Add(hero);

                OnPropertyChanged("BanAndPick");
                OnPropertyChanged("Recommend");
            }                
        }

        public void RemovePick(Hero hero)
        {
            if (PickList.Contains(hero))
            {
                PickList.Remove(hero);

                OnPropertyChanged("BanAndPick");
                OnPropertyChanged("Recommend");
            }
        }

        private List<KeyValuePair<Hero, int>> GetHeroSummary(List<Formation> formations)
        {
            Dictionary<Hero, int> heroes = new Dictionary<Hero, int>();

            formations.ForEach(formation =>
            {
                formation.Places.ForEach(place =>
                {
                    if (heroes.ContainsKey(place.Hero))
                    {
                        heroes[place.Hero]++;
                    }
                    else
                    {
                        heroes.Add(place.Hero, 1);
                    }
                });
            });

            List<KeyValuePair<Hero, int>> result = new List<KeyValuePair<Hero, int>>(heroes);

            result.Sort((x, y) =>
            {
                if (x.Value < y.Value)
                {
                    return 1;
                }
                else if (x.Value > y.Value)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            });

            return result;
        }

        public string GetRecommendPickHero()
        {
            StringBuilder result = new StringBuilder();

            var formation = FilterFromDisable();
            formation = FilterFromSelect(formation);

            var summary = GetHeroSummary(formation);

            foreach(var item in summary)
            {
                if (PickList.Contains(item.Key))
                {
                    continue;
                }

                result.Append($"{item.Key.Name} {item.Value}").Append("\n");
            }

            return result.ToString();
        }

        private List<Formation> FilterFromDisable()
        {
            var result = new List<Formation>();

            Formations.ForEach(formation =>
            {
                bool flag = false;

                BanList.ForEach(ban =>
                {
                    if (formation.ContainsHero(ban))
                    {
                        flag = true;
                    }
                });

                PickOfCompetitor.ForEach(pick =>
                {
                    if (formation.ContainsHero(pick))
                    {
                        flag = true;
                    }
                });

                if (!flag)
                {
                    result.Add(formation);
                }
            });

            return result;
        }

        private List<Formation> FilterFromSelect(List<Formation> formations)
        {
            var result = new List<Formation>(formations);

            formations.ForEach(formation =>
            {
                PickList.ForEach(ban =>
                {
                    if (!formation.ContainsHero(ban))
                    {
                        result.Remove(formation);
                    }
                });
            });

            return result;
        }
    }
}
