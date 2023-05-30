namespace UniverProject;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class SalesAppDatabase
{
    private string connectionString = "Data Source=localhost\\MYSQLS;Initial Catalog=ProjectDb;Integrated Security=True;"; // Replace with your SQL Server connection string

    public void CreateTables()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Create Customers table
            string createCustomersTableQuery =
                "CREATE TABLE Customers (CustomerId INT PRIMARY KEY IDENTITY(1,1), CustomerName VARCHAR(100), ContactNumber VARCHAR(20), Email VARCHAR(100), Address VARCHAR(200));";
            SqlCommand createCustomersTableCommand = new SqlCommand(createCustomersTableQuery, connection);
            createCustomersTableCommand.ExecuteNonQuery();

            // Create Articles table
            string createArticlesTableQuery =
                "CREATE TABLE Articles (ArticleId INT PRIMARY KEY IDENTITY(1,1), ArticleName VARCHAR(100), Price DECIMAL(10, 2), QuantityInStock INT);";
            SqlCommand createArticlesTableCommand = new SqlCommand(createArticlesTableQuery, connection);
            createArticlesTableCommand.ExecuteNonQuery();

            // Create SalesInvoices table
            string createSalesInvoicesTableQuery =
                "CREATE TABLE SalesInvoices (InvoiceId INT PRIMARY KEY IDENTITY(1,1), CustomerId INT, InvoiceDate DATETIME, FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId));";
            SqlCommand createSalesInvoicesTableCommand = new SqlCommand(createSalesInvoicesTableQuery, connection);
            createSalesInvoicesTableCommand.ExecuteNonQuery();

            // Create SalesInvoiceArticles junction table
            string createJunctionTableQuery =
                "CREATE TABLE SalesInvoiceArticles (InvoiceId INT, ArticleId INT, FOREIGN KEY (InvoiceId) REFERENCES SalesInvoices(InvoiceId), FOREIGN KEY (ArticleId) REFERENCES Articles(ArticleId));";
            SqlCommand createJunctionTableCommand = new SqlCommand(createJunctionTableQuery, connection);
            createJunctionTableCommand.ExecuteNonQuery();

            Console.WriteLine("Tables created successfully.");
        }
    }

    public int CreateCustomer(Customer customer)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO Customers (CustomerName, ContactNumber, Email, Address) VALUES (@Name, @ContactNumber, @Email, @Address); SELECT SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", customer.CustomerName);
            command.Parameters.AddWithValue("@ContactNumber", customer.ContactNumber);
            command.Parameters.AddWithValue("@Email", customer.Email);
            command.Parameters.AddWithValue("@Address", customer.Address);
            int customerId = Convert.ToInt32(command.ExecuteScalar());
            return customerId;
        }
    }

    public int CreateArticle(Article article)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO Articles (ArticleName, Price, QuantityInStock) VALUES (@Name, @Price, @Quantity); SELECT SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", article.ArticleName);
            command.Parameters.AddWithValue("@Price", article.Price);
            command.Parameters.AddWithValue("@Quantity", article.QuantityInStock);
            int articleId = Convert.ToInt32(command.ExecuteScalar());
            return articleId;
        }
    }

    public int CreateSalesInvoice(SalesInvoice invoice)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            // Insert sales invoice details
            string invoiceQuery = "INSERT INTO SalesInvoices (CustomerId, InvoiceDate) VALUES (@CustomerId, @InvoiceDate); SELECT SCOPE_IDENTITY();";
            SqlCommand invoiceCommand = new SqlCommand(invoiceQuery, connection);
            invoiceCommand.Parameters.AddWithValue("@CustomerId", invoice.Customer.CustomerId);
            invoiceCommand.Parameters.AddWithValue("@InvoiceDate", invoice.InvoiceDate);
            int invoiceId = Convert.ToInt32(invoiceCommand.ExecuteScalar());

            // Insert article details into the junction table
            string junctionQuery = "INSERT INTO SalesInvoiceArticles (InvoiceId, ArticleId) VALUES (@InvoiceId, @ArticleId);";
            SqlCommand junctionCommand = new SqlCommand(junctionQuery, connection);
            junctionCommand.Parameters.AddWithValue("@InvoiceId", invoiceId);
            foreach (Article article in invoice.Articles)
            {
                junctionCommand.Parameters.Clear();
                junctionCommand.Parameters.AddWithValue("@InvoiceId", invoiceId);
                junctionCommand.Parameters.AddWithValue("@ArticleId", article.ArticleId);
                junctionCommand.ExecuteNonQuery();
            }

            return invoiceId;
        }
    }

    public Customer GetCustomer(int customerId)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT CustomerId, CustomerName, ContactNumber, Email, Address FROM Customers WHERE CustomerId = @CustomerId;";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CustomerId", customerId);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Customer customer = new Customer();
                customer.CustomerId = Convert.ToInt32(reader["CustomerId"]);
                customer.CustomerName = reader["CustomerName"].ToString();
                customer.ContactNumber = reader["ContactNumber"].ToString();
                customer.Email = reader["Email"].ToString();
                customer.Address = reader["Address"].ToString();
                return customer;
            }
            return null;
        }
    }

    public Article GetArticle(int articleId)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT ArticleId, ArticleName, Price, QuantityInStock FROM Articles WHERE ArticleId = @ArticleId;";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ArticleId", articleId);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Article article = new Article();
                article.ArticleId = Convert.ToInt32(reader["ArticleId"]);
                article.ArticleName = reader["ArticleName"].ToString();
                article.Price = Convert.ToDecimal(reader["Price"]);
                article.QuantityInStock = Convert.ToInt32(reader["QuantityInStock"]);
                return article;
            }
            return null;
        }
    }

    public SalesInvoice GetSalesInvoice(int invoiceId)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string invoiceQuery = "SELECT InvoiceId, CustomerId, InvoiceDate FROM SalesInvoices WHERE InvoiceId = @InvoiceId;";
            SqlCommand invoiceCommand = new SqlCommand(invoiceQuery, connection);
            invoiceCommand.Parameters.AddWithValue("@InvoiceId", invoiceId);
            SqlDataReader invoiceReader = invoiceCommand.ExecuteReader();
            if (invoiceReader.Read())
            {
                SalesInvoice invoice = new SalesInvoice();
                invoice.InvoiceId = Convert.ToInt32(invoiceReader["InvoiceId"]);
                invoice.Customer = GetCustomer(Convert.ToInt32(invoiceReader["CustomerId"]));
                invoice.InvoiceDate = Convert.ToDateTime(invoiceReader["InvoiceDate"]);

                string junctionQuery = "SELECT ArticleId FROM SalesInvoiceArticles WHERE InvoiceId = @InvoiceId;";
                SqlCommand junctionCommand = new SqlCommand(junctionQuery, connection);
                junctionCommand.Parameters.AddWithValue("@InvoiceId", invoiceId);
                SqlDataReader junctionReader = junctionCommand.ExecuteReader();
                invoice.Articles = new List<Article>();
                while (junctionReader.Read())
                {
                    Article article = GetArticle(Convert.ToInt32(junctionReader["ArticleId"]));
                    if (article != null)
                    {
                        invoice.Articles.Add(article);
                    }
                }

                return invoice;
            }
            return null;
        }
    }
}
