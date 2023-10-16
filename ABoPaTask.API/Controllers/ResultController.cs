using ABoPaTask.API.Classes;
using ABoPaTask.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ABoPaTask.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ResultController : ControllerBase
    {
        private readonly ExperimentContext _clientsDBContext;
        public ResultController(ExperimentContext clientsDBContext)
        {
            _clientsDBContext = clientsDBContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Result>>> Select()
        {
            List<Result> select = await _clientsDBContext.Results.ToListAsync();
            return Ok(select);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<Result>> GetById(int Id)
        {
            Result result = await _clientsDBContext.Results.FirstOrDefaultAsync(e => e.Id == Id);
            return Ok(result);
        }

        [HttpGet("{Token}")]
        public async Task<ActionResult<Result>> GetByToken(string token)
        {
            Result result = await _clientsDBContext.Results.FirstOrDefaultAsync(e => e.deviceToken == token);
            return Ok(result);
        }

        [HttpGet("{value}")]
        public async Task<ActionResult<List<Result>>> GetByValue(string value)
        {
            List<Result> result = await _clientsDBContext.Results.Where(e => e.value == value).ToListAsync();
            return Ok(result);
        }

        [HttpGet("{x_name}")]
        public async Task<ActionResult<List<Result>>> GetByXName(string x_name)
        {
            List<Result> result = await _clientsDBContext.Results.Where(e => e.X_Name == x_name).ToListAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Create(Result experiment)
        {
            if (experiment != null) await _clientsDBContext.Results.AddAsync(experiment);
            await _clientsDBContext.SaveChangesAsync();
            return experiment != null ? true : false;
        }
        [HttpDelete]
        public async Task<ActionResult<bool>> Delete(int Id)
        {
            Result experiment = await _clientsDBContext.Results.FirstOrDefaultAsync(e => e.Id == Id);
            if (experiment != null) _clientsDBContext.Results.Remove(experiment);
            await _clientsDBContext.SaveChangesAsync();
            return experiment != null ? true : false;
        }

        [HttpDelete]
        public async Task<ActionResult<bool>> Clear()
        {
            List<Result> list = _clientsDBContext.Results.ToList();
            foreach (var entity in list) _clientsDBContext.Results.Remove(entity);
            await _clientsDBContext.SaveChangesAsync();
            return true;
        }

        [HttpPut]
        public async Task<ActionResult<bool>> Update(Result experiment)
        {
            Result entity = await _clientsDBContext.Results.FirstOrDefaultAsync(e => e.Id == experiment.Id);
            if (entity != null) _clientsDBContext.Results.Remove(experiment);
            await _clientsDBContext.SaveChangesAsync();
            return entity != null ? true : false;
        }

        [HttpGet]
        public async Task<ActionResult<string>> PassExperiment1([FromQuery] string hash, [FromQuery] int id, [FromQuery] string button_color)
        {
            Experiment experiment = await _clientsDBContext.Experiments.FirstOrDefaultAsync(e => e.Id == id);
            Result tempResult = await GetOrCreateResult(hash, experiment);

            if (tempResult.value != null)
                return Ok(tempResult.value);

            if (button_color == "null")
                return Ok(button_color);

            var entity = GenerateExperimentResult(experiment, hash, button_color);
            entity.value = new string((char[])entity.value.Where(c => c != ' ').ToArray());
            await _clientsDBContext.AddAsync(entity);
            await _clientsDBContext.SaveChangesAsync();

            return Ok(entity.value);
        }

        [HttpGet]
        public async Task<ActionResult<string>> PassExperiment2([FromQuery] string hash, [FromQuery] int id)
        {
            Experiment experiment = await _clientsDBContext.Experiments.FirstOrDefaultAsync(e => e.Id == id);
            Result tempResult = await GetOrCreateResult(hash, experiment);

            if (tempResult.value != null)
            {
                return Ok(tempResult.value);
            }

            var entity = GenerateExperimentResult(experiment, hash, null);
            await _clientsDBContext.AddAsync(entity);
            await _clientsDBContext.SaveChangesAsync();

            return Ok(entity.value);
        }
        private async Task<Result> GetOrCreateResult(string hash, Experiment experiment)
        {
            Result tempResult = await _clientsDBContext.Results
                .FirstOrDefaultAsync(e => e.deviceToken == hash && e.X_Name == experiment.Name);

            if (tempResult == null)
            {
                tempResult = new Result { deviceToken = hash, X_Name = experiment.Name };
            }

            return tempResult;
        }
        private Result GenerateExperimentResult(Experiment experiment, string hash, string button_color)
        {
            Result entity = new Result
            {
                deviceToken = hash,
                X_Name = experiment.Name
            };

            switch (experiment.Name)
            {
                case "button_color":
                    entity.value = button_color;
                    break;
                case "price":
                    entity.value = GeneratePriceValue();
                    break;
            }

            return entity;
        }
        private string GeneratePriceValue()
        {
            Random random = new Random();
            var number = random.Next(0, 99);
            if (number < 75)
                return "10";
            else if (number >= 75 && number < 85)
                return "20";
            else if (number >= 85 && number < 90)
                return "50";
            else
                return "5";
        }
    }
}

