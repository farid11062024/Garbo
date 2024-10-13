using AI_Emissions_Reduction.Data;
using AI_Emissions_Reduction.Model;
using AI_Emissions_Reduction.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AI_Emissions_Reduction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkloadController : ControllerBase
    {
        private readonly MyDBcontext _context;
        public WorkloadController(MyDBcontext context)
        {
            _context = context;  
        }
        private readonly AIService _aiService;

        public WorkloadController(AIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("predict")]
        public ActionResult<bool> Predict([FromBody] WorkloadData workloadData)
        {
            var result = _aiService.PredictWorkload(workloadData.WasteVolume, workloadData.TimeTaken);
            return Ok(result);
        }
    }

}
}
