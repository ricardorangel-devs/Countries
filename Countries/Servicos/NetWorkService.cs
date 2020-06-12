namespace Countries.Servicos
{
    using Countries.Models;
    using System.Net;

    public class NetWorkService
    {
        public Response CheckConnection()
        {
            var client = new WebClient();

            try
            {
                using (client.OpenRead("http://clients3.google.com/"))
                {
                    return new Response
                    {
                        IsSuccess = true
                    };
                }
            }
            catch
            {

                return new Response
                {
                    IsSuccess = false,
                    Message = "Configure a sua ligação á Internet"
                };
            }
        }
    }
}
