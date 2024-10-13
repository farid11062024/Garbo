using AI_Emissions_Reduction.Data;
using AI_Emissions_Reduction.Data.Entity.One_to_Many;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AI_Emissions_Reduction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderManagementController : ControllerBase
    {
        private readonly MyDBcontext _context;
        public OrderManagementController(MyDBcontext context)
        {
            _context = context;
        }
        public class WasteItem
        {
            public int WasteTypeId { get; set; }    // Zibil növü (plastik, kağız və s.)
            public decimal Weight { get; set; }     // Tullantının çəkisi (kg)
            public bool IsRecyclable { get; set; } // Geri dönüştürülə bilən olub-olmaması
        }

        // WasteType Modeli
        public class WasteType
        {
            public int Id { get; set; }              // Zibil növü ID-si
            public string Name { get; set; }         // Zibil növü adı (plastik, kağız, metal, şüşə)
            public decimal RecyclingEnergy { get; set; } // Kiloqram başına enerji (kWh/kg)
        }

        // User Modeli
        public class User
        {
            public int Id { get; set; }
            public string HomeSize { get; set; }      // Evin ölçüsü: "Small", "Medium", "Large"
            public decimal MonthlyEnergyConsumption { get; set; } // Aylıq enerji istehlakı (kWh)
            public ICollection<WasteCollectionRequest> WasteCollectionRequests { get; set; }
        }

        // WasteCollectionRequest Modeli
        public class WasteCollectionRequest
        {
            public int UserId { get; set; }           // İstifadəçi ID-si
            public int EmployeeId { get; set; }       // İşçi ID-si
            public List<WasteItem> WasteItems { get; set; }  // Zibil növləri və çəkiləri
        }

        // Payment Modeli
        public class Payment
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public decimal Amount { get; set; }    // Ödəniş məbləği
            public DateTime PaymentDate { get; set; }
            public string PaymentMethod { get; set; } // Ödəniş metodu
            public string PaymentStatus { get; set; } // Ödənişin statusu
        }

        // API Controller
        [ApiController]
        [Route("api/[controller]")]
        public class WasteCollectionController : ControllerBase
        {
            private readonly MyDBcontext _context;

            public WasteCollectionController(MyDBcontext context)
            {
                _context = context;
            }

            [HttpPost("create-waste-collection-request")]
            public async Task<IActionResult> CreateWasteCollectionRequestAsync([FromBody] WasteCollectionRequest request)
            {
                if (request == null || request.WasteItems == null || !request.WasteItems.Any())
                {
                    return BadRequest("Request or waste items cannot be null or empty.");
                }

                // 1. Tullantılardan alınan enerjini hesabla
                decimal totalEnergy = CalculateTotalEnergyFromWaste(request.WasteItems);

                // 2. İstifadəçinin enerji ehtiyacını tap
                User user = _context.Users.FirstOrDefault(u => u.Id == request.UserId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                decimal requiredEnergy = CalculateRequiredEnergy(user);

                // 3. Tullantılardan alınan enerji ilə istifadəçinin enerji ehtiyacını müqayisə et
                decimal paymentAmount = CalculatePaymentAmount(totalEnergy, requiredEnergy);

                // 4. Ödəniş məlumatını saxla
                Payment payment = new Payment
                {
                    UserId = request.UserId,
                    Amount = paymentAmount,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = "Waste Collection", // Tullantılardan ödəniş
                    PaymentStatus = "Completed"
                };
                 await _context.Payments.AllAsync(payment);
                await _context.SaveChangesAsync();

                // 5. WasteCollection məlumatını saxla
                WasteCollection wasteCollection = new WasteCollection
                {
                    UserId = request.UserId,
                    EmployeeId = request.EmployeeId,
                    CollectionDate = DateTime.Now,
                     = "Collected",
                    WasteItems = request.WasteItems // Tullantıları əlavə et
                };
                _context.WasteCollections.Add(wasteCollection);
                _context.SaveChanges();

                return Ok(new { payment.Amount, payment.PaymentStatus });
            }

            // Tullantılardan alınan enerjini hesablayan metod
            private decimal CalculateTotalEnergyFromWaste(List<WasteItem> wasteItems)
            {
                decimal totalEnergy = 0;

                foreach (var item in wasteItems)
                {
                    // Zibil növünü tapırıq
                    var wasteType = _context.WasteTypes.FirstOrDefault(w => w.Id == item.WasteTypeId);
                    if (wasteType != null && item.IsRecyclable)
                    {
                        // Enerji hesablamasını aparırıq: Energy = Weight * RecyclingEnergy
                        decimal energyFromItem = wasteType.RecyclingEnergy * item.Weight;
                        totalEnergy += energyFromItem;
                    }
                }

                return totalEnergy;
            }

            // İstifadəçinin enerji ehtiyacını hesablayan metod
            private decimal CalculateRequiredEnergy(User user)
            {
                // Evin ölçüsünə görə enerji ehtiyacını müəyyən edirik
                decimal energyFactor = user.HomeSize switch
                {
                    "Small" => 1.0m,
                    "Medium" => 1.5m,
                    "Large" => 2.0m,
                    _ => 1.0m
                };

                return user.MonthlyEnergyConsumption * energyFactor;
            }

            // Ödənişi hesablayan metod
            private decimal CalculatePaymentAmount(decimal totalEnergy, decimal requiredEnergy)
            {
                // Enerji məbləği ilə uyğun ödəniş hesablanır
                decimal energyDifference = Math.Abs(totalEnergy - requiredEnergy);
                decimal pricePerKWh = 0.1m; // KWh başına qiymət (misal üçün 0.1 AZN)

                return energyDifference * pricePerKWh;
            }
        }
}
