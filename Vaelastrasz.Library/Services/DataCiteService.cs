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
using Vaelastrasz.Library.Types;

namespace Vaelastrasz.Library.Services
{
    public class DataCiteService
    {
        private readonly Configuration _config;
        private HttpClient _client;

        public DataCiteService(Configuration config)
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

        public async Task<ApiResponse<ReadDataCiteModel>> CreateAsync(CreateDataCiteModel model)
        {
            try
            {
                var response = await _client.PostAsync($"api/datacite", model.AsJson());

                if (!response.IsSuccessStatusCode)
                    return ApiResponse<ReadDataCiteModel>.Failure(await response.Content.ReadAsStringAsync(), response.StatusCode);

                return ApiResponse<ReadDataCiteModel>.Success(JsonConvert.DeserializeObject<ReadDataCiteModel>(await response.Content.ReadAsStringAsync()), response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<ReadDataCiteModel>.Failure(JsonConvert.SerializeObject(ex), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(string doi)
        {
            try
            {
                var response = await _client.DeleteAsync($"api/datacite/{doi}");

                if (!response.IsSuccessStatusCode)
                    return ApiResponse<bool>.Failure(await response.Content.ReadAsStringAsync(), response.StatusCode);

                return ApiResponse<bool>.Success(JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync()), response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure(JsonConvert.SerializeObject(ex), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<List<ReadDataCiteModel>>> GetAsync()
        {
            try
            {
                var response = await _client.GetAsync($"api/datacite");

                if (!response.IsSuccessStatusCode)
                    return ApiResponse<List<ReadDataCiteModel>>.Failure(await response.Content.ReadAsStringAsync(), response.StatusCode);

                return ApiResponse<List<ReadDataCiteModel>>.Success(JsonConvert.DeserializeObject<List<ReadDataCiteModel>>(await response.Content.ReadAsStringAsync()), response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ReadDataCiteModel>>.Failure(JsonConvert.SerializeObject(ex), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<ReadDataCiteModel>> GetByDoiAsync(string doi)
        {
            try
            {
                var response = await _client.GetAsync($"api/datacite/{doi}");

                if (!response.IsSuccessStatusCode)
                    return ApiResponse<ReadDataCiteModel>.Failure(await response.Content.ReadAsStringAsync(), response.StatusCode);

                return ApiResponse<ReadDataCiteModel>.Success(JsonConvert.DeserializeObject<ReadDataCiteModel>(await response.Content.ReadAsStringAsync()), response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<ReadDataCiteModel>.Failure(JsonConvert.SerializeObject(ex), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<string>> GetCitationStyleByDoiAsync(string doi, DataCiteCitationStyleType citationStyle)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"api/datacite/{doi}/citations");
                request.Headers.Add("X-Citation-Style", EnumExtensions.GetEnumMemberValue(citationStyle));

                var response = await _client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return ApiResponse<string>.Failure(await response.Content.ReadAsStringAsync(), response.StatusCode);

                return ApiResponse<string>.Success(await response.Content.ReadAsStringAsync(), response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Failure(JsonConvert.SerializeObject(ex), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<string>> GetMetadataFormatByDoiAsync(string doi, DataCiteMetadataFormatType metadataFormat)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"api/datacite/{doi}/metadata");
                request.Headers.Add("X-Metadata-Format", EnumExtensions.GetEnumMemberValue(metadataFormat));

                var response = await _client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return ApiResponse<string>.Failure(await response.Content.ReadAsStringAsync(), response.StatusCode);

                return ApiResponse<string>.Success(await response.Content.ReadAsStringAsync(), response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Failure(JsonConvert.SerializeObject(ex), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<ReadDataCiteModel>> UpdateAsync(string doi, UpdateDataCiteModel model)
        {
            try
            {
                var response = await _client.PutAsync($"api/datacite/{doi}", model.AsJson());

                if (!response.IsSuccessStatusCode)
                    return ApiResponse<ReadDataCiteModel>.Failure(await response.Content.ReadAsStringAsync(), response.StatusCode);

                return ApiResponse<ReadDataCiteModel>.Success(JsonConvert.DeserializeObject<ReadDataCiteModel>(await response.Content.ReadAsStringAsync()), response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<ReadDataCiteModel>.Failure(JsonConvert.SerializeObject(ex), HttpStatusCode.InternalServerError);
            }
        }
    }
}