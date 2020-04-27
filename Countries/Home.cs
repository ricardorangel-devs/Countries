namespace Countries
{
    using Countries.Properties;
    using Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Windows.Forms;

    public partial class Home : Form
    {
        public DirectoryInfo Location { get; set; }
        public List<Country> ListCountryName { get; set; }
        public List<Language> ListLanguage { get; set; }
        public Home()
        {
            InitializeComponent();
            LoadCountries();
            
        }

        private async void LoadCountries()
        {

            //bool load;
            ProgressBar.Value = 0;

            var client = new HttpClient(); //Fazer ligação via Http
            client.BaseAddress = new Uri("http://restcountries.eu"); //Endereço base da API

            var response = await client.GetAsync("/rest/v2/all"); //Controlador da API

            var result = await response.Content.ReadAsStringAsync(); //Guarda as informações da API no objecto result

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.ReasonPhrase);
                return;
            }

            var countries = JsonConvert.DeserializeObject<List<Country>>(result);
            ListCountryName = countries;

            //Location = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent;
            

            cb_Countries.DisplayMember = "name";
            ProgressBar.Value = 100;
        }

        private void MenuSidebar_Click(object sender, EventArgs e)
        {
            if (Sidebar.Width == 90)
            {
                Sidebar.Visible = false;
                Sidebar.Width = 270;
                SidebarWrapper.Width = 300;
                LineaSidebar.Width = 252;
                AnimationSidebar.Show(Sidebar);
            }
            else
            {
                Sidebar.Visible = false;
                Sidebar.Width = 90;
                SidebarWrapper.Width = 120;
                LineaSidebar.Width = 74;
                AnimationSidebarBack.Show(Sidebar);
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_Minimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btn_Maximize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            btn_Maximize.Visible = false;
            btn_Restore.Visible = true;
        }

        private void btn_Restore_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            btn_Restore.Visible = false;
            btn_Maximize.Visible = true;
        }

        private void ShowHideSelectionBarWrapper()
        {
            SelecionBarValuesReset();

            if (SelectionBar.Width == 0)
            {
                SelectionBar.Visible = false;
                SelectionBar.Width = 300;
                SelectionBarWrapper.Width = 320;
                AnimationSelectionBar.Show(SelectionBar);
            }
            
        }
        private void SelecionBarValuesReset()
        {
            SelectionBar.Width = 0;
            SelectionBarWrapper.Width = 0;
        }

        private void SelectionBarLocation(int x, int y)
        {
            SelectionBar.Location = new Point(x, y);
        }

        //================================ Country Buttons =================================================//

        private void btn_Oceania_Click(object sender, EventArgs e)
        {
            cb_Countries.Items.Clear();
            ShowHideSelectionBarWrapper();

            SelectionBarLocation(10, 109);

            SelectionBar.Location = new Point(10, 109);

            foreach (var item in ListCountryName)
            {
                if (item.region == "Oceania")
                    cb_Countries.Items.Add(item.name);
            }
        }

        private void btn_Europe_Click(object sender, EventArgs e)
        {
            cb_Countries.Items.Clear();
            ShowHideSelectionBarWrapper();
            SelectionBarLocation(10, 203);

            SelectionBar.Location = new Point(10, 203);

            foreach (var item in ListCountryName)
            {
                if (item.region == "Europe")
                    cb_Countries.Items.Add(item.name);
            }
        }

        private void btn_Asia_Click(object sender, EventArgs e)
        {
            cb_Countries.Items.Clear();
            ShowHideSelectionBarWrapper();
            SelectionBarLocation(10, 297);

            foreach (var item in ListCountryName)
            {
                if (item.region == "Asia")
                    cb_Countries.Items.Add(item.name);
            }
        }

        private void btn_Africa_Click(object sender, EventArgs e)
        {
            cb_Countries.Items.Clear();
            ShowHideSelectionBarWrapper();
            SelectionBarLocation(10, 391);
            foreach (var item in ListCountryName)
            {
                if (item.region == "Africa")
                    cb_Countries.Items.Add(item.name);
            }
        }

        private void btn_America_Click(object sender, EventArgs e)
        {
            cb_Countries.Items.Clear();
            ShowHideSelectionBarWrapper();
            SelectionBarLocation(10, 485);

            foreach (var item in ListCountryName)
            {
                if (item.region == "Americas")
                    cb_Countries.Items.Add(item.name);
            }
        }

        //==================================================================================================//

        private void btn_MinimizeSelectionSideBarWrapper_Click(object sender, EventArgs e)
        {
            SelecionBarValuesReset();
        }

        private void btn_Antarctica_Click(object sender, EventArgs e)
        {
            cb_Countries.Items.Clear();
            ShowHideSelectionBarWrapper();
            SelectionBarLocation(10, 579);

            foreach (var item in ListCountryName)
            {
                if (item.region == "Polar")
                    cb_Countries.Items.Add(item.name);
            }
        }

        private void cb_Countries_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbl_CountryLanguages.Text = string.Empty;

            foreach (var item in ListCountryName)
            {
                if (cb_Countries.SelectedItem.ToString() == item.name)
                {
                    
                    lbl_CountryName.Text = item.name == null ? "N/A" : item.name;
                    lbl_CountryCapital.Text = item.capital == null ? "N/A" : item.capital;
                    lbl_CountryRegion.Text = item.region == null ? "N/A" : item.region;
                    lbl_CountrySubRegion.Text = item.subregion == null ? "N/A" : item.subregion;
                    lbl_CountryPopulation.Text = item.population == null ? "N/A" : item.population.ToString();
                    lbl_CountryGiniIndex.Text = item.gini == null ? "N/A" : item.gini.ToString();
                    lbl_CountryNativeName.Text = item.nativeName == null ? "N/A" : item.nativeName;
                    lbl_CountryArea.Text = item.area == null ? "N/A" : $"{item.area.ToString()} km²";
                    foreach (var language in item.languages)
                    {
                        lbl_CountryLanguages.Text += $"{language.name.ToString()}\n";

                    }
                    

                }
                    
            }
        }

    }
}
