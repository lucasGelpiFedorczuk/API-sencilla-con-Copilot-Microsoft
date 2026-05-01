using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using UserManagementAPI.Models.Dtos;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _service.GetAllAsync();
        var dto = users.Select(u => new UserReadDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            CreatedAt = u.CreatedAt
        });
        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        if (id == Guid.Empty) return BadRequest("Invalid id");
        var user = await _service.GetAsync(id);
        if (user is null) return NotFound();
        var dto = new UserReadDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserCreateDto userDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = new User
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email
        };

        User created;
        try
        {
            created = await _service.CreateAsync(user);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Email"))
        {
            return Conflict(new { message = ex.Message });
        }

        var readDto = new UserReadDto
        {
            Id = created.Id,
            FirstName = created.FirstName,
            LastName = created.LastName,
            Email = created.Email,
            CreatedAt = created.CreatedAt
        };

        return CreatedAtAction(nameof(Get), new { id = readDto.Id }, readDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto userDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (id != userDto.Id) return BadRequest();

        if (id == Guid.Empty) return BadRequest("Invalid id");

        var user = new User
        {
            Id = userDto.Id,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email
        };

        try
        {
            var ok = await _service.UpdateAsync(user);
            if (!ok) return NotFound();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Email"))
        {
            return Conflict(new { message = ex.Message });
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}