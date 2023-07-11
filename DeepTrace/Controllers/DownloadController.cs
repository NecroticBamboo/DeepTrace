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
            var modelStorage = await _modelService.Load();
            var model = modelStorage.FirstOrDefault(x=>x.Name==modelName) ?? throw new ApplicationException($"Model {modelName} not found");
            var previousIntervals = model.IntervalDefinitionList;

            var current = previousIntervals.First();
            var headers = string.Join(",", current.Data.Select((x, i) => $"Q{i + 1}min,Q{i + 1}max,Q{i + 1}avg,Q{i + 1}mean"));


            var writer = new StringBuilder();
            writer.AppendLine(headers);

            foreach (var currentInterval in previousIntervals)
            {
                var data = "";
                for (var i = 0; i < currentInterval.Data.Count; i++)
                {

                    var queryData = currentInterval.Data[i];
                    var min = queryData.Data.Min(x => x.Value);
                    var max = queryData.Data.Max(x => x.Value);
                    var avg = queryData.Data.Average(x => x.Value);
                    var mean = queryData.Data.Sum(x => x.Value) / queryData.Data.Count;

                    if (i == currentInterval.Data.Count - 1)
                    {
                        data += min + "," + max + "," + avg + "," + mean;
                    }
                    else
                    {
                        data += min + "," + max + "," + avg + "," + mean + ",";
                    }

                }
                writer.AppendLine(data);
            }
            return new(Encoding.UTF8.GetBytes(writer.ToString()),"text/csv")
            { 
                FileDownloadName = modelName+".csv" 
            };
        }
    }
}
