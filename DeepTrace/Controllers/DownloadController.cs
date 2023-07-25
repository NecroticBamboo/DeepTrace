using DeepTrace.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DeepTrace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DownloadController : Controller
    {
        private readonly IModelStorageService _modelService;

        public DownloadController(IModelStorageService modelService)
        {
            _modelService = modelService;
        }

        [HttpGet("mldata/{modelName}")]
        public async Task<FileContentResult> GetMLDataCsv([FromRoute] string modelName)
        {
            var ModelDefinition = await _modelService.Load();
            var model = ModelDefinition.FirstOrDefault(x=>x.Name==modelName) ?? throw new ApplicationException($"Model {modelName} not found");

            var csv = model.ToCsv();
            return new(Encoding.UTF8.GetBytes(csv),"text/csv")
            { 
                FileDownloadName = modelName+".csv" 
            };
        }
    }
}
