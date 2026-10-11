using LibraryInventory.Core.Entities;
using LibraryInventory.Core.Interfaces;
using LibraryInventory.Dtos.Members;
using Microsoft.AspNetCore.Mvc;

namespace LibraryInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembersController(IMemberService memberService) : ControllerBase
{
    /// <summary>GET /api/members — list all members (with pagination).</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberResponseDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var members = await memberService.GetAllAsync(page, pageSize, ct);
        return Ok(members.Select(ToDto));
    }

    private static MemberResponseDto ToDto(Member b) =>
        new(b.Id, b.MemberName, b.PhotoUrl);
}
