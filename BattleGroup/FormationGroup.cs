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

                text.Append("\nPickOfCompetitor:").Append(" ");

                PickOfCompetitor.ForEach(pick =>
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
            Dictionary<Hero.SpecialityType, int> placeSum = new Dictionary<Hero.SpecialityType, int>()
            {
                { Hero.SpecialityType.Protection, 0 },
                { Hero.SpecialityType.Control, 0 },
                { Hero.SpecialityType.Initiative, 0 },
                { Hero.SpecialityType.Counter, 0 },
                { Hero.SpecialityType.LineLeader, 0 },
                { Hero.SpecialityType.Brust, 0 },
                { Hero.SpecialityType.Consume, 0 },
                { Hero.SpecialityType.Reap, 0 },
            };

            formation.Places.ForEach(place =>
            {
                place.Hero.Specialities.ForEach(speciality =>
                {
                    placeSum[speciality.Type] += speciality.Value;
                });                
            });

            if (placeSum[Hero.SpecialityType.Control] > 1 &&
                placeSum[Hero.SpecialityType.Protection] > 0 &&
                placeSum[Hero.SpecialityType.Initiative] > 0 &&
                placeSum[Hero.SpecialityType.Counter] > 0 &&
                placeSum[Hero.SpecialityType.LineLeader] > 0)
            {
                if (placeSum[Hero.SpecialityType.Brust] > 0)
                {
                    return true;
                }
                else if (placeSum[Hero.SpecialityType.Consume] > 0 &&
                    placeSum[Hero.SpecialityType.Reap] > 0)
                {
                    return true;
                }
            }

            return false;
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

        private List<KeyValuePair<string, int>> GetHeroSummary(List<Formation> formations)
        {
            Dictionary<string, int> heroes = new Dictionary<string, int>();

            formations.ForEach(formation =>
            {
                formation.Places.ForEach(place =>
                {
                    if (heroes.ContainsKey(place.Hero.Name))
                    {
                        heroes[place.Hero.Name]++;
                    }
                    else
                    {
                        heroes.Add(place.Hero.Name, 1);
                    }
                });
            });

            List<KeyValuePair<string, int>> result = new List<KeyValuePair<string, int>>(heroes);

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
                result.Append($"{item.Key} {item.Value}").Append("\n");
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
