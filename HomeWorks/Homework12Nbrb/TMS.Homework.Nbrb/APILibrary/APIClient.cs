﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using APILibrary.Models;


namespace APILibrary
{
    public class APIClient
    {
        /// <summary>
        /// Dictionary for currencies (save code currency and internal code)
        /// </summary>
        private Dictionary<int, int> dictionaryCurrencies;

        public APIClient()
        {
            dictionaryCurrencies = new Dictionary<int, int>();
        }

        /// <summary>
        /// Getting full json file with all currencies
        /// </summary>
        /// <returns>list Currency</returns>
        private async Task<List<Currency>> GetAllCurrenciesAsync()
        {
            //HttpClient httpClient = new HttpClient();
            //string request = "https://www.nbrb.by/api/exrates/currencies";
            //HttpResponseMessage response = (await httpClient.GetAsync(request)).EnsureSuccessStatusCode();
            //string responseBody = await response.Content.ReadAsStringAsync();

            //return JsonConvert.DeserializeObject<List<Currency>>(responseBody);

            using (var httpClient = new HttpClient())
            {
                return await httpClient.GetFromJsonAsync<List<Currency>>("https://www.nbrb.by/api/exrates/currencies");
            }
        }

        /// <summary>
        /// Receiving currency as a short list (input 0 return all list) (Получение списка валют, указанное количество (если парам. 0 вернет весь список))
        /// </summary>
        /// <param name="countCurrencies">Count elements need result</param>
        /// <returns>list short currencies</returns>
        public async Task<List<ShortCurrency>> GetShortCurrenciesAsync(int countCurrencies)
        {
            List<ShortCurrency> vRes = new List<ShortCurrency>();

            var listCurrencies = (await GetAllCurrenciesAsync()).ToList();

            CreateDictionaryCurrencies(listCurrencies);

            return listCurrencies.Where(x => x.Cur_DateEnd > DateTime.Now)
                                .OrderBy(y => y.Cur_Code)
                                .Take(countCurrencies > 0 ? countCurrencies : listCurrencies.Count)
                                .Select(c => new ShortCurrency {
                                    Code = c.Cur_Code,
                                    Abbreviation = c.Cur_Abbreviation,
                                    Name = c.Cur_Name
                                }).ToList();

        }

        /// <summary>
        /// Create dictionary for currencies
        /// </summary>
        /// <param name="listCurrencies">list currencies</param>
        private void CreateDictionaryCurrencies(List<Currency> listCurrencies)
        {
            dictionaryCurrencies.Clear();

            foreach (var currency in listCurrencies.Where(c => c.Cur_DateEnd > DateTime.Now))
            {
                dictionaryCurrencies.Add(currency.Cur_Code, currency.Cur_ID);
            }
        }

        /// <summary>
        /// Task for get Rates
        /// </summary>
        /// <param name="forDate">Date currency</param>
        /// <param name="codeCurrency">Code currency</param>
        /// <returns>Task</returns>
        private async Task<Rate> GetRateOnDateAsync(DateTime forDate, int codeCurrency)
        {
            using (var httpClient = new HttpClient())
            {
                var searchDate = forDate.ToString("yyyy-M-d");
                var searchCode = dictionaryCurrencies.FirstOrDefault(x => x.Key == codeCurrency).Value;

                var request = "https://www.nbrb.by/api/exrates/rates/" + searchCode + "?ondate=" + searchDate;
                return await httpClient.GetFromJsonAsync<Rate>(request);
            }
        }


        /// <summary>
        /// Get rates on date (получение курса валюты на дату)
        /// </summary>
        /// <param name="forDate">Date currency</param>
        /// <param name="codeCurrency">Code currency</param>
        /// <returns>Rate</returns>
        public Task<Rate> GetRatesAsync(DateTime forDate, int codeCurrency) => GetRateOnDateAsync(forDate, codeCurrency);     

        /// <summary>
        /// Get list short rates (получение курса валюты за указанный период)
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="finishDate">Finish date</param>
        /// <param name="codeCurrency">Code currency</param>
        /// <returns>List short rate</returns>
        //public async Task<(List<ShortRate> listShortRate, int codeCurrency)> GetRates(DateTime startDate, DateTime finishDate, int codeCurrency)
        public (List<ShortRate> listShortRate, int codeCurrency) GetRates(DateTime startDate, DateTime finishDate, int codeCurrency)
        {
            try
            {
                SynchronizationContext.SetSynchronizationContext(null);
                //return (await GetRatesOnPeriod(startDate, finishDate, codeCurrency), codeCurrency);
                return (GetRatesOnPeriod(startDate, finishDate, codeCurrency).Result, codeCurrency);
            }
            catch
            {
                return (null, codeCurrency);
            }
        }

        /// <summary>
        /// Task for get list rates
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="finishDate">Finish date</param>
        /// <param name="codeCurrency">Code currency</param>
        /// <returns>Task</returns>
        private async Task<List<ShortRate>> GetRatesOnPeriod(DateTime startDate, DateTime finishDate, int codeCurrency)
        {
            using (var httpClient = new HttpClient())
            {
                var searchFirstDate = startDate.ToString("yyyy-M-d");
                var searchFinishDate = finishDate.ToString("yyyy-M-d");
                var searchCode = dictionaryCurrencies.FirstOrDefault(x => x.Key == codeCurrency).Value;

                var request = "https://www.nbrb.by/API/ExRates/Rates/Dynamics/" + searchCode + "?startDate=" + searchFirstDate + "&endDate=" + searchFinishDate;
                return await httpClient.GetFromJsonAsync<List<ShortRate>>(request);
            }
        }
    }

    
}