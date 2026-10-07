using Kt7Api.Data;
using Kt7Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kt7Api.Controllers;

[ApiController]
[Route("students")]
public class StudentsController(AppStore store) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        lock (store.SyncRoot)
        {
            return Ok(store.Students.ToArray());
        }
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        lock (store.SyncRoot)
        {
            var student = store.Students.FirstOrDefault(item => item.Id == id);
            return student is null ? NotFound(new { message = "Студент не найден" }) : Ok(student);
        }
    }

    [HttpPost]
    public IActionResult Create(StudentRequest request)
    {
        lock (store.SyncRoot)
        {
            var student = new Student { Id = store.NextStudentId++, Name = request.Name.Trim(), Age = request.Age!.Value };
            store.Students.Add(student);
            return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, StudentRequest request)
    {
        lock (store.SyncRoot)
        {
            var index = store.Students.FindIndex(item => item.Id == id);
            if (index == -1)
            {
                return NotFound(new { message = "Студент не найден" });
            }
            var student = new Student { Id = id, Name = request.Name.Trim(), Age = request.Age!.Value };
            store.Students[index] = student;
            return Ok(student);
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        lock (store.SyncRoot)
        {
            var removed = store.Students.RemoveAll(item => item.Id == id);
            return removed == 0 ? NotFound(new { message = "Студент не найден" }) : NoContent();
        }
    }
}
