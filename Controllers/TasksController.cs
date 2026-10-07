using Kt7Api.Data;
using Kt7Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kt7Api.Controllers;

[ApiController]
[Route("tasks")]
public class TasksController(AppStore store) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll([FromQuery] string? status)
    {
        if (status is not null && status != "new" && status != "in-progress" && status != "completed")
        {
            return BadRequest(new { message = "Допустимые статусы: new, in-progress, completed" });
        }
        lock (store.SyncRoot)
        {
            return Ok(store.Tasks.Where(item => status is null || item.Status == status).ToArray());
        }
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        lock (store.SyncRoot)
        {
            var task = store.Tasks.FirstOrDefault(item => item.Id == id);
            return task is null ? NotFound(new { message = "Задача не найдена" }) : Ok(task);
        }
    }

    [HttpPost]
    public IActionResult Create(TaskRequest request)
    {
        lock (store.SyncRoot)
        {
            var task = new TaskItem
            {
                Id = store.NextTaskId++, Title = request.Title.Trim(), Description = request.Description.Trim(), Status = "new"
            };
            store.Tasks.Add(task);
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }
    }

    [HttpPut("{id:int}/status")]
    public IActionResult UpdateStatus(int id, TaskStatusRequest request)
    {
        lock (store.SyncRoot)
        {
            var index = store.Tasks.FindIndex(item => item.Id == id);
            if (index == -1)
            {
                return NotFound(new { message = "Задача не найдена" });
            }
            var oldTask = store.Tasks[index];
            var task = new TaskItem { Id = id, Title = oldTask.Title, Description = oldTask.Description, Status = request.Status };
            store.Tasks[index] = task;
            return Ok(task);
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        lock (store.SyncRoot)
        {
            var removed = store.Tasks.RemoveAll(item => item.Id == id);
            return removed == 0 ? NotFound(new { message = "Задача не найдена" }) : NoContent();
        }
    }
}
