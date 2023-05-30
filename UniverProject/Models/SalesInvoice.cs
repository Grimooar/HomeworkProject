namespace UniverProject;

public class SalesInvoice
{
    public int InvoiceId { get; set; }
    public Customer Customer { get; set; }
    public DateTime InvoiceDate { get; set; }
    public List<Article> Articles { get; set; }
}