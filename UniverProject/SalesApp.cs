namespace UniverProject;

using System;

public class SalesApp
{
    private SalesAppDatabase database;

    public SalesApp()
    {
        database = new SalesAppDatabase();
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Create Customer");
            Console.WriteLine("2. Create Article");
            Console.WriteLine("3. Create Sales Invoice");
            Console.WriteLine("4. Create Tables(pick it if its yours first time)");
            Console.WriteLine("5. Exit");

            string option = Console.ReadLine();
            Console.WriteLine();

            switch (option)
            {
                case "1":
                    CreateCustomer();
                    break;
                case "2":
                    CreateArticle();
                    break;
                case "3":
                    CreateSalesInvoice();
                    break;
                case "5":
                    Console.WriteLine("Exiting...");
                    return;
                case "4":
                   database.CreateTables();
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

            Console.WriteLine();
        }
    }

    private void CreateCustomer()
    {
        Console.WriteLine("Enter Customer Details:");
        Console.Write("Name: ");
        string name = Console.ReadLine();
        Console.Write("Contact Number: ");
        string contactNumber = Console.ReadLine();
        Console.Write("Email: ");
        string email = Console.ReadLine();
        Console.Write("Address: ");
        string address = Console.ReadLine();

        Customer customer = new Customer
        {
            CustomerName = name,
            ContactNumber = contactNumber,
            Email = email,
            Address = address
        };

        int customerId = database.CreateCustomer(customer);
        Console.WriteLine($"Customer created with ID: {customerId}");
    }

    private void CreateArticle()
    {
        Console.WriteLine("Enter Article Details:");
        Console.Write("Name: ");
        string name = Console.ReadLine();
        Console.Write("Price: ");
        decimal price = Convert.ToDecimal(Console.ReadLine());
        Console.Write("Quantity in Stock: ");
        int quantity = Convert.ToInt32(Console.ReadLine());

        Article article = new Article
        {
            ArticleName = name,
            Price = price,
            QuantityInStock = quantity
        };

        int articleId = database.CreateArticle(article);
        Console.WriteLine($"Article created with ID: {articleId}");
    }

    private void CreateSalesInvoice()
    {
        Console.WriteLine("Enter Sales Invoice Details:");
        Console.Write("Customer ID: ");
        int customerId = Convert.ToInt32(Console.ReadLine());
        Console.Write("Invoice Date: ");
        DateTime invoiceDate = Convert.ToDateTime(Console.ReadLine());

        SalesInvoice invoice = new SalesInvoice
        {
            Customer = database.GetCustomer(customerId),
            InvoiceDate = invoiceDate,
            Articles = new List<Article>()
        };

        bool addMoreArticles = true;
        while (addMoreArticles)
        {
            Console.WriteLine("Add Article to Invoice:");
            Console.Write("Article ID: ");
            int articleId = Convert.ToInt32(Console.ReadLine());
            Article article = database.GetArticle(articleId);
            if (article != null)
            {
                invoice.Articles.Add(article);
                Console.WriteLine("Article added to the invoice.");
            }
            else
            {
                Console.WriteLine("Invalid Article ID. Please try again.");
            }

            Console.Write("Add another article? (Y/N): ");
            string choice = Console.ReadLine();
            addMoreArticles = (choice.ToLower() == "y");
        }

        int invoiceId = database.CreateSalesInvoice(invoice);
        Console.WriteLine($"Sales Invoice created with ID: {invoiceId}");
    }
}
