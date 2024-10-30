using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

public class Program
{
    public static async Task Main(string[] args)
    {
        KeyValutConfiguration keyValutConfiguration = new KeyValutConfiguration();
        await keyValutConfiguration.GetKeyVaultSecretAsync();
    }
}

public class KeyValutConfiguration
{
    private const string keyVaultUrl = "<keyVaultUrl>";
    public async Task GetKeyVaultSecretAsync()
    {
        // Directly provide credentials
        var clientId = "<ClientId>";
        var clientSecret = "<ClientSecret>";
        var tenantId = "<TenantId>";

        // Create a ClientSecretCredential using the provided credentials
        var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        // Create a SecretClient to interact with Key Vault
        var client = new SecretClient(new Uri(keyVaultUrl), clientSecretCredential);

        // Specify the name of the secret you want to retrieve
        string secretName = string.Empty;
        try
        {
            #region "using foreach Loop"
            // List all secrets in the Key Vault
            await foreach (var secretProperties in client.GetPropertiesOfSecretsAsync())
            {
                // Retrieve each secret value using its name
                KeyVaultSecret secret1 = await client.GetSecretAsync(secretProperties.Name);
                Console.WriteLine($"Secret Name: {secret1.Name}, Secret Value: {secret1.Value}");
            }
            #endregion

            #region"Find One by One"
            secretName = "consStr";
            // Retrieve the secret from Key Vault
            KeyVaultSecret secret = await client.GetSecretAsync(secretName);
            Console.WriteLine($"Secret Value: {secret.Value}");
            secretName = "ServiceBusConnection";
            secret = await client.GetSecretAsync(secretName);
            Console.WriteLine($"Secret Value: {secret.Value}");
            #endregion
        }
        catch (RequestFailedException ex) when (ex.Status == 403)
        {
            Console.WriteLine("Access denied. Ensure your application has permission to read secrets.");
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            Console.WriteLine($"Secret '{secretName}' not found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving secret: {ex.Message}");
        }
    }
}