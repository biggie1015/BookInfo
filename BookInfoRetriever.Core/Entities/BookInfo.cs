using System.Collections.Generic;


namespace BookInfoRetriever.Core.Entities
{
    public class BookInfo
    {

        public BookInfo()
        {
            Authors= new List<string>();
        }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public List<string> Authors { get; set; }
        public int NumberOfPages { get; set; }
        public string PublishDate { get; set; }
    }
}
