using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyGenerator
{
    public class Service
    {
        private HttpClient client = new HttpClient();
        public async Task<string> AddCat(Cat cat)
        {
            string json = JsonConvert.SerializeObject(cat);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:8080/cat/", content);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetCatById(int id)
        {
            return await client.GetStringAsync($"http://localhost:8080/cat/?id={id}");
        }

        public async Task<string> GetOwnerById(int id)
        {
            return await client.GetStringAsync($"http://localhost:8080/owner/?id={id}");
        }

        public async Task<string> AddOwner(Owner owner)
        {
            string json = JsonConvert.SerializeObject(owner);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:8080/owner/", content);
            return await response.Content.ReadAsStringAsync();
        }
    }
}