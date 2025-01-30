using TaskManagerMiac.Interfaces;

namespace TaskManagerMiac.Models
{
    public class FinancialTaskDocument : IDocument
    {
        public FinancialTaskDocument()
        {

        }
        public FinancialTaskDocument(string product, string oKPD, string price, string amount, string deliverPlace, string deliverDate, string guarantee, string notes)
        {
            Product = product;
            OKPD = oKPD;
            Price = price;
            Amount = amount;
            DeliverPlace = deliverPlace;
            DeliverDate = deliverDate;
            Guarantee = guarantee;
            Notes = notes;
        }
        public int IdDocument { get; set; }
        public string DocumentName => "Call.html";

        public string Product { get; set; }
        public string OKPD { get; set; }
        public string Price { get; set; }
        public string Amount { get; set; }
        public string DeliverPlace { get; set; }
        public string DeliverDate { get; set; }
        public string Guarantee { get; set; }
        public string Notes { get; set; }
        public string KVR { get; set; } = " ";
        public string KBK { get; set; } = " ";
        public string Law { get; set; } = " ";
        public string FinanceSource { get; set; } = " ";
        public Dictionary<string, string> GetParameters()
        {
            return new Dictionary<string, string>
            {
                { "BUY_OBJECT", Product },
                { "OKPD_OBJECT", OKPD },
                { "PRICE_OBJECT", Price },
                { "AMOUNT_OBJECT", Amount },
                { "ORDER_PLACE", DeliverPlace },
                { "WHEN_DELIVER", DeliverDate },
                { "GUARANTEE", Guarantee },
                { "NOTES_OBJECT", Notes },
                { "KVR", KVR },
                { "KBK", KBK },
                { "LAW", Law },
                { "FINANCE_SOURCE", FinanceSource },
            };
        }
        public virtual Document IdDocumentNavigation { get; set; }
    }
}
