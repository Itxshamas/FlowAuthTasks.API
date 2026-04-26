using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FlowAuthTasks.API.Data;
using FlowAuthTasks.API.Models;
using FlowAuthTasks.API.DTOs.Task;
using FlowAuthTasks.API.Services.Interfaces;
using FlowAuthTasks.API.Helpers;

namespace FlowAuthTasks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 🔥 All endpoints require login
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _service;

        public TaskController(ITaskService service)
        {
            _service = service;
        }

        // 🔹 Create Task (Admin / Manager)
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateTask(CreateTaskDto dto)
        {
            var result = await _service.CreateTaskAsync(dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // 🔹 Assign Task (Admin / Manager)
        [HttpPost("assign")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AssignTask(AssignTaskDto dto)
        {
            var result = await _service.AssignTaskAsync(dto);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        // 🔹 Get My Tasks (User)
        [HttpGet("my")]
        [Authorize(Roles = "User,Manager,Admin")]
        public async Task<IActionResult> GetMyTasks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.GetMyTasksAsync(userId!);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}