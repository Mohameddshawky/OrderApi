using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    [HttpPost]
    public IActionResult Create(CreateCategoryDto createCategoryDto)
    {
        return Ok(createCategoryDto);
    }
}
