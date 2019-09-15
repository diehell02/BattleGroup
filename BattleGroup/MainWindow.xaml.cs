using BattleGroup.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BattleGroup
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        FormationGroup FormationGroup;

        public MainWindow()
        {
            FormationGroup = new FormationGroup();

            FormationGroup.Load("克里斯不演必胜小队.xlsx");

            InitializeComponent();

            this.DataContext = FormationGroup;
            BanComboBox.ItemsSource = HeroConfig.Heroes;
            CompetitorComboBox.ItemsSource = HeroConfig.Heroes;
            PickComboBox.ItemsSource = HeroConfig.Heroes;
        }

        private void AddBanBtn_Click(object sender, RoutedEventArgs e)
        {
            var hero = BanComboBox.SelectedValue as Hero;

            FormationGroup.AddBan(hero);
        }

        private void AddCompetitor_Click(object sender, RoutedEventArgs e)
        {
            var hero = CompetitorComboBox.SelectedValue as Hero;

            FormationGroup.AddPickOfCompetitor(hero);
        }

        private void AddPick_Click(object sender, RoutedEventArgs e)
        {
            var hero = PickComboBox.SelectedValue as Hero;

            FormationGroup.AddPick(hero);
        }

        private void RemoveBanBtn_Click(object sender, RoutedEventArgs e)
        {
            var hero = BanComboBox.SelectedValue as Hero;

            FormationGroup.RemoveBan(hero);
        }

        private void RemoveCompetitor_Click(object sender, RoutedEventArgs e)
        {
            var hero = CompetitorComboBox.SelectedValue as Hero;

            FormationGroup.RemovePickOfCompetitor(hero);
        }

        private void RemovePick_Click(object sender, RoutedEventArgs e)
        {
            var hero = PickComboBox.SelectedValue as Hero;

            FormationGroup.RemovePick(hero);
        }
    }
}
