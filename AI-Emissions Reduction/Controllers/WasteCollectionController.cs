using AI_Emissions_Reduction.Data;
using AI_Emissions_Reduction.Data.Entity.One_to_Many;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AI_Emissions_Reduction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WasteCollectionController : ControllerBase
    {
        private readonly MyDBcontext _context;

        public WasteCollectionController(MyDBcontext context)
        {
            _context = context;
        }

        [HttpGet("AllOrders")]
        public async Task<ActionResult<IEnumerable<WasteCollection>>> GetWasteCollections()
        {
            return await _context.WasteCollections.Include(w => w.user)
                                                  .Include(w => w.WasteType)
                                                  .Include(w => w.employee)
                                                  .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WasteCollection>> GetWasteCollection(int id)
        {
            var wasteCollection = await _context.WasteCollections.Include(w => w.user)
                                                                  .Include(w => w.WasteType)
                                                                  .Include(w => w.employee)
                                                                  .FirstOrDefaultAsync(w => w.Id == id);

            if (wasteCollection == null)
            {
                return NotFound();
            }

            return wasteCollection;
        }

        [HttpPost("NewOrder")]
        public async Task<ActionResult<WasteCollection>> PostWasteCollection(WasteCollection wasteCollection)
        {
            _context.WasteCollections.Add(wasteCollection);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWasteCollection", new { id = wasteCollection.Id }, wasteCollection);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutWasteCollection(int id, WasteCollection wasteCollection)
        {
            if (id != wasteCollection.Id)
            {
                return BadRequest();
            }

            _context.Entry(wasteCollection).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWasteCollection(int id)
        {
            var wasteCollection = await _context.WasteCollections.FindAsync(id);
            if (wasteCollection == null)
            {
                return NotFound();
            }

            _context.WasteCollections.Remove(wasteCollection);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("collectWaste")]
        public async Task<IActionResult> CollectWaste([FromBody] WasteCollectionRequest request)
        {
            // 1. İstifadəçi mövcudluğu yoxlanılır
            var user = await _context.Users
                .Include(u => u.WasteCollectionRequests)
                .FirstOrDefaultAsync(u => u.Id == request.UserId);

            if (user == null)
                return NotFound("User not found.");

            // 2. Zibil yığımı üçün lazım olan məlumatlar
            var wasteCollection = new WasteCollection
            {
                UserId = request.UserId,
                EmployeeId = request.EmployeeId,
                CollectionDate = DateTime.UtcNow,
                Status = "Collected"
            };

            // 3. Hər bir tullantının növü üzrə point hesablanması
            decimal totalWeight = 0;
            int totalPoints = 0;
            decimal totalEnergy = 0;

            foreach (var item in request.WasteItems)
            {
                var wasteType = await _context.WasteTypes.FindAsync(item.WasteTypeId);
                if (wasteType == null)
                    return BadRequest("Invalid waste type.");

                // Zibilin çəkisi 0.5 kg-dan azsa, 1 point silinsin
                if (item.Weight < 0.5m)
                {
                    totalPoints -= 1;
                }
                else
                {
                    // Geri dönüştürülə bilən tullantılar üçün əlavə point
                    if (item.IsRecyclable)
                    {
                        totalPoints += 1;
                    }

                    // Zibilin ümumi çəkisini toplamaq
                    totalWeight += item.Weight;

                    // Geri dönüştürülə bilən tullantılardan enerji hesablanması
                    if (item.IsRecyclable)
                    {
                        totalEnergy += wasteType.RecyclingEnergy * item.Weight;
                    }
                }
            }

            // 4. Enerji hesablanması: Geri dönüştürülə bilən tullantılardan əldə edilən enerji
            decimal requiredEnergy = CalculateRequiredEnergy(user);

            // 5. Ödəniş hesablanması (enerji və pointlərə əsasən)
            decimal paymentAmount = CalculatePaymentAmount(totalEnergy, requiredEnergy);

            // 6. Zibilin toplanmasını tamamlayırıq
            _context.WasteCollections.Add(wasteCollection);
            await _context.SaveChangesAsync();

            // 7. Pointlərə əsasən istifadəçi balansını yeniləyirik
            user.Points += totalPoints;

            // 8. Ödənişi yarat
            var payment = new Payment
            {
                UserId = user.Id,
                Amount = paymentAmount,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "Waste Collection",
                PaymentStatus = "Completed"
            };
            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Waste collected successfully.",
                totalPoints,
                totalEnergy,
                paymentAmount
            });
        }

        // Enerji hesablanması üçün sadə formula
        private decimal CalculateRequiredEnergy(User user)
        {
            // Burada istifadəçinin evinə lazım olan ümumi enerji (işıq, qaz) hesablanacaq.
            // Misal üçün:
            return 200; // Bu sadəcə bir nümunədir, burada konkret məlumat bazası olacaq
        }

        // Ödəniş məbləğinin hesablanması
        private decimal CalculatePaymentAmount(decimal totalEnergy, decimal requiredEnergy)
        {
            // Bu sadə bir hesablamadır, real dünyada daha kompleks ola bilər
            decimal energyFactor = 0.1m; // Enerjinin qiyməti
            decimal energyCost = totalEnergy * energyFactor;

            // Müştəriyə aid olan enerji ehtiyacından fərqi çıxaraq ödənişi təyin edirik
            return Math.Max(0, energyCost - requiredEnergy);
        }
    }

    public class WasteCollectionRequest
    {
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        public List<WasteItem> WasteItems { get; set; }
    }

    public class WasteItem
    {
        public int WasteTypeId { get; set; }
        public decimal Weight { get; set; }
        public bool IsRecyclable { get; set; }
    }
}
