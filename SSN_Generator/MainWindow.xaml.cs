using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace SSN_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        List<Person> persons = new List<Person>();

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;


            RoutedCommand cmndSettingsClose = new RoutedCommand();
            cmndSettingsClose.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(cmndSettingsClose, CloseMenu_Click));

            RoutedCommand cmndSettingsSave = new RoutedCommand();
            cmndSettingsSave.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(cmndSettingsSave, SaveMeny_Click));

            RoutedCommand cmndSettingsAbout = new RoutedCommand();
            cmndSettingsAbout.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(cmndSettingsAbout, AboutMeny_Click));

            List<ComboItem> cboItems = new List<ComboItem>();
            cboItems.Add(new ComboItem("1900-1999"));
            cboItems.Add(new ComboItem("1854-1899"));
            cboItems.Add(new ComboItem("2000-2039"));
            cboItems.Add(new ComboItem("1940-1999"));
            cboBirth.DataContext = cboItems;

        }

        private void BtnRadio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtBxCnt.Background = Brushes.White;
                CheckReady();
                CheckCount();

            }
            catch (Exception ee)
            {
                btnGenerate.IsEnabled = false;
                txtBxCnt.Background = Brushes.PaleVioletRed;
                blckStatus.Text = $"ERROR, {ee.Message}";
            }
        }


        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string birthInt = ((ComboItem)cboBirth.SelectedItem).BirthInterval;
                string century = birthInt.Substring(0, 2);
                int cnt = int.Parse(txtBxCnt.Text);
                char sex = FindSex();

                if (!txtBxDob.Text.Equals(""))
                {
                    if (ValidDate())
                    {

                        string date = txtBxDob.Text.Substring(0, 4) + century + txtBxDob.Text.Substring(4);

                        DateTime dt = DateTime.ParseExact(txtBxDob.Text.Substring(0, 4) + century + txtBxDob.Text.Substring(4), "ddMMyyyy", CultureInfo.InvariantCulture);

                        persons = Person.GeneratePersonList(dt, cnt, birthInt, sex);

                        dataGridPersons.DataContext = persons;
                        blckStatus.Text = $"Success, {persons.Count()} numbers generated";
                    }
                    else
                    {
                        blckStatus.Text = "Fail, invalid date of birth";
                    }
                }
                else
                {
                    persons = Person.GenerateRandomList(cnt, birthInt, sex);
                    dataGridPersons.DataContext = persons;
                    blckStatus.Text = $"Success, {persons.Count()} numbers generated";
                }
            }
            catch (Exception ee)
            {
                blckStatus.Text = $"Fail: {ee.Message}";
            }

        }




        private void TxtBxDob_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                int.Parse((txtBxDob.Text == "") ? "0" : txtBxDob.Text);

                txtBxDob.Background = Brushes.White;
                CheckReady();
                CheckCount();
            }
            catch (Exception ee)
            {
                btnGenerate.IsEnabled = false;
                txtBxDob.Background = Brushes.PaleVioletRed;
                blckStatus.Text = $"ERROR, {ee.Message}";
            }
        }

        private void TxtBxCnt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (btnGenerate != null)
            {
                try
                {
                    int cnt = int.Parse((txtBxCnt.Text == "") ? "0" : txtBxCnt.Text);
                    char sex = FindSex();


                    txtBxCnt.Background = Brushes.White;
                    CheckReady();
                    CheckCount();
                }
                catch (Exception ee)
                {
                    btnGenerate.IsEnabled = false;
                    txtBxCnt.Background = Brushes.PaleVioletRed;
                    blckStatus.Text = $"ERROR, {ee.Message}";
                }
            }
        }


        private void CheckReady()
        {
            if (txtBxCnt.Background.Equals(Brushes.White) && txtBxDob.Background.Equals(Brushes.White))
            {
                btnGenerate.IsEnabled = true;
                blckStatus.Text = "Ready";
            }
        }

        private void CheckCount()
        {
            char sex = FindSex();
            string birthInt = ((ComboItem)cboBirth.SelectedItem).BirthInterval; 

            int cnt = int.Parse((txtBxCnt.Text == "") ? "0" : txtBxCnt.Text);
            if (sex == 'B' && cnt > 400 && !string.IsNullOrWhiteSpace(txtBxDob.Text)) throw new ArgumentException("Max 400 numbers available for both sexes when birthdate is specified");
            if (sex == 'F' && cnt > 200 && !string.IsNullOrWhiteSpace(txtBxDob.Text)) throw new ArgumentException("Max 200 numbers available for males when birthdate is specified");
            if (sex == 'M' && cnt > 200 && !string.IsNullOrWhiteSpace(txtBxDob.Text)) throw new ArgumentException("Max 200 numbers available for females when birthdate is specified");
            if (sex != 'B' && birthInt.Equals("1940-1999") && cnt > 40 && !string.IsNullOrWhiteSpace(txtBxDob.Text)) throw new ArgumentException("Max 40 numbers available when birthdate is 1940-1999 and gender is specified");
            if (sex == 'B' && birthInt.Equals("1940-1999") && cnt > 80 && !string.IsNullOrWhiteSpace(txtBxDob.Text)) throw new ArgumentException("Max 80 numbers available when birthdate is 1940-1999 and gender is specified");
            txtBxDob.Background = Brushes.White;
        }

        private char FindSex()
        {
            if (btnBoth.IsChecked == true) return 'B';
            if (btnFemale.IsChecked == true) return 'F';
            if (btnMale.IsChecked == true) return 'M';
            return 'X';
        }

        private bool ValidDate()
        {
            string birthInt = ((ComboItem)cboBirth.SelectedItem).BirthInterval;
            int from = int.Parse(birthInt.Split('-')[0].Substring(2, 2));
            int to = int.Parse(birthInt.Split('-')[1].Substring(2, 2));
            int year = int.Parse(txtBxDob.Text.Substring(4, 2));
            return (year >= from && year <= to) ? true : false;
        }

        struct ComboItem
        {
            public string BirthInterval { get; set; }

            public ComboItem(string birthInterval)
            {
                BirthInterval = birthInterval;
            }

            public override string ToString()
            {
                return BirthInterval;
            }
        }

        private void SaveMeny_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var p in persons)
            {
                sb.AppendLine(p.ToString());
            }

            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*|CSV (Comma delimited)|*.csv"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, sb.ToString());
            }
        }

        private void CloseMenu_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnClipboard_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var p in persons)
            {
                sb.Append(p.Ssn);
                if (chbxSex.IsChecked == true) sb.Append($";{p.Sex}");
                if (chbxDob.IsChecked == true) sb.Append($";{p.Dob.ToString("yyyy-MM-dd")}");
                sb.AppendLine();
            }

            Clipboard.SetText(sb.ToString());
        }

        private void AboutMeny_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tool to generate Norwegian Social Security numbers.\n" +
                "https://en.wikipedia.org/wiki/National_identification_number#Norway" +
                "\n\n" +
                "Magnus V.Tisløv, 2018", "About", MessageBoxButton.OK);
        }

    }
}
