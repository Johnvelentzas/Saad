
using Models;
using Producion_Line_Manager.Helpers;
using System.Net.Http.Json;

namespace Producion_Line_Manager.Services
{
    public class RestService
    {
        HttpClient _client;

        public RestService()
        {
            _client = new HttpClient();
        }


        /// <summary>
        /// Retrieves all items of the specified type that match the given or default request parameters.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve. Must implement the IEntity interface.</typeparam>
        /// <param name="parameters">The parameters used to filter or modify the request. If null, default parameters are used.</param>
        /// <returns>A RequestResult&lt;T&gt; with the matching items, or null if no items are found.</returns>
        public async Task<RequestResult<T>?> Get<T>(RequestParameters? parameters = null)
            where T : class, IEntity
        {
            parameters ??= new();
            return await Get<RequestResult<T>>(parameters.BuildURI(URI.GetURI<T>()));
        }

        // returns the T item with the specified id
        public async Task<T?> Get<T>(int id)
            where T : class, IEntity
        {
            return await Get<T>(URI.GetURI<T>(id));
        }


        // returns all A items that belong to the T item with the specified id
        public async Task<RequestResult<A>?> Get<T, A>(int id, RequestParameters? parameters = null)
            where T : class, IEntity
            where A : class, IEntity
        {
            parameters ??= new();
            return await Get<RequestResult<A>>(parameters.BuildURI(URI.GetURI<T, A>(id)));
        }



        // returns the result of a GET request to the specified URI, deserialized as an object of type T
        private async Task<T?> Get<T>(string uri)
            where T : class
        {
            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            throw new Exception($"Failed to retrieve data from {uri}. Status code: {response.StatusCode}");

        }

        public async Task Put<T>(T item)
            where T : class, IEntity
        {
            var response = await _client.PutAsJsonAsync(URI.GetURI<T>(item.Id), item);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to update item with id {item.Id}. Status code: {response.StatusCode}");
            }
        }

        public async Task Post<T>(T item)
            where T : class, IEntity
        {
            var response = await _client.PostAsJsonAsync(URI.GetURI<T>(), item);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to create item. Status code: {response.StatusCode}");
            }
        }
    }

    public static class URI
    {
        public const string RootURI = "http://localhost:5167/api";

        public const string Areas = $"/areas";
        public const string AtributeValues = $"/atributevalues";
        public const string Categories = $"/categories";
        public const string Customers = $"/customers";
        public const string Models = $"/models";
        public const string Orders = $"/orders";
        public const string Patterns = $"/patterns";
        public const string Processes = $"/processes";
        public const string Products = $"/products";
        public const string TaskAtributes = $"/taskatributes";
        public const string TaskDependencies = $"/taskdependencies";
        public const string Tasks = $"/tasks";
        public const string UserProcesses = $"/userprocesses";
        public const string Users = $"/users";

        private static string GetURI(string typeName)
        {
            return typeName.ToLower() switch
            {
                "patternareas" => Areas,
                "atributevalues" => AtributeValues,
                "productcategories" => Categories,
                "customers" => Customers,
                "models" => Models,
                "orders" => Orders,
                "patterns" => Patterns,
                "processes" => Processes,
                "products" => Products,
                "taskatributes" => TaskAtributes,
                "taskdependencies" => TaskDependencies,
                "tasks" => Tasks,
                "userprocesses" => UserProcesses,
                "users" => Users,
                _ => $"/{typeName.ToLower()}"
            };
        }

        public static string GetURI<T>() where T : class, IEntity
        {
            return $"{RootURI}{GetURI(typeof(T).Name)}";
        }

        public static string GetURI<T>(int id) where T : class, IEntity
        {
            return GetURI<T>() + $"/{id}";
        }

        public static string GetURI<T, A>(int id)
            where T : class, IEntity
            where A : class, IEntity
        {
            return GetURI<T>(id) + GetURI(typeof(A).Name);
        }
    }
}
