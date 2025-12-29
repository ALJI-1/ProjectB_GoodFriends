
using Models.Interfaces;
using Models.Common;

namespace AppMvc.Models
{
   public class FavoriteQuoteIM
    {
        public StatusIM StatusIM { get; set; }
        public Guid QuoteId { get; set; } = Guid.NewGuid();
        public string Quote { get; set; }
        public string Author { get; set; }


        public FavoriteQuoteIM() { StatusIM = StatusIM.Unchanged; }

        public FavoriteQuoteIM(FavoriteQuoteIM original)
        {
            StatusIM = original.StatusIM;

            QuoteId = original.QuoteId;
            Quote = original.Quote;
            Author = original.Author;
        }
        public FavoriteQuoteIM(IQuote quote)
        {
            StatusIM = StatusIM.Unchanged;
            QuoteId = quote.QuoteId;
            Quote = quote.QuoteText;
            Author = quote.Author;
        }
    }
}