using Countries.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Countries.Servicos
{
    public class DataService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService;

        private DirectoryInfo Location;



        public DataService()
        {
            Location = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent;

            dialogService = new DialogService();

            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            if (!Directory.Exists(Location.FullName + "\\Flags"))
            {
                Directory.CreateDirectory(Location.FullName + "\\Flags");
            }

            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlcommand = "CREATE TABLE IF NOT EXISTS COUNTRIES" +
                    "(alpha3code char(3) PRIMARY KEY," +
                    "name VARCHAR(100)," +
                    "capital VARCHAR(100)," +
                    "region VARCHAR(50)," +
                    "subregion VARCHAR(50)," +
                    "area DECIMAL(10,2)," +
                    "population INT," +
                    "gini DECIMAL(4,2)," +
                    "nativeName VARCHAR(50))";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();

                sqlcommand = "CREATE TABLE IF NOT EXISTS LANGUAGES(" +
                    "name VARCHAR(100) UNIQUE)";

                command.CommandText = sqlcommand;

                command.ExecuteNonQuery();

                sqlcommand = "CREATE TABLE IF NOT EXISTS COUNTRYLANGUAGES(" +
                    "countrycode CHAR(3)," +
                    "languageName VARCHAR(100)," +
                    "foreign key(countrycode) REFERENCES COUNTRIES(alpha3code)," +
                    "foreign key(languageName) REFERENCES LANGUAGES(name))";

                command.CommandText = sqlcommand;

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {

                dialogService.ShowMessage("Error", e.Message);
            }


        }

        public void SaveData(List<Country> countries)
        {
            try
            {
                foreach (var country in countries)
                {
                    string sql = string.Format("INSERT INTO COUNTRIES" +
                        "(alpha3code, name, capital, region, subregion, area, population, gini, nativeName) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',{6},'{7}','{8}')" +
                        "", country.alpha3Code, country.name.Replace("'", "+"), country.capital.Replace("'", "+"), country.region, country.subregion, country.area, country.population, country.gini, country.nativeName.Replace("'", "+"));

                    command = new SQLiteCommand(sql, connection);

                    command.ExecuteNonQuery();

                    foreach (Language language in country.languages)
                    {
                        if(language.name != null)
                        {
                            sql = $"INSERT OR IGNORE INTO LANGUAGES" +
                            $"(name) VALUES('{language.name.Replace("'","+")}')";

                            command.CommandText = sql;

                            command.ExecuteNonQuery();

                            sql = $"INSERT INTO COUNTRYLANGUAGES(" +
                                $"countrycode, languageName) VALUES('{country.alpha3Code}','{language.name.Replace("'", "+")}')";

                            command.CommandText = sql;

                            command.ExecuteNonQuery();
                        }
                        
                        
                    }
                }

                connection.Close();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        public void DeleteData()
        {
            try
            {
                string sql = "DELETE FROM COUNTRIES";

                command = new SQLiteCommand(sql, connection);

                command.ExecuteNonQuery();

                sql = "DELETE FROM LANGUAGES";

                command.CommandText = sql;

                command.ExecuteNonQuery();

                sql = "DELETE FROM COUNTRYLANGUAGES";

                command.CommandText = sql;

                command.ExecuteNonQuery();
            }
            catch (Exception)
            {

            }
            
        }

        public List<Country> GetData()
        {
            List<Country> countries = new List<Country>();

            try
            {
                string sql = "SELECT alpha3code, name, capital, region, subregion, area, population, gini, nativeName FROM COUNTRIES";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    //MessageBox.Show(Convert.ToDouble((decimal)reader["gini"]).ToString());
                    countries.Add(new Country {
                    alpha3Code = (string)reader["alpha3code"],
                    name = (string)reader["name"],
                    capital = (string)reader["capital"],
                    region = (string)reader["region"],
                    subregion = (string)reader["subregion"],
                    area = Convert.ToDouble((decimal)reader["area"]),
                    population = (int)reader["population"],
                    gini = Convert.ToDouble((decimal)reader["gini"]),
                    nativeName = (string)reader["nativeName"],
                    languages = new List<Language>()
                    });
                }

                CountryLanguages(countries);

                connection.Close();

                return countries;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void CountryLanguages(List<Country> countries)
        {
            foreach (Country country in countries)
            {
                string sql = $"SELECT DISTINCT LANGUAGES.name FROM COUNTRYLANGUAGES INNER JOIN LANGUAGES ON languageName = LANGUAGES.name INNER JOIN COUNTRIES ON countrycode = alpha3code WHERE alpha3code = \"{country.alpha3Code}\"";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    country.languages.Add(new Language {
                            
                        name = (string)reader["name"]
                    });
                }
            }
        }
    }
}
