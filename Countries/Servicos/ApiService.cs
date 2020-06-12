namespace Countries.Servicos
{
    using Countries.Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ApiService
    {
        public async Task<Response> GetInfo(string urlBase, string controller)
        {
            try
            {
                var client = new HttpClient(); //Fazer ligação via Http
                client.BaseAddress = new Uri(urlBase); //Endereço base da API

                var response = await client.GetAsync(controller); //Controlador da API

                var result = await response.Content.ReadAsStringAsync(); //Guarda as informações da API no objecto result

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result
                    };
                }

                var countries = JsonConvert.DeserializeObject<List<Country>>(result);

                return new Response
                {
                    IsSuccess = true,
                    Result = countries
                };
            }
            catch (Exception ex)
            {

                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
