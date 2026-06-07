
using Models;
using Models.Attributes;
using Models.Management;
using Models.Production;
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
            //throw new Exception($"Failed to retrieve data from {uri}. Status code: {response.StatusCode}");
            return null;
        }

        public async Task Put<T>(T item)
            where T : class, IEntity
        {
            var response = await _client.PutAsJsonAsync(URI.GetURI<T>(item.Id), item);
            if (!response.IsSuccessStatusCode)
            {
                //throw new Exception($"Failed to update item with id {item.Id}. Status code: {response.StatusCode}");
            }
        }

        public async Task<T?> Post<T>(T item)
            where T : class, IEntity
        {
            item.Id = 0;
            var response = await _client.PostAsJsonAsync(URI.GetURI<T>(), item);
            if (!response.IsSuccessStatusCode)
            {
                // THIS is the goldmine! It will contain the exact JSON error from Azure
                string errorDetails = await response.Content.ReadAsStringAsync();

                // Put a breakpoint on this line and inspect 'errorDetails'
                Console.WriteLine(errorDetails);
            }
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            //throw new Exception($"Failed to create item. Status code: {response.StatusCode}");
            return null;
        }

        public async Task Delete<T>(int id)
            where T : class, IEntity
        {
            var response = await _client.DeleteAsync(URI.GetURI<T>(id));
            if (!response.IsSuccessStatusCode)
            {
                //throw new Exception($"Failed to delete item with id {id}. Status code: {response.StatusCode}");
            }
        }

        public async Task DeleteEntity(IEntity entity)
        {
            switch (entity)
            {
                case Customers c:
                    await Delete<Customers>(entity.Id);
                    break;
                case Orders o:
                    await Delete<Orders>(entity.Id);
                    break;
                case Products p:
                    await Delete<Products>(entity.Id);
                    break;
                case Users u:
                    await Delete<Users>(entity.Id);
                    break;
                case Models.Attributes.Models m:
                    await Delete<Models.Attributes.Models>(entity.Id);
                    break;
                case ProductCategories pc:
                    await Delete<ProductCategories>(entity.Id);
                    break;
                case Fabrics pc:
                    await Delete<Fabrics>(entity.Id);
                    break;
                case YarnColors pc:
                    await Delete<YarnColors>(entity.Id);
                    break;
                case StitchTypes pc:
                    await Delete<StitchTypes>(entity.Id);
                    break;
                case Patterns pc:
                    await Delete<Patterns>(entity.Id);
                    break;
                case Tasks t:
                    await Delete<Tasks>(entity.Id);
                    break;
                default:
                    throw new NotImplementedException($"Delete not supported for type {entity.GetType().Name}");
            }
        }


        public async Task<string?> UploadImage(Stream fileStream, string fileName)
        {
            try
            {
                var content = new MultipartFormDataContent();

                // Convert the stream into StreamContent for the HTTP body
                var streamContent = new StreamContent(fileStream);

                // "file" MUST match the name of the IFormFile parameter in your Azure Web API controller!
                content.Add(streamContent, "file", fileName);

                // Send the multipart form data post request to your API
                var response = await _client.PostAsync(URI.GetImagesURI(), content);

                if (response.IsSuccessStatusCode)
                {
                    // Read the raw plain-text Cloudinary URL directly from the response
                    string imageUrl = await response.Content.ReadAsStringAsync();
                    return imageUrl;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload failed inside RestService: {ex.Message}");
                return null;
            }
        }


        public async Task<IEntity?> Sync(IEntity item)
        {
            return item switch
            {
                Customers c => await Get<Customers>(c.Id),
                Orders o => await Get<Orders>(o.Id),
                Products p => await Get<Products>(p.Id),
                Models.Attributes.Models m => await Get<Models.Attributes.Models>(m.Id),
                ProductCategories pc => await Get<ProductCategories>(pc.Id),
                Tasks t => await Get<Tasks>(t.Id),
                Users u => await Get<Users>(u.Id),
                Fabrics u => await Get<Fabrics>(u.Id),
                YarnColors u => await Get<YarnColors>(u.Id),
                StitchTypes u => await Get<StitchTypes>(u.Id),
                Patterns u => await Get<Patterns>(u.Id),
                _ => throw new NotImplementedException($"Update not supported for type {item.GetType().Name}")
            };
        }
    }

    public static class URI
    {
        //public const string RootURI = "http://localhost:5167/api";
        public const string RootURI = "https://saadwebapitest-csacgsbjfqhfhyht.centralus-01.azurewebsites.net/api";

        public const string Categories = $"/categories";
        public const string Customers = $"/customers";
        public const string Models = $"/models";
        public const string Orders = $"/orders";
        public const string Patterns = $"/patterns";
        public const string Processes = $"/processes";
        public const string Products = $"/products";
        public const string TaskDependencies = $"/taskdependencies";
        public const string Tasks = $"/tasks";
        public const string UserProcesses = $"/userprocesses";
        public const string Users = $"/users";
        public const string Fabrics = $"/fabrics";
        public const string StitchTypes = $"/stitchtypes";

        private static string GetURI(string typeName)
        {
            return typeName.ToLower() switch
            {
                "productcategories" => Categories,
                "customers" => Customers,
                "models" => Models,
                "orders" => Orders,
                "patterns" => Patterns,
                "processes" => Processes,
                "products" => Products,
                "taskdependencies" => TaskDependencies,
                "tasks" => Tasks,
                "userprocesses" => UserProcesses,
                "users" => Users,
                "fabrics" => Fabrics,
                "stitchtypes" => StitchTypes,
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

        public static string GetImagesURI()
        {
            return $"{RootURI}/images";
        }
    }
}
