using Kt7Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Kt7Api.Controllers;

[ApiController]
[Route("resources")]
public class ResourcesController(AppStore store) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        lock (store.SyncRoot)
        {
            return Ok(store.Resources.ToArray());
        }
    }

    [HttpGet("available")]
    public IActionResult GetAvailable([FromQuery] DateTimeOffset? start, [FromQuery] DateTimeOffset? end, [FromQuery] string? type)
    {
        if (!start.HasValue || !end.HasValue || start >= end)
        {
            return BadRequest(new { message = "Укажите период, окончание должно быть позже начала" });
        }
        lock (store.SyncRoot)
        {
            if (type is not null && !store.Resources.Any(item => item.Type == type))
            {
                return BadRequest(new { message = "Неизвестный тип ресурса" });
            }
            var available = store.Resources.Where(item => (type is null || item.Type == type)
                && !store.HasConflict(item.Id, start.Value, end.Value)).ToArray();
            return Ok(available);
        }
    }
}
