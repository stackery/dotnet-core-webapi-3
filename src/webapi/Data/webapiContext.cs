using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using webapi.Models;

namespace webapi.Data
{
    public class webapiContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        private Task<GetSecretValueResponse> secretResponseTask;
    
        public webapiContext()
        {
            AmazonSecretsManagerClient client = new AmazonSecretsManagerClient();

            secretResponseTask = client.GetSecretValueAsync(new GetSecretValueRequest
            {
                SecretId = $"{Environment.GetEnvironmentVariable("SECRETS_NAMESPACE")}dbPassword"
            });
        }

        override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            GetSecretValueResponse response = secretResponseTask.Result;

            String dbPassword = response.SecretString;
            String dbAddress = Environment.GetEnvironmentVariable("DB_ADDRESS");
            String dbPort = Environment.GetEnvironmentVariable("DB_PORT");

            optionsBuilder.UseSqlServer($"Data Source={dbAddress},{dbPort};Initial Catalog=books;User ID=root;Password={dbPassword}");
        }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Book> Books { get; set; }
    }
}
