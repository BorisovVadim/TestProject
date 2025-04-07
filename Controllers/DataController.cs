using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Models;
using TestProject.DTOs;

namespace TestProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DataController : ControllerBase
{
    private readonly AppDbContext _db;

    public DataController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save([FromBody] List<Dictionary<string, string>> input)
    {
        _db.Records.RemoveRange(_db.Records);

        var parsedRecords = input
           .SelectMany(recordDict => recordDict)
           .Select(keyValue => new Record 
           { 
               Code = int.Parse(keyValue.Key),
               Value = keyValue.Value
           })
           .OrderBy(record => record.Code)
           .ToList();

        await _db.Records.AddRangeAsync(parsedRecords);
        await _db.SaveChangesAsync();

        return Ok(parsedRecords);
    }

    [HttpGet("get")]
    public async Task<IActionResult> Get([FromQuery] int? code = null)
    {
        var query = _db.Records.AsQueryable();

        if (code.HasValue)
        {
            query = query.Where(record => record.Code == code.Value);
        }

        var result = await query.ToListAsync();
        return Ok(result);
    }
}