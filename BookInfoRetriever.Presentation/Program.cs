using BookInfoRetriever.Core.Emuns;
using BookInfoRetriever.Core.Entities;
using BookInfoRetriever.Infrastructure.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookInfoRetriever.Presentation
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
           
            string inputFile = "../../Files/ISBN_Input_File.txt";
            string[] isbnNumbers = File.ReadAllLines(inputFile);
            
            string outputFile = "output.csv";
            using (var writer = new StreamWriter(outputFile))
            {
                writer.WriteLine("Row Number,Data Retrieval Type,ISBN,Title,Subtitle,Author Name(s),Number of Pages,Publish Date");

                var cache = new Dictionary<string, BookInfo>();
                

                for (int i = 0; i < isbnNumbers.Length; i++)
                {
                    string[] isbnArray = isbnNumbers[i].Split(',');
                    int rowNumber = i + 1;

                    for (int j = 0; j < isbnArray.Length; j++)
                    {
                        string isbn = isbnArray[j];
                        if (cache.ContainsKey(isbn))
                        {
                            var bookInfo = cache[isbn];
                            WriteToCsv(writer, rowNumber, DataRetrievalType.Cache.ToString(), isbn, bookInfo);
                        }
                        else
                        {
                            var bookInfo = await BookInfoService.RetrieveBookInfoFromAPI(isbn);
                            WriteToCsv(writer, rowNumber, DataRetrievalType.Server.ToString(), isbn, bookInfo);
                            cache[isbn] = bookInfo;
                        }
                    }
                    
                }
            }
        }

        private static void WriteToCsv(StreamWriter writer, int rowNumber, string dataRetrievalType, string isbn, BookInfo bookInfo)
        {
            string publishDate = $"\"{bookInfo.PublishDate.Replace("\"", "\"\"")}\"";
            string authors = string.Join(";", bookInfo.Authors.Select(author => $"\"{author.Replace("\"", "\"\"")}\""));
            writer.WriteLine($"{rowNumber},{dataRetrievalType},{isbn},{bookInfo.Title},{bookInfo.Subtitle},{authors},{bookInfo.NumberOfPages},{publishDate}");
        }
    }
    
}
