using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using TestEHandel.Models;

namespace TestEHandel.Service
{
    public class CartService(HttpClient httpClient, IConfiguration configuration)
    {


        private readonly HttpClient _httpClient = httpClient;
        private IConfiguration _configuration = configuration;
        private readonly AuthenticationStateProvider _authProvider;
        public event Action OnChange;







        public async Task<ShoppingDto> GetCart(Guid id)
        {




            try
            {
                // Anropa API:t för att hämta kundvagnen baserat på id (UserId)
                var response = await _httpClient.GetFromJsonAsync<ShoppingDto>($"https://localhost:7197/api/Shopping/GetUserId/{id}");

                // Returnera svaret eller en ny DTO om inget hittas

                //  return response ?? new ShoppingDto { UserId = id };

                var shoppingCart = response ?? new ShoppingDto { UserId = id };

                // Utlös OnChange om shoppingCart har data
                if (shoppingCart.Items.Count > 0)
                {
                    OnChange?.Invoke(); // Meddela om kundvagnen har innehåll
                }
                OnChange?.Invoke();
                return shoppingCart;


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel inträffade: {ex.Message}");

                return new ShoppingDto { UserId = id }; // Returnera en tom kundvagn som fallback
            }




        }



        //TEsting DecrementCart


        //Funkar ej



        public async Task<ShoppingDto> DecrementCart(Guid productId, Guid userId)
        {
            // Hämta befintlig kundvagn för användaren
            var existingCartResponse = await _httpClient.GetAsync($"https://localhost:7197/api/Shopping/GetUserId/{userId}");   
            if (!existingCartResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Kundvagn hittades inte för användar-ID: {userId}");
                return null;  // Returnera null om ingen kundvagn finns
            }

            var existingCartContent = await existingCartResponse.Content.ReadAsStringAsync();
            var shoppingDto = JsonConvert.DeserializeObject<ShoppingDto>(existingCartContent);

            // Hitta produkten i kundvagnen
            var existingItem = shoppingDto.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                if (existingItem.Quantity == 1)
                {
                    shoppingDto.Items.Remove(existingItem);
                }
                else
                {
                    existingItem.Quantity -= 1;
                }
            }
            else
            {
                Console.WriteLine("Produkten hittades inte i kundvagnen.");
                return shoppingDto;
            }

            // Uppdatera kundvagnen på servern om det har skett en ändring
            var content = JsonConvert.SerializeObject(shoppingDto);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"https://localhost:7197/api/Shopping/Update/{userId}", bodyContent);
            OnChange?.Invoke();

            if (response.IsSuccessStatusCode)
            {
                OnChange?.Invoke();
                return shoppingDto;
            }
            else
            {
                Console.WriteLine($"Misslyckades med att uppdatera kundvagnen: {response.StatusCode}"); 
OnChange?.Invoke();
                return shoppingDto; ;
            }
        }












        /*  public async Task<ShoppingDto> DecrementCart(Guid userId, Guid productId)

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
                  // Om kvantiteten är 1, ta bort produkten från kundvagnen
                  if (existingItem.Quantity == 1)
                  {
                      shoppingDto.Items.Remove(existingItem);
                  }
                  else
                  {
                      // Annars minska kvantiteten med 1
                      existingItem.Quantity -= 1;
                  }
              }

      }




              // Uppdatera kundvagnen på servern
              var content = JsonConvert.SerializeObject(shoppingDto);
              var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
              var response = await _httpClient.PostAsync($"https://localhost:7197/api/Shopping/Update/{userId}", bodyContent);

              if (response.IsSuccessStatusCode)
              {
                  return shoppingDto; // Returnera den uppdaterade kundvagnen
              }
              else
              {
                  Console.WriteLine($"Misslyckades med att uppdatera kundvagnen: {response.StatusCode}");
                  return null;
              }
          }




  */




















    }
}
