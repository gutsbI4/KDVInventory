using System.Text.Json.Serialization;

namespace api.Models.DTO
{
    public class EnergyValueDTO
    {
        [JsonConstructor]
        public EnergyValueDTO()
        {
            
        }
        public EnergyValueDTO(EnergyValue energyValue)
        {
            ProductId = energyValue.ProductId;
            Proteins = energyValue.Proteins;
            Fats = energyValue.Fats;
            Carbs = energyValue.Carbs;
            Calories = energyValue.Calories;
        }
        public int ProductId { get; set; }

        public double? Proteins { get; set; }

        public double? Fats { get; set; }

        public double? Carbs { get; set; }

        public double? Calories { get; set; }
    }
}
