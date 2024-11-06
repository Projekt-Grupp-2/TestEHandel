using System.ComponentModel.DataAnnotations;

namespace TestEHandel.Models
{
    public class ProduktDTO
    {

       
        public Guid ProductId { get; set; } // Primary Key

        public string? ProductTitle { get; set; }
        public string? Description { get; set; }




    }



}

