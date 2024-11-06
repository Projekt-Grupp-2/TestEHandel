using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using TestEHandel.Models;

namespace TestEHandel.Service
{
    public class ProduktService(HttpClient httpClient, IConfiguration configuration, CartService cartService)
    {


        private readonly HttpClient _httpClient = httpClient;
        private IConfiguration _configuration = configuration;
        private string BaseServerUrl;
        private readonly CartService _cartService;

        public event Action Onchange;


        public async Task<IEnumerable<ProduktDTO>> GetAll()
        {
            var response = await _httpClient.GetAsync("https://localhost:7169/api/Product");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<IEnumerable<ProduktDTO>>(content);

                return products;
            }

            return new List<ProduktDTO>();

        }

        public async Task<ProduktDTO> Get(Guid productId)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7169/api/Product/{productId}");
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {

                var product = JsonConvert.DeserializeObject<ProduktDTO>(content);

                return product;
            }
            else
            {
                var errorModel = JsonConvert.DeserializeObject<ErrorModelDTO>(content);
                throw new Exception(errorModel.ErrorMessage);
            }
        }





        public async Task<ShoppingDto> BuyProduct(Guid productId, Guid userId)
        {
            try
            {
                // Hämta befintlig kundvagn för användaren
                var existingCartResponse = await _httpClient.GetAsync($"https://localhost:7197/api/Shopping/GetUserId/{userId}");

                ShoppingDto shoppingDto;

                if (existingCartResponse.IsSuccessStatusCode)
                {
                    // Om kundvagnen finns, deserialisera den
                    var existingCartContent = await existingCartResponse.Content.ReadAsStringAsync();
                    shoppingDto = JsonConvert.DeserializeObject<ShoppingDto>(existingCartContent);

                    // Kontrollera om produkten redan finns i kundvagnen




                    var existingItem = shoppingDto.Items.FirstOrDefault(i => i.ProductId == productId);
                    if (existingItem != null)
                    {
                        // Uppdatera kvantiteten om produkten redan finns
                        existingItem.Quantity += 1;
                    }
                    else
                    {
                        // Lägg till den nya produkten i Items-listan
                        shoppingDto.Items.Add(new CartItemDto
                        {



                            ProductId = productId,
                            Quantity = 1
                        });
                    }
                }
                else
                {
                    // Om ingen kundvagn finns, skapa en ny med produkten
                    shoppingDto = new ShoppingDto
                    {
                        ShoppingId = Guid.NewGuid(), // Skapa ett nytt ShoppingId
                        UserId = userId,
                        Items = new List<CartItemDto>() // Initierar en tom lista
                    };

                    // Skapa ett nytt CartItemDto och tilldela ShoppingId från shoppingDto
                    var cartItem = new CartItemDto
                    {

                        ShoppingId = shoppingDto.ShoppingId, // Använd ShoppingId från den nyskapade shoppingDto
                        ProductId = productId,
                        Quantity = 1
                    };

                    // Lägg till det nya CartItemDto till Items-listan på shoppingDto
                    shoppingDto.Items.Add(cartItem);



                    var content = JsonConvert.SerializeObject(shoppingDto);
                    var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
                    Console.WriteLine("Body content JSON: " + content);

                    // Skicka POST-förfrågan för att skapa eller uppdatera kundvagnen //Felet är här!
                    var response = await _httpClient.PostAsync("https://localhost:7197/api/Shopping/Create", bodyContent);

                }

                // Serialisera och skicka den uppdaterade kundvagnen till API:t
                var updatecontent = JsonConvert.SerializeObject(shoppingDto);
                var updatebodyContent = new StringContent(updatecontent, Encoding.UTF8, "application/json");
                Console.WriteLine("Body content JSON: " + updatecontent);

                // Skicka POST-förfrågan för att skapa eller uppdatera kundvagnen //Felet är här!
                var updateresponse = await _httpClient.PostAsync($"https://localhost:7197/api/Shopping/Update/{userId}", updatebodyContent); ///Shopping/Update/{userId} //Id hittas ej för den letar efter Shopping ID. 
                Onchange.Invoke();
                // Kontrollera om förfrågan var framgångsrik
                if (updateresponse.IsSuccessStatusCode)
                {
                    var responseResult = await updateresponse.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ShoppingDto>(responseResult);
                    Onchange?.Invoke();
                    return result;
                }
                else
                {
                    // Console.WriteLine($"Köpet misslyckades: {updateresponse.StatusCode}");
                    var errorContent = await updateresponse.Content.ReadAsStringAsync();
                    // Console.WriteLine($"Error response content: {errorContent}");

                    //Testar ny
                    Console.WriteLine("OnChange event anropas nu...");
                    Onchange?.Invoke();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel uppstod vid köp: {ex.Message}");
                
            }
            
            return new ShoppingDto(); // Returnera en tom DTO om köpet misslyckades
        }











    }
}
