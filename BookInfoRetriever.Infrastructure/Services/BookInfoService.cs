using BookInfoRetriever.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookInfoRetriever.Infrastructure.Services
{
    public class BookInfoService
    {
        public static async Task<BookInfo> RetrieveBookInfoFromAPI(string isbn)
        {
            string[] isbnArray = isbn.Split(',');
            try
            {
                using (var httpClient = new HttpClient())
                {
                    List<BookInfo> bookInfos = new List<BookInfo>();

                    foreach (var item in isbnArray)
                    {
                        string apiUrl = $"https://openlibrary.org/api/books?bibkeys=ISBN:{item}&format=json&jscmd=data";
                        var response = await httpClient.GetAsync(apiUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            using (JsonDocument document = JsonDocument.Parse(json))
                            {
                                if (document.RootElement.TryGetProperty($"ISBN:{item}", out JsonElement bookDetails))
                                {
                                    var bookInfo = CreateBookInfoFromJsonElement(bookDetails);
                                    bookInfos.Add(bookInfo);
                                }
                                else if (document.RootElement.TryGetProperty(item, out bookDetails))
                                {
                                    var bookInfo = CreateBookInfoFromJsonElement(bookDetails);
                                    bookInfos.Add(bookInfo);
                                }
                            }
                        }
                    }

                    if (bookInfos.Count > 0)
                    {
                        return bookInfos[0];
                    }

                    throw new Exception($"Book details not found ISBNs: {isbn}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving book information from API for ISBN: {isbn}", ex);
            }
        }




        private static BookInfo CreateBookInfoFromJsonElement(JsonElement bookDetails)
        {
            var bookInfo = new BookInfo
            {
                Title = bookDetails.TryGetProperty("title", out JsonElement titleProperty) ? titleProperty.GetString() : "N/A",
                Subtitle = bookDetails.TryGetProperty("subtitle", out JsonElement subtitleProperty) ? subtitleProperty.GetString() : "N/A",
                Authors = bookDetails.TryGetProperty("authors", out JsonElement authorsProperty)
                                        ? authorsProperty.EnumerateArray().Select(a => a.GetProperty("name").GetString()).ToList()
                                        : new List<string>(),
                NumberOfPages = bookDetails.TryGetProperty("number_of_pages", out JsonElement pagesProperty)
                                        ? pagesProperty.GetInt32()
                                        : 0,
                PublishDate = bookDetails.GetProperty("publish_date").GetString()
            };

            return bookInfo;
        }
    }
}
