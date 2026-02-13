using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Vaelastrasz.Library.Configurations;
using Vaelastrasz.Library.Extensions;
using Vaelastrasz.Library.Models;

namespace Vaelastrasz.Library.Services
{
    public class DOIService
    {
        private readonly Configuration _config;
        private HttpClient _client;

        public DOIService(Configuration config)
        {
            _config = config;
            _client = new HttpClient
            {
                BaseAddress = new Uri(_config.Host)
            };

            _client.DefaultRequestHeaders.Add("Vaelastrasz.Library", $"{Assembly.GetExecutingAssembly().GetName().Version.ToString()}");

            if (_config.Username != null && _config.Password != null)
                _client.DefaultRequestHeaders.Authorization = _config.GetBasicAuthenticationHeaderValue();
        }

        public async Task<ApiResponse<ReadDOIModel>> CreateAsync(CreateDOIModel model)
        {
            try
            {
                var response = await _client.PostAsync($"api/dois", model.AsJson());

                if (!response.IsSuccessStatusCode)
                    return ApiResponse<ReadDOIModel>.Failure(await response.Content.ReadAsStringAsync(), response.StatusCode);

                return ApiResponse<ReadDOIModel>.Success(JsonConvert.DeserializeObject<ReadDOIModel>(await response.Content.ReadAsStringAsync()), response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<ReadDOIModel>.Failure(JsonConvert.SerializeObject(ex), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<bool>> DeleteByIdAsync(long id)
        {
            try
            {
                var response = await _client.DeleteAsync($"api/dois/{id}");

                if (!response.IsSuccessStatusCode)
                    return ApiResponse<bool>.Failure(await response.Content.ReadAsStringAsync(), response.StatusCode);

                return ApiResponse<bool>.Success(JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync()), response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure(JsonConvert.SerializeObject(ex), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<List<ReadDOIModel>>> GetAsync()
        {
            try
            {
                var response = await _client.GetAsync($"api/dois");

                if (!response.IsSuccessStatusCode)
                    return ApiResponse<List<ReadDOIModel>>.Failure(await response.Content.ReadAsStringAsync(), response.StatusCode);

                return ApiResponse<List<ReadDOIModel>>.Success(JsonConvert.DeserializeObject<List<ReadDOIModel>>(await response.Content.ReadAsStringAsync()), response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ReadDOIModel>>.Failure(JsonConvert.SerializeObject(ex), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<ReadDOIModel>> GetByIdAsync(long id)
        {
            try
            {
                var response = await _client.GetAsync($"/api/dois/{id}");

                if (!response.IsSuccessStatusCode)
                    return ApiResponse<ReadDOIModel>.Failure(await response.Content.ReadAsStringAsync(), response.StatusCode);

                return ApiResponse<ReadDOIModel>.Success(JsonConvert.DeserializeObject<ReadDOIModel>(await response.Content.ReadAsStringAsync()), response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<ReadDOIModel>.Failure(JsonConvert.SerializeObject(ex), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<string>> GenerateAsync(CreateSuffixModel model)
        {
            try
            {
                var response_prefix = await _client.GetAsync($"api/prefixes");

                if (!response_prefix.IsSuccessStatusCode)
                    return ApiResponse<string>.Failure(await response_prefix.Content.ReadAsStringAsync(), response_prefix.StatusCode);

                var response_suffix = await _client.PostAsync($"api/suffixes", model.AsJson());

                if (!response_suffix.IsSuccessStatusCode)
                    return ApiResponse<string>.Failure(await response_suffix.Content.ReadAsStringAsync(), response_suffix.StatusCode);

                return ApiResponse<string>.Success($"{await response_prefix.Content.ReadAsStringAsync()}/{await response_suffix.Content.ReadAsStringAsync()}", response_suffix.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Failure(JsonConvert.SerializeObject(ex), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<ReadDOIModel>> UpdateAsync(long id, UpdateDOIModel model)
        {
            try
            {
                var response = await _client.PutAsync($"api/datacite/{id}", model.AsJson());

                if (!response.IsSuccessStatusCode)
                    return ApiResponse<ReadDOIModel>.Failure(await response.Content.ReadAsStringAsync(), response.StatusCode);

                return ApiResponse<ReadDOIModel>.Success(JsonConvert.DeserializeObject<ReadDOIModel>(await response.Content.ReadAsStringAsync()), response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<ReadDOIModel>.Failure(JsonConvert.SerializeObject(ex), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<ReadDOIModel>> UpdateAsync(string doi, UpdateDOIModel model)
        {
            try
            {
                var response = await _client.PutAsync($"api/datacite/{doi}", model.AsJson());

                if (!response.IsSuccessStatusCode)
                    return ApiResponse<ReadDOIModel>.Failure(await response.Content.ReadAsStringAsync(), response.StatusCode);

                return ApiResponse<ReadDOIModel>.Success(JsonConvert.DeserializeObject<ReadDOIModel>(await response.Content.ReadAsStringAsync()), response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<ReadDOIModel>.Failure(JsonConvert.SerializeObject(ex), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<ReadDOIModel>> UpdateAsync(string prefix, string suffix, UpdateDOIModel model)
        {
            try
            {
                var response = await _client.PutAsync($"api/datacite/{prefix}/{suffix}", model.AsJson());

                if (!response.IsSuccessStatusCode)
                    return ApiResponse<ReadDOIModel>.Failure(await response.Content.ReadAsStringAsync(), response.StatusCode);

                return ApiResponse<ReadDOIModel>.Success(JsonConvert.DeserializeObject<ReadDOIModel>(await response.Content.ReadAsStringAsync()), response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<ReadDOIModel>.Failure(JsonConvert.SerializeObject(ex), HttpStatusCode.InternalServerError);
            }
        }
    }
}