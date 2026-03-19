
using Models.Attributes;
using Models.Finances;
using Models.Production;
using System.Net.Http.Json;

namespace Producion_Line_Manager.Services
{
    public class RestService
    {
        HttpClient _client;
        string _rootURI = "http://localhost:5167";

        public RestService()
        {
            _client = new HttpClient();
        }

        List<AttributeValues> AttributeValues = new();
        List<Models.Attributes.Models> Models = new();
        List<PatternAreas> PatternAreas = new();
        List<Patterns> Patterns = new();
        List<ProductCategories> ProductCategories = new();
        List<TaskAtributes> TaskAtributes = new();
        List<Customers> Customers = new();
        List<Orders> Orders = new();
        List<Processes> Processes = new();
        List<Products> Products = new();
        List<TaskDependencies> TaskDependencies = new();
        List<Tasks> Tasks = new();
        List<UserProcesses> UserProcesses = new();
        List<Users> Users = new();

        public async Task<List<AttributeValues>> GetAttributeValues()
        {
            var uri = $"{_rootURI}/attributevalues";
            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                AttributeValues = await response.Content.ReadFromJsonAsync<List<AttributeValues>>() ?? new();
            }
            return AttributeValues;
        }

        public async Task<List<Users>> GetUsers()
        {
            var uri = $"{_rootURI}/users";
            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                Users = await response.Content.ReadFromJsonAsync<List<Users>>() ?? new();
            }
            return Users;
        }

        public async Task<Users?> GetUser(int id)
        {
            var uri = $"{_rootURI}/users/{id}";
            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Users>();
            }
            return null;
        }

        public async Task<List<Processes>> GetUserProcesses(Users user)
        {
            var uri = $"{_rootURI}/users/{user.Id}/processes";
            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                Processes = await response.Content.ReadFromJsonAsync<List<Processes>>() ?? new();
            }
            return Processes;
        }

    }
}
