namespace Countries
{
    using BunifuAnimatorNS;
    using Properties;
    using Models;
    using Newtonsoft.Json;
    using Svg;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Versioning;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Countries.Servicos;

    public partial class Home : Form
    {
        public DirectoryInfo Location { get; set; }
        //public List<Country> ListCountryName { get; set; } = new List<Country>();
        public List<Language> ListLanguage { get; set; }

        private List<Country> ListCountryName = new List<Country>();
        private NetWorkService networkService { get; set; }
        private ApiService apiService { get; set; }
        private DialogService dialogService;
        private DataService dataService;

        public Home()
        {
            InitializeComponent();

            networkService = new NetWorkService();
            apiService = new ApiService();
            dialogService = new DialogService();
            Location = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent;
            dataService = new DataService();
            LoadCountries();
        }

        private async void LoadCountries()
        {

            bool load;
            

            lbl_Resultado.Text = "A atualizar informações...";

            var connection = networkService.CheckConnection();
            
            
            if (!connection.IsSuccess)
            {
                LoadLocalInfo();
                load = false;
            }
            else
            {
                await Task.Run(() => LoadApiInfo());
                load = true;
            }

            if(ListCountryName.Count == 0)
            {
                lbl_Resultado.Text = "Não há ligação á Internet" + Environment.NewLine + 
                    "e não foram previamente carregadas as informações" + Environment.NewLine +
                    "Tente mais tarde!";
                return;
            }

            await RunDownloadParallelAsync();

            cb_Countries.DisplayMember = "name";

            lbl_Resultado.Text = "Informações atualizadas";

            if (load)
            {
                lbl_Status.Text = string.Format("Informações carregadas da Internet em {0:F}", DateTime.Now);
            }
            else
            {
                lbl_Status.Text = string.Format("Informações carregadas da Base de Dados");
            }

            ProgressBar.Value = 100;
        }

        private void LoadLocalInfo()
        {
            ListCountryName = dataService.GetData();
        }

        private async Task LoadApiInfo()
        {
            ProgressBar.Value = 0;

            var response = await apiService.GetInfo("http://restcountries.eu", "/rest/v2/all");

            ListCountryName = (List<Country>)response.Result;
            dataService.DeleteData();
            dataService.SaveData(ListCountryName);
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
                    pic_Flag.Source = File.Exists($"{Location.FullName}\\Flags\\{item.name}.svg") ? $"{Location.FullName}\\Flags\\{item.name}.svg" : Location.FullName + "\\Resources\\notavailable.svg";
                    lbl_CountryName.Text = item.name == null ? "N/A" : item.name;
                    lbl_CountryCapital.Text = string.IsNullOrEmpty(item.capital) ? "N/A" : item.capital;
                    lbl_CountryRegion.Text = string.IsNullOrEmpty(item.region) ? "N/A" : item.region;
                    lbl_CountrySubRegion.Text = string.IsNullOrEmpty(item.subregion) ? "N/A" : item.subregion;
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

        private void btn_Outros_Click(object sender, EventArgs e)
        {
            cb_Countries.Items.Clear();
            ShowHideSelectionBarWrapper();
            SelectionBarLocation(10, 673);

            foreach (var item in ListCountryName)
            {
                if (string.IsNullOrEmpty(item.region))
                    cb_Countries.Items.Add(item.name);
            }
        }

        private void DownloadSVG(DirectoryInfo Location, Country country)
        {
            string path = Location.FullName + "\\Flags\\";

            WebClient webClient = new WebClient();

            try
            {
                webClient.DownloadFile(new Uri(country.flag), $"{path}{country.name}.svg");
            }
            catch
            {
            }



            webClient.Dispose();
        }

        private async Task RunDownloadParallelAsync()
        {
            List<Task> tasks = new List<Task>();

            foreach (Country pais in ListCountryName)
            {
                if (!File.Exists($"{Location.FullName}\\Flags\\{pais.name}.bmp") && !string.IsNullOrEmpty(pais.flag))
                {
                    tasks.Add(Task.Run(() => DownloadSVG(Location, pais)));
                }
            }


            await Task.WhenAll(tasks);
        }

    }
}
